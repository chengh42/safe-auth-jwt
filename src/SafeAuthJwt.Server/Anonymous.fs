module SafeAuthJwt.Server.Anonymous

open System.Security.Claims
open Fable.Remoting.Giraffe
open Fable.Remoting.Server

open SafeAuthJwt.Shared.API
open SafeAuthJwt.Shared.Errors

// let private register (req: Request.Register) =
//     task {
//         let newUserId = Storage.registerNewUser req
//         return newUserId
//     }

let private login (cfg: Jwt.JwtConfiguration) (req: Request.Login) =
    let userToToken (cfg:Jwt.JwtConfiguration) (user:Storage.DbUser) : Jwt.Token =
        [ Claim("id", user.Id.ToString()) ]
        |> List.toSeq
        |> Jwt.createToken cfg

    let tokenToResponse (t:Jwt.Token) : Response.JwtToken =
        { Token = t.Token }

    task {
        let maybeUser = req.Email |> Storage.getUserByEmail
        return
            maybeUser
            |> Option.bind (fun x ->
                if Password.verifyPassword x.PwdHash req.Password then Some x else None
            )
            |> Option.map (userToToken cfg >> tokenToResponse)
            |> ServerError.ofOption (Authentication "Bad login or password")
    }

let private getAnonymousService (cfg:Jwt.JwtConfiguration) =
    {
        Login = login cfg >> Async.AwaitTask
        GetPublicMessage = fun () -> async { return "This is a public message!" }
    }

let anonymousAPI (cfg:Jwt.JwtConfiguration) =
    Remoting.createApi()
    |> Remoting.withRouteBuilder AnonymousAPI.RouteBuilder
    |> Remoting.fromValue (getAnonymousService cfg)
    |> Remoting.withErrorHandler SafeAuthJwt.Server.Remoting.Remoting.errorHandler // see? we use our error handler here!
    |> Remoting.buildHttpHandler