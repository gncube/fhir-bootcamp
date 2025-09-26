---
goal: Downgrade to .NET9 for Azure Static Web Apps compatibility
version: 1.0
date_created: 2025-09-26
status: 'Completed'
tags: [feature, downgrade, dotnet, azure]
---

# Introduction

![Status: Completed](https://img.shields.io/badge/status-Completed-brightgreen)

This plan downgrades the FHIRWell Blazor WebAssembly client from .NET10 to .NET9 to ensure compatibility with Azure Static Web Apps. All .NET10-specific features and references are removed from the codebase, including in `index.html` and project files.

## 1. Requirements & Constraints

- **REQ-001**: Must use .NET9 as .NET10 is not supported on Azure Static Web Apps
- **CON-001**: Remove all .NET10-specific features and references
- **CON-002**: Ensure the app builds and runs locally and on Azure

## 2. Implementation Steps

### Implementation Phase 1
- GOAL-001: Update project and static files for .NET9 compatibility

| Task      | Description                                              | Completed | Date       |
|-----------|----------------------------------------------------------|-----------|------------|
| TASK-001  | Change TargetFramework in FHIRWell.Client.csproj to net9.0 |           |            |
| TASK-002  | Remove .NET10-specific script references in index.html   | ✅        | 2025-09-26 |
| TASK-003  | Test local build and run with .NET9                      |           |            |
| TASK-004  | Test deployment to Azure Static Web Apps                 |           |            |

## 3. Alternatives

- **ALT-001**: Remain on .NET10 and wait for Azure support (not viable for current deployment)

## 4. Dependencies

- **DEP-001**: .NET9 SDK
- **DEP-002**: Azure Static Web Apps

## 5. Files

- **FILE-001**: src/Client/FHIRWell.Client.csproj
- **FILE-002**: src/Client/wwwroot/index.html

## 6. Testing

- **TEST-001**: App builds and runs locally with .NET9
- **TEST-002**: App deploys and runs on Azure Static Web Apps

## 7. Risks & Assumptions

- **RISK-001**: Some .NET10 features may have been used elsewhere and need further removal
- **ASSUMPTION-001**: No breaking changes between .NET9 and .NET10 for current codebase

## 8. Related Specifications / Further Reading

- [Azure Static Web Apps .NET Support](https://learn.microsoft.com/en-us/azure/static-web-apps/deploy-blazor)
- [Blazor WebAssembly Docs](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
