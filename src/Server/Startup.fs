module Startup

open System
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Giraffe

type Startup(cfg:IConfiguration, env:IWebHostEnvironment) =
    // read values from config or ENV vars
    let cfgJwt : Jwt.JwtConfiguration = {
        Audience = cfg.["JwtAudience"]
        Issuer = cfg.["JwtIssuer"]
        Secret = cfg.["JwtSecret"]
        AccessTokenLifetime = TimeSpan.FromMinutes 10.
    }

    member __.ConfigureServices (services:IServiceCollection) =
        let authPolicy =
            Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build()
        services
            .AddAuthorization(fun auth ->
                auth.DefaultPolicy <- authPolicy
            )
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(Action<JwtBearerOptions>(fun opts ->
                    opts.TokenValidationParameters <- Jwt.getParameters cfgJwt
                )
            )
            |> ignore

        services
            .AddApplicationInsightsTelemetry(cfg.["APPINSIGHTS_INSTRUMENTATIONKEY"])
            .AddGiraffe() |> ignore

    member __.Configure(app:IApplicationBuilder) =
        app
            .UseStaticFiles()
            .UseAuthentication()
            .UseGiraffe (WebApp.webApp cfgJwt)