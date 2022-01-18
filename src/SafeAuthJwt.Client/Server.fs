module SafeAuthJwt.Client.Server

open Fable.Remoting.Client
open SafeAuthJwt.Shared.API

let service =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Service.RouteBuilder
    |> Remoting.buildProxy<Service>