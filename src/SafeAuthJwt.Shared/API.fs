module SafeAuthJwt.Shared.API

[<RequireQualifiedAccess>]
module Request =
    type Login = {
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

// type Service = {
//     GetMessage : unit -> Async<string>
// }
// with
//     static member RouteBuilder _ m = sprintf "/api/service/%s" m