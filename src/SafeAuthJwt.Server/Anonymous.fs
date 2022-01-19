module SafeAuthJwt.Server.Anonymous

open System.Security.Claims
open Fable.Remoting.Giraffe
open Fable.Remoting.Server

open SafeAuthJwt.Shared.API
open SafeAuthJwt.Shared.Errors

type UserFromDb = {
    Id : System.Guid
    Name : string
    Email : string
    PwdHash : string
}
with
    static member Bob =
        { Email = "bob@fsharp.net"
          Id = System.Guid.Parse "2b88b8c1-a76c-444c-84ff-d74bde45bce8"
          Name = "Bob"
          PwdHash = "Str0ngP@zzword4Bob" |> Password.createHash }

let private getUserByEmail (email:string) =
    async {
        match email with
        | "bob@fsharp.net" ->
            return Some UserFromDb.Bob
        | _ ->
            return None
    }

let private userToToken (cfg:Jwt.JwtConfiguration) (user:UserFromDb) : Jwt.Token =
    [ Claim("id", user.Id.ToString()) ]
    |> List.toSeq
    |> Jwt.createToken cfg

let private tokenToResponse (t:Jwt.Token) : Response.JwtToken =
    { Token = t.Token }

let private login (cfg: Jwt.JwtConfiguration) (req:Request.Login) =
    task {
        let! maybeUser = req.Email |> getUserByEmail
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
    }

let anonymousAPI (cfg:Jwt.JwtConfiguration) =
    Remoting.createApi()
    |> Remoting.withRouteBuilder AnonymousAPI.RouteBuilder
    |> Remoting.fromValue (getAnonymousService cfg)
    |> Remoting.withErrorHandler SafeAuthJwt.Server.Remoting.Remoting.errorHandler // see? we use our error handler here!
    |> Remoting.buildHttpHandler