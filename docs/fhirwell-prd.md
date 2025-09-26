# FHIRWell Patient Management App - Product Requirements Document

## 1. Elevator Pitch

A web-based application for healthcare providers and staff to manage patient records using FHIR standards. The app enables users to create, edit, delete, and search for FHIR Patient resources via a FHIR server, focusing on open access and rapid usability for MVP.

## 2. Who is this app for

- Healthcare providers and administrative staff
- Developers and testers learning FHIR and healthcare interoperability

## 3. Functional Requirements

- List all patients from a FHIR server (name, gender, date of birth)
- Create new patients with a validated form (name, gender, date of birth, phone number)
- Edit existing patient records
- Delete patient records
- Search for patients by name (partial match)
- Bonus: Search by name or phone number (partial match)
- Support multiple FHIR server endpoints

## 4. Non-Functional Requirements

- Use FHIR REST APIs for all operations
- Validate all form fields
- Responsive performance for listing/searching
- Secure data in transit (HTTPS); open access for MVP
- Clear error messages and feedback
- Graceful handling of FHIR server errors
- Success: All CRUD/search operations work without errors
- UI/UX: Bootstrap, Bootstrap Icons, Font Awesome (free)
- Accessibility: Best practices for forms/navigation

## 5. User Stories

- As a user, I can view a list of all patients
- As a user, I can add a new patient
- As a user, I can edit a patient’s details
- As a user, I can delete a patient
- As a user, I can search by name or phone number
- As a user, I receive validation feedback for invalid data

## 6. User Interface

- Main page: Table of patients (name, gender, DOB)
- Search bar: Search by name or phone number
- Patient form: Create/edit with validation and errors
- Action buttons: Edit/delete per patient
- Responsive for desktop/mobile
- Styled with Bootstrap, Bootstrap Icons, Font Awesome (free)
