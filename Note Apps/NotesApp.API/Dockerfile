# ── Build Stage ───────────────────────────────────────────────────────────────
# Use the official .NET 10 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

# Copy project file first and restore dependencies
# This layer is cached — only re-runs if .csproj changes
COPY *.csproj .
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o out

# ── Runtime Stage ─────────────────────────────────────────────────────────────
# Use the smaller runtime image — no SDK needed to run the app
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

# Copy only the published output from the build stage
COPY --from=build /app/out .

# Port the app listens on
EXPOSE 5104

ENTRYPOINT ["dotnet", "NotesApp.API.dll"]