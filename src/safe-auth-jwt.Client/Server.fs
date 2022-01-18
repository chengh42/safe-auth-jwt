module safe_auth_jwt.Client.Server

open Fable.Remoting.Client
open safe_auth_jwt.Shared.API

let service =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Service.RouteBuilder
    |> Remoting.buildProxy<Service>