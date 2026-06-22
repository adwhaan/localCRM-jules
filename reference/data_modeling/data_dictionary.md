# 📚 Data Model Specification (localCRM)

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
