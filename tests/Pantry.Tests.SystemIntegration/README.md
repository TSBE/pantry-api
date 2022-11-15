# Pantry.Tests.SystemIntegration

## Instructions

### Run tests locally (easier)

To build and the the system integration tests locally, follow the instructions:

1. Run the application locally with docker-compose (set docker-compose as startup project in Visual Studio, launch with F5)
2. Open a powershell window in the root folder of the Git repository
3. Run the tests with the dotnet cli: `dotnet test --no-build` (no build because the DLLs are in use by the running application)

### Run tests locally in Docker container (harder)

The system integration tests can be run in a Docker container against the application. That's exactly what happens on the build server.

To build and run the system integration tests locally in a Docker container, follow the instructions:

1. Publish the system integration tests project
2. Switch to publish folder
3. Build the docker image
4. Run the application locally with docker-compose (set docker-compose as startup project in Visual Studio, launch with F5)
5. Find the docker network name that was created and the application belongs to using `docker network ls`
6. Run the created docker image

```powershell
dotnet publish -o .\publish
cd .\publish
docker build -t Pantry.Tests.SystemIntegration .
docker network ls # gather network name
docker run --env TestOutputDirectoryPath="/tmp" --env AppUrl="http://Pantry.Service:80" --env Kafka__BootstrapServers=kafka1:29092 --network=">>network name here<<" tests
```
