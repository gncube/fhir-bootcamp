## PatientBridge Workspace Copilot Instructions

### Clean Architecture & Feature-based Structure

- Use Clean Architecture principles: separate Core, Infrastructure, and UI (Client) layers.
- Organize code by feature, not by technical type, within each layer.
- All business logic and domain models go in `src/PatientBridge.Core`.
- Infrastructure (e.g., data access, external services) goes in `src/PatientBridge.Infrastructure`.
- Blazor WebAssembly client goes in `src/Client` (project: `PatientBridge.Client`).
- Unit tests go in `tests/PatientBridge.UnitTests`.

### Solution & Project Setup

**Development Commands:**

```bash
# Solution Setup
dotnet new sln
dotnet new blazorwasm -n PatientBridge.Client -o src/Client --auth IndividualB2C -p -e -f net9.0
dotnet new classlib -o src/PatientBridge.Core
dotnet new classlib -o src/PatientBridge.Infrastructure
dotnet new xunit -o tests/PatientBridge.UnitTests

# Development
dotnet build
dotnet run --project src/Client
dotnet test
```

### Documentation Organization

All architecture and design documentation is in the `docs/` folder:

```bash
docs/
├── architecture/
│   ├── decisions/        # ADRs
│   ├── diagrams/         # PlantUML/C4 diagrams
│   └── principles.md     # Core principles
├── domain/
│   ├── aggregates/       # Aggregate documentation
│   └── entities/         # Entity documentation
└── infrastructure/
    ├── persistence/      # Data storage decisions
    └── security/         # Security architecture
```

**ADRs:**
Keep Architectural Decision Records in `docs/architecture/decisions/`.

**Diagrams:**
Use PlantUML or C4 Model for system, container, and component diagrams in `docs/architecture/diagrams/`.

### Coding Standards

- Use C# 9+ features and .NET 9.0 as target framework.
- Follow SOLID, DRY, and YAGNI principles.
- Use dependency injection throughout.
- Write unit tests for all business logic.
- Use feature folders for Blazor components and pages.

### Workflow

1. Scaffold solution and projects as above.
2. Implement features in Core, Infrastructure, and Client layers.
3. Document all major decisions as ADRs.
4. Keep diagrams and principles up to date in docs.
5. Write and run unit tests for all new features.

### Notes

- Do not add links, images, or integrations unless explicitly requested.
- Use '.' as the working directory for all commands.
- Do not create extra folders unless specified.
- All generated code and docs must follow the above structure and standards.
