# Anemoi_Open

**Anemoi_Open** is an open-source microservices project built with modern architectural patterns, designed to provide a scalable and robust foundation for microservices-based applications. This project incorporates **CQRS (Command Query Responsibility Segregation)**, **Event-Driven Architecture (EDA)**, and **Saga Orchestration** patterns. Additionally, it features **Attribute-based Data Mapping**, simplifying data handling across services and enhancing maintainability.

## 🌟 Project Highlights

- **CQRS Pattern**: Separates command (write) and query (read) responsibilities, improving scalability and performance.
- **Event-Driven Architecture (EDA)**: Uses an event-based model for communication between services, reducing dependencies and enabling asynchronous operations.
- **Saga Orchestration**: Manages distributed transactions across multiple services to ensure data consistency.
- **Attribute-based Data Mapping**: Streamlines data mapping across services using custom attributes, reducing repetitive code and improving readability.

## 🛠 Technology Stack

- **.NET Core**: For backend microservices.
- **Docker**: Containerization for easy deployment and scaling.
- **RabbitMQ**: Message brokers to facilitate event-driven communication.
- **Entity Framework Core**: ORM for seamless data handling.
- **AutoMapper**: Simplifies object mapping between layers.

## 📂 Repository Structure

The project is organized as follows:

```plaintext
├── /Anemoi                     # Source code for each microservice
│   ├── Anemoi.BuildingBlocks   # Core code base
│   ├── Anemoi.Centralize       # Aggreagted and controllers
│   ├── Anemoi.Contract         # DTOs and Ids
│   ├── Anemoi.Grpc             # Gprc
│   ├── Anemoi.Identity         # An Identity service
│   ├── Anemoi.MasterData       # For the master data
│   ├── Anemoi.Notification     # Email service
│   ├── Anemoi.Orchestrator     # Saga Orchestration
│   ├── Anemoi.Secure           # Secure service
│   ├── Anemoi.Workspace        # The Workspace service

🚀 Getting Started
Prerequisites
.NET Core SDK
Docker
Rabbitmq: https://hub.docker.com/r/masstransit/rabbitmq for event handling

🌐 Usage
CQRS Pattern: Commands and queries are separated for scalable data management.
EDA: Events are published by producers and consumed by subscribers within the microservices.
Saga Orchestration: Manages complex transactions across multiple services to ensure data consistency.

🤝 Contributing
Contributions are welcome! To get involved:

Fork the repository
Create a new branch (git checkout -b feature-branch)
Commit your changes (git commit -m 'Add new feature')
Push to the branch (git push origin feature-branch)
Open a pull request

🔗 Connect
Thank you for your interest in Anemoi_Open! Feel free to reach out or contribute—let's build something amazing together!
