# Framework Architecture & Test Workflow

This document provides a non-technical overview of the **ReqnRoll Playwright BDD MSTest Framework**. It is designed to help stakeholders understand how business requirements are translated into automated tests and how results are reported.

## 🏗️ System Architecture & Workflow

```
graph TD
    %% LAYER 1: INPUTS
    subgraph INPUT_LAYER [🎯 1. Business Requirements]
        direction TB
        Gherkin["📄 <b>Gherkin Feature Files</b><br/>User Stories in Plain English<br/>(Given, When, Then)"]
    end

    %% LAYER 2: AUTOMATION CODE
    subgraph CODE_LAYER [⚙️ 2. Automation Logic]
        direction TB
        Steps["<b>Step Definitions</b><br/>Translates English to Code"]
        Hooks["<b>Test Hooks</b><br/>Handles Setup & Teardown"]
    end

    %% LAYER 3: PAGE OBJECT MODEL
    subgraph POM_LAYER [🏛️ 3. Structure & Elements]
        direction TB
        Pages["<b>Page Object Classes</b><br/>Maps of Website Elements"]
        BasePage["<b>Base Action Library</b><br/>Reusable Clicks & Inputs"]
    end

    %% LAYER 4: ENGINE & DATA
    subgraph ENGINE_LAYER [🚀 4. Execution Engine]
        direction TB
        Playwright["<b>Playwright Driver</b><br/>Controls the Browser"]
        Config[("📂 <b>Config & Test Data</b><br/>JSON & RunSettings")]
    end

    %% LAYER 5: OUTPUTS
    subgraph OUTPUT_LAYER [📊 5. Actionable Insights]
        direction TB
        LivingDoc["📜 <b>Living Documentation</b><br/>Stakeholder Progress Report"]
        Screenshots[("📸 <b>Screenshot Vault</b><br/>Organized Visual Evidence")]
        Translator["<b>Exception Translator</b><br/>Friendly Error Messages"]
        Logs[("📝 <b>Technical Logs</b><br/>Detailed Execution History")]
    end

    %% FLOW CONNECTIONS
    Gherkin -->|Defined By| Steps
    Steps -->|Controls| Hooks
    Hooks -->|Uses| Pages
    Pages -->|Inherits From| BasePage
    BasePage -->|Drives| Playwright
    Playwright --- Config
    
    %% REPORTING FLOW
    Playwright --> Translator
    Translator --> LivingDoc
    Translator --> Screenshots
    Translator --> Logs

    %% STYLING - COMPLEMENTARY COLORS
    %% Blue for Input
    style INPUT_LAYER fill:#E3F2FD,stroke:#1976D2,stroke-width:2px,color:#0D47A1
    %% Orange for Code (Complementary to Blue)
    style CODE_LAYER fill:#FFF3E0,stroke:#F57C00,stroke-width:2px,color:#BF360C
    %% Purple for Structure
    style POM_LAYER fill:#F3E5F5,stroke:#7B1FA2,stroke-width:2px,color:#4A148C
    %% Green for Engine
    style ENGINE_LAYER fill:#F1F8E9,stroke:#689F38,stroke-width:2px,color:#1B5E20
    %% Grey/Brown for Evidence
    style OUTPUT_LAYER fill:#FAFAFA,stroke:#455A64,stroke-width:2px,color:#263238

    %% INDIVIDUAL SHAPE STYLING
    style Gherkin fill:#FFFFFF,stroke:#1976D2
    style Config fill:#DCEDC8,stroke:#689F38
    style Screenshots fill:#ECEFF1,stroke:#455A64
    style Logs fill:#ECEFF1,stroke:#455A64
    style LivingDoc fill:#C8E6C9,stroke:#2E7D32,font-weight:bold
```

---

## 📘 Component Descriptions

### **1. Stakeholder Requirements (Blue Layer)**
Everything starts with your business rules. We write these in plain language so everyone—from Project Managers to Developers—understands what is being tested.

### **2. Automation Logic (Orange Layer)**
The "Glue" that connects your business language to the actual automation script. It ensures that when a stakeholder says *"When I click login"*, the computer knows exactly which button to find.

### **3. Structure & Elements (Purple Layer)**
This is our "Virtual Map" of the website. By separating the **Logic** from the **Elements**, the framework becomes much easier to maintain. If a button moves or changes color, we only update it in one place.

### **4. Execution Engine (Green Layer)**
The technical core of the system.
*   **Playwright:** A modern, high-speed tool that controls browsers.
*   **Config & Test Data:** External storage for settings and test scenarios, keeping code separate from data.

### **5. Actionable Insights (Grey Layer)**
The most critical part for stakeholders. Instead of seeing scary code, you get:
*   **Living Documentation:** A clear report (LivingDoc) showing which business rules passed or failed.
*   **Organized Screenshots:** Visual evidence of the test path, organized by Date and Scenario.
*   **Exception Translator:** A custom tool that translates technical errors into human-friendly "Business Insights".
