module Views

open Feliz
open Feliz.Bulma
open Feliz.UseDeferred

open Shared

let displayStronglyTypedError = function
    // choose how to display errors based on your needs
    | ServerError.Exception x -> Html.text x
    | ServerError.Authentication x -> Html.div x

let styleBox = [
    style.borderStyle.solid
    style.borderWidth 3
    style.borderRadius 5
    style.padding 20
    style.margin 20
]

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
        prop.style styleBox
        prop.children [
            Bulma.block [
                Bulma.title.h5 "BOX 1. Login with credentials"
                Bulma.text.p "Credentials available:"
                Html.ul [
                    prop.style [
                        style.listStyleType.initial
                        style.listStylePosition.inside
                    ]
                    prop.children [
                        for (email, pwd) in [
                            "bob@fsharp.net", "Str0ngP@zzword4Bob"
                            "alice@fsharp.net", "Str0ngP@zzword4Alice"
                        ] do
                        Html.li [
                            Html.code email
                            Html.span " & "
                            Html.code pwd
                        ]
                    ]
                ]
            ]
            Html.input [
                prop.type'.text
                prop.placeholder "Email"
                prop.onTextChange (fun x -> { loginForm with Email = x } |> setLoginForm)
            ]
            Html.input [
                prop.type'.password
                prop.placeholder "Password"
                prop.onTextChange (fun x -> { loginForm with Password = x } |> setLoginForm)
            ]
            Html.button [
                prop.text "LOGIN"
                if Deferred.inProgress loginReq then prop.disabled true
                prop.onClick login
            ]
            Html.div [ result ]
        ]
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
        prop.style styleBox
        prop.children [
            Bulma.block [
                Bulma.title.h5 "BOX 2. Check user info"
                Bulma.text.p "After signing-in (you should get the \"YOU ARE IN!\" in BOX 1), refresh webpage, and then click the button below."
            ]
            info
            Html.button [
                prop.text "WHO AM I?!"
                if Deferred.inProgress profileReq then prop.disabled true
                prop.onClick getProfile
            ]
        ]
    ]

[<ReactComponent>]
let RevokeTokenButton () =
    let signoutReq, setSignoutReq = React.useState(Deferred.HasNotStartedYet)
    Html.div [
        prop.style styleBox
        prop.children [
            Bulma.block [
                Bulma.title.h5 "BOX 3. Revoke token"
                Bulma.text.p "Click the button below, refresh webpage, and then check the user info in BOX 2 again."
            ]
            Html.button [
                prop.text "Revoke token"
                if Deferred.inProgress signoutReq then prop.disabled true
                prop.onClick (fun _ -> Browser.WebStorage.localStorage.clear() )
            ]
        ]
    ]