# ExperimentPlatformApi

A .NET 9 Web API for managing A/B experiments and tracking user events asynchronously using RabbitMQ for distributed message processing.

## Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              Client Requests                                │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  ExperimentPlatform (ASP.NET Core Web API)                                  │
│  ├── Controllers                                                            │
│  │   ├── ExperimentsController  → Create experiments, assign variants       │
│  │   └── EventsController        → Track events (async via RabbitMQ)        │
│  └── Program.cs                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                    ┌─────────────────┼─────────────────┐
                    ▼                 ▼                 ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  ExperimentPlatformApplication (Business Logic Layer)                       │
│  ├── CreateExperimentHandler                                                │
│  ├── AssignVariantHandler                                                  │
│  └── TrackEventHandler ──────────────────────► IBackgroundQueue            │
└─────────────────────────────────────────────────────────────────────────────┘
                                                                               │
                                                                               ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  ExperimentPlatformInfrastructure (Data Access & Background Processing)     │
│  ├── Persistence (EF Core + PostgreSQL)                                     │
│  │   ├── ExperimentRepository                                               │
│  │   ├── EventRepository                                                    │
│  │   └── ExperimentDbContext                                               │
│  └── Background                                                            │
│      ├── RabbitMqBackgroundQueue ──────────────────────────────┐           │
│      └── InMemoryBackgroundQueue (fallback)                    │           │
└────────────────────────────────────────────────────────────────┼───────────┘
                                      │                           │
                                      │                           ▼
                                      │              ┌────────────────────────┐
                                      │              │  RabbitMQ Broker       │
                                      │              │  ├── Exchange          │
                                      │              │  ├── Queue (durable)   │
                                      │              │  └── Consumer          │
                                      │              └────────────────────────┘
                                      ▼                           │
                    ┌──────────────────────────────────────────────┘
                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  PostgreSQL Database                                                        │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Project Structure

| Project                            | Purpose                                            |
| ---------------------------------- | -------------------------------------------------- |
| `ExperimentPlatform`               | Web API entry point (controllers, middleware)      |
| `ExperimentPlatformApplication`    | Use case handlers and abstractions                 |
| `ExperimentPlatformDomain`         | Entities, interfaces, enums, value objects         |
| `ExperimentPlatformInfrastructure` | EF Core repositories, PostgreSQL, background queue |
| `ExperimentPlatformWorker`         | Background service for async processing            |

## Key Features

- **Experiments CRUD**: Create A/B experiments with weighted variants
- **Variant Assignment**: Deterministic user-to-variant mapping
- **Event Tracking**: Asynchronous event ingestion via RabbitMQ
- **Message Broker**: RabbitMQ integration with durable queues and persistent messages
- **Distributed Processing**: Decoupled event publishing and consumption
- **Reliable Delivery**: Automatic acknowledgment with retry on failure (requeue)
- **Background Processing**: RabbitMQ consumer processes queued events in order
- **PostgreSQL**: Persistent storage with Entity Framework Core
- **Docker Ready**: Multi-container setup with `docker-compose` (API, PostgreSQL, RabbitMQ)

## API Endpoints

| Method | Endpoint                       | Description                |
| ------ | ------------------------------ | -------------------------- |
| `POST` | `/api/experiments`             | Create a new experiment    |
| `POST` | `/api/experiments/{id}/assign` | Assign a variant to a user |
| `POST` | `/api/experiments/{id}/events` | Track an event             |
| `GET`  | `/health`                      | Health check               |

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Run Locally

```bash
# Start PostgreSQL and RabbitMQ
docker compose up db rabbitmq -d

# Run the API
cd ExperimentPlatform && dotnet run
```

The API will be available at `https://localhost:8081` (Swagger at `/swagger` in Development).
RabbitMQ Management UI will be available at `http://localhost:15672` (guest/guest).

### Run with Docker Compose

```bash
# Set the database connection string
$env:ConnectionStrings__db = "Host=localhost;Port=5434;Database=experiments;Username=postgres;Password=BaraoLaika"

docker compose up --build
```

## Configuration

Connection strings and queue settings are managed via environment variables and `appsettings.json`:

### Database Configuration

| Variable                | Default                        | Description                  |
| ----------------------- | ------------------------------ | ---------------------------- |
| `ConnectionStrings__db` | `Host=localhost;Port=5434;...` | PostgreSQL connection string |

### RabbitMQ Configuration

Configure RabbitMQ settings in `appsettings.Development.json`:

```json
{
  "QueueSettings": {
    "Type": "RabbitMQ"
  },
  "RabbitMQ": {
    "HostName": "rabbitmq",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "QueueName": "experiment-events",
    "ExchangeName": "experiment-exchange",
    "RoutingKey": "event.tracked"
  }
}
```

### Queue Implementation

The system supports two queue implementations:
- **RabbitMQ** (default): Distributed, persistent message queue with automatic retry
- **InMemory**: Simple in-memory queue for development/testing (fallback)

Set `QueueSettings:Type` to switch between implementations.

## RabbitMQ Integration Details

### Message Flow

1. **Event Publishing**: When an event is tracked via `/api/experiments/{id}/events`, the `TrackEventHandler` serializes the event and publishes it to RabbitMQ
2. **Exchange & Routing**: Messages are published to a durable exchange (`experiment-exchange`) with routing key (`event.tracked`)
3. **Queue**: The exchange routes messages to a durable queue (`experiment-events`)
4. **Consumer**: `RabbitMqBackgroundQueue` consumes messages asynchronously
5. **Processing**: Each message is deserialized and persisted to PostgreSQL via `IEventRepository`
6. **Acknowledgment**: Successfully processed messages are acknowledged; failed messages are requeued for retry

### Key Benefits

- **Durability**: Messages persist across restarts
- **Reliability**: Automatic requeue on processing failures
- **Scalability**: Multiple consumers can process messages in parallel
- **Decoupling**: API responds immediately without waiting for database writes
- **Observability**: RabbitMQ Management UI for monitoring queue depth and throughput

### RabbitMQ Settings

| Setting        | Description                                  | Default                  |
| -------------- | -------------------------------------------- | ------------------------ |
| `HostName`     | RabbitMQ broker hostname                     | `rabbitmq`               |
| `Port`         | AMQP protocol port                           | `5672`                   |
| `UserName`     | Authentication username                      | `guest`                  |
| `Password`     | Authentication password                      | `guest`                  |
| `QueueName`    | Durable queue for events                     | `experiment-events`      |
| `ExchangeName` | Direct exchange for routing                  | `experiment-exchange`    |
| `RoutingKey`   | Routing key for event messages               | `event.tracked`          |

## Future Improvements

- **Dead Letter Queues**: Route permanently failed messages to DLQ for manual inspection
- **Retry Policies**: Exponential backoff with maximum retry attempts
- **Metrics**: Prometheus metrics for queue depth, processing latency, and error rates
- **Event Sourcing**: Store events as an immutable log for analytics and replay
- **Multiple Consumers**: Scale horizontally with multiple worker instances
- **Circuit Breaker**: Prevent cascading failures when database is unavailable

## Tech Stack

- **Framework**: ASP.NET Core 9
- **ORM**: Entity Framework Core 9
- **Database**: PostgreSQL 16
- **Message Broker**: RabbitMQ 3 with Management Plugin
- **Client Library**: RabbitMQ.Client
- **Serialization**: `System.Text.Json`
- **Background Processing**: RabbitMQ Consumer with `AsyncEventingBasicConsumer`
