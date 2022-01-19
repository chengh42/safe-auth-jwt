module SafeAuthJwt.Server.Secured

open Fable.Remoting.Giraffe
open Fable.Remoting.Server
open Microsoft.AspNetCore.Http

open SafeAuthJwt.Shared.API
open SafeAuthJwt.Shared.Errors
open Anonymous

let private getUserById (id:System.Guid) =
    let bobId = System.Guid.Parse "2b88b8c1-a76c-444c-84ff-d74bde45bce8"
    async {
        match id with
        | id when id = bobId -> return Some UserFromDb.Bob
        | _ -> return None
    }

let private userToResponse (user:UserFromDb) : Response.UserInfo =
    {
        Name = user.Name
        Email = user.Email
    }

let private getUserInfo userId () =
    task {
        let! maybeUser = userId |> getUserById
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