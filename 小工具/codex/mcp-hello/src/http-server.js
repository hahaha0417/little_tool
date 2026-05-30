import { randomUUID } from "node:crypto";
import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { StreamableHTTPServerTransport } from "@modelcontextprotocol/sdk/server/streamableHttp.js";
import { createMcpExpressApp } from "@modelcontextprotocol/sdk/server/express.js";
import { isInitializeRequest } from "@modelcontextprotocol/sdk/types.js";
import { z } from "zod";
import http from "http";

const server = new McpServer({
  name: "mcp-hello-http",
  version: "1.0.0",
});

server.registerTool(
  "hello",
  {
    title: "Hello",
    description: "Return a simple greeting",
    inputSchema: {
      name: z.string().optional(),
    },
  },
  async ({ name = "world" }) => ({
    content: [
      {
        type: "text",
        text: `Hello, ${name}!`,
      },
    ],
  })
);

const app = createMcpExpressApp();
const transports = new Map();

// 🔥 重要：拿到 http server instance
let httpServer;

// =======================
// MCP ROUTES
// =======================

app.post("/mcp", async (req, res) => {
  const sessionId = req.headers["mcp-session-id"];
  let transport = sessionId ? transports.get(sessionId) : null;

  if (!transport) {
    if (!isInitializeRequest(req.body)) {
      res.status(400).json({
        jsonrpc: "2.0",
        error: { code: -32000, message: "Bad Request: No valid session ID provided" },
        id: null,
      });
      return;
    }

    transport = new StreamableHTTPServerTransport({
      sessionIdGenerator: () => randomUUID(),
      onsessioninitialized: (newSessionId) => {
        transports.set(newSessionId, transport);
      },
    });

    transport.onclose = () => {
      const sid = transport.sessionId;
      if (sid) transports.delete(sid);
    };

    await server.connect(transport);
  }

  await transport.handleRequest(req, res, req.body);
});

app.get("/mcp", async (req, res) => {
  const sessionId = req.headers["mcp-session-id"];
  const transport = sessionId ? transports.get(sessionId) : null;

  if (!transport) {
    res.status(400).send("Invalid or missing session ID");
    return;
  }

  await transport.handleRequest(req, res);
});

app.delete("/mcp", async (req, res) => {
  const sessionId = req.headers["mcp-session-id"];
  const transport = sessionId ? transports.get(sessionId) : null;

  if (!transport) {
    res.status(400).send("Invalid or missing session ID");
    return;
  }

  await transport.handleRequest(req, res);
});

// =======================
// START SERVER
// =======================

const port = process.env.MCP_PORT ? Number.parseInt(process.env.MCP_PORT, 10) : 3000;
const host = process.env.MCP_HOST || "127.0.0.1";

httpServer = app.listen(port, host, () => {
  console.log(`MCP HTTP server running at http://${host}:${port}/mcp`);
});

// =======================
// SHUTDOWN LOGIC
// =======================

let shuttingDown = false;

async function shutdown(signal) {
  if (shuttingDown) return;
  shuttingDown = true;

  console.log(`\n[Shutdown] Signal: ${signal}`);

  try {
    // 1. 關 transports（MCP sessions）
    for (const [id, transport] of transports) {
      try {
        await transport.close?.();
      } catch {}
      transports.delete(id);
    }

    // 2. 關 MCP server
    try {
      await server.close?.();
    } catch {}

    // 3. 關 HTTP server（Express）
    await new Promise((resolve) => {
      if (!httpServer) return resolve();
      httpServer.close(() => resolve());
    });

    // 4. 強制清理 socket（避免 Windows hang）
    try {
      process.stdin.destroy();
      process.stdout.end();
      process.stderr.end();
    } catch {}

    console.log("[Shutdown] completed");
  } finally {
    // 5. fallback 強制退出（避免卡死）
    setTimeout(() => process.exit(0), 200);
  }
}

// =======================
// SIGNAL HANDLERS
// =======================

process.on("SIGINT", () => shutdown("SIGINT"));
process.on("SIGTERM", () => shutdown("SIGTERM"));
process.on("SIGBREAK", () => shutdown("SIGBREAK")); // Windows Ctrl+Break

process.on("uncaughtException", (err) => {
  console.error("uncaughtException", err);
  shutdown("uncaughtException");
});

process.on("unhandledRejection", (err) => {
  console.error("unhandledRejection", err);
  shutdown("unhandledRejection");
});