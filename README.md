# AuthSketch

This is a **sketch** of the main **authentication** and **authorization** techniques in ASP.NET Core Api.   
It contains custom implementation of authentication and authorization handlers, custom access token creation, policies, role based authorization.   
Among custom logic it also uses **JwtBearer library** from Microsoft as a default authentication scheme to perform more robust auth logic and has an implementation of two factor authentication based on **Otp.NET library**.   
The application provides the ability to **sign up**, **verify** identity via email and then **sign in** in the system.   
If **TFA is enabled**, it is required to pass the TOTP code to sign in in the system. The code itself can be found in authenticator app (previsouly you need to add the secret to your authenticator) or it's possible to send it to an email.   
Sign in generates a pair of **access and refresh tokens**. Refresh token can be **exchanged** for a new pair of tokens or can be **revoked**.   
There is ability to **sign in with external providers** - **GitHub** and **Google** using **OAuth 2.0 with PKCE**. To make this logic work, add **ClientId** and **ClientSecret** to appsettings.json for corresponding providers.   
The **forgot, reset and change password** functionality allows to manage account security. The forgot password logic generates a **reset link** which then is being sent to the user's email.   
When resetting the password, all user's refresh tokens are revoked.   
Another way to sign in into the system is to use endpoints from **"DumbController"** which generates a **custom access token** (simple base64 string without any protection).    
That token can be used to access authorized resources just like a regular jwt token.    
The dumb token **must be in a format of "Dumb your_token"**.

# Main Tools and Technologies

- Otp.NET
- FluentEmail
- Postgres

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [Docker](https://docs.docker.com/get-docker)

To start the required app's infrastructure via Docker, type the following command at the solution directory:

`docker compose up -d`

Examples of requests can be found in **Insomnia AuthSketch Requests.json** file. As a name suggests, it can be imported to the Insomnia app.
