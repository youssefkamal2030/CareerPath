# Career Path API Documentation

## Overview

This document provides comprehensive information about the Career Path API endpoints, their request/response formats, and usage examples.

## Authentication

All API endpoints require authentication. Include a valid JWT token in the Authorization header:

```
Authorization: Bearer {your_jwt_token}
```

## API Endpoints

### CV Analysis

#### Get CV Analysis

Retrieves a user's CV analysis data.

- **URL**: `/api/cv-analysis/{userId}`
- **Method**: `GET`
- **URL Parameters**:
  - `userId`: ID of the user

**Success Response**:
- **Code**: 200 OK
- **Content Example**:
```json
{
  "personalInformation": {
    "name": "John Doe",
    "email": "john.doe@example.com",
    "phone": "+1234567890",
    "address": "123 Main St, City, Country"
  },
  "skills": [
    {
      "skillName": "C#",
      "proficiencyLevel": "Expert"
    },
    {
      "skillName": "JavaScript",
      "proficiencyLevel": "Intermediate"
    }
  ],
  "workExperiences": [
    {
      "jobTitle": "Full Stack Developer",
      "jobLevel": "Senior",
      "company": "Tech Corp",
      "startYear": 2018,
      "startMonth": 3,
      "endYear": 2022,
      "endMonth": 6,
      "jobDescription": "Developed and maintained enterprise web applications"
    }
  ],
  "educations": [
    {
      "institution": "University of Technology",
      "degree": "Bachelor's",
      "fieldOfStudy": "Computer Science",
      "startYear": 2014,
      "startMonth": 9,
      "endYear": 2018,
      "endMonth": 6,
      "educationLevel": "Bachelor"
    }
  ],
  "projects": [
    {
      "projectName": "E-commerce Platform",
      "startDate": "2021-02-01T00:00:00",
      "endDate": "2021-08-30T00:00:00",
      "url": "https://github.com/johndoe/ecommerce",
      "description": "Built a full-featured e-commerce platform using ASP.NET Core and React"
    }
  ]
}
```

**Error Response**:
- **Code**: 404 Not Found
- **Content**: `{ "message": "CV analysis not found for the specified user" }`

#### Save CV Analysis

Saves or updates a user's CV analysis data.

- **URL**: `/api/cv-analysis/{userId}`
- **Method**: `POST`
- **URL Parameters**:
  - `userId`: ID of the user
- **Request Body**: CV Analysis data in JSON format (see example response above)

**Success Response**:
- **Code**: 200 OK
- **Content**: `{ "success": true, "message": "CV analysis saved successfully" }`

**Error Response**:
- **Code**: 400 Bad Request
- **Content**: `{ "message": "Invalid CV analysis data" }`

### Job Recommendations

#### Get Job Recommendations

Retrieves job recommendations based on the user's profile data.

- **URL**: `/api/recommendations/jobs/{userId}`
- **Method**: `GET`
- **URL Parameters**:
  - `userId`: ID of the user

**Success Response**:
- **Code**: 200 OK
- **Content Example**:
```json
{
  "recommendations": [
    {
      "jobTitle": "Senior Software Engineer",
      "company": "Tech Innovations Inc",
      "description": "Looking for an experienced software engineer to join our team...",
      "matchScore": 0.85
    },
    {
      "jobTitle": "Full Stack Developer",
      "company": "Digital Solutions",
      "description": "Seeking a full stack developer with experience in .NET and React...",
      "matchScore": 0.78
    }
  ]
}
```

**Error Response**:
- **Code**: 404 Not Found
- **Content**: `{ "message": "User data not found" }`

### Skill Recommendations

#### Get Skill Recommendations

Retrieves skill-based job recommendations by matching user skills with job descriptions.

- **URL**: `/api/recommendations/skills/{userId}`
- **Method**: `GET`
- **URL Parameters**:
  - `userId`: ID of the user

**Success Response**:
- **Code**: 200 OK
- **Content Example**:
```json
{
  "recommendations": [
    {
      "job_title": "Full Stack Developer",
      "similarity_score": 0.89
    },
    {
      "job_title": "Backend Engineer",
      "similarity_score": 0.75
    },
    {
      "job_title": "Software Architect",
      "similarity_score": 0.68
    }
  ]
}
```

**Error Response**:
- **Code**: 404 Not Found
- **Content**: `{ "message": "User skills data not found" }`

## Integration with External API

The recommendation endpoints connect to an external AI recommender system API:

- Job Recommendations: Calls the `/recommend` endpoint
- Skill Recommendations: Calls the `/recomendersystem` endpoint

The external API processes the user's profile data and returns relevant job matches based on skills, work experience, and projects.

## Error Handling

All endpoints return appropriate HTTP status codes:

- `200 OK`: The request was successful
- `400 Bad Request`: The request was invalid or could not be processed
- `401 Unauthorized`: Authentication failure
- `404 Not Found`: The requested resource was not found
- `422 Unprocessable Entity`: The request was well-formed but could not be processed due to semantic errors
- `500 Internal Server Error`: An unexpected server error occurred

Error responses include a message explaining the issue.

## Request Format Examples

### Job Recommendation Request

```json
{
  "skills": [
    {
      "skillName": "C#",
      "proficiencyLevel": "Expert"
    }
  ],
  "work_experience": [
    {
      "jobTitle": "Full Stack Developer",
      "jobLevel": "Senior",
      "company": "Tech Corp",
      "startYear": 2020,
      "startMonth": 5,
      "endYear": 2023,
      "endMonth": 6,
      "jobDescription": "Full stack .NET developer responsible for all products"
    }
  ],
  "projects": [
    {
      "projectName": "Chat Application",
      "startDate": "2022-01-01T00:00:00",
      "endDate": "2022-06-30T00:00:00",
      "url": "github.com/username/chatapp",
      "description": "Full stack chat application with real-time messaging"
    }
  ],
  "user_skills": [
    {
      "skillName": "C#",
      "proficiencyLevel": "Expert"
    }
  ],
  "job_descriptions": [
    "Full stack .NET developer responsible for all products"
  ]
}
```

### Skill Recommendation Request

```json
{
  "user_skills": "C#, JavaScript, React, .NET Core, SQL",
  "job_descriptions": [
    {
      "title": "Full Stack Developer",
      "description": "We are looking for a full stack developer with experience in .NET Core, React, and SQL"
    },
    {
      "title": "Backend Engineer",
      "description": "Seeking experienced C# developer for backend systems development"
    }
  ]
}
``` 