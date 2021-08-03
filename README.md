<p align="center">
  <h3 align="center">ModerationToolbox</h3>
  <p align="center">An SCP: SL Plugin that transends player punishments, targeting servers running <a href="https://github.com/Exiled-Team/EXILED" target="_blank">EXILED</a>.</p>
  <p align="center"><a href="https://github.com/jacany/ModerationToolbox/actions/workflows/build.yml" target="_blank"><img src="https://github.com/jacany/ModerationToolbox/actions/workflows/build.yml/badge.svg" /></a></p>
</p>

---

## Features

- [x] Connecting to Database
- [ ] Muting Players
- [X] Banning Players

## Building from Source

Run `git clone --recursive https://github.com/jacany/ModerationToolbox.git` to clone the repository.

There are two ways to build. <br />
<a href="#visual-studio-2019">Visual Studio 2019</a> <br />
<a href="#dotnet-cli">Dotnet CLI</a>

### Visual Studio 2019

Open the solution is VS2019, let it restore all of the NuGet packages, and then build the solution.

### Dotnet CLI

Navigate into the repository in your terminal, then run: <br />
`dotnet restore` to restore all NuGet packages <br />
`dotnet build` to build the solution <br />
That's It!

## Setup

Simply install the plugin and it's dependencies, run your server once, stop it, configure the plugin with your database credentials, and the rest should be automatic.
