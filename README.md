# Ambev Developer Evaluation

A .NET 8 Web API project for managing sales, developed with **Domain-Driven Design (DDD)** and **Hexagonal Architecture**.  
It demonstrates the use of **CQRS**, **EF Core with PostgreSQL**, **MediatR**, and a clean separation of concerns between core business logic and infrastructure.

---

## 📚 Table of Contents

- [Getting Started](#getting-started)
- [Project Architecture](#project-architecture)
- [Setup and Execution](#setup-and-execution)
- [Usage Examples](#api-usage-examples)
- [Running Tests](#testing)
- [Business Rules](#business-rules)

---

<a id="getting-started"></a>
## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker & Docker Compose](https://docs.docker.com/get-docker/)

---

<a id="project-architecture"></a>
## 🏗 Project Architecture

This project follows a **Hexagonal (Ports & Adapters)** and **DDD** structure:

- **Domain** (`Ambev.DeveloperEvaluation.Domain`)  
  Entities, value objects, domain rules, events, and interfaces (`ports`) for external dependencies.

- **Application** (`Ambev.DeveloperEvaluation.Application`)  
  Command and query handlers (CQRS) using **MediatR**, plus application-specific use cases.

- **Infrastructure (Driven Adapters)**  
  - `Ambev.DeveloperEvaluation.ORM`: EF Core repositories, mappings, and DbContext
  - PostgreSQL as persistence layer

- **API (Driver Adapter)**  
  - `Ambev.DeveloperEvaluation.WebApi`: REST endpoints, input/output DTOs, and Swagger UI.

- **Cross-Cutting**  
  - `Ambev.DeveloperEvaluation.IoC`: Dependency injection setup  
  - `Ambev.DeveloperEvaluation.Common`: Utility classes (e.g., token generation, logging)

---

<a id="setup-and-execution"></a>

## 🛠️ Setup and Execution

### 1. **Clone repository**

```bash
   git clone https://github.com/ronaldeived/SalesChallenge.git
   cd SalesChallenge
```

### 2. **Start infrastructure**
```bash
    docker compose up -d
```

### 3. **Apply database migrations**
```bash
    dotnet tool install --global dotnet-ef
    dotnet ef database update -p src/Ambev.DeveloperEvaluation.ORM -s src/Ambev.DeveloperEvaluation.WebApi

```

### 4. **Run the Web API**
```bash
    dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

<a id="api-usage-examples"></a>

## 📖 API Usage Examples
### **Post - Create Sale -  /api/sales**
Request body
```json
{
  "number": "S-1001",
  "date": "2025-09-24T10:00:00Z",
  "customerId": "11111111-1111-1111-1111-111111111111",
  "customerName": "John Doe",
  "branchId": "22222222-2222-2222-2222-222222222222",
  "branchName": "Main Branch",
  "items": [
    {
      "productId": "33333333-3333-3333-3333-333333333333",
      "productName": "Product A",
      "quantity": 5,
      "unitPrice": 10.0
    }
  ]
}

```

Response 201 Created:
```json
{
  "id": "a1141515-31ef-4504-9803-d5c4d6ca6c63",
  "number": "S-1001",
  "date": "2025-09-24T10:00:00Z",
  "total": 45
}
```

### **Get - Sale by Id - /api/sales/{id}**

Response 200 Ok:
```json
{
  "data": {
    "id": "a1141515-31ef-4504-9803-d5c4d6ca6c63",
    "number": "S-1001",
    "date": "2025-09-24T10:00:00Z",
    "customerId": "11111111-1111-1111-1111-111111111111",
    "customerName": "John Doe",
    "branchId": "22222222-2222-2222-2222-222222222222",
    "branchName": "Main Branch",
    "isCancelled": false,
    "total": 45,
    "items": [
      {
        "id": "b902ca71-9207-47dc-9349-4fcd406cf223",
        "productId": "33333333-3333-3333-3333-333333333333",
        "productName": "Product A",
        "quantity": 5,
        "unitPrice": 10,
        "discountPercent": 10,
        "total": 45
      }
    ]
  },
  "success": true,
  "message": "",
  "errors": []
}
```

### **Get - List All Sales - /api/sales**

Response 200 Ok:
```json
{
  "data": [
    {
      "id": "a1141515-31ef-4504-9803-d5c4d6ca6c63",
      "number": "S-1001",
      "date": "2025-09-24T10:00:00Z",
      "customerName": "John Doe",
      "branchName": "Main Branch",
      "isCancelled": false,
      "total": 45
    }
  ],
  "success": true,
  "message": "",
  "errors": []
}

```

### **Put - Update a Sale - /api/sales/{id}**

Request body
```json
{
  "id": "a1141515-31ef-4504-9803-d5c4d6ca6c63",
  "number": "S-1001-Updated",
  "date": "2025-09-24T12:00:00Z",
  "customerId": "11111111-1111-1111-1111-111111111111",
  "customerName": "John Doe Updated",
  "branchId": "22222222-2222-2222-2222-222222222222",
  "branchName": "Updated Branch",
  "items": [
    {
      "id": "55555555-5555-5555-5555-555555555555",
      "productId": "33333333-3333-3333-3333-333333333333",
      "productName": "Product A",
      "quantity": 10,
      "unitPrice": 10.0
    }
  ]
}

```

Response 200 Ok:
```json
{
  "data": {
    "id": "a1141515-31ef-4504-9803-d5c4d6ca6c63",
    "number": "S-1001-Updated",
    "date": "2025-09-24T12:00:00Z",
    "customerId": "11111111-1111-1111-1111-111111111111",
    "customerName": "John Doe Updated",
    "branchId": "22222222-2222-2222-2222-222222222222",
    "branchName": "Updated Branch",
    "isCancelled": false,
    "total": 80,
    "items": [
      {
        "id": "b413ed68-f7cf-43f0-8195-2e584a9e099b",
        "productId": "33333333-3333-3333-3333-333333333333",
        "productName": "Product A",
        "quantity": 10,
        "unitPrice": 10,
        "discountPercent": 20,
        "total": 80
      }
    ]
  },
  "success": true,
  "message": "",
  "errors": []
}

```

### **Put - Cancel a Sale - /api/sales/cancel/{id}**

Request body

Response 200 Ok:
```json
{
  "id": "44444444-4444-4444-4444-444444444444",
  "number": "S-1001-Updated",
  "date": "2025-09-24T12:00:00Z",
  "customerId": "11111111-1111-1111-1111-111111111111",
  "customerName": "John Doe Updated",
  "branchId": "22222222-2222-2222-2222-222222222222",
  "branchName": "Updated Branch",
  "isCancelled": true,
  "total": 80.0,
  "items": [
    {
      "id": "55555555-5555-5555-5555-555555555555",
      "productId": "33333333-3333-3333-3333-333333333333",
      "productName": "Product A",
      "quantity": 10,
      "unitPrice": 10.0,
      "discountPercent": 0.2,
      "total": 80.0
    }
  ]
}

```

### **Delete - Delete a Sale - /api/sales/{id}**
Response 204 No Content

---

<a id="testing"></a>

## 🧪 Testing
#### **Run the tests with**
```bash
dotnet test
```
---

<a id="business-rules"></a>

## Business rules
For each product:

- less than 4 units → no discount

- 4 to 9 units → 10% discount

- 10 to 20 units → 20% discount

- more than 20 units → not allowed (exception)

- Sales can be cancelled → after cancellation no more modifications