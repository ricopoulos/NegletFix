# Session: Karpathy Wiki Build

**Date**: 2026-04-14
**Status**: Complete

## Objectives
- Compile the NegletFix knowledge corpus into a navigable wiki at `.brain/wiki/` using the Karpathy ingest pattern (compilation, not storage).
- Preserve accuracy as a personal medical record — every scientific claim cited, confidence-rated.

## Outcomes
- 10 wiki pages produced (1,719 lines total), all with frontmatter, wikilinks, confidence tags.
- 16 papers indexed in `research-papers-index.md`.
- Source corpus compiled: all `.brain/` files, `RESEARCH_SUMMARY.md`, `NEUROFEEDBACK_PROTOCOL.md`, `VR_REHABILITATION_TASKS.md`, `PROJECT_SUMMARY.md`, `docs/research/*`, all 10 Unity C# scripts.

### Pages created (at `.brain/wiki/`)
| File | Lines | Confidence |
|---|---|---|
| `index.md` | 90 | — (catalog) |
| `scientific-foundation.md` | 130 | MIXED |
| `erics-baseline.md` | 134 | HIGH |
| `contrast-sensitivity-test.md` | 196 | HIGH |
| `audiovisual-training-protocol.md` | 205 | HIGH |
| `eeg-neurofeedback.md` | 211 | MEDIUM |
| `unity-architecture.md` | 201 | HIGH |
| `hardware-setup.md` | 182 | HIGH |
| `rehabilitation-roadmap.md` | 158 | MIXED |
| `research-papers-index.md` | 212 | MIXED |

## Gaps / Flags for Eric's review
1. **Temporal binding window conflict** — `docs/research/scientific_foundation.md:30` says ~100–500ms (SC-neuron level); prompt spec said ~16ms (perceptual binding). Both documented in wiki; pick implementation target.
2. **7 DOIs unverifiable** from source files — tagged `[CITATION NEEDED — verify]` (Bolognini 2005, Cuppini 2017, Elliott 1990, Robineau 2017, Sitaram 2017, Network Neuroscience 2022, Arch PM&R 2024).
3. **Two latent code bugs** discovered while reading scripts (documented in `unity-architecture.md#known-code-issues`):
   - `EEGSimulator.OnKeyPress()` isn't a Unity message — E/R shortcuts never fire.
   - `alphaWeight` in `EngagementCalculator.cs:28` declared but unused.
4. **Engagement formula mismatch** — `EEGSimulator` uses `(β+γ)/(α+θ)`; production `EngagementCalculator` differs. Clarified in `eeg-neurofeedback.md`.
5. **Path casing** — repo `NegletFix/`, Unity project `Unity/NeglectFix/`. Preserved as-is.

## Files Modified
- Added: `.brain/wiki/` (10 new files)
- Added: `.brain/sessions/2026-04-14-karpathy-wiki-build.md` (this file)
- Updated: `.brain/index.json` (wiki catalog added, last_updated bumped)

## VPS Drift Check
- N/A — NegletFix has no remote server. Local-only.

## Branch Status
- All work on `main`. No feature branches open.

## Next Steps
- Eric verifies the 7 flagged DOIs before any clinical/publication use.
- Decide temporal binding window target (~16ms vs ~100–500ms) for audiovisual implementation.
- Fix the two latent simulator bugs (low severity, easy wins).
- Resume WIP-001: Audiovisual Training Module — protocol fully documented in `audiovisual-training-protocol.md`, ready to implement.
