FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Sales.sln ./
COPY Sales.Api/Sales.Api.csproj Sales.Api/
COPY Sales.Application/Sales.Application.csproj Sales.Application/
COPY Sales.Domain/Sales.Domain.csproj Sales.Domain/
COPY Sales.Infrastructure/Sales.Infrastructure.csproj Sales.Infrastructure/
RUN dotnet restore Sales.sln

COPY . .
RUN dotnet publish Sales.Api/Sales.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Sales.Api.dll"]
