module SafeAuthJwt.Client.Pages.Login

open Feliz
open Feliz.DaisyUI
open Feliz.UseDeferred
open SafeAuthJwt.Shared.Errors
open SafeAuthJwt.Client

let displayStronglyTypedError = function
    // choose how to display errors based on your needs
    | ServerError.Exception x -> Html.text x
    | ServerError.Authentication x -> Html.div x

[<ReactComponent>]
let LoginView () =
    let req : SafeAuthJwt.Shared.API.Request.Login = { Email = ""; Password = "" }
    let loginForm,setLoginForm = React.useState(req)
    let loginReq, setLoginReq = React.useState(Deferred.HasNotStartedYet)
    let login = React.useDeferredCallback ((fun _ -> Server.anonymousAPI.Login loginForm), setLoginReq)

    let result =
        match loginReq with
        | Deferred.HasNotStartedYet
        | Deferred.InProgress -> Html.none
        | Deferred.Failed ex -> ex |> Server.exnToError |> displayStronglyTypedError
        | Deferred.Resolved resp ->
            Browser.WebStorage.localStorage.setItem("token", resp.Token) // store for later usage
            Html.text "YOU ARE IN!"

    Daisy.card [
        prop.className "p-10 bg-base-200"
        prop.children [
            Daisy.formControl [
                Daisy.label [Daisy.labelText "Email"]
                Daisy.input [
                    input.bordered; prop.placeholder "Email"
                    prop.type'.text
                    prop.onTextChange (fun x -> { loginForm with Email = x } |> setLoginForm)
                ]
            ]
            Daisy.formControl [
                Daisy.label [Daisy.labelText "Password"]
                Daisy.input [
                    input.bordered; prop.placeholder "Password"
                    prop.type'.password
                    prop.onTextChange (fun x -> { loginForm with Password = x } |> setLoginForm)
                ]
            ]
            Daisy.button.button [
                button.primary
                prop.text "LOGIN"
                if Deferred.inProgress loginReq then prop.disabled true
                prop.onClick login
            ]

            result
        ]
    ]