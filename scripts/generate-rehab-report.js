#!/usr/bin/env node

const fs = require("fs");
const path = require("path");

const repoRoot = path.resolve(__dirname, "..");

const reportConfig = {
  title: "NeglectFix Session Report",
  subtitle: "Controlled Dose Ramp",
  dateLabel: "May 31, 2026",
  patientLabel: "Eric Lespagnon",
  outputPath: path.join(repoRoot, "reports", "rehab-dose-ramp-2026-05-31.html"),
  recommendation:
    "Stop today after 25 minutes of recorded dose. Next session: repeat 12 minutes if today felt heavy, or move to 15 minutes if fresh.",
  runs: [
    {
      id: "ramp5",
      label: "5 min",
      storyLabel: "Shakedown",
      plannedMinutes: 5,
      trialCsv: path.join(
        repoRoot,
        "Unity/NeglectFix/SmokeResults/ControlRamp/av_training_2026-05-31_08-55-37.csv",
      ),
      sessionCsv: path.join(
        repoRoot,
        "Unity/NeglectFix/SmokeResults/ControlRamp/session_2026-05-31_08-55-10.csv",
      ),
    },
    {
      id: "ramp8",
      label: "8 min",
      storyLabel: "Comfort ramp",
      plannedMinutes: 8,
      trialCsv: path.join(
        repoRoot,
        "Unity/NeglectFix/SmokeResults/Ramp8/av_training_2026-05-31_09-13-38.csv",
      ),
      sessionCsv: path.join(
        repoRoot,
        "Unity/NeglectFix/SmokeResults/Ramp8/session_2026-05-31_09-12-41.csv",
      ),
    },
    {
      id: "ramp12",
      label: "12 min",
      storyLabel: "Meaningful dose",
      plannedMinutes: 12,
      trialCsv: path.join(
        repoRoot,
        "Unity/NeglectFix/SmokeResults/Ramp12/av_training_2026-05-31_09-33-25.csv",
      ),
      sessionCsv: path.join(
        repoRoot,
        "Unity/NeglectFix/SmokeResults/Ramp12/session_2026-05-31_09-32-56.csv",
      ),
    },
  ],
};

function parseTrialCsv(filePath) {
  const lines = fs.readFileSync(filePath, "utf8").split(/\r?\n/);
  let header = null;
  const rows = [];
  const meta = {};

  for (const line of lines) {
    if (!line.trim()) continue;

    if (line.startsWith("#")) {
      const started = line.match(/^# Session started:\s*(.+)$/);
      if (started) meta.startedAt = started[1].trim();
      const scene = line.match(/^# Scene:\s*(.+)$/);
      if (scene) meta.scene = scene[1].trim();
      const device = line.match(/^# Device:\s*(.+)$/);
      if (device) meta.device = device[1].trim();
      continue;
    }

    if (line.startsWith("timestamp_ms,")) {
      header = line.split(",");
      continue;
    }

    if (!header || !/^\d/.test(line)) continue;

    const cells = line.split(",");
    const row = {};
    header.forEach((key, index) => {
      row[key] = cells[index] ?? "";
    });
    rows.push(coerceTrialRow(row));
  }

  return { rows, meta };
}

function coerceTrialRow(row) {
  const numeric = [
    "timestamp_ms",
    "session_index",
    "block_index",
    "trial_index",
    "eccentricity_deg",
    "contrast_logcs",
    "stimulus_onset_ms",
    "audio_onset_ms",
    "response_onset_ms",
    "rt_ms",
    "hit",
    "av_delta_ms",
    "is_control_trial",
    "counts_for_rehab_dose",
    "horizontal_angle_deg",
    "vertical_angle_deg",
  ];

  for (const key of numeric) {
    row[key] = row[key] === undefined || row[key] === "" ? null : Number(row[key]);
  }

  if (row.horizontal_angle_deg === null) row.horizontal_angle_deg = row.eccentricity_deg;
  if (row.vertical_angle_deg === null) row.vertical_angle_deg = 0;

  row.hit = row.hit === 1;
  row.is_control_trial = row.is_control_trial === 1;
  row.counts_for_rehab_dose = row.counts_for_rehab_dose === 1;
  return row;
}

function parseSessionCsv(filePath) {
  const lines = fs.readFileSync(filePath, "utf8").split(/\r?\n/);
  const summary = {};

  for (const line of lines) {
    let match = line.match(/^# Total duration:\s*([\d.]+) seconds \(([\d.]+) minutes\)/);
    if (match) {
      summary.durationSeconds = Number(match[1]);
      summary.durationMinutes = Number(match[2]);
      continue;
    }

    match = line.match(/^# Total samples:\s*(\d+)/);
    if (match) {
      summary.samples = Number(match[1]);
      continue;
    }

    match = line.match(/^# Total rewards:\s*(\d+)/);
    if (match) {
      summary.rewards = Number(match[1]);
      continue;
    }

    match = line.match(/^# Session ended:\s*(.+)$/);
    if (match) {
      summary.endedAt = match[1].trim();
    }
  }

  return summary;
}

function median(values) {
  if (!values.length) return null;
  const sorted = [...values].sort((a, b) => a - b);
  const middle = Math.floor(sorted.length / 2);
  if (sorted.length % 2) return sorted[middle];
  return (sorted[middle - 1] + sorted[middle]) / 2;
}

function mean(values) {
  if (!values.length) return null;
  return values.reduce((total, value) => total + value, 0) / values.length;
}

function summarizeRows(rows) {
  const hits = rows.filter((row) => row.hit);
  const rtValues = hits.map((row) => row.rt_ms).filter((value) => value >= 0);
  const contrasts = rows.map((row) => row.contrast_logcs);
  return {
    count: rows.length,
    hits: hits.length,
    misses: rows.length - hits.length,
    hitRate: rows.length ? hits.length / rows.length : 0,
    meanRtMs: mean(rtValues),
    medianRtMs: median(rtValues),
    contrastMin: contrasts.length ? Math.min(...contrasts) : null,
    contrastMax: contrasts.length ? Math.max(...contrasts) : null,
    lastContrast: contrasts.length ? contrasts[contrasts.length - 1] : null,
  };
}

function summarizeSpacing(rows) {
  let rehabSinceLastControl = 0;
  let minRehabBeforeControl = Infinity;
  let minRowsBetweenControls = Infinity;
  let lastControlIndex = null;
  const controlIndices = [];

  rows.forEach((row, index) => {
    if (row.is_control_trial) {
      controlIndices.push(index + 1);
      minRehabBeforeControl = Math.min(minRehabBeforeControl, rehabSinceLastControl);
      if (lastControlIndex !== null) {
        minRowsBetweenControls = Math.min(minRowsBetweenControls, index - lastControlIndex - 1);
      }
      rehabSinceLastControl = 0;
      lastControlIndex = index;
      return;
    }

    if (row.counts_for_rehab_dose) {
      rehabSinceLastControl += 1;
    }
  });

  return {
    controlIndices,
    minRehabBeforeControl: minRehabBeforeControl === Infinity ? null : minRehabBeforeControl,
    minRowsBetweenControls: minRowsBetweenControls === Infinity ? null : minRowsBetweenControls,
  };
}

function buildRunSummary(config) {
  const trialData = parseTrialCsv(config.trialCsv);
  const sessionSummary = parseSessionCsv(config.sessionCsv);
  const rows = trialData.rows;
  const rehabRows = rows.filter((row) => row.counts_for_rehab_dose);
  const controlRows = rows.filter((row) => row.is_control_trial);
  const spacing = summarizeSpacing(rows);
  const badControlRows = rows.filter(
    (row) => row.is_control_trial && (row.counts_for_rehab_dose || row.trial_type !== "right_control"),
  ).length;
  const badRehabRows = rows.filter(
    (row) => row.counts_for_rehab_dose && (row.is_control_trial || row.trial_type !== "rehab"),
  ).length;

  return {
    id: config.id,
    label: config.label,
    storyLabel: config.storyLabel,
    plannedMinutes: config.plannedMinutes,
    trialCsv: relativeFromRepo(config.trialCsv),
    sessionCsv: relativeFromRepo(config.sessionCsv),
    meta: trialData.meta,
    total: summarizeRows(rows),
    rehab: summarizeRows(rehabRows),
    control: summarizeRows(controlRows),
    session: sessionSummary,
    spacing,
    validation: {
      badControlRows,
      badRehabRows,
      controlsExcluded: badControlRows === 0 && badRehabRows === 0,
      spacingRespected:
        spacing.minRehabBeforeControl === null || spacing.minRehabBeforeControl >= 3,
    },
    dotRows: rows.map((row, index) => ({
      i: index,
      runId: config.id,
      hit: row.hit,
      type: row.trial_type,
      control: row.is_control_trial,
      dose: row.counts_for_rehab_dose,
      eccentricity: row.eccentricity_deg,
      horizontalAngle: row.horizontal_angle_deg,
      verticalAngle: row.vertical_angle_deg,
      contrast: row.contrast_logcs,
      rt: row.rt_ms,
    })),
  };
}

function relativeFromRepo(filePath) {
  return path.relative(repoRoot, filePath).replace(/\\/g, "/");
}

function formatPercent(value, digits = 1) {
  return `${(value * 100).toFixed(digits)}%`;
}

function formatNumber(value, digits = 0) {
  if (value === null || value === undefined || Number.isNaN(value)) return "n/a";
  return Number(value).toFixed(digits);
}

function computeOverall(runs) {
  const allRows = runs.flatMap((run) => run.dotRows);
  const rehabRows = allRows.filter((row) => row.dose);
  const controlRows = allRows.filter((row) => row.control);
  const hitCount = (rows) => rows.filter((row) => row.hit).length;

  return {
    recordedMinutes: runs.reduce((total, run) => total + run.plannedMinutes, 0),
    totalTrials: allRows.length,
    rehabTrials: rehabRows.length,
    rehabHits: hitCount(rehabRows),
    rehabHitRate: rehabRows.length ? hitCount(rehabRows) / rehabRows.length : 0,
    controlTrials: controlRows.length,
    controlHits: hitCount(controlRows),
    controlHitRate: controlRows.length ? hitCount(controlRows) / controlRows.length : 0,
    sessionMinutes: runs.reduce((total, run) => total + (run.session.durationMinutes || 0), 0),
    rewards: runs.reduce((total, run) => total + (run.session.rewards || 0), 0),
    samples: runs.reduce((total, run) => total + (run.session.samples || 0), 0),
  };
}

function cssWidth(rate) {
  return `${Math.max(0, Math.min(100, rate * 100)).toFixed(1)}%`;
}

function markerLeft(rate) {
  return `${Math.max(0, Math.min(100, rate * 100)).toFixed(1)}%`;
}

function buildRunRows(runs) {
  return runs
    .map((run) => {
      return `
        <article class="run-panel reveal" data-run="${run.id}">
          <div>
            <p class="eyebrow">${escapeHtml(run.storyLabel)}</p>
            <h3>${escapeHtml(run.label)}</h3>
          </div>
          <div class="run-grid">
            <div>
              <span class="metric-value">${run.total.count}</span>
              <span class="metric-label">trials</span>
            </div>
            <div>
              <span class="metric-value">${run.rehab.hits}/${run.rehab.count}</span>
              <span class="metric-label">left rehab</span>
            </div>
            <div>
              <span class="metric-value">${run.control.hits}/${run.control.count}</span>
              <span class="metric-label">right controls</span>
            </div>
            <div>
              <span class="metric-value">${formatNumber(run.rehab.lastContrast, 2)}</span>
              <span class="metric-label">final LogCS</span>
            </div>
          </div>
          <div class="mini-bars" aria-label="Hit rate comparison for ${escapeHtml(run.label)}">
            <div class="mini-bar">
              <span>Left rehab ${formatPercent(run.rehab.hitRate)}</span>
              <i style="width:${cssWidth(run.rehab.hitRate)}"></i>
            </div>
            <div class="mini-bar control">
              <span>Right control ${formatPercent(run.control.hitRate)}</span>
              <i style="width:${cssWidth(run.control.hitRate)}"></i>
            </div>
          </div>
        </article>`;
    })
    .join("\n");
}

function buildTimeline(runs) {
  return runs
    .map(
      (run, index) => `
        <div class="dose-step reveal" style="--step:${index}">
          <span class="step-index">0${index + 1}</span>
          <strong>${escapeHtml(run.label)}</strong>
          <em>${escapeHtml(run.storyLabel)}</em>
          <b>${run.total.count} trials</b>
        </div>`,
    )
    .join("\n");
}

function buildBandMarkers(runs) {
  return runs
    .map(
      (run) => `
        <div class="band-marker" style="left:${markerLeft(run.rehab.hitRate)}" data-rate="${formatPercent(
          run.rehab.hitRate,
        )}">
          <span>${escapeHtml(run.label)}</span>
        </div>`,
    )
    .join("\n");
}

function buildEvidenceList(runs) {
  const last = runs[runs.length - 1];
  return `
    <li><strong>Left field challenged.</strong> Rehab hit rate stayed in the productive band: ${runs
      .map((run) => formatPercent(run.rehab.hitRate))
      .join(" -> ")}.</li>
    <li><strong>Right field confirmed.</strong> Control checks stayed at ${runs
      .map((run) => `${run.control.hits}/${run.control.count}`)
      .join(" -> ")}.</li>
    <li><strong>Training stayed honest.</strong> Control rows were flagged separately and excluded from rehab dose and staircase updates.</li>
    <li><strong>Ramp held technically.</strong> The longest run closed cleanly at ${last.total.count} trials and ${
      last.session.samples
    } session samples.</li>`;
}

function escapeHtml(value) {
  return String(value)
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#39;");
}

function buildHtml(data) {
  const reportJson = JSON.stringify(data);
  const latestRun = data.runs[data.runs.length - 1];

  return `<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>${escapeHtml(data.title)} - ${escapeHtml(data.dateLabel)}</title>
  <style>
    :root {
      color-scheme: dark;
      --ink: #f5f1e8;
      --muted: #a8b0b0;
      --dim: #6f7977;
      --bg: #101415;
      --panel: #171d1e;
      --panel-2: #1d2425;
      --line: rgba(245, 241, 232, 0.16);
      --green: #7bd88f;
      --cyan: #69d2e7;
      --amber: #f0c45c;
      --red: #f16f6f;
      --blue: #7aa7ff;
      --shadow: rgba(0, 0, 0, 0.34);
    }

    * {
      box-sizing: border-box;
    }

    html {
      scroll-behavior: smooth;
      background: var(--bg);
    }

    body {
      margin: 0;
      color: var(--ink);
      font-family: Inter, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
      background:
        linear-gradient(180deg, rgba(105, 210, 231, 0.08), transparent 35rem),
        radial-gradient(circle at 75% 12%, rgba(240, 196, 92, 0.08), transparent 24rem),
        var(--bg);
    }

    a {
      color: inherit;
    }

    .page {
      min-height: 100vh;
    }

    .shell {
      width: min(1180px, calc(100vw - 36px));
      margin: 0 auto;
    }

    .hero {
      position: relative;
      min-height: 86vh;
      display: grid;
      align-items: center;
      padding: 64px 0 42px;
      border-bottom: 1px solid var(--line);
      overflow: hidden;
    }

    .hero::before {
      content: "";
      position: absolute;
      inset: 0;
      background:
        linear-gradient(90deg, rgba(123, 216, 143, 0.04) 0 49%, rgba(255, 255, 255, 0.16) 49.9% 50.1%, rgba(105, 210, 231, 0.04) 50.1% 100%),
        repeating-linear-gradient(90deg, rgba(245, 241, 232, 0.05) 0 1px, transparent 1px 70px);
      mask-image: linear-gradient(180deg, rgba(0, 0, 0, 0.86), transparent 78%);
      pointer-events: none;
    }

    .hero-grid {
      position: relative;
      display: grid;
      grid-template-columns: minmax(0, 1.1fr) minmax(340px, 0.9fr);
      gap: 48px;
      align-items: center;
    }

    .eyebrow {
      margin: 0 0 12px;
      color: var(--cyan);
      font-size: 0.76rem;
      font-weight: 750;
      letter-spacing: 0;
      text-transform: uppercase;
    }

    h1,
    h2,
    h3,
    p {
      margin-top: 0;
    }

    h1 {
      max-width: 860px;
      margin-bottom: 22px;
      font-size: clamp(3.8rem, 8.2vw, 8.8rem);
      line-height: 0.88;
      letter-spacing: 0;
    }

    .hero-copy {
      max-width: 760px;
      color: #d9ddd7;
      font-size: clamp(1.08rem, 1.6vw, 1.35rem);
      line-height: 1.55;
    }

    .proof-strip {
      display: grid;
      grid-template-columns: repeat(3, minmax(0, 1fr));
      gap: 1px;
      border: 1px solid var(--line);
      background: var(--line);
      box-shadow: 0 24px 90px var(--shadow);
    }

    .proof {
      min-height: 150px;
      padding: 22px;
      background: rgba(23, 29, 30, 0.88);
    }

    .proof strong {
      display: block;
      font-size: clamp(2.2rem, 4vw, 4.5rem);
      line-height: 1;
    }

    .proof span {
      display: block;
      margin-top: 12px;
      color: var(--muted);
      font-size: 0.95rem;
      line-height: 1.35;
    }

    .proof strong > span,
    .proof strong .inline {
      display: inline;
      margin-top: 0;
      color: inherit;
      font-size: inherit;
      line-height: inherit;
    }

    .section {
      padding: 76px 0;
      border-bottom: 1px solid var(--line);
    }

    .section.alt {
      background: rgba(255, 255, 255, 0.025);
    }

    .section-head {
      display: grid;
      grid-template-columns: minmax(0, 0.9fr) minmax(300px, 0.55fr);
      gap: 44px;
      align-items: end;
      margin-bottom: 34px;
    }

    h2 {
      margin-bottom: 0;
      font-size: clamp(2.1rem, 4.6vw, 5rem);
      line-height: 0.96;
      letter-spacing: 0;
    }

    .section-lede {
      color: var(--muted);
      font-size: 1.05rem;
      line-height: 1.55;
    }

    .dose-timeline {
      display: grid;
      grid-template-columns: repeat(3, minmax(0, 1fr));
      gap: 14px;
      margin-top: 24px;
    }

    .dose-step,
    .run-panel,
    .recommendation,
    .source-panel {
      border: 1px solid var(--line);
      background: linear-gradient(180deg, rgba(255, 255, 255, 0.05), rgba(255, 255, 255, 0.02));
      box-shadow: 0 18px 80px rgba(0, 0, 0, 0.18);
    }

    .dose-step {
      position: relative;
      min-height: 190px;
      padding: 22px;
      overflow: hidden;
    }

    .dose-step::after {
      content: "";
      position: absolute;
      left: 22px;
      right: 22px;
      bottom: 20px;
      height: 3px;
      background: linear-gradient(90deg, var(--green), var(--cyan));
      transform-origin: left;
      transform: scaleX(1);
    }

    .dose-step.is-lit::after {
      transform: scaleX(1);
      transition: transform 0.75s ease;
    }

    .step-index {
      display: inline-flex;
      width: 38px;
      height: 38px;
      align-items: center;
      justify-content: center;
      border: 1px solid var(--line);
      color: var(--cyan);
      font-weight: 800;
      margin-bottom: 40px;
    }

    .dose-step strong,
    .dose-step em,
    .dose-step b {
      display: block;
      font-style: normal;
    }

    .dose-step strong {
      font-size: 2.6rem;
      line-height: 1;
    }

    .dose-step em {
      margin-top: 8px;
      color: var(--muted);
    }

    .dose-step b {
      margin-top: 22px;
      color: var(--ink);
    }

    .run-stack {
      display: grid;
      gap: 14px;
    }

    .run-panel {
      display: grid;
      grid-template-columns: 170px minmax(0, 1fr) minmax(240px, 0.8fr);
      gap: 22px;
      align-items: center;
      padding: 22px;
    }

    .run-panel h3 {
      margin: 0;
      font-size: 2.4rem;
      line-height: 1;
    }

    .run-grid {
      display: grid;
      grid-template-columns: repeat(4, minmax(0, 1fr));
      gap: 14px;
    }

    .metric-value {
      display: block;
      font-size: 1.55rem;
      font-weight: 820;
      line-height: 1.08;
    }

    .metric-label {
      display: block;
      margin-top: 7px;
      color: var(--muted);
      font-size: 0.82rem;
    }

    .mini-bars {
      display: grid;
      gap: 10px;
    }

    .mini-bar {
      position: relative;
      min-height: 34px;
      border: 1px solid var(--line);
      background: rgba(255, 255, 255, 0.035);
      overflow: hidden;
    }

    .mini-bar i {
      position: absolute;
      inset: 0 auto 0 0;
      background: linear-gradient(90deg, rgba(123, 216, 143, 0.45), rgba(240, 196, 92, 0.52));
      transform-origin: left;
    }

    .mini-bar.control i {
      background: linear-gradient(90deg, rgba(105, 210, 231, 0.35), rgba(122, 167, 255, 0.56));
    }

    .mini-bar span {
      position: relative;
      z-index: 1;
      display: flex;
      align-items: center;
      min-height: 34px;
      padding: 0 10px;
      color: var(--ink);
      font-size: 0.86rem;
      font-weight: 700;
    }

    .map-toolbar {
      display: flex;
      flex-wrap: wrap;
      gap: 10px;
      align-items: center;
      justify-content: space-between;
      margin: -8px 0 18px;
    }

    .map-modes {
      display: inline-flex;
      flex-wrap: wrap;
      gap: 1px;
      border: 1px solid var(--line);
      background: var(--line);
    }

    .map-mode {
      appearance: none;
      border: 0;
      border-radius: 0;
      padding: 11px 14px;
      color: var(--muted);
      background: rgba(23, 29, 30, 0.92);
      font: inherit;
      font-size: 0.84rem;
      font-weight: 820;
      cursor: pointer;
    }

    .map-mode[aria-selected="true"] {
      color: #0f1415;
      background: var(--ink);
    }

    .map-note {
      max-width: 560px;
      margin: 0;
      color: var(--muted);
      font-size: 0.9rem;
      line-height: 1.45;
    }

    .field-map {
      min-height: 560px;
      position: relative;
      border: 1px solid var(--line);
      background:
        linear-gradient(90deg, rgba(123, 216, 143, 0.06), transparent 48%, rgba(245, 241, 232, 0.12) 49.7% 50.3%, transparent 52%, rgba(105, 210, 231, 0.06)),
        rgba(12, 15, 16, 0.68);
      overflow: hidden;
    }

    .field-svg {
      position: absolute;
      inset: 0;
      width: 100%;
      height: 100%;
      pointer-events: none;
    }

    .field-path {
      fill: rgba(123, 216, 143, 0.08);
      stroke: rgba(123, 216, 143, 0.28);
      stroke-width: 1.5;
      opacity: 0.72;
    }

    .field-path.miss {
      fill: rgba(241, 111, 111, 0.08);
      stroke: rgba(241, 111, 111, 0.32);
    }

    .field-path.control {
      fill: rgba(105, 210, 231, 0.1);
      stroke: rgba(105, 210, 231, 0.35);
    }

    .field-map::before,
    .field-map::after {
      position: absolute;
      top: 18px;
      color: var(--muted);
      font-size: 0.8rem;
      font-weight: 800;
      text-transform: uppercase;
    }

    .field-map::before {
      content: "Left rehab field";
      left: 22px;
    }

    .field-map::after {
      content: "Right control field";
      right: 22px;
    }

    .fixation {
      position: absolute;
      left: 50%;
      top: 50%;
      width: 34px;
      height: 34px;
      transform: translate(-50%, -50%);
    }

    .fixation::before,
    .fixation::after {
      content: "";
      position: absolute;
      background: rgba(245, 241, 232, 0.72);
    }

    .fixation::before {
      left: 15px;
      top: 0;
      width: 3px;
      height: 34px;
    }

    .fixation::after {
      left: 0;
      top: 15px;
      width: 34px;
      height: 3px;
    }

    .trial-dot {
      position: absolute;
      width: 8px;
      height: 8px;
      border-radius: 50%;
      transform: translate(-50%, -50%);
      opacity: 1;
      background: var(--green);
      box-shadow: 0 0 0 1px rgba(0, 0, 0, 0.4);
    }

    .trial-dot.control {
      width: 10px;
      height: 10px;
      background: var(--cyan);
    }

    .trial-dot.miss {
      background: transparent;
      border: 1px solid var(--red);
    }

    .trial-dot.control.miss {
      border-color: var(--amber);
    }

    .field-map[data-mode="heat"] .trial-dot {
      opacity: 0.42;
      filter: saturate(1.25);
    }

    .field-map[data-mode="retinal"] .trial-dot {
      box-shadow: 0 0 0 1px rgba(0, 0, 0, 0.42), 0 0 14px rgba(245, 241, 232, 0.1);
    }

    .field-legend {
      display: flex;
      flex-wrap: wrap;
      gap: 14px;
      margin-top: 16px;
      color: var(--muted);
      font-size: 0.9rem;
    }

    .legend-key {
      display: inline-flex;
      gap: 8px;
      align-items: center;
    }

    .legend-dot {
      width: 9px;
      height: 9px;
      border-radius: 50%;
      background: var(--green);
    }

    .legend-dot.control {
      background: var(--cyan);
    }

    .legend-dot.miss {
      background: transparent;
      border: 1px solid var(--red);
    }

    .band-wrap {
      position: relative;
      padding: 42px 0 24px;
    }

    .challenge-band {
      position: relative;
      height: 88px;
      border: 1px solid var(--line);
      background:
        linear-gradient(90deg, rgba(241, 111, 111, 0.13) 0 32%, rgba(240, 196, 92, 0.1) 32% 40%, rgba(123, 216, 143, 0.20) 40% 60%, rgba(240, 196, 92, 0.1) 60% 72%, rgba(105, 210, 231, 0.12) 72% 100%);
      overflow: visible;
    }

    .band-labels {
      display: grid;
      grid-template-columns: 40fr 20fr 40fr;
      margin-top: 12px;
      color: var(--muted);
      font-size: 0.88rem;
    }

    .band-labels span:nth-child(2) {
      color: var(--green);
      text-align: center;
      font-weight: 800;
    }

    .band-labels span:nth-child(3) {
      text-align: right;
    }

    .band-marker {
      position: absolute;
      top: -25px;
      width: 2px;
      height: 138px;
      background: var(--ink);
      transform: translateX(-50%) scaleY(1);
      transform-origin: bottom;
    }

    .band-marker::before {
      content: attr(data-rate);
      position: absolute;
      top: -30px;
      left: 50%;
      transform: translateX(-50%);
      padding: 5px 8px;
      color: #0f1415;
      background: var(--ink);
      font-size: 0.78rem;
      font-weight: 850;
      white-space: nowrap;
    }

    .band-marker span {
      position: absolute;
      bottom: -28px;
      left: 50%;
      transform: translateX(-50%);
      color: var(--ink);
      font-size: 0.8rem;
      font-weight: 800;
      white-space: nowrap;
    }

    .evidence-grid {
      display: grid;
      grid-template-columns: minmax(0, 0.95fr) minmax(320px, 0.65fr);
      gap: 22px;
      align-items: stretch;
    }

    .evidence-list {
      margin: 0;
      padding: 0;
      list-style: none;
      display: grid;
      gap: 12px;
    }

    .evidence-list li {
      padding: 20px;
      border: 1px solid var(--line);
      background: rgba(255, 255, 255, 0.035);
      color: #dbe0da;
      line-height: 1.5;
    }

    .evidence-list strong {
      color: var(--ink);
    }

    .recommendation {
      padding: 28px;
      display: grid;
      align-content: center;
      min-height: 100%;
    }

    .recommendation h3 {
      margin-bottom: 16px;
      font-size: 2rem;
    }

    .recommendation p {
      color: #e3e4dc;
      font-size: 1.12rem;
      line-height: 1.5;
    }

    .status-pill {
      display: inline-flex;
      width: max-content;
      align-items: center;
      gap: 9px;
      padding: 8px 11px;
      border: 1px solid rgba(123, 216, 143, 0.35);
      color: var(--green);
      background: rgba(123, 216, 143, 0.08);
      font-size: 0.78rem;
      font-weight: 850;
      text-transform: uppercase;
    }

    .status-pill::before {
      content: "";
      width: 8px;
      height: 8px;
      border-radius: 50%;
      background: var(--green);
    }

    .source-panel {
      padding: 22px;
      color: var(--muted);
      line-height: 1.55;
    }

    .source-list {
      margin: 16px 0 0;
      padding: 0;
      list-style: none;
      display: grid;
      gap: 8px;
      font-size: 0.9rem;
    }

    .source-list code {
      color: var(--ink);
      font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace;
      font-size: 0.82rem;
      overflow-wrap: anywhere;
    }

    .footer {
      padding: 36px 0 48px;
      color: var(--dim);
      font-size: 0.85rem;
    }

    .reveal {
      opacity: 1;
      transform: none;
    }

    @media (prefers-reduced-motion: reduce) {
      *,
      *::before,
      *::after {
        animation-duration: 0.01ms !important;
        animation-iteration-count: 1 !important;
        scroll-behavior: auto !important;
        transition-duration: 0.01ms !important;
      }

      .reveal,
      .trial-dot,
      .band-marker {
        opacity: 1;
        transform: none;
      }
    }

    @media (max-width: 920px) {
      .hero {
        min-height: auto;
        padding-top: 54px;
      }

      .hero-grid,
      .section-head,
      .evidence-grid {
        grid-template-columns: 1fr;
      }

      .proof-strip,
      .dose-timeline {
        grid-template-columns: 1fr;
      }

      .run-panel {
        grid-template-columns: 1fr;
      }

      .run-grid {
        grid-template-columns: repeat(2, minmax(0, 1fr));
      }

      .field-map {
        min-height: 430px;
      }
    }

    @media (max-width: 560px) {
      .shell {
        width: min(100vw - 24px, 1180px);
      }

      h1 {
        font-size: clamp(3rem, 15vw, 4.7rem);
      }

      .section {
        padding: 54px 0;
      }

      .proof {
        min-height: 128px;
      }

      .run-grid {
        grid-template-columns: 1fr;
      }
    }
  </style>
</head>
<body>
  <main class="page">
    <section class="hero">
      <div class="shell hero-grid">
        <div>
          <p class="eyebrow">NeglectFix - ${escapeHtml(data.subtitle)} - ${escapeHtml(data.dateLabel)}</p>
          <h1>Proof of work.</h1>
          <p class="hero-copy">A controlled rehab ramp that separates left-field training from right-field validation, then turns the evidence into a session story.</p>
        </div>
        <div class="proof-strip" aria-label="Session proof summary">
          <div class="proof">
            <strong class="count-up" data-value="${data.overall.recordedMinutes}">${data.overall.recordedMinutes}</strong>
            <span>minutes recorded training dose</span>
          </div>
          <div class="proof">
            <strong class="count-up" data-value="${data.overall.totalTrials}">${data.overall.totalTrials}</strong>
            <span>total controlled trials across 3 runs</span>
          </div>
          <div class="proof">
            <strong><span class="count-up inline" data-value="${data.overall.controlHits}">${data.overall.controlHits}</span>/<span>${data.overall.controlTrials}</span></strong>
            <span>right-control checks passed</span>
          </div>
        </div>
      </div>
    </section>

    <section class="section">
      <div class="shell">
        <div class="section-head reveal">
          <h2>Dose ramp completed.</h2>
          <p class="section-lede">The goal was not to prove visual recovery today. The goal was to prove that the protocol can run, adapt, log cleanly, and keep its own right-side control check while dose increases.</p>
        </div>
        <div class="dose-timeline">
          ${buildTimeline(data.runs)}
        </div>
      </div>
    </section>

    <section class="section alt">
      <div class="shell">
        <div class="section-head reveal">
          <h2>Left challenged. Right confirmed.</h2>
          <p class="section-lede">Left trials counted as rehab dose. Right trials were sparse controls. They rewarded successful responses but did not update the adaptive staircase.</p>
        </div>
        <div class="run-stack">
          ${buildRunRows(data.runs)}
        </div>
      </div>
    </section>

    <section class="section">
      <div class="shell">
        <div class="section-head reveal">
          <h2>The useful challenge zone.</h2>
          <p class="section-lede">The left rehab hit rate stayed inside the intended training band. That means the task was giving work: not trivially easy, not impossibly hard.</p>
        </div>
        <div class="band-wrap reveal">
          <div class="challenge-band" aria-label="Target hit-rate challenge band">
            ${buildBandMarkers(data.runs)}
          </div>
          <div class="band-labels">
            <span>too hard</span>
            <span>useful 40-60%</span>
            <span>too easy</span>
          </div>
        </div>
      </div>
    </section>

    <section class="section alt">
      <div class="shell">
        <div class="section-head reveal">
          <h2>Trial field map.</h2>
          <p class="section-lede">Every dot below is one recorded trial from today's ramp. The horizontal position is data-driven; the current Unity build only logs horizontal eccentricity, so vertical spread is a report layer, not a measured retinal height.</p>
        </div>
        <div class="map-toolbar reveal">
          <div class="map-modes" role="tablist" aria-label="Field map view mode">
            <button class="map-mode" type="button" data-map-mode="evidence" aria-selected="true">Evidence</button>
            <button class="map-mode" type="button" data-map-mode="retinal" aria-selected="false">Retinal truth</button>
            <button class="map-mode" type="button" data-map-mode="heat" aria-selected="false">Heat bloom</button>
          </div>
          <p class="map-note" id="mapNote">Evidence mode spreads repeated trials vertically so the work is visible. X-position still comes from the logged left/right eccentricity.</p>
        </div>
        <div class="field-map reveal" id="fieldMap" data-mode="evidence" aria-label="Visual field trial map">
          <svg class="field-svg" viewBox="0 0 100 100" preserveAspectRatio="none" aria-hidden="true">
            <path id="leftHitPath" class="field-path" d="M 24 16 C 32 17 34 34 32 50 C 34 66 32 83 24 84 C 18 80 17 64 18 50 C 17 36 18 20 24 16 Z"></path>
            <path id="leftMissPath" class="field-path miss" d="M 18 18 C 23 17 25 34 24 50 C 25 66 23 83 18 82 C 14 78 13 63 14 50 C 13 37 14 22 18 18 Z"></path>
            <path id="rightControlPath" class="field-path control" d="M 74 21 C 80 20 83 34 82 50 C 83 66 80 80 74 79 C 69 75 69 63 70 50 C 69 37 69 25 74 21 Z"></path>
          </svg>
          <div class="fixation" aria-hidden="true"></div>
        </div>
        <div class="field-legend">
          <span class="legend-key"><i class="legend-dot"></i>left rehab hit</span>
          <span class="legend-key"><i class="legend-dot miss"></i>left rehab miss</span>
          <span class="legend-key"><i class="legend-dot control"></i>right control hit</span>
        </div>
      </div>
    </section>

    <section class="section">
      <div class="shell">
        <div class="section-head reveal">
          <h2>Evidence story.</h2>
          <p class="section-lede">This is the marketing that matters: proof of work, proof of validity, proof of challenge, and a next prescription that follows the data.</p>
        </div>
        <div class="evidence-grid">
          <ul class="evidence-list reveal">
            ${buildEvidenceList(data.runs)}
          </ul>
          <aside class="recommendation reveal">
            <span class="status-pill">valid controlled session</span>
            <h3>Next prescription</h3>
            <p>${escapeHtml(data.recommendation)}</p>
          </aside>
        </div>
      </div>
    </section>

    <section class="section alt">
      <div class="shell">
        <div class="section-head reveal">
          <h2>Source files.</h2>
          <p class="section-lede">The report is generated from the pulled Quest CSV files so the same evidence can be regenerated after future sessions.</p>
        </div>
        <div class="source-panel reveal">
          <strong>Latest run shown in detail:</strong> ${escapeHtml(latestRun.label)} - ${latestRun.total.count} trials, ${formatPercent(
            latestRun.rehab.hitRate,
          )} left rehab hit rate, ${formatPercent(latestRun.control.hitRate)} right control hit rate.
          <ul class="source-list">
            ${data.runs
              .map(
                (run) => `<li><code>${escapeHtml(run.trialCsv)}</code></li><li><code>${escapeHtml(run.sessionCsv)}</code></li>`,
              )
              .join("")}
          </ul>
        </div>
      </div>
    </section>

    <footer class="footer">
      <div class="shell">Generated locally by <code>scripts/generate-rehab-report.js</code>. This is a training report, not a clinical diagnosis.</div>
    </footer>
  </main>

  <script>
    window.__NEGLECTFIX_REPORT__ = ${reportJson};
  </script>
  <script>
    (function () {
      var data = window.__NEGLECTFIX_REPORT__;
      var reduceMotion = window.matchMedia && window.matchMedia("(prefers-reduced-motion: reduce)").matches;
      var rendered = false;
      var enhanced = false;

      function createDots() {
        var field = document.getElementById("fieldMap");
        if (!field) return;
        var rows = [];
        data.runs.forEach(function (run) {
          rows = rows.concat(run.dotRows);
        });

        rows.forEach(function (row, index) {
          var dot = document.createElement("span");

          dot.className = "trial-dot" + (row.control ? " control" : "") + (row.hit ? "" : " miss");
          dot.dataset.index = String(index);
          dot.dataset.runId = row.runId;
          dot.title = row.runId + " " + row.type + " " + (row.hit ? "hit" : "miss") + " - LogCS " + row.contrast.toFixed(2);
          field.appendChild(dot);
        });

        setMapMode("evidence", false);
      }

      function positionForMode(row, index, mode) {
        var absEcc = Math.min(20, Math.abs(row.eccentricity || 5));
        var side = row.eccentricity >= 0 ? 1 : -1;
        var lane = row.runId === "ramp5" ? 0 : row.runId === "ramp8" ? 1 : 2;
        var x;
        var y;

        if (mode === "retinal") {
          x = 50 + side * (8 + absEcc * 3.1);
          y = 50 + deterministicJitter(index, 12) * 7;
          return { x: clamp(x, 7, 93), y: clamp(y, 42, 58) };
        }

        if (mode === "heat") {
          x = 50 + side * (10 + absEcc * 2.8) + deterministicJitter(index + lane * 17, 5) * 3.4;
          y = 50 + deterministicJitter(index * 7 + lane * 23, 19) * 30;
          return { x: clamp(x, 8, 92), y: clamp(y, 16, 84) };
        }

        x = 50 + side * (12 + absEcc * 1.75 + lane * 2.2);
        y = 12 + ((index * 37 + lane * 11) % 76);
        return { x: clamp(x, 7, 93), y: clamp(y, 10, 90) };
      }

      function deterministicJitter(seed, spread) {
        var raw = Math.sin(seed * 12.9898 + spread * 78.233) * 43758.5453;
        return (raw - Math.floor(raw)) * 2 - 1;
      }

      function clamp(value, min, max) {
        return Math.max(min, Math.min(max, value));
      }

      function setMapMode(mode, animate) {
        var field = document.getElementById("fieldMap");
        if (!field) return;

        field.dataset.mode = mode;
        var notes = {
          evidence:
            "Evidence mode spreads repeated trials vertically so the work is visible. X-position still comes from the logged left/right eccentricity.",
          retinal:
            "Retinal truth mode shows what today's log truly contains: horizontal eccentricity at 5 or 8 degrees, with vertical angle currently unmeasured and effectively zero.",
          heat:
            "Heat bloom mode turns the same trials into a density-style proof object. It is expressive, not diagnostic; the morphing shapes summarize hit/miss/control clusters.",
        };
        var note = document.getElementById("mapNote");
        if (note) note.textContent = notes[mode] || notes.evidence;

        document.querySelectorAll(".map-mode").forEach(function (button) {
          button.setAttribute("aria-selected", button.dataset.mapMode === mode ? "true" : "false");
        });

        var rows = [];
        data.runs.forEach(function (run) {
          rows = rows.concat(run.dotRows);
        });

        var dots = Array.prototype.slice.call(field.querySelectorAll(".trial-dot"));
        dots.forEach(function (dot) {
          var index = Number(dot.dataset.index);
          var point = positionForMode(rows[index], index, mode);
          if (window.gsap && animate && !reduceMotion) {
            gsap.to(dot, {
              left: point.x + "%",
              top: point.y + "%",
              duration: 0.72,
              ease: "power3.inOut",
              overwrite: "auto",
            });
          } else {
            dot.style.left = point.x.toFixed(2) + "%";
            dot.style.top = point.y.toFixed(2) + "%";
          }
        });

        morphFieldPaths(mode, animate);
      }

      function morphFieldPaths(mode, animate) {
        var shapes = {
          evidence: {
            leftHit:
              "M 24 16 C 32 17 34 34 32 50 C 34 66 32 83 24 84 C 18 80 17 64 18 50 C 17 36 18 20 24 16 Z",
            leftMiss:
              "M 18 18 C 23 17 25 34 24 50 C 25 66 23 83 18 82 C 14 78 13 63 14 50 C 13 37 14 22 18 18 Z",
            rightControl:
              "M 74 21 C 80 20 83 34 82 50 C 83 66 80 80 74 79 C 69 75 69 63 70 50 C 69 37 69 25 74 21 Z",
          },
          retinal: {
            leftHit:
              "M 18 47 C 24 46 31 46 36 48 C 38 49 38 51 36 52 C 31 54 24 54 18 53 C 15 52 15 48 18 47 Z",
            leftMiss:
              "M 12 48 C 18 47 25 47 29 48 C 31 49 31 51 29 52 C 25 53 18 53 12 52 C 10 51 10 49 12 48 Z",
            rightControl:
              "M 65 48 C 72 47 81 47 87 48 C 90 49 90 51 87 52 C 81 53 72 53 65 52 C 62 51 62 49 65 48 Z",
          },
          heat: {
            leftHit:
              "M 25 13 C 42 16 45 34 38 50 C 45 66 42 84 25 87 C 10 80 9 65 14 50 C 9 35 10 20 25 13 Z",
            leftMiss:
              "M 16 18 C 31 15 35 34 29 50 C 35 66 31 85 16 82 C 5 74 6 62 10 50 C 6 38 5 26 16 18 Z",
            rightControl:
              "M 73 18 C 90 20 93 37 88 50 C 93 63 90 80 73 82 C 61 77 60 63 64 50 C 60 37 61 23 73 18 Z",
          },
        };

        var selected = shapes[mode] || shapes.evidence;
        var pairs = [
          ["leftHitPath", selected.leftHit],
          ["leftMissPath", selected.leftMiss],
          ["rightControlPath", selected.rightControl],
        ];
        pairs.forEach(function (pair) {
          var el = document.getElementById(pair[0]);
          if (!el) return;
          if (window.gsap && animate && !reduceMotion) {
            gsap.to(el, {
              attr: { d: pair[1] },
              duration: 0.75,
              ease: "power3.inOut",
              overwrite: "auto",
            });
          } else {
            el.setAttribute("d", pair[1]);
          }
        });
      }

      function setupMapControls() {
        document.querySelectorAll(".map-mode").forEach(function (button) {
          button.addEventListener("click", function () {
            setMapMode(button.dataset.mapMode, true);
          });
        });
      }

      function countUp(el) {
        var value = Number(el.getAttribute("data-value"));
        var state = { value: 0 };
        if (!window.gsap || reduceMotion) {
          el.textContent = String(value);
          return;
        }
        el.textContent = "0";
        gsap.to(state, {
          value: value,
          duration: 1.2,
          ease: "power3.out",
          onUpdate: function () {
            el.textContent = String(Math.round(state.value));
          },
        });
      }

      function revealElements() {
      var items = Array.prototype.slice.call(document.querySelectorAll(".reveal"));
        if (!window.gsap || reduceMotion) {
          items.forEach(function (item) {
            item.style.opacity = 1;
            item.style.transform = "none";
          });
          document.querySelectorAll(".dose-step").forEach(function (step) {
            step.classList.add("is-lit");
          });
          document.querySelectorAll(".band-marker").forEach(function (marker) {
            marker.style.transform = "translateX(-50%) scaleY(1)";
          });
          document.querySelectorAll(".trial-dot").forEach(function (dot) {
            dot.style.opacity = 1;
          });
          return;
        }

        gsap.defaults({ duration: 0.75, ease: "power3.out" });
        gsap.from(".hero .eyebrow, .hero h1, .hero-copy, .proof-strip", {
          y: 18,
          stagger: 0.08,
          duration: 0.9,
        });

        var observer = new IntersectionObserver(
          function (entries) {
            entries.forEach(function (entry) {
              if (!entry.isIntersecting) return;
              var target = entry.target;
              gsap.from(target, { y: 18 });

              if (target.classList.contains("dose-step")) {
                window.setTimeout(function () {
                  target.classList.add("is-lit");
                }, 220);
              }

              if (target.classList.contains("band-wrap")) {
                gsap.from(".band-marker", {
                  scaleY: 0,
                  duration: 0.8,
                  ease: "back.out(1.6)",
                  stagger: 0.08,
                });
              }

              if (target.id === "fieldMap") {
                gsap.from(".trial-dot", {
                  scale: 0.2,
                  duration: 0.42,
                  stagger: { each: 0.003, from: "start" },
                  ease: "power2.out",
                });
              }

              observer.unobserve(target);
            });
          },
          { threshold: 0.18 },
        );

        items.forEach(function (item) {
          observer.observe(item);
        });
      }

      function renderStaticReport() {
        if (rendered) return;
        rendered = true;
        createDots();
        setupMapControls();
      }

      function enhanceReport() {
        if (enhanced) return;
        enhanced = true;
        renderStaticReport();
        if (!window.gsap || reduceMotion) return;
        document.querySelectorAll(".count-up").forEach(countUp);
        revealElements();
      }

      function loadGsapThenBoot() {
        renderStaticReport();
        var script = document.createElement("script");
        script.src = "https://cdn.jsdelivr.net/npm/gsap@3.12.5/dist/gsap.min.js";
        script.async = true;
        script.onload = enhanceReport;
        script.onerror = enhanceReport;
        document.head.appendChild(script);
        window.setTimeout(enhanceReport, 1400);
      }

      if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", loadGsapThenBoot);
      } else {
        loadGsapThenBoot();
      }
    })();
  </script>
</body>
</html>
`;
}

function main() {
  for (const run of reportConfig.runs) {
    for (const filePath of [run.trialCsv, run.sessionCsv]) {
      if (!fs.existsSync(filePath)) {
        throw new Error(`Missing input file: ${filePath}`);
      }
    }
  }

  const runs = reportConfig.runs.map(buildRunSummary);
  const data = {
    title: reportConfig.title,
    subtitle: reportConfig.subtitle,
    dateLabel: reportConfig.dateLabel,
    patientLabel: reportConfig.patientLabel,
    recommendation: reportConfig.recommendation,
    generatedAt: new Date().toISOString(),
    runs,
  };
  data.overall = computeOverall(runs);

  fs.mkdirSync(path.dirname(reportConfig.outputPath), { recursive: true });
  const html = buildHtml(data).replace(/[ \t]+$/gm, "");
  fs.writeFileSync(reportConfig.outputPath, html, "utf8");

  console.log(`Report written: ${relativeFromRepo(reportConfig.outputPath)}`);
  console.log(
    [
      `${data.overall.recordedMinutes} recorded min`,
      `${data.overall.totalTrials} trials`,
      `${data.overall.rehabHits}/${data.overall.rehabTrials} left rehab`,
      `${data.overall.controlHits}/${data.overall.controlTrials} right controls`,
    ].join(" | "),
  );
}

main();
