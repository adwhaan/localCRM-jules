# 👥 Seeded Roles and Permission Matrix (MyCRM)

*This document defines the normative default seeded roles and permission assignments for MyCRM. These defaults are applied during initialization and may later be adjusted through authorized administration where allowed.*

---

# I. Seeded Roles

The application must seed the following roles during initialization:

1. `Administrator`
2. `User`

---

# II. Permission Model Reference

## 1. Business Entity Permissions
Each soft-deletable business entity supports:

- `{entity}.read`
- `{entity}.read_deleted`
- `{entity}.create`
- `{entity}.update`
- `{entity}.delete`
- `{entity}.restore`

Where `{entity}` includes:
- `companies`
- `contacts`
- `interactions`
- `engagements`
- `notes`
- `documents`

## 2. User and System Permissions
Additional permissions include:

- `users.read`
- `users.create`
- `users.update`
- `users.disable`
- `users.manage`

- `roles.assign_permissions`
- `settings.manage`
- `audit.read`
- `doc_visibility.elevate`
- `export.csv`

## 3. Link Management Rule
Link creation/removal is governed by the relevant parent entity's `update` permission and does not use separate link-table permissions.

---

# III. Seeded Role: Administrator

## 1. Assignment Rule
The seeded `Administrator` role receives **all available permissions** by default.

This includes:
- all business entity read/create/update/delete/restore permissions
- all read_deleted permissions
- all user-management permissions
- role-permission assignment permission
- settings management
- audit log access
- CSV export
- private visibility elevation

## 2. Admin Scope Behavior
Administrators may:
- view and restore soft-deleted records where permitted by permission
- access audit logs
- manage users
- manage settings
- assign permissions to roles
- use elevated private visibility mode when explicitly activated

Note: private note/document visibility still requires explicit elevated mode even for administrators.

---

# IV. Seeded Role: User

## 1. General Rule
The seeded `User` role is intended as a standard operational role.

It receives:
- read, create, and update permissions on core business entities
- no deleted-record read scope
- no general delete/restore permissions on business entities
- no administrative/system-management permissions
- no private visibility elevation permission

## 2. Granted Permissions

### 2.1 Companies
- `companies.read`
- `companies.create`
- `companies.update`

Not granted:
- `companies.read_deleted`
- `companies.delete`
- `companies.restore`

### 2.2 Contacts
- `contacts.read`
- `contacts.create`
- `contacts.update`

Not granted:
- `contacts.read_deleted`
- `contacts.delete`
- `contacts.restore`

### 2.3 Interactions
- `interactions.read`
- `interactions.create`
- `interactions.update`

Not granted:
- `interactions.read_deleted`
- `interactions.delete`
- `interactions.restore`

### 2.4 Engagements
- `engagements.read`
- `engagements.create`
- `engagements.update`

Not granted:
- `engagements.read_deleted`
- `engagements.delete`
- `engagements.restore`

### 2.5 Notes
- `notes.read`
- `notes.create`

Not granted by default:
- `notes.read_deleted`
- `notes.update`
- `notes.delete`
- `notes.restore`

### 2.6 Documents
- `documents.read`
- `documents.create`

Not granted by default:
- `documents.read_deleted`
- `documents.update`
- `documents.delete`
- `documents.restore`

### 2.7 Export
- `export.csv`

### 2.8 Not Granted
The seeded `User` role does **not** receive:
- `users.read`
- `users.create`
- `users.update`
- `users.disable`
- `users.manage`
- `roles.assign_permissions`
- `settings.manage`
- `audit.read`
- `doc_visibility.elevate`

---

# V. Ownership-Based Exceptions for the Seeded User Role

The seeded `User` role has limited ownership-based exceptions for notes and documents.

## 1. Own Note/Document Update
Even though `notes.update` and `documents.update` are not granted generally, a seeded `User` may update:
- notes they created
- documents they created

## 2. Own Note/Document Delete
Even though `notes.delete` and `documents.delete` are not granted generally, a seeded `User` may soft-delete:
- notes they created
- documents they created

## 3. No Own Restore Exception
The seeded `User` role may **not** restore even their own soft-deleted notes/documents unless explicitly granted restore permissions through role configuration.

---

# VI. Data Access Behavior for the Seeded User Role

## 1. Update Scope
The seeded `User` role may update any accessible:
- company
- contact
- interaction/task
- engagement

It is **not** limited only to records the user created.

## 2. Interaction/Task Scope
The seeded `User` role may create and update interactions/tasks as part of normal operational usage.

## 3. Link Management
Because link management is governed by parent entity `update` permission, the seeded `User` role may create/remove links where they hold the necessary update permission.

Examples:
- link contacts to companies
- link companies/contacts to engagements
- link notes/documents to accessible parent entities

## 4. Note/Document Linking Constraint
The seeded `User` role may link notes/documents to a parent entity only if:
1. the user can update the parent entity, and
2. the user can read the note/document being linked

---

# VII. Password and Authentication Behavior for the Seeded User Role

The seeded `User` role:
- may authenticate normally if `is_active = TRUE`
- may change its own password without needing user-management permissions
- may not read user records
- may not administer other users

---

# VIII. Dashboard and Metrics Access

## 1. Business Dashboard
The seeded `User` role may access the normal business dashboard, subject to standard entity read permissions and visibility filtering.

## 2. System Metrics
The seeded `User` role may **not** access system metrics by default.

System metrics access is treated as administrator-only by default.

---

# IX. Implementation Notes

1. These seeded role assignments are the normative default baseline.
2. Future role customization may extend or reduce granted permissions through `role_permissions_link`.
3. Ownership-based exceptions for notes/documents must be enforced in the service layer, not only the UI.
4. Export behavior must always respect actual read permissions and visibility rules, even if `export.csv` is granted.
5. UI feature visibility must reflect both role permissions and ownership-based exceptions where applicable.