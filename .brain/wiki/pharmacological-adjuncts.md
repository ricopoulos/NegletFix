---
title: Pharmacological Adjuncts
last_updated: 2026-04-27
confidence: MIXED
sources:
  - compass_artifact_wf-f2397be7-2bde-4899-b1f9-271446d4f3e3_text_markdown.md
  - .brain/sessions/2026-04-27-quest-setup-pharma.md
---

# Pharmacological Adjuncts

Compounds referenced in NegletFix's research corpus as **theoretical adjuncts** to audiovisual training. None have proven efficacy specifically for visual cortex rehabilitation; both candidates below have evidence extrapolated from **motor stroke recovery** trials. This page exists so the names don't get lost — not as a recommendation.

> **⚠ Medical disclaimer**: Eric is post right-PCA stroke (July 2021). Any pharmacological intervention requires neurologist approval. Several of these compounds interact with antiplatelets/anticoagulants, and some carry stroke-recurrence risk. Treat this page as background for a clinical conversation, not a protocol.

See [[rehabilitation-roadmap]]#unknown--at-risk for where this fits in the project's risk map, and [[audiovisual-training-protocol]] for the behavioral training that any adjunct would augment.

---

## 1. Candidates Referenced in Source Material

| Compound | Dose cited | Origin trial | Mechanism | Status for visual rehab |
|----------|-----------|--------------|-----------|------------------------|
| **Levodopa (L-DOPA)** | 100 mg | Scheidtmann et al. 2001 (Lancet, motor stroke) | Dopaminergic; hypothesized Nogo-A pathway downregulation → enhanced plasticity | No proven visual efficacy. DARS trial 2019 negative for motor. |
| **Fluoxetine** | (dose unspecified in source) | FLAME trial 2011 (motor stroke); FOCUS / EFFECTS / AFFINITY 2019 negative replication | SSRI; ↑BDNF | Active trial **NCT02737930** investigating visual field recovery. |

Source quote (`compass_artifact_wf-f2397be7-2bde-4899-b1f9-271446d4f3e3_text_markdown.md`):

> *"Regarding pharmacology, no medications have proven efficacy specifically for visual cortex rehabilitation, but theoretical candidates include levodopa (100mg), which enhanced motor recovery in the Scheidtmann trial and may promote plasticity through Nogo-A pathway downregulation. Fluoxetine increased BDNF and improved motor recovery in the FLAME trial; an active clinical trial (NCT02737930) is investigating its effects on visual field recovery. These medications should be considered adjuncts to behavioral training rather than standalone treatments."*

And in the recommendation section of the same artifact:

> *"A discussion with the neurologist about adjunctive levodopa (100mg) or fluoxetine during the rehabilitation period may enhance plasticity, though evidence is extrapolated from motor recovery."*

---

## 2. Why These Are Only Theoretical Here

- **Mechanism is plausible, transfer is not proven.** Both compounds plausibly enhance neuroplasticity. Whether the visual cortex / superior colliculus pathway responds the way M1 does is an open question.
- **Replication problem.** FLAME (positive for fluoxetine in motor recovery) was followed by three large negative trials (FOCUS, EFFECTS, AFFINITY). The current view is that fluoxetine's stroke-rehab benefit is at best small and condition-specific.
- **Time-window matters.** Scheidtmann's positive levodopa effect was in subacute motor rehab. Eric is **chronic** (>4 years post-stroke as of 2026-04). Saionz et al. 2022 found subacute training is ~6× more effective than chronic — adjunct effects likely scale similarly.
- **Confound for the project.** If Eric starts an adjunct *and* WIP-001 audiovisual training simultaneously, an eventual LogCS gain can't be attributed to either alone. Sequence matters: train first, measure response, consider adjunct only if plateau is reached.

---

## 3. Substances NOT Recommended Without Strong Medical Supervision

For completeness — these come up in stroke-rehab adjacent literature but are not in NegletFix's source material and carry meaningful risk in a post-PCA-stroke patient:

- **D-amphetamine** — historic Walker-Batson studies, mostly negative replication, **stroke-recurrence risk**
- **Methylphenidate** — small trials in *spatial neglect* (different syndrome), arousal effect
- **Donepezil** — cholinergic, attention/cognition signal, mixed evidence in stroke
- **Modafinil** — alertness, no specific visual rehab evidence
- **Citicoline (CDP-choline)** — OTC in France, broad post-stroke recovery signal (ICTUS trial 2012 mixed), low-risk profile, sometimes used as a low-stakes adjunct

---

## 4. The Honest Take

The audiovisual training itself (Daibert-Nido protocol, see [[audiovisual-training-protocol]]) is the **active ingredient**. Cohort gains of +0.31–0.54 LogCS were achieved without any pharmacological adjunct. Adding a drug:

1. Introduces a confound for measurement
2. Has uncertain transfer from motor → visual
3. Has uncertain transfer from subacute → chronic
4. Carries non-zero risk

**Suggested order if Eric ever wants to try this**:

1. Run the full Daibert-Nido 20-session program first (open-loop, no adjunct)
2. Re-test contrast sensitivity vs. baseline (Left 0.00 LogCS, Right 2.25)
3. If gains < +0.30 LogCS or plateau hit, *then* discuss with neurologist whether a second 20-session block with levodopa adjunct is reasonable
4. If trying, single variable change only (don't simultaneously add EEG closed-loop and a drug — can't disentangle)

---

## 5. Cross-References

- Where this fits in project risk: [[rehabilitation-roadmap]]#unknown--at-risk
- The behavioral protocol any adjunct would augment: [[audiovisual-training-protocol]]
- Evidence for the Daibert-Nido baseline gains: [[research-papers-index]]
- Source artifact: `compass_artifact_wf-f2397be7-2bde-4899-b1f9-271446d4f3e3_text_markdown.md` at repo root (untracked, worth filing under `docs/research/`)

---

## 6. Open Items

- [ ] Verify the Scheidtmann 2001 Lancet citation (levodopa + motor rehab) — full DOI
- [ ] Verify NCT02737930 trial status and read-out date
- [ ] Decide whether to file the compass artifacts under `docs/research/` with proper names
- [ ] If interested clinically: prepare a one-page summary for Dr. Jacquemin (the medical brief in `docs/Eric Files/` is a starting template)
