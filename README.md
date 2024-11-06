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
│   ├── Anemoi.BuildingBlocks   # Core codebase and shared utilities
│   ├── Anemoi.Centralize       # Aggregates services, exposing APIs to external clients
│   ├── Anemoi.Contract         # Data Transfer Objects (DTOs) and shared identifiers
│   ├── Anemoi.Grpc             # gRPC service for high-performance communication between microservices
│   ├── Anemoi.Identity         # Identity management service for authentication and authorization
│   ├── Anemoi.MasterData       # Provides and manages static or master data shared across services
│   ├── Anemoi.Notification     # Manages notifications, particularly email communications
│   ├── Anemoi.Orchestrator     # Orchestrates distributed transactions with Saga pattern
│   ├── Anemoi.Secure           # Provides data encryption, security-related utilities, and compliance tools
│   ├── Anemoi.Workspace        # Workspace management, handling user or resource-specific configurations

Service Details
Anemoi.BuildingBlocks
Contains the foundational code, utilities, and shared components used across all services. This module provides reusable blocks, helping to reduce redundancy and keep the code consistent across the microservices.

Anemoi.Centralize
Acts as the central aggregation layer, exposing a unified API interface for external consumers. This service can aggregate data from multiple microservices and acts as a centralized access point, making it easier for clients to interact with the system.

Anemoi.Contract
Contains Data Transfer Objects (DTOs) and shared IDs that define how data is transferred between services. This module standardizes data structures across the system, ensuring smooth and consistent communication between services.

Anemoi.Grpc
A gRPC-based service for efficient, high-performance communication between microservices. gRPC is used to facilitate fast, binary-encoded communication, making it ideal for inter-service communication where speed and efficiency are priorities.

Anemoi.Identity
Manages authentication and authorization, ensuring secure access to the system. This service is responsible for user management, access control, and identity verification, playing a crucial role in the overall security architecture.

Anemoi.MasterData
Maintains static or master data used across the application. This could include reference data, configuration settings, and other data points that do not frequently change but are accessed by multiple services.

Anemoi.Notification
Handles email notifications and other communication tasks. This service is responsible for sending alerts, updates, and notifications to users, using email or potentially other channels.

Anemoi.Orchestrator
Implements Saga Orchestration to manage distributed transactions across microservices. By coordinating workflows and transactions, it ensures data consistency and reliable outcomes in complex, multi-service operations.

Anemoi.Secure
Provides security-related functions such as data encryption and compliance with security standards. This service is designed to protect sensitive data, support encryption/decryption, and add security-focused capabilities to the application.

Anemoi.Workspace
Manages user or resource-specific settings, allowing customization and configuration of workspaces. This service is responsible for handling configurations and settings unique to individual users or resources within the system.

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
