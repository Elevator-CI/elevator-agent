FROM mcr.microsoft.com/powershell:latest as base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Elevator.Agent/Elevator.Agent.csproj", "Elevator.Agent/"]
RUN dotnet restore "Elevator.Agent/Elevator.Agent.csproj"
COPY . .
WORKDIR "/src/Elevator.Agent"
RUN dotnet build "Elevator.Agent.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Elevator.Agent.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/powershell AS powershell

FROM base AS final
WORKDIR /
RUN where powershell
RUN powershell -NoProfile -InputFormat None -ExecutionPolicy Bypass -Command "[System.Net.ServicePointManager]::SecurityProtocol = 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))" && SET "PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin"

RUN choco install -y dotnet-5.0-sdk
RUN choco install -y nodejs
RUN choco install -y git

WORKDIR /app
COPY --from=publish /app/publish .
WORKDIR /app
ENTRYPOINT ["/Program Files/dotnet/dotnet.exe", "Elevator.Agent.dll"]