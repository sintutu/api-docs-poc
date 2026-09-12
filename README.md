# OpenAPI-to-Apidog documentation POC

This proof of concept shows that a rich OpenAPI contract can be the single source of truth for useful API documentation. It uses synthetic data and intentionally has no server, database, credentials, integration, or CI workflow.

## Architecture decision

[`openapi.yaml`](./openapi.yaml) is the canonical API contract. It describes the API, documentation, examples, validation rules, and reusable models in one versioned file. Apidog imports that contract to present interactive documentation; it is not edited as a competing source of truth.

The focused domain is an Orders API with `GET /orders/{orderId}` and `POST /orders`. The reusable `Order`, `CreateOrderRequest`, `Money`, and `Problem` schemas ensure that nested data, money, and errors remain structured rather than being flattened into vague strings.

## Demonstrate the documentation update

1. Create an Apidog project and import [`examples/openapi-before.yaml`](./examples/openapi-before.yaml).
2. Inspect the `Order` schema and the two endpoints.
3. Re-import [`openapi.yaml`](./openapi.yaml), choosing the update/overwrite option for existing endpoints and schemas.
4. Confirm that `Order.status` now appears as a required field with its lifecycle description, four allowed values, and a `confirmed` example.

[`examples/openapi-after.yaml`](./examples/openapi-after.yaml) is the compact snapshot of that same meaningful change. `openapi.yaml` is richer and remains the source of truth for all future changes.

## What to verify in Apidog

- Both endpoints display their summaries, descriptions, parameters, request bodies, and success responses.
- `Money` and `OrderItem` expand as reusable object schemas wherever referenced.
- The request and response examples are visible and understandable without reading source code.
- `400`, `404`, and `409` responses expose reusable `Problem` details and concrete examples.
- Re-importing the current contract visibly updates the `Order` documentation with `status`.

## Later automation

The next step—not implemented here—is a CI job that validates `openapi.yaml` and imports it into the chosen Apidog project using securely stored credentials. It should report import failures and never make Apidog the primary copy of the contract.
