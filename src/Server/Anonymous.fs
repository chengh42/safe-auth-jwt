module Anonymous

open System.Security.Claims
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open FSharp.Control.Tasks

open Shared
open JWT

type UserFromDb =
    {
        Id : System.Guid
        Name : string
        Email : string
        PwdHash : string
    }
    with

    // for now using hardcoded users
    static member UserBob =
        {
            Id = System.Guid.NewGuid()
            Name = "Bob"
            Email = "bob@fsharp.net"
            PwdHash = "Str0ngP@zzword4Bob" |> Password.createHash
        }
    static member UserAlice =
        {
            Id = System.Guid.NewGuid()
            Name = "Alice"
            Email = "alice@fsharp.net"
            PwdHash = "Str0ngP@zzword4Alice" |> Password.createHash
        }

let private getUserByEmail (email:string) : Async<UserFromDb option> =
    async {
        match email with
        | "bob@fsharp.net" -> return Some UserFromDb.UserBob
        | "alice@fsharp.net" -> return Some UserFromDb.UserAlice
        | _ -> return None
    }

let private userToToken (cfg:JwtConfiguration) (user:UserFromDb) : Token =
    [ Claim("id", user.Id.ToString()) ]
    |> List.toSeq
    |> JWT.createToken cfg

let private tokenToResponse (t:Token) : Response.JwtToken =
    { Token = t.Token }

let private login (cfg:JwtConfiguration) (req:Request.Login) =
    task {
        let! maybeUser = req.Email |> getUserByEmail // implement such function
        return
            maybeUser
            |> Option.bind (fun x ->
                if Password.verifyPassword x.PwdHash req.Password then Some x else None
            )
            |> Option.map (userToToken cfg >> tokenToResponse)
            |> ServerError.ofOption (Authentication "Bad login or password")
    }

let private getAnonymousService (cfg:JwtConfiguration) =
    {
        Login = login cfg >> Async.AwaitTask
    }

let anonymousAPI (cfg:JwtConfiguration) =
    Remoting.createApi()
    |> Remoting.withRouteBuilder AnonymousAPI.RouteBuilder
    |> Remoting.fromValue (getAnonymousService cfg)
    |> Remoting.withErrorHandler Remoting.errorHandler // see? we use our error handler here!
    |> Remoting.buildHttpHandler
