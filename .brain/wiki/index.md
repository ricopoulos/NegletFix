# NegletFix Wiki — Index

Compiled knowledge base for the NegletFix project (VR audiovisual + EEG neurofeedback rehabilitation for left homonymous hemianopia). Both a development reference and Eric's personal medical record.

**Last compiled**: 2026-04-14
**Last research audit**: 2026-06-11 — clinical-trials watchlist and static HTML research monitor added for AV+tDCS, audiovisual/multisensory training, visual restoration, and neuromodulation leads; LuMamba/BioFoundation added as an EEG foundation-model watch lead. Key status correction: NCT04963075 is now completed with posted results; NCT05894434 is not yet recruiting; NCT06116760 is the completed exact AV+tDCS chronic HVFD trial linked to Diana 2025.
**Last implementation update**: 2026-06-11 — quick field-mapping calibration is built and Quest-validated; Session1Pilot now supports field-guided rehab targets and completed a 12.5-minute Quest run at left `-5°`. Interpretation guard: Eric's subjective report suggests the `-5°` run was partly audio-cued/reflexive and too close to center, so next work is target/probe retuning (`-5°` + harder `-8°`, catch/probe trials, staircase cap) before increasing dose; see [[audiovisual-training-protocol]], [[unity-architecture]], [[hardware-setup]], and [[rehabilitation-roadmap]].
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
| [[audiovisual-training-protocol]] | Daibert-Nido family — 3 dose options (Daibert-Nido 5h pediatric / Misawa 5.25h pediatric Quest / **Alharshan 15h adult stroke** ← recommended for Eric); **2026-06-11**: quick field map built/validated, first field-guided run completed at left `-5°`, and next target/probe retuning decision documented | **2026-06-11** | MIXED ↓ |
| [[eeg-neurofeedback]] | Muse TP10 pipeline; **2026-05-14**: hemianopia-vs-neglect category mismatch flagged, Treves 2025 consumer-NF null result, Muse signal-quality downgrade; **2026-06-11**: LuMamba/BioFoundation added as EEG-AI methods watch lead, no protocol impact | **2026-06-11** | MIXED ↓ |
| [[unity-architecture]] | Unity architecture and runtime state; **2026-06-11**: field-mapping calibration scene/script/test added and Session1Pilot can use field-guided rehab angles while keeping controls separate | **2026-06-11** | HIGH |
| [[rehabilitation-roadmap]] | Honest DONE / NEXT / UNKNOWN progress map; **2026-06-11**: field map + field-guided run completed; next is retuning targets/probes/staircase before increasing dose | **2026-06-11** | MIXED |
| [[hardware-setup]] | Quest + Muse + Mind Monitor + Mac, OSC port 5000, Quest ADB/MQDH/Wi-Fi workflow, troubleshooting table | **2026-05-30** | HIGH |
| [[research-papers-index]] | Every cited paper — full citation, DOI, key finding, NegletFix relevance; **2026-06-11**: AV+tDCS and visual-restoration clinical-trial status refresh added | **2026-06-11** | HIGH (citations now verified) |
| [[clinical-trials-watchlist]] | Live-tracked NCT registry watchlist for AV+tDCS, AV/multisensory, VRT, VR cross-modal, and neuromodulation trials; source CSV in `docs/research/clinical-trials-watchlist-2026-06-11.csv`; static monitor in `docs/research/research-monitor-2026-06-11.html` | **2026-06-11** | HIGH for registry status / MIXED for protocol impact |
| [[pharmacological-adjuncts]] | Medication adjuncts and doctor-conversation boundary; **2026-05-26**: refreshed against DARS/ESTREL levodopa evidence and 2024 fluoxetine IPD meta-analysis; no medication promoted for chronic HH recovery | **2026-05-26** | MIXED |

---

## Paper Citations Index (Alphabetical)

Full entries in [[research-papers-index]]. DOI status:
- ✓ verified or derived from URL in source files
- ⚠ flagged `[CITATION NEEDED — verify]` — cited in project without a traceable DOI

Updated 2026-05-26 with medication-adjunct caution entries from the doctor-brief sprint, after the 2026-05-14 PubMed verification audit.

| Author/Year | DOI | Role |
|-------------|-----|------|
| Alharshan/Alwashmi et al. (2026) ★ | PMID 41421499 | DTI mechanism in adult stroke HH AV training (n=15) |
| Alwashmi et al. (2024) ★ | PMID 38048921 | VR AV training fMRI mechanism (n=20 healthy) |
| Archives PM&R (2024) — VR stroke meta-analysis | ✓ derived | VR rehab breadth |
| Bagherzadeh et al. (2026) ★ | NeuroImage 332:121912 | Closed-loop IAF-NF causal evidence (n=108 healthy) |
| Bean, Stein & Rowland (2023) ★ | PMID 37724427 | Mechanism revision (animal) |
| Bolognini et al. (2005) | ✓ 10.1093/brain/awh662 | Multisensory integration in hemianopia |
| Broustail/Ingolfsson et al. (2026) | ✓ 10.48550/arXiv.2603.19100 | EEG foundation-model methods watch lead |
| Chen et al. (2026) ★ | JMIR 28:e79132 | BCI meta-analysis, 21 RCTs chronic stroke |
| Daibert-Nido et al. (2021) | ✓ 10.3389/fneur.2021.680211 | AV home rehab pilot (N=2 pediatric) — *reframed 2026-05-14* |
| Diana et al. (2025) ★ | ✓ 10.1111/ene.16559 | AV+tDCS RCT chronic HVFD (n=18) |
| DARS / Ford et al. (2019) | PMID 31122493 | Co-careldopa stroke-rehab caution |
| Elliott, Bullimore & Bailey (1991) ✱ | ✓ 10.1111/j.1475-1313.1991.tb00368.x | Pelli-Robson reliability (±0.24 LogCS) |
| ESTREL (2025) | PMID 40982270 | Levodopa stroke-rehab caution |
| ESO Guideline / Rowe et al. (2025) ★ | PMID 40401755 | First international consensus on post-stroke vision |
| Fluoxetine IPD meta-analysis (2024) | PMC11298115 | Routine fluoxetine recovery caution |
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
| NCT06116760 | ClinicalTrials.gov | Completed AV+tDCS chronic HVFD trial; exact adjunct lead |
| NCT04963075 | ClinicalTrials.gov | Completed Wake Forest multisensory rehab with posted results |
| NCT07358832 | ClinicalTrials.gov | Recruiting subacute AV+tDCS trial to watch |
| Yang/Cavanaugh/Saionz et al. (2023) ★ | PMC10491352 (preprint) | Chronic CS reality check (n=12) |
| Yang/Fiebelkorn/Kastner et al. (2024) ★ | PNAS 121(45):e2313304121 | Alpha-gating mechanism (ECoG) |
| Zeri et al. (2018) ★ | PMID 28639086 | LCD-vs-printed Pelli-Robson divergence |

★ = added 2026-05-14. ✱ = citation corrected 2026-05-14.

---

## Module Readiness Map

Source: `.brain/index.json`, updated after the 2026-06-11 field-map-guided rehab run.

| Module | Status | Health | Path | Notes |
|--------|--------|--------|------|-------|
| Contrast Sensitivity Test | operational | 🟢 green | `Unity/NeglectFix/Assets/Scripts/Assessment/ContrastSensitivityTest.cs` | Hemifield bug fixed 2025-12-15, first baseline recorded |
| EEG Pipeline (code) | implemented | 🟡 yellow | `Unity/NeglectFix/Assets/Scripts/EEG/` | Code written + simulator path works; real Muse untested on Eric; LuMamba/BioFoundation tracked as future offline signal-analysis lead only |
| EEG Integration (hardware) | planned | ⚪ gray | — | Requires Muse + Mind Monitor + extOSC install |
| Audiovisual Training Module | field-guided-rehab-headset-validated | 🟡 yellow | `Unity/NeglectFix/Assets/Scripts/Tasks/AudioVisualTraining.cs` | Field-guided 12.5-minute Quest run completed 2026-06-11 at left `-5°`; next build must retune targets/probes/staircase before increasing dose |
| Field Mapping Calibration | operational | 🟢 green | `Unity/NeglectFix/Assets/Scripts/Assessment/FieldMappingCalibration.cs` / `Unity/NeglectFix/Assets/Scenes/FieldMappingCalibration.unity` | Fixed cross + controlled left/right/up/down points + spatial/head-pose logging validated on Quest; first recommendation was left `-5°`, vertical `0°` |
| Research Watchlist | operational | 🟢 green | `docs/research/source-queue-2026-05-25.csv` / `docs/research/clinical-trials-watchlist-2026-06-11.csv` / `docs/research/research-monitor-2026-06-11.html` / `[[clinical-trials-watchlist]]` | Source intake now includes YouTube, X, LinkedIn/arXiv, and ClinicalTrials.gov rows; no automation created yet |
| Quest VR Deployment | operational | 🟢 green | `Builds/AVTrainingSession1Pilot.apk` / `Builds/AVTrainingQuickReadyCheck.apk` | Quest 2 USB ADB authorized; MQDH restored USB handshake; Wi-Fi ADB helper available in `scripts/quest-adb.sh` |
| Reward Controller + Gaze Detector | implemented | 🟡 yellow | `Unity/NeglectFix/Assets/Scripts/Utils/` | Code written, will be exercised by AV training task |
| Data Logger | implemented | 🟡 yellow | `Unity/NeglectFix/Assets/Scripts/Utils/DataLogger.cs` | CSV export wired, 10 Hz logging, AV trial rows split by rehab/control flags |

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
