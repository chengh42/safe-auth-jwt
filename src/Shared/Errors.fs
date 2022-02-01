module Shared.Errors

type ServerError =
    | Exception of string
    | Authentication of string
    // add here any other you want to use

exception ServerException of ServerError

module ServerError =
    let failwith (er:ServerError) = raise (ServerException er)

    let ofOption err (v:Option<_>) =
        v
        |> Option.defaultWith (fun _ -> err |> failwith)