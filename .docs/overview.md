# 🎯 MyCRM Application Development Specification (High-Level Overview)

## 📝 Metadata and Scope
---
*   **System Name:** MyCRM
*   **Primary Goal:** A multi-user, concurrent Customer Relationship Management (CRM) system.
*   **Data Persistence:** Local data store.
*   **Scope Level:** High-level functional specification. Detailed implementations must reference associated files.
*   **Target Platform Models:**
    *   **Web:** Single Page Application (SPA).
    *   **Desktop/Windows:** Standalone application with modular pages/dialogs.
---

## 📚 I. Core System Architecture Summary

The application structure is modular, consisting of a centralized Dashboard and dedicated modules for each primary entity (e.g., Contact Management, Company Overview, Interaction Logging).

### 💡 Functional Breakdown
*   **Main Entry Point:** Dashboard page.
*   **User Flow:** Login $\rightarrow$ Dashboard $\rightarrow$ Entity Detail View (List/Edit) $\rightarrow$ Action (e.g., Add Interaction).
*   **Concurrency:** Must support multiple simultaneous user sessions and write operations.

## 🔗 II. Dependencies & Specifications Reference Index

*This section serves as the single source of truth for where specific technical details reside. Developers must read the referenced files before coding.*

| Component | Purpose | Reference File | Type | Required Actions |
| :--- | :--- | :--- | :--- | :--- |
| **Requirements** | Detailed feature constraints and non-functional requirements. | `./requirements/requirement_details.md` | Specification | Review all constraints (e.g., performance, scalability). |
| **Architecture** | System diagram, component interaction, data flow paths. | `./architecture.md` | Architecture Diagram | Implement component separation and service boundaries. |
| **Tech Stack** | Technology choices (Languages, Frameworks, Database). | `./technology_stack.md` | Configuration | Adhere to specified tech versions and patterns. |
| **Data Model** | Defines all entities, fields, types, and relationships. | `./data_modeling/data_dictionary.md` | Data Schema | Define all database schemas and ORM models. |
| **UI/UX** | Visual design details, component layout, and interaction flow. | `./ui_ux_design.md` | Design Pattern | Follow visual specifications and component behaviors. |

## ⚙️ III. Structured Use Cases (Feature Implementation)

*Each use case is structured with explicit **Preconditions**, **Steps**, **Inputs**, and **Outputs/Error Handling** to guide development.*

### ➡️ USE CASE ID: UC01 - Authentication / Startup
**TITLE:** User Login Process
**TRIGGER:** Application startup.
**SCOPE:** Authentication Module (Backend/Frontend).

| Field | Specification | Detail |
| :--- | :--- | :--- |
| **Preconditions** | User is on the main startup screen. System is initialized. | Credentials must be read from a defined data store. |
| **Process Steps** | 1. Present Login Dialog. 2. User enters Username and Password. 3. System attempts validation against stored hashes. |
| **Input Data** | `username` (String), `password` (String) |
| **Success Criteria** | Credentials match. Application navigates to the Dashboard (UC02). |
| **Error Handling** | Credentials mismatch: Display non-specific error dialog ("Invalid credentials"). User remains on Login Dialog. |

### ➡️ USE CASE ID: UC02 - Dashboard View
**TITLE:** Main Dashboard Overview
**TRIGGER:** Successful login (following UC01).
**SCOPE:** Primary Dashboard Component (UI/UX).

| Field | Specification | Detail |
| :--- | :--- | :--- |
| **Preconditions** | User is logged in and redirected to the main Dashboard view. | Dashboard must load operational metrics first. |
| **Process Steps** | 1. Fetch key summary metrics (e.g., active companies, recent contacts). 2. Display metrics in a "flashcard-like" view. 3. Display a list of recent activity (new interactions, etc.). |
| **Interaction Detail** | Clicking a flashcard or list item must trigger navigation to the respective entity's overview page. |
| **Output** | Fully populated Dashboard view. |

### ➡️ USE CASE ID: UC03 - Create Interaction Record
**TITLE:** Add New Interaction Record
**TRIGGER:** Navigation to an Interaction Overview page.
**SCOPE:** Form Management / Data Linking Module.

| Field | Specification | Detail |
| :--- | :--- | :--- |
| **Preconditions** | User is viewing an existing entity and initiates the "Add Interaction" action. | A dedicated input dialog must be presented. |
| **Required Fields** | Interaction Details (Date, Notes, etc.), `company_id` (Reference), `contact_id` (Reference). The latter 2 fields will be created in the `interactions_link` table. |
| **Process Steps** | 1. Display the interaction form/dialog. 2. **Handle Dependencies:** User selects a Contact or Company. 3. **Core Logic:** If `contact_id` is selected: The system *must* automatically populate `company_id` based on the contact's active company at the specified date/time. 4. If the Contact has no active company (or the date is outside the active window), the user must be allowed to manually select/enter a `company_id`. 5. Save the record. |
| **Input Constraints** | `company_id` and `contact_id` must be linked via combobox/selection mechanism. |
| **Success Output** | A new Interaction record is created and linked to the specified Company and Contact. |

## 🧩 IV. Development Checklist (To-Do for Agent)

*Use this checklist to ensure all required components are addressed in the code implementation.*

*   [ ] Implement the full data schema based on `./data_modeling/data_dictionary.md`.
*   [ ] Develop the core service layer for CRUD operations, following the architecture defined in `./architecture.md`.
*   [ ] Build the Login Module adhering strictly to UC01 requirements (Error Handling, Credentials Check).
*   [ ] Build the Dashboard UI component, ensuring asynchronous data loading for metrics (UC02).
*   [ ] Implement CRUD interfaces for all primary entities (Companies, Contacts, Interactions, Engagements).
*   [ ] Implement the complex data logic for UC03 (Company/Contact auto-linking logic).
*   [ ] Ensure adherence to the specified front-end framework (defined in `./technology_stack.md`).
