# 🛠️ Technology Stack Specification (LocalCRM)

*This document specifies all mandated technologies, frameworks, and libraries. Agents must install and utilize these dependencies to ensure system consistency.*

## 📊 I. Stack Summary Overview

| Layer | Component | Technology | Version Policy | Key Purpose |
| :--- | :--- | :--- | :--- | :--- |
| **Backend (API)** | Core Language | C# / .NET | Latest Stable LTS | Business Logic Execution, API Hosting. |
| | Web Framework | ASP.NET Core Web API | Latest Stable LTS | Exposing REST/GraphQL endpoints. |
| | Documentation | Swagger (Swashbuckle) | 6.5.0 | Interactive API testing and discovery. |
| | Data Access (ORM) | Entity Framework Core | Latest Stable LTS | Managing domain models and transactions (CRUD). |
| | Database Engine | SQLite | Latest Stable | Local file-based relational database, no external DB installation required. |
| **Backend Testing** | Unit Testing | xUnit | Latest Stable | Testing isolated service methods and domain logic. |
| | Mocking | Moq | Latest Stable | Creating mock dependencies for unit testing. |
| **Frontend (SPA)** | Primary UI - A | Blazor WebAssembly | 8.0 | Full SPA implementation for one client view. |
| | Primary UI - B | Angular | Latest Stable | Full SPA implementation for second client view. |
| | Styling (A) | MudBlazor | Latest Stable | Material Design component library for Blazor. |
| | Styling (B) | TailwindCSS | Latest Stable | Utility-first CSS framework for Angular. |
| | Components (B) | Angular Material Design | Latest Stable | Pre-built, consistent UI components for Angular. |
| **Testing (E2E)**| Unit Testing | Jasmine | Latest Stable | Testing frontend component logic and interactions. |
| | End-to-End (E2E) | Karma | Latest Stable | Coordinating full client-server interaction tests. |

---

## ⚙️ II. Detailed Backend Technology Stack (C#/.NET)

### 1. Core Technologies
| Component | Dependency/Library | Version | Purpose | Implementation Constraint |
| :--- | :--- | :--- | :--- | :--- |
| **Runtime** | C# / .NET | Latest Stable LTS | Programming language. | Must use modern C# features (async/await). |
| **API Host** | ASP.NET Core Web API | Latest Stable LTS | Hosts the GraphQL API endpoints. | Must handle multi-user context and authorization checks on every endpoint, using JWT Bearer tokens. |
| **Database** | SQLite | Latest Stable | The persistent, local data store. | Must support the transactional and foreign key requirements defined in `data_dictionary.md`. |
| **Data Access (Primary)** | Entity Framework Core | Latest Stable LTS | Used for complex domain model changes, transactions, and initial CRUD operations. | Must be used for implementing the audit logging service layer. |

### 2. Backend Testing Stack
| Tool | Purpose | Usage Context | Requirement |
| :--- | :--- | :--- | :--- |
| **xUnit** | Unit Testing Framework | Testing business logic service classes in isolation. | Unit tests must cover every required API endpoint's success path and all defined failure/error paths. |
| **Moq** | Mocking Framework | Simulating external dependencies (DB context, API calls, services) during unit testing. | Mandatory for isolating service methods from the actual database connection. |

---

## 🎨 III. Detailed Frontend Technology Stacks

### 1. Stack A: Blazor WebAssembly
This stack should be used for one version of the SPA.
*   **Framework:** Blazor WebAssembly (.NET 8.0).
*   **UI Library:** MudBlazor (Material Design components).
*   **Purpose:** Full Single Page Application client, communicating via GraphQL for reads and/or REST for all.
*   **Key Concern:** Managing asynchronous state and component lifecycle hooks to ensure data coherence between UI and API response.

### 2. Stack B: Angular
This stack should be used for the second version of the SPA.
*   **Framework:** Angular (Latest Stable).
*   **Styling:** TailwindCSS (Utility-First CSS framework).
*   **Components:** Angular Material Design (Mandatory component library for consistent UI/UX).
*   **Routing:** Angular Router (Mandatory for managing SPA navigation flow).
*   **Key Concern:** Implementing the complex component structure (List $\rightarrow$ Detail $\rightarrow$ Dashboard) while strictly adhering to the Material Design guidelines.

---

## ✅ IV. Testing Strategy (Full Stack)

### 1. Frontend Testing Tools
| Tool | Scope | Purpose | Implementation Notes |
| :--- | :--- | :--- | :--- |
| **Jasmine** | Unit Testing | Testing individual components, methods, and view logic in isolation. | Focus on component rendering and internal data manipulation *before* API calls. |
| **Karma** | End-to-End (E2E) Testing | Simulating real user paths through the application (e.g., Login $\rightarrow$ View Dashboard $\rightarrow$ Click Card $\rightarrow$ Fill Form $\rightarrow$ Submit). | Must verify correct API communication, token management, and UI state changes across multiple screens. |
