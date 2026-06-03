# 📊 Dashboard Metrics Contract (LocalCRM)

*This document defines the normative semantics for dashboard summary cards and operational metrics in LocalCRM.*

---

# I. General Rules

1. All dashboard metrics must respect authorization, visibility, and soft-delete rules unless a metric is explicitly defined otherwise.
2. Soft-deleted entities must be excluded from standard dashboard metrics.
3. Private note/document visibility rules must also apply to dashboard-derived counts and summaries.
4. Business dashboard metrics and system metrics are distinct concerns.

---

# II. Tasks Card

## 1. Definition of Upcoming Tasks
The “Top 5 upcoming tasks” metric is defined as interactions where:
- `is_task = TRUE`
- `is_deleted = FALSE`
- the interaction is scheduled in the future using `interaction_date` and optional `interaction_time`
- `state` is not in the configured closed/terminal interaction state set

The configured terminal/closed interaction states are read from settings key:
- `interaction_closed_states`

## 2. Ordering Rule
Upcoming tasks are ordered by:
1. earliest `interaction_date`
2. earliest `interaction_time`

If `interaction_time` is null for tasks on the same date:
- tasks with explicit times sort first
- tasks with null time sort after timed tasks on that date
- but before tasks on later dates

## 3. Result Count
Only the top 5 tasks are shown.

---

# III. Companies Card

## 1. Last Created / Last Updated
The Companies card displays both:
- last created company
- last updated company

## 2. Industry Segmentation
Company industry counts are grouped by:
- `companies.branch`

using values from the `company_branch` tag group.

## 3. Size Segmentation
Company size counts are grouped by:
- `companies.size`

using values from the `company_size` tag group.

## 4. Interaction Count per Company Industry
This metric includes:
1. interactions linked directly to companies
2. interactions linked via contacts, where the active company at the interaction date/time can be resolved

The resulting interaction is then grouped by the resolved company’s:
- `branch`

If a company cannot be resolved for a contact-linked interaction at the interaction date/time, that interaction is excluded from this metric.

---

# IV. Contacts Card

## 1. Last Created / Last Updated
The Contacts card displays both:
- last created contact
- last updated contact

## 2. Total Count
The total contact count excludes:
- soft-deleted contacts
- unauthorized contacts

## 3. Contact Role Segmentation
Contact role counts are based on the contact’s **current active role** from `company_contacts_link`.

If a contact has multiple active company relationships with different active roles:
- the contact is counted once in each active role bucket

## 4. Interaction Count per Contact Role
This metric includes interactions linked to contacts and groups them by the contact’s active role at the interaction date/time.

If the role cannot be resolved at the interaction date/time:
- the interaction is grouped into an `Unknown/Unresolved` bucket

---

# V. Interactions Card

## 1. Total Interaction Count
The total interaction count includes:
- non-task interactions only

It excludes:
- interactions where `is_task = TRUE`
- soft-deleted interactions
- unauthorized interactions

## 2. Top Interaction Types
The “Top 5 interaction types” mini-table includes:
- non-task interactions only

It excludes tasks.

---

# VI. Engagements Card

## 1. Total Count
The total engagement count excludes:
- soft-deleted engagements
- unauthorized engagements

## 2. Open vs Closed
Open vs Closed engagement counts are determined using the configured closed-status set from settings key:
- `engagement_closed_statuses`

## 3. Top 3 Currently Open Engagements
The “Top 3 currently open engagements” are ranked by:
- most recent associated interaction

If an open engagement has no associated interaction:
- it is still eligible for display
- it sorts after open engagements that do have associated interactions

## 4. Last Interaction Summary
The engagement card’s last interaction summary includes:
- interaction date/time
- interaction subject
- interaction type

---

# VII. System Metrics

## 1. Uptime
“Running time” / “Uptime” means:
- process uptime since the current application start

## 2. Last API Action
“Last API action” means:
- the last successful write action only

It must include:
- entity name
- entity id
- action type

## 3. Total API Calls Since Startup
This metric counts:
- raw HTTP requests only

It does not separately count nested GraphQL field resolution.

## 4. Total API Call Failures Since Startup
This metric includes:
- 5xx/system failures
- authentication failures
- authorization failures

## 5. Total Database Size
This metric should represent operational storage footprint.

Preferred definition:
- SQLite database file size plus WAL/journal files

Fallback definition if this is not practically feasible:
- logical size estimation from database-level introspection

---

# VIII. Implementation Notes

1. Dashboard cards should be backed by dedicated backend aggregation logic, not assembled entirely client-side.
2. All dashboard aggregations must respect current user permissions and current visibility/elevation state.
3. Settings-backed metric rules must be validated at application startup and defaulted safely when missing.