# Contributing to GameFuse SDK

We welcome contributions to the GameFuse SDK for Unity! This document provides guidelines and instructions for contributing.

## Code of Conduct

Please help us keep the GameFuse community open and inclusive. Be respectful to others and follow good practices when contributing code.

## How to Contribute

### Reporting Bugs

If you find a bug in the SDK:

1. Check if the bug has already been reported in our [Issues](https://github.com/gamefuse/unity-sdk/issues) page.
2. If not, create a new issue with a clear title and description.
3. Include steps to reproduce the bug, expected behavior, and actual behavior.
4. Include your Unity version, SDK version, and platform information.

### Suggesting Enhancements

1. Check if the enhancement has already been suggested in our [Issues](https://github.com/gamefuse/unity-sdk/issues) page.
2. If not, create a new issue with a clear title and description of your enhancement suggestion.
3. Explain why this enhancement would be useful to most GameFuse SDK users.

### Pull Requests

1. Fork the repository and create your branch from `main`.
2. Make your changes and ensure they follow our coding conventions.
3. Add or update tests as necessary.
4. Ensure all tests pass.
5. Update documentation as needed.
6. Submit a pull request with a clear description of your changes.

## Development Setup

1. Clone the repository.
2. Open the project in Unity 2022 LTS or later.
3. Install the required dependencies if needed.

## Coding Conventions

- Follow C# naming conventions:
  - PascalCase for public members and types
  - camelCase for private members
  - Use meaningful names that describe the purpose of the variable, method, or class
- Adhere to SOLID principles, particularly the Single Responsibility Principle
- Keep methods concise and focused on a single responsibility
- Use XML documentation comments for all public APIs
- Use [JsonProperty] attributes for all JSON serialization
- Follow the architectural pattern established in the codebase

## Testing

- Write comprehensive tests for all new functionality
- Ensure all tests pass before submitting a pull request
- Aim for 100% test coverage of public APIs

## Documentation

- Update the documentation when adding or modifying features
- Use XML documentation comments for all public APIs
- Keep the README and other documentation files up-to-date

## License

By contributing to the GameFuse SDK, you agree that your contributions will be licensed under the same [MIT License](LICENSE) that covers the project.