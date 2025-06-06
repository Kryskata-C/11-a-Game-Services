# Game Services Platform

This project is a web application for hiring professional gamers who are students at 11-A in Softuni Buditel. The platform allows users to browse through a catalog of available players and teams, view their profiles, and hire them for gaming services.

## Features

Based on the project structure, the application includes the following features:

* **User Authentication:** Secure registration and login system for users, powered by ASP.NET Core Identity.
* **Player Catalog:** A comprehensive list of all available players. Users can view detailed player profiles, including their skills, ratings, and hiring price.
* **Team Catalog:** Users can browse and hire entire pre-made teams for cooperative gaming.
* **Shopping Cart System:** Users can add multiple players or teams to a shopping basket and proceed to a checkout.
* **Ordering and Hiring:** A complete order management system where users can finalize the hiring process and view their order history.
* **Player and Team Reviews:** After a service is complete, users can leave reviews and ratings for the players they've hired.
* **User Profiles:** Registered users have a personal profile where they can manage their information and deposit funds into their account balance.
* **Admin Panel:** A dedicated administrative area for managing players, teams, orders, and all other aspects of the platform.

## Technologies Used

* **Backend:** C#, ASP.NET Core MVC
* **Database:** Microsoft SQL Server with Entity Framework Core
* **Authentication:** ASP.NET Core Identity
* **Frontend:** Razor Pages, HTML, CSS, JavaScript
* **Libraries:** Bootstrap, jQuery

## Getting Started

To get a local copy up and running, follow these simple steps.

### Prerequisites

* .NET 8 SDK
* An IDE like Visual Studio 2022 or VS Code
* SQL Server (Express, Developer, or other editions)

### Installation

1.  **Clone the repository:**
    ```sh
    git clone <your-repository-url>
    ```
2.  **Configure the database connection:**
    * Open the `appsettings.json` file inside the `WebApplication1` directory.
    * Modify the `DefaultConnection` string to point to your local SQL Server instance. It should look something like this:
        ```json
        "ConnectionStrings": {
          "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=GameServiceDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
        }
        ```
3.  **Apply Database Migrations:**
    * Open a terminal or command prompt in the `WebApplication1` directory.
    * Run the following command to create the database and apply the schema:
        ```sh
        dotnet ef database update
        ```
4.  **Run the application:**
    * You can run the project from your IDE (e.g., by pressing F5 in Visual Studio) or by using the following command in the terminal:
        ```sh
        dotnet run
        ```
    * The application will be available at `https://localhost:XXXX` and `http://localhost:YYYY`, where the ports are specified in the `Properties/launchSettings.json` file.

## Usage

Once the application is running, you can register as a new user to start Browse and hiring players.

### Administrator Access

The application is pre-configured with a default administrator account to manage the site.
* **Email:** `admin@gmail.com`
* **Password:** `Admin1`

Log in with these credentials to access the Admin Panel and manage the application's data.
