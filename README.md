# Nambaya Project

This is the central repository for the Nambaya project, a comprehensive system with a microservices architecture.

## Technologies Used

This project utilizes a variety of technologies to deliver a robust and scalable solution:

### Backend
- **C# .NET:** The backend is built on the .NET framework, using a microservices architecture.
- **Docker:** All services are containerized using Docker for easy deployment and scaling.

### Frontend
- **Angular:** The main web application is developed using Angular.
- **HTML/CSS:** A separate static homepage is also included.

### Testing
- **Java:** Automated tests are written in Java using Maven.

### DevOps
- **Jenkins:** A Jenkinsfile is included for continuous integration and deployment.

## Project Structure

The project is organized into the following main directories:

- `Automation/`: Contains automated tests for the system.
- `HomePage/`: Contains a static homepage.
- `Presentation/`: Contains the main Angular web application.
- `Services/`: Contains the backend microservices.
- `Solutions/`: Contains the Visual Studio solution files for the .NET projects.
- `Deployment/`: Contains Docker Compose files for production deployment.

## Getting Started

To get started with this project, you will need to have the following installed:

- .NET SDK
- Node.js and npm
- Docker and Docker Compose
- Java Development Kit (JDK)
- Maven

Once you have the prerequisites installed, you can follow these steps:

1. **Clone the repository:**
   ```bash
   git clone https://github.com/wasimalam/nambaya.git
   ```
2. **Build the .NET solution:**
   - Open the `Solutions/Complete/nambaya.sln` file in Visual Studio and build the solution.
3. **Install frontend dependencies:**
   ```bash
   cd Presentation/webApp
   npm install
   ```

## Deployment

The application can be deployed using the Docker Compose file located in the `Deployment` directory.

### Prerequisites

- Docker and Docker Compose
- An environment file (`.env`) configured with the necessary variables. See the `docker-compose.yml` file in the `Deployment` directory for the required variables.

### Running the Application

1. Navigate to the `Deployment` directory:
   ```bash
   cd Deployment
   ```
2. Run the following command to start the application:
   ```bash
   docker-compose up -d
   ```

