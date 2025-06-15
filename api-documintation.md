# Career Path API Documentation

## Overview
This document provides comprehensive information about the Career Path API endpoints, their request/response formats, and usage examples.

## Authentication
Most API endpoints require authentication. Include a valid JWT token in the Authorization header:

```
Authorization: Bearer {your_jwt_token}
```

## API Endpoints

### Auth

- **POST** `/api/auth/register` — Register a new user
- **POST** `/api/auth/login` — Login and receive a JWT token
- **POST** `/api/auth/forgot-password` — Request a password reset email
- **POST** `/api/auth/reset-password` — Reset password using token
- **GET** `/api/auth/hello` — Health check (no auth required)

### User Profiles

- **GET** `/api/profiles` — Get all user profiles (auth required)
- **GET** `/api/profiles/{id}` — Get a user profile by ID (auth required)
- **PUT** `/api/profiles/{id}` — Update a user profile (auth required, only self)
- **DELETE** `/api/profiles/{id}` — Delete a user profile (auth required, only self)

### Companies

- **GET** `/api/companies` — Get all companies
- **GET** `/api/companies/{id}` — Get a company by ID
- **POST** `/api/companies` — Create a new company (not yet implemented)
- **PUT** `/api/companies/{id}` — Update a company (not yet implemented)
- **DELETE** `/api/companies/{id}` — Delete a company (not yet implemented)
- **GET** `/api/companies/{id}/jobs` — Get jobs for a specific company

### Job Applications

- **GET** `/api/jobapplication/{id}` — Get a job application by ID
- **GET** `/api/jobapplication/user?id={userId}` — Get job applications for a user
- **GET** `/api/jobapplication/jobs` — Get all job applications

### AI & CV Analysis

- **POST** `/api/ai/extract` — Save extracted CV analysis data
  - Request body: `CVAnalysisDto`
- **GET** `/api/ai/analysis/{email}` — Get CV analysis by user email
- **POST** `/api/ai/recommend/{userId}` — Recommend jobs for a user
- **POST** `/api/ai/recommenderSystem/{userId}` — Get job recommendations from recommender system

## Example Request/Response Formats

### Auth
#### Register
- **POST** `/api/auth/register`
- Request body:
```json
{
  "email": "user@example.com",
  "password": "string",
  "username": "string"
}
```
- Success: `200 OK` `{ "Message": "User registered successfully" }`
- Error: `400 Bad Request` `{ "Error": "..." }`

#### Login
- **POST** `/api/auth/login`
- Request body:
```json
{
  "email": "user@example.com",
  "password": "string"
}
```
- Success: `200 OK` `{ "Token": "..." }`
- Error: `400 Bad Request` `{ "Error": "..." }`

### User Profiles
#### Get Profile
- **GET** `/api/profiles/{id}`
- Success: `200 OK` — User profile object
- Error: `404 Not Found` `{ "Error": "Profile not found or database error occurred" }`

#### Update Profile
- **PUT** `/api/profiles/{id}`
- Request body: `UpdateUserProfileDto`
- Success: `200 OK` — Updated profile object
- Error: `500 Internal Server Error` `{ "Error": "..." }`

#### Delete Profile
- **DELETE** `/api/profiles/{id}`
- Success: `204 No Content`
- Error: `500 Internal Server Error` `{ "Error": "..." }`

### Companies
#### Get All Companies
- **GET** `/api/companies`
- Success: `200 OK` — Array of companies

#### Get Company by ID
- **GET** `/api/companies/{id}`
- Success: `200 OK` — Company object
- Error: `404 Not Found` `{ "Error": "Company with ID {id} not found" }`

#### Create/Update/Delete Company
- **POST/PUT/DELETE** — Not yet implemented, returns `501 Not Implemented`

#### Get Company Jobs
- **GET** `/api/companies/{id}/jobs`
- Success: `200 OK` — Array of jobs for the company

### Job Applications
#### Get by ID
- **GET** `/api/jobapplication/{id}`
- Success: `200 OK` — Job application object
- Error: `404 Not Found`

#### Get by User
- **GET** `/api/jobapplication/user?id={userId}`
- Success: `200 OK` — Array of job applications

#### Get All
- **GET** `/api/jobapplication/jobs`
- Success: `200 OK` — Array of job applications

### AI & CV Analysis
#### Save Extracted Data
- **POST** `/api/ai/extract`
- Request body: `CVAnalysisDto`
- Success: `200 OK` `{ "message": "CV analysis data saved successfully" }`
- Error: `400 Bad Request` or `500 Internal Server Error`

#### Get CV Analysis by Email
- **GET** `/api/ai/analysis/{email}`
- Success: `200 OK` — CV analysis object
- Error: `404 Not Found` or `500 Internal Server Error`

#### Recommend Jobs
- **POST** `/api/ai/recommend/{userId}`
- Success: `200 OK` — Recommendation response
- Error: `500 Internal Server Error`

#### Recommender System
- **POST** `/api/ai/recommenderSystem/{userId}`
- Success: `200 OK` — Recommendation response
- Error: `500 Internal Server Error`

## Error Handling
All endpoints return appropriate HTTP status codes:
- `200 OK`: The request was successful
- `201 Created`: Resource created
- `204 No Content`: Resource deleted
- `400 Bad Request`: The request was invalid
- `401 Unauthorized`: Authentication failure
- `403 Forbidden`: Not allowed
- `404 Not Found`: The requested resource was not found
- `500 Internal Server Error`: An unexpected server error occurred
- `501 Not Implemented`: Endpoint not implemented

Error responses include a message explaining the issue.

## Notes
- Some endpoints (company create/update/delete) are not yet implemented and will return a 501 status.
- All endpoints requiring authentication expect a JWT token in the `Authorization` header. 