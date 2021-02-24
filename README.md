# GenePlanet Test - Host an ASP.NET Core App with Nginx and Docker: SSL and Load Balancing

## Project creation
 Reading article _[Configure ASP.NET Core to work with proxy servers and load balancers](https://dev.to/avinashth/containerize-a-net-core-web-api-project-4p05), I created core application, based on default "Weather Forecast" web api service.
 
 Added MySQL Connector
 ```bash
  dotnet add package MySqlConnector
  ```

 Project compilation:
 ```bash
 dotnet publish -c Release
 ```
 Project loading&running:
  ```bash
 dotnet run
 ```

After testing, I ran project inside Docker containter (created Docker image and container):
```bash
docker build -t testimage -f Dockerfile .
docker run --name=testcontainer -p 8080:80 testimage
docker images
```

First initial Dockerfile for  project:
```bash
FROM mcr.microsoft.com/dotnet/aspnet:5.0

 COPY bin/Release/net5.0/publish/ MyWebAPI/
 WORKDIR /MyWebAPI
 ENTRYPOINT ["dotnet", "MyWebAPI.dll"]
```

Test example of web service access:
http://localhost:8080/weatherforecast

## Web service exposure

There are few web services that can be accessed:
  - Basic information, like service hostname, ip gateway and server ip address
  http://localhost
  - Default Weather forecast service with randomiozed data
  http://localhost/weatherforecast
  - User name Helloworld display
  http://localhost/api/helloworld/{Enter your name}
  - Count +1 when called and store to sql db
  http://localhost:5000/api/count
  - Show count score from sql db
  http://localhost:5000/api/allcounts
  
## Database preparation

I highly recommend creating and running MySQL server locally. I recommend _[HeidiSQL](https://www.heidisql.com/) or MySQL Workbench

## Set database access in asp.net core app

## Load Balancing
This is example of more simple mechanism, that can be implemented in docker-compose.yml file. 
Basicly it is Nginx web server acting as Loadbalancer as docker container and working application servers (asp.net core api) as worker nodes. MySQL DB is single, and has volume stored in host mysql server. (dockerized mysql server is using filesystem of host mysql server. Host mysql servrer needs to be shutdown or dockerized server set on different ports)

Finally the magic is done by first two commands. Third one is for adding additional worker. Parameter --scale api=4 is set for 4 worker nodes.
```bash
docker-compose build
docker-compose up --scale api=4 --build

docker-compose up
```

## Useful tools, when I was developing this demo

This demo works best on Linux, like Ubnuntu, System76 or RedHat or MacOS. I didn't manage it to run on Windows 10. On Windows is problem of docker mysql server volume file mounting.

With this command, I have managed to get ip addresses of docker containers.
```bash
docker inspect -f '{{.Name}} - {{.NetworkSettings.IPAddress }}' $(docker ps -aq)
```

## Alternatives to this case fo Loadbalancing

This could be achived by using Kubernetes (on service provider like Linode, DO, Azure, aws) or _[Minikube](https://minikube.sigs.k8s.io/docs/start/)

Or using _[KEDA](https://blog.tomkerkhove.be/2019/06/14/scaling-apps-with-keda/) on Azure.

## SSL

### Generate an OpenSSL certificate

On Windows, if you have _Git for Windows_ installed, then you can use the `openssl` command directly. Otherwise, the official page: [OpenSSL.Wiki: Binaries](https://wiki.openssl.org/index.php/Binaries) contains useful URLs for downloading and installation guides.

```bash
openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout localhost.key -out localhost.crt -passin pass:YourSecurePassword
```

This command will generate two files: `localhost.crt` and `localhost.key`.

Generated files are already included in this github repo for your convinience.
