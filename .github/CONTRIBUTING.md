# Contributing to City Skylines Mods

## Welcome

Thank you for your interest in contributing to City Skylines Mods by Tec de Monterrey, campus Querétaro! We welcome all contributions, including bug fixes, new features, and documentation improvements.

## Setting Up Your Development Environment

### Prerequisites

- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/preview/vs2022/)
- [Unity 2020.3.19f1](https://unity3d.com/es/get-unity/download/archive)
- [City Skylines](https://store.steampowered.com/app/255710/Cities_Skylines/)
- [Steam](https://store.steampowered.com/about/)
- [Steam Workshop](https://steamcommunity.com/workshop/about/?appid=255710)
- [Workshop Uploader](https://steamcommunity.com/sharedfiles/filedetails/?id=450877484)
- [Windows Machine](https://www.microsoft.com/en-us/windows/get-windows-10)
- [Git](https://git-scm.com/downloads)

### Installation

1. Clone or Fork the repository
2. Clone your fork locally: `git clone https://github.com/City-Skylines-Modding/City-Skylines-Mod.git`
3. Navigate to the directory: `cd City-Skylines-Mod`
4. Install any necessary dependencies
5. Open the project in Visual Studio 2022
6. Creating new branches: `git checkout -b <branch-name>`

## Contributing Guidelines

1. Ensure your changes do not break existing functionality. Run the tests if available.
2. Ensure your changes are consistent with the overall project architecture.
3. Ensure your changes are consistent with the coding style of the rest of the project.
4. Ensure your changes do not add unnecessary dependencies.
5. Ensure your code is properly documented and commented.
6. Update any relevant documentation to reflect your changes
7. Ensure your changes do not introduce any security vulnerabilities.
8. Ensure your changes are not unnecessarily complex.
9. Ensure your changes do not duplicate existing code.
10. Ensure your changes are not overly optimized.
11. Ensure your changes are not overly generic.
12. Ensure your changes are not overly opinionated.
13. Ensure your changes are not overly coupled.
14. Ensure your changes are not overly abstracted.
15. Ensure your changes are not overly complicated.
16. Ensure your changes are not overly clever.

### Readme

#### Main Readme

If you want to add a new mod to the main readme, you must add the following information:

- Mod Name
- Description
- Author
- Status

The available statuses are:

- Research
- In Progress
- Bug Fixes
- Completed
- Deployed

### Bug Reports

1. Ensure the bug was not already reported by searching on GitHub under [Issues](https://github.com/City-Skylines-Modding/City-Skylines-Mod/issues)
2. If you're unable to find an open issue addressing the problem, [open a new one](https://github.com/City-Skylines-Modding/City-Skylines-Mod/issues/new). Be sure to use the template provided when opening an issue.

### Commits

1. Ensure your commit messages are descriptive and in the proper format.
2. Ensure your commits only include related changes. If you have unrelated changes, split them into separate commits.
3. Ensure your commits are properly signed.

#### Commit Message Format

Each commit message consists of

- a **type**,
- a **scope**,
- a **subject**,
- and a **body**.

```{text}
<type>(<scope>): <subject>
<BLANK LINE>
<body>
```

The **type** is one of the following:

- **build**: Changes that affect the build system or external dependencies (example scopes: gulp, broccoli, npm)
- **ci**: Changes to our CI configuration files and scripts (example scopes: Travis, Circle, BrowserStack, SauceLabs)
- **docs**: Documentation only changes
- **feat**: A new feature
- **fix**: A bug fix
- **perf**: A code change that improves performance
- **refactor**: A code change that neither fixes a bug nor adds a feature
- **style**: Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, etc)
- **test**: Adding missing tests or correcting existing tests

### Branches

1. Ensure your branch name is descriptive and in the proper format.
2. Ensure your branch name is prefixed with the type of change it contains.

#### Branch Name Format

Each branch name consists of

- a **type**,
- a **scope**,
- and a **subject**.

```{text}
<type>/<subject>
```

If it is a change, it should be prefixed with the type of change it contains. If it is a feature, it should be prefixed with the type of feature it contains.

1. Ensure your branch name is prefixed with the issue number it addresses.

Each branch name consists of

- a **type**,
- a **issue_number**,
- a **subject**,

```{text}
<type>/<issue-number>-<subject>
```

### Pull Requests

1. Ensure your branch is up-to-date with the base branch.
2. Push your changes to your forked repository.
3. Create a new pull request against the base branch.
4. Describe your changes in the pull request. Be sure to use the template provided when opening a pull request.
5. Wait for a maintainer to review your pull request. Make any requested changes.
6. Once your pull request is approved and merged, you can pull the changes from the base branch to your forked repository.
7. Delete your branch.

## Code of Conduct

In order to foster an inclusive, kind, harassment-free, and cooperative community, we enforce this code of conduct on our open source projects.

### Our Pledge

Please adhere to the project's [Code of Conduct](./CODE_OF_CONDUCT.md). Ensure that your interactions with the community are respectful and positive.

Thank you for contributing to City-Skyline-Mod!
