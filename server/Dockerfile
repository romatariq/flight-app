FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY *.props .
COPY *.sln .

COPY App.BLL/*.csproj ./App.BLL/
COPY App.Contracts.BLL/*.csproj ./App.Contracts.BLL/
COPY App.Contracts.DAL/*.csproj ./App.Contracts.DAL/
COPY App.DAL.EF/*.csproj ./App.DAL.EF/
COPY App.Domain/*.csproj ./App.Domain/
COPY App.Helpers/*.csproj ./App.Helpers/
COPY App.Mappers/*.csproj ./App.Mappers/
COPY App.Private.DTO/*.csproj ./App.Private.DTO/
COPY App.Public.DTO/*.csproj ./App.Public.DTO/
COPY Base.BLL/*.csproj ./Base.BLL/
COPY Base.Contracts/*.csproj ./Base.Contracts/
COPY Base.Contracts.BLL/*.csproj ./Base.Contracts.BLL/
COPY Base.Contracts.DAL/*.csproj ./Base.Contracts.DAL/
COPY Base.Contracts.Domain/*.csproj ./Base.Contracts.Domain/
COPY Base.DAL/*.csproj ./Base.DAL/
COPY Base.DAL.EF/*.csproj ./Base.DAL.EF/
COPY Base.Domain/*.csproj ./Base.Domain/
COPY Base.Helpers/*.csproj ./Base.Helpers/
COPY Base.Public.DTO/*.csproj ./Base.Public.DTO/
COPY FlightInfoCollector/*.csproj ./FlightInfoCollector/
COPY Tests/*.csproj ./Tests/
COPY WebApp/*.csproj ./WebApp/

RUN dotnet restore


# copy everything else and build app
COPY App.BLL/. ./App.BLL/
COPY App.Contracts.BLL/. ./App.Contracts.BLL/
COPY App.Contracts.DAL/. ./App.Contracts.DAL/
COPY App.DAL.EF/. ./App.DAL.EF/
COPY App.Domain/. ./App.Domain/
COPY App.Helpers/. ./App.Helpers/
COPY App.Mappers/. ./App.Mappers/
COPY App.Private.DTO/. ./App.Private.DTO/
COPY App.Public.DTO/. ./App.Public.DTO/
COPY Base.BLL/. ./Base.BLL/
COPY Base.Contracts/. ./Base.Contracts/
COPY Base.Contracts.BLL/. ./Base.Contracts.BLL/
COPY Base.Contracts.DAL/. ./Base.Contracts.DAL/
COPY Base.Contracts.Domain/. ./Base.Contracts.Domain/
COPY Base.DAL/. ./Base.DAL/
COPY Base.DAL.EF/. ./Base.DAL.EF/
COPY Base.Domain/. ./Base.Domain/
COPY Base.Helpers/. ./Base.Helpers/
COPY Base.Public.DTO/. ./Base.Public.DTO/
COPY FlightInfoCollector/. ./FlightInfoCollector/
COPY Tests/. ./Tests/
COPY WebApp/. ./WebApp/

WORKDIR /src/WebApp
RUN dotnet publish -c Release -o out



FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
EXPOSE 80

COPY --from=build /src/WebApp/out ./

ENTRYPOINT ["dotnet", "WebApp.dll"]