# CareerPath API Documentation

## Base URL
```
https://careerpath-production.up.railway.app
```

## Authentication
This API uses JWT Bearer token authentication. Include the token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

## Table of Contents
- [Authentication Endpoints](#authentication-endpoints)
- [AI Endpoints](#ai-endpoints)
- [User Profile Endpoints](#user-profile-endpoints)
- [Company Endpoints](#company-endpoints)
- [Job Application Endpoints](#job-application-endpoints)
- [Data Models](#data-models)
- [Error Handling](#error-handling)

---

## Authentication Endpoints

### Register User
**POST** `/api/auth/register`

Register a new user account.

**Request Body:**
```json
{
  "email": "user@example.com",
  "username": "johndoe",
  "password": "securepassword123"
}
```

**Response:** `200 OK`

---

### Login
**POST** `/api/auth/login`

Authenticate user and receive JWT token.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "securepassword123"
}
```

**Response:** `200 OK`

---

### Forgot Password
**POST** `/api/auth/forgot-password`

Request password reset email.

**Request Body:**
```json
{
  "email": "user@example.com"
}
```

**Response:** `200 OK`

---

### Reset Password
**POST** `/api/auth/reset-password`

Reset password using token from email.

**Request Body:**
```json
{
  "email": "user@example.com",
  "token": "reset-token-from-email",
  "newPassword": "newpassword123",
  "confirmPassword": "newpassword123"
}
```

**Response:** `200 OK`

---

### Health Check
**GET** `/api/auth/hello`

Simple health check endpoint.

**Response:** `200 OK`

---

## AI Endpoints

### Extract CV Data
**POST** `/api/ai/extract`

Extract structured data from CV analysis.

**Request Body:**
```json
{
  "personalInformation": {
    "name": "John Doe",
    "email": "john@example.com",
    "phone": "+1234567890",
    "address": "123 Main St, City, Country"
  },
  "skills": [
    {
      "skillName": "JavaScript",
      "proficiencyLevel": "Advanced"
    }
  ],
  "workExperiences": [
    {
      "jobTitle": "Software Developer",
      "jobLevel": "Senior",
      "company": "Tech Corp",
      "startYear": 2020,
      "startMonth": 1,
      "endYear": 2023,
      "endMonth": 12,
      "jobDescription": "Developed web applications..."
    }
  ],
  "educations": [
    {
      "institution": "University of Technology",
      "degree": "Bachelor of Science",
      "fieldOfStudy": "Computer Science",
      "startYear": 2016,
      "startMonth": 9,
      "endYear": 2020,
      "endMonth": 6,
      "educationLevel": "Bachelor"
    }
  ],
  "projects": [
    {
      "projectName": "E-commerce Platform",
      "startDate": "2022-01-01T00:00:00Z",
      "endDate": "2022-06-01T00:00:00Z",
      "url": "https://github.com/user/project",
      "description": "Built a full-stack e-commerce platform..."
    }
  ]
}
```

**Response:** `200 OK`

---

### Get Analysis
**GET** `/api/ai/analysis`

Retrieve AI analysis results.

**Response:** `200 OK`

---

### Upload CV
**POST** `/api/ai/upload`

Upload CV file for processing.

**Request Body:** `multipart/form-data`
- `cv`: File (binary)

**Response:** `200 OK`

---

### Download CV
**GET** `/api/ai/download-cv`

Download processed CV file.

**Response:** `200 OK`

---

### Get Recommendations
**POST** `/api/ai/recommend/{userId}`

Get job recommendations for a specific user.

**Parameters:**
- `userId` (path): User ID

**Response:** `200 OK`

---

### Recommender System
**POST** `/api/ai/recommenderSystem/{userId}`

Advanced recommender system for personalized suggestions.

**Parameters:**
- `userId` (path): User ID

**Response:** `200 OK`

---

## User Profile Endpoints

### Get All Profiles
**GET** `/api/profiles`

Retrieve all user profiles.

**Response:** `200 OK`

---

### Get Profile by ID
**GET** `/api/profiles/{id}`

Retrieve specific user profile.

**Parameters:**
- `id` (path): User ID

**Response:** `200 OK`

---

### Update Profile
**PUT** `/api/profiles/{id}`

Update user profile information.

**Parameters:**
- `id` (path): User ID

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "bio": "Experienced software developer...",
  "location": "New York, NY",
  "avatarUrl": "https://example.com/avatar.jpg",
  "coverUrl": "https://example.com/cover.jpg",
  "jobTitle": "Senior Software Developer",
  "skills": ["JavaScript", "React", "Node.js"]
}
```

**Response:** `200 OK`

---

### Delete Profile
**DELETE** `/api/profiles/{id}`

Delete user profile.

**Parameters:**
- `id` (path): User ID

**Response:** `200 OK`

---

## Company Endpoints

### Get All Companies
**GET** `/api/companies`

Retrieve all companies.

**Response:** `200 OK`
```json
[
  {
    "id": "company-uuid",
    "name": "Tech Corp",
    "companyProfile": "Leading technology company...",
    "location": "San Francisco, CA",
    "website": "https://techcorp.com",
    "foundedDate": "2010-01-01T00:00:00Z",
    "employeeCount": 500,
    "industry": "Technology",
    "logoUrl": "https://techcorp.com/logo.png",
    "contacts": "hr@techcorp.com",
    "officeLocation": "123 Tech Street, SF, CA",
    "jobs": []
  }
]
```

---

### Get Company by ID
**GET** `/api/companies/{id}`

Retrieve specific company details.

**Parameters:**
- `id` (path): Company ID

**Response:** `200 OK` or `404 Not Found`

---

### Create Company
**POST** `/api/companies`

Create a new company.

**Request Body:**
```json
{
  "name": "New Tech Company",
  "companyProfile": "Innovative startup...",
  "location": "Austin, TX",
  "website": "https://newtech.com",
  "foundedDate": "2023-01-01T00:00:00Z",
  "employeeCount": 50,
  "industry": "Software",
  "logoUrl": "https://newtech.com/logo.png",
  "contacts": "contact@newtech.com",
  "officeLocation": "456 Innovation Ave, Austin, TX"
}
```

**Response:** `201 Created` or `400 Bad Request`

---

### Update Company
**PUT** `/api/companies/{id}`

Update company information.

**Parameters:**
- `id` (path): Company ID

**Request Body:** Same as create company (all fields optional)

**Response:** `200 OK`, `400 Bad Request`, or `404 Not Found`

---

### Delete Company
**DELETE** `/api/companies/{id}`

Delete a company.

**Parameters:**
- `id` (path): Company ID

**Response:** `204 No Content` or `404 Not Found`

---

### Get Company Jobs
**GET** `/api/companies/{id}/jobs`

Retrieve all jobs for a specific company.

**Parameters:**
- `id` (path): Company ID

**Response:** `200 OK` or `404 Not Found`
```json
[
  {
    "jobId": "job-uuid",
    "title": "Senior Developer",
    "jobIndustry": "Technology",
    "companyName": "Tech Corp",
    "description": "We are looking for...",
    "requiredSkills": "JavaScript, React, Node.js",
    "experienceLevel": "Senior",
    "educationLevel": "Bachelor",
    "certificationsRequired": "None",
    "requiredLanguage": "English",
    "location": "Remote",
    "salaryRange": "$80,000 - $120,000",
    "employmentType": "Full-time",
    "postingDate": "2023-01-01T00:00:00Z",
    "applicationDeadline": "2023-02-01T00:00:00Z",
    "age": null,
    "gender": null,
    "nationality": null
  }
]
```

---

## Job Application Endpoints

### Get Application by ID
**GET** `/api/JobApplication/{id}`

Retrieve specific job application.

**Parameters:**
- `id` (path): Application ID (integer)

**Response:** `200 OK`
```json
{
  "id": 1,
  "jobId": "job-uuid",
  "userId": "user-uuid",
  "applicationStatus": "Pending",
  "applicationDate": "2023-01-01T00:00:00Z",
  "resumeUrl": "https://example.com/resume.pdf",
  "coverLetterUrl": "https://example.com/cover.pdf"
}
```

---

### Get User Applications
**GET** `/api/JobApplication/user?id={userId}`

Retrieve applications for a specific user.

**Query Parameters:**
- `id`: User ID

**Response:** `200 OK`

---

### Get All Job Applications
**GET** `/api/JobApplication/jobs`

Retrieve all job applications.

**Response:** `200 OK`

---

## Data Models

### Personal Information
```json
{
  "name": "string",
  "email": "string",
  "phone": "string",
  "address": "string"
}
```

### Skill
```json
{
  "skillName": "string",
  "proficiencyLevel": "string"
}
```

### Work Experience
```json
{
  "jobTitle": "string",
  "jobLevel": "string",
  "company": "string",
  "startYear": "integer",
  "startMonth": "integer",
  "endYear": "integer",
  "endMonth": "integer",
  "jobDescription": "string"
}
```

### Education
```json
{
  "institution": "string",
  "degree": "string",
  "fieldOfStudy": "string",
  "startYear": "integer",
  "startMonth": "integer",
  "endYear": "integer",
  "endMonth": "integer",
  "educationLevel": "string"
}
```

### Project
```json
{
  "projectName": "string",
  "startDate": "string (ISO 8601)",
  "endDate": "string (ISO 8601)",
  "url": "string",
  "description": "string"
}
```

---

## Error Handling

### Standard Error Response
```json
{
  "type": "string",
  "title": "string",
  "status": "integer",
  "detail": "string",
  "instance": "string"
}
```

### Common Status Codes
- `200 OK`: Request successful
- `201 Created`: Resource created successfully
- `204 No Content`: Request successful, no content to return
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Missing or invalid authentication
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

---

## Usage Examples

### JavaScript/Fetch Example
```javascript
// Login
const loginResponse = await fetch('https://careerpath-production.up.railway.app/api/auth/login', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    email: 'user@example.com',
    password: 'password123'
  })
});

// Get companies with authentication
const companiesResponse = await fetch('https://careerpath-production.up.railway.app/api/companies', {
  headers: {
    'Authorization': 'Bearer your-jwt-token-here'
  }
});
const companies = await companiesResponse.json();
```

### cURL Example
```bash
# Login
curl -X POST https://careerpath-production.up.railway.app/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "user@example.com", "password": "password123"}'

# Get companies
curl -X GET https://careerpath-production.up.railway.app/api/companies \
  -H "Authorization: Bearer your-jwt-token-here"
```

---

## Notes for Frontend Team

1. **Authentication**: Always include the JWT token in the Authorization header for protected endpoints
2. **Error Handling**: Implement proper error handling for all HTTP status codes
3. **File Uploads**: Use FormData for file uploads (CV upload endpoint)
4. **Date Formats**: All dates are in ISO 8601 format
5. **Content-Type**: Use `application/json` for all JSON requests
6. **Base URL**: Always use the full base URL for all API calls

For any questions or issues, please contact the backend team.