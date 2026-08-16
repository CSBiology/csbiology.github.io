# Implementation Plan: Journal Design System (Revised)

## 1. Objective

Build the foundational visual system for the website redesign around a **written-journal / field-notebook aesthetic**, using **Astro v7, Tailwind CSS v4, daisyUI v5**, with Astro-first components and React only if genuinely required. Deliverable: reusable primitives + a living showcase at `/dev/components`.

**Scope: purely additive.** No changes to `Layout.astro`, `Navbar.astro`, or `index.astro` in this project. daisyUI theme changes are scoped to `/dev/components` via `data-theme="journal"`.

## 2. Phase 1 — Audit results (already verified)

Current state — implementers do not need to re-audit unless versions drift:

| Item | Status |
|---|---|
| astro `^7.2.1` | OK, no update needed |
| tailwindcss `^4.3.3` + `@tailwindcss/vite` | OK, CSS-first already |
| daisyui `^5.7.16` | OK |
| `app.css` | Already `@import "tailwindcss"` + `@plugin "daisyui" { themes: silk --default, dark --prefersdark; }` — correct v5 pattern |
| react/react-dom `^19` + `@astrojs/react` | Installed but **unused by this plan**; no React components |
| tsconfig paths | `@assets/*`, `@components/*`, `@layouts/*`, `@lib/*` |
| Styles | No `src/styles/` yet; create it |
| Fonts | None configured — system stacks only (no new deps, no network fonts) |
| `astro check` | Not set up — add `@astrojs/check` + `typescript` as devDependencies |
| Sitemap | `@astrojs/sitemap` active — must exclude `/dev/components` |

## 3. Phase 2 — Design tokens (Tailwind v4 CSS-first)

Create:

```text
src/styles/
├── global.css      # app-wide resets/extensions (minimal)
├── journal.css     # @theme tokens + journal component CSS
└── textures.css    # paper grain, ruled lines, page edge
```

`src/assets/app.css` gains imports of `journal.css` and `textures.css` after `@import "tailwindcss"`.

### 3.1 Colors — `@theme` in `journal.css`

Tailwind v4 generates utilities (`bg-paper`, `text-ink`, `border-line`) from these:

```css
@theme {
  --color-paper:       oklch(97% 0.01 85);
  --color-paper-dark:  oklch(93% 0.015 85);
  --color-paper-muted: oklch(90% 0.01 85);
  --color-ink:         oklch(28% 0.03 60);
  --color-ink-muted:   oklch(45% 0.025 60);
  --color-accent:      oklch(55% 0.11 35);
  --color-accent-red:  oklch(60% 0.16 25);
  --color-accent-blue: oklch(50% 0.10 245);
  --color-accent-yellow: oklch(85% 0.14 95);
  --color-line:        oklch(80% 0.02 85);
}
```

Exact values are tunable; names are the contract.

### 3.2 Typography — system stacks only

```css
@theme {
  --font-body: Georgia, "Times New Roman", serif;
  --font-sans: system-ui, "Segoe UI", sans-serif;
  --font-hand: "Segoe Script", "Bradley Hand", "Comic Sans MS", cursive;
  --font-mono: ui-monospace, "Cascadia Code", Consolas, monospace;
}
```

`font-body` is default body copy; `font-hand` only for short handwritten annotations.

### 3.3 daisyUI alignment — custom `journal` theme

Register a custom theme (daisyUI v5 mechanism, per official docs) and enable it:

```css
@plugin "daisyui" {
  themes: silk --default, dark --prefersdark, journal;
}

@plugin "daisyui/theme" {
  name: "journal";
  color-scheme: light;
  --color-base-100: oklch(97% 0.01 85);   /* paper */
  --color-base-200: oklch(93% 0.015 85);   /* paper-dark */
  --color-base-300: oklch(90% 0.01 85);    /* paper-muted */
  --color-base-content: oklch(28% 0.03 60);/* ink */
  --color-primary: oklch(55% 0.11 35);     /* accent */
  --color-primary-content: oklch(97% 0.01 85);
  --color-secondary: oklch(50% 0.10 245);  /* accent-blue */
  --color-secondary-content: oklch(97% 0.01 85);
  --color-accent: oklch(85% 0.14 95);      /* accent-yellow */
  --color-accent-content: oklch(28% 0.03 60);
  --color-neutral: oklch(28% 0.03 60);
  --color-neutral-content: oklch(97% 0.01 85);
  --color-info: oklch(60% 0.08 220);
  --color-info-content: oklch(97% 0.01 85);
  --color-success: oklch(60% 0.12 145);
  --color-success-content: oklch(97% 0.01 85);
  --color-warning: oklch(80% 0.14 85);
  --color-warning-content: oklch(28% 0.03 60);
  --color-error: oklch(60% 0.16 25);
  --color-error-content: oklch(97% 0.01 85);
  --radius-selector: 0.25rem;
  --radius-field: 0.25rem;
  --radius-box: 0.25rem;
  --size-selector: 0.25rem;
  --size-field: 0.25rem;
  --border: 1px;
  --depth: 0;
  --noise: 0;
}
```

- `silk`/`dark` remain the site defaults (`<html data-theme="silk">` unchanged).
- `/dev/components` wraps its content in `data-theme="journal"` so daisyUI functional components (menu, badge, kbd) inherit the journal palette automatically.
- Journal primitives (Paper, Note, etc.) use journal tokens only — never daisyUI component classes.

### 3.4 `textures.css` — opt-in utilities

- `paper-grain`: layered CSS gradients (no image assets)
- `paper-ruled`: `repeating-linear-gradient` ruled lines
- `paper-edge`: subtle page-edge shading

All opt-in via `@utility`; no component is distressed by default.

## 4. Phase 3 — File structure

```text
src/
├── assets/
│   └── svg/                     # hand-drawn stroke assets (see §6)
│       ├── arrow.svg  underline.svg  circle.svg  scribble.svg
│       ├── divider.svg  star.svg  check.svg  bracket.svg
│       └── paperclip.svg
├── components/
│   ├── journal/
│   │   ├── JournalHeader.astro
│   │   ├── JournalMeta.astro
│   │   └── Marginalia.astro
│   ├── paper/
│   │   ├── Paper.astro
│   │   ├── Note.astro
│   │   ├── Tape.astro
│   │   └── PaperClip.astro
│   ├── illustration/
│   │   ├── Sketch.astro
│   │   ├── Scribble.astro
│   │   └── Arrow.astro
│   ├── media/
│   │   ├── ImageFrame.astro
│   │   ├── Polaroid.astro
│   │   └── ContactSheet.astro
│   └── typography/
│       ├── Highlight.astro
│       ├── Underline.astro
│       └── Handwritten.astro
├── pages/dev/components.astro
└── styles/ (journal.css, textures.css, global.css)
```

(`Doodle.astro` removed — redundant with Sketch + Scribble.)

## 5. Component requirements

Conventions shared by all components:

- `rotation?: "none" | "left" | "right"` maps to `0` / `-1.2deg` / `+1.4deg`.
- `class` prop is always available as an escape hatch and merged last.
- Decorative SVG output always carries `aria-hidden="true"`.

### `Paper` — surface primitive
`variant?: "plain" | "note" | "card" | "sketch"`, `rotation?: "none" | "left" | "right"`. Paper bg, subtle border, subtle shadow, consistent padding, composable children. Not distressed by default.

### `Note` — handwritten note surface
`rotation?`, `accent?`, `meta?`. Uses `font-hand`; example: `<Note>Remember to come back here in autumn.</Note>`.

### `Highlight` — marker stroke, not rectangle
Inline component; two overlapping semi-transparent `accent-yellow` gradient strokes with slight rotation/irregular edge (CSS in `journal.css`, layered skewed gradients). Example: `This is <Highlight>particularly important</Highlight>.`

### `Underline` — hand-drawn emphasis
Renders the `underline.svg` asset absolutely positioned under the text (same asset as Scribble's underline variant). Irregular path, tintable via `accent` token colors.

### `Sketch` — framed illustration wrapper
`src`, `caption?`, `rotation?`, `annotation?`; paper surface via `Paper variant="sketch"`.

### `Scribble` — primary decorative stroke renderer
`variant: "underline" | "circle" | "loop" | "divider" | "star"` — renders the matching `?raw` SVG from `src/assets/svg/` with `stroke="currentColor"`, `aria-hidden="true"`. All variants share one stroke language; sizing via wrapper `class`.

### `Arrow` — thin wrapper over Scribble
`direction: "up" | "down" | "left" | "right"` — rotates the arrow SVG. Inherits Scribble's stroke language.

### `ImageFrame` — image presentation
Variants `printed` | `clipped` | `sketch`; props `src`, `alt` (required), `caption?`, `rotation?`. Encapsulates the full visual treatment. (No `polaroid` variant — see `Polaroid`.)

### `Polaroid` — dedicated instant-photo component
White frame + caption area in `font-hand`, optional rotation; composes `ImageFrame variant="printed"`. Optional `tape?: boolean` convenience flag that adds a `Tape` corner.

### `ContactSheet` — grid composition
`columns?`, `label?`; composes `Polaroid`/`ImageFrame` children in a print-sheet grid with subtle alternating rotations.

### `Tape` — masking tape attachment
`position: "top-left" | "top-right" | "bottom-left" | "bottom-right"`; semi-transparent accent tint, torn edges via `clip-path`, subtle. `aria-hidden`.

### `PaperClip` — decorative clip
SVG wire clip, `currentColor`, sized/positioned by caller; `aria-hidden`.

### `JournalHeader` — entry heading
`date`, `location`, `time?`, `title`, `subtitle?`, `annotation?`. Establishes journal page hierarchy. Example:

```text
12 AUGUST 2026
ELsenfeld · 18:42
A walk along the river
```

Uses `JournalMeta` for the date/location line.

### `JournalMeta` — meta line primitive
`date`, `location?`, `time?`; uppercase small-caps body text (`font-body`, `tracking-wide`, `ink-muted`). Reused by JournalHeader and standalone.

### `Marginalia` — responsive side note
`side?: "left" | "right"`. Desktop: `grid lg:grid-cols-[14rem_1fr_14rem]`, note in the left/right column, content in center. Mobile: collapses to inline flow (note before/after content). Pure CSS, **no React**.

### `Handwritten` — typography primitive
`size?: "sm" | "md" | "lg"`, `color?` limited to token names. `font-hand` only; never for long-form text.

## 6. SVG illustration system

- Assets live in `src/assets/svg/*.svg`; components import them via `?raw` and inline them.
- Stroke language: `stroke="currentColor"`, `stroke-width="2"`, `stroke-linecap="round"`, slightly irregular paths, no excessive detail.
- `Scribble` is the primary renderer; `Underline` uses the same `underline.svg` asset inline for its under-text positioning.
- All decorative output carries `aria-hidden="true"` baked in.

## 7. Tailwind v4 integration

Prefer utilities over component CSS: `<div class="bg-paper text-ink shadow-paper">` over `.journal-paper`. CSS reserved for: textures, marker effects, torn tape edges, irregular shapes, global tokens. No `tailwind.config.js` (v3 pattern) — the project does not require one.

## 8. daisyUI v5 integration

- Functional UI layer only: menu (showcase nav), badge, kbd, buttons, tooltips.
- Journal primitives never depend on daisyUI classes.
- The `journal` theme (§3.3) makes functional components match the palette automatically, scoped via `data-theme="journal"` on the showcase.

## 9. Component API conventions

Small intentional variant sets; `class` prop as escape hatch on every component; no color-picker props; no dozens of props. All decorative SVGs `aria-hidden`.

## 10. `/dev/components` showcase

Living documentation page, not screenshots. Every component, multiple realistic examples (real dates, locations, observations — no lorem ipsum).

### 10.1 Layout & sidebar
- Desktop: sticky sidebar (`lg:sticky lg:top-4` wrapper) containing daisyUI `menu menu-sm` with one link per component (`<a href="#paper">Paper</a>`); content column scrolls independently.
- Mobile: `menu menu-horizontal` in a horizontally scrollable strip (`flex-nowrap overflow-x-auto`) under the page header.
- Every section has a stable `id` and `scroll-margin-top` so anchors are never hidden under sticky bars.

### 10.2 Section format
Component name → short description → visual examples → variants list (using `kbd`/`badge` where helpful). The page itself should feel like a large design notebook; sidebar stays conventional.

### 10.3 Examples per component
Paper: plain / note / rotated / varied sizes. Note: short / multiline / rotated / accented. ImageFrame: printed / clipped / sketch / captioned / rotated. Polaroid: standard / rotated / captioned / taped. Highlight: phrase / sentence / multiple. Marginalia: left / right / multiple / mobile collapse. Etc.

### 10.4 Sitemap
Exclude the route: `sitemap({ filter: (page) => !page.includes("/dev/") })` in `astro.config.mjs`.

## 11. Accessibility

Semantic HTML; required `alt` on images; keyboard-accessible interactive elements; visible focus states; AA contrast; no info conveyed only by rotation/color; decorative SVGs `aria-hidden`; `prefers-reduced-motion` respected for any animation; annotations never interrupt reading order; handwritten font never for essential long-form text.

## 12. Responsive

Three real layouts (mobile/tablet/desktop), not scaled-down desktop. Marginalia collapses to flow on mobile. Showcase sidebar: sticky column → horizontal scroll strip. Scrapbook compositions overlap/offset on desktop, simple vertical flow on mobile.

## 13. Avoid overengineering

No React for static components; no design-system abstraction layer; no CMS; no animation libraries; no state management; no daisyUI replacement; no Tailwind v3 config patterns; no random per-render transformations.

## 14. Implementation order (with verification gates)

Each step ends with **`npm run check` and `npm run build` green** before moving on.

1. Add devDeps (`@astrojs/check`, `typescript`), add `"check": "astro check"` script, sitemap filter for `/dev/`.
2. Create `src/styles/` (journal.css tokens, textures.css, global.css), wire imports, add `journal` daisyUI theme.
3. Create `src/assets/svg/` stroke assets (arrow, underline, circle, scribble, divider, star, check, bracket, paperclip).
4. Implement `Paper`, `Note`, `Highlight`, `Underline`, `Handwritten`.
5. Implement `Scribble`, `Arrow`, `Sketch`, `Tape`, `PaperClip`.
6. Implement `ImageFrame`, `Polaroid`, `ContactSheet`.
7. Implement `JournalHeader`, `JournalMeta`, `Marginalia`.
8. Create `/dev/components` skeleton with `data-theme="journal"` wrapper + sidebar menu + anchor sections.
9. Fill every section with realistic examples and variant info.
10. Responsive pass (sidebar strip, marginalia collapse, scrapbook flow).
11. Accessibility pass (keyboard, focus, contrast, alt, aria-hidden).
12. Final visual consistency pass (stroke widths, rotations, spacing rhythm).

## 15. Definition of done

- [ ] Tailwind v4 CSS-first config intact; no v3 patterns
- [ ] daisyUI v5 intact; `journal` theme registered and applied **only** on `/dev/components`
- [ ] Journal tokens centralized in `@theme`; system-font stacks only (no new font deps)
- [ ] All primitives are Astro components; zero new React usage
- [ ] Hand-drawn SVG assets exist with consistent stroke language; consumed via `Scribble`
- [ ] Paper/note/annotation, image/polaroid, and header/marginalia systems exist
- [ ] Small documented APIs with `class` escape hatch
- [ ] `/dev/components` exists, contains every component, multiple realistic examples each
- [ ] Sticky menu sidebar with anchor links + `scroll-margin-top`; works on mobile
- [ ] Sitemap excludes `/dev/components`
- [ ] `npm run check` and `npm run build` pass
- [ ] Basic accessibility + responsive requirements met
- [ ] The system feels like one coherent journal, not a collection of unrelated effects
