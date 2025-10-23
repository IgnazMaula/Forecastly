<img width="848" height="237" alt="logo-text" src="https://github.com/user-attachments/assets/1c786238-d665-491a-a3ea-dcfeeb6af4c2" />

# Forecastly Weather App 🌤️
Forecastly is a sleek, interactive web application that lets users explore real-time weather conditions across the globe. Simply select a country and city to instantly view detailed weather information, including temperature (°C & °F), wind speed and direction, visibility, sky conditions, dew point, humidity, and pressure.

Designed with a modern **.NET Core Web API** and **Blazor WebAssembly**, Forecastly demonstrates seamless integration of data services and user-friendly interfaces. Ideal for anyone who wants accurate, accessible, and up-to-date weather insights with minimal effort.


## Features

-   **Country selection:** Fetches countries from backend (mocked or seeded).
    
-   **City selection:** Fetches cities for selected country dynamically.
    
-   **Weather details:** Displays:
    
    -   Location (City, Country)
        
    -   UTC time
        
    -   Wind speed & direction
        
    -   Visibility
        
    -   Sky conditions
        
    -   Temperature (°F and °C)
        
    -   Dew Point
        
    -   Relative Humidity
        
    -   Pressure
        
-   **Mocked API & live API support:** Can work offline with mocked responses.
    
-   **Unit tested:** Covers temperature conversion, service calls, and controller endpoints.

## Live Demo / Hosted Application

- **Frontend (Blazor Web App):** [https://forecastly.azurewebsites.net](https://forecastly.azurewebsites.net)  
- **Backend API (Swagger):** [https://forecastly-api.azurewebsites.net/swagger/index.html](https://forecastly-api.azurewebsites.net/swagger/index.html)

## Screenshots
<img width="1920" height="1147" alt="screencapture-localhost-5002-2025-10-24-03_20_10" src="https://github.com/user-attachments/assets/aa653957-acbb-484f-a777-b2b9caab5698" />
<img width="1920" height="1147" alt="screencapture-localhost-5002-2025-10-24-03_24_46" src="https://github.com/user-attachments/assets/ea96146b-8ec4-41d0-8cdb-b8fd1b86fb7d" />
<img width="1920" height="1147" alt="screencapture-localhost-5002-2025-10-24-03_26_48" src="https://github.com/user-attachments/assets/5016c493-fca8-4150-a9b0-3707abb8d390" />




## Technology Stack

|Layer| Technology | Purpose
|--|--|--|
| Back-end | .NET Core 8 Web API (Latest LTS) |Provides RESTful endpoints
| Front-end | Blazor WebAssembly (.NET 8) |Single-page application for interactive UI
| Testing | xUnit + Moq |Unit testing for services and controllers
| API| OpenWeatherMap + CountriesNow |Fetches coutries, cities, and live weather data


## Architecture & Design

-   **Backend**

    -   `CountriesController` exposes `/api/countries/` and `/api/countries/{CountryCode}/cities`  endpoints.
    
    -   `WeatherController` exposes `/api/weather/{cityName}` endpoint.
        
    -   `WeatherService` implements `IWeatherService`, handles API calls, temperature conversion, and dew point calculation.
        
    -   Mocked responses for offline testing.
        
-   **Frontend**
    
    -   Blazor components: `Country Dropdown`, `City Dropdown`, `Weather Card`.
    -   Type-ahead search on country and city dropdown enables users to quickly find and select a country or city by typing a few letters  
        
-   **Testing**
    
    -   `WeatherServiceTests`: Validates temperature conversion, success & failure HTTP calls.
        
    -   `WeatherControllerTests`: Validates controller endpoints with mocked service.
        
    -   All tests are offline-friendly.

## Setup & Installation

### Prerequisites

-   .NET 8 SDK
    
    
-   Visual Studio 2022 / VS Code
    

### Steps

1.  **Clone the repository**
    

`git clone https://github.com/IgnazMaula/Forecastly.git` 


2.  **Open the solution**
    

-   Open `Forecastly.sln` in **Visual Studio 2022** (or VS Code).
    

3.  **Run the backend API**
    

-   Set `Forecastly.Api` as the startup project.
    
-   Press **F5** or click **Run**.
    
-   API will run at `https://localhost:5001`.
    

4.  **Run the Blazor frontend**
    

-   Set `Forecastly.Web` as the startup project.
    
-   Press **F5** or click **Run**.
    
-   App will open at `https://localhost:5002`.
    

5.  **Access the application**
    

-   Navigate to `https://localhost:5002` in your browser.
    
-   Select a **country → city → view weather**.

## Testing

1.  Navigate to test project:
    

`cd Forecastly.Api.Tests` 

2.  Run tests:
    

`dotnet test` 

-   Includes tests for:
    
    -   Temperature conversion (F → C)
        
    -   Dew point calculation
        
    -   Service API calls (success & failure, mocked)
        
    -   Controller endpoints with mocked services
        
	-   All tests run offline and must pass.
