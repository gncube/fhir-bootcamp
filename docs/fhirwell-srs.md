# FHIRWell Patient Management App - Software Requirements Specification

## System Design

- Mobile-first, responsive web app for managing FHIR Patient resources
- Card-based dashboard with search, add, edit, and delete patient features
- Deployed as a static web app on Azure Static Web Apps

## Architectural pattern

- Component-based architecture using Blazor WebAssembly
- Follows MVU (Model-View-Update) and separation of concerns

## State management

- Built-in Blazor state (cascading parameters, component state, local storage for persistence)
- Minimal global state, mostly per-page/component

## Data flow

- UI events trigger Blazor component methods
- Components call FHIR REST APIs via HttpClient
- API responses update component state and UI
- Toast notifications for feedback

## Technical Stack

- C# with Blazor WebAssembly (net9.0)
- Bootstrap, Bootstrap Icons, Font Awesome (free)
- Azure Static Web Apps for hosting
- FHIR server (configurable endpoint)

## Authentication Process

- MVP: Open access, no authentication
- Future: Option to add Azure AD B2C or OAuth2 for secure access

## Route Design

- / : Home (patient list, search, add)
- /patient/{id} : Patient details and edit
- /add : Add new patient
- /settings : App settings and FHIR server config

## API Design

- Uses FHIR REST API (R4 or later)
- GET /Patient : List/search patients
- POST /Patient : Create patient
- PUT /Patient/{id} : Update patient
- DELETE /Patient/{id} : Delete patient
- Supports query params for name, phone, etc.

## Database Design ERD

- No local database; data stored on external FHIR server
- Patient resource fields: id, name, gender, birthDate, telecom (phone)
