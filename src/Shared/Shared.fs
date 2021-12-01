namespace Shared

open System

type ServerError =
    | Exception of string
    | Authentication of string
    // add here any other you want to use

exception ServerException of ServerError

module ServerError =
    let failwith (er:ServerError) = raise (ServerException er)

    let ofOption err (v:Option<_>) =
        v
        |> Option.defaultWith (fun _ -> err |> failwith)

[<RequireQualifiedAccess>]
module Request =
    type Login =
        {
            Email : string
            Password : string
        }

[<RequireQualifiedAccess>]
module Response =
    type JwtToken = {
        Token : string
    }

    type UserInfo = {
        Name : string
        Email : string
    }

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type AnonymousAPI = {
    Login : Request.Login -> Async<Response.JwtToken> // note no Result here!
}
with
    static member RouteBuilder _ m = sprintf "/api/anonymous/%s" m

type SecuredAPI = {
    GetUserInfo : unit -> Async<Response.UserInfo> // note no Result here!
}
with
    static member RouteBuilder _ m = sprintf "/api/secured/%s" m
