# Task TASK-001: Create solution and project structure

## Description
Create the .NET 10 solution with three source projects (AiDevLoop.Cli, AiDevLoop.Core, AiDevLoop.Shell) and three test projects (AiDevLoop.Core.Tests, AiDevLoop.Shell.Tests, AiDevLoop.E2E.Tests). Ensure project reference graph follows the Functional Core / Imperative Shell boundary and shared build settings via `Directory.Build.props`.

## Definition of Done
- [ ] Solution file exists and references all 6 projects
- [ ] `dotnet build -warnaserror` succeeds with zero warnings
- [ ] `dotnet test` executes successfully
- [ ] `Directory.Build.props` sets `TargetFramework`, `Nullable`, and `ImplicitUsings`
- [ ] Project reference graph: Core → nothing, Shell → Core, Cli → Core + Shell
- [ ] No lint/type errors

## Steps
1. Create solution `AiDevLoop.sln`.
2. Add projects: `AiDevLoop.Core`, `AiDevLoop.Shell`, `AiDevLoop.Cli` and test projects.
3. Add project references according to architecture.
4. Create `Directory.Build.props` with shared settings.
5. Add minimal placeholder source files so projects compile.
6. Run `dotnet build -warnaserror` and `dotnet test`.
7. Document implementation notes and run code review.

## Acceptance Criteria
- [ ] All validation criteria in Definition of Done are satisfied
- [ ] Implementation notes and review report saved under `./context/`
- [ ] Task is ready for user verification before commit
