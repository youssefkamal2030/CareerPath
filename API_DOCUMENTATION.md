# CareerPath API – Frontend Guide

**Base URL:**  
https://careerpath-production.up.railway.app

**Authentication:**  
Most endpoints require a JWT token.  
Add this header to your requests:
```
Authorization: Bearer {your_jwt_token}
```

---

## Auth Endpoints

| Action                | Method | URL                                 | Body/Params                | Response (200)         |
|-----------------------|--------|-------------------------------------|----------------------------|------------------------|
| Register              | POST   | `/api/auth/register`                | `{ email, password, username }` | `{ Message }`          |
| Login                 | POST   | `/api/auth/login`                   | `{ email, password }`      | `{ Token }`            |
| Forgot Password       | POST   | `/api/auth/forgot-password`         | `{ email }`                | `{ ... }`              |
| Reset Password        | POST   | `/api/auth/reset-password`          | `{ email, token, newPassword, confirmPassword }` | `{ ... }` |
| Hello (no auth)       | GET    | `/api/auth/hello`                   | –                          | `{ ... }`              |

---

## User Profiles

| Action                | Method | URL                                 | Body/Params                | Response (200)         |
|-----------------------|--------|-------------------------------------|----------------------------|------------------------|
| Get all profiles      | GET    | `/api/profiles`                     | –                          | `[ ...profiles ]`      |
| Get profile by ID     | GET    | `/api/profiles/{id}`                | –                          | `{ ...profile }`       |
| Update profile        | PUT    | `/api/profiles/{id}`                | `UpdateUserProfileDto`     | `{ ...updatedProfile }`|
| Delete profile        | DELETE | `/api/profiles/{id}`                | –                          | `204 No Content`       |

**UpdateUserProfileDto Example:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "bio": "Software developer",
  "location": "NYC",
  "avatarUrl": "https://...",
  "coverUrl": "https://...",
  "jobTitle": "Engineer",
  "skills": ["C#", "React"]
}
```

---

## Companies

| Action                | Method | URL                                 | Body/Params                | Response (200)         |
|-----------------------|--------|-------------------------------------|----------------------------|------------------------|
| Get all companies     | GET    | `/api/companies`                    | –                          | `[ ...companies ]`     |
| Get company by ID     | GET    | `/api/companies/{id}`               | –                          | `{ ...company }`       |
| Get jobs for company  | GET    | `/api/companies/{id}/jobs`          | –                          | `[ ...jobs ]`          |

---

## Job Applications

| Action                | Method | URL                                 | Body/Params                | Response (200)         |
|-----------------------|--------|-------------------------------------|----------------------------|------------------------|
| Get by ID             | GET    | `/api/JobApplication/{id}`          | –                          | `{ ...application }`   |
| Get by user           | GET    | `/api/JobApplication/user?id={userId}` | –                      | `[ ...applications ]`  |
| Get all               | GET    | `/api/JobApplication/jobs`          | –                          | `[ ...applications ]`  |

---

## AI & CV Analysis

### CV Analysis Data
| Action                | Method | URL                                 | Body/Params                | Response (200)         |
|-----------------------|--------|-------------------------------------|----------------------------|------------------------|
| Save extracted data   | POST   | `/api/ai/extract`                   | `CVAnalysisDto`            | `{ message }`          |
| Get analysis by email | GET    | `/api/ai/analysis/{email}`          | –                          | `{ ...analysis }`      |

**CVAnalysisDto Example:**
```json
{
  "personalInformation": {
    "name": "John Doe",
    "email": "john@example.com",
    "phone": "123456789",
    "address": "NYC"
  },
  "skills": [
    { "skillName": "C#", "proficiencyLevel": "Expert" }
  ],
  "workExperiences": [
    { "jobTitle": "Dev", "company": "Acme", "startYear": 2020, "jobDescription": "..." }
  ],
  "educations": [
    { "institution": "MIT", "degree": "BSc", "fieldOfStudy": "CS" }
  ],
  "projects": [
    { "projectName": "MyApp", "description": "A cool app" }
  ]
}
```

---

### Job Recommendations
| Action                | Method | URL                                 | Body/Params                | Response (200)         |
|-----------------------|--------|-------------------------------------|----------------------------|------------------------|
| Recommend jobs        | POST   | `/api/ai/recommend/{userId}`        | –                          | `{ ...recommendations }`|
| Recommender system    | POST   | `/api/ai/recommenderSystem/{userId}`| –                          | `{ ...recommendations }`|

---

### CV Upload & Download

| Action                | Method | URL                                 | Body/Params                | Response (200)         |
|-----------------------|--------|-------------------------------------|----------------------------|------------------------|
| Upload CV (PDF)       | POST   | `/api/ai/upload`                    | `multipart/form-data` with `cv` (PDF file) | `{ Message, CvId, FileName, UploadDate }` |
| Download CV (PDF)     | GET    | `/api/ai/download-cv`               | – (auth required)          | PDF file download      |

**Upload Example (form-data):**
- Key: `cv`
- Value: (select your PDF file)

**Download Example:**
- Authenticated GET request to `/api/ai/download-cv`  
- Returns: PDF file as download

---

## Error Handling

- `200 OK`: Success
- `201 Created`: Resource created
- `204 No Content`: Resource deleted
- `400 Bad Request`: Invalid request
- `401 Unauthorized`: Not authenticated
- `403 Forbidden`: Not allowed
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

Error responses include a JSON message explaining the issue.

---

## Security

- All endpoints (except `/api/auth/hello`) require a JWT token in the `Authorization` header.
- Use HTTPS for all requests.

---

## Swagger/OpenAPI

- **Swagger UI:**  
  https://careerpath-production.up.railway.app/swagger/index.html
- **OpenAPI JSON:**  
  https://careerpath-production.up.railway.app/swagger/v1/swagger.json

---

**For any questions, see the Swagger UI or contact the backend team.** 