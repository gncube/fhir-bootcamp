# FHIRWell Patient Management App - User Interface Description Document

## 1. Introduction and Overview

This document describes the user interface design for the FHIRWell Patient Management App, a mobile-first web application for managing FHIR Patient resources. The design prioritizes a clean, modern, Apple-inspired look, with a card-based dashboard and support for dark mode.

## 2. User Personas

- Clinical staff (doctors, nurses) who need quick access to patient records
- Administrative staff (front desk, records) managing patient data
- Users are often on mobile devices, but may also use tablets or desktops

## 3. Information Architecture

- Home: Patient list (card view), search bar, add patient button
- Patient Details: Edit, delete, and view patient information
- Add/Edit Patient: Form for patient data entry
- Settings: FHIR server selection and app preferences

## 4. Wireframes

- Home screen: Search bar at top, scrollable patient cards, floating action button (FAB) for adding patients
- Patient card: Name, gender, date of birth, edit/delete button
- Add/Edit form: Large, touch-friendly fields, clear validation
- Settings: Simple list of options, server selector

## 5. Visual Design System

- Light, neutral color palette with subtle shadows and rounded corners
- Clean, modern typography and iconography (Bootstrap Icons, Font Awesome)
- Apple-inspired minimalism: focus on clarity, space, and ease of use
- Support for dark mode with automatic or manual toggle

## 6. UI Components

- Search bar (sticky at top)
- Patient card (name, gender, DOB, actions)
- Floating action button (FAB) for add
- Modal forms for add/edit
- Toast notifications for feedback
- Settings menu and server selector

## 7. Page Templates

- Home: Search bar, patient card list, FAB
- Patient Details: Card with info, edit/delete actions
- Add/Edit Patient: Modal form
- Settings: List of preferences and server endpoints

## 8. Interaction Design

- Tap patient card to view/edit details
- Swipe left on card to reveal delete action (mobile)
- FAB always visible for quick add
- Forms validate on input and submit, with instant feedback
- Smooth transitions and subtle animations for navigation
- Dark mode toggle in settings

## 9. Accessibility Considerations

- Large touch targets and clear focus indicators
- Sufficient color contrast in both light and dark modes
- Semantic HTML and ARIA roles for screen readers
- Keyboard navigation for all actions
- Descriptive labels and error messages

## 10. Design Specifications

- Mobile-first responsive layout, adapts to tablet/desktop
- Minimum 44x44px touch targets
- Font: System UI or Apple San Francisco
- Color palette: Light gray/white backgrounds, blue accents, dark mode uses dark gray/black backgrounds
- Icon set: Bootstrap Icons, Font Awesome (free)
- Animations: 150-250ms for transitions, ease-in-out
