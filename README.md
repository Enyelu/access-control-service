# Microservices Architecture

This repository contains the code and documentation for API Gateway services. Each microservices (API Gateway, Access Control and User Management ) is hosted in its own repository, and the overall architecture leverages modern technologies and patterns for a scalable and maintainable solution.

## Technologies Used

- **Clean Architecture**: Implements principles of Clean Architecture for separation of concerns and maintainability.
- **CQRS (Command Query Responsibility Segregation)**: Separates command and query operations to enhance performance and scalability.
- **Serilog with Seq**: Uses Serilog for structured logging and Seq for log aggregation. (Install Seq on your local machine if you want to run it locally or replace the seq url in the appsettings with remote url.)
- **JWT Authentication**: JSON Web Tokens are used for secure authentication. Tokens are generated by the API Gateway and validated by the microservices.
- **Hangfire**: Configured in the Access Control service to run a job every 5 minutes that handles locking open locks.
- **YARP (Yet Another Reverse Proxy)**: Utilized in the API Gateway service to route requests to the appropriate microservices.

## Repositories

- **Access Control Service**: [https://github.com/Enyelu/access-control-service]
- **User Management Service**:[https://github.com/Enyelu/user-management]
- **API Gateway Service**: [https://github.com/Enyelu/gateway]

## Installation and Setup

### Prerequisites

1. **.NET 8 SDK**: Ensure the .NET 8 SDK is installed. Download it from the [official .NET website](https://dotnet.microsoft.com/download).
2. **Seq**: To run Serilog locally, install Seq. Instructions can be found in the [Seq documentation](https://docs.datalust.co/docs/getting-started).
3. **SqlServer** Download from nuget package manged or use any database of your choice.

### Setting Up Each Microservice

1. **Clone the Repositories**

   ```bash
   git clone https://github.com/Enyelu/access-control.git
   git clone https://github.com/Enyelu/user-management.git
   git clone https://github.com/Enyelu/api-gateway.git
