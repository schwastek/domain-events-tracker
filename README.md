# Domain Events Tracker

A C# .NET solution demonstrating **Domain Events**, **Entity Change Tracking**, and a simple **playground API** to test behavior - all layered with EF Core and MediatR.

🚀 Just want to see how it works?

Jump into [`UserDomainEventsTests`](./UnitTests/Domain/Users/UserDomainEventsTests.cs) or [`EntityChangeTrackerTests`](./UnitTests/Domain/Events/Core/ChangeTracking/EntityChangeTrackerTests.cs) - they show everything in action with real use cases.

## Getting started

### Prerequisites

Make sure you have installed the following prerequisites:

* [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet)
```sh
dotnet --version
# => 8.0.0 or later 
```

* [EF Core CLI tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)
```sh
dotnet ef --version
# => 9.0.0
```

* [Docker](https://www.docker.com/)
```sh
docker --version
# => 28.1.1
```

### Installation

1. Clone the repository
```sh
git clone https://github.com/schwastek/domain-events-tracker.git
```

2. Set up environment variables
* Copy the `.env.example` file and rename it to `.env`
* Update SQL Server credentials

3. Set up SQL Server instance
```sh
docker compose up -d
```

This will create and run a SQL Server instance in a container.

#### Docker Compose commands
* `docker-compose up -d` - start (runs in background)
* `docker-compose down` - stop (keep data)
* `docker-compose down -v` - stop and delete all data (volumes)

4. Update connection string
* Modify `appsettings.json` in the `Api` and `Data.Migrations` projects
* Replace the connection string with one matching your `.env` settings

5. Apply database migrations
```sh
cd Data.Migrations
dotnet ef database update
```

### Running application

1. Start the API
```sh
cd Api
dotnet run
```

2. Ensure SQL Server is running
* The SQL Server container should already be running from `docker compose`
* You can check with:
```sh
docker ps
```

3. Access Swagger UI
* Open your browser at https://localhost:7027/swagger/index.html 
* Use Swagger to explore and test the available API endpoints

## Project overview

### Core components

* Domain Entities: `User`, `Authentication`, `AccessRight`, `Application`
    * Each has encapsulated setters and tracking capabilities.
    * They record changes (modifications, creations, removals) via domain events.

* DomainEvents class:
    * Attaches to entities to register domain events.
    * Provides utility methods like `AddOnce`, `AddOrReplaceAll` etc. These ensure correct merging and help prevent duplicate or outdated domain events.

* EntityChangeTracker class:
    * Attaches to entities to track property and collection modifications.
    * Ensures that events are **merged intelligently** (only one change per member).
    * Uses `IEqualityComparer` to determine when a new event replaces or updates an existing one.

### Event types

* **EntityCreatedEvent:** Signals entity creation.
* **EntityChangedEvent:** Signals entity modification.
* **MemberChangedEvent / CollectionChangedEvent:** Events for member modifications.
* **ToString() overrides on each:** Provide detailed, human-readable summaries like:
```
User: 26ab6263-d6b8-473c-8c66-d80eab868f63 created.
AccessRights changed: added [AccessRight: [Application: Galactus, User: ..., App User ID: xMAy3X]];
removed []. 
Authentication changed from null to Authentication [Username: mmEEjR].
```

### Event publishing

* A custom middleware/filter (`TransactionFilter`) commits events to the DB **before** saving changes.
* This ensures that **event logs** are persisted **in the same transaction** as underlying data changes.

### Event flow summary

1. **User is created** → emits `EntityCreatedEvent<User>`.
2. **Authentication is added or changed** → emits `PropertyChanged<Authentication>`.
3. **AccessRight is added or changed** → emits `CollectionChangedEvent<AccessRight>`.
4. **Events are merged and tracked** via `DomainEvents` and `EntityChangeTracker`.
5. **Before saving**, events are published and audit messages are persisted in the same transaction.

### Developer notes

* **ToString() with navigation properties**  
If your event's `ToString()` accesses nested relationships and EF didn't eagerly load them, you'll get nulls in the event's description.

* **EF nulls navigation properties after SaveChanges**  
When an entity is removed, EF sets navigation properties to `null` after `SaveChanges`.

* **EF always calls parameterless constructor**  
Even if an entity already exists in the DB, EF initializes it using the parameterless constructor before setting any properties like `Id`. Therefore, any constructor side-effects (like raising domain events) fire on every load.

* **Only the latest change should exist for a given member**  
When comparing or merging change events (e.g. changing the same property multiple times), default equality isn't enough. Change events like `PropertyChangedEvent` or `CollectionChangedEvent` must implement proper equality logic (`IEquatable<T>` or `IEqualityComparer<T>`) to allow merging and avoid duplicates.

* **Always compare by consistent identity**  
Use immutable, unique identifiers (e.g. database ID or natural key) in `GetHashCode`. For example, compare `Application` by `Code` (it never changes), but do not compare `Authentication` by `Username` - it's unique, but modifiable.

* **Removing items from a list requires index updates**  
If you use a `List<DomainEvent>`, removing events by type would require shifting all subsequent indices in the tracking structure. Use `LinkedList<DomainEvent>` for efficient removals and updates.

* **Avoid using GetType() to track or replace domain events**  
Avoid type-based logic via reflection.

## Resources

* [Domain events: Design and implementation (microsoft.com)](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation)
* [CSharpFunctionalExtensions by Vladimir Khorikov (github.com)](https://github.com/vkhorikov/CSharpFunctionalExtensions/blob/master/CSharpFunctionalExtensions/Entity/Entity.cs)
* ["Entity Base Class" by Vladimir Khorikov (enterprisecraftsmanship.com)](https://enterprisecraftsmanship.com/posts/entity-base-class/)
