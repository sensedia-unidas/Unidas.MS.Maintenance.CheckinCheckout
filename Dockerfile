#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Unidas.MS.Maintenance.CheckinCheckout.Consumer/Unidas.MS.Maintenance.CheckinCheckout.Consumer.csproj", "Unidas.MS.Maintenance.CheckinCheckout.Consumer/"]
COPY ["Unidas.MS.Maintenance.CheckinCheckout.Application/Unidas.MS.Maintenance.CheckinCheckout.Application.csproj", "Unidas.MS.Maintenance.CheckinCheckout.Application/"]
COPY ["Unidas.MS.Maintenance.CheckinCheckout.Infra.IoC/Unidas.MS.Maintenance.CheckinCheckout.Infra.IoC.csproj", "Unidas.MS.Maintenance.CheckinCheckout.Infra.IoC/"]
RUN dotnet restore "Unidas.MS.Maintenance.CheckinCheckout.Consumer/Unidas.MS.Maintenance.CheckinCheckout.Consumer.csproj"
COPY . .
WORKDIR "/src/Unidas.MS.Maintenance.CheckinCheckout.Consumer"
RUN dotnet build "Unidas.MS.Maintenance.CheckinCheckout.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Unidas.MS.Maintenance.CheckinCheckout.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Unidas.MS.Maintenance.CheckinCheckout.Consumer.dll"]