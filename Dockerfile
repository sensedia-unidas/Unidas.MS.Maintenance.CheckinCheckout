#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Unidas.MS.Maintenance.CheckinCheckout.ConsumerWorker/Unidas.MS.Maintenance.CheckinCheckout.ConsumerWorker.csproj", "Unidas.MS.Maintenance.CheckinCheckout.ConsumerWorker/"]
COPY ["Unidas.MS.Maintenance.CheckinCheckout.Infra.IoC/Unidas.MS.Maintenance.CheckinCheckout.Infra.IoC.csproj", "Unidas.MS.Maintenance.CheckinCheckout.Infra.IoC/"]
COPY ["Unidas.MS.Maintenance.CheckinCheckout.Application/Unidas.MS.Maintenance.CheckinCheckout.Application.csproj", "Unidas.MS.Maintenance.CheckinCheckout.Application/"]
RUN dotnet restore "Unidas.MS.Maintenance.CheckinCheckout.ConsumerWorker/Unidas.MS.Maintenance.CheckinCheckout.ConsumerWorker.csproj"
COPY . .
WORKDIR "/src/Unidas.MS.Maintenance.CheckinCheckout.ConsumerWorker"
RUN dotnet build "Unidas.MS.Maintenance.CheckinCheckout.ConsumerWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Unidas.MS.Maintenance.CheckinCheckout.ConsumerWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Unidas.MS.Maintenance.CheckinCheckout.ConsumerWorker.dll"]