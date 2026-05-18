<div align="center">

<img src="https://img.shields.io/badge/FlowBoard-Agile%20Project%20Management-6c63ff?style=for-the-badge&logo=trello" />

# FlowBoard

### A Production-Grade Project Management Platform Built on Microservices

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-17-DD0031?style=flat-square&logo=angular)](https://angular.dev/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?style=flat-square&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-Cache-DC382D?style=flat-square&logo=redis&logoColor=white)](https://redis.io/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=flat-square&logo=docker&logoColor=white)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)

<p>FlowBoard is a modern, scalable project management and collaboration platform (Trello-clone) engineered with a full microservices architecture. It enables users to create workspaces, manage Kanban boards, collaborate on tasks, and organize work at scale.</p>

[Architecture](#-system-architecture) · [Microservices](#-microservices-overview) · [API Docs](#-api-reference) · [Setup](#-getting-started)

---

</div>

## 📌 Table of Contents

- [Tech Stack](#-tech-stack)
- [Architecture Overview](#-system-architecture)
- [UML Diagrams](#-uml-diagrams)
  - [Use Case Diagram](#1-use-case-diagram)
  - [System Architecture](#2-system-architecture-diagram)
  - [Entity Class Diagram](#3-entity-class-diagram)
  - [Task Creation Sequence](#4-task-card-creation-flow)
  - [Angular Component Diagram](#5-angular-frontend-component-diagram)
  - [Inter-Service Communication](#6-inter-service-communication-map)
- [Microservices Overview](#-microservices-overview)
- [Core Features](#-core-features)
- [Database Schema](#-database-schema)
- [API Reference](#-api-reference)
- [Infrastructure](#-infrastructure)
- [Design Patterns](#-key-design-patterns)
- [Getting Started](#-getting-started)
- [Roadmap](#-roadmap)

---

## 🛠 Tech Stack

| Layer | Technology | Purpose |
|-------|------------|---------|
| **Backend** | ASP.NET Core 8 Web API | 8 independent microservices |
| **Frontend** | Angular 17 | Standalone components, Single-page application |
| **Database** | PostgreSQL 16 | Relational data storage with Code-first migrations |
| **ORM** | Entity Framework Core | Database access and modeling |
| **Cache** | Redis | Caching for performance (e.g., Cards/Tasks) |
| **Auth** | JWT (HS256) | Stateless authentication |
| **Gateway** | YARP Reverse Proxy | Single entry point, API routing |
| **Containerization** | Docker & Docker Compose | Local orchestration |
| **Deployment** | Render.com | Cloud production environment |

---

## 🏗 System Architecture

FlowBoard follows a strict **Microservices Architecture** with these core principles:

| Pattern | Applied Where |
|---------|--------------|
| ✅ **Microservices** | 8 independently deployable domain-driven services |
| ✅ **API Gateway** | Single YARP entry point for all frontend traffic |
| ✅ **Repository Pattern** | Clean separation of data access in every microservice |
| ✅ **Service Layer** | Business logic encapsulated within dedicated services |
| ✅ **Distributed Cache** | Redis used for fast retrieval of high-traffic items |
| ✅ **DTO Pattern** | Separate request/response models from database entities |
| ✅ **Standalone Frontend** | Modern Angular architecture without NgModules |

---

## 📐 UML Diagrams

### 1. Use Case Diagram

> All actors and use cases across every module of FlowBoard

```mermaid
graph LR
    GUEST(["👤 Guest"])
    USER(["👤 Team Member"])

    subgraph UC["«system» FlowBoard — Project Management Platform"]

        subgraph AUTH["🔐 Authentication"]
            UC1["Register Account"]
            UC2["Login / Get JWT"]
        end

        subgraph WORKSPACE["🏢 Workspaces"]
            UC3["Create Workspace"]
            UC4["Manage Members"]
        end

        subgraph BOARD["📊 Boards"]
            UC5["Create Kanban Board"]
            UC6["Set Board Visibility"]
        end

        subgraph LIST["📋 Lists (Columns)"]
            UC7["Create List (To Do, Done)"]
            UC8["Reorder Lists"]
        end

        subgraph CARD["📝 Cards (Tasks)"]
            UC9["Create Task Card"]
            UC10["Move Card between Lists"]
            UC11["Assign Members"]
        end

        subgraph DETAILS["🏷️ Card Details"]
            UC12["Add Comments"]
            UC13["Add Color Labels"]
            UC14["Create Checklists"]
        end

    end

    %% Guest
    GUEST --> UC1 & UC2

    %% Registered User
    USER --> UC3 & UC4
    USER --> UC5 & UC6
    USER --> UC7 & UC8
    USER --> UC9 & UC10 & UC11
    USER --> UC12 & UC13 & UC14

    %% Styling
    style GUEST fill:#1565C0,color:#fff,stroke:#1565C0
    style USER fill:#2E7D32,color:#fff,stroke:#2E7D32
```

---

### 2. System Architecture Diagram

> Full deployment view — Client → API Gateway → Microservices → Infrastructure

```mermaid
graph TB
    subgraph CLIENT["🌐 Client Layer"]
        WEB["Angular 17 SPA\nFlowBoard.UI"]
    end

    subgraph GATEWAY["🔀 API Gateway — YARP Reverse Proxy"]
        GW["Routes:\n/api/auth → Auth\n/api/workspaces → Workspace\n/api/boards → Board\n/api/lists → List\n/api/cards → Card\n/api/comments → Comment\n/api/labels → Label\n/api/checklists → Checklist"]
    end

    subgraph SERVICES["⚙️ Microservices Layer"]
        AUTH["Auth.API"]
        WORKSPACE["Workspace.API"]
        BOARD["Board.API"]
        LIST["List.API"]
        CARD["Card.API\n(Redis Cache)"]
        COMMENT["Comment.API"]
        LABEL["Label.API"]
        CHECKLIST["Checklist.API"]
    end

    subgraph INFRA["🗄️ Infrastructure"]
        PG[("PostgreSQL\nRelational Data")]
        REDIS[("Redis\nPerformance Cache")]
    end

    WEB -->|"HTTP REST + JWT"| GW
    GW --> AUTH & WORKSPACE & BOARD & LIST & CARD & COMMENT & LABEL & CHECKLIST

    AUTH & WORKSPACE & BOARD & LIST & CARD & COMMENT & LABEL & CHECKLIST -->|"EF Core (Read/Write)"| PG
    CARD -->|"Cache Get/Set"| REDIS

    style WEB fill:#DD0031,color:#fff,stroke:#DD0031
    style GW fill:#6c63ff,color:#fff,stroke:#6c63ff
    style PG fill:#336791,color:#fff,stroke:#336791
    style REDIS fill:#DC382D,color:#fff,stroke:#DC382D
```

---

### 3. Entity Class Diagram

> Domain model mapping the core hierarchical structure of FlowBoard

```mermaid
classDiagram
    direction TB

    class User {
        +int UserId PK
        +string Username
        +string Email
        +string PasswordHash
    }

    class Workspace {
        +int WorkspaceId PK
        +string Name
        +string Description
        +int OwnerId FK
    }

    class Board {
        +int BoardId PK
        +int WorkspaceId FK
        +string Name
        +string Visibility
        +int OwnerId FK
    }

    class List {
        +int ListId PK
        +int BoardId FK
        +string Title
        +int Position
    }

    class Card {
        +int CardId PK
        +int ListId FK
        +string Title
        +string Description
        +int Position
    }

    class Comment {
        +int CommentId PK
        +int CardId FK
        +int UserId FK
        +string Text
    }

    class Label {
        +int LabelId PK
        +int BoardId FK
        +string Name
        +string ColorHex
    }

    class Checklist {
        +int ChecklistId PK
        +int CardId FK
        +string Title
    }

    class ChecklistItem {
        +int ItemId PK
        +int ChecklistId FK
        +string Content
        +bool IsCompleted
    }

    User "1" --> "0..*" Workspace : owns/member of
    Workspace "1" --> "0..*" Board : contains
    Board "1" --> "0..*" List : contains
    List "1" --> "0..*" Card : contains
    Card "1" --> "0..*" Comment : has
    Card "1" --> "0..*" Checklist : has
    Board "1" --> "0..*" Label : defines
    Checklist "1" --> "0..*" ChecklistItem : contains
```

---

### 4. Task (Card) Creation Flow

> Sequence diagram — How a card is created and cached

```mermaid
sequenceDiagram
    actor User
    participant FE as Angular Frontend
    participant GW as YARP Gateway
    participant CARD as Card.API
    participant DB as PostgreSQL
    participant REDIS as Redis Cache

    User->>FE: Click "Add Card"
    FE->>GW: POST /api/cards (JWT + Title + ListId)
    GW->>CARD: Route request

    CARD->>CARD: Validate JWT & Data
    CARD->>DB: Insert Card Entity
    DB-->>CARD: Save Changes (CardId generated)
    
    CARD->>REDIS: Invalidate/Update List Cache
    REDIS-->>CARD: Cache Updated

    CARD-->>GW: 201 Created {CardDto}
    GW-->>FE: 201 Created
    FE-->>User: ✅ Card appears in List
```

---

### 5. Angular Frontend Component Diagram

> Standalone component architecture and routing

```mermaid
graph LR
    subgraph GUARDS["🛡️ Guards"]
        AG["AuthGuard"]
    end

    subgraph INTERCEPTORS["⚡ Interceptors"]
        AI["AuthInterceptor\n(Attaches JWT)"]
    end

    subgraph COMPONENTS["📄 Standalone Components"]
        LOGIN["LoginComponent\n/login"]
        REG["RegisterComponent\n/register"]
        DASH["DashboardComponent\n/workspaces 🔒"]
        WORKSPACE["WorkspaceDetailComponent\n/workspace/:id 🔒"]
        BOARD["BoardComponent\n/board/:id 🔒"]
        CARD_MODAL["CardDetailModal\n(Dialog)"]
    end

    subgraph SERVICES["🔌 API Services"]
        AS["AuthService"]
        WS["WorkspaceService"]
        BS["BoardService"]
        LS["ListService"]
        CS["CardService"]
        XTRAS["Comment/Label/Checklist Services"]
    end

    AG --> DASH & WORKSPACE & BOARD
    AI -.->|Intercepts| AS & WS & BS & LS & CS & XTRAS

    DASH --> WS
    WORKSPACE --> WS & BS
    BOARD --> BS & LS & CS
    CARD_MODAL --> CS & XTRAS

    style AG fill:#DD0031,color:#fff,stroke:#DD0031
    style AI fill:#c2185b,color:#fff,stroke:#c2185b
    style BOARD fill:#1976D2,color:#fff,stroke:#1976D2
    style CARD_MODAL fill:#0288D1,color:#fff,stroke:#0288D1
```

---

### 6. Inter-Service Communication Map

> In FlowBoard, services are highly decoupled. Most communication is orchestrated by the client or Gateway, but services share standard JWT validation.

```mermaid
graph LR
    GW["YARP Gateway"]
    AUTH["Auth.API"]
    WORKSPACE["Workspace.API"]
    BOARD["Board.API"]
    CARD["Card.API"]

    GW -->|"Routes /api/auth"| AUTH
    GW -->|"Routes /api/workspaces"| WORKSPACE
    GW -->|"Routes /api/boards"| BOARD
    GW -->|"Routes /api/cards"| CARD
    
    note["Services validate JWT signatures independently using the shared secret key. Data is isolated per service domain."]
```

---

## 📦 Microservices Overview

| Service | Responsibility | Port (Local) |
|---------|----------------|:------------:|
| **Auth.API** | User registration, authentication, JWT generation | 5001 |
| **Workspace.API** | Workspace creation, member management | 5002 |
| **Board.API** | Board creation, visibility, workspace linkage | 5003 |
| **List.API** | Kanban columns (To Do, In Progress, etc.) | 5004 |
| **Card.API** | Tasks, positioning, descriptions, Redis caching | 5005 |
| **Comment.API** | User discussions on specific cards | 5006 |
| **Label.API** | Color-coded tags for organization | 5007 |
| **Checklist.API** | Sub-tasks and completion tracking | 5008 |
| **Gateway** | YARP Reverse proxy routing all traffic | 5000 |

---

## 🎯 Core Features

- **Authentication:** Secure JWT-based login and registration.
- **Hierarchical Organization:** Workspaces -> Boards -> Lists -> Cards.
- **Real-time feel (Kanban):** Manage tasks dynamically across lists.
- **Deep Collaboration:** Add comments to tasks to keep the team updated.
- **Task Granularity:** Break down large cards using Checklists.
- **Visual Organization:** Apply custom colored Labels to cards.
- **Performance:** Redis caching implemented for heavy-read operations.
- **Cloud-Ready:** Fully containerized and configured for platforms like Render.

---

## 🗄 Database Schema

FlowBoard utilizes **PostgreSQL** with Entity Framework Core migrations. While in development a shared database instance can be used, the schema logically isolates tables per microservice context:

- `Users`
- `Workspaces`, `WorkspaceMembers`
- `Boards`, `BoardMembers`
- `Lists`
- `Cards`
- `Comments`
- `Labels`, `CardLabels`
- `Checklists`, `ChecklistItems`

---

## 🌐 API Reference

*(All endpoints prefixed with Gateway URL, e.g., `https://flowboard-gateway.onrender.com`)*

**Auth**
- `POST /api/auth/register`
- `POST /api/auth/login`

**Workspaces**
- `GET /api/workspaces/my`
- `POST /api/workspaces`
- `GET /api/workspaces/{id}`

**Boards**
- `GET /api/boards/workspace/{workspaceId}`
- `POST /api/boards`
- `GET /api/boards/{id}`

**Lists & Cards**
- `GET /api/lists/board/{boardId}`
- `POST /api/lists`
- `GET /api/cards/list/{listId}`
- `POST /api/cards`

**Details (Comments, Labels, Checklists)**
- `GET /api/comments/card/{cardId}`
- `GET /api/labels/board/{boardId}`
- `GET /api/checklists/card/{cardId}`

*(All protected routes require `Authorization: Bearer <token>` header)*

---

## 🔧 Infrastructure

### YARP API Gateway
- Centralized entry point on port 5000 (or 8080 in production).
- Configured via `appsettings.json` to route `/api/boards/{**catch-all}` to the Board service, etc.

### Redis Cache
- Integrated into the **Card.API**.
- Speeds up retrieval of cards within busy lists.

### Containerization
- Each of the 8 microservices, plus the gateway, contains a `Dockerfile`.
- `docker-compose.yml` orchestrates the entire backend, spinning up Postgres, Redis, and all .NET APIs simultaneously.

---

## 📊 Key Design Patterns

| Pattern | Implementation |
|---------|----------------|
| **Repository Pattern** | Abstracts EF Core database operations (e.g., `IBoardRepository`). |
| **Service Layer** | Contains business logic, separated from Controllers. |
| **DTOs & AutoMapper** | Prevents over-posting and formats data for the client. |
| **Reverse Proxy** | YARP handles external traffic and forwards to internal services. |
| **Standalone Components**| Modern Angular approach, removing the need for `NgModules`. |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) (for Angular)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Running with Docker Compose (Recommended)

1. Clone the repository.
2. Navigate to the backend root directory.
3. Start the infrastructure and microservices:
   ```bash
   docker-compose up --build -d
   ```
4. Apply database migrations (if not auto-applied by startup scripts).
5. The API Gateway will be available at `http://localhost:5000`.

### Running the Frontend

1. Navigate to the `flowboard-frontend` directory.
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the development server:
   ```bash
   npm start
   ```
4. Access the UI at `http://localhost:4200`.

---

## 🗺 Roadmap

- [x] Base Microservices Architecture setup
- [x] JWT Authentication & User Management
- [x] Workspaces, Boards, Lists, and Cards CRUD
- [x] Redis Caching implementation
- [x] Angular Standalone UI Integration
- [x] Cloud Deployment (Render.com)
- [x] SonarQube Local Code Analysis
- [ ] Drag and Drop Card reordering in UI
- [ ] Real-time updates via SignalR
- [ ] Email notifications for assigned tasks

---

<div align="center">
Made with ❤️ · FlowBoard — Microservices Project Management
</div>
