# Blazor WebAssembly = file statici. Build con l'SDK .NET, poi serviti da Caddy (con i .br precompressi).
# Build context: la ROOT del repository.
#   docker build -f src/LegacyApp.Blazor.Wasm/Dockerfile -t legacyapp-wasm .
#   docker run -p 8080:8080 -e API_BASE_URL=http://localhost:5080/ legacyapp-wasm

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY global.json Directory.Build.props Directory.Packages.props ./
COPY src/LegacyApp.Client/LegacyApp.Client.vbproj               src/LegacyApp.Client/
COPY src/LegacyApp.Blazor.Shared/LegacyApp.Blazor.Shared.csproj src/LegacyApp.Blazor.Shared/
COPY src/LegacyApp.Blazor.Wasm/LegacyApp.Blazor.Wasm.csproj     src/LegacyApp.Blazor.Wasm/
RUN dotnet restore src/LegacyApp.Blazor.Wasm

COPY src/LegacyApp.Client        src/LegacyApp.Client
COPY src/LegacyApp.Blazor.Shared src/LegacyApp.Blazor.Shared
COPY src/LegacyApp.Blazor.Wasm   src/LegacyApp.Blazor.Wasm
RUN dotnet publish src/LegacyApp.Blazor.Wasm -c Release --no-restore -o /app

FROM caddy:2-alpine AS runtime
COPY --from=build /app/wwwroot /srv
COPY src/LegacyApp.Blazor.Wasm/Caddyfile /etc/caddy/Caddyfile
COPY src/LegacyApp.Blazor.Wasm/docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh

# URL dell'API (con lo slash finale), scritto in appsettings.json all'avvio del container.
ENV API_BASE_URL="" \
    PORT=8080
ENTRYPOINT ["/docker-entrypoint.sh"]
