# 📜 Requirement Specification Details (LocalCRM)

*This document outlines all mandatory functional and non-functional requirements for the LocalCRM application. Agents should treat these points as definitive implementation constraints.*

---

## 🎯 I. Functional Requirement Sets (FR)

### FR-1: Authentication and User Management
| ID | Requirement | Detail / Constraint | Scope | Priority |
| :--- | :--- | :--- | :--- | :--- |
| **FR-1.1** | Basic Login Support | Must support user authentication (Username/Password). | Core | P1 |
| **FR-1.2** | User/Role Scalability | Initial state must be Administrator only. Must provide functionality to **add, maintain, and manage multiple user accounts** and associated roles. | User Management | P1 |
| **FR-1.3** | Permissions Model | A robust role-based access control (RBAC) system is mandatory. Roles must dictate permissible actions (CRUD) on different entities. | Security | P1 |

### FR-2: Core Entity Management (Data Model)
The system must be capable of tracking the following distinct entities. Each entity requires a dedicated data model and CRUD interface.

*   **[Entity] Companies:** Core company information.
*   **[Entity] Contacts:** Individual person information (linked to Companies).
*   **[Entity] Interactions:** Records of contact interactions (logs).
*   **[Entity] Documents:** Reference storage only. **Constraint:** The system must store metadata/references (e.g., path, upload date, associated entity ID), but *not* the actual binary file payload initially.
*   **[Entity] Tasks:** Future, planned interactions (Short-term tasks).
*   **[Entity] Engagements:** Long-running, spanning interactions (Complex tracking).
*   **[Entity] Users:** User account records.
*   **[Entity] Roles/Permissions:** Definition of system roles and access rights.

### FR-3: Supporting Data and System Settings
| Entity | Purpose | Detail |
| :--- | :--- | :--- |
| **Tags** | Flexible, cross-entity categorization. | Must support adding/applying tags to multiple entities (e.g., Company, Contact, Interaction). |
| **Application Settings** | System configuration values. | Mechanism to store global parameters (e.g., default timezone, API keys, billing intervals). |

---

## 🚨 II. Non-Functional Constraints (NFR)

### NFR-1: Data Persistence and Auditing
| Constraint Type | Requirement | Implementation Rule | Agent Action |
| :--- | :--- | :--- | :--- |
| **Audit Logging** | Mandatory audit trail for all data changes. | **Scope:** Entity-level auditing (e.g., "User X updated Company Y"). **Level:** Must track all CRUD operations (Create, Update, Delete, Link/unlink). **Detail:** Log must include: `(User ID, Timestamp, Entity Name, Entity ID, Action, Change Summary/Before Value)`. | Implement a central `AuditLog` service layer. |
| **Data Deletion** | Support for soft deletion. | **Mechanism:** Entities must include a `is_deleted` (Boolean) and `deleted_at` (Timestamp) flag. **Constraint:** Deleting an entity *does not* physically remove it; it merely sets the flag. | Modify all relevant ORM models. |
| **Concurrency** | Optimistic Concurrency | **Mechanism:** A "first-to-save wins" strategy using the `updated_at` column for validation before saving changes. | Implement check in service layer. |
| **Query Scope** | System queries must exclude deleted items by default. | **Constraint:** All standard read queries (`SELECT`) must include a `WHERE is_deleted = FALSE` clause unless explicitly overridden (e.g., in the Admin view). | Modify all repository/service methods. |
| **Admin Override** | Only the Administrator user role can view soft-deleted entities. | **Mechanism:** A specific Admin view/endpoint is required to query entities where `is_deleted = TRUE`. | Implement restricted admin functionality. |
| **Admin Recovery** | The Administrator must have the ability to revert a soft-delete action (Undo). | **Constraint:** Undoing deletion must generate a new, specific audit log entry. | Implement the `restore_entity` service method. |

---

## 🧩 III. Summary Checklist for Agent Implementation

*Use this list to validate completeness.*

*   [ ] **Authentication:** Built-in multi-user/role management (RBAC), using JWT Bearer tokens with token expiry duration.
*   [ ] **Core Schema:** All primary entities (`Companies`, `Contacts`, `Interactions`, `Documents`, `Engagements`, `Users`) are defined with appropriate fields (leaving out `Tasks` here, as this is a specialization of `Interactions`).
*   [ ] **Secondary Schema:** Supporting entities (`Roles`, `Permissions`, `Notes`, `Tags`, `Settings`, `AuditLog`) are implemented.
*   [ ] **Persistence Rule:** All data write operations (CRUD) must trigger the `AuditLog` service.
*   [ ] **Deletion Rule:** Every core entity supports `soft_delete` mechanism (`is_deleted` flag).
*   [ ] **Query Rule:** All public-facing queries default to filtering out soft-deleted records.
*   [ ] **Admin Scope:** An isolated, restricted path exists for Admin users to view and restore deleted records.# 📚 Data Model Specification (localCRM)

*This document defines the explicit schema, data types, primary keys (PK), foreign keys (FK), and constraints for all database tables. This version incorporates clarified modeling decisions and should be treated as the single source of truth for the database layer.*

## ⚙️ I. Global Schema Constraints & Rules

These rules apply to all core entity tables unless explicitly overridden.

### 1. Naming Conventions
*   **Table Names:** Must be plural (e.g., `companies`, `contacts`).
*   **Primary Key:** Every main entity must have an auto-incrementing integer primary key field named `[entity_name]_id`.
*   **Historical Link Tables:** Historical relationship tables use a **composite natural key** based on the two linked entity IDs plus `start_date`.

### 2. Temporal/Audit Fields (Core Entities)
All primary entity tables MUST include the following fields unless explicitly overridden:

| Field Name | Data Type | Required | Constraint | Purpose |
| :--- | :--- | :--- | :--- | :--- |
| `created_at` | TIMESTAMP | Yes | Default: CURRENT_TIMESTAMP | Time of initial record creation. |
| `created_by` | TEXT | Yes | Logical FK to `users.username` | User who created the record. |
| `updated_at` | TIMESTAMP | No | Auto-update on change | Time of last record modification. |
| `updated_by` | TEXT | No | Logical FK to `users.username` | User who last updated the record. |
| `is_deleted` | BOOLEAN | Yes | Default: `FALSE` | Soft-delete flag. |
| `deleted_at` | TIMESTAMP | No | Nullable | Timestamp of soft deletion. |

### 3. Temporal/Audit Fields (Soft-Deletable Link Tables)
Soft-deletable link tables MUST include the following fields:

| Field Name | Data Type | Required | Constraint | Purpose |
| :--- | :--- | :--- | :--- | :--- |
| `created_at` | TIMESTAMP | Yes | Default: CURRENT_TIMESTAMP | Time of initial link creation. |
| `created_by` | TEXT | Yes | Logical FK to `users.username` | User who created the link. |
| `is_deleted` | BOOLEAN | Yes | Default: `FALSE` | Soft-delete flag for the link. |
| `deleted_at` | TIMESTAMP | No | Nullable | Timestamp of soft deletion. |

### 4. Relationship Constraints (Referential Integrity)
*   **Foreign Keys (FKs):** All explicit entity relationships must be implemented as defined FKs where practical.
*   **Lookup Fields Using `tags`:** Fields such as `company_type`, `branch`, `size`, `sex`, `role`, etc. use `TEXT` values and are treated as **logical references** to `tags.tag_value` within the appropriate `tag_group`. These are not required to be strict database-level foreign keys.
*   **Linking (M:M):** Many-to-many and historical relationships use dedicated link tables.
*   **Soft Delete Scope:** All standard queries must filter `is_deleted = FALSE` unless explicitly operating in an authorized recovery/admin context.

### 5. Soft-Delete Rule
All soft-deletable entities and all soft-deletable link tables MUST include:
- `is_deleted BOOLEAN NOT NULL DEFAULT FALSE`
- `deleted_at TIMESTAMP NULL`

### 6. Task Interpretation Rule
Tasks are a specialization of `interactions`:
- `is_task = TRUE` indicates the interaction is a task.
- “Upcoming tasks” are interactions where:
  - `is_task = TRUE`
  - `is_deleted = FALSE`
  - `interaction_date` and optional `interaction_time` indicate a future point in time
  - `state` is not in a closed/completed terminal status as defined by the application’s allowed `interaction_state` values

---

## 🏷️ II. Lookup Tables & Enums

### Entity: `tags`
Defines a structured key-value pair system for categorization and controlled vocabularies.

| Field Name | Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `tag_id` | INTEGER | PK, Auto-Increment | Unique identifier. |
| `tag_group` | TEXT | Required | Defines the context (e.g., `company_type`, `contact_sex`). |
| `tag_name` | TEXT | Required | Human-readable display name. |
| `tag_value` | TEXT | Required | Value stored in referencing entity fields. |
| `tag_order` | INTEGER | Optional | Display order (Default: 0). |

### Tag Group Definitions
The following groups are treated as constrained lookup vocabularies.

| Group Name | Purpose | Values (Examples) |
| :--- | :--- | :--- |
| `entity_tags` | User-supplied freeform tags used in delimited tag fields across supported entities. | User-defined |
| `company_type` | Company classification. | `Recruitment`: `recr` \| `Client`: `clnt` \| `Services`: `svc` \| `Employer`: `empl` \| `Consulting`: `cslt` |
| `company_branch` | Industry sector classification. | `Technology`: `tech` \| `Financial Services`: `finsvc` \| ... |
| `company_size` | Company size classification. | `Micro`: `1–9 employees` \| `Small`: `10–49 employees` \| ... |
| `contact_sex` | Contact gender. | `Male`: `M` \| `Female`: `F` \| `Neutral`: `x` \| `Other`: `O` \| `Unknown`: `u` |
| `contact_role` | Contact role within a company relationship. | `Management`: `mgmt` \| `Client`: `clnt` \| ... |
| `engagement_conf` | Engagement visibility/confidentiality level. | `Public`: `pub` \| `Limited`: `lmtd` \| `Private`: `priv` |
| `engagement_status` | Current state of the engagement. | `Planned`: `plan` \| `Active`: `act` \| `On hold`: `hold` \| ... |
| `interaction_state` | Status of the interaction or task. | `Open`: `open` \| `Active`: `actv` \| `Closed`: `clsd` \| ... |
| `interaction_type` | Medium or form of interaction. | `Email`: `eml` \| `Call`: `call` \| ... |
| `interaction_direction` | Who initiated the contact. | `Inbound`: `in` \| `Outbound`: `out` \| `N/A`: `n_a` |
| `doc_visibility` | Visibility for notes and documents. | `Private`: `priv` \| `Limited`: `lmtd` \| `Team`: `team` |
| `document_type` | Type of document reference. | `Proposal`: `prop` \| `Contract`: `cntr` \| ... |

---

## 📂 III. Core Entity Schemas

### 1. Entity: `users`

Users are not soft-deletable. Users should not be deleted to preserve historic data consistency.  
Initially, on database setup, the table will be populated with an Administrator account.

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `user_id` | INTEGER | PK, Auto-Increment | |
| `username` | TEXT | UNIQUE, NOT NULL | Used for login and logical audit references. |
| `password_hash` | TEXT | NOT NULL | Stored secure password hash. |
| `role_id` | INTEGER | FK to `roles.role_id`, NOT NULL | Primary role for the user. |
| `is_active` | BOOLEAN | NOT NULL | Enabled/Disabled status. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `updated_at` | TIMESTAMP | NULL | |
| `updated_by` | TEXT | NULL | |

---

### 2. Entity: `roles`

Roles are system data and are not soft-deletable.

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `role_id` | INTEGER | PK, Auto-Increment | |
| `role_name` | TEXT | UNIQUE, NOT NULL | Human-readable role name. |

---

### 3. Entity: `permissions`

Permissions are system data and are not soft-deletable.

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `permission_id` | INTEGER | PK, Auto-Increment | |
| `permission_name` | TEXT | UNIQUE, NOT NULL | e.g., `read_all`, `manage_users`. |

---

### 4. Entity: `settings`

Settings are system data and are not soft-deletable.  
If a setting key is missing, the application should use an internal default.

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `setting_id` | INTEGER | PK, Auto-Increment | |
| `key` | TEXT | UNIQUE, NOT NULL | Compound key allowed (e.g., `company.self`). |
| `value` | TEXT | NOT NULL | Configuration value. |

---

### 5. Entity: `companies`

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `company_id` | INTEGER | PK, Auto-Increment | |
| `company_ref` | TEXT | UNIQUE, NOT NULL | Secondary business reference. |
| `name` | TEXT | NOT NULL | Legal/company name. |
| `address_line_1` | TEXT | NULL | |
| `address_line_2` | TEXT | NULL | |
| `postal_code` | TEXT | NULL | |
| `city` | TEXT | NOT NULL | |
| `country` | TEXT | NULL | |
| `phone` | TEXT | NULL | |
| `website` | TEXT | NULL | |
| `email` | TEXT | NULL | General email contact. |
| `company_tags` | TEXT | NULL | Delimited list of `tag_value` entries from `entity_tags`. |
| `company_type` | TEXT | NOT NULL | Logical reference to `tags.tag_value` in `company_type`. |
| `branch` | TEXT | NULL | Logical reference to `tags.tag_value` in `company_branch`. |
| `size` | TEXT | NULL | Logical reference to `tags.tag_value` in `company_size`. |
| `rating` | INTEGER | NOT NULL | Trustworthiness rating (expected range 0-4). |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `updated_at` | TIMESTAMP | NULL | |
| `updated_by` | TEXT | NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 6. Entity: `contacts`

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `contact_id` | INTEGER | PK, Auto-Increment | |
| `contact_ref` | TEXT | UNIQUE, NULL | Optional secondary business reference. |
| `first_name` | TEXT | NOT NULL | |
| `middle_name` | TEXT | NULL | |
| `last_name` | TEXT | NOT NULL | |
| `phone` | TEXT | NULL | |
| `email` | TEXT | NULL | |
| `linkedin_url` | TEXT | NULL | Optional LinkedIn URL. |
| `birthdate` | DATE | NULL | |
| `contact_tags` | TEXT | NULL | Delimited list of `tag_value` entries from `entity_tags`. |
| `rating` | INTEGER | NOT NULL | Trustworthiness rating (expected range 0-4). |
| `sex` | TEXT | NULL | Logical reference to `tags.tag_value` in `contact_sex`. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `updated_at` | TIMESTAMP | NULL | |
| `updated_by` | TEXT | NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 7. Entity: `interactions`

Tasks are represented using `is_task = TRUE`.

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `interaction_id` | INTEGER | PK, Auto-Increment | |
| `interaction_date` | DATE | NOT NULL | Date of the interaction or planned task. |
| `interaction_time` | TIME | NULL | Time of the interaction or task. |
| `direction` | TEXT | NULL | Logical reference to `tags.tag_value` in `interaction_direction`. |
| `interaction_type` | TEXT | NOT NULL | Logical reference to `tags.tag_value` in `interaction_type`. |
| `subject` | TEXT | NOT NULL | Subject/title. |
| `note` | TEXT | NULL | Summary notes. |
| `state` | TEXT | NOT NULL | Logical reference to `tags.tag_value` in `interaction_state`. |
| `prev_interaction_id` | INTEGER | NULL, FK to `interactions.interaction_id` | Link to a previous related interaction. |
| `interaction_tags` | TEXT | NULL | Delimited list of `tag_value` entries from `entity_tags`. |
| `is_task` | BOOLEAN | NOT NULL | `TRUE` if this record is a task. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `updated_at` | TIMESTAMP | NULL | |
| `updated_by` | TEXT | NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 8. Entity: `engagements`

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `engagement_id` | INTEGER | PK, Auto-Increment | |
| `engagement_ref` | TEXT | UNIQUE, NULL | Optional shorthand reference. |
| `description` | TEXT | NULL | Summary description. |
| `engagement_tags` | TEXT | NULL | Delimited list of `tag_value` entries from `entity_tags`. |
| `confidentiality` | TEXT | NULL | Logical reference to `tags.tag_value` in `engagement_conf`. |
| `engagement_status` | TEXT | NOT NULL | Logical reference to `tags.tag_value` in `engagement_status`. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `updated_at` | TIMESTAMP | NULL | |
| `updated_by` | TEXT | NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 9. Entity: `notes`

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `note_id` | INTEGER | PK, Auto-Increment | |
| `subject` | TEXT | NOT NULL | Short description. |
| `body` | TEXT | NULL | Full note body. |
| `visibility` | TEXT | NOT NULL | Logical reference to `tags.tag_value` in `doc_visibility`. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `updated_at` | TIMESTAMP | NULL | |
| `updated_by` | TEXT | NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 10. Entity: `documents`

This entity stores metadata/reference information only, not binary payloads.

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `document_id` | INTEGER | PK, Auto-Increment | |
| `document_ref` | TEXT | UNIQUE, NOT NULL | Shorthand reference code. |
| `subject` | TEXT | NULL | Optional description. |
| `document_type` | TEXT | NOT NULL | Logical reference to `tags.tag_value` in `document_type`. |
| `document_url` | TEXT | NOT NULL | URL or path to the document reference. |
| `visibility` | TEXT | NOT NULL | Logical reference to `tags.tag_value` in `doc_visibility`. |
| `checksum` | TEXT | NULL | Optional checksum/hash value for verification. |
| `is_checked` | BOOLEAN | NOT NULL, Default `FALSE` | If `TRUE`, `checksum` should be present and valid. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `updated_at` | TIMESTAMP | NULL | |
| `updated_by` | TEXT | NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 11. Entity: `audit_logs`

This table is append-only. Audit log rows must never be updated or deleted by normal application logic.

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `audit_id` | INTEGER | PK, Auto-Increment | |
| `entity_name` | TEXT | NOT NULL | Table/entity name involved (e.g., `companies`). |
| `entity_id` | INTEGER | NOT NULL | PK value of the affected record. |
| `action_type` | TEXT | NOT NULL | Fixed set: `CREATE`, `UPDATE`, `DELETE`, `SOFT_DELETE`, `RESTORE`, `LINK_ADD`, `LINK_REMOVE`, `ERROR`. |
| `performed_at` | TIMESTAMP | NOT NULL, Default CURRENT_TIMESTAMP | |
| `performed_by` | TEXT | NOT NULL | Username of acting user or `system`. |
| `notes` | TEXT | NULL | Optional descriptive text. |

## Additional Security Support Entities

### Entity: `refresh_tokens`

Stores server-side refresh token sessions for revocation, rotation, and reuse detection.

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `refresh_token_id` | INTEGER | PK, Auto-Increment | |
| `user_id` | INTEGER | FK to `users.user_id`, NOT NULL | |
| `session_id` | TEXT | NOT NULL | Stable session-family identifier; corresponds to JWT `sid`. |
| `token_hash` | TEXT | NOT NULL | Hash of refresh token value. |
| `issued_at` | TIMESTAMP | NOT NULL | |
| `expires_at` | TIMESTAMP | NOT NULL | |
| `revoked_at` | TIMESTAMP | NULL | |
| `replaced_by_token_id` | INTEGER | NULL, FK to `refresh_tokens.refresh_token_id` | Rotation chain support. |
| `reuse_detected_at` | TIMESTAMP | NULL | Token reuse detection marker. |
| `created_by_ip` | TEXT | NULL | Optional source IP. |
| `user_agent` | TEXT | NULL | Optional client descriptor. |

### Add fields to `users`

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `must_change_password` | BOOLEAN | NOT NULL, Default `FALSE` | Required password change before normal login. |
| `failed_login_attempts` | INTEGER | NOT NULL, Default `0` | Optional user-level lockout support. |
| `locked_until` | TIMESTAMP | NULL | Temporary lockout end time. |

---

## 🔗 IV. Relationship (Link) Tables

### 1. `company_contacts_link`

Tracks historical relationships between companies and contacts.

**Primary Key:** (`company_id`, `contact_id`, `start_date`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `company_id` | INTEGER | PK, FK to `companies.company_id`, NOT NULL | |
| `contact_id` | INTEGER | PK, FK to `contacts.contact_id`, NOT NULL | |
| `start_date` | DATE | PK, NOT NULL | Start of relationship. |
| `end_date` | DATE | NULL | End of relationship; null means still active/open. |
| `role` | TEXT | NULL | Logical reference to `tags.tag_value` in `contact_role`. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 2. `interactions_link`

Provides the contextual 1:1 linkage for an interaction.

**Primary Key:** (`interaction_id`)

Rules:
- One row per interaction.
- `contact_id` and `company_id` are **mutually exclusive**.
- If `contact_id` is present, `company_id` must be `NULL`.
- `engagement_id` is independent and may be present or null.
- Company may be derived from the contact’s active company at the interaction date/time in application logic.

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `interaction_id` | INTEGER | PK, FK to `interactions.interaction_id`, NOT NULL | |
| `contact_id` | INTEGER | NULL, FK to `contacts.contact_id` | |
| `company_id` | INTEGER | NULL, FK to `companies.company_id` | Must be null when `contact_id` is populated. |
| `engagement_id` | INTEGER | NULL, FK to `engagements.engagement_id` | Optional direct engagement association. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 3. `company_notes_link`

**Primary Key:** (`company_id`, `note_id`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `company_id` | INTEGER | PK, FK to `companies.company_id`, NOT NULL | |
| `note_id` | INTEGER | PK, FK to `notes.note_id`, NOT NULL | |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 4. `company_documents_link`

**Primary Key:** (`company_id`, `document_id`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `company_id` | INTEGER | PK, FK to `companies.company_id`, NOT NULL | |
| `document_id` | INTEGER | PK, FK to `documents.document_id`, NOT NULL | |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 5. `contact_notes_link`

**Primary Key:** (`contact_id`, `note_id`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `contact_id` | INTEGER | PK, FK to `contacts.contact_id`, NOT NULL | |
| `note_id` | INTEGER | PK, FK to `notes.note_id`, NOT NULL | |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 6. `interactions_notes_link`

**Primary Key:** (`interaction_id`, `note_id`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `interaction_id` | INTEGER | PK, FK to `interactions.interaction_id`, NOT NULL | |
| `note_id` | INTEGER | PK, FK to `notes.note_id`, NOT NULL | |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 7. `interactions_documents_link`

**Primary Key:** (`interaction_id`, `document_id`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `interaction_id` | INTEGER | PK, FK to `interactions.interaction_id`, NOT NULL | |
| `document_id` | INTEGER | PK, FK to `documents.document_id`, NOT NULL | |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 8. `engagement_companies_link`

Tracks historical participation of companies in engagements.

**Primary Key:** (`engagement_id`, `company_id`, `start_date`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `engagement_id` | INTEGER | PK, FK to `engagements.engagement_id`, NOT NULL | |
| `company_id` | INTEGER | PK, FK to `companies.company_id`, NOT NULL | |
| `start_date` | DATE | PK, NOT NULL | Start of involvement. |
| `end_date` | DATE | NULL | End of involvement; null means active/open. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 9. `engagement_contacts_link`

Tracks historical participation of contacts in engagements.

**Primary Key:** (`engagement_id`, `contact_id`, `start_date`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `engagement_id` | INTEGER | PK, FK to `engagements.engagement_id`, NOT NULL | |
| `contact_id` | INTEGER | PK, FK to `contacts.contact_id`, NOT NULL | |
| `start_date` | DATE | PK, NOT NULL | Start of involvement. |
| `end_date` | DATE | NULL | End of involvement; null means active/open. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 10. `engagement_notes_link`

**Primary Key:** (`engagement_id`, `note_id`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `engagement_id` | INTEGER | PK, FK to `engagements.engagement_id`, NOT NULL | |
| `note_id` | INTEGER | PK, FK to `notes.note_id`, NOT NULL | |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 11. `engagement_documents_link`

**Primary Key:** (`engagement_id`, `document_id`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `engagement_id` | INTEGER | PK, FK to `engagements.engagement_id`, NOT NULL | |
| `document_id` | INTEGER | PK, FK to `documents.document_id`, NOT NULL | |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 12. `role_permissions_link`

Defines role access rights. This table is system configuration and is not soft-deletable.

**Primary Key:** (`role_id`, `permission_id`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `role_id` | INTEGER | PK, FK to `roles.role_id`, NOT NULL | |
| `permission_id` | INTEGER | PK, FK to `permissions.permission_id`, NOT NULL | |

---

## 🛠️ V. Development Summary & Action Items

1.  **Database Layer:** Implement all schemas defined above.
2.  **Service Layer (Critical):** Develop the `AuditService`. All CRUD operations on primary entities and all link add/remove operations must call this service.
3.  **Soft Delete Rule:** All standard read methods must enforce `is_deleted = FALSE` by default.
4.  **Recovery Rule:** Implement service methods allowing authorized restore of soft-deleted entities and link rows.
5.  **Interaction Context Rule:** `interactions_link` is the canonical contextual association record for linking an interaction to a contact, company, and/or engagement according to the defined exclusivity rules.
6.  **Lookup Rule:** UI forms and backend validation must use the defined `tag_group` vocabularies for controlled text fields.
7.  **Tag Rule:** Delimited freeform tag fields are supported on `companies`, `contacts`, `interactions`, and `engagements`, using values from the `entity_tags` group.

# LocalCRM Working Memory

This file captures resolved decisions from the interactive clarification process so far.

---

## 1. Data model decisions

### Soft delete
- All soft-deletable entities and soft-deletable link tables include:
  - `is_deleted BOOLEAN NOT NULL DEFAULT FALSE`
  - `deleted_at TIMESTAMP NULL`

### Interaction linking
- Keep `interactions_link` as a **1:1 extension** of `interactions`.
- Do **not** move `company_id` / `contact_id` directly onto `interactions`.
- If a contact is selected for an interaction, store **only `contact_id`**, not both contact and derived company.
- `interactions_link` may contain nullable:
  - `contact_id`
  - `company_id`
  - `engagement_id`
- `contact_id` and `company_id` are **mutually exclusive**.
- `engagement_id` is placed in `interactions_link`, not `interactions`.

### Historical link table key style
- Historical link tables use **composite natural keys**, not surrogate keys.
- Use:
  - `company_contacts_link`: `(company_id, contact_id, start_date)`
  - `engagement_companies_link`: `(engagement_id, company_id, start_date)`
  - `engagement_contacts_link`: `(engagement_id, contact_id, start_date)`
- Do **not** include nullable end dates in PKs.

### Company-contact relationship
- `company_contacts_link` includes:
  - `role`
- `role` is:
  - nullable
  - uses tag group `contact_role`

### Tags
- Use simple delimited text tags, not many-to-many tag link tables.
- Support tag fields on:
  - `companies`
  - `contacts`
  - `interactions`
  - `engagements`
- Use entity-specific columns:
  - `company_tags`
  - `contact_tags`
  - `interaction_tags`
  - `engagement_tags`
- Rename reusable tag group from `company_tags` to:
  - `entity_tags`

### Engagement / interaction association
- Add optional `engagement_id` association via `interactions_link`.

### Tasks
- Tasks remain a minimal specialization of `interactions`:
  - `is_task = TRUE`
- “Upcoming tasks” should be explicitly defined in spec.

### Audit logs
- Keep `audit_logs` simple.
- Do **not** add structured before/after fields.

### Audit ownership references
- Keep `created_by` / `updated_by` as username-based text values.
- Do not switch to numeric user-id audit references.

### Tag-backed lookups
- Lookup-like fields remain `TEXT` storing `tag_value`.
- These are logical references to `tags`, not strict DB-enforced composite FKs.

### Link table audit fields
- Soft-deletable link tables include:
  - `created_at`
  - `created_by`
  - `is_deleted`
  - `deleted_at`
- They do **not** include:
  - `updated_at`
  - `updated_by`

### Role-permission link
- `role_permissions_link` is system configuration.
- It is **not** soft-deletable.

### Settings
- `settings` remain non-soft-deletable system data.
- Remove audit fields from `settings`.

### Users
- Keep audit fields on `users`.

### Documents
- Change `documents.checksum` from `INTEGER` to `TEXT`.
- Make `document_ref` unique.

### Refs
- `contact_ref` and `engagement_ref` remain:
  - optional
  - unique when present

### Visibility tag group
- Shared visibility semantics for notes and documents.
- Rename group from `note_visibility` to:
  - `doc_visibility`

### Engagement confidentiality
- Keep `engagement_conf` separate from `doc_visibility`.

---

## 2. RBAC decisions

### Permission model
- Use a **hybrid RBAC model**:
  - entity-action permissions for business entities
  - coarse system permissions for system/admin capabilities

### Read vs deleted scope
- Split normal read from deleted/recovery read:
  - e.g. `companies.read`
  - `companies.read_deleted`

### Restore
- Restore is its own permission per entity.

### Link permissions
- Do **not** create separate link-table permissions.
- Link add/remove is governed by parent entity `update` permission.

### Business entities with CRUD/restore permissions
- Define per-entity permissions for:
  - companies
  - contacts
  - interactions
  - engagements
  - notes
  - documents

### Users permissions
- Hybrid user permissions:
  - `users.read`
  - `users.create`
  - `users.update`
  - `users.disable`
  - optional umbrella `users.manage`

### Roles/permissions administration
- Roles/permissions are seeded and mostly stable.
- Admins may manage `role_permissions_link`.
- Dedicated permission:
  - `roles.assign_permissions`

### Settings permissions
- Settings are admin-manageable via:
  - `settings.manage`

### Audit access
- Audit log access via dedicated permission:
  - `audit.read`

### Private visibility elevation
- Access to private notes/documents requires:
  - permission `doc_visibility.elevate`
  - explicit UI/request elevation mode
- Both are required.

### Private visibility aggregation behavior
- Without elevated mode, private notes/documents are excluded from:
  - lists
  - details
  - counts
  - aggregations
  - exports

### Creator access
- Creators may see their own private notes/documents without elevated mode.

### `team` visibility
- Keep `team` in `doc_visibility`.
- For now treat it as equivalent to normal authorized visibility until a team model exists.

---

## 3. API decisions

### API styles
- Support both:
  - GraphQL
  - REST
- Both are first-class.
- Frontend may use either per use case.

### GraphQL
- Supports:
  - reads
  - writes
  - restore mutations
  - bulk operations

### REST
- Supports:
  - reads
  - writes
  - soft-delete via normal `DELETE`
  - restore via dedicated action endpoints
  - bulk operations
  - export

### GraphQL pagination
- Use offset pagination.
- Standard list result wrapper:
  - `items`
  - `totalCount`
  - `offset`
  - `limit`

### Filtering/sorting
- Filtering and sorting are primarily **server-side**.

### REST soft delete
- `DELETE /api/{entity}/{id}` performs soft-delete.

### REST restore
- Use dedicated action endpoint:
  - `POST /api/{entity}/{id}/restore`

### GraphQL restore
- GraphQL mutations include restore operations.

### Concurrency
- Client must send `updated_at` on update.
- REST concurrency conflicts return:
  - `409 Conflict`
- GraphQL concurrency conflicts use application error code:
  - `concurrency_conflict`

### Error contracts
- REST errors use a standard payload:
  - `code`
  - `message`
  - `details`
  - `traceId`
- GraphQL errors expose equivalent metadata in `extensions`.

### Bulk operations
- Supported in both REST and GraphQL.

### CSV export
- Server-side export.
- REST only.
- Supports:
  - selected rows
  - full filtered result sets
- Always respects:
  - permissions
  - soft-delete visibility
  - private visibility
  - current elevation mode

### System metrics API
- Dedicated dashboard/system metrics API exists.
- Exposed in both REST and GraphQL.

---

## 4. Authentication decisions

### Token model
- Use:
  - JWT access token
  - refresh token
- Refresh tokens are:
  - stored server-side
  - revocable

### Disabled users
- If `is_active = FALSE`:
  - new login blocked
  - refresh tokens invalidated
  - API access denied

### Password management
- Self-service password change: yes
- Admin password reset for another user: yes

### Audit logging of auth/security events
- Failed login attempts: audit log yes
- Authorization failures (403): audit log yes
- Ordinary validation failures: no

---

## 5. Seeded roles and defaults

### Seeded roles
- `Administrator`
- `User`

### Administrator
- Receives all permissions by default.

### User baseline
- Read + create + update on:
  - companies
  - contacts
  - interactions
  - engagements
- No:
  - `*.read_deleted`
  - delete
  - restore
  - user management
  - role-permission assignment
  - settings management
  - audit access
  - `doc_visibility.elevate`

### Notes/documents for User
- Initial `User` gets:
  - `notes.read`
  - `notes.create`
  - `documents.read`
  - `documents.create`
- No general:
  - update
  - delete
  - restore
- Ownership exception:
  - may update own notes/documents
  - may soft-delete own notes/documents
  - may not restore own deleted notes/documents by default

### Export
- Dedicated permission:
  - `export.csv`
- Initial `User` gets `export.csv`.

### Dashboard/system metrics
- Initial `User` may access business dashboard.
- Initial `User` may **not** access system metrics.
- System metrics are admin-only implicitly.

### User record access
- Initial `User` may not read user records.

### Operational update scope
- Initial `User` may update any accessible:
  - companies
  - contacts
  - engagements
  - interactions/tasks
- Not limited to self-created records.

### Linking
- Initial `User` may create/remove links where they have update permission.
- May link notes/documents only if:
  - can update parent entity
  - can read the note/document

## 6. REST endpoint catalog decisions

### Base conventions
- Use uniform base pattern:
  - `/api/{entity}`
  - `/api/{entity}/{id}`
- Use database-style plural entity names in REST paths.
- Where applicable, unique business refs are also supported.
- Unique ref lookup uses dedicated path segment:
  - `/api/{entity}/by-ref/{ref}`

### Collection/list/search
- Standard collection filtering/sorting/pagination use query parameters on:
  - `GET /api/{entity}`
- Also provide:
  - `POST /api/{entity}/search`
- Search returns same paged shape as list:
  - `items`
  - `totalCount`
  - `offset`
  - `limit`

### Standard CRUD mapping
- `GET /api/{entity}` → list
- `GET /api/{entity}/{id}` → detail
- `POST /api/{entity}` → create
- `PUT /api/{entity}/{id}` → full update
- `PATCH /api/{entity}/{id}` → partial update
- `DELETE /api/{entity}/{id}` → soft-delete
- `POST /api/{entity}/{id}/restore` → restore

### Deleted records
- Provide dedicated deleted-record list endpoint:
  - `GET /api/{entity}/deleted`
- Deleted list supports same filtering/sorting/pagination as normal list.

### Bulk operations
- Use uniform subresource endpoints such as:
  - `POST /api/{entity}/bulk-delete`
  - `POST /api/{entity}/bulk-restore`
- Bulk endpoints accept explicit IDs:
  - `{ "ids": [1,2,3] }`
- Bulk endpoints also support filter-based operations.
- If both `ids` and `filter` are provided, reject request as invalid.
- Bulk restore exists for all soft-deletable business entities.
- No bulk create/update for now.

### Export
- Per-entity export endpoint:
  - `POST /api/{entity}/export`
- Export request supports either:
  - `ids`
  - or `filter`
  - but not both
- Export returns direct CSV file download response.
- Export is REST only.

### Dashboard/system
- Business dashboard endpoint:
  - `GET /api/dashboard`
- System metrics endpoint:
  - `GET /api/system/metrics`
- Keep dashboard and system metrics under separate paths.

### Auth/users
- Auth endpoints under `/api/auth/...`
- User endpoints under `/api/users/...`
- Include:
  - `POST /api/auth/login`
  - `POST /api/auth/refresh`
  - `POST /api/auth/logout`
  - `POST /api/auth/change-password`
- Admin password reset:
  - `POST /api/users/{id}/reset-password`
- Dedicated enable/disable actions:
  - `POST /api/users/{id}/disable`
  - `POST /api/users/{id}/enable`
- Users also support:
  - `GET /api/users`
  - `GET /api/users/{id}`
  - `POST /api/users`
  - `PUT /api/users/{id}`
  - `PATCH /api/users/{id}`

### Roles/permissions/settings/audit
- Role-permission assignment management endpoints exist, including:
  - `GET /api/roles`
  - `GET /api/permissions`
  - `GET /api/roles/{id}/permissions`
  - `POST /api/roles/{id}/permissions`
  - `DELETE /api/roles/{id}/permissions/{permissionId}`
- Settings endpoints:
  - `GET /api/settings`
  - `GET /api/settings/{key}`
  - `PUT /api/settings/{key}`
- Audit endpoints:
  - `GET /api/audit-logs`
  - `GET /api/audit-logs/{id}`
  - `POST /api/audit-logs/search`
- Audit endpoints require proper permission (`audit.read`).

### Relationship endpoints
- Expose link management primarily through dedicated nested relationship endpoints.
- Use nested-resource style, e.g.:
  - `POST /api/companies/{id}/contacts`
  - `DELETE /api/companies/{id}/contacts/{contactId}`
- Historical relationship payloads include temporal fields.
- Deleting historical relationships uses natural key in route/query, e.g.:
  - `DELETE /api/companies/{companyId}/contacts/{contactId}?startDate=2025-01-01`
- Related collection endpoints exist, e.g.:
  - `GET /api/companies/{id}/contacts`
  - `GET /api/companies/{id}/notes`
  - `GET /api/companies/{id}/documents`
- Related collection endpoints support filtering/sorting/pagination.
- Note/document relationship endpoints exist for all applicable parents:
  - companies
  - contacts
  - interactions
  - engagements

### Interactions API representation
- `interactions_link` is managed through main interaction create/update payloads.
- Interaction write payloads include optional:
  - `contactId`
  - `companyId`
  - `engagementId`
- Interaction detail responses expose associations in nested/link form.
- For related references, expose API URLs such as:
  - `/api/contacts/10`
  - `/api/companies/5`
  - `/api/engagements/3`

### Response style
- Use URL-reference style broadly in REST detail responses for related resources where practical.
- Collection/list items include:
  - `self`
- REST responses return both canonical IDs and URLs.
- Create returns:
  - `201 Created`
  - `Location` header
  - response body
- Update returns updated entity representation.
- Delete/restore return minimal success payload.
- List responses use uniform envelope:
  - `items`
  - `totalCount`
  - `offset`
  - `limit`

## 7. GraphQL schema convention decisions

### Endpoint
- Single GraphQL endpoint:
  - `/graphql`

### Naming conventions
- Type names use singular PascalCase.
- Collection queries use plural camelCase.
- Single-item queries use singular camelCase.

### Paged results
- Use a standard paged result type per entity, e.g.:
  - `CompanyListResult`
  - `ContactListResult`
- Fields:
  - `items`
  - `totalCount`
  - `offset`
  - `limit`

### List arguments
- Standard list query arguments:
  - `offset`
  - `limit`
  - `filter`
  - `sort`

### Filter/sort inputs
- Each entity gets typed filter and sort inputs:
  - `CompanyFilterInput`
  - `CompanySortInput`
  - etc.

### Single-item lookup
- Single-item queries support lookup by:
  - `id`
  - `ref` where applicable
- If both `id` and `ref` are provided, reject as invalid.

### Mutation naming
- Use `verbEntity` style:
  - `createCompany`
  - `updateCompany`
  - `deleteCompany`
  - `restoreCompany`

### Mutation inputs
- Create/update mutations accept typed input objects.
- Update mutations take `updatedAt` as a top-level argument.

### Mutation return shapes
- Create/update mutations return full entity objects.
- Delete/restore mutations return a minimal shared result type:
  - `MutationResult { success, id }`

### Bulk mutations
- Support explicit bulk mutation names such as:
  - `bulkDeleteCompanies`
  - `bulkRestoreCompanies`
- Bulk mutations accept either:
  - `ids`
  - or `filter`
  - but not both

### Relations
- GraphQL exposes related entities as nested object fields where useful.
- For interactions, expose flattened relation fields on `Interaction`:
  - `contact`
  - `company`
  - `engagement`
- Also expose raw contextual IDs on `Interaction`:
  - `contactId`
  - `companyId`
  - `engagementId`

### Deleted-record access
- Expose deleted-record access through separate queries:
  - `deletedCompanies(...)`
  - etc.
- Deleted queries use same filter/sort/pagination pattern as normal queries.

### Audit/dashboard/system
- Expose:
  - `auditLogs(...)`
  - `auditLog(id: Int!)`
  - `dashboard`
  - `systemMetrics`

### Auth and admin operations
- GraphQL also exposes auth mutations:
  - `login`
  - `refreshToken`
  - `logout`
  - `changePassword`
- Also expose admin user-management mutations:
  - `createUser`
  - `updateUser`
  - `disableUser`
  - `enableUser`
  - `resetUserPassword`
- Also expose role-permission operations.
- Also expose settings operations.

### Export
- CSV export remains REST-only.

### Scalars
- Use custom scalars:
  - `Date`
  - `Time`
  - `DateTime`

### URL fields
- GraphQL does not include REST-style `self`/URL fields.

### Documentation
- The spec should include a normative GraphQL operation catalog.

## 8. Authentication / JWT / session lifecycle decisions

### Token lifetimes
- Access token lifetime is configurable via settings key:
  - `token_lifetime`
- Refresh token lifetime is configurable via settings key:
  - `refresh_token_lifetime`

### Refresh token behavior
- Refresh tokens use rotation.
- Reuse of a previously rotated refresh token triggers token reuse detection and revokes the session/token family.
- Multiple concurrent refresh-token sessions per user are allowed.
- No individual-session revocation UI/API; only revoke-all-sessions per user.

### Session revocation
- Current user can revoke all own sessions.
- Admin can revoke all sessions for another user.
- Admin revoke-user-sessions requires dedicated permission:
  - `users.revoke_sessions`

### JWT claims
- Access token includes at minimum:
  - `sub`
  - `name`
  - `role`
  - `permissions`
  - `exp`
  - `sid`
  - `preferred_username`
- `sub` represents `user_id`.
- Effective permissions are included directly in the token.

### Permission/security invalidation
- Role/permission changes force session invalidation.
- Password change revokes all existing sessions.
- Admin password reset revokes all existing sessions for the target user.

### JWT validation config
- JWT validation includes issuer and audience checks.
- Settings keys:
  - `jwt_issuer`
  - `jwt_audience`
- JWT signing key/secret comes from environment/host configuration, not app settings.

### Current user/session endpoint
- Expose:
  - REST: `GET /api/auth/me`
  - GraphQL: `me`
- `me` includes:
  - username
  - role
  - effective permissions

### Auth response shapes
- Login returns:
  - access token
  - refresh token
  - current user payload
- Refresh returns:
  - new access token
  - new refresh token
  - refreshed current user payload
- Logout invalidates current session only.

### Revoke-all endpoints
- Current user:
  - REST: `POST /api/auth/revoke-all-sessions`
  - GraphQL: `revokeAllMySessions`
- Admin:
  - REST: `POST /api/users/{id}/revoke-sessions`
  - GraphQL: `revokeUserSessions(id: Int!)`

### Documentation
- Authentication/session rules should be documented as a normative auth contract.

## 9. Dashboard metric definition decisions

### Tasks card
- “Top 5 upcoming tasks” means:
  - `is_task = TRUE`
  - not deleted
  - state not terminal/closed
  - ordered by earliest future `interaction_date`, then `interaction_time`
- Terminal/closed interaction states are configured from settings key:
  - `interaction_closed_states`
- For same-day tasks with null `interaction_time`:
  - sort after timed tasks on that day
  - before tasks on later days

### Companies card
- Show both:
  - last created company
  - last updated company
- Industry segmentation is by:
  - `branch` using `company_branch`
- Size segmentation is by:
  - `size` using `company_size`
- “Interaction count per Company Industry” includes:
  - interactions linked directly to companies
  - interactions linked via contacts, deriving active company at interaction date
  - grouped by resolved company `branch`
- If company cannot be resolved for a contact-linked interaction:
  - exclude from this metric

### Contacts card
- Show both:
  - last created contact
  - last updated contact
- Total contact count excludes soft-deleted rows and respects permission filtering.
- Contact role segmentation is based on:
  - current active role from `company_contacts_link`
- If a contact has multiple active roles:
  - count once in each active role bucket
- “Interaction count per Contact Role” means:
  - interactions linked to contacts
  - grouped by contact’s active role at interaction date
- If role at interaction date cannot be resolved:
  - place in `Unknown/Unresolved` bucket

### Interactions card
- Total interaction count excludes tasks.
- Top 5 interaction types excludes tasks.

### Engagements card
- Total engagement count excludes soft-deleted rows and respects permission filtering.
- Open vs Closed engagement counts use configured closed statuses from settings key:
  - `engagement_closed_statuses`
- “Top 3 currently open engagements” are ranked by:
  - most recent associated interaction
- Open engagements with no associated interactions:
  - are included
  - sort after those with interactions
- Last interaction summary includes:
  - interaction date/time
  - subject
  - type

### System metrics
- “Last API action” means:
  - last successful write action only
- It includes:
  - entity name
  - entity id
  - action type
- “Total API call failures since startup” includes:
  - 5xx/system failures
  - auth failures
  - authorization failures
- “Total API calls made since startup” counts:
  - raw HTTP requests only
- “Total database size” means:
  - DB file + WAL/journal files when feasible
  - fallback to logical-size estimation if not feasible
- “Running time / uptime” means:
  - process uptime since current app start

### Documentation
- Dashboard metric definitions should be documented as a normative contract.

## 10. User-management and password-policy decisions

### User creation
- Newly created users are active immediately by default.
- A user must have exactly one primary role at creation time.
- Admin sets the initial password directly.

### Username rules
- Usernames are case-insensitive for login and uniqueness.
- Usernames cannot be changed after creation.

### Forced password change
- Admin-created users must change password on first login.
- Admin password reset also forces change password on next login.
- Do not expose a `mustChangePassword` boolean in normal auth success responses.
- If password change is required, login is refused with error code:
  - `password_change_required`
- Provide dedicated forced-password-change operations:
  - REST: `POST /api/auth/complete-password-change`
  - GraphQL: `completePasswordChange`
- Forced password change uses a short-lived password-change token returned by the refused login flow.
- Password-change token lifetime is configurable via:
  - `password_change_token_lifetime`

### User visibility and filtering
- Disabled users remain visible in normal admin user lists.
- User list APIs support filtering by:
  - `is_active`
  - `role_id`
  - `username`
- Non-admin users may only access their identity through:
  - `me`
  not generic user endpoints.

### Role changes
- Admins may change a user's role through normal user update operations.
- Changing a user's role triggers session invalidation.

### Password policy
- Password policy is explicitly defined.
- Minimum password length is configurable via:
  - `min_password_length`
- Password policy is length-only, no character-class diversity requirement.
- Reuse of the current password is forbidden.
- No password history beyond current password.

### Login lockout
- Repeated failed login attempts trigger temporary account lockout.
- Lockout policy is configurable.
- Settings keys:
  - `max_failed_login_attempts`
  - `login_lockout_duration`
- Lockout is tracked separately by:
  - username
  - source IP

### Documentation
- These rules should be documented as a normative user-management contract.# 🏗️ Application Architecture Design Specification (Canonical)

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
  - Dapper optionally for read-heavy queries
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
The seeded `User` role receives the baseline permissions defined in `seeded_roles_and_permissions.md`.# 📚 Data Model Specification (localCRM)

*This document defines the explicit schema, data types, primary keys (PK), foreign keys (FK), and constraints for all database tables. This version incorporates clarified modeling decisions and should be treated as the single source of truth for the database layer.*

## ⚙️ I. Global Schema Constraints & Rules

These rules apply to all core entity tables unless explicitly overridden.

### 1. Naming Conventions
*   **Table Names:** Must be plural (e.g., `companies`, `contacts`).
*   **Primary Key:** Every main entity must have an auto-incrementing integer primary key field named `[entity_name]_id`.
*   **Historical Link Tables:** Historical relationship tables use a **composite natural key** based on the two linked entity IDs plus `start_date`.

### 2. Temporal/Audit Fields (Core Entities)
All primary entity tables MUST include the following fields unless explicitly overridden:

| Field Name | Data Type | Required | Constraint | Purpose |
| :--- | :--- | :--- | :--- | :--- |
| `created_at` | TIMESTAMP | Yes | Default: CURRENT_TIMESTAMP | Time of initial record creation. |
| `created_by` | TEXT | Yes | Logical FK to `users.username` | User who created the record. |
| `updated_at` | TIMESTAMP | No | Auto-update on change | Time of last record modification. |
| `updated_by` | TEXT | No | Logical FK to `users.username` | User who last updated the record. |
| `is_deleted` | BOOLEAN | Yes | Default: `FALSE` | Soft-delete flag. |
| `deleted_at` | TIMESTAMP | No | Nullable | Timestamp of soft deletion. |

### 3. Temporal/Audit Fields (Soft-Deletable Link Tables)
Soft-deletable link tables MUST include the following fields:

| Field Name | Data Type | Required | Constraint | Purpose |
| :--- | :--- | :--- | :--- | :--- |
| `created_at` | TIMESTAMP | Yes | Default: CURRENT_TIMESTAMP | Time of initial link creation. |
| `created_by` | TEXT | Yes | Logical FK to `users.username` | User who created the link. |
| `is_deleted` | BOOLEAN | Yes | Default: `FALSE` | Soft-delete flag for the link. |
| `deleted_at` | TIMESTAMP | No | Nullable | Timestamp of soft deletion. |

### 4. Relationship Constraints (Referential Integrity)
*   **Foreign Keys (FKs):** All explicit entity relationships must be implemented as defined FKs where practical.
*   **Lookup Fields Using `tags`:** Fields such as `company_type`, `branch`, `size`, `sex`, `role`, etc. use `TEXT` values and are treated as **logical references** to `tags.tag_value` within the appropriate `tag_group`. These are not required to be strict database-level foreign keys.
*   **Linking (M:M):** Many-to-many and historical relationships use dedicated link tables.
*   **Soft Delete Scope:** All standard queries must filter `is_deleted = FALSE` unless explicitly operating in an authorized recovery/admin context.

### 5. Soft-Delete Rule
All soft-deletable entities and all soft-deletable link tables MUST include:
- `is_deleted BOOLEAN NOT NULL DEFAULT FALSE`
- `deleted_at TIMESTAMP NULL`

### 6. Task Interpretation Rule
Tasks are a specialization of `interactions`:
- `is_task = TRUE` indicates the interaction is a task.
- “Upcoming tasks” are interactions where:
  - `is_task = TRUE`
  - `is_deleted = FALSE`
  - `interaction_date` and optional `interaction_time` indicate a future point in time
  - `state` is not in a closed/completed terminal status as defined by the application’s allowed `interaction_state` values

---

## 🏷️ II. Lookup Tables & Enums

### Entity: `tags`
Defines a structured key-value pair system for categorization and controlled vocabularies.

| Field Name | Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `tag_id` | INTEGER | PK, Auto-Increment | Unique identifier. |
| `tag_group` | TEXT | Required | Defines the context (e.g., `company_type`, `contact_sex`). |
| `tag_name` | TEXT | Required | Human-readable display name. |
| `tag_value` | TEXT | Required | Value stored in referencing entity fields. |
| `tag_order` | INTEGER | Optional | Display order (Default: 0). |

### Tag Group Definitions
The following groups are treated as constrained lookup vocabularies.

| Group Name | Purpose | Values (Examples) |
| :--- | :--- | :--- |
| `entity_tags` | User-supplied freeform tags used in delimited tag fields across supported entities. | User-defined |
| `company_type` | Company classification. | `Recruitment`: `recr` \| `Client`: `clnt` \| `Services`: `svc` \| `Employer`: `empl` \| `Consulting`: `cslt` |
| `company_branch` | Industry sector classification. | `Technology`: `tech` \| `Financial Services`: `finsvc` \| ... |
| `company_size` | Company size classification. | `Micro`: `1–9 employees` \| `Small`: `10–49 employees` \| ... |
| `contact_sex` | Contact gender. | `Male`: `M` \| `Female`: `F` \| `Neutral`: `x` \| `Other`: `O` \| `Unknown`: `u` |
| `contact_role` | Contact role within a company relationship. | `Management`: `mgmt` \| `Client`: `clnt` \| ... |
| `engagement_conf` | Engagement visibility/confidentiality level. | `Public`: `pub` \| `Limited`: `lmtd` \| `Private`: `priv` |
| `engagement_status` | Current state of the engagement. | `Planned`: `plan` \| `Active`: `act` \| `On hold`: `hold` \| ... |
| `interaction_state` | Status of the interaction or task. | `Open`: `open` \| `Active`: `actv` \| `Closed`: `clsd` \| ... |
| `interaction_type` | Medium or form of interaction. | `Email`: `eml` \| `Call`: `call` \| ... |
| `interaction_direction` | Who initiated the contact. | `Inbound`: `in` \| `Outbound`: `out` \| `N/A`: `n_a` |
| `doc_visibility` | Visibility for notes and documents. | `Private`: `priv` \| `Limited`: `lmtd` \| `Team`: `team` |
| `document_type` | Type of document reference. | `Proposal`: `prop` \| `Contract`: `cntr` \| ... |

---

## 📂 III. Core Entity Schemas

### 1. Entity: `users`

Users are not soft-deletable. Users should not be deleted to preserve historic data consistency.  
Initially, on database setup, the table will be populated with an Administrator account.

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `user_id` | INTEGER | PK, Auto-Increment | |
| `username` | TEXT | UNIQUE, NOT NULL | Used for login and logical audit references. |
| `password_hash` | TEXT | NOT NULL | Stored secure password hash. |
| `role_id` | INTEGER | FK to `roles.role_id`, NOT NULL | Primary role for the user. |
| `is_active` | BOOLEAN | NOT NULL | Enabled/Disabled status. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `updated_at` | TIMESTAMP | NULL | |
| `updated_by` | TEXT | NULL | |

---

### 2. Entity: `roles`

Roles are system data and are not soft-deletable.

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `role_id` | INTEGER | PK, Auto-Increment | |
| `role_name` | TEXT | UNIQUE, NOT NULL | Human-readable role name. |

---

### 3. Entity: `permissions`

Permissions are system data and are not soft-deletable.

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `permission_id` | INTEGER | PK, Auto-Increment | |
| `permission_name` | TEXT | UNIQUE, NOT NULL | e.g., `read_all`, `manage_users`. |

---

### 4. Entity: `settings`

Settings are system data and are not soft-deletable.  
If a setting key is missing, the application should use an internal default.

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `setting_id` | INTEGER | PK, Auto-Increment | |
| `key` | TEXT | UNIQUE, NOT NULL | Compound key allowed (e.g., `company.self`). |
| `value` | TEXT | NOT NULL | Configuration value. |

---

### 5. Entity: `companies`

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `company_id` | INTEGER | PK, Auto-Increment | |
| `company_ref` | TEXT | UNIQUE, NOT NULL | Secondary business reference. |
| `name` | TEXT | NOT NULL | Legal/company name. |
| `address_line_1` | TEXT | NULL | |
| `address_line_2` | TEXT | NULL | |
| `postal_code` | TEXT | NULL | |
| `city` | TEXT | NOT NULL | |
| `country` | TEXT | NULL | |
| `phone` | TEXT | NULL | |
| `website` | TEXT | NULL | |
| `email` | TEXT | NULL | General email contact. |
| `company_tags` | TEXT | NULL | Delimited list of `tag_value` entries from `entity_tags`. |
| `company_type` | TEXT | NOT NULL | Logical reference to `tags.tag_value` in `company_type`. |
| `branch` | TEXT | NULL | Logical reference to `tags.tag_value` in `company_branch`. |
| `size` | TEXT | NULL | Logical reference to `tags.tag_value` in `company_size`. |
| `rating` | INTEGER | NOT NULL | Trustworthiness rating (expected range 0-4). |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `updated_at` | TIMESTAMP | NULL | |
| `updated_by` | TEXT | NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 6. Entity: `contacts`

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `contact_id` | INTEGER | PK, Auto-Increment | |
| `contact_ref` | TEXT | UNIQUE, NULL | Optional secondary business reference. |
| `first_name` | TEXT | NOT NULL | |
| `middle_name` | TEXT | NULL | |
| `last_name` | TEXT | NOT NULL | |
| `phone` | TEXT | NULL | |
| `email` | TEXT | NULL | |
| `linkedin_url` | TEXT | NULL | Optional LinkedIn URL. |
| `birthdate` | DATE | NULL | |
| `contact_tags` | TEXT | NULL | Delimited list of `tag_value` entries from `entity_tags`. |
| `rating` | INTEGER | NOT NULL | Trustworthiness rating (expected range 0-4). |
| `sex` | TEXT | NULL | Logical reference to `tags.tag_value` in `contact_sex`. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `updated_at` | TIMESTAMP | NULL | |
| `updated_by` | TEXT | NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 7. Entity: `interactions`

Tasks are represented using `is_task = TRUE`.

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `interaction_id` | INTEGER | PK, Auto-Increment | |
| `interaction_date` | DATE | NOT NULL | Date of the interaction or planned task. |
| `interaction_time` | TIME | NULL | Time of the interaction or task. |
| `direction` | TEXT | NULL | Logical reference to `tags.tag_value` in `interaction_direction`. |
| `interaction_type` | TEXT | NOT NULL | Logical reference to `tags.tag_value` in `interaction_type`. |
| `subject` | TEXT | NOT NULL | Subject/title. |
| `note` | TEXT | NULL | Summary notes. |
| `state` | TEXT | NOT NULL | Logical reference to `tags.tag_value` in `interaction_state`. |
| `prev_interaction_id` | INTEGER | NULL, FK to `interactions.interaction_id` | Link to a previous related interaction. |
| `interaction_tags` | TEXT | NULL | Delimited list of `tag_value` entries from `entity_tags`. |
| `is_task` | BOOLEAN | NOT NULL | `TRUE` if this record is a task. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `updated_at` | TIMESTAMP | NULL | |
| `updated_by` | TEXT | NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 8. Entity: `engagements`

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `engagement_id` | INTEGER | PK, Auto-Increment | |
| `engagement_ref` | TEXT | UNIQUE, NULL | Optional shorthand reference. |
| `description` | TEXT | NULL | Summary description. |
| `engagement_tags` | TEXT | NULL | Delimited list of `tag_value` entries from `entity_tags`. |
| `confidentiality` | TEXT | NULL | Logical reference to `tags.tag_value` in `engagement_conf`. |
| `engagement_status` | TEXT | NOT NULL | Logical reference to `tags.tag_value` in `engagement_status`. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `updated_at` | TIMESTAMP | NULL | |
| `updated_by` | TEXT | NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 9. Entity: `notes`

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `note_id` | INTEGER | PK, Auto-Increment | |
| `subject` | TEXT | NOT NULL | Short description. |
| `body` | TEXT | NULL | Full note body. |
| `visibility` | TEXT | NOT NULL | Logical reference to `tags.tag_value` in `doc_visibility`. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `updated_at` | TIMESTAMP | NULL | |
| `updated_by` | TEXT | NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 10. Entity: `documents`

This entity stores metadata/reference information only, not binary payloads.

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `document_id` | INTEGER | PK, Auto-Increment | |
| `document_ref` | TEXT | UNIQUE, NOT NULL | Shorthand reference code. |
| `subject` | TEXT | NULL | Optional description. |
| `document_type` | TEXT | NOT NULL | Logical reference to `tags.tag_value` in `document_type`. |
| `document_url` | TEXT | NOT NULL | URL or path to the document reference. |
| `visibility` | TEXT | NOT NULL | Logical reference to `tags.tag_value` in `doc_visibility`. |
| `checksum` | TEXT | NULL | Optional checksum/hash value for verification. |
| `is_checked` | BOOLEAN | NOT NULL, Default `FALSE` | If `TRUE`, `checksum` should be present and valid. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `updated_at` | TIMESTAMP | NULL | |
| `updated_by` | TEXT | NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 11. Entity: `audit_logs`

This table is append-only. Audit log rows must never be updated or deleted by normal application logic.

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `audit_id` | INTEGER | PK, Auto-Increment | |
| `entity_name` | TEXT | NOT NULL | Table/entity name involved (e.g., `companies`). |
| `entity_id` | INTEGER | NOT NULL | PK value of the affected record. |
| `action_type` | TEXT | NOT NULL | Fixed set: `CREATE`, `UPDATE`, `DELETE`, `SOFT_DELETE`, `RESTORE`, `LINK_ADD`, `LINK_REMOVE`, `ERROR`. |
| `performed_at` | TIMESTAMP | NOT NULL, Default CURRENT_TIMESTAMP | |
| `performed_by` | TEXT | NOT NULL | Username of acting user or `system`. |
| `notes` | TEXT | NULL | Optional descriptive text. |

## Additional Security Support Entities

### Entity: `refresh_tokens`

Stores server-side refresh token sessions for revocation, rotation, and reuse detection.

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `refresh_token_id` | INTEGER | PK, Auto-Increment | |
| `user_id` | INTEGER | FK to `users.user_id`, NOT NULL | |
| `session_id` | TEXT | NOT NULL | Stable session-family identifier; corresponds to JWT `sid`. |
| `token_hash` | TEXT | NOT NULL | Hash of refresh token value. |
| `issued_at` | TIMESTAMP | NOT NULL | |
| `expires_at` | TIMESTAMP | NOT NULL | |
| `revoked_at` | TIMESTAMP | NULL | |
| `replaced_by_token_id` | INTEGER | NULL, FK to `refresh_tokens.refresh_token_id` | Rotation chain support. |
| `reuse_detected_at` | TIMESTAMP | NULL | Token reuse detection marker. |
| `created_by_ip` | TEXT | NULL | Optional source IP. |
| `user_agent` | TEXT | NULL | Optional client descriptor. |

### Add fields to `users`

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `must_change_password` | BOOLEAN | NOT NULL, Default `FALSE` | Required password change before normal login. |
| `failed_login_attempts` | INTEGER | NOT NULL, Default `0` | Optional user-level lockout support. |
| `locked_until` | TIMESTAMP | NULL | Temporary lockout end time. |

---

## 🔗 IV. Relationship (Link) Tables

### 1. `company_contacts_link`

Tracks historical relationships between companies and contacts.

**Primary Key:** (`company_id`, `contact_id`, `start_date`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `company_id` | INTEGER | PK, FK to `companies.company_id`, NOT NULL | |
| `contact_id` | INTEGER | PK, FK to `contacts.contact_id`, NOT NULL | |
| `start_date` | DATE | PK, NOT NULL | Start of relationship. |
| `end_date` | DATE | NULL | End of relationship; null means still active/open. |
| `role` | TEXT | NULL | Logical reference to `tags.tag_value` in `contact_role`. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 2. `interactions_link`

Provides the contextual 1:1 linkage for an interaction.

**Primary Key:** (`interaction_id`)

Rules:
- One row per interaction.
- `contact_id` and `company_id` are **mutually exclusive**.
- If `contact_id` is present, `company_id` must be `NULL`.
- `engagement_id` is independent and may be present or null.
- Company may be derived from the contact’s active company at the interaction date/time in application logic.

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `interaction_id` | INTEGER | PK, FK to `interactions.interaction_id`, NOT NULL | |
| `contact_id` | INTEGER | NULL, FK to `contacts.contact_id` | |
| `company_id` | INTEGER | NULL, FK to `companies.company_id` | Must be null when `contact_id` is populated. |
| `engagement_id` | INTEGER | NULL, FK to `engagements.engagement_id` | Optional direct engagement association. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 3. `company_notes_link`

**Primary Key:** (`company_id`, `note_id`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `company_id` | INTEGER | PK, FK to `companies.company_id`, NOT NULL | |
| `note_id` | INTEGER | PK, FK to `notes.note_id`, NOT NULL | |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 4. `company_documents_link`

**Primary Key:** (`company_id`, `document_id`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `company_id` | INTEGER | PK, FK to `companies.company_id`, NOT NULL | |
| `document_id` | INTEGER | PK, FK to `documents.document_id`, NOT NULL | |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 5. `contact_notes_link`

**Primary Key:** (`contact_id`, `note_id`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `contact_id` | INTEGER | PK, FK to `contacts.contact_id`, NOT NULL | |
| `note_id` | INTEGER | PK, FK to `notes.note_id`, NOT NULL | |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 6. `interactions_notes_link`

**Primary Key:** (`interaction_id`, `note_id`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `interaction_id` | INTEGER | PK, FK to `interactions.interaction_id`, NOT NULL | |
| `note_id` | INTEGER | PK, FK to `notes.note_id`, NOT NULL | |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 7. `interactions_documents_link`

**Primary Key:** (`interaction_id`, `document_id`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `interaction_id` | INTEGER | PK, FK to `interactions.interaction_id`, NOT NULL | |
| `document_id` | INTEGER | PK, FK to `documents.document_id`, NOT NULL | |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 8. `engagement_companies_link`

Tracks historical participation of companies in engagements.

**Primary Key:** (`engagement_id`, `company_id`, `start_date`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `engagement_id` | INTEGER | PK, FK to `engagements.engagement_id`, NOT NULL | |
| `company_id` | INTEGER | PK, FK to `companies.company_id`, NOT NULL | |
| `start_date` | DATE | PK, NOT NULL | Start of involvement. |
| `end_date` | DATE | NULL | End of involvement; null means active/open. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 9. `engagement_contacts_link`

Tracks historical participation of contacts in engagements.

**Primary Key:** (`engagement_id`, `contact_id`, `start_date`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `engagement_id` | INTEGER | PK, FK to `engagements.engagement_id`, NOT NULL | |
| `contact_id` | INTEGER | PK, FK to `contacts.contact_id`, NOT NULL | |
| `start_date` | DATE | PK, NOT NULL | Start of involvement. |
| `end_date` | DATE | NULL | End of involvement; null means active/open. |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 10. `engagement_notes_link`

**Primary Key:** (`engagement_id`, `note_id`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `engagement_id` | INTEGER | PK, FK to `engagements.engagement_id`, NOT NULL | |
| `note_id` | INTEGER | PK, FK to `notes.note_id`, NOT NULL | |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 11. `engagement_documents_link`

**Primary Key:** (`engagement_id`, `document_id`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `engagement_id` | INTEGER | PK, FK to `engagements.engagement_id`, NOT NULL | |
| `document_id` | INTEGER | PK, FK to `documents.document_id`, NOT NULL | |
| `created_at` | TIMESTAMP | NOT NULL | |
| `created_by` | TEXT | NOT NULL | |
| `is_deleted` | BOOLEAN | NOT NULL, Default `FALSE` | |
| `deleted_at` | TIMESTAMP | NULL | |

---

### 12. `role_permissions_link`

Defines role access rights. This table is system configuration and is not soft-deletable.

**Primary Key:** (`role_id`, `permission_id`)

| Field Name | Data Type | Constraints | Notes |
| :--- | :--- | :--- | :--- |
| `role_id` | INTEGER | PK, FK to `roles.role_id`, NOT NULL | |
| `permission_id` | INTEGER | PK, FK to `permissions.permission_id`, NOT NULL | |

---

## 🛠️ V. Development Summary & Action Items

1.  **Database Layer:** Implement all schemas defined above.
2.  **Service Layer (Critical):** Develop the `AuditService`. All CRUD operations on primary entities and all link add/remove operations must call this service.
3.  **Soft Delete Rule:** All standard read methods must enforce `is_deleted = FALSE` by default.
4.  **Recovery Rule:** Implement service methods allowing authorized restore of soft-deleted entities and link rows.
5.  **Interaction Context Rule:** `interactions_link` is the canonical contextual association record for linking an interaction to a contact, company, and/or engagement according to the defined exclusivity rules.
6.  **Lookup Rule:** UI forms and backend validation must use the defined `tag_group` vocabularies for controlled text fields.
7.  **Tag Rule:** Delimited freeform tag fields are supported on `companies`, `contacts`, `interactions`, and `engagements`, using values from the `entity_tags` group.

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
  - Dapper optionally for read-heavy queries
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
The seeded `User` role receives the baseline permissions defined in `seeded_roles_and_permissions.md`.# 🎯 localCRM Application Development Specification (Canonical Overview)

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
- permission-aware UI behavior and restricted feature rendering# 📚 LocalCRM Canonical Spec Index

This index lists the canonical specification files that together define the current agreed system behavior.

## Core
- `overview.md`
- `architecture.md`
- `technology_stack.md`
- `requirements/requirement_details.md`
- `data_modeling/data_dictionary.md`
- `ui_ux_design.md`

## Clarifications and Contracts
- `rbac_and_api_clarifications.md`
- `seeded_roles_and_permissions.md`
- `rest_endpoint_catalog.md`
- `graphql_operation_catalog.md`
- `auth_contract.md`
- `dashboard_metrics_contract.md`
- `user_management_contract.md`

## Working Reference
- `working_memory_consolidated.md`# 🛠️ Technology Stack Specification (LocalCRM)

*This document specifies all mandated technologies, frameworks, and libraries. Agents must install and utilize these dependencies to ensure system consistency.*

## 📊 I. Stack Summary Overview

| Layer | Component | Technology | Version Policy | Key Purpose |
| :--- | :--- | :--- | :--- | :--- |
| **Backend (API)** | Core Language | C# / .NET | Latest Stable LTS | Business Logic Execution, API Hosting. |
| | Web Framework | ASP.NET Core Web API | Latest Stable LTS | Exposing REST/GraphQL endpoints. |
| | Documentation | Swagger (Swashbuckle) | 6.5.0 | Interactive API testing and discovery. |
| | Data Access (ORM) | Entity Framework Core | Latest Stable LTS | Managing domain models and transactions (CRUD). |
| | Data Access (Mapper) | Dapper | Latest Stable LTS | High-performance data read operations. |
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
| **Data Access (Secondary)** | Dapper | Latest Stable LTS | Used for optimized, high-speed read queries (SELECT). | Reserved for read-heavy endpoints (e.g., `GET /companies`). |

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
# ✨ UI/UX Specification (Canonical)

## 📐 I. General SPA Rules
- Application is a SPA.
- First unauthenticated screen is Login.
- First authenticated screen is Dashboard.
- UI must enforce route and action visibility based on permissions.
- UI must support explicit private-visibility elevation mode when user has `doc_visibility.elevate`.

---

## 🧩 II. Core Reusable Components

### 1. Card
Used for dashboard summaries and system metrics.

### 2. List Table
Reusable listing component supporting:
- server-side filtering
- server-side sorting
- server-side pagination
- selection for bulk actions
- CSV export initiation

### 3. EditableForm
Supports:
- read-only initial state
- explicit Edit mode
- Save / Cancel controls always visible
- local validation before API call
- field-level validation feedback

### 4. Modal/Dialog
Used for:
- confirmations
- soft-delete confirmation
- unsaved changes confirmation
- temporary alerts

---

## 🔒 III. Security and Authorization UX

### 1. Authentication Flow
- User logs in with username/password.
- If login succeeds, navigate to Dashboard.
- If login fails with `password_change_required`, navigate to forced-password-change screen.
- If lockout or invalid credentials occurs, show appropriate generic/auth-specific errors.

### 2. UI Guarding
Buttons and pages must be conditionally rendered based on effective permissions.

### 3. Private Visibility
Private notes/documents:
- are hidden by default
- require explicit elevation mode and permission `doc_visibility.elevate`
- creators may still see their own private items without elevated mode

---

## 🌐 IV. Dashboard

### 1. Tasks Card
- top-most, full-width
- shows Top 5 upcoming tasks according to dashboard contract

### 2. Entity Cards
Must include:
- Companies
- Contacts
- Interactions
- Engagements

### 3. System Metrics Cards
Shown only when authorized for system/admin scope.

### 4. Navigation
Clicking a card navigates to the relevant list/detail module.

---

## 📄 V. List Pages

List pages must include:
- filter controls
- paginated result table
- Add New
- Export
- View Details
- Delete where authorized
- bulk actions where authorized

Filtering and sorting are server-side.  
CSV export uses backend export endpoints.

---

## 📝 VI. Detail/Edit Pages

Detail pages must:
- start read-only unless creating new entity
- support explicit Edit mode
- support link management UI where applicable
- support note/document relationship management
- support interaction contextual fields (`contactId`, `companyId`, `engagementId`) in the interaction form

---

## 👤 VII. User Management UX

The UI must support:
- user listing
- create user
- update user
- enable/disable user
- reset password
- revoke user sessions
- change own password
- complete forced password change flow

Non-admin users must access self-context only via the equivalent of `me`, not via generic user-administration views.# 🎯 localCRM Application Development Specification (Canonical Overview)

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
- permission-aware UI behavior and restricted feature rendering# 📚 LocalCRM Canonical Spec Index

This index lists the canonical specification files that together define the current agreed system behavior.

## Core
- `overview.md`
- `architecture.md`
- `technology_stack.md`
- `requirements/requirement_details.md`
- `data_modeling/data_dictionary.md`
- `ui_ux_design.md`

## Clarifications and Contracts
- `rbac_and_api_clarifications.md`
- `seeded_roles_and_permissions.md`
- `rest_endpoint_catalog.md`
- `graphql_operation_catalog.md`
- `auth_contract.md`
- `dashboard_metrics_contract.md`
- `user_management_contract.md`

## Working Reference
- `working_memory_consolidated.md`# 🛠️ Technology Stack Specification (LocalCRM)

*This document specifies all mandated technologies, frameworks, and libraries. Agents must install and utilize these dependencies to ensure system consistency.*

## 📊 I. Stack Summary Overview

| Layer | Component | Technology | Version Policy | Key Purpose |
| :--- | :--- | :--- | :--- | :--- |
| **Backend (API)** | Core Language | C# / .NET | Latest Stable LTS | Business Logic Execution, API Hosting. |
| | Web Framework | ASP.NET Core Web API | Latest Stable LTS | Exposing REST/GraphQL endpoints. |
| | Documentation | Swagger (Swashbuckle) | 6.5.0 | Interactive API testing and discovery. |
| | Data Access (ORM) | Entity Framework Core | Latest Stable LTS | Managing domain models and transactions (CRUD). |
| | Data Access (Mapper) | Dapper | Latest Stable LTS | High-performance data read operations. |
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
| **Data Access (Secondary)** | Dapper | Latest Stable LTS | Used for optimized, high-speed read queries (SELECT). | Reserved for read-heavy endpoints (e.g., `GET /companies`). |

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
# ✨ UI/UX Specification (Canonical)

## 📐 I. General SPA Rules
- Application is a SPA.
- First unauthenticated screen is Login.
- First authenticated screen is Dashboard.
- UI must enforce route and action visibility based on permissions.
- UI must support explicit private-visibility elevation mode when user has `doc_visibility.elevate`.

---

## 🧩 II. Core Reusable Components

### 1. Card
Used for dashboard summaries and system metrics.

### 2. List Table
Reusable listing component supporting:
- server-side filtering
- server-side sorting
- server-side pagination
- selection for bulk actions
- CSV export initiation

### 3. EditableForm
Supports:
- read-only initial state
- explicit Edit mode
- Save / Cancel controls always visible
- local validation before API call
- field-level validation feedback

### 4. Modal/Dialog
Used for:
- confirmations
- soft-delete confirmation
- unsaved changes confirmation
- temporary alerts

---

## 🔒 III. Security and Authorization UX

### 1. Authentication Flow
- User logs in with username/password.
- If login succeeds, navigate to Dashboard.
- If login fails with `password_change_required`, navigate to forced-password-change screen.
- If lockout or invalid credentials occurs, show appropriate generic/auth-specific errors.

### 2. UI Guarding
Buttons and pages must be conditionally rendered based on effective permissions.

### 3. Private Visibility
Private notes/documents:
- are hidden by default
- require explicit elevation mode and permission `doc_visibility.elevate`
- creators may still see their own private items without elevated mode

---

## 🌐 IV. Dashboard

### 1. Tasks Card
- top-most, full-width
- shows Top 5 upcoming tasks according to dashboard contract

### 2. Entity Cards
Must include:
- Companies
- Contacts
- Interactions
- Engagements

### 3. System Metrics Cards
Shown only when authorized for system/admin scope.

### 4. Navigation
Clicking a card navigates to the relevant list/detail module.

---

## 📄 V. List Pages

List pages must include:
- filter controls
- paginated result table
- Add New
- Export
- View Details
- Delete where authorized
- bulk actions where authorized

Filtering and sorting are server-side.  
CSV export uses backend export endpoints.

---

## 📝 VI. Detail/Edit Pages

Detail pages must:
- start read-only unless creating new entity
- support explicit Edit mode
- support link management UI where applicable
- support note/document relationship management
- support interaction contextual fields (`contactId`, `companyId`, `engagementId`) in the interaction form

---

## 👤 VII. User Management UX

The UI must support:
- user listing
- create user
- update user
- enable/disable user
- reset password
- revoke user sessions
- change own password
- complete forced password change flow

Non-admin users must access self-context only via the equivalent of `me`, not via generic user-administration views.