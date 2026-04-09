# ExperimentPlatformApi

A.NET 9 Web API for managing A/B experiments and tracking user events asynchronously using an in-memory background queue.

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
│  │   └── EventsController        → Track events (async via queue)            │
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
                    │                                                         │
                    ▼                                                         ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  ExperimentPlatformInfrastructure (Data Access & Background Processing)     │
│  ├── Persistence (EF Core + PostgreSQL)                                     │
│  │   ├── ExperimentRepository                                               │
│  │   ├── EventRepository                                                    │
│  │   └── ExperimentDbContext                                               │
│  └── Background                                                            │
│      └── InMemoryBackgroundQueue ──────────► BackgroundQueueWorker          │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
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
- **Event Tracking**: Asynchronous event ingestion via in-memory queue
- **Background Processing**: Hosted service processes queued events in order
- **PostgreSQL**: Persistent storage with Entity Framework Core
- **Docker Ready**: Multi-container setup with `docker-compose`

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
# Start PostgreSQL
docker compose up db -d

# Run the API
cd ExperimentPlatform && dotnet run
```

The API will be available at `https://localhost:8081` (Swagger at `/swagger` in Development).

### Run with Docker Compose

```bash
# Set the database connection string
$env:ConnectionStrings__db = "Host=localhost;Port=5434;Database=experiments;Username=postgres;Password=BaraoLaika"

docker compose up --build
```

## Configuration

Connection strings are managed via environment variables:

| Variable                | Default                        | Description                  |
| ----------------------- | ------------------------------ | ---------------------------- |
| `ConnectionStrings__db` | `Host=localhost;Port=5434;...` | PostgreSQL connection string |

## Future Improvements

- **RabbitMQ Integration**: Replace in-memory background queue with RabbitMQ for persistent, distributed message processing
- **Retry Logic**: Add dead-letter queues and retry policies for failed event processing
- **Metrics**: Prometheus metrics for queue depth and processing latency
- **Event Sourcing**: Store events as an immutable log for analytics

## Tech Stack

- **Framework**: ASP.NET Core 9
- **ORM**: Entity Framework Core 9
- **Database**: PostgreSQL 16
- **Serialization**: `System.Text.Json`
- **Background Processing**: `System.Threading.Channels`
