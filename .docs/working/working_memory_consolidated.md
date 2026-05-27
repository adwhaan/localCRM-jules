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
- These rules should be documented as a normative user-management contract.