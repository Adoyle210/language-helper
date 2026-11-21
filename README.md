# Language Helper

A language-learning helper API. Save a word you meet in a video caption, keep the sentence it appeared in, and review it later as a spaced-repetition flashcard. A small language model will explain each word in context.

**Status:** early development. The core words API is in progress.

## Tech stack

- C# / ASP.NET Core minimal API
- Entity Framework Core (in-memory database for now, SQLite planned)
- Swagger UI for trying endpoints
- Planned: xUnit tests, GitHub Actions CI, a local language model through Ollama, Docker Compose, and a YouTube browser extension

## Running locally

```bash
dotnet run
```

Then open `/swagger` on the local address shown in the terminal to try the endpoints.

## Endpoints

| Method | Route | Purpose | Status |
|---|---|---|---|
| POST | /words | Save a word with translation, language and sentence. Creates its flashcard and a capture; an existing word just gets a new capture. | Done |
| GET | /words | List words (filters for language and starred coming) | Done|
| GET | /words/{id} | One word with its card and captures | Done |
| PUT | /words/{id} | Update a word's translation or starred flag | Done |
| DELETE | /words/{id} | Delete a word along with its card and captures | Done |
| GET | /cards/due | Cards due for review | To Do |
| POST | /cards/{id}/review | Submit a review score (0-5) and reschedule the card | To Do |
| GET | /stats | Totals, cards due today, and core words met | To Do |
| POST | /explain | Ask the language model to explain a word in its sentence context | To Do |

## Roadmap

1. **Core API and flashcards:** words, cards and captures, spaced-repetition scheduling (SM-2), tests and CI.
2. **AI and Docker:** a local model behind an interface, an evaluation set, and a Docker Compose setup.
3. **YouTube extension:** click a caption word to save it.
4. **Polish:** core-word progress, demo video, and a short write-up.