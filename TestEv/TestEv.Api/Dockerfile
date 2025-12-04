FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["TestEv.Api/TestEv.Api.csproj", "TestEv.Api/"]
COPY ["TestEv.Application/TestEv.Application.csproj", "TestEv.Application/"]
COPY ["TestEv.Domain/TestEv.Domain.csproj", "TestEv.Domain/"]
COPY ["TestEv.Infrastructure/TestEv.Infrastructure.csproj", "TestEv.Infrastructure/"]

RUN dotnet restore "TestEv.Api/TestEv.Api.csproj"

COPY . .
WORKDIR "/src/TestEv.Api"
RUN dotnet build "TestEv.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TestEv.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "TestEv.Api.dll"]
