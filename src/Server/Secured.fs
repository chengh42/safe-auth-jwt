module Secured

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http

open Shared
open Anonymous

let private getUserById (guid:System.Guid) : Async<UserFromDb option> =
    let guidBob = UserFromDb.UserBob.Id
    let guidAlice = UserFromDb.UserAlice.Id
    async {
        match guid with
        | i when i = guidBob -> return Some UserFromDb.UserBob
        | i when i = guidAlice -> return Some UserFromDb.UserAlice
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
    }

let securedAPI : Giraffe.Core.HttpFunc -> HttpContext -> Giraffe.Core.HttpFuncResult =
    Remoting.createApi()
    |> Remoting.withRouteBuilder SecuredAPI.RouteBuilder
    |> Remoting.fromContext getSecuredService // <-- we need context here
    |> Remoting.withErrorHandler Remoting.errorHandler // see? we use our error handler here!
    |> Remoting.buildHttpHandler
