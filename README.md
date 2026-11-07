\# 📚 ReadingTracker API



A robust RESTful API backend for tracking personal reading habits, managing book libraries, and logging daily reading sessions. 



Built with .NET Core and C#, this project demonstrates enterprise-level backend engineering principles, focusing on separation of concerns, secure data access, and high-performance querying.



\## 🏗 Architecture \& Design Patterns

This project strictly adheres to Clean Architecture (Domain-Driven Design) to ensure business logic is decoupled from external frameworks and data access layers.



&#x20;  Domain Layer Enterprise entities (`Book`, `UserBook`, `ReadingSession`) and Core Interfaces.

&#x20;  Application Layer Business logic, Services, DTOs, and JWT Authentication mapping.

&#x20;  Infrastructure Layer Data persistence utilizing Entity Framework Core and PostgreSQL. Implements the Repository Pattern to abstract database operations.

&#x20;  Presentation (API) Layer Controllers, Dependency Injection configuration, and SwaggerOpenAPI documentation.



\## ⚙️ Tech Stack

&#x20;  Framework .NET 8  ASP.NET Core Web API

&#x20;  Language C#

&#x20;  Database PostgreSQL

&#x20;  ORM Entity Framework Core (Code-First Migrations)

&#x20;  Authentication JWT (JSON Web Tokens) via HttpOnly Cookies  Bearer

&#x20;  Testing NUnit \& Moq



\## 🚀 Key Features

&#x20;  JWT Authentication \& Authorization Secure endpoints with user-specific data isolation (BOLAIDOR prevention).

&#x20;  Library Management Add books from the global catalog to a personal user library with custom reading statuses.

&#x20;  Session Tracking Log daily pages read and duration for specific books.

&#x20;  Optimized Data Access Utilization of `IQueryable` projections, `.AsNoTracking()` for read-heavy operations, and explicitly tuned execution plans.



\## 💻 Local Setup \& Installation



\### Prerequisites

&#x20;  \[.NET SDK](httpsdotnet.microsoft.comdownload)

&#x20;  \[PostgreSQL](httpswww.postgresql.orgdownload) installed and running locally

&#x20;  pgAdmin 4 (Optional, for database management)



\### 1. Clone the repository

```bash

git clone \[httpsgithub.comyourusernameReadingTracker.git](httpsgithub.comyourusernameReadingTracker.git)

cd ReadingTracker

