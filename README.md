# Notes App

A full-stack notes app built with Vue 3 and ASP.NET Core.

---

## Tech Stack

**Backend:** ASP.NET Core Web API, C#, Dapper, SQL Server, JWT

**Frontend:** Vue 3, TypeScript, TailwindCSS, Pinia, Axios

---

## Features

- Register and login with JWT authentication
- Create, edit, delete notes
- Search notes by title or content
- Sort by date or title
- Pagination
- Users can only see their own notes
- Responsive UI

---

## Setup

### Requirements

- .NET 10 SDK
- Node.js 18+
- SQL Server

---

### Backend

1. Go to `NotesApp.API/appsettings.json` and update:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=NotesApp;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Secret": "YOUR_SECRET_KEY_MIN_32_CHARACTERS",
    "Issuer": "NotesApp",
    "Audience": "NotesApp",
    "ExpiryHours": "24"
  }
}
```

2. Run the SQL schema against your SQL Server:

```
NotesApp.API/Data/Scripts/schema.sql
```

3. Start the API:

```bash
cd NotesApp.API
dotnet run
```

API runs on `http://localhost:5104`  
Swagger at `http://localhost:5104/swagger`

---

### Frontend

1. Install dependencies:

```bash
cd notes-app-frontend
npm install
```

2. Start the app:

```bash
npm run dev
```

App runs on `http://localhost:5173`

---

## API Endpoints

| Method | Endpoint             | Auth | Description                        |
| ------ | -------------------- | ---- | ---------------------------------- |
| POST   | `/api/auth/register` | No   | Register                           |
| POST   | `/api/auth/login`    | No   | Login, returns JWT                 |
| GET    | `/api/notes`         | Yes  | Get notes (search, sort, paginate) |
| GET    | `/api/notes/{id}`    | Yes  | Get one note                       |
| POST   | `/api/notes`         | Yes  | Create note                        |
| PUT    | `/api/notes/{id}`    | Yes  | Update note                        |
| DELETE | `/api/notes/{id}`    | Yes  | Delete note                        |

---

## Notes

- Passwords are hashed with BCrypt
- All SQL queries use parameterized inputs — no SQL injection
- Every note query is scoped to the logged-in user
- Pagination runs at the database level with OFFSET / FETCH

## Extra Things

⭐ JWT authentication
⭐ BCrypt password hashing
⭐ Global exception middleware
⭐ Pagination at database level
⭐ Parameterized queries — SQL injection protection
⭐ Route guard
⭐ Axios interceptors
⭐ Custom delete confirmation modal
⭐ Loading states + error states
⭐ Empty state UI
⭐ Docker setup
