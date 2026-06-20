# 🎯 localCRM Application Development Specification (Canonical Overview)

## 📝 Metadata and Scope
---
- **System Name:** localCRM
- **Primary Goal:** A multi-user, concurrent Customer Relationship Management (CRM) system.
- **Data Persistence:** Local data store.
- **Primary Platform:** Web Single Page Application (SPA).
- **Optional Secondary Client:** Alternate SPA implementation using a second frontend framework.
- **Scope Level:** Canonical consolidated specification.
---

## 📚 I. Core System Summary

localCRM is a modular CRM system centered around:
- authenticated multi-user access,
- role-based authorization,
- a dashboard-first workflow,
- soft-delete and auditability,
- hybrid REST and GraphQL APIs,
- SQLite-backed local persistence.

The application structure is organized around:
- Dashboard
- Companies
- Contacts
- Interactions
- Tasks
- Engagements
- Notes
- Documents
- Users / Roles / Permissions / Settings
- Audit Log

---

## 🔗 II. Canonical Specification Reference Index

| Component | Purpose | Reference File |
| :--- | :--- | :--- |
| Requirements & Functional Scope | Functional and non-functional behavior | `./requirements/requirement_details.md` |
| Architecture | Backend/frontend architecture and API model | `./architecture.md` |
| Technology Stack | Required platform and library choices | `./technology_stack.md` |
| Data Model | Canonical schema definition | `./data_modeling/data_dictionary.md` |
| UI/UX | SPA behavior and interaction rules | `./ui_ux_design.md` |
| RBAC + API Clarifications | Authorization and API conventions | `./rbac_and_api_clarifications.md` |
| Seeded Roles & Permission Matrix | Default roles and permissions | `./seeded_roles_and_permissions.md` |
| GraphQL Catalog | Canonical GraphQL operations | `./graphql_operation_catalog.md` |
| Auth Contract | JWT, refresh token, session rules | `./auth_contract.md` |
| Dashboard Metrics | Dashboard metric semantics | `./dashboard_metrics_contract.md` |
| User Management Contract | User lifecycle and password policy | `./user_management_contract.md` |

---

## ⚙️ III. Canonical User Flow

Primary flow:
1. User opens application
2. Login screen is shown
3. User authenticates or is diverted into forced password change flow if required
4. Successful authentication establishes session token(s)
5. User is navigated to Dashboard
6. User navigates to list/detail/edit views according to permissions
7. All reads/writes are executed through authenticated GraphQL and/or REST APIs

---

## 🧩 IV. Canonical Use Cases

### UC01 — Authentication / Startup
**Trigger:** Application startup

**Process:**
1. Show login screen
2. User enters username and password
3. System validates credentials
4. If credentials are valid and no forced password change is pending:
   - issue access token
   - issue refresh token
   - return current-user payload
   - navigate to Dashboard
5. If password change is required:
   - reject normal login with `password_change_required`
   - provide secure forced-password-change continuation path

**Failure handling:**
- invalid credentials → generic invalid credentials response
- locked out → lockout error
- password change required → `password_change_required`
- disabled user → authentication denied

---

### UC02 — Dashboard View
**Trigger:** Successful authentication

**Process:**
1. Fetch business dashboard data
2. Fetch system metrics if authorized
3. Render task card first
4. Render entity summary cards
5. Render system metrics cards if permitted
6. Clicking cards navigates to relevant list or detail modules

**Metric semantics:** governed by `dashboard_metrics_contract.md`

---

### UC03 — Create Interaction Record
**Trigger:** User starts add interaction flow

**Process:**
1. Present interaction form
2. User enters interaction data
3. User optionally links:
   - contact
   - company
   - engagement
4. If contact is selected:
   - interaction context stores `contact_id`
   - company is derived later from contact-company history when needed
5. Save interaction
6. Persist contextual linkage internally using `interactions_link`

**Rules:**
- `contact_id` and `company_id` are mutually exclusive in interaction context
- tasks are represented as `interactions.is_task = TRUE`

---

## ✅ V. Implementation Completion Criteria

The implementation is considered specification-complete when all of the following exist:

- authentication with JWT + refresh token lifecycle
- seeded roles and permission model
- canonical data model with soft-delete support
- audit logging for writes and security events
- dashboard and system metrics endpoints
- CRUD + restore for all business entities
- GraphQL and REST contracts aligned to canonical spec
- user-management and forced-password-change flow
- permission-aware UI behavior and restricted feature rendering