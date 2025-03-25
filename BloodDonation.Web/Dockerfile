#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BloodDonation.Web/BloodDonation.Web.csproj", "BloodDonation.Web/"]
COPY ["BloodDonation.Infra.Data/BloodDonation.Infra.Data.csproj", "BloodDonation.Infra.Data/"]
COPY ["BloodDonation.Domain/BloodDonation.Domain.csproj", "BloodDonation.Domain/"]
COPY ["BloodDonation.Infra.IoC/BloodDonation.Infra.IoC.csproj", "BloodDonation.Infra.IoC/"]
COPY ["BloodDonation.Application/BloodDonation.Application.csproj", "BloodDonation.Application/"]
RUN dotnet restore "./BloodDonation.Web/BloodDonation.Web.csproj"
COPY . .
WORKDIR "/src/BloodDonation.Web"
RUN dotnet build "./BloodDonation.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BloodDonation.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BloodDonation.Web.dll"]