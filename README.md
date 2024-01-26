[![Build and deploy ASP.Net Core app to Azure Web App - hcqs-backend](https://github.com/duong-hong-quan/HCQS-backend/actions/workflows/main_hcqs-backend.yml/badge.svg?branch=main&event=deployment)](https://github.com/duong-hong-quan/HCQS-backend/actions/workflows/main_hcqs-backend.yml)
# House Construction Quotation System - Back End Repository

## Overview
The House Construction Quotation System is a comprehensive solution designed to streamline the entire process of house construction, from generating quotes to managing construction contracts and overseeing the execution of the project. This system aims to enhance efficiency, accuracy, and transparency in the construction workflow.

## Features
1. Quotation Generation
The system allows users to input project details, materials, labor costs, and other relevant information to automatically generate accurate and detailed construction quotations. This feature ensures consistency and reduces the time and effort required for manual quote preparation.

2. Contract Management
Once a quote is accepted, the system facilitates the creation and management of construction contracts. Users can define project timelines, milestones, and deliverables, making it easier to track progress and manage expectations between the client and the construction team.

3. Construction Inventory Management
The system includes a module for inventory management, enabling users to keep track of construction materials, tools, and equipment. This feature helps prevent delays due to material shortages, ensures proper resource allocation, and optimizes inventory levels.

4. Project Execution Tracking
The system provides real-time tracking of construction progress, allowing project managers to monitor milestones, identify potential delays, and make informed decisions to keep the project on schedule. This feature enhances communication and collaboration among team members.

5. User Access Control
To ensure data security and privacy, the system implements user access control mechanisms. Different user roles (e.g., administrator, project manager, site supervisor) have varying levels of access to the system's functionalities, ensuring that sensitive information is only accessible to authorized personnel.

## Installation
### Prerequisites
1. Database: Set up a compatible database (e.g., MySQL, PostgreSQL).
2. Web Server: Install a web server IIS.
3. .NET: Ensure that .NET is installed on the server (compatible version).
4. Composer: Install Composer for .NET dependency management.
### Installation Steps
1. Clone the repository: git clone https://github.com/duong-hong-quan/HCQS-backend.git.
2. Configure the database connection in the appsettings.json file.
3. Install dependencies: composer install.
4. Run migrations to set up the database: db migration with entity framework.
## Usage
1. Access the system through the web browser.
2. Register an account or log in with existing credentials.
3. Navigate through the different modules (Quotation, Contract Management, Inventory, Project Tracking).
4. Follow the intuitive user interface to perform various tasks within each module.
5. Refer to the documentation for detailed instructions on using specific features.
## Documentation
For detailed information on using the House Construction Quotation System, refer to the documentation included with the repository. The documentation covers installation, configuration, and usage guidelines.

## Support and Issues
1. For any issues, bugs, or questions, please create an issue on the GitHub repository. Our team will respond promptly to address your concerns.

License
This House Construction Quotation System is licensed under the MIT License. Feel free to use, modify, and distribute the system according to the terms of this license.

## Acknowledgements
We would like to acknowledge the following individuals and projects for their contributions and inspiration to the House Construction Quotation System:

### BACK END
- [duong-hong-quan](https://github.com/duong-hong-quan)
- [KhangPhamBM](https://github.com/KhangPhamBM)

### FRONT END
- [chau-ngoc-tram](https://github.com/ChauNgocTram)


#### Copyright &#169; 2024 - Dương Hồng Quân & Phạm Bùi Minh Khang
