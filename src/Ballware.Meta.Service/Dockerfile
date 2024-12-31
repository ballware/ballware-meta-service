FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
WORKDIR /src

COPY ./Ballware.Meta.Authorization ./Ballware.Meta.Authorization
COPY ./Ballware.Meta.Authorization.Jint ./Ballware.Meta.Authorization.Jint
COPY ./Ballware.Meta.Caching ./Ballware.Meta.Caching
COPY ./Ballware.Meta.Data ./Ballware.Meta.Data
COPY ./Ballware.Meta.Data.Ef ./Ballware.Meta.Data.Ef
COPY ./Ballware.Meta.Service ./Ballware.Meta.Service
COPY ./Ballware.Meta.Tenant.Data ./Ballware.Meta.Tenant.Data
COPY ./Ballware.Meta.Tenant.Data.SqlServer ./Ballware.Meta.Tenant.Data.SqlServer

WORKDIR "/src/Ballware.Meta.Service"
RUN dotnet build "Ballware.Meta.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ballware.Meta.Service.csproj" -c Release -o /app/publish

FROM base AS final
USER app
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Ballware.Meta.Service.dll"]