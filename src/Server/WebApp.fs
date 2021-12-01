module WebApp

open Giraffe
open Microsoft.AspNetCore.Authentication.JwtBearer

open JWT

let private mustBeLoggedIn : HttpHandler =
    requiresAuthentication (RequestErrors.UNAUTHORIZED JwtBearerDefaults.AuthenticationScheme "" "User not logged in")

let webApp (cfg:JwtConfiguration) : HttpHandler =
    choose [
        Anonymous.anonymousAPI cfg
        mustBeLoggedIn >=> choose [
            Secured.securedAPI
        ]
        htmlFile "public/index.html"
    ]
