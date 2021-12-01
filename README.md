# SAFE Authentication with JWT

This web app demos authentication using Json Web Token following [the tutorial by @Dzoukr](https://github.com/Zaid-Ajaj/Fable.Remoting/blob/master/documentation/src/full-auth-example.md). It is built with the [SAFE Stack](https://safe-stack.github.io/).

## Install pre-requisites

You'll need to install the following pre-requisites in order to build SAFE applications

* [.NET Core SDK](https://www.microsoft.com/net/download) 5.0 or higher
* [Node LTS](https://nodejs.org/en/download/)

## Starting the application

Before you run the project **for the first time only** you must install dotnet "local tools" with this command:

```bash
dotnet tool restore
```

To concurrently run the server and the client components in watch mode use the following command:

```bash
dotnet run
```

Then open `http://localhost:8080` in your browser.
