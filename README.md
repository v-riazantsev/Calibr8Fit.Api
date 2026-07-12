# Calibr8Fit - Backend API

Backend API for the Calibr8Fit fitness and lifestyle mobile application.

The service provides RESTful endpoints and real-time communication features for fitness tracking, social interactions, messaging, and user-generated content.

## Project Overview

- **REST API Endpoints:** 120+
- **Database Tables:** 25+
- **Real-time Messaging:** SignalR

## Tech Stack

- ASP.NET Core Web API (.NET 9)
- Entity Framework Core
- PostgreSQL
- SignalR
- JWT Authentication
- Firebase Cloud Messaging (FCM)
- Scalar API Documentation

## Features

### Authentication & Users
- JWT-based authentication
- User profile management
- User settings management

### Fitness & Lifestyle
- Nutrition tracking
- Hydration tracking
- Weight logging
- Exercise logging
- Custom food creation
- Custom meal creation
- Custom exercise creation
- Daily plan creation

### Social Features
- User posts
- Likes and comments
- Friend relationships
- User subscriptions

### Communication
- Real-time messaging
- Push notifications

## Architecture & Design Patterns

The project follows a layered architecture approach.

Implemented patterns:

- **DTO Pattern** - separates API contracts from database entities
- **Repository Pattern** - abstracts database operations from business logic
- **Service Layer Pattern** - contains application business logic
- **Dependency Injection** - manages application dependencies

## Project Structure

```
├── Controllers/             # API endpoints
|   ├── Abstract/            # Base controller classes
├── Data/                    # EF Core DbContext
├── DataTransferObjects/     # API request and response models
├── Enums/                   # Enum definitions
├── Extensions/              # Extension methods and service registrations
├── Hubs/                    # SignalR hubs
├── Interfaces/              # Contracts for services, repositories, models
├── Mappers/                 # Entity to DTO mappings
├── Migrations/              # EF Core database migrations
├── Models/                  # Database entities
├── Options/                 # Configuration models
├── Repository/              # Database access layer
├── Services/                # Business logic layer
├── Validators/              # Request validation
├── Program.cs               # Application configuration
└── README.md
```

## Environment Configuration

Example `.env`:

```env
DefaultConnection="Host=localhost;Port=5432;Database=calibr8fitdb;Username=calibr8fit;Password=***"

JWT_ISSUER=http://localhost:5246
JWT_AUDIENCE=http://localhost:5246
JWT_SIGNING_KEY=LONGRANDOMSIGNINGKEY

FirebaseCredentialPath=credentials/***.json
```

## Running the Project

```bash
dotnet run --urls "http://127.0.0.1:5054"
```

## Database Schema

<img width="1921" height="2368" alt="Untitled" src="https://github.com/user-attachments/assets/ad449d51-298b-49cb-8403-075ba39fa7d0" />

## Related Repositories

- [React Native Frontend](https://github.com/v-riazantsev/Calibr8Fit)
