# 🌐 GraphQL Operation Catalog (MyCRM)

*This document defines the normative GraphQL endpoint, naming conventions, query patterns, and mutation catalog for MyCRM.*

---

# I. Endpoint

The application exposes a single GraphQL endpoint:

- `/graphql`

---

# II. Naming Conventions

## 1. Types
GraphQL object type names use **singular PascalCase**.

Examples:
- `Company`
- `Contact`
- `Interaction`
- `Engagement`
- `User`
- `AuditLog`

## 2. Collection Queries
Collection query names use **plural camelCase**.

Examples:
- `companies`
- `contacts`
- `interactions`
- `engagements`
- `users`
- `auditLogs`

## 3. Single-Item Queries
Single-item query names use **singular camelCase**.

Examples:
- `company`
- `contact`
- `interaction`
- `engagement`
- `user`
- `auditLog`

## 4. Mutations
Mutations use **verbEntity** naming.

Examples:
- `createCompany`
- `updateCompany`
- `deleteCompany`
- `restoreCompany`

---

# III. Shared GraphQL Conventions

## 1. Scalars
The schema uses custom scalars:
- `Date`
- `Time`
- `DateTime`

## 2. Paged Result Types
Each entity has a paged result type.

Examples:
- `CompanyListResult`
- `ContactListResult`
- `InteractionListResult`

Each list result contains:
- `items`
- `totalCount`
- `offset`
- `limit`

## 3. List Query Arguments
Collection queries use standard arguments:
- `offset`
- `limit`
- `filter`
- `sort`

## 4. Typed Filter and Sort Inputs
Each entity defines its own filter and sort input types.

Examples:
- `CompanyFilterInput`
- `CompanySortInput`
- `ContactFilterInput`
- `ContactSortInput`

## 5. Single-Item Lookup Rule
Where applicable, single-item queries support lookup by:
- `id`
- `ref`

If both `id` and `ref` are supplied in the same request, the API must reject the request as invalid.

## 6. Mutation Result Type
For non-entity-returning actions, the schema uses a shared minimal result type:

```graphql
type MutationResult {
  success: Boolean!
  id: Int
}
```

Used for actions such as:

* delete
* restore
* enable/disable user
* remove permission from role

# IV. Business Entity Queries
## 1. Companies
### Queries
* `companies(offset, limit, filter, sort): CompanyListResult!`
* `company(id: Int, ref: String): Company`
* `deletedCompanies(offset, limit, filter, sort): CompanyListResult!`

### Mutations
* `createCompany(input: CreateCompanyInput!): Company!`
* `updateCompany(id: Int!, input: UpdateCompanyInput!, updatedAt: DateTime!): Company!`
* `deleteCompany(id: Int!): MutationResult!`
* `restoreCompany(id: Int!): MutationResult!`
* `bulkDeleteCompanies(ids: [Int!], filter: CompanyFilterInput): MutationResult!`
* `bulkRestoreCompanies(ids: [Int!], filter: CompanyFilterInput): MutationResult!`

## 2. Contacts
### Queries
* contacts(offset, limit, filter, sort): ContactListResult!
* contact(id: Int, ref: String): Contact
* deletedContacts(offset, limit, filter, sort): ContactListResult!

### Mutations
* createContact(input: CreateContactInput!): Contact!
* updateContact(id: Int!, input: UpdateContactInput!, updatedAt: DateTime!): Contact!
* deleteContact(id: Int!): MutationResult!
* restoreContact(id: Int!): MutationResult!
* bulkDeleteContacts(ids: [Int!], filter: ContactFilterInput): MutationResult!
* bulkRestoreContacts(ids: [Int!], filter: ContactFilterInput): MutationResult!

## 3. Interactions
### Queries
* interactions(offset, limit, filter, sort): InteractionListResult!
* interaction(id: Int, ref: String): Interaction
* deletedInteractions(offset, limit, filter, sort): InteractionListResult!

### Mutations
* createInteraction(input: CreateInteractionInput!): Interaction!
* updateInteraction(id: Int!, input: UpdateInteractionInput!, updatedAt: DateTime!): Interaction!
* deleteInteraction(id: Int!): MutationResult!
* restoreInteraction(id: Int!): MutationResult!
* bulkDeleteInteractions(ids: [Int!], filter: InteractionFilterInput): MutationResult!
* bulkRestoreInteractions(ids: [Int!], filter: InteractionFilterInput): MutationResult!

## 4. Engagements
### Queries
* engagements(offset, limit, filter, sort): EngagementListResult!
* engagement(id: Int, ref: String): Engagement
* deletedEngagements(offset, limit, filter, sort): EngagementListResult!

### Mutations
* createEngagement(input: CreateEngagementInput!): Engagement!
* updateEngagement(id: Int!, input: UpdateEngagementInput!, updatedAt: DateTime!): Engagement!
* deleteEngagement(id: Int!): MutationResult!
* restoreEngagement(id: Int!): MutationResult!
* bulkDeleteEngagements(ids: [Int!], filter: EngagementFilterInput): MutationResult!
* bulkRestoreEngagements(ids: [Int!], filter: EngagementFilterInput): MutationResult!

## 5. Notes
### Queries
* notes(offset, limit, filter, sort): NoteListResult!
* note(id: Int!): Note
* deletedNotes(offset, limit, filter, sort): NoteListResult!

### Mutations
* createNote(input: CreateNoteInput!): Note!
* updateNote(id: Int!, input: UpdateNoteInput!, updatedAt: DateTime!): Note!
* deleteNote(id: Int!): MutationResult!
* restoreNote(id: Int!): MutationResult!
* bulkDeleteNotes(ids: [Int!], filter: NoteFilterInput): MutationResult!
* bulkRestoreNotes(ids: [Int!], filter: NoteFilterInput): MutationResult!

## 6. Documents
### Queries
* documents(offset, limit, filter, sort): DocumentListResult!
* document(id: Int, ref: String): Document
* deletedDocuments(offset, limit, filter, sort): DocumentListResult!

### Mutations
* createDocument(input: CreateDocumentInput!): Document!
* updateDocument(id: Int!, input: UpdateDocumentInput!, updatedAt: DateTime!): Document!
* deleteDocument(id: Int!): MutationResult!
* restoreDocument(id: Int!): MutationResult!
* bulkDeleteDocuments(ids: [Int!], filter: DocumentFilterInput): MutationResult!
* bulkRestoreDocuments(ids: [Int!], filter: DocumentFilterInput): MutationResult!

# V. Interaction Schema Convention
The Interaction type exposes contextual association data directly as GraphQL fields even though those relationships are persisted internally via interactions_link.

## Fields
* contactId: Int
* companyId: Int
* engagementId: Int
* contact: Contact
* company: Company
* engagement: Engagement

This flattening is part of the public GraphQL contract.

# VI. Relationship Traversal
GraphQL should expose related entities as nested object fields where useful.

Examples:
* Company.contacts
* Company.notes
* Company.documents
* Contact.notes
* Interaction.documents
* Engagement.companies
* Engagement.contacts
* Engagement.documents

Authorization and visibility filtering still apply to nested fields.

# VII. Deleted Record Queries
Deleted-record access is exposed through separate queries, not flags.

Examples:
* deletedCompanies(...)
* deletedContacts(...)
* deletedInteractions(...)

These queries use the same:
* filter structure
* sort structure
* pagination structure
as the corresponding non-deleted queries.

# VIII. Audit, Dashboard, and System Queries
## Audit
* auditLogs(offset, limit, filter, sort): AuditLogListResult!
* auditLog(id: Int!): AuditLog

## Dashboard
* dashboard: Dashboard!

## System Metrics
* systemMetrics: SystemMetrics!

All authorization rules apply.

# IX. Authentication and Administration
## 1. Authentication Mutations
GraphQL also exposes authentication operations:

* login(input: LoginInput!): AuthPayload!
* refreshToken(input: RefreshTokenInput!): AuthPayload!
* logout(input: LogoutInput): MutationResult!
* changePassword(input: ChangePasswordInput!): MutationResult!

## 2. User Management
Administrative user-management operations are exposed as mutations:

* createUser(input: CreateUserInput!): User!
* updateUser(id: Int!, input: UpdateUserInput!, updatedAt: DateTime!): User!
* disableUser(id: Int!): MutationResult!
* enableUser(id: Int!): MutationResult!
* resetUserPassword(id: Int!, input: ResetUserPasswordInput!): MutationResult!

## 3. Role/Permission Management
GraphQL exposes role-permission operations:

### Queries
* roles: [Role!]!
* permissions: [Permission!]!

### Mutations
* assignPermissionToRole(roleId: Int!, permissionId: Int!): MutationResult!
* removePermissionFromRole(roleId: Int!, permissionId: Int!): MutationResult!

## 4. Settings
Settings operations are also exposed:

### Queries
* settings: [Setting!]!
* setting(key: String!): Setting

### Mutations
* updateSetting(key: String!, value: String!): Setting!

# X. Export Rule
CSV export remains REST-only and is not exposed via GraphQL.

# XI. Error and Concurrency Rules
## 1. Concurrency
Update mutations require:

* updatedAt: DateTime!

If the provided timestamp does not match the persisted record, the API must return an application error with:

* code = "concurrency_conflict"

## 2. Error Extensions
GraphQL errors must include standardized application metadata in extensions where applicable:

* code
* message
* details
* traceId
