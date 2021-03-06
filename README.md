# SAFE Authentication with JWT Bearer token

This web app demos authentication using Json Web Token (JWT) following [the tutorial by @Dzoukr](https://github.com/Zaid-Ajaj/Fable.Remoting/blob/master/documentation/src/full-auth-example.md). It is built with the [SAFEr Template](https://github.com/Dzoukr/SAFEr.Template).

<img src="https://github.com/chengh42/safe-auth-jwt/raw/main/safe-auth-jwt.gif" data-canonical-src="https://github.com/chengh42/safe-auth-jwt/raw/main/safe-auth-jwt.gif" height="300" />

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

### 2. Define JWT configuration

Go to `src/Server/appsettings.Development.json` and use your own JWT configuration.

### 3. Run the app

To concurrently run the server and the client components in watch mode use the following command:

```bash
dotnet run
```

Then open `http://localhost:8080` in your browser.

Login credentials:
* Email: `bob@fsharp.net`
* Password: `Str0ngP@zzword4Bob`

## Notes

Some notes taken from the tutorial:

> * Exceptions are used for errors, but with well-defined union type inside
> * Remoting uses custom error handler to wrap such errors and set 4xx status code for HTTP response
> * API definition does not use `Result` type directly
> * Registration neither token refresh is not part of this example, but can be easily added
> * Database for users and its functions are not implemented, only used "to implement" functions
> * Authentication is hard - don't do it manually. Use some existing service like Auth0, Azure AD, Identity Server or so.

### Docker

```
docker build -t safe-auth-jwt .
docker run -it -p 5000:5000 safe-auth-jwt
```
