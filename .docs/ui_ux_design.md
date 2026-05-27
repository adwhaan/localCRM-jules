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

Non-admin users must access self-context only via the equivalent of `me`, not via generic user-administration views.