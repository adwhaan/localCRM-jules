# 👤 User Management and Password Policy Contract (LocalCRM)

*This document defines the normative user-management, username, password, and account-lockout behavior for LocalCRM.*

---

# I. User Lifecycle Rules

## 1. User Creation
When an administrator creates a new user:
- the account is active immediately by default
- exactly one primary role must be assigned
- the administrator sets the initial password directly

## 2. Username Rules
Usernames are:
- unique case-insensitively
- used case-insensitively for login
- immutable after creation

Because audit references use `username` text values, username mutation is not allowed.

---

# II. Password Change Requirements

## 1. First-Login Password Change
A user created by an administrator must change password before normal login is allowed.

## 2. Post-Reset Password Change
If an administrator resets a user's password, that user must change password before normal login is allowed again.

## 3. Login Behavior When Password Change Is Required
If a login attempt is made while password change is required:
- normal login is refused
- the API returns error code:
  - `password_change_required`

The API does not expose a normal successful login payload with a `mustChangePassword` flag for this flow.

---

# III. Forced Password Change Flow

## 1. Dedicated Operations
A dedicated forced-password-change operation is required.

### REST
- `POST /api/auth/complete-password-change`

### GraphQL
- `completePasswordChange`

## 2. Authentication for Forced Change
Forced password change uses a **short-lived password-change token** returned by the refused login flow.

## 3. Token Lifetime
The password-change token lifetime is configurable through settings key:
- `password_change_token_lifetime`

---

# IV. User Visibility and Access

## 1. Disabled Users
Users are not soft-deletable. Disabled users remain visible in normal administrative user lists.

## 2. User List Filtering
Administrative user list APIs must support filtering by:
- `is_active`
- `role_id`
- `username`

## 3. Self Access Rule
Non-admin users do not access their own user record through generic user endpoints.

They access their identity only through:
- REST: `GET /api/auth/me`
- GraphQL: `me`

---

# V. Role Changes

## 1. Role Assignment Model
Each user has exactly one primary role.

## 2. Role Update
Administrators may change a user's role through normal user update operations.

## 3. Session Invalidation
Changing a user's role must invalidate the user's existing sessions.

---

# VI. Password Policy

## 1. Minimum Length
Minimum password length is configurable through settings key:
- `min_password_length`

## 2. Composition Policy
Password policy is length-based only.

The system does **not** require:
- uppercase
- lowercase
- digits
- special characters

unless such rules are added in a future revision.

## 3. Password Reuse
Users may not reuse their current password.

The system does not maintain broader password history in the current specification.

---

# VII. Failed Login Lockout

## 1. Lockout Requirement
Repeated failed login attempts trigger a temporary account lockout.

## 2. Configurable Settings
Lockout policy is configurable through:

- `max_failed_login_attempts`
- `login_lockout_duration`

## 3. Tracking Scope
Lockout is tracked separately by:
- username
- source IP address

The backend may use both signals together for enforcement and security logging.

---

# VIII. Related Authentication and Session Rules

The following rules apply in conjunction with this contract:

1. password change revokes all existing sessions
2. admin password reset revokes all existing sessions for the target user
3. failed login attempts are audit logged
4. authorization failures are audit logged
5. disabled users lose API access immediately

---

# IX. Implementation Notes

1. Username uniqueness should be enforced using normalized/case-insensitive comparison.
2. Since usernames are immutable, user-edit APIs must reject username changes.
3. The forced-password-change flow should be implemented separately from ordinary authenticated password change.
4. The refused-login response for `password_change_required` should provide only the information required to proceed securely with forced password change.
5. Lockout counters and timers should be implemented in a way that survives normal concurrent access safely.