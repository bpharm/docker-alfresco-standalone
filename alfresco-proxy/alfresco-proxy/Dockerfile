FROM microsoft/aspnetcore-build:1.1.2 AS build-env
WORKDIR /app

ADD *.csproj ./
RUN dotnet restore

ADD . ./
RUN dotnet publish -c Release -o out

FROM microsoft/aspnetcore:1.1.2
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "alfresco-proxy.dll"]




