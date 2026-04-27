# NegletFix Wiki — Index

Compiled knowledge base for the NegletFix project (VR audiovisual + EEG neurofeedback rehabilitation for left homonymous hemianopia). Both a development reference and Eric's personal medical record.

**Last compiled**: 2026-04-14
**Karpathy principle**: *Query, don't load.* Read pages by name as you need them. Do not preload the whole wiki — the whole point is that it lives in files, not in a working-memory dump.

---

## How to Use This Wiki

- **Starting fresh?** Read [[scientific-foundation]] → [[erics-baseline]] → [[rehabilitation-roadmap]] in that order.
- **Writing code?** Go to [[unity-architecture]]; it lists every script, LOC, responsibility.
- **Writing the next training module?** [[audiovisual-training-protocol]] has exact parameters from Daibert-Nido 2021.
- **Adding a new contrast sensitivity measurement?** Append a row to [[erics-baseline]]#progress-log. Never overwrite.
- **Verifying a scientific claim?** Every claim cites a paper — follow the citation to [[research-papers-index]] for the full entry.
- **Curious about adjunctive medications?** [[pharmacological-adjuncts]] catalogs what's been theorized (levodopa, fluoxetine) — neurologist territory only.
- **Confidence tags** (`[HIGH]`, `[MEDIUM]`, `[LOW]`, `[THEORETICAL]`, `[MEASURED]`) inline in each page indicate reliability per claim.

Cross-links between wiki pages use `[[wikilinks]]`. Code references use `path:line` format (e.g., `ContrastSensitivityTest.cs:347`).

---

## Page Catalog

| Page | Description | Last updated | Overall confidence |
|------|-------------|--------------|--------------------|
| [[scientific-foundation]] | Theoretical backbone — two visual pathways (damaged V1 vs. preserved superior colliculus), Stein & Meredith's three principles of multisensory integration, clinical evidence for audiovisual rehab | 2026-04-14 | HIGH |
| [[erics-baseline]] | The 2025-12-15 anchor measurement (Left 0.00, Right 2.25, Central ~1.05 LogCS) + progress log for future sessions | 2026-04-14 | HIGH |
| [[contrast-sensitivity-test]] | Measurement instrument — Modified Pelli-Robson protocol, Sloan letters, hemifield positioning fix, gamma correction | 2026-04-14 | HIGH |
| [[audiovisual-training-protocol]] | Daibert-Nido 2021 protocol — 400 Hz/250 ms audio, looming 55→75 dB, 8°→24°→56° eccentricity, 15 min × ~20 sessions, target +0.31-0.54 LogCS | 2026-04-14 | HIGH |
| [[eeg-neurofeedback]] | Muse TP10 pipeline — β/α ratio engagement formula, adaptive threshold, Ros et al. 2017 basis, simulator details | 2026-04-14 | MIXED |
| [[unity-architecture]] | 10 scripts / 3,339 LOC across Assessment, EEG, Tasks, Utils — data flow diagram, deployment, known tech debt | 2026-04-14 | HIGH |
| [[rehabilitation-roadmap]] | Honest DONE / NEXT / UNKNOWN progress map — critical path, risks, open questions | 2026-04-14 | MIXED |
| [[hardware-setup]] | Quest + Muse + Mind Monitor + Mac, OSC port 5000, troubleshooting table, ~$500 total cost | 2026-04-14 | HIGH |
| [[research-papers-index]] | Every cited paper — full citation, DOI, key finding, NegletFix relevance, confidence rating | 2026-04-14 | MIXED |
| [[pharmacological-adjuncts]] | Levodopa 100mg + Fluoxetine — theoretical adjuncts to AV training, evidence extrapolated from motor stroke trials, neurologist-only territory | 2026-04-27 | MIXED |

---

## Paper Citations Index (Alphabetical)

Full entries in [[research-papers-index]]. DOI status:
- ✓ verified or derived from URL in source files
- ⚠ flagged `[CITATION NEEDED — verify]` — cited in project without a traceable DOI

| Author/Year | DOI | Role |
|-------------|-----|------|
| Archives PM&R (2024) — VR stroke meta-analysis | ⚠ | VR rehab breadth |
| Bolognini et al. (2005) | ⚠ | Multisensory integration in hemianopia |
| Cuppini et al. (2017) | ⚠ | Computational model of AV rehab |
| Daibert-Nido et al. (2021) | ✓ 10.3389/fneur.2021.680211 | **Anchor paper** — AV home rehab protocol |
| Elliott, Sanderson & Conkey (1990) | ⚠ | Pelli-Robson reliability (±0.24 LogCS) |
| Frontiers (2023) — VR-VET RCT | ✓ 10.3389/fnins.2023.1142663 | VR for neglect (adjacent) |
| J. NeuroEngineering Rehab (2025) | ✓ 10.1186/s12984-025-01573-4 | Multisensory home telerehab |
| Network Neuroscience / MIT Press (2022) | ⚠ | EEG spectral signature of neglect |
| Pelli, Robson & Wilkins (1988) | Pre-DOI | Original PR chart methodology |
| REINVENT (Spicer et al. 2019) | ✓ 10.3389/fnhum.2019.00210 | Closed-loop VR-BCI precedent |
| Robineau et al. (2017) | ⚠ | Self-regulation of inter-hem. balance |
| Ros et al. (2017) | ✓ 10.1155/2017/7407241 | EEG NFB in neglect (primary NFB basis) |
| Sitaram et al. (2017) | ⚠ | Neurofeedback methodology review |
| Stein & Meredith (1993) | N/A (book) | Multisensory integration theory |
| Topics in Stroke Rehab (2020) | ✓ 10.1080/10749357.2020.1716531 | Ecological VR training |
| Wake Forest / Rowland & Stein | N/A (trials) | Alternative AV parameters |

---

## Module Readiness Map

Source: `.brain/index.json` (as of 2025-12-15), updated reading of code as of 2026-04-14.

| Module | Status | Health | Path | Notes |
|--------|--------|--------|------|-------|
| Contrast Sensitivity Test | operational | 🟢 green | `Unity/NeglectFix/Assets/Scripts/Assessment/ContrastSensitivityTest.cs` | Hemifield bug fixed 2025-12-15, first baseline recorded |
| EEG Pipeline (code) | implemented | 🟡 yellow | `Unity/NeglectFix/Assets/Scripts/EEG/` | Code written + simulator path works; real Muse untested on Eric |
| EEG Integration (hardware) | planned | ⚪ gray | — | Requires Muse + Mind Monitor + extOSC install |
| Audiovisual Training Module | planned | ⚪ gray | `Unity/NeglectFix/Assets/Scripts/Tasks/` (to be created) | WIP-001 in backlog; parameters fully specified |
| Quest VR Deployment | planned | ⚪ gray | — | OpenXR + Meta Quest Support configured in Unity, not yet sideloaded |
| Reward Controller + Gaze Detector | implemented | 🟡 yellow | `Unity/NeglectFix/Assets/Scripts/Utils/` | Code written, will be exercised by AV training task |
| Data Logger | implemented | 🟡 yellow | `Unity/NeglectFix/Assets/Scripts/Utils/DataLogger.cs` | CSV export wired, 10 Hz logging |

Legend: 🟢 operational, 🟡 code ready but untested end-to-end on Eric, ⚪ not yet started or hardware-gated.

---

## Quick Links

- Project root: `/Users/ericlespagnon/Dropbox/DEV-LOCAL/NegletFix/`
- Unity project: `Unity/NeglectFix/`
- Brain memory: `.brain/index.json`, `.brain/sessions/`, `.brain/backlog.md`
- GitHub: https://github.com/ricopoulos/NegletFix
- Anchor paper: Daibert-Nido et al. 2021, DOI 10.3389/fneur.2021.680211
