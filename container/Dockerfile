# Use the .NET 9 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the solution file and project files
COPY ../WizardAssessment.sln .
COPY ../src/WizardAssessment.API/WizardAssessment.API.csproj src/WizardAssessment.API/
COPY ../src/WizardAssessment.Application/WizardAssessment.Application.csproj src/WizardAssessment.Application/
COPY ../src/WizardAssessment.Domain/WizardAssessment.Domain.csproj src/WizardAssessment.Domain/
COPY ../src/WizardAssessment.Infrastructure/WizardAssessment.Infrastructure.csproj src/WizardAssessment.Infrastructure/
COPY ../src/WizardAssessment.Tests/WizardAssessment.Tests.csproj src/WizardAssessment.Tests/

# Restore dependencies for all projects
RUN dotnet restore WizardAssessment.sln

# Copy the rest of the source code
COPY ../src/. ./src/

# Publish the API project
WORKDIR /src/src/WizardAssessment.API
RUN dotnet publish -c Release -o /app/publish

# Use the ASP.NET 9 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port the app runs on
EXPOSE 5173

# Set the entrypoint for the container
ENTRYPOINT ["dotnet", "WizardAssessment.API.dll"]
