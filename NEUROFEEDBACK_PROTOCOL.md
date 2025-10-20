# EEG Neurofeedback Protocol for Hemispatial Neglect
## NeglectFix V0 - Evidence-Based Training Specification

**Based on**: Ros et al. (2017) - "Increased Alpha-Rhythm Dynamic Range Promotes Recovery from Visuospatial Neglect"
**Target condition**: Left hemispatial neglect + hemianopia post-stroke
**Hardware**: Muse EEG headband (TP9, AF7, AF8, TP10)
**Date**: October 2025

---

## Executive Summary

This protocol implements EEG neurofeedback targeting the right posterior parietal cortex (rPPC) to improve left-space awareness. The approach trains users to:
1. **Reduce alpha power** (8-12 Hz) during attention tasks → cortical activation
2. **Increase beta power** (13-30 Hz) → enhanced alertness and sustained attention
3. **Receive real-time VR feedback** when successful brain patterns emerge

**Key evidence**: 5 stroke patients successfully learned to regulate parietal alpha over 6 days (Ros et al., 2017), demonstrating feasibility and therapeutic potential.

---

## Neurophysiological Rationale

### Alpha Oscillations (8-12 Hz)

**Role in neglect**:
- Excessive alpha in right parietal cortex correlates with neglect severity
- Alpha represents "cortical idling" - reduced information processing
- Alpha desynchronization (power reduction) = cortical activation

**Target mechanism**:
- Train to **reduce** alpha during left-attention tasks
- Lower alpha → increased neural excitability in rPPC
- Enhanced processing of left-space stimuli

**Research finding** (Ros et al., 2017):
> "Patients demonstrated successful neurofeedback learning between training sessions, shown by improved regulation of alpha oscillations from rPPC"

### Beta Oscillations (13-30 Hz)

**Role in neglect**:
- Beta activity correlates with alertness and sustained attention
- Neglect patients show **reduced beta** activity
- Recovery associates with **enhanced beta** activity

**Target mechanism**:
- Train to **increase** beta during attention tasks
- Higher beta → improved alertness and vigilance
- Sustained attention to contralesional (left) space

**Research finding**:
> "Neglect is associated with an EEG profile consistent with an alertness deficit, and recovery in severely impaired neglect patients is associated with enhanced beta activity"

### Theta Oscillations (4-8 Hz)

**Role** (monitoring only):
- Theta inversely correlates with alertness
- Elevated theta indicates reduced vigilance
- Used as control/baseline metric

**Target mechanism**:
- Monitor but don't directly train
- Beta/theta ratio as composite alertness index
- Successful training → ↑ beta/theta ratio

---

## Muse EEG Channel Selection

### Primary Target: TP10 (Right Posterior Parietal)

**Anatomical location**:
- Right temporal-parietal junction
- Overlies right posterior parietal cortex (rPPC)
- Critical hub for spatial attention

**Why TP10**:
✅ Directly over lesion site in many stroke patients with neglect
✅ Validated target in Ros et al. (2017) neurofeedback study
✅ Accessible with consumer-grade Muse headband
✅ Strong alpha/beta signals measurable

**Neurofeedback signal**: Extract alpha (8-12 Hz) and beta (13-30 Hz) from TP10

### Secondary Channels (Baseline/Control)

**TP9** (Left temporal-parietal):
- Mirror location on intact hemisphere
- Control comparison (should show normal patterns)
- Not used for training feedback

**AF7/AF8** (Frontal):
- Monitor frontal theta (alertness baseline)
- Artifact detection (eye blinks, movement)
- Not used for primary neurofeedback

---

## Training Protocol

### Session Structure (Based on Ros et al., 2017)

**Duration**: 20 minutes per session
**Frequency**: Daily (or minimum every other day)
**Minimum course**: 6 sessions (1 week)
**Recommended course**: 20-30 sessions (4-6 weeks)

### Session Phases

**Phase 1: Baseline (2 minutes)**
- Eyes open, resting state
- No VR task, neutral environment
- Measure baseline alpha/beta/theta from TP10
- Calculate personalized thresholds

**Phase 2: Training (15 minutes)**
- Active VR exploration task
- Real-time neurofeedback enabled
- Closed-loop: EEG → VR rewards
- Adaptive threshold adjustment

**Phase 3: Cool-down (3 minutes)**
- Reduced task difficulty
- Consolidate learning
- Measure post-session EEG
- Compare to baseline

### Threshold Calculation

**Initial threshold** (first session):
```
alpha_threshold = baseline_alpha_mean - (0.5 × baseline_alpha_std)
beta_threshold = baseline_beta_mean + (0.5 × baseline_beta_std)
```

**Success criterion**:
```
engagement_score = (beta / alpha) × (1 - theta_factor)
reward_trigger = engagement_score > threshold
```

**Adaptive adjustment** (within session):
- If success rate < 30%: Lower threshold by 10%
- If success rate > 70%: Raise threshold by 10%
- Target: 40-60% success rate (optimal learning)

**Between-session progression**:
- Each session starts with new baseline measurement
- Thresholds adapt to improving brain patterns
- Gradual increase in difficulty as learning progresses

---

## Frequency Band Definitions

### Band-Pass Filters (Digital Signal Processing)

**Delta**: 1-4 Hz (not used, artifact monitoring only)
**Theta**: 4-8 Hz (alertness baseline)
**Alpha**: 8-12 Hz (primary target - REDUCE)
**Beta**: 13-30 Hz (primary target - INCREASE)
**Gamma**: 30-50 Hz (not used in V0)

### Power Extraction Method

**Muse data stream** (via Mind Monitor OSC):
- Pre-computed band powers sent at ~10 Hz
- Absolute power values (μV²)
- Already filtered and processed

**Unity processing**:
1. Receive OSC band power packets
2. Apply 1-second moving average (smoothing)
3. Calculate relative power: `power_band / total_power`
4. Compute engagement score
5. Trigger VR feedback if threshold exceeded

**Alternative** (if raw EEG available):
1. Apply band-pass filters (Butterworth, order 4)
2. Calculate FFT (Fast Fourier Transform)
3. Extract power spectral density
4. Sum power in each band range

---

## Closed-Loop Feedback Mechanism

### Input Signals (Multi-Modal)

**Signal 1: EEG Engagement (TP10)**
```
eeg_engagement = (beta_power / alpha_power) × (1 - theta_normalized)
eeg_success = eeg_engagement > eeg_threshold
```

**Signal 2: Gaze Direction (Head Tracking)**
```
head_yaw = Quest2_head_rotation.y
left_gaze = head_yaw > 15° (looking left)
```

**Combined Trigger**:
```
if (eeg_success AND left_gaze):
    trigger_reward()
    log_success_event()
```

### VR Reward Mechanisms

**Visual Feedback** (Primary):
- ✨ Brighten left environment (+20% luminosity)
- 🌟 Highlight/glow objects on left side
- 🎯 Reveal hidden targets (progressive disclosure)
- 🎨 Color saturation boost (make left more vibrant)

**Auditory Feedback** (Secondary):
- 🔔 Pleasant chime sound (spatial audio from left)
- 🎵 Musical tone progression (pitch rises with engagement)
- 💬 Verbal encouragement ("Great! Keep focusing left")

**Haptic Feedback** (Optional, if Quest 2 controllers used):
- Gentle vibration in left controller
- Pulse intensity matches engagement strength

**Timing**:
- Immediate (< 100ms latency from EEG trigger)
- Duration: 2-3 seconds per reward
- Cooldown: 1 second before next reward (prevent spam)

### Negative Feedback (Absence of Reward)

**Do NOT punish failures** - research shows positive reinforcement more effective

When engagement drops or gaze shifts right:
- Gradual dimming of left environment (not sudden)
- Neutral state (no active punishment)
- Audio cues gently redirect: "Try looking left"

---

## Data Logging & Progress Tracking

### Per-Session Metrics

**EEG data** (logged every 100ms):
- Timestamp
- TP10 alpha power (μV²)
- TP10 beta power (μV²)
- TP10 theta power (μV²)
- Engagement score (calculated)
- Threshold value (current)

**Behavioral data** (logged every 100ms):
- Head yaw angle (degrees)
- Head pitch angle (degrees)
- Left-gaze indicator (boolean)
- Reward trigger events (timestamps)

**Performance summary** (per session):
- Total session duration
- Number of reward triggers
- Success rate (% time in engaged state)
- Average engagement score
- Baseline vs. end-session EEG comparison

### Progress Indicators

**Short-term** (within session):
- ✅ Increasing success rate over 15 minutes
- ✅ Rising engagement score trend
- ✅ More time spent gazing left

**Medium-term** (across sessions):
- ✅ Lower baseline alpha (session-to-session)
- ✅ Higher baseline beta (session-to-session)
- ✅ Reduced threshold to maintain 50% success rate

**Long-term** (4-6 weeks):
- ✅ Subjective awareness improvement (self-report)
- ✅ Behavioral assessments (e.g., line bisection test)
- ✅ Functional vision improvements (daily activities)

---

## Safety & Contraindications

### Safe for Home Use ✅

- Non-invasive surface EEG (no implants)
- Consumer-grade hardware (FDA-approved devices)
- No electrical stimulation (passive recording only)
- VR exposure controlled (start with short sessions)

### Considerations

**VR motion sickness**:
- Start with 5-10 minute sessions
- Gradually increase to 20 minutes
- Take breaks if nausea/dizziness occurs
- Avoid sessions if fatigued

**EEG artifact sources**:
- Jaw clenching (contaminates signal)
- Eye blinks (frontal electrodes)
- Head movement (cable pull)
- Poor electrode contact (dry skin)

**Contraindications**:
- Active seizure disorder (VR/flashing lights risk)
- Severe motion sickness history
- Skin irritation at electrode sites
- Acute stroke phase (wait for medical clearance)

---

## Expected Outcomes

### Research-Based Predictions (Ros et al., 2017)

**Learning curve**:
- Session 1-2: Establishing baseline, inconsistent control
- Session 3-4: Beginning to recognize "engaged" brain state
- Session 5-6: Reliable voluntary modulation of alpha/beta
- Session 10+: Sustained improvements, better awareness

**Success indicators**:
- Patients successfully learned neurofeedback between sessions
- Improved regulation of parietal alpha oscillations
- Feasibility demonstrated in stroke population
- Theoretical support for neuroplastic recovery

**Individual variability**:
- Some learn neurofeedback faster than others
- More severe impairments may benefit most (from REINVENT study)
- Consistent daily practice improves outcomes

---

## Protocol Adaptations

### Difficulty Progression

**Week 1** (Sessions 1-6):
- Simple environments (kitchen, bedroom)
- Large, obvious targets on left
- Generous thresholds (easy to trigger rewards)
- Frequent audio cues

**Week 2-3** (Sessions 7-18):
- Complex environments (outdoor scenes, multiple rooms)
- Smaller, varied targets
- Adaptive thresholds (moderate difficulty)
- Reduced audio cues (more independent scanning)

**Week 4+** (Sessions 19+):
- Real-world simulation tasks
- Functional activities (navigation, search)
- Challenging thresholds (sustained engagement required)
- Minimal cues (self-directed exploration)

### Personalization Options

**Threshold strategy**:
- Conservative: Easier thresholds, more frequent success
- Aggressive: Harder thresholds, stronger neuroplastic drive
- Adaptive: Auto-adjust to maintain 50% success rate (recommended)

**Feedback modality**:
- Visual-only (for auditory sensitivity)
- Audio-visual (optimal multi-sensory)
- Haptic + visual (for hearing impaired)

**Session timing**:
- Morning (higher alertness baseline)
- Evening (if preferred, though beta may be lower)
- Consistent time each day (habit formation)

---

## Technical Requirements

### Hardware Minimum Specs

**Muse EEG**:
- Muse 2 or Muse S (4-channel EEG)
- Fully charged (2+ hours battery)
- Proper fit (electrode contact quality > 60%)

**Mobile device** (for Mind Monitor):
- iOS 12+ or Android 8+
- Bluetooth 4.0+
- WiFi for OSC streaming

**Computer** (for Unity):
- Windows 10/11 or macOS 10.15+
- 8GB RAM minimum, 16GB recommended
- GPU: GTX 1060 or equivalent (VR-ready)
- Network connection to receive OSC

**VR headset**:
- Meta Quest 2 (or Quest 3)
- Link cable or Air Link setup
- Comfortable fit with Muse worn simultaneously

### Software Stack

**Muse data acquisition**:
- Mind Monitor app (iOS/Android)
- OSC streaming enabled
- Output to Unity computer IP address

**Unity development**:
- Unity 2021.3 LTS or newer
- XR Plugin Management (Meta Quest support)
- OSC receiver package (e.g., extOSC, OscCore)

**Data logging**:
- CSV export for analysis
- Optional: Python scripts for visualization

---

## Research Citations

1. **Ros, T., et al. (2017)**. "Increased Alpha-Rhythm Dynamic Range Promotes Recovery from Visuospatial Neglect: A Neurofeedback Study." *Neural Plasticity*, 2017, 7407241.
   - https://pubmed.ncbi.nlm.nih.gov/28529806/

2. **Network connectivity studies** (MIT Press, 2022). Disruption of large-scale electrophysiological networks in stroke patients with visuospatial neglect.
   - https://direct.mit.edu/netn/article/6/1/69/107407/

3. **NIHR Open Research**. Novel neurofeedback intervention to reduce neglect and improve function in stroke patients.
   - https://openresearch.nihr.ac.uk/documents/2-51

4. **Beta/theta training**: Editorial on Clinical Neurofeedback (PMC, 2021)
   - https://pmc.ncbi.nlm.nih.gov/articles/PMC9421443/

---

## Next Steps: Implementation

### Phase 1: OSC Integration ✅ (Next)
- Implement OSC receiver in Unity
- Parse Muse band power packets
- Validate data stream (plot real-time values)

### Phase 2: Signal Processing
- Calculate engagement score from TP10
- Implement adaptive threshold logic
- Test with live Muse data

### Phase 3: VR Feedback
- Design reward visual effects
- Implement audio cue system
- Connect EEG triggers to VR events

### Phase 4: Data Logging
- CSV export implementation
- Session summary generation
- Progress visualization dashboard

### Phase 5: Protocol Testing
- Personal baseline sessions (2-3)
- Calibrate thresholds
- Iterate based on experience

---

**Document version**: 1.0
**Last updated**: October 19, 2025
**Status**: Ready for implementation
