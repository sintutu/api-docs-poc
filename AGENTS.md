# Repository Guidelines

## Project

This is a weekend proof of concept for automatically generating useful API documentation from OpenAPI and synchronising it with Apidog. Use synthetic data only. Its goal is to prove that API changes can update useful documentation without developers maintaining a separate artefact manually.

Keep the implementation small and focused: do not introduce production infrastructure or over-engineer. Before implementing a non-obvious component or integration, explain the architectural decision and why it is the smallest approach that proves the concept.

## Acceptance Criteria

The completed proof of concept must:

1. Provide an OpenAPI specification for the API.
2. Define meaningful, structured schemas in that specification.
3. Produce documentation that is usable in Apidog.
4. Update the documentation when the API changes.
5. Leave a process that can eventually be automated from CI.

## Project Structure & Module Organization

This repository is currently an empty scaffold. Put source in `src/`, tests in `tests/` (or alongside source where conventional), and static resources in `assets/`. Keep package, formatter, and CI configuration at the root. Keep generated output such as `dist/` or `coverage/` separate and ignored by Git.

## Build, Test, and Development Commands

No build system is configured yet. When adding one, document canonical, repeatable commands in the README. For example:

```sh
npm run dev    # start local development
npm test       # run the full test suite
npm run lint   # check code quality and formatting
npm run build  # produce a deployable build
```

Expose validation through the selected build tool, not only one-off local commands.

## Coding Style & Naming Conventions

Follow the adopted formatter and linter. Use 2 spaces for YAML, JSON, and Markdown lists unless the language standard differs. Use descriptive `camelCase` variables/functions, `PascalCase` classes/components, and `kebab-case` filenames. Keep modules focused and avoid unused exports.

## Testing Guidelines

Add tests with each behavior change. Name them after the behavior, for example `auth-service.test.ts` or `test_auth_service.py`. Cover normal behavior, invalid input, and regressions. Run the full suite and linting before opening a pull request; set coverage thresholds after a test runner is established.

## Commit & Pull Request Guidelines

No existing Git history establishes a convention. Use concise, imperative subjects such as `Add API reference layout`. Keep commits narrowly scoped. Pull requests must describe the change, note validation, link issues, and include screenshots for visible documentation or UI changes. Request review only when buildable and passing.
