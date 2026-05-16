# NegletFix Wiki — Index

Compiled knowledge base for the NegletFix project (VR audiovisual + EEG neurofeedback rehabilitation for left homonymous hemianopia). Both a development reference and Eric's personal medical record.

**Last compiled**: 2026-04-14
**Last research audit**: 2026-05-14 — see [[research-papers-index]] Recent Additions for 20+ new PubMed-verified citations, 3 corrected citation errors, and the Daibert-Nido 2021 reframing.
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
| [[scientific-foundation]] | Theoretical backbone — two visual pathways, Stein & Meredith's three principles, multisensory rehab evidence; **2026-05-14**: chronic-vs-subacute reality check (Saionz 2020/2025), Daibert-Nido downgrade, NF replication caveats | **2026-05-14** | HIGH (mech) / MIXED (effect sizes) |
| [[erics-baseline]] | The 2025-12-15 anchor measurement (Left 0.00, Right 2.25, Central ~1.05 LogCS) + progress log for future sessions | 2026-04-14 | HIGH |
| [[contrast-sensitivity-test]] | Measurement instrument — Modified Pelli-Robson; **2026-05-14**: LCD-vs-printed Zeri 2018 caveat, qCSF future-upgrade note | **2026-05-14** | HIGH (within-instrument) / MEDIUM (absolute) |
| [[audiovisual-training-protocol]] | Daibert-Nido family — 3 dose options (Daibert-Nido 5h pediatric / Misawa 5.25h pediatric Quest / **Alharshan 15h adult stroke** ← recommended for Eric); **2026-05-14**: reframed from "anchor protocol with HIGH confidence" to "synthesized protocol family" | **2026-05-14** | MIXED ↓ |
| [[eeg-neurofeedback]] | Muse TP10 pipeline; **2026-05-14**: hemianopia-vs-neglect category mismatch flagged, Treves 2025 consumer-NF null result, Muse signal-quality downgrade | **2026-05-14** | MIXED ↓ |
| [[unity-architecture]] | 10 scripts / 3,339 LOC across Assessment, EEG, Tasks, Utils — data flow diagram, deployment, known tech debt | 2026-04-14 | HIGH |
| [[rehabilitation-roadmap]] | Honest DONE / NEXT / UNKNOWN progress map; **2026-05-14**: added Namgung 2024/2025 + Raffin 2025 + Diana 2025 to evidence base, downgraded Daibert-Nido expectations | **2026-05-14** | MIXED |
| [[hardware-setup]] | Quest + Muse + Mind Monitor + Mac, OSC port 5000, troubleshooting table, ~$500 total cost | 2026-04-14 | HIGH |
| [[research-papers-index]] | Every cited paper — full citation, DOI, key finding, NegletFix relevance; **2026-05-14**: 3 citation corrections (Cuppini→Magosso, Elliott 1990→1991, Robineau 2017→2014) + 20+ new PubMed-verified entries | **2026-05-14** | HIGH (citations now verified) |
| [[pharmacological-adjuncts]] | Levodopa + Fluoxetine — theoretical adjuncts to AV training; **2026-05-14**: FLUORESCE (NCT02737930) confirmed completed/published 2023, not active | **2026-05-14** | MIXED |

---

## Paper Citations Index (Alphabetical)

Full entries in [[research-papers-index]]. DOI status:
- ✓ verified or derived from URL in source files
- ⚠ flagged `[CITATION NEEDED — verify]` — cited in project without a traceable DOI

Updated 2026-05-14 with PubMed-verified DOIs for previously-`[CITATION NEEDED]` entries and 20+ new papers (see [[research-papers-index]] Recent Additions for the full set).

| Author/Year | DOI | Role |
|-------------|-----|------|
| Alharshan/Alwashmi et al. (2026) ★ | PMID 41421499 | DTI mechanism in adult stroke HH AV training (n=15) |
| Alwashmi et al. (2024) ★ | PMID 38048921 | VR AV training fMRI mechanism (n=20 healthy) |
| Archives PM&R (2024) — VR stroke meta-analysis | ✓ derived | VR rehab breadth |
| Bagherzadeh et al. (2026) ★ | NeuroImage 332:121912 | Closed-loop IAF-NF causal evidence (n=108 healthy) |
| Bean, Stein & Rowland (2023) ★ | PMID 37724427 | Mechanism revision (animal) |
| Bolognini et al. (2005) | ✓ 10.1093/brain/awh662 | Multisensory integration in hemianopia |
| Chen et al. (2026) ★ | JMIR 28:e79132 | BCI meta-analysis, 21 RCTs chronic stroke |
| Daibert-Nido et al. (2021) | ✓ 10.3389/fneur.2021.680211 | AV home rehab pilot (N=2 pediatric) — *reframed 2026-05-14* |
| Diana et al. (2025) ★ | ✓ 10.1111/ene.16559 | AV+tDCS RCT chronic HVFD (n=18) |
| Elliott, Bullimore & Bailey (1991) ✱ | ✓ 10.1111/j.1475-1313.1991.tb00368.x | Pelli-Robson reliability (±0.24 LogCS) |
| ESO Guideline / Rowe et al. (2025) ★ | PMID 40401755 | First international consensus on post-stroke vision |
| Frontiers (2023) — VR-VET RCT | ✓ 10.3389/fnins.2023.1142663 | VR for neglect (adjacent) |
| J. NeuroEngineering Rehab (2025) | ✓ 10.1186/s12984-025-01573-4 | Multisensory home telerehab |
| Laver et al. — Cochrane VR (2025) ★ | ✓ 10.1002/14651858.CD008349.pub5 | Latest Cochrane on VR-stroke |
| Magosso, Cuppini & Bertini (2017) ✱ | ✓ 10.3389/fncom.2017.00113 | Computational model of AV rehab |
| Misawa/Daibert-Nido et al. (2024) ★ | PMID 39687429 | Quest 2/Pro home delivery validation (n=10 pediatric) |
| Namgung et al. (2024) ★ | ✓ 10.1002/brb3.3525 | Chronic VR-VPL multicenter RCT (n=82) |
| Namgung et al. (2025) ★ | ✓ 10.1001/jamanetworkopen.2025.11068 | Personalized chronic VR-VPL JAMA RCT (n=82) |
| Network Neuroscience / MIT Press (2022) | ✓ derived | EEG spectral signature of neglect |
| Pelli, Robson & Wilkins (1988) | Pre-DOI | Original PR chart methodology |
| Raffin et al. (2025) ★ | PMID 41243213 | cf-tACS V1↔MT in chronic hemianopia (n=16) |
| REINVENT (Spicer et al. 2019) | ✓ 10.3389/fnhum.2019.00210 | Closed-loop VR-BCI precedent |
| Robineau et al. (2014) ✱ | ✓ 10.1016/j.neuropsychologia.2014.07.020 | fMRI NF in left HH (direct precedent) |
| Ros et al. (2017) | ✓ 10.1155/2017/7407241 | EEG NFB in neglect |
| Rowland/Bushnell/Duncan/Stein (2023) ★ | PMID 36604169 | Chronic AV training adult stroke (n=2) |
| Saionz et al. (2020) ★ | ✓ 10.1093/brain/awaa145 | Subacute >> chronic finding |
| Saionz et al. (2025 — VF natural hx) ★ | PMID 40478590 | Chronic VF stability natural history (n=73) |
| Scheidtmann et al. (2001) ★ | ✓ 10.1016/S0140-6736(01)05456-X | Levodopa motor-stroke anchor |
| Schneider et al. (FLUORESCE 2023) ★ | PMID 36166771 | Fluoxetine pilot (acute only) |
| Sitaram et al. (2017) | ✓ 10.1038/nrn.2016.164 | Neurofeedback methodology review |
| Stein & Meredith (1993) | N/A (book) | Multisensory integration theory |
| Topics in Stroke Rehab (2020) | ✓ 10.1080/10749357.2020.1716531 | Ecological VR training |
| Treves et al. (2025) ★ | PMID 40246295 | Consumer-NF meta-analysis (null) |
| Wake Forest / Rowland & Stein | N/A (trials) | Alternative AV parameters |
| Yang/Cavanaugh/Saionz et al. (2023) ★ | PMC10491352 (preprint) | Chronic CS reality check (n=12) |
| Yang/Fiebelkorn/Kastner et al. (2024) ★ | PNAS 121(45):e2313304121 | Alpha-gating mechanism (ECoG) |
| Zeri et al. (2018) ★ | PMID 28639086 | LCD-vs-printed Pelli-Robson divergence |

★ = added 2026-05-14. ✱ = citation corrected 2026-05-14.

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
- Original anchor paper: Daibert-Nido et al. 2021, DOI 10.3389/fneur.2021.680211 (*N=2 pediatric pilot — see [[research-papers-index]] for reframing*)
- Closest adult-stroke evidence: Alharshan/Alwashmi et al. 2026, PMID 41421499
- Strongest chronic-stroke RCT: Namgung et al. 2025 JAMA Netw Open, n=82 chronic post-stroke
