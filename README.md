# CarAssistant

CarAssistant is a .NET 8 API with an SQLite database and an OpenAI-powered car assistant.

## Local setup

Install the .NET 8 SDK, then configure development credentials using .NET user secrets:

```sh
dotnet user-secrets set "OpenAi:ApiKey" "YOUR_NEW_API_KEY" --project CarAssistant.Api
dotnet user-secrets set "Jwt:Key" "YOUR_RANDOM_SIGNING_KEY_AT_LEAST_32_BYTES" --project CarAssistant.Api
dotnet run --project CarAssistant.Api --launch-profile http
```

The HTTP launch profile uses the Development environment, which loads user secrets automatically. Secrets are stored outside the repository. Replace any previously committed credentials at their issuing service before using new ones.

For deployment, configure `OpenAi__ApiKey` and `Jwt__Key` as environment variables through your host's secret settings. Keep these values out of source files and committed configuration. The OpenAI model is configured with `OpenAi:Model` in `CarAssistant.Api/appsettings.json`.

Build output, local IDE state, environment files, and local SQLite databases are ignored by Git. Database schema migrations remain tracked.
