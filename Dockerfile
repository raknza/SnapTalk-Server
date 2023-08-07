FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
COPY cert.pfx ./
EXPOSE 5277

# 安裝網絡工具
RUN apt-get update && apt-get install -y iputils-ping dnsutils

ENV ASPNETCORE_URLS=http://+:5277

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["android-backend.csproj", "./"]
RUN dotnet restore "android-backend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "android-backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "android-backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "android-backend.dll"]

