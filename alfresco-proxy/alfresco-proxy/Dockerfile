FROM microsoft/aspnetcore-build:1.1.2 AS build-env
WORKDIR /app
# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM microsoft/aspnetcore:1.1.2
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 5004
ENTRYPOINT ["dotnet", "alfresco-proxy.dll"]




