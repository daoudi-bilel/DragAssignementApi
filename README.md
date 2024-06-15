# C# Backend Project for LD Academy Training

This project is a part of the LD Academy training program. It is a C# backend application built with ASP.NET Core, featuring JWT authentication with refresh/access tokens, automatic reuse detection for refresh tokens, and various controllers for managing projects, objectives, members, and issues. Additionally, it includes functionality for sending emails.

## Features

- **JWT Authentication**: Secure authentication using JWT access and refresh tokens.
  - **Refresh Token Mutation**: Supports automatic detection and prevention of refresh token reuse.
- **Controllers**:
  - **Projects**: Manage projects.
  - **Objectives**: Handle objectives related to projects.
  - **Members**: Manage members associated with projects.
  - **Issues**: Track and manage issues within projects.
- **Email Sending**: Send emails using the integrated mail service.

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or another compatible database
- [Postman](https://www.postman.com/downloads/) (optional, for API testing)

### Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/daoudi-bilel/DragAssignementApi
   cd DragAssignementApi
