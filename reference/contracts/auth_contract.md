# 🔑 Authentication and Session Contract (LocalCRM)

*This document defines the normative authentication, JWT, refresh token, and session lifecycle behavior for LocalCRM.*

---

# I. Authentication Model

LocalCRM uses:
- JWT access tokens
- refresh tokens
- server-side refresh token storage and revocation

Authentication is available through:
- REST

---

# II. Token Lifetimes

## 1. Access Token Lifetime
The access token lifetime is configurable through application settings key:

- `token_lifetime`

## 2. Refresh Token Lifetime
The refresh token lifetime is configurable through application settings key:

- `refresh_token_lifetime`

---

# III. JWT Access Token Claims

The JWT access token must include at minimum:

- `sub` — the stable user identifier (`user_id`)
- `name`
- `preferred_username`
- `role`
- `permissions`
- `sid` — session identifier
- `exp`

## Rules
1. `sub` represents `user_id`, not username.
2. Effective permissions are embedded in the token.
3. The token must be validated using issuer and audience constraints.

---

# IV. JWT Validation Settings

JWT validation must enforce:
- issuer
- audience
- signature validation
- expiry validation

Settings keys:
- `jwt_issuer`
- `jwt_audience`

The JWT signing secret/key must **not** be stored in application settings.
It must come from environment/host configuration or equivalent secure secret management.

---

# V. Refresh Token Model

## 1. Rotation
Refresh tokens use **rotation**:
- each successful refresh invalidates the previous refresh token
- each successful refresh issues a new refresh token

## 2. Reuse Detection
If a previously rotated refresh token is used again:
- treat this as token reuse detection
- revoke the entire session/token family

## 3. Concurrent Sessions
Multiple concurrent refresh-token sessions per user are allowed.

## 4. Session Revocation Scope
The system supports:
- logout of the current session
- revoke all sessions for the current user
- revoke all sessions for another user (admin/authorized capability)

It does **not** expose individual session revocation as a normal API feature.

---

# VI. Session Invalidation Rules

Existing sessions must be revoked when any of the following occur:

1. user is disabled (`is_active = FALSE`)
2. user's role or effective permissions change
3. user changes their password
4. administrator resets the user's password
5. token reuse is detected

When a user is disabled:
- new login attempts must fail
- API access must be denied
- existing refresh-token sessions must become invalid

---

# VII. REST Authentication Endpoints

The REST authentication surface includes:

- `POST /api/auth/login`
- `POST /api/auth/refresh`
- `POST /api/auth/logout`
- `POST /api/auth/change-password`
- `GET /api/auth/me`
- `POST /api/auth/revoke-all-sessions`

Administrative user-session operations include:

- `POST /api/users/{id}/reset-password`
- `POST /api/users/{id}/disable`
- `POST /api/users/{id}/enable`
- `POST /api/users/{id}/revoke-sessions`

---

# VIII. GraphQL Authentication Operations

GraphQL exposes authentication/session operations as mutations and queries.

## Queries
- `me`

## Mutations
- `login`
- `refreshToken`
- `logout`
- `changePassword`
- `revokeAllMySessions`
- `revokeUserSessions(id: Int!)`
- `resetUserPassword`
- `disableUser`
- `enableUser`

---

# IX. Response Contracts

## 1. Login Response
A successful login returns:
- access token
- refresh token
- current user payload

## 2. Refresh Response
A successful refresh returns:
- new access token
- new refresh token
- refreshed current user payload

## 3. `me` Response
The authenticated current-user/session response includes at minimum:
- username
- role
- effective permissions

---

# X. Logout and Revoke Behavior

## 1. Logout
Logout invalidates the **current refresh-token session only**.

## 2. Revoke All My Sessions
A user may revoke all their own sessions through:
- REST: `POST /api/auth/revoke-all-sessions`
- GraphQL: `revokeAllMySessions`

## 3. Revoke Another User’s Sessions
An authorized administrator may revoke all sessions for another user through:
- REST: `POST /api/users/{id}/revoke-sessions`
- GraphQL: `revokeUserSessions(id: Int!)`

This capability requires permission:
- `users.revoke_sessions`

---

# XI. Authorization and Audit

The following security-relevant events must be audit logged:
- failed login attempts
- authorization failures
- write operations
- link changes
- API/system errors

Ordinary validation failures are not audit logged by default.

---

# XII. Implementation Notes

1. Access token expiration values must be parsed from `token_lifetime`.
2. Refresh token expiration values must be parsed from `refresh_token_lifetime`.
3. Refresh tokens must be stored server-side in a revocable form.
4. Access tokens may contain permissions for performance, but backend authorization must still verify session validity and user active status.
5. Service-layer logic must enforce forced logout/session invalidation on:
   - role changes
   - permission changes
   - password changes
   - password resets
   - user disable
6. The `me` response should be the canonical way for clients to initialize current-user context after login/refresh.

# XIII. Forced Password Change Login Refusal

If password change is required:
- normal login must be refused
- error code returned:
  - `password_change_required`
- backend may issue a short-lived password-change token for use with:
  - `POST /api/auth/complete-password-change`
  - `completePasswordChange`