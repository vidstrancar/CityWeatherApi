# Cities Temperature Statistics API

## Getting Started

1. **Clone the repository.**
2. **Run the application:**
   ```bash
   dotnet run --project IndigoApi2
   ```
3. **Access the API Reference:** Navigate to `http://localhost:5269/scalar/` in your browser (only available in Development mode).

If you are unable to connect, check `IndigoApi2/Properties/launchSettings.json` to verify the configured ports for your local environment.

## API Endpoints

All endpoints (except documentation) require the `ApiKey` header by default.

- **`GET /api/Weather`**: Retrieve all processed city weather statistics.
- **`GET /api/Weather/{city}`**: Retrieve statistics for a specific city.
  - **Note:** The city name is case-sensitive and must start with a capital letter (e.g., `Zagreb`).
- **`GET /api/Weather/filter?minAvg=X&maxAvg=Y`**: Filter cities by their average temperature.
- **`POST /api/Weather/recalculate`**: Manually trigger a full re-processing of the CSV file.
- **`POST /api/Weather/newWeatherEntry`**: Add a new measurement entry.
  - **Body Format (JSON):**
    ```json
    {
      "city": "Zagreb",
      "temperature": 22.5,
      "date": "2026-04-01T12:00:00"
    }
    ```
  - **Date Format:** Expected in ISO 8601 format (e.g., `YYYY-MM-DD` or `YYYY-MM-DDTHH:mm:ss`).

## Configuration

The application is configured via `appsettings.json`. Key settings include:

```json
{
  "Authentication": {
    "ApiKey": "YOUR_API_KEY"
  },
  "WeatherSettings": {
    "MeasurementFilePath": "Data/measurements.csv"
  }
}
```

- `Authentication:ApiKey`: The key required in the `ApiKey` request header for all API calls.
- `WeatherSettings:MeasurementFilePath`: The path to the CSV file containing weather measurements.

## Security

To access the API when middleware is active, include the following header in your requests:

| Header | Description |
| :--- | :--- |
| `ApiKey` | Your configured API secret key. |

The default API key for development is: `123`

### Convenience (Testing)

If you are testing locally and want to avoid providing the `ApiKey` header with every request, you can temporarily disable the security middleware in `Program.cs`.

Comment out the following line:

```csharp
// app.UseMiddleware<ApiKeyMiddleware>();
```

**Note:** Ensure this is uncommented before deploying to any shared or production environment.

## Project Structure

- `IndigoApi2/Controllers`: RESTful endpoints for weather data.
- `IndigoApi2/Services`: Business logic for data processing and caching.
- `IndigoApi2/Middleware`: Custom middleware for authentication.
- `IndigoApi2/DTOs`: Data Transfer Objects for API requests/responses.
- `IndigoApi2/Configuration`: Strongly-typed configuration options.
- `IndigoApi2/Data`: Default directory for measurement CSV files.

## Technology Stack

- **Framework:** .NET 10.0
- **Language:** C#
- **API Documentation:** Microsoft.AspNetCore.OpenApi & Scalar.AspNetCore
- **Storage:** CSV (Flat-file data)
