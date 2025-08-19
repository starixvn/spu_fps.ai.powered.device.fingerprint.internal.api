FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder

WORKDIR /src

COPY ["./src/Device.Fingerprint", "Device.Fingerprint/"]

COPY ["./src/Device.Fingerprint.Core", "Device.Fingerprint.Core/"]

COPY ["./src/Device.Fingerprint.Model", "Device.Fingerprint.Model/"]

COPY ["./src/Device.Fingerprint.Redis", "Device.Fingerprint.Redis/"]

COPY ["./src/Device.Fingerprint.InternalAPI/Device.Fingerprint.InternalAPI.csproj", "Device.Fingerprint.InternalAPI/"]

WORKDIR "/src/Device.Fingerprint.InternalAPI"

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

RUN dotnet restore -v n

COPY ./src/Device.Fingerprint.InternalAPI .

RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine-25h1 AS base

WORKDIR /app

COPY --from=builder /app .

ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Device.Fingerprint.InternalAPI.dll"]
