# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy everything and restore dependencies
COPY . ./
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o /publish

# Use a lightweight runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Add a non-root user and set permissions
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app

# Switch to the non-root user
USER appuser

# Copy the published files from the build stage
COPY --from=build /publish .

# Expose the port your app runs on
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "DockerIpService.dll"]

