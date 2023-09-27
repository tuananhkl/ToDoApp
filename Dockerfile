# Use the official .NET SDK image as a build stage.
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS build

# Set the working directory to /app
WORKDIR /app

# Copy the source code to the container
COPY ./TodoAppApi /app

# Build the application
RUN dotnet publish -c Release -o out

# Use the official runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

# Set the working directory to /app
WORKDIR /app

# Copy the published app from the build stage
COPY --from=build /app/out .

# Expose a port (change this if your app listens on a different port)
#EXPOSE 80

# Define the command to run your application
CMD ["./TodoAppApi"]

# Note: Make sure to replace "TodoAppApi" with the actual name of your API project's output executable.