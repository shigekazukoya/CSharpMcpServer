using System.Collections.Generic;

namespace MemoryBankTools.Constants;

public static class MemoryBankConstants
{
    public static readonly string[] CoreMemoryBankFiles = new[]
    {
        "projectbrief.md",
        "productContext.md", 
        "systemPatterns.md", 
        "techContext.md", 
        "activeContext.md", 
        "progress.md"
    };

    public static readonly Dictionary<string, string> CoreFileTemplates = new()
    {
        ["projectbrief.md"] = "# Project Brief\n\n" +
                            "## Overview\n\n" +
                            "<!-- Foundation document that shapes all other files. Define core requirements and goals. -->\n\n" +
                            "## Project Scope\n\n" +
                            "<!-- Define the boundaries of what this project will and won't do. -->\n\n" +
                            "## Key Requirements\n\n" +
                            "<!-- List the essential requirements that must be fulfilled. -->\n\n",
        
        ["productContext.md"] = "# Product Context\n\n" +
                              "## Purpose\n\n" +
                              "<!-- Why this project exists. -->\n\n" +
                              "## Problems Solved\n\n" +
                              "<!-- What problems this project solves. -->\n\n" +
                              "## User Experience Goals\n\n" +
                              "<!-- How the product should work from a user perspective. -->\n\n",
        
        ["systemPatterns.md"] = "# System Patterns\n\n" +
                              "## Architecture\n\n" +
                              "<!-- System architecture overview. -->\n\n" +
                              "## Design Patterns\n\n" +
                              "<!-- Key design patterns in use. -->\n\n" +
                              "## Component Relationships\n\n" +
                              "<!-- How components interact with each other. -->\n\n" +
                              "## Critical Implementation Paths\n\n" +
                              "<!-- Key technical workflows. -->\n\n",
        
        ["techContext.md"] = "# Technical Context\n\n" +
                           "## Technologies\n\n" +
                           "<!-- Technologies used in this project. -->\n\n" +
                           "## Development Setup\n\n" +
                           "<!-- How to set up the development environment. -->\n\n" +
                           "## Technical Constraints\n\n" +
                           "<!-- Limitations and constraints to be aware of. -->\n\n" +
                           "## Dependencies\n\n" +
                           "<!-- External dependencies and how they're managed. -->\n\n",
        
        ["activeContext.md"] = "# Active Context\n\n" +
                             "## Current Focus\n\n" +
                             "<!-- What is currently being worked on. -->\n\n" +
                             "## Recent Changes\n\n" +
                             "<!-- Recent important changes to the project. -->\n\n" +
                             "## Next Steps\n\n" +
                             "<!-- What needs to be done next. -->\n\n" +
                             "## Active Decisions\n\n" +
                             "<!-- Important decisions being considered. -->\n\n" +
                             "## Learnings and Insights\n\n" +
                             "<!-- Recent learnings that impact the project. -->\n\n",
        
        ["progress.md"] = "# Progress\n\n" +
                        "## Completed\n\n" +
                        "<!-- Features or tasks that have been completed. -->\n\n" +
                        "## In Progress\n\n" +
                        "<!-- Features or tasks currently being worked on. -->\n\n" +
                        "## Planned\n\n" +
                        "<!-- Features or tasks planned for the future. -->\n\n" +
                        "## Known Issues\n\n" +
                        "<!-- Known issues or bugs that need attention. -->\n\n"
    };
}
