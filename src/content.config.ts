// Import the glob loader
import { glob } from "astro/loaders";
// Import utilities from `astro:content`
import { defineCollection } from "astro:content";
// Import Zod
import { z } from "astro/zod";

const RoleList = ["Professor", "PostDoc", "PhD Student", "Master Student", "Bachelor Student", "Team Assistant", "Alumni"] as const;

// Define a `loader` and `schema` for each collection
const team = defineCollection({
    loader: glob({ pattern: '**/[^_]*.md', base: "./src/content/team" }),
    schema: ({image}) => z.object({
      name: z.string(),
      img: image().optional(),
      role: z.enum(RoleList),
      github: z.string().optional(),
      orcid: z.string().optional(),
      email: z.string().optional(),
      alumni: z.array(z.object({
        year: z.number(),
        role: z.enum(RoleList.filter(role => role !== "Alumni") as (typeof RoleList)[number][])
      })).optional()
    })
});

// Export a single `collections` object to register your collection(s)
export const collections = { team };