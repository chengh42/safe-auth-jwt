module safe_auth_jwt.Shared.API

type Service = {
    GetMessage : unit -> Async<string>
}
with
    static member RouteBuilder _ m = sprintf "/api/service/%s" m