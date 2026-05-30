# MCP Hello

This project provides a minimal MCP server with one tool: `hello`.

## Tools

- `hello`
  - Input: `name` (optional)
  - Output: `Hello, <name>!`
  - Default name: `world`

## Install

```bash
cd mcp-hello
npm install
```

## Run

stdio transport:

```bash
npm start
```

HTTP transport:

```bash
npm run http
```

Or, for the existing alias:

```bash
npm run start:http
```

The HTTP server listens on:

```text
http://127.0.0.1:3000/mcp
```

You can override the port and host with:

- `MCP_PORT`
- `MCP_HOST`

Example:

```bash
$env:MCP_PORT = '3001'
$env:MCP_HOST = '127.0.0.1'
npm run start:http
```

## MCP client config

stdio example:

```json
{
  "mcpServers": {
    "mcp-hello": {
      "command": "node",
      "args": ["d:/desktop/codex/mcp-hello/src/server.js"]
    }
  }
}
```

HTTP example:

```json
{
  "mcpServers": {
    "mcp-hello-http": {
      "url": "http://127.0.0.1:3000/mcp"
    }
  }
}
```

If your client expects a command-based server, use one of these:

```json
{
  "mcpServers": {
    "mcp-hello": {
      "command": "npm",
      "args": ["run", "http"],
      "cwd": "d:/desktop/codex/mcp-hello"
    }
  }
}
```

If your client expects a URL-based server, point it at the HTTP endpoint:

```json
{
  "mcpServers": {
    "mcp-hello-http": {
      "url": "http://127.0.0.1:3000/mcp"
    }
  }
}
```

If you want to change the port or host, set:

- `MCP_PORT`
- `MCP_HOST`

Example:

```bash
$env:MCP_PORT = '3001'
$env:MCP_HOST = '127.0.0.1'
npm run http
```

## Call example

```json
{
  "name": "hello",
  "arguments": {
    "name": "Codex"
  }
}
```

Response:

```text
Hello, Codex!
```
