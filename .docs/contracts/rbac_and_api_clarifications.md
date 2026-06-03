# 🔐 RBAC and API Clarifications (LocalCRM)

*This document captures the resolved authorization and API contract decisions for LocalCRM. It supersedes contradictory or ambiguous statements in earlier specifications where applicable.*

---

# I. RBAC / Authorization Model

## 1. Authorization Model Overview
LocalCRM uses a **hybrid RBAC model**:
- **Entity-action permissions** for normal business entities
- **Coarse system permissions** for cross-cutting administrative capabilities

Authorization is enforced on:
- REST endpoints
- GraphQL queries and mutations
- UI rendering/feature access

By default, users may only access data and actions allowed by their assigned role permissions.

---

## 2. Permission Design Principles

### 2.1 Business Entity Permissions
Each soft-deletable business entity has its own permission set:

- `companies.read`
- `companies.read_deleted`
- `companies.create`
- `companies.update`
- `companies.delete`
- `companies.restore`

- `contacts.read`
- `contacts.read_deleted`
- `contacts.create`
- `contacts.update`
- `contacts.delete`
- `contacts.restore`

- `interactions.read`
- `interactions.read_deleted`
- `interactions.create`
- `interactions.update`
- `interactions.delete`
- `interactions.restore`

- `engagements.read`
- `engagements.read_deleted`
- `engagements.create`
- `engagements.update`
- `engagements.delete`
- `engagements.restore`

- `notes.read`
- `notes.read_deleted`
- `notes.create`
- `notes.update`
- `notes.delete`
- `notes.restore`

- `documents.read`
- `documents.read_deleted`
- `documents.create`
- `documents.update`
- `documents.delete`
- `documents.restore`

### 2.2 Link Management Permissions
Link management does **not** use separate link-table permissions.  
Instead, link add/remove operations are governed by the relevant parent entity's **update** permission.

Examples:
- linking/unlinking contacts to companies → `companies.update`
- linking/unlinking notes to engagements → `engagements.update`
- linking/unlinking documents to interactions → `interactions.update`

### 2.3 User Permissions
Users use a hybrid model with both action permissions and optional coarse management capability.

Supported permissions include:
- `users.read`
- `users.create`
- `users.update`
- `users.disable`
- `users.manage`

`users.manage` may be used as an umbrella permission in UI and service-layer checks where appropriate.

### 2.4 Role/Permission Administration
Role and permission definitions are seeded system data and are mostly stable.  
However, administrators may manage role-permission assignments through `role_permissions_link`.

This capability is governed by:
- `roles.assign_permissions`

### 2.5 Settings Permissions
Application settings are admin-manageable and governed by:
- `settings.manage`

### 2.6 Audit Log Permission
Audit log access is governed by:
- `audit.read`

---

## 3. Soft-Delete and Recovery Authorization

### 3.1 Read vs Read Deleted
Read access is explicitly split into:
- normal read access (`*.read`)
- deleted/recovery scope read access (`*.read_deleted`)

A user must have `*.read_deleted` to query soft-deleted records in restricted/admin recovery views.

### 3.2 Restore
Restore is a first-class permission and is not implied by delete or admin role identity alone.

Examples:
- `companies.restore`
- `contacts.restore`
- `interactions.restore`

---

## 4. Sensitive Visibility / Private Data

### 4.1 Elevation Requirement
Private notes/documents are excluded from standard views by default, even for administrators.

To access private note/document data, a user must:
1. have the permission:
   - `doc_visibility.elevate`
2. explicitly enable elevated/private view mode in the UI or request context

Both conditions are required.

### 4.2 Creator Access Rule
The creator of a private note or document may access their own private item without elevated mode.

### 4.3 Aggregation Rule
When elevated/private mode is not enabled, private notes/documents must be excluded from:
- list results
- detail results
- counts
- aggregates
- exports

### 4.4 `team` Visibility
The `team` value in `doc_visibility` remains valid, but until a formal team model exists, it is treated as equivalent to standard authorized visibility.

---

# II. Authentication & Session Rules

## 1. Token Model
Authentication uses:
- JWT access token with expiry
- refresh token

Refresh tokens are:
- stored server-side
- revocable

## 2. Disabled Users
If `users.is_active = FALSE`:
- new login is blocked
- existing refresh tokens are invalidated
- API access must be denied

## 3. Password Management
Supported password flows:
- authenticated user self-service password change
- administrator password reset for another user

## 4. Security Event Auditing
The following events must be audit logged:
- failed login attempts
- authorization failures (403)
- write operations and link changes
- API/system errors

Ordinary validation failures are **not** audit logged unless they also qualify as a security or system event.

---

# III. API Contract Clarifications

## 1. API Style
LocalCRM supports both:
- **GraphQL**
- **REST**

Both are first-class API styles.

### 1.1 GraphQL
GraphQL supports:
- reads
- writes
- restore mutations
- bulk operations

### 1.2 REST
REST supports:
- reads
- writes
- soft-delete via standard `DELETE`
- restore via dedicated action endpoints
- bulk operations
- export operations

### 1.3 Frontend Integration
The frontend may use either GraphQL or REST per use case.  
Neither is considered exclusively primary.

---

## 2. Standard REST Semantics

### 2.1 Soft Delete
A normal REST `DELETE` operation performs a soft-delete.

Example:
- `DELETE /api/companies/{id}`

### 2.2 Restore
Restore is exposed as a dedicated REST action endpoint.

Example:
- `POST /api/companies/{id}/restore`

### 2.3 Export
CSV export is exposed via REST only.

Export must support:
- selected rows
- full filtered result sets

Export output must always respect:
- permission filtering
- soft-delete visibility rules
- private visibility rules
- current elevated/private mode state

### 2.4 Bulk Operations
Bulk operations are supported in REST.

Examples:
- bulk delete
- bulk restore (where authorized)
- bulk tagging

---

## 3. Standard GraphQL Semantics

### 3.1 Supported Operations
GraphQL supports:
- queries for reads
- mutations for create/update/delete/restore
- bulk mutations

### 3.2 Pagination
List queries use **offset pagination**.

Standard result wrapper:
- `items`
- `totalCount`
- `offset`
- `limit`

### 3.3 Filtering and Sorting
Filtering and sorting are primarily **server-side**.

GraphQL list resolvers should accept standard filter and sort inputs and apply them before pagination.

### 3.4 Concurrency
Mutations that update existing records must receive the client's last known `updated_at` value.

On mismatch, the API must return a concurrency conflict using a standardized application error code:
- `concurrency_conflict`

### 3.5 Bulk Operations
Bulk operations are supported in GraphQL where applicable.

---

## 4. Concurrency Rules

### 4.1 Contract
Optimistic concurrency is enforced using `updated_at`.

The client must submit `updated_at` for update operations.

### 4.2 REST Error Behavior
If `updated_at` does not match current persisted state:
- return HTTP `409 Conflict`

### 4.3 GraphQL Error Behavior
GraphQL must return a structured application error with:
- `code = "concurrency_conflict"`

---

## 5. Error Contract

### 5.1 REST Error Payload
All REST errors must follow a standard payload contract:

```json
{
  "code": "validation_failed",
  "message": "Validation failed",
  "details": {},
  "traceId": "..."
}
```
### 5.2 GraphQL Error Extensions
GraphQL errors must include equivalent application metadata in the error extensions object where applicable:

* code
* message
* details
* traceId

# IV. System Metrics / Dashboard API
## 1. Dedicated Metrics API
A dedicated system metrics API is required to support dashboard operational cards.

Metrics include:

* uptime
* last API action
* database size
* total API calls since startup
* total API failures since startup

## 2. Exposure
System metrics are exposed in:

* REST
* GraphQL

# V. Implementation Notes
1. UI controls must be conditionally rendered based on resolved permissions.
2. Backend authorization checks are mandatory even when UI hides controls.
3. All read paths must apply:
    * RBAC scope
    * soft-delete scope
    * private visibility scope
4. Export and dashboard aggregation endpoints must apply the same visibility rules as normal list/detail endpoints.
5. Role-permission assignment changes must be auditable.