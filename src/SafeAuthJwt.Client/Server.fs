module SafeAuthJwt.Client.Server

open Fable.Remoting.Client
open Fable.SimpleJson
open SafeAuthJwt.Shared.API
open SafeAuthJwt.Shared.Errors

let exnToError (e:exn) : ServerError =
    match e with
    | :? ProxyRequestException as ex ->
        try
            let serverError = Json.parseAs<{| error: ServerError |}>(ex.Response.ResponseBody)
            serverError.error
        with _ ->
            if ex.StatusCode = 401 then ex.Response.ResponseBody |> ServerError.Authentication
            else (ServerError.Exception(e.Message))
    | _ -> (ServerError.Exception(e.Message))

let anonymousAPI =
    Remoting.createApi()
    |> Remoting.withRouteBuilder AnonymousAPI.RouteBuilder
    |> Remoting.buildProxy<AnonymousAPI>

let onSecuredAPI (fn:SecuredAPI -> Async<'a>) =
    let token = Browser.WebStorage.localStorage.getItem "token"
    Remoting.createApi()
    |> Remoting.withRouteBuilder SecuredAPI.RouteBuilder
    |> Remoting.withAuthorizationHeader (sprintf "Bearer %s" token)
    |> Remoting.buildProxy<SecuredAPI>
    |> fn

// let service =
//     Remoting.createApi()
//     |> Remoting.withRouteBuilder Service.RouteBuilder
//     |> Remoting.buildProxy<Service>