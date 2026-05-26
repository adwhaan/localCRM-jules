# 🏗️ Application Architecture Design Specification (MyCRM)

*This document defines the system's architectural boundaries, technical guidelines, and API contracts. Coding agents must treat this as the primary blueprint for the entire codebase.*

## 📜 I. Global Architectural Guidelines (Compliance Checklist)

All code generated for both frontend and backend must strictly adhere to these non-negotiable principles.

### ✅ General Principles (Best Practices)
*   **Security:** Strict compliance with OWASP Top 10 guidelines (Injection Prevention, XSS prevention, secure session management, etc.).
*   **Error Handling:** All API functions must include comprehensive try-catch blocks, ensuring graceful failure and clear error payloads (e.g., 400 Bad Request, 401 Unauthorized, 403 Forbidden) using uniform contracts like
```json
{
  "code": "validation_failed",
  "message": "Validation failed",
  "details": {
    "email": ["Invalid email format"]
  },
  "traceId": "..."
}
```
*   **Design Patterns:** Use established design patterns (e.g., Repository Pattern, Unit of Work, Service Layer pattern) to maximize modularity and testability.
*   **Code Quality:** Code must be production-grade, highly performant, and extensively commented.

### 🛡️ Data Integrity and Safety
*   **Data Validation:** Backend must perform all validation (type checking, required fields, length constraints) before processing any data. No invalid data is permitted into the data store.
*   **Concurrency:** The backend service must be multi-user and multi-threaded safe to prevent deadlocks or race conditions during concurrent writes.
*   **Transactionality:** Any business action that modifies data across multiple entities (e.g., creating an Interaction which also updates a Company's last activity) MUST be wrapped in a database transaction (ACID compliance).
*   **Data Store Requirement:** The backend must support native data store integration (no external data store installation required for basic operation).

### ⚙️ Backend Operational Constraints
*   **Architecture:** Implement a clear separation between the API Layer (public interface) and the Business Logic/Model Layer (internal reusable parts).
*   **Audit Logging:**
    *   The `audit_logs` table is **WRITE-ONLY** for the backend.
    *   The log must capture *all* data modification intent and *all* API errors (including the error message and user/system context).
    *   Audit logs are immutable and cannot be modified or deleted.
    *   Administrator can query the log.
*   **Authentication:**
    *   All passwords MUST be stored as secure hashes (e.g., bcrypt).
    *   The initial Administrator user must be created via a dedicated command-line initialization process:
        - `--init`: Initializes the database schema and seeds basic roles.
        - `--admin-password [password]`: Sets the password for the default `admin` user during initialization.
*   **Database Setup:** Use SQL migration scripts run at application startup. Migration tracking must be handled via a dedicated `__migrations` table.

## 💻 II. Backend Architecture (The API Contract)

### 🎯 API Standard
The backend API follows a hybrid approach:
- **GraphQL:** Used for all operations (lists, details, and complex queries). This allows clients (frontends) to request exactly the data they need, optimizing payload size and improving flexibility. We use **HotChocolate** for the GraphQL implementation. Pagination will be supported using offsets.
- **REST:** Provided for **all** operations (CRUD). REST endpoints allow for better/easier caching and provide a traditional interface for simpler interactions.

**Note** that although `Tasks` is a special variant of `Interactions`, it still gets its own API endpoint. The backend API will ensure that all requests to this endpoint will include the filter `is_task` = TRUE.

### 🔑 Core API Functions
The API must expose a dedicated set of service endpoints structured around the main entities.

1.  **User/Authentication:** Dedicated endpoints for login, registration, and user management (Admin Scope). Authentication will be done using JWT Bearer tokens and token expiry.
2.  **Resource CRUD:** Standard GraphQL for Read for all primary entities, and REST for Create, Read, Update, Delete, and Restore for all entities. Standard endpoint format, including filtering parameters.
3.  **Relationship Management:** Dedicated resolvers/mutations for linking and unlinking entities (e.g., `companies_contacts_link`).

### 📝 Generic CRUD Schema Template (GraphQL Type)
Due to the consistent pattern across all major entities (`companies`, `contacts`, `interactions`, etc.), a single, reusable schema template is defined below.

**GraphQL Object Type Structure:**
Every core entity type (e.g., `Company`, `Contact`) will expose the following fields:
*   `id`: ID (Integer) - Primary Key
*   `name`: Text - Primary Display Name
*   `ref`: Text - Shorthand reference name (if applicable)
*   `isDeleted`: Boolean - Soft-delete flag.
*   `createdAt`: DateTime
*   `createdBy`: String (User Identifier)
*   `updatedAt`: DateTime
*   `updatedBy`: String (User Identifier)
*   *(... plus all entity-specific fields defined in data_dictionary.md)*

**GraphQL Mutation/Query Template:**
| Action | HTTP Method (GraphQL equivalent) | Scope | Description |
| :--- | :--- | :--- | :--- |
| **List/Read All** | `query` | Authorized/Admin | Retrieves a paginated list of entities. Must respect `is_deleted=FALSE` by default. |
| **Read Specific** | `query` | Authorized/Admin | Retrieves a single entity by ID. |
| **Create** | `mutation` | Authorized | Creates a new record, triggers initial audit log entry. |
| **Update** | `mutation` | Authorized | Modifies an existing record, triggers audit log entry. |
| **Soft-Delete** | `mutation` | Authorized | Sets the `is_deleted` flag and records the action in the `audit_logs`. |
| **Restore** | `mutation` | Admin Only | Reverts the `is_deleted` flag (sets to `FALSE`) and records a `RESTORE` action in the audit log. |

### 💡 Critical Transaction Logic Examples
The backend must implement specific transactional logic for complex workflows:

1.  **Interaction Creation (UC03):** When creating an `Interaction`, the service layer must:
    *   (A) Read the specified Contact.
    *   (B) Determine the contact's active company at the recorded date/time.
    *   (C) If (B) is successful, automatically set the `company_id` foreign key.
    *   (D) Log the creation transaction and the data state to the `audit_logs`.

2.  **Link Management:** Creating or deleting a link record (e.g., `company_contacts_link`) must trigger both the link table update and an immediate audit log entry for the relationship change (`LINK_ADD`/`LINK_REMOVE`).

## 🎨 III. Frontend Architecture (The Client)

### 🖥️ Design Goal
The frontend's sole responsibility is presenting the data and handling user interaction; it must never perform complex business logic or directly interact with the database. All data mutations must be routed through the authenticated GraphQL or REST API endpoints.

### 🧩 Required Components and Views
The Single Page Application (SPA) must be structured using the following modular components:

1.  **Dashboard View:** The primary landing page, utilizing a card-like layout to display key metrics and recent activities. Must handle asynchronous data loading for all dashboard components.
2.  **Entity List View:** A generalized, reusable component responsible for displaying a paginated list of records. Must contain comprehensive filtering, sorting, and search capabilities.
3.  **Entity Detail/Edit View:** The comprehensive form component for creating or editing an entity. Must dynamically render input fields based on the entity type and its allowed reference/linking groups.
4.  **Read-Only View:** Used for dedicated detail viewing, particularly for non-editable data like the Audit Log.

### 🌐 Technology Split
*   **Frontend Frameworks:** Support for two frameworks (Blazor and Angular) is required. The data fetching and component logic must be strictly decoupled from the UI framework implementation.
*   **API Consumption:** The frontend must manage the user authentication flow (Login $\rightarrow$ Token acquisition) and attach the resulting authorization token to *every* subsequent API call.