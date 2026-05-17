# Backlog

> Work that's blocked, on hold, or waiting for something.
>
> Use status:
> - **WAITING**: Blocked on external factor (API approval, user decision, etc.)
> - **BLOCKED**: Technical blocker needs resolution
> - **PAUSED**: Intentionally set aside, can resume anytime

---

## WIP-001: Audiovisual Training Module

**Status**: SCAFFOLDED (Phase 1 complete 2026-05-16, smoke test pending)
**Created**: 2025-12-15
**Priority**: High

#### Context
Main rehabilitation module — cross-modal audiovisual stimulation for left hemifield recovery. **Reframed 2026-05-14**: was previously framed around Daibert-Nido 2021, now correctly built as Paradigm B (congruent-pair detection, Wake Forest / Rowland 2023 lineage) at the Alharshan/Alwashmi 2026 dose for chronic adult stroke.

#### Phase 1 Complete (2026-05-16)
Scaffolded in `Unity/NeglectFix/Assets/Scripts/Tasks/`:
- `AudioVisualTraining.cs` — main task, 30-min sessions, 3×10-min blocks, 2-up/1-down weighted staircase, sub-50ms AV sync, baseline-driven personalization
- `ProgramScheduler.cs` — session state JSON
- `EccentricityProgression.cs` — CS asymmetry → severity-classified ladder

Plus: `DataLogger.LogTrainingTrial()` added; `RewardController.RewardMode` enum (OpenLoop default, EEG decoupled). Unity 6.2 imports clean, XR Plug-in Management configured (OpenXR + Meta Quest Support). Commits `ecf327f`, `7ea389c`, `24a3075` on `main`.

#### Remaining Work
1. **Smoke test in Editor** (15-20 min fresh-head task): create scene, add `AVTrainingSystem` GameObject with components, override timings to ~1 min total for testing, hit Play, watch console + game view, press SPACE during training, verify trials log to CSV at `Application.persistentDataPath/training_trials/`
2. **Quest controller input binding** — replace keyboard SPACE fallback in `DetectResponse()` with InputSystem.InputAction bound to Quest trigger
3. **Visual stimulus prefab** — replace programmatic Sphere fallback with Gabor patch / high-contrast disk
4. **SSAO disable** for Quest deployment performance (URP renderer asset)
5. **Phase 2 launch** after smoke test passes: 30 sessions × 5 days/week × 6 weeks at the Alharshan dose; mid-program CS checks at sessions 5/10/15/20/25; full reassessment at session 30

---

## WIP-002 (future): Paradigm A — 3D-MOT-IVR

**Status**: NOT-STARTED
**Created**: 2026-05-16 (Phase 3 in build plan)
**Priority**: Medium (deferred until Phase 2 results)

#### Context
After Phase 2 (Paradigm B) runs 30 sessions and produces responder data, optionally build Paradigm A (3D Multiple-Object Tracking in IVR, authentic Daibert-Nido). Significantly larger Unity build — 360° spherical environment, 3D physics for target spheres, distractor logic, gaze/controller pointer selection, adaptive difficulty via target count + sphere velocity. ~4-6 weeks dev time estimate.

#### Decision Gate
- Phase 2 gains ≥ +0.30 LogCS at any trained location → continue with A or extend B
- Phase 2 no detectable gain → switch to A (engages different mechanisms)

---

*Last updated: 2026-05-16*
