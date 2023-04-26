# RosettaStone :: DOCKER

Getting an ```HttpListener``` application up and running in Docker can be rather tricky given how 1) Docker acts as a network proxy and 2) HttpListener isn't friendly to ```HOST``` header mismatches.  Thus, it is **critical** that you run your containers using ```--user ContainerAdministrator``` (Windows) or ```--user root``` (Linux or Mac) to bypass the ```HttpListener``` restrictions.  There are likely ways around this, but I have been unable to find one.  

## Before you Begin

As a persistent storage platform, data stored within the container, will be lost once the container terminates.  Similarly, any metadata stored in a database would be lost if the database resides within the container.  Likewise, any object data stored within the filesystem of the container would also be lost should the container terminate.

As such, it is important that you properly configure when deploying using containers.  Use the following best practices.

### Copy in Node Configuration

The ```rosettastone.json``` file which defines the configuration for your node should be either be copied in as part of your ```Dockerfile``` or overridden using either ```-v``` or ```docker-compose```.  Do not allow it to be built dynamically.

Set ```rosettastone.json``` ```EnableConsole``` to false and ```Logging.ConsoleLogging``` to false.  

Set your ```rosettastone.json``` ```Webserver.DnsHostname``` to ```*```.

### Use an External Database

RosettaStone relies on a database for storing object (and other) metadata.  While capable of using Sqlite, Sqlite databases are stored on the filesystem within the container which will be lost once the container terminates.  Use an external database such as SQL Server, MySQL, or PostgreSQL.  Modify the ```rosettastone.json``` ```Database``` section accordingly. 

Valid values for ```Type``` are: ```Mysql```, ```SqlServer```, ```Postgresql```, and ```Sqlite```
```
  "Database": {
    "Type": "Mysql",  
    "Hostname": "[database server hostname]",
    "Port": 3306,
    "DatabaseName": "rosettastone",
    "Instance": null,
    "Username": "root",
    "Password": "[password]"
  }
```

## Steps to Run RosettaStone in Docker

IMPORTANT: in some cases, you may have to use ```sudo``` in front of your ```docker``` commands.  If you receive a socket error when issuing a ```docker version``` command, you likely need to use ```sudo```.

1) Ensure you have installed the .NET 6.0 SDK.

2) View and modify the ```Dockerfile``` as appropriate, and execute the Docker build process:
```
$ docker build -t rosettastone -f Dockerfile .
```
**IMPORTANT** if you receive a failure indicating that dependencies were unable to be resolved, first ensure you have .NET 6.0 SDK installed and then issue the following commands (from the solution or project directory):
```
$ dotnet restore --force
$ dotnet build -c debug
$ dotnet build -c release
```

3) Verify the image exists:
```
$ docker images
REPOSITORY                              TAG                 IMAGE ID            CREATED             SIZE
rosettastone                            latest              047e29f37f9c        2 seconds ago       328MB
```
 
4) Execute the container:
```
Windows
$ docker run --user ContainerAdministrator -d -p 8000:8000 rosettastone 

Linux or Mac 
$ docker run --user root -d -p 8000:8000 rosettastone
```

To run using a ```rosettastone.json``` from your filesystem (or external storage) use the following.  Note that the first parameter to ```-v``` is the path to the file outside of the container image and the second parameter is the path within the image.  The app is in ```/app``` so the path will need to reflect that.
```
Windows
$ docker run --user ContainerAdministrator -p 8000:8000 -v /[PathOnLocalFilesystem]/rosettastone.json:/app/rosettastone.json rosettastone

Linux or Mac 
$ docker run --user root -p 8000:8000 -v /[PathOnLocalFilesystem]/rosettastone.json:/app/rosettastone.json rosettastone
```

5) Connect to RosettaStone in your browser: 
```
http://localhost:8000
```

6) Get the container name:
```
$ docker ps
CONTAINER ID        IMAGE               COMMAND                  CREATED              STATUS              PORTS                 
3627b4e812fd        rosettastone        "dotnet RosettaSt..."    About a minute ago   Up About a minute   0.0.0.0:8000->8000/tcp
```

7) Attach to the container's shell:
```
$ sudo docker exec -it [CONTAINER ID] /bin/bash
```

8) Kill a running container:
```
$ docker kill [CONTAINER ID]
```

9) Delete a container image:
```
$ docker rmi [IMAGE ID] -f
```

## Example rosettastone.json File

Notice in the ```rosettastone.json``` example provided below that:

- ```EnableConsole``` and ```Logging.ConsoleLogging``` are false, so it is safe to detach using ```-d``` in ```docker run```
- An external ```MySql``` database is being used, so object metadata will persist even when the container is terminated
- External storage is used for object data, so object data will persist even when the container is terminated
- The ```Webserver.DnsHostname``` property is set to ```*```.

```
{
  "EnableConsole": false,
  "Webserver": {
    "DnsHostname": "*",
    "Port": 8000,
    "Ssl": false,
    "AdminApiKeyHeader": "x-api-key",
    "AdminApiKey": "rosettastoneadmin"
  },
  "Logging": {
    "SyslogServerIp": "127.0.0.1",
    "SyslogServerPort": 514,
    "MinimumSeverity": 1,
    "ConsoleLogging": false,
    "ConsoleColors": false,
    "LogDirectory": "./logs/",
    "LogFilename": "rosettastone.log"
  },
  "Database": { 
    "Type": "Mysql",
    "Hostname": "localhost",
    "Port": 3306,
    "Username": "root",
    "Password": "[redacted]",
    "DatabaseName": "rosettastone"
  }
}
```