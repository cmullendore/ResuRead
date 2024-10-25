# ResuRead
A simple proof of concept demonstrating a terminal application accepting the path to a resume file, uploading this file to an AI/LLM model, and return the unstructured resume file as structured JSON.

## Case Scenario
Resume files are effectively unstructured data. For effective system integration, this unstructured data must be understood and translated into a defined structure so as to be consumable by enterprise systems.

In support of this scenario, this application allows a user to run the command line application, providing an absolute path to the resume. The application then uploads that resume to a configurable AI/LLM agent with a configurable directive. Per the directive, the agent then returns the consumed resume in the desired structure.

## Goals
- Allow for configuration and flexibility as much as is reasonable. For example, the prompt to the LLM may need refining over time. This refinement should not necessitate developer activity to modify.
- Allow the possibility for alternate models to be used without needing to rebuild the entire application. By using a plugin style model, additional libraries can be created for any alternative agent, and these libraries can be consumed by the engine without needing to rebuild the engine itself.
- Allow alternative interfaces to be developed beyond the simple command line interface. For example, allow the functionality to be exposed via a web interface without requiring significant rework.
- Enable direct integration through a similar plugin model for output destinations and possible integrations with target systems without needing significant rework of the core engine or agent integration.

## Components
The solution is composed of the following projects:

### ResuRead.CLI
The main command-line interface for the application. It owns creating the core components as necessary for its own run such as loading configuration, logging, and the engine itself. It is intentionally minimal as it is really nothing more than a loader for the engine.

### ResuRead.Engine
The core engine for ResuRead that accepts the configuration, loads the correct plugins, and calls the necessary components such as the agent plugin and (TBD) any future library for integration with a target business system.

This also serves as the consumable library for plugins that are to implement the necessary interfaces so as to be loadable by the engine. Any plugin that inherits from the AgentModelBase class, which includes the IAgentModel interface, can be loaded by the ResuRead engine.

This should not know about any specific AI/LLM's functionality, interacting with the model solely via the IAgentModel interface.

## ResuRead.Models.OpenAI
The initial plugin for ResuRead containing the code for OpenAI ChatGPT. This contains all code and functionality that is specific to the OpenAI API. This library is dynamically loaded at runtime as defined in the configuration via implementation of the IAgentModel interface (through inheriting from AgentModelBase).

## AgentPrompts.json
This is the primary configuration file containing identification of the agent library to load, agent class that implements the IAgentModel interface, and model-specific initiation parameters for the specified library to load and class to be instantiated. 

This file also contains the key initialization prompt to be used for configuring the model prior to analysis of the uploaded resume.

Note that for the ChatGPT plugin the Request and Reset prompts are not used or necessary.

# Development State
Currently only the ChatGPT module needs additional work. The prompt works as expected as demonstrated via [OpenAI's web UI](https://chatgpt.com/share/671b1912-a23c-800d-9db2-646fb92db7b4). However, there remains issues with ChatGPT's ability to consume the uploaded resume file and return a valid result via the API.

**This is not currently where I would like it to be** for demonstration purposes and additional work will continue until it behaves as expected.

# Usage
1. [Acquire an API key](https://platform.openai.com/api-keys) for using against the ChatGPT API.
2. Configure this key as an environment variable in the terminal window/environment to be used for running the CLI. The variable must be named **ApiKey** and the key itself must be the value. ex: `set ApiKey=sk-svc...` before running the tool.
3. run ResuRead.CLI passing the fully qualified file path to the resume to be uploaded: `ResuRead.CLI.exe c:\johndoe.pdf` and press enter.
   
This application can be compiled to Windows, Linux, and MacOS compatible binaries and should be runable from any of those platforms.