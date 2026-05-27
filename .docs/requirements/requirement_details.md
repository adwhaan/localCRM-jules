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
*   [ ] **Admin Scope:** An isolated, restricted path exists for Admin users to view and restore deleted records.