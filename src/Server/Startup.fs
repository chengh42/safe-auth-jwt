module Startup

open System
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Giraffe

open JWT

type Startup(cfg:IConfiguration, evn:IWebHostEnvironment) =
    // read values from config or ENV vars
    let cfg =
        {
            Audience = cfg.["JwtAudience"]
            Issuer = cfg.["JwtIssuer"]
            Secret = cfg.["JwtSecret"]
            AccessTokenLifetime = TimeSpan.FromMinutes 10.
        }

    member _.ConfigureServices (services:IServiceCollection) =
        services
            .AddAuthorization(fun auth ->
                auth.DefaultPolicy <-
                    Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build()
            )
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(Action<JwtBearerOptions>(fun opts ->
                    opts.TokenValidationParameters <- JWT.getParameters cfg
                )
            )
            |> ignore
        services.AddGiraffe() |> ignore
    member _.Configure(app:IApplicationBuilder) =
        app
            .UseStaticFiles()
            .UseAuthentication()
            .UseGiraffe (WebApp.webApp cfg)
