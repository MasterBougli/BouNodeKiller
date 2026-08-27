# BouNodeKiller

BouNodeKiller is a Windows utility for spotting Node.js processes, understanding what they are running, and stopping one, several, or all of them when needed.

## Quick Links

- [Français](README.fr.md)
- [English](README.en.md)
- [Español](README.es.md)
- [GitHub repository](https://github.com/MasterBougli/BouNodeKiller)
- [Donate via Streamlabs](https://streamlabs.com/bouglitv)

## What it does

- lists `node.exe` and `nodejs.exe` processes
- shows the full command line
- identifies the script or target being executed
- shows the user, PID, parent process, and start time
- shows the detected launch working directory
- lets you stop the selected process
- lets you stop all Node processes at once

## Build State

- Windows desktop app built with WPF
- Current version: `1.0.4`
- GitHub Actions CI on every push and pull request
- GitHub Actions release builds for tagged releases

## Release

Releases are published automatically from tags that start with `v`.

Example:

```bash
git tag v1.0.4
git push origin v1.0.4
```

## Screenshots and UI

The app includes:

- a process table with search
- a launch-directory column
- a version label
- an About window with links to the repository and releases

