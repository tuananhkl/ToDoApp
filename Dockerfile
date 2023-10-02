﻿FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

#COPY *.csproj ./
COPY TodoAppApi/TodoAppApi.csproj ./
RUN dotnet restore

# Set the target architecture to ARM64
ENV DOTNET_ROOT=/usr/share/dotnet-arm64
COPY . ./
RUN dotnet publish -c Release -o out -r linux-arm64

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "TodoAppApi.dll"]
