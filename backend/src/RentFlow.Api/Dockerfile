FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend/RentFlow.sln backend/
COPY backend/src/RentFlow.Api/RentFlow.Api.csproj backend/src/RentFlow.Api/
COPY backend/src/RentFlow.Application/RentFlow.Application.csproj backend/src/RentFlow.Application/
COPY backend/src/RentFlow.Domain/RentFlow.Domain.csproj backend/src/RentFlow.Domain/
COPY backend/src/RentFlow.Infrastructure/RentFlow.Infrastructure.csproj backend/src/RentFlow.Infrastructure/
COPY backend/tests/RentFlow.Tests/RentFlow.Tests.csproj backend/tests/RentFlow.Tests/

RUN dotnet restore backend/src/RentFlow.Api/RentFlow.Api.csproj

COPY backend/ backend/
RUN dotnet publish backend/src/RentFlow.Api/RentFlow.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RentFlow.Api.dll"]
