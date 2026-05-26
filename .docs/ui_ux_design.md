# ✨ User Interface / User Experience (UI/UX) Specification

*This document specifies the required appearance, component behavior, and interaction flows for the frontend. All agents must prioritize consistency, usability, and strict adherence to the established workflow.*

## 📐 I. General UI/UX Component Library (Reusable Components)

These components must be built once and reused across all modules (Dashboard, List Pages, Detail Pages).

### 1. Global Layout & Flow
*   **Architecture:** The application must function as a Single Page Application (SPA).
*   **Primary Navigation:** The Dashboard is the mandatory starting point after successful login.
*   **Standard Components:**
    *   **Card Component:** Reusable container used for the Dashboard overview.
    *   **List Table Component:** Highly reusable table for listing entities, supporting client-side filtering and sorting (pagination handled by API).
    *   **Modal/Dialog Component:** Used for confirmations (e.g., soft-delete) and displaying temporary alerts.

### 2. Form/Detail Page Behavior (`EditableForm`)
A standardized component must govern all screens for adding or editing entries.

| Feature | State | Rule Description | Implementation Note |
| :--- | :--- | :--- | :--- |
| **Initial State** | Read-Only | The page loads displaying data only. | An **'Edit' button** must be present to transition to editing mode, unless the entity is being newly created (no 'Edit' button needed). |
| **Validation** | Validation Phase | Triggered on Save attempt. | Must validate all required fields and adherence to defined format/length rules. |
| **Error Handling** | Error State | Displayed for any failed validation. | 1. Border color of the invalid field must change to `error_border_color`. 2. An explanatory message must appear immediately above/below the field, styled with `error_text_color`. 3. When the user focuses on the field, the error message must clear/hide. 4. The border color remains visually alarming until the next validation pass. |
| **Interactivity** | Controls | The **Save** and **Cancel** buttons must always be visible and enabled, even if form validation errors are present. |
| **Save Button** | Action | Attempts to send data to the backend API. | If validation fails, the API call is blocked, and the local error state must be displayed. |
| **Cancel Button** | Action | Triggered by click. | Must invoke a Confirmation Dialog (see below). If no changes have been made since the page loaded, simply exit the view. |

### 3. Deletion Workflow (Confirmation Dialog)
*   When a user attempts to delete a record (from a list table), a mandatory confirmation modal must appear.
*   The modal must explain the consequence of the action (i.e., "This action will perform a Soft-Delete and retain historical data").

## 🔒 II. Security and Authorization (Permissions Layer)

The entire UI must enforce a strict two-level security model.

1.  **Authentication (Global):** The first view must be the Login Dialog/Page. Successful login must result in the user session token being established.
2.  **Authorization (Data Level):** Every data retrieval or mutation request must pass the current user's permissions check.
    *   **Default Visibility:** By default, the user must only see data they are explicitly permissioned to view.
    *   **Private Data:** Entries marked as private (e.g., `notes` with visibility 'priv') must be filtered out of all standard views, even for Admin users, unless specific elevation is used.
    *   **UI Guarding:** Buttons and controls for accessing restricted features (e.g., "Restore User," "Delete Role") must be conditionally rendered based on the user's roles and assigned permissions (`role_permissions_link`).

## 🌐 III. Specific Page Layouts

### 1. The Dashboard Page (Landing View)
The layout must be a grid of **Cards**. The Dashboard must load asynchronously and display the following sections:

#### A. Mandatory Entity Cards (Drilldown Targets)
These cards provide a summary and link to the entity's dedicated list/overview page.

| Entity | Required Metrics/Summary Data | Actions on Click |
| :--- | :--- | :--- |
| **Companies** | 1. Last added/edited Company (Reference, Name). 2. Count of companies segmented by Industry (`company_tags` group). 3. Count of companies segmented by Size (`company_tags` group). 4. Interaction count per Company Industry. | Takes user to the Company Overview Page. |
| **Contacts** | 1. Last added/edited Contact (Full Name, Role, Company Name if available). 2. Total count of contacts. 3. Count of contacts segmented by Role (`contact_role` group). 4. Interaction count per Contact Role. | Takes user to the Contact List/Overview Page. |
| **Interactions** | 1. Total number of recorded interactions. 2. A small table listing the Top 5 interaction types (by count). | Takes user to the Interaction List/Overview Page. |
| **Engagements** | 1. Total count of engagements. 2. Count of 'Open' vs. 'Closed' engagements. 3. A mini-table listing the Top 3 currently open engagements, including a summary of the last interaction associated with it. | Takes user to the Engagement Overview Page. |
| **Tasks** | **(High Priority)** Must be the top-most, full-width card. Displays the Top 5 upcoming tasks. | Takes user to the Task Detail Page. |

#### B. General System Metrics Cards (Operational Insight)
These cards provide operational data for system health monitoring.
*   Running time (Uptime/Elapsed time).
*   Last API action (Entity Name and ID).
*   Total database size (via API/Backend endpoint).
*   Total API calls made since startup.
*   Total API call failures (network/system errors).

To support this, the backend will need to supply a special REST endpoint to return these values.

### 2. Entity List Page (Overview Page)
*   **Primary Function:** Listing and searching records.
*   **Core Elements:**
    *   Filter panel (Mandatory).
    *   Results table (Paginated).
    *   Action buttons: 'Add New', 'Export Data', 'View Details'.
*   **Functionality:**
    *   **Exporting:** Export selected/all grid rows to CSV.
    *   **Filtering:** Must support filtering on *all* defined searchable columns, including filtering by `is_deleted` (Admin only).
    *   **Bulk Actions:** Implement controls for bulk actions (e.g., bulk deletion, bulk tagging).
    *   **Soft-Delete:** Every row must include a visible 'Delete' option, which triggers the Confirmation Dialog.

### 3. Entity Detail Page (CRUD/Form)
*   **Structure:** Must display read-only data first.
*   **Editing:** The 'Edit' button toggles the page into the editable mode, activating the standardized form validation rules (Section I.2).
*   **Inter-Entity Linking:** Must provide dynamic UI components to handle relationship creation (e.g., a multi-select dropdown to link a note to the current company, utilizing the `company_notes_link` structure).
