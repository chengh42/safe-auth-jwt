module Views

open Feliz
open Feliz.UseDeferred

open Shared

let displayStronglyTypedError = function
    // choose how to display errors based on your needs
    | ServerError.Exception x -> Html.text x
    | ServerError.Authentication x -> Html.div x

[<ReactComponent>]
let LoginView () =
    let emptyForm : Request.Login = { Email = ""; Password = "" }
    let loginForm,setLoginForm = React.useState(emptyForm)
    let loginReq, setLoginReq = React.useState(Deferred.HasNotStartedYet)
    let login = React.useDeferredCallback ((fun _ -> Server.anonymousAPI.Login loginForm), setLoginReq)

    let result =
        match loginReq with
        | Deferred.HasNotStartedYet -> Html.text "Insert credentials to sign-in."
        | Deferred.InProgress -> Html.text "Signing-in ..."
        | Deferred.Failed ex -> ex |> Server.exnToError |> displayStronglyTypedError
        | Deferred.Resolved resp ->
            Browser.WebStorage.localStorage.setItem("token", resp.Token) // store for later usage
            Html.text "YOU ARE IN!"

    Html.div [
        Html.input [
            prop.type'.text
            prop.placeholder "bob@fsharp.net"
            prop.onTextChange (fun x -> { loginForm with Email = x } |> setLoginForm)
        ]
        Html.input [
            prop.type'.password
            prop.placeholder "Str0ngP@zzword4Bob"
            prop.onTextChange (fun x -> { loginForm with Password = x } |> setLoginForm)
        ]
        Html.button [
            prop.text "LOGIN"
            if Deferred.inProgress loginReq then prop.disabled true
            prop.onClick login
        ]
        Html.div [ result ]
    ]

[<ReactComponent>]
let MyProfileView () =
    let profileReq, setProfileReq = React.useState(Deferred.HasNotStartedYet)
    let getProfile = React.useDeferredCallback ((fun _ -> Server.onSecuredAPI.GetUserInfo()), setProfileReq)

    let info =
        match profileReq with
        | Deferred.HasNotStartedYet
        | Deferred.InProgress -> Html.none
        | Deferred.Failed ex -> ex |> Server.exnToError |> displayStronglyTypedError |> Html.div
        | Deferred.Resolved resp -> Html.div $"You are {resp.Name} with email {resp.Email}"

    Html.div [
        info
        Html.button [
            prop.text "WHO AM I?!"
            if Deferred.inProgress profileReq then prop.disabled true
            prop.onClick getProfile
        ]
    ]