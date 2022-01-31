module SafeAuthJwt.Server.Secured

open Fable.Remoting.Giraffe
open Fable.Remoting.Server
open Microsoft.AspNetCore.Http

open SafeAuthJwt.Shared.API
open SafeAuthJwt.Shared.Errors

let private userToResponse (user: Storage.DbUser) : Response.UserInfo =
    {
        Name = user.Name
        Email = user.Email
    }

let private getUserInfo userId () =
    task {
        let maybeUser = userId |> Storage.getUserById
        return
            maybeUser
            |> Option.map userToResponse
            |> ServerError.ofOption (Authentication "User account not found")
    }

let private getSecuredService (ctx:HttpContext) =
    let userId = ctx.User.Claims |> Seq.find (fun x -> x.Type = "id") |> (fun x -> System.Guid x.Value)
    {
        GetUserInfo = getUserInfo userId >> Async.AwaitTask
        GetPrivateMessage = fun () -> async { return "This is a protected message!" }
    }

let securedAPI : Giraffe.Core.HttpHandler =
    Remoting.createApi()
    |> Remoting.withRouteBuilder SecuredAPI.RouteBuilder
    |> Remoting.fromContext getSecuredService // <-- we need context here
    |> Remoting.withErrorHandler SafeAuthJwt.Server.Remoting.Remoting.errorHandler // see? we use our error handler here!
    |> Remoting.buildHttpHandler