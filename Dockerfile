#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5010
EXPOSE 44326

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ZeroBrowser.Crawler/ZeroBrowser.Crawler.Api/ZeroBrowser.Crawler.Api.csproj", "ZeroBrowser.Crawler.Api/"]
RUN dotnet restore "ZeroBrowser.Crawler.Api/ZeroBrowser.Crawler.Api.csproj"
COPY . .
WORKDIR "/src/ZeroBrowser.Crawler/ZeroBrowser.Crawler.Api"
RUN dotnet build "ZeroBrowser.Crawler.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet dev-certs https -ep /app/aspnetapp.pfx --trust -p demo
RUN dotnet publish "ZeroBrowser.Crawler.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /app/aspnetapp.pfx .
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=demo
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/aspnetapp.pfx
ENTRYPOINT ["dotnet", "ZeroBrowser.Crawler.Api.dll"]