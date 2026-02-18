# AiDevLoop
“Spec in. Software out.”

## Overview

AiDevLoop is a .NET 10.0 CLI tool that automates the Phase 2 development loop of the LLM-Assisted Development Methodology. It orchestrates LLM agents to implement tasks from an `implementation-plan.md`, run validations, manage review-fix cycles, and commit with user approval.

## Agents

The system uses a set of specialized agents to manage the development workflow. These agents are defined in the `.github/agents` directory.

### `current-task-runner.agent.md`

This is the master orchestrator agent. It follows the steps outlined in `prompts/current-task-prompt.md` to drive the entire development loop, from selecting a task to creating a pull request.

### `dotnet-task-coder.agent.md`

This agent is responsible for the actual coding. It takes a task definition and implements the required C# code, following the project's established coding standards and architectural patterns. It also creates tests and documents its work.

### `dotnet-code-reviewer.agent.md`

This agent acts as a senior C# code reviewer. It analyzes code for adherence to project standards, best practices, and overall quality, providing feedback categorized into blocking issues, non-blocking suggestions, and nitpicks.

## How to Run

The primary entry point for the automated workflow is the `current-task-runner` agent. To start the process, you invoke this agent.

The specific command to run the agent will depend on the CLI tool that interprets these agent files. Assuming a hypothetical `agent-runner` CLI, the command would be:

```bash
agent-runner ./.github/agents/current-task-runner.agent.md
```

This will kick off the entire automated task implementation process.
