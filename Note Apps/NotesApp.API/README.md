# Notes App

A full-stack notes application built with Vue 3 and ASP.NET Core.

## Tech Stack

**Frontend:** Vue 3, TypeScript, TailwindCSS, Pinia, Axios

**Backend:** ASP.NET Core Web API, C#, Dapper, SQL Server, JWT

## Features

- User registration and login with JWT authentication
- Create, read, update, delete notes
- Search notes by title or content
- Sort by created date, updated date, or title
- Pagination at the database level
- Users can only access their own notes
- Responsive UI

## Getting Started

### Prerequisites

- .NET 10 SDK
- Node.js 18+
- SQL Server

### Backend Setup

```bash
cd NotesApp.API
```

Update the connection string in `appsettings.json`:

```json
"DefaultConnection": "Server=YOUR_SERVER;Database=NotesApp;Trusted_Connection=True;TrustServerCertificate=True;"
```

Run the SQL schema in `Data/Scripts/schema.sql` against your SQL Server instance.

```bash
dotnet run
```

API runs on `http://localhost:5104`
Swagger UI at `http://localhost:5104/swagger`

### Frontend Setup

```bash
cd notes-app-frontend
npm install
npm run dev
```

App runs on `http://localhost:5173`

## Project Structure

```
NotesApp.API/
├── Controllers/    # HTTP endpoints
├── Services/       # Business logic
├── Data/
│   ├── Repositories/   # SQL queries via Dapper
│   └── Scripts/        # SQL schema
├── DTOs/           # Request and response shapes
├── Models/         # Database entities
└── Middleware/     # Global exception handling

notes-app-frontend/
├── src/
│   ├── api/        # Axios calls
│   ├── stores/     # Pinia state
│   ├── views/      # Pages
│   ├── types/      # TypeScript interfaces
│   └── router/     # Routes and guards
```

## Design Decisions

**Single project backend** — Clean layered separation (Controllers → Services → Repositories) without the overhead of multiple projects. Right choice for this scope.

**Dapper over EF Core** — SQL is explicit, visible, and auditable. Pagination at the DB level is trivial with OFFSET/FETCH.

**JWT in localStorage** — Acceptable for a demo. In production would use httpOnly cookies with a refresh token.

**LIKE search over full-text** — Sufficient for a notes app at this scale. Full-text indexes would be the next step.
