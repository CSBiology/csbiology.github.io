import { readFileSync, readdirSync } from "node:fs";
import { resolve } from "node:path";
import { fileURLToPath } from "node:url";
import type { Loader } from "astro/loaders";

interface BibtexEntry {
  type: string;
  key: string;
  fields: Record<string, string>;
}

function parseBibtexEntry(content: string): BibtexEntry | null {
  const entryMatch = content.match(/@(\w+)\s*\{\s*([^,\s]+)\s*,/s);
  if (!entryMatch) return null;

  const [, type, key] = entryMatch;
  const fields: Record<string, string> = {};

  const bodyStart = entryMatch.index! + entryMatch[0].length;
  let pos = bodyStart;

  while (pos < content.length) {
    while (pos < content.length && /[\s,]/.test(content[pos])) pos++;
    if (pos >= content.length || content[pos] === "}") break;

    const fieldRegex = /^(\w+)\s*=\s*/;
    const fieldMatch = content.slice(pos).match(fieldRegex);
    if (!fieldMatch) break;

    const fieldName = fieldMatch[1].toLowerCase();
    pos += fieldMatch[0].length;

    if (content[pos] === "{") {
      let depth = 1;
      const valueStart = pos + 1;
      pos = valueStart;
      while (pos < content.length && depth > 0) {
        if (content[pos] === "{") depth++;
        else if (content[pos] === "}") depth--;
        pos++;
      }
      fields[fieldName] = content.slice(valueStart, pos - 1).trim();
    } else if (content[pos] === '"') {
      const valueStart = pos + 1;
      pos = valueStart;
      while (pos < content.length && content[pos] !== '"') {
        if (content[pos] === "\\") pos++;
        pos++;
      }
      fields[fieldName] = content.slice(valueStart, pos).trim();
      pos++;
    } else {
      break;
    }
  }

  return { type, key, fields };
}

function cleanDoi(doi: string): string {
  return doi.replace(/^doi:\s*/i, "");
}

export function bibLoader(options: { base: string }): Loader {
  return {
    name: "bib-loader",
    load: async ({ store, parseData, logger, config }) => {
      const dirPath = fileURLToPath(new URL(options.base, config.root));
      const files = readdirSync(dirPath).filter((f) => f.endsWith(".bib"));

      for (const filename of files) {
        const content = readFileSync(resolve(dirPath, filename), "utf-8");
        const entry = parseBibtexEntry(content);

        if (!entry) {
          logger.warn(`Failed to parse BibTeX entry: ${filename}`);
          continue;
        }

        const id = filename.replace(/\.bib$/, "");
        const year = parseInt(entry.fields.year, 10);

        const rawData: Record<string, unknown> = {};
        for (const [key, value] of Object.entries(entry.fields)) {
          if (key === "year") {
            rawData[key] = Number.isNaN(year) ? 0 : year;
          } else if (key === "doi") {
            rawData[key] = cleanDoi(value);
          } else {
            rawData[key] = value;
          }
        }

        try {
          const data = await parseData({ id, data: rawData });
          store.set({ id, data });
        } catch (err) {
          logger.error(
            `Schema validation failed for ${filename}: ${err instanceof Error ? err.message : String(err)}`,
          );
        }
      }
    },
  };
}
