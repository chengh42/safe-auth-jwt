module Pages.Index

open Feliz
open Feliz.DaisyUI
open Elmish
open Feliz.UseElmish

type State = {
    Message : string
}

type Msg =
    | AskForPublicMessage
    | AskForPrivateMessage
    | MessageReceived of string
    | ErrorReceived of exn

let init () = { Message = "Click above button to get a message" }, Cmd.none

let update (msg:Msg) (model:State) : State * Cmd<Msg> =
    match msg with
    | AskForPublicMessage ->
        model, Cmd.OfAsync.perform Server.anonymousAPI.GetPublicMessage () MessageReceived
    | AskForPrivateMessage ->
        let getPrivateMessage = fun (securedApi: Shared.API.SecuredAPI) ->
            securedApi.GetPrivateMessage ()
        model, Cmd.OfAsync.either (fun _ -> Server.onSecuredAPI getPrivateMessage) ()
            MessageReceived ErrorReceived
    | MessageReceived msg ->
        { model with Message = msg }, Cmd.none
    | ErrorReceived err ->
        { model with Message = err.Message }, Cmd.none

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
