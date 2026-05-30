import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import { z } from "zod";

const server = new McpServer({
  name: "mcp-hello",
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

const transport = new StdioServerTransport();
await server.connect(transport);
