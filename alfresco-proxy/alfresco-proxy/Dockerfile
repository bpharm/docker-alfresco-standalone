FROM microsoft/aspnetcore:1.1.2
ARG source
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

COPY --from=build-env /app/out ./
ENTRYPOINT ["dotnet", "alfresco-proxy.dll"]




