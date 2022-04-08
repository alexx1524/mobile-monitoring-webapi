#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Infotecs.Mobile.Monitoring/Infotecs.Mobile.Monitoring.csproj", "Infotecs.Mobile.Monitoring/"]
RUN dotnet restore "Infotecs.Mobile.Monitoring/Infotecs.Mobile.Monitoring.csproj"
COPY . .
WORKDIR "/src/Infotecs.Mobile.Monitoring"
RUN dotnet build "Infotecs.Mobile.Monitoring.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Infotecs.Mobile.Monitoring.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Infotecs.Mobile.Monitoring.dll"]