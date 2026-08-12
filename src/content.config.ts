import { glob, file } from "astro/loaders";
import { defineCollection, reference } from "astro:content";
import { z } from "astro/zod";
import { bibLoader } from "./lib/bibParser";

const RoleList = ["Professor", "PostDoc", "PhD Student", "Scientific Assistant", "Master Student", "Bachelor Student", "Team Assistant", "Alumni"] as const;

// Define a `loader` and `schema` for each collection
const team = defineCollection({
    loader: glob({ pattern: '**/[^_]*.md', base: "./src/content/team" }),
    schema: ({image}) => z.object({
      name: z.string(),
      img: image().optional(),
      role: z.enum(RoleList),
      socials: z.object({
        github: z.string().optional(),
        orcid: z.string().optional(),
        twitter: z.string().optional(),
        linkedin: z.string().optional(),
      }).optional(),
      email: z.string().optional(),
      phone: z.string().optional(),
      alumni: z.array(z.object({
        year: z.number(),
        role: z.enum(RoleList.filter(role => role !== "Alumni") as (typeof RoleList)[number][])
      })).optional()
    })
});

const publications = defineCollection({
  loader: bibLoader({ base: "./src/content/publications" }),
  schema: () => z.object({
    abstract: z.string().optional(),
    author: z.string(),
    title: z.string(),
    year: z.number(),
    doi: z.string().optional(),
  })
})

// Export a single `collections` object to register your collection(s)
export const collections = { team, publications };