French verb conjugation using modern .NET and AI

## 🚀 Features

- **AI-Powered Conjugations**: Leverages various AI providers through Semantic Kernel
- **Flexible AI Provider**: Supports OpenRouter, OpenAI, Ollama, and other compatible endpoints
- **Rate Limiting**: Built-in protection against API abuse
- **Docker Support**: Containerized deployment with security best practices

## 🛠️ Technology Stack

- **.NET 9** with C# 12
- **ASP.NET Core** Minimal APIs
- **Semantic Kernel** for AI orchestration
- **Docker** with Alpine Linux for lightweight containers

## 📦 Installation

### Local Development

```sh
git clone https://github.com/yourusername/conj-ai.git
cd conj-ai
dotnet restore
```

Using Docker

```sh
docker build -t conj-ai .
docker run -p 8080:8080 -e OpenAI__ApiKey="key" conj-ai
```

## ⚙️ Configuration

### Dev Setup

- Use .NET user secrets for secure local development and set your key:

```sh
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "sk-or-v1-your-key"
```

- Alternative: `appsettings.Development.json`

```json
{
  "OpenAI": {
    "ApiKey": "sk-or-v1-your-key"
  }
}
```

> ⚠️ Never commit API keys to version control

## 🤖 Supported AI Providers

- OpenRouter:

```json
{
  "OpenAI": {
    "Endpoint": "https://openrouter.ai/api/v1"
  }
}
```

- OpenAI:

```json
{
  "OpenAI": {
    "Endpoint": "https://api.openai.com/v1"
  }
}
```

- Ollama:

```json
{
  "OpenAI": {
    "Endpoint": "http://localhost:11434/v1"
  }
}
```

- Open WebUI:

```json
{
  "OpenAI": {
    "Endpoint": "http://localhost:8080/ollama/api"
  }
}
```

## 🐳 Docker Deployment

### Docker Compose

Create a .env file (ignored by Git):

```env
OPENAI_MODEL=meta-llama/llama-3.2-3b-instruct:free
OPENAI_ENDPOINT=https://openrouter.ai/api/v1
OPENAI_API_KEY=<your-api-key>
```

Then run:

```sh
docker-compose up -d
```

> ⚠️ For production, use proper secret management

## 🚦 Usage

- Example Request:

```sh
curl http://localhost:8080/parler
```

- Example Response:

```json
{
  "infinitive": "parler",
  "present": {
    "je": "parle",
    "tu": "parles",
    "il": "parle",
    "nous": "parlons",
    "vous": "parlez",
    "ils": "parlent"
  },
  "passeCompose": {
    "je": "ai parlé",
    "tu": "as parlé",
    ...
  }
}
```
