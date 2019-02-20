FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["PriceAlerts.Api/PriceAlerts.Api.csproj", "PriceAlerts.Api/"]
COPY ["PriceAlerts.Common/PriceAlerts.Common.csproj", "PriceAlerts.Common/"]
RUN dotnet restore "PriceAlerts.Api/PriceAlerts.Api.csproj"
COPY . .
WORKDIR "/src/PriceAlerts.Api"
RUN dotnet build "PriceAlerts.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "PriceAlerts.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PriceAlerts.Api.dll"]