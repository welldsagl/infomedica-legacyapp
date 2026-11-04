# Blazor Server: app ASP.NET Core con SignalR (WebSocket). Build context: la ROOT del repository.
#   docker build -f src/LegacyApp.Blazor.Server/Dockerfile -t legacyapp-blazor-server .
#   docker run -p 8080:8080 -e ApiBaseUrl=http://host.docker.internal:5080/ legacyapp-blazor-server

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY global.json Directory.Build.props Directory.Packages.props ./
COPY src/LegacyApp.Client/LegacyApp.Client.vbproj               src/LegacyApp.Client/
COPY src/LegacyApp.Blazor.Shared/LegacyApp.Blazor.Shared.csproj src/LegacyApp.Blazor.Shared/
COPY src/LegacyApp.Blazor.Server/LegacyApp.Blazor.Server.csproj src/LegacyApp.Blazor.Server/
RUN dotnet restore src/LegacyApp.Blazor.Server

COPY src/LegacyApp.Client        src/LegacyApp.Client
COPY src/LegacyApp.Blazor.Shared src/LegacyApp.Blazor.Shared
COPY src/LegacyApp.Blazor.Server src/LegacyApp.Blazor.Server
RUN dotnet publish src/LegacyApp.Blazor.Server -c Release --no-restore -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_ENVIRONMENT=Production
USER $APP_UID
CMD ["sh", "-c", "ASPNETCORE_HTTP_PORTS=${PORT:-8080} exec dotnet LegacyApp.Blazor.Server.dll"]
