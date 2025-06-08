---
title: Getting Started
---

# Getting Started

Follow these steps to get up and running with **SimArena**.

## 1. Installation

```bash
git clone https://github.com/arbyun/Simulation-Toolkit-AI.git
```

Then, direct your command prompt to the folder where you have cloned the repository and run:

```bash
dotnet build
```

## 2. Unity Integration

If you are using Unity, you can import the project as a submodule on your Assets folder:

```bash
git submodule add https://github.com/arbyun/Simulation-Toolkit-AI.git External/SimArena
```

For full integration, you will need to have System.Text.Json and System.Text.Json.Serialization available in your project. 
If you are using .NET 4.x, you will need to install the System.Text.Json NuGet package. If you are using .NET 5 or later, 
you can use the built-in System.Text.Json namespace. 

Unity does not support System.Text.Json natively, so you will need to import the System.Text.Json.dll file from the NuGet package into your Unity project's Assets folder.

A recommended way to do this is using [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity). Simply follow the 
instructions on the GitHub page to install the package, and then use the NuGet package manager to install System.Text.Json.

## 3. Usage

For usage instructions, please refer to the [Usage](usage.md) documentation.