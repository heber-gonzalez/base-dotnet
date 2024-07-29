# Use SDK image as build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
# Set /app as working directory
WORKDIR /app
EXPOSE 5224

ENV ASPNETCORE_URLS=http://+:5224
# Copy .csproj file and restore dependencies
COPY *.csproj .
RUN dotnet restore
# Copy everything else and publish app
COPY . .
RUN dotnet publish -c Release -o /app/published-app

# Use runtime image as final stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 as runtime
# Set /app as working directory
WORKDIR /app
# Copy published app from build stage
COPY --from=build /app/published-app /app
# Run app when container starts
ENTRYPOINT [ "dotnet", "BaseBackend.dll" ]
