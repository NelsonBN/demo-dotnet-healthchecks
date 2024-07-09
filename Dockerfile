FROM mcr.microsoft.com/dotnet/aspnet:8.0.3-alpine3.19-amd64 AS base-env

USER app
WORKDIR /app
EXPOSE 8080

HEALTHCHECK --interval=10s \
            --timeout=5s \
            --start-period=3s \
            --retries=5 \
    CMD wget --quiet --tries=1 --spider http://localhost:8080/healthz/ready || exit 1

ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS http://*:8080



FROM mcr.microsoft.com/dotnet/sdk:8.0.203 AS build-env

WORKDIR /src

COPY ./src/*.csproj .
RUN dotnet restore ./*.csproj --runtime linux-musl-x64

COPY ./src/ .
RUN dotnet build -c Release ./*.csproj --runtime linux-musl-x64

RUN dotnet publish ./*.csproj \
    -c Release \
    --runtime linux-musl-x64 \
    --no-build \
    --no-restore \
    --self-contained false \
    /p:UseAppHost=false \
    -o /publish



FROM base-env AS run-env

COPY --from=build-env /publish .

ENTRYPOINT ["dotnet", "Demo.Api.dll"]
