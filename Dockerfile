FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 1000
EXPOSE 1443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TodoAppApi/TodoAppApi.csproj", "TodoAppApi/"]
RUN dotnet restore "TodoAppApi/TodoAppApi.csproj"
COPY . .
WORKDIR "/src/TodoAppApi"
RUN dotnet build "TodoAppApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TodoAppApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoAppApi.dll"]
