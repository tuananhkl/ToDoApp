# FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/sdk:6.0 AS build   
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# FROM mcr.microsoft.com/dotnet/sdk:6.0-bookworm-slim-arm64v8 AS build
WORKDIR /app

#COPY *.csproj ./
COPY TodoAppApi/TodoAppApi.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

# FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/aspnet:6.0 AS final
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
# FROM mcr.microsoft.com/dotnet/aspnet:6.0-bookworm-slim-arm64v8 AS final

WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "TodoAppApi.dll"]
