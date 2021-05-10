# H2020 IPM Decisions User Provision Service

APS.NET Core service in charge to authenticate and authorise users and clients that can access the H2020 IPM Decisions API Gateway.

## Branch structure

The project development will follow [Git Flow](https://nvie.com/posts/a-successful-git-branching-model/) branching model where active development will happen in the develop branch. Master branch will only have release ans stable versions of the service.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

The User Provision Service uses the following technologies:

```
ASP.NET Core 3.1.101
PostgreSQL 12.2
```

1. [Install](https://dotnet.microsoft.com/download/dotnet-core/3.1) the required .NET Core SDK.

2. [Install](https://www.postgresql.org/download/) the required PostgreSQL database.

### Getting the solution

The easiest way to get the sample is by cloning the samples repository with [git](https://git-scm.com/downloads), using the following instructions. As explained above, the develop branch is an active development branch, so checkout this branch to get the latest version.

```console
git clone https://github.com/H2020-IPM-Decisions/UserProvisionService.git

cd UserProvisionService

git checkout develop
```

You can also [download the repository as a zip](https://github.com/H2020-IPM-Decisions/UserProvisionService/archive/develop.zip).

### How to build and download dependencies

You can build the tool using the following commands. The instructions assume that you are in the root of the repository. You will need to run the commands from the API project.

```console
cd H2020.IPMDecisions.UPR.API

cp appsettingsTemplate.json appsettings.json

dotnet build
```

### Setting up PostgreSQL with PostGIS Extension database on Docker

This instructions are simplified as they are for testing proposes, please follow official instructions for detailed information.

```console
docker run --name postgresdev -d -p 5432:5432 -e POSTGRES_PASSWORD=postgres postgis/postgis:12-2.5-alpine
```

Your `ConnectionStrings\MyPostgreSQLConnection` will be `Host=127.0.0.1;Port=5432;Database=H2020.IPMDecisions.UPR;Username=postgres;Password=postgres`

Use pgAdmin4 to manage your DB

```
docker run -p 5555:80 --name pgadmin4 --env PGADMIN_DEFAULT_EMAIL=admin@test.com --env PGADMIN_DEFAULT_PASSWORD=admin -d dpage/pgadmin4
```

### How to connect and start the database

Open file `H2020.IPMDecisions.UPR.API\appsettings.json` and change the json object `ConnectionStrings\MyPostgreSQLConnection` with your PostgreSQL instance.
The following command will create a database and add all the tables necessary to run the solution.
The instructions assume that you are in the **API project** of the repository.

```console
dotnet ef database update
```

Open your PostgreSQL instance and check that the database has been created and tables added.

If new tables are added to the project using [CodeFirst approach](https://entityframeworkcore.com/approach-code-first), to add new migrations to the database run these commands:

```console
dotnet ef migrations add YourMessage --project ..\H2020.IPMDecisions.UPR.Data\H2020.IPMDecisions.UPR.Data.csproj
dotnet ef database update
```

### How to create backups to use on Docker-compose init scrip

```console
docker exec -u postgres <containerName> pg_dump --file "/var/lib/postgresql/data/dbBackup.sql" --username "yourUserName" --no-password --verbose --format=p --blobs --no-owner --section=pre-data --section=post-data --no-privileges --no-tablespaces --schema public "H2020.IPMDecisions.UPR"
```

The following command will copy the backup into your local machine. Add this file into the `Docker\UPR_Postgresql_Init_Script` folder with the name `2.dbBackup.sql`

```console
docker cp <containerName>:/var/lib/postgresql/data/dbBackup.sql your\local\folder\path\2.dbBackup.sql
```

### How to set-up the JsonWebToken (JWT) provider

Open file `H2020.IPMDecisions.UPR.API\appsettings.json` and change the json section `JwtSettings` with the your server information.

1. SecretKey: This parameter holds a secret key to sign the JWT. Your resource API should have the same secret in the JWT properties.
2. IssuerServerUrl: This parameter holds who is issuing the certificate, usually will be this server. Your resource API should have the same issuer url in the JWT properties.
3. ValidAudiences: This parameter holds which clients URLs can use this UPR service. The different URLS should be separated by a semicolon **";"**. At least one of the client URL should be added into your resource API JWT properties.

### How to run the project

You can build the tool using the following commands. The instructions assume that you are in the root of the repository.
As explained above, the develop branch is an active development branch.

```console
dotnet run
```

The solution will start in the default ports (https://localhost:5001;http://localhost:5000) defined in the file. `H2020.IPMDecisions.UPR.API\Properties\launchSettings.json`

### How to set-up CORS policy

Open file `H2020.IPMDecisions.UPR.API\appsettings.json` and change the json section `AllowedHosts` with the host that you would like to allow to consume the API.
The different URLS should be separated by a semicolon **";"**. If you would like to allow any origin, write an asterisk on the string **"\*"**

# Creating a Docker Image locally

A Docker file has been created to allow to build and run the image in your preferred server. Before building your image ensure that your 'appsettings.json' is configured correctly.

---

**NOTE**
This docker build image doesn't include the "PostgreSQL" database.

---

Remember to change the **EXPOSE** ports in the `Dockerfile` if the default ports are taken (80 and 443).
The following commands assumes that you are in the root directory of the application.

- The image created will be called: `h2020.ipmdecisions.userprovisionservice`
- The container created will be called `UPR` and will be running in the port `5006`
- The command bellow assumes that the URL port `H2020.IPMDecisions.UPR.API\Properties\launchSettings.json` is 5006

```Console
docker build . --rm --pull --no-cache  -f ".\Docker\Dockerfile" -t "ipmdecisions/userprovisionservice:latest" --build-arg BUILDER_VERSION=latest

docker run  -d -p 443:443/tcp -p 5006:5006/tcp --name UPR ipmdecisions/userprovisionservice:latest
```

Now you should be able to user your API in the docker container.

## Deployment with Docker Compose

You can deploy the User Provision Service API, including a PostgreSQL database with the database structure and a pgAdmin4 UI to manage the database, using a docker compose.
A file called `docker-compose.yml` is located in the following folder `Docker` locate in the root of the project.
To run the following command:

```console
docker-compose -f "./Docker/Docker-compose.yml" up -d
```

If no data have been modified in the `docker-compose.yml` the solution will be working in the URL `localhost:5006`.

## Versioning

For the versions available, see the [tags on this repository](https://github.com/H2020-IPM-Decisions/IdentityProviderService/tags).

## Authors

- **ADAS Modelling and Informatics** - _Initial work_ - [ADAS](https://www.adas.uk/)
