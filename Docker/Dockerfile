FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

ARG URL_EXPOSE_PORT=80
ARG URL_EXPOSE_PORT_HTTPS=443
ARG BUILDER_VERSION=latest
ARG RELEASE_DATE="2020-04-02"

LABEL h2020.ipmdecisions.upr.version=${BUILDER_VERSION} \
    vendor="RSK ADAS Ltd" \
    h2020.ipmdecisions.uprrelease-date=${RELEASE_DATE}

WORKDIR /app
EXPOSE ${URL_EXPOSE_PORT}
EXPOSE ${URL_EXPOSE_PORT_HTTPS}

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["H2020.IPMDecisions.UPR.API/H2020.IPMDecisions.UPR.API.csproj", "H2020.IPMDecisions.UPR.API/"]
COPY ["H2020.IPMDecisions.UPR.API/appsettingsTemplate.json", "H2020.IPMDecisions.UPR.API/appsettings.json"]
RUN dotnet restore "H2020.IPMDecisions.UPR.API/H2020.IPMDecisions.UPR.API.csproj"

COPY . .
WORKDIR /src/H2020.IPMDecisions.UPR.API
RUN dotnet build "H2020.IPMDecisions.UPR.API.csproj" -c Release -o /app/build

#WORKDIR /src/H2020.IPMDecisions.UPR.Tests
#RUN dotnet test "H2020.IPMDecisions.UPR.Tests.csproj" --filter Category!=Docker

WORKDIR /src/H2020.IPMDecisions.UPR.API
FROM build AS publish
RUN dotnet publish "H2020.IPMDecisions.UPR.API.csproj" -c Release -o /app/publish

FROM base AS final
ARG URL_PORT=5006
ARG URL_PORT_HTTPS=443

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "H2020.IPMDecisions.UPR.API.dll"]