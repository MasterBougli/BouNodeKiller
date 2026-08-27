# BouNodeKiller

BouNodeKiller is a Windows utility for spotting Node.js processes, understanding what they are running, and stopping one, several, or all of them when needed.

## Quick links

- [Français](README.fr.md)
- [Español](README.es.md)
- [GitHub repository](https://github.com/MasterBougli/BouNodeKiller)
- [Donate via Streamlabs](https://streamlabs.com/bouglitv)

## Features

- lists `node.exe` and `nodejs.exe` processes
- shows the full command line
- identifies the script or target being executed
- shows the user, PID, parent process, and launch time
- displays the detected launch working directory
- lets you stop the selected process
- lets you stop all Node processes at once

## Project status

- Windows desktop app built with WPF
- current version: `1.0.4`
- GitHub Actions CI on every push and pull request
- automatic release build for tags that start with `v`

## Release flow

Releases are created automatically from `v*` tags.

Example:

```bash
git tag v1.0.4
git push origin v1.0.4
```

## In the app

The UI includes:

- a searchable process table
- a launch-directory column
- a visible version label
- an About window with repository and releases links

