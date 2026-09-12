# Orders API documentation laboratory

A deliberately small ASP.NET Core API for learning how C# models become OpenAPI documentation and how that documentation can be exercised manually in Apidog. It uses only synthetic, in-memory data—there is no database, authentication, Docker, deployment setup, or Apidog automation.

## Prerequisites and reproducibility

Install the .NET 10 SDK. This repository records SDK `10.0.400` in [`global.json`](./global.json); `dotnet --version` should report that version or a compatible patch. The SDK is machine software. NuGet dependencies are declared in the project files and downloaded automatically into your local NuGet cache by `dotnet restore`; do not install them globally.

## Run locally

```sh
dotnet restore ApiDocsPoc.slnx
dotnet run --project src/OrdersApi --urls http://127.0.0.1:5079
```

With the API running, use these URLs:

- API base: `http://127.0.0.1:5079`
- Swagger UI: `http://127.0.0.1:5079/swagger`
- OpenAPI document: `http://127.0.0.1:5079/swagger/v1/swagger.json`
- Seed order: `GET http://127.0.0.1:5079/orders/84f5e6b0-b2bd-4dc4-8ab1-5b105c0986c8`

Create an order:

```sh
curl -i -X POST http://127.0.0.1:5079/orders \
  -H 'Content-Type: application/json' \
  -d '{"customerId":"cus_newcustomer","items":[{"sku":"notebook-a5","name":"A5 Notebook","quantity":2,"unitPrice":{"amount":2499,"currency":"ZAR"}}]}'
```

Run the compact integration suite with `dotnet test ApiDocsPoc.slnx`.

## Architecture

The API is controller-based to keep HTTP behaviour, validation, and generated metadata easy to inspect. `OrderStore` is a singleton in-memory store seeded with one order. Data-annotation validation produces standard problem responses for invalid requests; unknown orders return `ProblemDetails` with HTTP 404.

`Order`, `OrderItem`, and `Money` are structured C# objects, while `OrderStatus` is an enum. Swagger is generated from the controllers, response metadata, validation attributes, and XML comments at runtime. Swashbuckle is the only third-party dependency because it supplies the requested Swagger UI; its version is declared in the API project.

The pre-existing root [`openapi.yaml`](./openapi.yaml) and the files in [`examples/`](./examples/) are static before/after documentation snapshots from the earlier spec-first experiment. For this C# phase, use the generated `/swagger/v1/swagger.json` document when testing the local API.

## Use with Apidog

1. Start the API using the command above and open the OpenAPI document URL locally to confirm it responds.
2. In Apidog, create or open a project, then choose **Import Data** → **OpenAPI/Swagger**.
3. Import from a URL using `http://127.0.0.1:5079/swagger/v1/swagger.json` (or upload a saved copy of that document).
4. Set the Apidog environment base URL to `http://127.0.0.1:5079` and send the seeded `GET` request or the sample `POST` request.

Apidog must run on the same Mac, or otherwise have network access to the Mac. A remote/cloud service cannot reach `127.0.0.1`; use Apidog’s desktop client/local agent or make the API reachable on your LAN only when you deliberately choose to do so. No Apidog credentials or automation are stored here.

## Later, deliberately excluded

CI validation, import automation, credentials, code generation, persistence, containerisation, and production deployment are deferred. The later automation path is to validate the generated OpenAPI document and import it using credentials held by CI—not to maintain a second documentation source.
