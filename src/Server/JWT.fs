module JWT

open System
open System.IdentityModel.Tokens.Jwt
open Microsoft.IdentityModel.Tokens

type Token = {
    Token : string
    ExpiresOn : DateTimeOffset
}

type JwtConfiguration = {
    Audience : string
    Issuer : string
    Secret : string
    AccessTokenLifetime : TimeSpan
}

let private isBeforeValid (before:Nullable<DateTime>) =
    if before.HasValue && before.Value > DateTime.UtcNow then false else true

let private isExpirationValid (expires:Nullable<DateTime>) =
    if expires.HasValue && DateTime.UtcNow > expires.Value then false else true

let private getKey (secret:string) = SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret))

let createToken (config:JwtConfiguration) claims =
    let credentials = SigningCredentials(getKey config.Secret, SecurityAlgorithms.HmacSha256)
    let issuedOn = DateTimeOffset.UtcNow
    let expiresOn = issuedOn.Add(config.AccessTokenLifetime)
    let jwtToken = JwtSecurityToken(config.Issuer, config.Audience, claims, (issuedOn.UtcDateTime |> Nullable), (expiresOn.UtcDateTime |> Nullable), credentials)
    let handler = JwtSecurityTokenHandler()
    { Token = handler.WriteToken(jwtToken); ExpiresOn = expiresOn }

let getParameters (config:JwtConfiguration) =
    let validationParams = TokenValidationParameters()
    validationParams.RequireExpirationTime <- true
    validationParams.ValidAudience <- config.Audience
    validationParams.ValidIssuer <- config.Issuer
    validationParams.ValidateLifetime <- true
    validationParams.LifetimeValidator <- (fun before expires _ _  -> isBeforeValid before && isExpirationValid expires)
    validationParams.ValidateIssuerSigningKey <- true
    validationParams.IssuerSigningKey <- config.Secret |> getKey
    validationParams

let validateToken (validationParams:TokenValidationParameters) (token:string) =
    try
        let handler = JwtSecurityTokenHandler()
        let principal = handler.ValidateToken(token, validationParams, ref null)
        principal.Claims |> Some
    with _ -> None
