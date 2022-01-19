module SafeAuthJwt.Server.Remoting

open System
open Fable.Remoting.Server
open Microsoft.AspNetCore.Http
open SafeAuthJwt.Shared.Errors

[<RequireQualifiedAccess>]
module Remoting =
    let private statusCode = function
        | Exception _ -> 500
        | Authentication _ -> 401
        | _ -> 400

    let rec errorHandler (ex: Exception) (routeInfo: RouteInfo<HttpContext>) =
        match ex with
        | ServerException err ->
            routeInfo.httpContext.Response.StatusCode <- err |> statusCode
            Propagate err
        | e when e.InnerException |> isNull |> not -> errorHandler e.InnerException routeInfo
        | _ -> Propagate (ServerError.Exception(ex.Message))