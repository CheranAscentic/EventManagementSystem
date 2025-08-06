# JWT Debug Endpoints

This document describes the debug endpoints created for JWT token inspection and troubleshooting.

## Overview

The debug endpoints provide comprehensive information about JWT tokens, user authentication status, and authorization claims. These are primarily intended for development and debugging purposes.

## Available Endpoints

### 1. `/api/debug/token` 
**Policy:** `DebugToken` (Custom policy with detailed logging)
**Method:** GET
**Authorization:** Requires authenticated user

This endpoint uses the custom `DebugToken` authorization policy that logs comprehensive JWT token information to the console/logs and returns detailed token analysis.

**Features:**
- Logs all JWT claims to the application logs
- Shows token expiration information
- Displays all user roles and identity information
- Performs role verification tests
- Returns complete token breakdown

**Response Example:**
```json
{
  "success": true,
  "message": "JWT Token Debug Information",
  "data": {
    "authentication": {
      "isAuthenticated": true,
      "authenticationType": "Bearer",
      "name": "ADMIN"
    },
    "tokenHeader": {
      "hasAuthorizationHeader": true,
      "authorizationScheme": "Bearer",
      "tokenLength": 234,
      "tokenPrefix": "eyJhbGciOi"
    },
    "keyClaims": {
      "nameIdentifier": "32c170ce-57f7-4e52-1705-08ddd411614f",
      "email": "admin@mail.com",
      "name": "ADMIN",
      "sub": "32c170ce-57f7-4e52-1705-08ddd411614f",
      "exp": "1754398628",
      "iss": "https://localhost:7049",
      "aud": "https://localhost:7049"
    },
    "roles": ["Admin"],
    "roleChecks": {
      "isAdmin": true,
      "isUser": false,
      "hasAnyRole": true
    },
    "allClaims": [...],
    "identities": [...],
    "summary": {
      "totalClaims": 7,
      "totalRoles": 1,
      "totalIdentities": 1,
      "userId": "32c170ce-57f7-4e52-1705-08ddd411614f"
    }
  },
  "timestamp": "2025-01-06T09:30:00.000Z"
}
```

### 2. `/api/debug/token/raw`
**Policy:** Default authentication (any authenticated user)
**Method:** GET
**Authorization:** Requires authenticated user

Returns basic token information without the custom policy logging.

**Response Example:**
```json
{
  "success": true,
  "message": "Raw JWT Token Information",
  "data": {
    "isAuthenticated": true,
    "authenticationType": "Bearer",
    "name": "ADMIN",
    "claimsCount": 7,
    "identitiesCount": 1
  },
  "timestamp": "2025-01-06T09:30:00.000Z"
}
```

### 3. `/api/debug/user`
**Policy:** Default authentication (any authenticated user)
**Method:** GET
**Authorization:** Requires authenticated user

Shows information from the `ICurrentUserService` about the current user.

**Response Example:**
```json
{
  "success": true,
  "message": "Current User Service Information",
  "data": {
    "userId": "32c170ce-57f7-4e52-1705-08ddd411614f",
    "email": "admin@mail.com",
    "isAdmin": true,
    "isAuthenticated": true
  },
  "timestamp": "2025-01-06T09:30:00.000Z"
}
```

## Usage Examples

### Using cURL

```bash
# Get comprehensive token debug info (with detailed logging)
curl -X GET "https://localhost:7049/api/debug/token" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"

# Get basic token info
curl -X GET "https://localhost:7049/api/debug/token/raw" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"

# Get current user service info
curl -X GET "https://localhost:7049/api/debug/user" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

### Using JavaScript/Fetch

```javascript
const token = "YOUR_JWT_TOKEN_HERE";

// Debug token with detailed logging
const debugResponse = await fetch('/api/debug/token', {
  headers: {
    'Authorization': `Bearer ${token}`
  }
});
const debugData = await debugResponse.json();
console.log('Debug Info:', debugData);
```

## DebugToken Authorization Policy

The `DebugToken` policy includes a custom authorization handler (`DebugTokenHandler`) that:

1. **Logs Authentication Status**: Whether user is authenticated
2. **Logs All Claims**: Every claim in the JWT token with full details
3. **Logs Key Claims**: Important claims like NameIdentifier, Email, etc.
4. **Logs Roles**: All assigned roles and role verification tests
5. **Logs Identities**: All identity contexts
6. **Logs Token Expiration**: When token expires and time remaining
7. **Grants Access**: Allows any authenticated user (for debugging purposes)

## Log Output Example

When calling `/api/debug/token`, you'll see detailed logs like:

```
[INFO] === DebugTokenHandler: JWT Token Analysis ===
[INFO] User Authentication Status: True
[INFO] Authentication Type: Bearer
[INFO] User Name: ADMIN
[INFO] === JWT Claims Analysis ===
[INFO] Total Claims Count: 7
[INFO] Claim: Type='sub', Value='32c170ce-57f7-4e52...', ValueType='http://www.w3.org/2001/XMLSchema#string', Issuer='https://localhost:7049'
[INFO] Claim: Type='http://schemas.microsoft.com/ws/2008/06/identity/claims/role', Value='Admin', ValueType='http://www.w3.org/2001/XMLSchema#string', Issuer='https://localhost:7049'
[INFO] === Key Claims Summary ===
[INFO]   NameIdentifier: 32c170ce-57f7-4e52-1705-08ddd411614f
[INFO]   Email: admin@mail.com
[INFO]   Name: ADMIN
[INFO] === Roles Analysis ===
[INFO] Total Roles Count: 1
[INFO] Role: Admin
[INFO] === Role Verification Tests ===
[INFO] IsInRole('Admin'): True
[INFO] IsInRole('User'): False
[INFO] === Token Expiration Info ===
[INFO] Token Expires At: 2025-01-06 12:30:28 UTC
[INFO] Time Until Expiry: 02:59:45
[INFO] Is Token Expired: False
[INFO] === End JWT Token Analysis ===
```

## Security Note

These debug endpoints should only be enabled in development environments. In production, consider removing or securing these endpoints appropriately.

## Troubleshooting Common Issues

1. **401 Unauthorized**: Ensure you're including a valid JWT token in the Authorization header
2. **No logs appearing**: Check your logging level is set to Information or Debug
3. **Token validation fails**: Use these endpoints to verify token claims match your configuration
4. **Role issues**: Check the roles in the token output to verify they're correctly assigned