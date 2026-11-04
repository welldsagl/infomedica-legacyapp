# Build context: la ROOT del repository (servono Directory.*.props, global.json e LegacyApp.Data).
#   docker build -f src/LegacyApp.Api/Dockerfile -t legacyapp-api .
# Su Railway: variabile RAILWAY_DOCKERFILE_PATH=src/LegacyApp.Api/Dockerfile (root directory = /).

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Prima i file di progetto: il restore resta in cache finche' non cambiano le dipendenze
COPY global.json Directory.Build.props Directory.Packages.props ./
COPY src/LegacyApp.Data/LegacyApp.Data.vbproj src/LegacyApp.Data/
COPY src/LegacyApp.Api/LegacyApp.Api.vbproj   src/LegacyApp.Api/
RUN dotnet restore src/LegacyApp.Api

COPY src/LegacyApp.Data src/LegacyApp.Data
COPY src/LegacyApp.Api  src/LegacyApp.Api
RUN dotnet publish src/LegacyApp.Api -c Release --no-restore -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Su Linux esiste solo il provider SqlServer (ACE/Access e' Windows-only).
# La connection string arriva da ConnectionStrings__SqlServer; le origini CORS da Cors__AllowedOrigins__0, __1, ...
ENV ASPNETCORE_ENVIRONMENT=Production \
    Data__Provider=SqlServer

USER $APP_UID
# Railway (e simili) assegnano la porta in $PORT; 8080 in locale.
CMD ["sh", "-c", "ASPNETCORE_HTTP_PORTS=${PORT:-8080} exec dotnet LegacyApp.Api.dll"]
