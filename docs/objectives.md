---
title: Objectives
---

# Objectives

Objectives are the goals of a simulation. They determine when the simulation ends and how the results are evaluated. 
Objectives are attached to [simulations](simulation.md).

# Index

- [Objective Types](#objective-types)
- [Objective Creation](#objective-creation)
- [Using Objectives](#using-objectives)
- [Objective Configuration](#objective-configuration)

## Objective Types

Objectives can be of different types, each with its own set of parameters and behavior. We provide the following objective examples:

- `StepsObjective`: The simulation ends after a certain number of steps.
- `CapturePointObjective`: The simulation ends when a team captures a point a certain number of times.
- `DefendObjective`: The simulation ends when a team defends a point for a certain amount of time.
- `DeathmatchObjective`: The simulation ends when a team reaches a certain number of kills.

Objectives are intended to be extended by users to create custom objectives, according to their needs. 
Objectives are intended to be fully customizable.

## Objective Creation

Rather than a single class, objectives are created through a configuration class and managed by the following helper classes:

- `ObjectiveConfiguration`: The base class for all objective configurations. 
  - When loading an objective configuration, this class creates all the other necessary helper classes for the objective.
- `IObjectiveTracker`: The interface for all objective trackers. 
  - Objective trackers are responsible for tracking the progress of the objective and determining when it is complete, so the simulation can end. 
- `IBuildsResult`: Builds the input data from the tracker. 
  - We also call these "input data" since their only purpose is to provide the input to the result builder.
- `ISimulationResultBuilder`: Builds the result from the input data given.
- `ISimulationResult`: Stores the result of the objective.
  - The result is built from the input data by the result builder. 
  - Usually, the result is a POCO that can be logged if needed.

## Using Objectives

Objectives cannot be directly used; instead, they are loaded at runtime from your `GameConfiguration`.

## Objective Configuration

Objectives are configured through their respective configuration classes, which inherit from `ObjectiveConfiguration`. 
These configuration classes are used to create objective instances. See the [Custom Objectives](custom-objectives.md) 
documentation for more information on creating custom objective configurations.
