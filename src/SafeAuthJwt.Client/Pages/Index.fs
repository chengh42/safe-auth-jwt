module SafeAuthJwt.Client.Pages.Index

open Feliz
open Feliz.DaisyUI
open Elmish
open Feliz.UseElmish
open SafeAuthJwt.Client

type State = {
    Message : string
}

type Msg =
    | AskForPublicMessage
    | AskForPrivateMessage
    | MessageReceived of string

let init () = { Message = "Click above button to get a message" }, Cmd.none

let update (msg:Msg) (model:State) : State * Cmd<Msg> =
    match msg with
    | AskForPublicMessage ->
        model, Cmd.OfAsync.perform Server.anonymousAPI.GetPublicMessage () MessageReceived
    | AskForPrivateMessage ->
        let getPrivateMessage = fun (securedApi: SafeAuthJwt.Shared.API.SecuredAPI) ->
            securedApi.GetPrivateMessage ()
        model, Cmd.OfAsync.perform (fun _ -> Server.onSecuredAPI getPrivateMessage) () MessageReceived
    | MessageReceived msg ->
        printfn "msg = %A" msg
        { model with Message = msg }, Cmd.none

[<ReactComponent>]
let IndexView () =
    let state, dispatch = React.useElmish(init, update, [| |])

    Html.div [
        Daisy.cardTitle "Get a message! Which type?"
        Daisy.buttonGroup [
            Daisy.button.button [
                button.primary
                prop.text "Public message"
                prop.onClick (fun _ -> AskForPublicMessage |> dispatch)
            ]
            Daisy.button.button [
                button.secondary
                prop.text "Private message"
                prop.onClick (fun _ -> AskForPrivateMessage |> dispatch)
            ]
        ]
        Daisy.labelText state.Message
    ]
