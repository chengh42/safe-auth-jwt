module Index

open Elmish
open Fable.Remoting.Client
open Shared

type Model = { UserInfo : Response.UserInfo option }

type Msg =
    | DummyMsg

let init () : Model * Cmd<Msg> =
    let model = { UserInfo = None }
    let cmd = Cmd.none

    model, cmd

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | DummyMsg ->
        model,
        Cmd.none

open Feliz
open Feliz.Bulma

let view (model: Model) (dispatch: Msg -> unit) =
    Bulma.hero [
        hero.isFullHeight
        color.isLight
        prop.children [
            Bulma.heroBody [
                Bulma.container [
                    Bulma.column [
                        column.is6
                        column.isOffset3
                        prop.children [
                            Bulma.title [
                                text.hasTextCentered
                                prop.text "SAFE Authentication with JWT"
                            ]
                            Views.LoginView ()
                            Views.MyProfileView ()
                            Views.RevokeTokenButton ()
                        ]
                    ]
                ]
            ]
        ]
    ]
