module Pages.MyProfile

open Feliz
open Feliz.DaisyUI
open Feliz.UseDeferred
open Shared.Errors

let displayStronglyTypedError = function
    // choose how to display errors based on your needs
    | ServerError.Exception x -> Html.text x
    | ServerError.Authentication x -> Html.div x

let getUserInfo = fun (securedApi: Shared.API.SecuredAPI) ->
    securedApi.GetUserInfo()

[<ReactComponent>]
let MyProfileView () =
    let profileReq, setProfileReq = React.useState(Deferred.HasNotStartedYet)
    let signOutReq, setSignOutReq = React.useState(Deferred.HasNotStartedYet)
    let getProfile = React.useDeferredCallback ((fun _ -> Server.onSecuredAPI getUserInfo), setProfileReq)
    let signOut = React.useDeferredCallback (
        (fun _ -> async {
            Browser.WebStorage.localStorage.clear()
            return () }),
        setSignOutReq)

    let info =
        match profileReq with
        | Deferred.HasNotStartedYet
        | Deferred.InProgress -> Html.none
        | Deferred.Failed ex -> ex |> Server.exnToError |> displayStronglyTypedError
        | Deferred.Resolved resp -> Html.div $"You are {resp.Name} with email {resp.Email}"

    Html.div [
        info
        Daisy.button.button [
            button.primary
            prop.text "WHO AM I?!"
            if Deferred.inProgress profileReq then prop.disabled true
            prop.onClick getProfile
        ]
        match profileReq with
        | Deferred.Resolved _ ->
            Daisy.button.button [
                button.secondary
                prop.text "Sign out"
                prop.onClick signOut
            ]
            match signOutReq with
            | Deferred.Resolved _ -> Html.text "You are signed out."
            | _ -> ()
        | _ -> ()
    ]