# Relationships — NegletFix

> Per-project counterparty tracking. State, temperature, blockers, and free-form context for every human who gates decisions or carries strategic weight on this project. Keep updated; the bridge aggregates across projects for cross-cutting views.

**Schema**: name • role • relationship_type • state • temperature • waiting_on • blocker • last_contact • context

**Legend**:
- *state*: `active` · `waiting-on-them` · `waiting-on-me` · `stalled` · `wound-down` · `not-yet-engaged` · `patient` — project-side status
- *temperature*: `hot` · `warm` · `lukewarm` · `cold` · `frozen` — **human/relationship warmth only**, NOT project blocker status (frictions live in `state` + `blocker`)
- *relationship_type*: `friend-partner` · `partner` · `collaborator` · `service` · `family` · `medical` · `customers` · `self`

---

## Eric (self)

- **Role**: Patient + operator — the user the rehab system is designed for, and the person building it
- **Relationship type**: self
- **State**: active
- **Temperature**: warm
- **Waiting on**: self — fresh-head smoke test of Phase 1 scaffold + Quest controller input binding before Phase 2 program launch
- **Blocker**: smoke test pending (15-min Editor task); Quest 3 acquisition deferred to budget
- **Last contact**: ongoing (self)

**Context**:
Right-PCA stroke July 2021 → left homonymous hemianopia. NegletFix is rehab, not accommodation — explicit framing in `cross-cutting.md`: *"This is a rehabilitation project, not accommodation."*

Originally documented as the most extreme research-to-execution inversion in the portfolio (11 wiki pages, only 5 sessions; WIP-001 "PAUSED, ready to begin" longer than the entire Charles pilot ran). That changed on **2026-05-16**: WIP-001 scaffolded in one session (Phase 1 — `AudioVisualTraining.cs` + `ProgramScheduler.cs` + `EccentricityProgression.cs` + DataLogger extension + RewardController decouple). Unity 6.2 imported clean, XR Plug-in Management configured, 6 commits pushed. Self-blocker pattern is officially partially broken — code exists, smoke test next.

- 2026-05-16: Massive productive session — Quest dev mode resolved + Phase 1 Unity scaffold built. 6 commits pushed to main. Self-blocker partially broken (smoke test still pending but execution gear engaged).

---

## Charlie (daughter)

- **Role**: Was the previous primary account on the Quest 2 — has now handed over device to Eric for dev work
- **Relationship type**: family
- **State**: active (no longer a blocker)
- **Temperature**: hot
- **Waiting on**: nothing project-related
- **Blocker**: none — Quest dev mode unlocked 2026-05-16
- **Last contact**: 2026-05-16 (unpaired Quest from her Meta Horizon app so Eric could re-pair to his account)

**Context**:
Practical hardware blocker, family relationship. Resolved 2026-05-16 by her unpairing the Quest from her Meta Horizon app, followed by factory reset + re-pair to Eric's account. The April 27 theory that this required inviting her account to Eric's dev org turned out to be wrong — community evidence and the working fix confirmed "owner" in Meta's error message means primary device-paired account, not org owner.

- 2026-05-16: Unpaired Quest 2 from her Meta Horizon app on request, enabling factory-reset + re-pair to Eric's account. Dev mode now functional. Quest 2 is effectively transferred to Eric for the rehab project; she retains her Meta account purchases (just inaccessible on this specific headset until/unless she logs in elsewhere).

---

## Dr. Jacquemin

- **Role**: Neurologist — clinical oversight for any pharmacological adjuncts (levodopa, fluoxetine) discussed in `wiki/pharmacological-adjuncts.md`
- **Relationship type**: medical
- **State**: not-yet-engaged
- **Temperature**: lukewarm
- **Waiting on**: Eric to surface this conversation (only after WIP-001 generates baseline data — "if gains < +0.30 LogCS or plateau hit, *then* discuss with neurologist")
- **Blocker**: gated downstream of WIP-001 — no data to present yet
- **Last contact**: (fill in)

**Context**:
Mentioned in `cross-cutting.md` and `wiki/pharmacological-adjuncts.md` as the medical contact for any adjunctive medication conversation. Neurologist-only territory — explicit medical disclaimer in the wiki: *"Several of these compounds interact with antiplatelets/anticoagulants, and some carry stroke-recurrence risk."*

Not engaged on the rehab project yet. Sequential dependency: WIP-001 ships → baseline + 20-session data → discuss adjunct second block. The brain has a one-page summary prep item for this conversation already templated.

---

## Future medical contacts (placeholder)

If you decide to loop in additional clinicians — orthoptist, ophthalmologist, rehab therapist — promote them to entries here. The brain currently has zero of these.
