FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CoffeeWise.Api/CoffeeWise.Api.csproj", "CoffeeWise.Api/"]
COPY ["CoffeeWise.Data/CoffeeWise.Data.csproj", "CoffeeWise.Data/"]
COPY ["CoffeeWise.BusinessLogic/CoffeeWise.BusinessLogic.csproj", "CoffeeWise.BusinessLogic/"]
RUN dotnet restore "CoffeeWise.Api/CoffeeWise.Api.csproj"
COPY . .
WORKDIR "/src/CoffeeWise.Api"
RUN dotnet build "CoffeeWise.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoffeeWise.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoffeeWise.Api.dll"]