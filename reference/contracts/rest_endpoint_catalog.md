# 🔌 REST Endpoint Catalog (LocalCRM)

## I. Base Conventions
- `/api/{entity}`
- `/api/{entity}/{id}`
- `/api/{entity}/by-ref/{ref}` where applicable
- plural entity naming

---

## II. Standard Entity Endpoints

### Companies
- `GET /api/companies`
- `POST /api/companies`
- `POST /api/companies/search`
- `GET /api/companies/deleted`
- `GET /api/companies/{id}`
- `GET /api/companies/by-ref/{company_ref}`
- `PUT /api/companies/{id}`
- `PATCH /api/companies/{id}`
- `DELETE /api/companies/{id}`
- `POST /api/companies/{id}/restore`
- `POST /api/companies/bulk-delete`
- `POST /api/companies/bulk-restore`
- `POST /api/companies/export`

### Contacts
- `GET /api/contacts`
- `POST /api/contacts`
- `POST /api/contacts/search`
- `GET /api/contacts/deleted`
- `GET /api/contacts/{id}`
- `GET /api/contacts/by-ref/{contact_ref}`
- `PUT /api/contacts/{id}`
- `PATCH /api/contacts/{id}`
- `DELETE /api/contacts/{id}`
- `POST /api/contacts/{id}/restore`
- `POST /api/contacts/bulk-delete`
- `POST /api/contacts/bulk-restore`
- `POST /api/contacts/export`

### Interactions
- `GET /api/interactions`
- `POST /api/interactions`
- `POST /api/interactions/search`
- `GET /api/interactions/deleted`
- `GET /api/interactions/{id}`
- `PUT /api/interactions/{id}`
- `PATCH /api/interactions/{id}`
- `DELETE /api/interactions/{id}`
- `POST /api/interactions/{id}/restore`
- `POST /api/interactions/bulk-delete`
- `POST /api/interactions/bulk-restore`
- `POST /api/interactions/export`

### Engagements
- `GET /api/engagements`
- `POST /api/engagements`
- `POST /api/engagements/search`
- `GET /api/engagements/deleted`
- `GET /api/engagements/{id}`
- `GET /api/engagements/by-ref/{engagement_ref}`
- `PUT /api/engagements/{id}`
- `PATCH /api/engagements/{id}`
- `DELETE /api/engagements/{id}`
- `POST /api/engagements/{id}/restore`
- `POST /api/engagements/bulk-delete`
- `POST /api/engagements/bulk-restore`
- `POST /api/engagements/export`

### Notes
- `GET /api/notes`
- `POST /api/notes`
- `POST /api/notes/search`
- `GET /api/notes/deleted`
- `GET /api/notes/{id}`
- `PUT /api/notes/{id}`
- `PATCH /api/notes/{id}`
- `DELETE /api/notes/{id}`
- `POST /api/notes/{id}/restore`
- `POST /api/notes/bulk-delete`
- `POST /api/notes/bulk-restore`
- `POST /api/notes/export`

### Documents
- `GET /api/documents`
- `POST /api/documents`
- `POST /api/documents/search`
- `GET /api/documents/deleted`
- `GET /api/documents/{id}`
- `GET /api/documents/by-ref/{document_ref}`
- `PUT /api/documents/{id}`
- `PATCH /api/documents/{id}`
- `DELETE /api/documents/{id}`
- `POST /api/documents/{id}/restore`
- `POST /api/documents/bulk-delete`
- `POST /api/documents/bulk-restore`
- `POST /api/documents/export`

---

## III. Auth/User/System Endpoints

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

### Roles / Permissions
- `GET /api/roles`
- `GET /api/permissions`
- `GET /api/roles/{id}/permissions`
- `POST /api/roles/{id}/permissions`
- `DELETE /api/roles/{id}/permissions/{permissionId}`

### Settings
- `GET /api/settings`
- `GET /api/settings/{key}`
- `PUT /api/settings/{key}`

### Audit
- `GET /api/audit-logs`
- `GET /api/audit-logs/{id}`
- `POST /api/audit-logs/search`

### Dashboard / System
- `GET /api/dashboard`
- `GET /api/system/metrics`

---

## IV. Relationship Endpoints

### Company relationships
- `GET /api/companies/{id}/contacts`
- `POST /api/companies/{id}/contacts`
- `DELETE /api/companies/{companyId}/contacts/{contactId}?startDate=YYYY-MM-DD`

- `GET /api/companies/{id}/notes`
- `POST /api/companies/{id}/notes`
- `DELETE /api/companies/{id}/notes/{noteId}`

- `GET /api/companies/{id}/documents`
- `POST /api/companies/{id}/documents`
- `DELETE /api/companies/{id}/documents/{documentId}`

### Contact relationships
- `GET /api/contacts/{id}/notes`
- `POST /api/contacts/{id}/notes`
- `DELETE /api/contacts/{id}/notes/{noteId}`

### Interaction relationships
- `GET /api/interactions/{id}/notes`
- `POST /api/interactions/{id}/notes`
- `DELETE /api/interactions/{id}/notes/{noteId}`

- `GET /api/interactions/{id}/documents`
- `POST /api/interactions/{id}/documents`
- `DELETE /api/interactions/{id}/documents/{documentId}`

### Engagement relationships
- `GET /api/engagements/{id}/companies`
- `POST /api/engagements/{id}/companies`
- `DELETE /api/engagements/{engagementId}/companies/{companyId}?startDate=YYYY-MM-DD`

- `GET /api/engagements/{id}/contacts`
- `POST /api/engagements/{id}/contacts`
- `DELETE /api/engagements/{engagementId}/contacts/{contactId}?startDate=YYYY-MM-DD`

- `GET /api/engagements/{id}/notes`
- `POST /api/engagements/{id}/notes`
- `DELETE /api/engagements/{id}/notes/{noteId}`

- `GET /api/engagements/{id}/documents`
- `POST /api/engagements/{id}/documents`
- `DELETE /api/engagements/{id}/documents/{documentId}`