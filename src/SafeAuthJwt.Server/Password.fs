module SafeAuthJwt.Server.Password

open System
open System.Runtime.CompilerServices
open System.Security.Cryptography
open Microsoft.AspNetCore.Cryptography.KeyDerivation

[<Literal>]
let private saltSize = 16

[<Literal>]
let private numBytesRequested = 32
[<Literal>]
let private iterationCount = 10000

[<MethodImpl(MethodImplOptions.NoInlining ||| MethodImplOptions.NoOptimization)>]
let private bytesAreEqual (a:byte []) (b:byte []) =
    if a = null && b = null then
        true
    elif a = null || b = null || a.Length <> b.Length then
        false
    else
        let mutable areEqual = true
        for i = 0 to a.Length - 1 do
            areEqual <- areEqual && (a.[i] = b.[i])
        areEqual

let private writeNetworkByteOrder (buffer:byte []) (offset:int) (value:uint32) =
    buffer.[(offset + 0)] <- (value >>> 24) |> byte
    buffer.[(offset + 1)] <- (value >>> 16) |> byte
    buffer.[(offset + 2)] <- (value >>> 8) |> byte
    buffer.[(offset + 3)] <- (value >>> 0) |> byte
    ()

let private readNetworkByteOrder (buffer:byte []) offset =
    (buffer.[offset + 0] |> uint32 <<< 24)
    ||| (buffer.[offset + 1] |> uint32 <<< 16)
    ||| (buffer.[offset + 2] |> uint32 <<< 8)
    ||| (buffer.[offset + 3] |> uint32)

let createHash password =
    let rng = RandomNumberGenerator.Create()
    let salt = Array.zeroCreate<Byte> saltSize
    rng.GetBytes(salt)
    let subKey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, iterationCount, numBytesRequested)

    let outputBytes = Array.zeroCreate<Byte> (13 + salt.Length + subKey.Length)
    outputBytes.[0] <- 0x01 |> byte
    writeNetworkByteOrder outputBytes 1 (KeyDerivationPrf.HMACSHA256 |> uint32)
    writeNetworkByteOrder outputBytes 5 (iterationCount |> uint32)
    writeNetworkByteOrder outputBytes 9 (saltSize |> uint32)
    Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length)
    Buffer.BlockCopy(subKey, 0, outputBytes, 13 + salt.Length, subKey.Length)

    outputBytes
    |> Convert.ToBase64String

let verifyPassword hash password =
    let hashedPassword = hash |> Convert.FromBase64String
    let prf : KeyDerivationPrf = readNetworkByteOrder hashedPassword 1 |> int |> enum
    let iterCount = readNetworkByteOrder hashedPassword 5 |> int
    let saltLength = readNetworkByteOrder hashedPassword 9 |> int

    let currentSalt = Array.zeroCreate<Byte> saltLength
    Buffer.BlockCopy(hashedPassword, 13, currentSalt, 0, currentSalt.Length)

    let subKeyLength = hashedPassword.Length - 13 - currentSalt.Length
    let expectedSubKey = Array.zeroCreate<Byte> subKeyLength
    Buffer.BlockCopy(hashedPassword, 13 + currentSalt.Length, expectedSubKey, 0, expectedSubKey.Length)
    let actualSubKey = KeyDerivation.Pbkdf2(password, currentSalt, prf, iterCount, subKeyLength)
    bytesAreEqual actualSubKey expectedSubKey