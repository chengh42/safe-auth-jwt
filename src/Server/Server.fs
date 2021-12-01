module Server

open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Hosting

Host.CreateDefaultBuilder()
    .ConfigureWebHostDefaults(
        fun webHostBuilder ->
            webHostBuilder
                .UseStartup(typeof<Startup.Startup>)
                .UseUrls([|"http://0.0.0.0:8085"|])
                .UseWebRoot("public")
                |> ignore)
    .Build()
    .Run()