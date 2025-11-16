# Wizard Assessment Project

This is a simple project for a wizard-style assessment. It uses a rule engine to decide what is the next question and recommend a bucket according to the answers.

## How to run this thing

You have two options to run the project:

### Running with Docker (Recommended)

This is the easiest way to get the entire application running.

1.  Make sure you have Docker Desktop installed and running on your machine.
2.  Open a terminal in the root of the project.
3.  Run the following command:
    ```bash
    docker-compose -f container/docker-compose.yml up --build
    ```

**What to expect:**

The startup process takes about 30-40 seconds. You'll see the following stages:

1.  **Building the API** - Docker will build the .NET application image (first run only).
2.  **Starting SQL Server** - The database container starts up.
3.  **Initializing Database** - You'll see a message saying "Initializing Database..."
4.  **Database Ready** - Look for the message: **"✓ DATABASE READY!"** with a confirmation that all tables and data are initialized.
5.  **API Starting** - After the database is ready, the API container will start automatically.

Once you see the "✓ DATABASE READY!" message and the API logs show it's listening, the application is fully ready to use.

The database will be accessible at `localhost:1433`, and the API will be running at `http://localhost:5173`.

You can then go to `http://localhost:5173/swagger/index.html` to see the Swagger UI and interact with the API.

**To stop the application:**
-   Press `Ctrl+C` in the terminal.
-   Run `docker-compose -f container/docker-compose.yml down` to remove the containers.

### Manual Setup

If you prefer not to use Docker, you can set up the project manually.

You need two things:
1. .NET 9
2. Microsoft SQL Server 2022

#### DB setup
All the relevant DB files are in `src/DBScripts`.
You need to run them in this order:
1. `Schema.sql` - to make the tables.
2. `system-data.sql` - This file has all the data the wizard needs to run. It has the questions, the possible answers, and the navigation rules that control the flow. It also defines the final "buckets" (Lite, Medium, Premium).
3. `demo-data.sql` - This is demo data for you to test with. It creates a few organizations, including one that has several environments, so you can see how the wizard works differently for different organizations.

Before you run the project, you need to change the connection string in `src/WizardAssessment.API/appsettings.json` and update the UserID, Password (IP and port if relevant). This is how it looks now:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=McWizardDB;User Id=sa;Password=Hila1234;TrustServerCertificate=True;"
}
```
You change to your user and password for SQL server.

#### Project run
Open terminal in `src/WizardAssessment.API` and run this:

```bash
dotnet run
```

Then you can go to `http://localhost:5173/swagger/index.html` to see the Swagger.

---

## Testing the API

A `requests.http` file is included in the project root with pre-configured request examples for different wizard flows:
- **Flow 1**: Regular flow for a small startup (simple path, no environment questions)
- **Flow 2**: Environment selection flow (organization HAS existing environments - selects from list)
- **Flow 3**: Environment flow (organization does NOT have environments - selects from predefined ranges)
- **Flow 4**: Coverage flow for medium organizations

To use these requests:
1. Open `requests.http` in VS Code with the REST Client extension installed
2. Click "Send Request" above any request
3. Copy the `sessionId` from the response and replace `YOUR_SESSION_ID_HERE` in subsequent requests

You can also use the Swagger UI at `http://localhost:5173/swagger/index.html` to test the API interactively.

---

## Code Structure

I have a few projects in this solution.

-   `WizardAssessment.API`
    -   This is the web api project. It has the controllers. The "main" of the program.
-   `WizardAssessment.Application`
    -   Here is the main logic for the wizard. The `WizardService` is here.
-   `WizardAssessment.Domain`
    -   This is the core. It has all the business models, interfaces, and the rule engine logic.
-   `WizardAssessment.Infrastructure`
    -   All the database things are here. Repositories, DbContext and caching.
-   `WizardAssessment.Tests`
    -   Tests for the project.

---

## Database Structure

### Main Tables

**Organizations** - The organizations using the system
```sql
Id, Name, CreatedAt
```

**Environments** - Each organization's environments
```sql
Id, OrganizationId, Name, CreatedAt
```

**Questions** - All wizard questions
```sql
Code (PK), Text, Type
```

**QuestionOptions** - Static options for questions
```sql
Id, QuestionCode, Value, DisplayText, SortOrder
```

**NavigationRules** - Question Navigation Logic
```sql
RuleId, CurrentQuestionCode, AnswerValue, NextQuestionCode, 
RecommendedBucket, ConditionType, Priority
```
Rules are checked by priority (lower = higher priority). If a rule has a condition, it's only used if the condition passes. This way you can have fallback rules.

**BucketConfigurations** - Possible result buckets
```sql
BucketName (PK), MinEnvironments, MaxEnvironments
```

**WizardSessions** - Active/completed wizard sessions
```sql
Id (GUID), OrganizationId, CurrentQuestionCode, IsCompleted, 
RecommendedBucket, CreatedAt, CompletedAt
```

**SessionAnswers** - Log of all answers
```sql
Id, SessionId, QuestionCode, Answer, AnsweredAt
```

---

## The Logic

The flow logic of the wizard is determined by a rule engine that is data-driven according to rules that are stored in the DB.

The rules and other system configurations(questions, bucket recommendations and etc) are cached in-memory on startup to reduce DB queries for optimization.


The flow is like this:
1. You start the wizard for an organization.
2. The engine looks at the `NavigationRules` and finds the first question.
3. You send an answer.
4. The engine takes your answer, checks the rules again, and finds the next step.
5. The next step could be either a question or a final recommendation for a bucket.
6.  It continues until the wizard is complete with a recommendation.

This way, to change the wizard flow, you only change data in the `NavigationRules` table. No code change.

The only time you need code changes is for custom conditions (like HasEnvironments) or special navigators (like EnvSelection).

---

## API

The API is very simple.

| Method | Path                               | Description                |
| ------ | ---------------------------------- | -------------------------- |
| POST   | `/api/wizard/start`                | Starts a new wizard session. |
| POST   | `/api/wizard/sessions/{id}/answer` | Submits an answer.         |

### API Reference

### POST /api/wizard/start

Start a new wizard session.

**Request Body:**
```json
{
  "organizationId": number (required)
}
```

**Response:** WizardStepResponse (see below)

**Errors:**
- 404: Organization not found

---

### POST /api/wizard/sessions/{sessionId}/answer

Submit answer to current question.

**Path Parameters:**
- sessionId: GUID

**Request Body:**
```json
{
  "questionCode": string (required),
  "answer": string (required, comma-separated for multi-choice)
}
```

**Response:** WizardStepResponse (see below)

**Errors:**
- 400: Invalid answer, wrong question code, validation failed
- 404: Session not found, question not found
- 409: Session already completed

---

### WizardStepResponse Structure

```json
{
  "sessionId": "guid",
  "isCompleted": boolean,
  "question": {
    "questionCode": "string",
    "text": "string",
    "type": "SingleChoice|MultiChoice",
    "options": [
      {
        "value": "string",
        "displayText": "string"
      }
    ]
  } | null,
  "recommendedBucket": "Lite|Medium|Premium" | null
}
```

When `isCompleted` is true:
- question is null
- recommendedBucket has a value

When `isCompleted` is false:
- question is populated
- recommendedBucket is null

---

## What I did not do

This is a home assignment, so I didn't do everything. Here are things I thought about but are not in the code. 
-   **Scalability**:
    -   The code now runs on one server. For big scale, we can use a distributed cache like Redis instead of the in-memory cache I used for system data.
    -   We can put the application in containers (Docker) and run it on Kubernetes to scale out.
-   **Resilience and Stability**:
    -   I have a basic exception middleware. For a real system, I would add more logging, and maybe a retry policy for database calls (like with Polly).
    -   No monitoring. I would add health checks endpoints and use something like Prometheus and Grafana to see what's happening in the system.
-   **Database**:
    -   The DB schema is simple. For a very big system, maybe we need to optimize it, add indexes, or even use a different type of database for some parts.
-   **Rule Engine**:
    -   The engine is good, but for very complex rules, maybe we need something more powerful and sophisticated.
-   **Security**:
    -   There is no authentication or authorization. Of course, in a real application, this is a must.
-   **Testing**:
    -   I didn't implement integration tests and the testing coverage is not 100%.
    -   In a real world scenario these are a must.
-   **DB**:
    -   Data TTL is not implemented, in a real world application you would want to delete certain data after a TTL (e.g Active Sessions/Logs/etc...)
-  **Sessions**:
    -   Sessions can't be resumed, If you quit the browser there is no API way to retrieve the session ID nor an API to get the current question for a session.