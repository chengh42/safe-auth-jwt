module SafeAuthJwt.Server.Storage

type DbUser = {
    Id: System.Guid
    Name: string
    Email: string
    PwdHash: string
}

let users : ResizeArray<DbUser> = ResizeArray()

let registerNewUser (req: SafeAuthJwt.Shared.API.Request.Register) =
    let userId = System.Guid.NewGuid ()
    let user =
        { Id = userId
          Name = req.Username
          Email = req.Email
          PwdHash = req.Password |> Password.createHash }

    users.Add user |> ignore
    userId

do
    registerNewUser { Username = "Bob"; Email = "bob@fsharp.net"; Password = "Str0ngP@zzword4Bob" } |> ignore
    registerNewUser { Username = "Alice"; Email = "alice@fsharp.net"; Password = "Str0ngP@zzword4Alice" } |> ignore


let getUserByEmail (email: string) =
    try
        users.Find (fun user -> user.Email = email) |> Some
    with ex ->
        printfn "Error when getUserByEmail: %s" ex.Message
        None

let getUserById (userId: System.Guid) =
    try
        users.Find (fun user -> user.Id = userId) |> Some
    with ex ->
        printfn "Error when getUserById: %s" ex.Message
        None