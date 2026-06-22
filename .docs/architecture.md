# 🏗️ Application Architecture Design Specification (Canonical)

## 📜 I. Global Architectural Principles

All code generated for backend and frontend must adhere to:

- OWASP-aligned secure design
- uniform error contracts
- separation of API layer and business/service layer
- strong validation and transactional integrity
- optimistic concurrency via `updated_at`
- auditability of writes and security-relevant failures
- soft-delete semantics by default
- SQLite as default local store
- migration tracking through `__migrations`

---

## 🛡️ II. Data Integrity and Safety

### 1. Validation
Backend validation is mandatory for:
- required fields
- type correctness
- format constraints
- business rule constraints
- permission checks

### 2. Concurrency
Optimistic concurrency uses `updated_at`.

Rules:
- client must send `updated_at` for update operations
- REST mismatch returns `409 Conflict`
- GraphQL mismatch returns application error code `concurrency_conflict`

### 3. Transactionality
Any business action modifying multiple related records must be transactional.

Examples:
- interaction create + contextual link create + audit log
- link add/remove + audit log
- restore + audit log
- user role change + session invalidation

### 4. Audit Logging
Audit logging is required for:
- entity create/update/delete/restore
- link add/remove
- failed login attempts
- authorization failures
- API/system errors

Ordinary validation failures are not audited by default.

---

## ⚙️ III. API Architecture

## 1. API Standard
The backend exposes both:

- **GraphQL**
- **REST**

Both are first-class APIs.

### GraphQL supports:
- reads
- writes
- restore
- bulk operations
- auth/session operations
- administrative operations
- dashboard and metrics queries

### REST supports:
- reads
- writes
- soft-delete via `DELETE`
- restore via action endpoints
- bulk operations
- export operations
- dashboard and system metrics endpoints
- auth/session endpoints

The frontend may use either API style per use case.

---

## 2. REST Conventions

### Base Patterns
- `/api/{entity}`
- `/api/{entity}/{id}`

### Common operations
- `GET /api/{entity}`
- `GET /api/{entity}/{id}`
- `POST /api/{entity}`
- `PUT /api/{entity}/{id}`
- `PATCH /api/{entity}/{id}`
- `DELETE /api/{entity}/{id}`
- `POST /api/{entity}/{id}/restore`

### Search
- `POST /api/{entity}/search`

### Deleted records
- `GET /api/{entity}/deleted`

### Bulk operations
- `POST /api/{entity}/bulk-delete`
- `POST /api/{entity}/bulk-restore`

### Export
- `POST /api/{entity}/export`

### Dashboard/system
- `GET /api/dashboard`
- `GET /api/system/metrics`

### Auth
- `POST /api/auth/login`
- `POST /api/auth/refresh`
- `POST /api/auth/logout`
- `POST /api/auth/change-password`
- `POST /api/auth/complete-password-change`
- `POST /api/auth/revoke-all-sessions`
- `GET /api/auth/me`

### Users
- `GET /api/users`
- `GET /api/users/{id}`
- `POST /api/users`
- `PUT /api/users/{id}`
- `PATCH /api/users/{id}`
- `POST /api/users/{id}/reset-password`
- `POST /api/users/{id}/disable`
- `POST /api/users/{id}/enable`
- `POST /api/users/{id}/revoke-sessions`

---

## 3. GraphQL Conventions

### Endpoint
- `/graphql`

### Naming
- type names: singular PascalCase
- collection queries: plural camelCase
- single-item queries: singular camelCase
- mutations: `verbEntity`

### Pagination
Offset pagination with:
- `items`
- `totalCount`
- `offset`
- `limit`

### Filters and Sorting
Each entity has typed filter/sort inputs.

### Deleted data
Deleted data is exposed through separate queries such as:
- `deletedCompanies`
- `deletedContacts`

### Mutations
Create/update return full entity objects.  
Delete/restore return shared `MutationResult`.

---

## 💻 IV. Backend Component Model

## 1. Layers
- API Layer
  - REST controllers
  - GraphQL resolvers
- Application Layer
  - services
  - authorization
  - audit logging
  - session management
- Data Access Layer
  - EF Core for writes / transactions
- Infrastructure Layer
  - SQLite
  - migrations
  - JWT signing and validation
  - config providers

## 2. Required Services
- `AuditService`
- `AuthService`
- `RefreshTokenService`
- `PermissionService`
- `DashboardService`
- `ExportService`
- entity-specific CRUD services
- link management services

---

## 🌐 V. Frontend Architecture

The frontend is a SPA and must:
- manage auth and refresh lifecycle
- enforce route and UI guarding
- honor forced-password-change flow
- support dashboard/list/detail/edit views
- use server-side filtering/sorting semantics
- render controls conditionally based on permissions
- support explicit private-visibility elevation mode where permitted

The UI may be implemented in:
- Angular
- Blazor WebAssembly

Feature parity expectations should be project-planned explicitly.

---

## 🧾 VI. Error Contract

### REST
Errors must use:
```json
{
  "code": "validation_failed",
  "message": "Validation failed",
  "details": {},
  "traceId": "..."
}

### GraphQL
Application errors must populate extensions with:

* `code`
* `message`
* `details`
* `traceId`

---

# `requirements/requirement_details.md`

```md
# 📜 Requirement Specification Details (Canonical)

## 🎯 I. Functional Requirements

### FR-1 Authentication and User Management
1. Username/password authentication is required.
2. JWT access tokens and refresh tokens are required.
3. Forced-password-change flow is required for:
   - admin-created users on first login
   - admin-reset users on next login
4. Multi-user support is required.
5. User management, role assignment, password reset, enable/disable, and revoke-sessions support are required.
6. RBAC is mandatory.

### FR-2 Core Entity Management
The system must support:
- Companies
- Contacts
- Interactions
- Documents
- Tasks (as `interactions.is_task = TRUE`)
- Engagements
- Users
- Roles/Permissions
- Notes
- Tags
- Settings
- AuditLog

### FR-3 Supporting Data and Settings
1. Tags are supported as:
   - structured lookup values via `tags`
   - delimited entity tag fields on selected business entities
2. Settings are supported as global configuration values.
3. Dashboard/system metrics use settings for configurable semantics where specified.

---

## 🚨 II. Non-Functional Constraints

### NFR-1 Persistence and Auditing
- Soft-delete is required for business entities and soft-deletable link tables.
- Standard public reads exclude deleted data by default.
- Restore support is required.
- Audit logging is mandatory for write operations and security-relevant failures.
- Audit logs are append-only.

### NFR-2 Security
- Passwords must be stored as secure hashes.
- Access token and refresh token flows are required.
- Refresh tokens are server-side stored and revocable.
- Session invalidation is required on:
  - role/permission changes
  - password changes
  - admin password reset
  - user disable
  - token reuse detection

### NFR-3 Concurrency
- Optimistic concurrency via `updated_at`
- first-to-save wins
- conflict must be surfaced to clients explicitly

### NFR-4 Dashboard Semantics
- Dashboard metric definitions are governed by the dashboard metrics contract.
- System metrics and business metrics are separate authorization scopes.

---

## 🧩 III. Seeded Defaults
Initialization must seed at minimum:
- roles:
  - `Administrator`
  - `User`
- default permission catalog
- role-permission assignments
- required tag groups
- required settings keys as appropriate

The seeded `Administrator` role receives all permissions by default.  
The seeded `User` role receives the baseline permissions defined in `seeded_roles_and_permissions.md`.