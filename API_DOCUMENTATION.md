# CareerPath API Documentation

## Base URL
`{Base_url} = https://careerpath.runasp.net` 

## Table of Contents
- [Authentication Endpoints](#authentication-endpoints)
- [User Profile Endpoints](#user-profile-endpoints)

## Authentication Endpoints

Base path: `/auth`

### Register User
- **Endpoint:** `POST /auth/register`
- **URL:** `{Base_url}/auth/register`
- **Description:** Creates a new user account
- **Authentication:** Not required
- **Request Body:**
```json
{
    "email": "string",     // Valid email address
    "password": "string",  // Minimum 8 characters
    "jobTitle": "string"   // Optional
}
```
- **Success Response (200 OK):**
```json
{
    "message": "User registered successfully"
}
```
- **Error Response (400 Bad Request):**
```json
{
    "error": "string"  // Error message (e.g., "Email already in use")
}
```

### Login
- **Endpoint:** `POST /auth/login`
- **URL:** `{Base_url}/auth/login`
- **Description:** Authenticates a user and returns a JWT token
- **Authentication:** Not required
- **Request Body:**
```json
{
    "email": "string",     // Registered email address
    "password": "string"   // User's password
}
```
- **Success Response (200 OK):**
```json
{
    "token": "string"  // JWT token for authentication
}
```
- **Error Response (400 Bad Request):**
```json
{
    "error": "string"  // Error message (e.g., "Invalid credentials")
}
```

### Forgot Password
- **Endpoint:** `POST /auth/forgot-password`
- **URL:** `{Base_url}/auth/forgot-password`
- **Description:** Initiates the password reset process by sending a reset link to the user's email
- **Authentication:** Not required
- **Request Body:**
```json
{
    "email": "string"  // Registered email address
}
```
- **Success Response (200 OK):**
```json
{
    "message": "If your email exists in our system, you will receive password reset instructions."
}
```
- **Error Response (400 Bad Request):**
```json
{
    "error": "string"  // Error message
}
```

### Reset Password
- **Endpoint:** `POST /auth/reset-password`
- **URL:** `{Base_url}/auth/reset-password`
- **Description:** Resets the user's password using the token received via email
- **Authentication:** Not required
- **Request Body:**
```json
{
    "email": "string",           // User's email address
    "token": "string",          // Reset token received via email
    "newPassword": "string",    // New password (minimum 8 characters)
    "confirmPassword": "string" // Must match newPassword
}
```
- **Success Response (200 OK):**
```json
{
    "message": "Password has been reset successfully. You can now log in with your new password."
}
```
- **Error Response (400 Bad Request):**
```json
{
    "error": "string"  // Error message (e.g., "Invalid token" or "Passwords must match")
}
```

## User Profile Endpoints

Base path: `/api/profiles`

⚠️ **Note:** All profile endpoints require authentication via JWT token in the Authorization header.

### Get All Profiles
- **Endpoint:** `GET /api/profiles`
- **URL:** `{Base_url}/api/profiles`
- **Description:** Retrieves all user profiles
- **Authentication:** Required (JWT token)
- **Success Response (200 OK):**
```json
[
    {
        "id": "string",
        "firstName": "string",
        "lastName": "string",
        "bio": "string",
        "location": "string",
        "avatarUrl": "string",
        "coverUrl": "string",
        "jobTitle": "string",
        "skills": ["string"]
    }
]
```
- **Error Response (401 Unauthorized):**
```json
{
    "error": "Unauthorized access"
}
```

### Get Profile by ID
- **Endpoint:** `GET /api/profiles/{id}`
- **URL:** `{Base_url}/api/profiles/{id}`
- **Description:** Retrieves a specific user profile by ID
- **Authentication:** Required (JWT token)
- **Parameters:** 
  - `id` (path parameter): The ID of the user profile
- **Success Response (200 OK):**
```json
{
    "id": "string",
    "firstName": "string",
    "lastName": "string",
    "bio": "string",
    "location": "string",
    "avatarUrl": "string",
    "coverUrl": "string",
    "jobTitle": "string",
    "skills": ["string"]
}
```
- **Error Responses:**
  - 401 Unauthorized: Not authenticated
  - 404 Not Found: Profile not found
```json
{
    "error": "Profile not found or database error occurred"
}
```

### Get My Profile
- **Endpoint:** `GET /api/profiles/me`
- **URL:** `{Base_url}/api/profiles/me`
- **Description:** Retrieves the profile of the currently authenticated user
- **Authentication:** Required (JWT token)
- **Success Response (200 OK):**
```json
{
    "id": "string",
    "firstName": "string",
    "lastName": "string",
    "bio": "string",
    "location": "string",
    "avatarUrl": "string",
    "coverUrl": "string",
    "jobTitle": "string",
    "skills": ["string"]
}
```
- **Error Responses:**
  - 401 Unauthorized: Not authenticated
  - 404 Not Found: Profile not found

### Create Profile
- **Endpoint:** `POST /api/profiles`
- **URL:** `{Base_url}/api/profiles`
- **Description:** Creates a new profile for the authenticated user
- **Authentication:** Required (JWT token)
- **Request Body:**
```json
{
    "firstName": "string",
    "lastName": "string",
    "bio": "string",
    "location": "string",
    "avatarUrl": "string",
    "coverUrl": "string",
    "jobTitle": "string",
    "skills": ["string"]
}
```
- **Success Response (201 Created):**
```json
{
    "id": "string",
    "firstName": "string",
    "lastName": "string",
    "bio": "string",
    "location": "string",
    "avatarUrl": "string",
    "coverUrl": "string",
    "jobTitle": "string",
    "skills": ["string"]
}
```
- **Error Responses:**
  - 400 Bad Request: Profile already exists
  - 401 Unauthorized: Not authenticated
  - 500 Internal Server Error: Creation failed

### Update Profile
- **Endpoint:** `PUT /api/profiles/{id}`
- **URL:** `{Base_url}/api/profiles/{id}`
- **Description:** Updates an existing user profile
- **Authentication:** Required (JWT token)
- **Parameters:**
  - `id` (path parameter): The ID of the profile to update (must be the authenticated user's profile)
- **Request Body:**
```json
{
    "firstName": "string",
    "lastName": "string",
    "bio": "string",
    "location": "string",
    "avatarUrl": "string",
    "coverUrl": "string",
    "jobTitle": "string",
    "skills": ["string"]
}
```
- **Success Response (200 OK):**
```json
{
    "id": "string",
    "firstName": "string",
    "lastName": "string",
    "bio": "string",
    "location": "string",
    "avatarUrl": "string",
    "coverUrl": "string",
    "jobTitle": "string",
    "skills": ["string"]
}
```
- **Error Responses:**
  - 401 Unauthorized: Not authenticated
  - 403 Forbidden: Attempting to update another user's profile
  - 500 Internal Server Error: Update failed

### Delete Profile
- **Endpoint:** `DELETE /api/profiles/{id}`
- **URL:** `{Base_url}/api/profiles/{id}`
- **Description:** Deletes a user profile
- **Authentication:** Required (JWT token)
- **Parameters:**
  - `id` (path parameter): The ID of the profile to delete (must be the authenticated user's profile)
- **Success Response (204 No Content)**
- **Error Responses:**
  - 401 Unauthorized: Not authenticated
  - 403 Forbidden: Attempting to delete another user's profile
  - 500 Internal Server Error: Deletion failed

## Important Notes

### Authentication
1. All requests to protected endpoints must include the JWT token in the Authorization header:
   ```
   Authorization: Bearer <your_jwt_token>
   ```
2. The JWT token is obtained from the login endpoint and is valid for 1 hour

### General
1. All requests should include the header `Content-Type: application/json`
2. Error responses will always include an `error` field with a descriptive message
3. The profile endpoints enforce ownership - users can only modify their own profiles
4. Server errors (500) include generic messages for security, but are logged on the server for debugging
5. Password requirements:
   - Minimum 8 characters
   - Should contain a mix of letters, numbers, and special characters (recommended)
6. The reset password link sent to email will be valid for 24 hours
7. For security reasons, the forgot password endpoint always returns a success message, even if the email doesn't exist in the system 