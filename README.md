# GenePlanet Test - Host an ASP.NET Core App with Nginx and Docker: Load Balancing

## Project creation
 Reading article [Configure ASP.NET Core to work with proxy servers and load balancers](https://dev.to/avinashth/containerize-a-net-core-web-api-project-4p05), I created core application, based on default "Weather Forecast" web api service.
 
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

I highly recommend creating and running MySQL server locally. I recommend [HeidiSQL](https://www.heidisql.com/) or MySQL Workbench. Not mention anywhere, we need firstly to prepare database, table and user on host db server. Later, we will use host's files as volume to access it from dockerized mysql server.

Create database named "gene-task". Inside that db, load and run "setup.sql" script (on root folder, "sql-scripts"). This will create "LogAccessCounts" table, where web api services /api/count and /api/allcounts will insert or select data.

Don't forget to set user connect from anywhere ( % ) in local MySQL server.

In Docker-compose.yml script, you can see how dockerized MySQL is set, but user, db and tables are being read and written on local host mysql (which has to be shutdown). That's why it is unnececary to setup mysql in docker. With this configuration, dockerized mysql is seen and accessed by loadbalanced workers (same ip range). Docker has big problems with using loadbalancing and database servers, servers by default don't see each other because ip range difference.

## Set database access in asp.net core app

## Load Balancing
This is example of more simple mechanism, that can be implemented in docker-compose.yml file. 
Basicly it is Nginx web server acting as Loadbalancer as docker container and working application servers (asp.net core api) as worker nodes. MySQL DB is single, and has volume stored in host mysql server. (dockerized mysql server is using filesystem of host mysql server. Host mysql servrer needs to be shutdown or dockerized server set on different ports)

Demo is using http headers X-Forwarded-For, X-Forwarded-Proto, X-Forwarded-Host to do load balancing task.

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

Remotly connect to bash on docker container:
```bash
docker exec -it Docker-Container /bin/bash
```

Get logs from docker container
```bash
docker logs some-mysql
```

See all established connedtions on host or in docker 
```bash
netstat -aon | findstr :80
```
## Alternatives to this case fo Loadbalancing

This could be achived by using Kubernetes (on service provider like Linode, DO, Azure, aws) or [Minikube](https://minikube.sigs.k8s.io/docs/start/)

Or using _[KEDA](https://blog.tomkerkhove.be/2019/06/14/scaling-apps-with-keda/) on Azure.

## SSL

### Generate an OpenSSL certificate

On Windows, if you have _Git for Windows_ installed, then you can use the `openssl` command directly. Otherwise, the official page: [OpenSSL.Wiki: Binaries](https://wiki.openssl.org/index.php/Binaries) contains useful URLs for downloading and installation guides.

```bash
openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout localhost.key -out localhost.crt -passin pass:YourSecurePassword
```

This command will generate two files: `localhost.crt` and `localhost.key`.

Generated files are already included in this github repo for your convinience.
