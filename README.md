# SAFE Authentication with JWT Bearer token

This web app demos authentication using Json Web Token (JWT) following [the tutorial by @Dzoukr](https://github.com/Zaid-Ajaj/Fable.Remoting/blob/master/documentation/src/full-auth-example.md). It is built with the [SAFEr Template](https://github.com/Dzoukr/SAFEr.Template). Some notes taken from the tutorial:

> * Exceptions are used for errors, but with well-defined union type inside
> * Remoting uses custom error handler to wrap such errors and set 4xx status code for HTTP response
> * API definition does not use `Result` type directly
> * Registration neither token refresh is not part of this example, but can be easily added
> * Database for users and its functions are not implemented, only used "to implement" functions
> * Authentication is hard - don't do it manually. Use some existing service like Auth0, Azure AD, Identity Server or so.

## Prerequisites

* [.NET Core SDK](https://www.microsoft.com/net/download) 6.0 or higher
* [Node LTS](https://nodejs.org/en/download/)
* [yarn](https://classic.yarnpkg.com/en/)

## How to run

### 1. Restore .NET local tools

Before you run the project **for the first time only** you must install dotnet "local tools" with this command:

```bash
dotnet tool restore
```

### 2. Define claims

Navigate to `src/SafeAuthJwt.Server/Startup.fs` and define claims. Below is a simplistic example:

```fsharp
type Startup(cfg:IConfiguration, evn:IWebHostEnvironment) =
    // read values from config or ENV vars
    let cfg =
        {
            Audience = "a non-null string"
            Issuer = "a non-null string"
            Secret = "q4t7w!z%C*F-JaNdRgUkXn2r5u8x/A?D" // Note that secret has to be 128-bit min
            AccessTokenLifetime = TimeSpan.FromMinutes 10.
        }

    (...)
```

### 3. Run the app

To concurrently run the server and the client components in watch mode use the following command:

```bash
dotnet run
```

Then open `http://localhost:8080` in your browser.

Login credentials:
* Email: `bob@fsharp.net`
* Password: `Str0ngP@zzword4Bob`