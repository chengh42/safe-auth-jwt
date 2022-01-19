module SafeAuthJwt.Server.WebApp

open Giraffe
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Microsoft.AspNetCore.Authentication.JwtBearer
open SafeAuthJwt.Shared.API

let private mustBeLoggedIn : HttpHandler =
    requiresAuthentication (RequestErrors.UNAUTHORIZED JwtBearerDefaults.AuthenticationScheme "" "User not logged in")

// let service = {
//     GetMessage = fun _ -> task { return "Hi from Server!" } |> Async.AwaitTask
// }

let webApp (cfg:Jwt.JwtConfiguration): HttpHandler =
    // let remoting =
    //     Remoting.createApi()
    //     |> Remoting.withRouteBuilder Service.RouteBuilder
    //     |> Remoting.fromValue service
    //     |> Remoting.buildHttpHandler

    // choose [
    //     remoting
    //     htmlFile "public/index.html"
    // ]
    choose [
        Anonymous.anonymousAPI cfg
        mustBeLoggedIn >=> choose [
            Secured.securedAPI
        ]
        htmlFile "public/index.html"
    ]