#!/usr/bin/env python3
"""Build the September research snapshot and standalone monitor from the August CSV."""

import csv
import html
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
OLD = ROOT / "docs/research/clinical-trials-watchlist-2026-08-13.csv"
NEW = ROOT / "docs/research/clinical-trials-watchlist-2026-09-25.csv"
MONITOR = ROOT / "docs/research/research-monitor-2026-09-25.html"


with OLD.open(newline="", encoding="utf-8") as source:
    reader = csv.DictReader(source)
    fields = reader.fieldnames
    rows = list(reader)

for row in rows:
    row["last_checked"] = "2026-09-25"
    if row["id"] == "CTG-012":
        row["status"] = "Completed with results posted"
        row["next_check"] = "Review follow-up publication and registry results; completed 2026-09-18"
    elif row["id"] == "CTG-016":
        row["status"] = "Completed with results posted"
        row["next_check"] = "Review device preference and adverse events; completed 2026-08-19"
    elif row["id"] == "CTG-030":
        row["linked_pubmed"] = ""
        row["evidence_role"] = "Completed AP-HP/Pitie-Salpetriere chronic hemianopia tACS trial; no posted registry results. Raffin 2025 is a separate paper, not a verified trial publication"
        row["next_check"] = "Watch for HEMIANOTACS results or AP-HP follow-up; ask team about eligibility for other studies"

rows.append({
    "id": "CTG-036",
    "nct_id": "NCT07830745",
    "priority": "Medium",
    "track": "France/reading optokinetic comparator",
    "status": "Not yet recruiting",
    "last_checked": "2026-09-25",
    "condition": "Hemianopic alexia after stroke",
    "intervention": "Optokinetic stimulation without reading versus reading task; randomized crossover",
    "linked_pubmed": "",
    "evidence_role": "Newly registered French reading-function comparator at Clinique Les Trois Soleils, Boissise-le-Roi; first posted 2026-09-21",
    "protocol_impact": "Reading-specific comparator only; no change to Quest audiovisual protocol",
    "next_check": "Check recruitment, age and reading eligibility, full protocol, and results",
})

assert len(rows) == 36
assert len({r["id"] for r in rows}) == len(rows)
assert len({r["nct_id"] for r in rows}) == len(rows)
with NEW.open("w", newline="", encoding="utf-8") as output:
    writer = csv.DictWriter(output, fieldnames=fields, lineterminator="\n")
    writer.writeheader()
    writer.writerows(rows)


def card(row):
    esc = html.escape
    status = row["status"]
    state = "recruiting" if status == "Recruiting" else "completed" if status.startswith("Completed") else "watch"
    return f'''<article class="trial" data-search="{esc(' '.join(row.values()).lower(), quote=True)}" data-state="{state}">
      <div class="trial-top"><span class="id">{esc(row['id'])}</span><a href="https://clinicaltrials.gov/study/{esc(row['nct_id'])}" target="_blank" rel="noopener">{esc(row['nct_id'])} ↗</a><span class="state {state}">{esc(status)}</span></div>
      <h3>{esc(row['track'])}</h3><p>{esc(row['condition'])}</p><p class="intervention">{esc(row['intervention'])}</p>
      <div class="role">{esc(row['evidence_role'])}</div><div class="impact">{esc(row['protocol_impact'])}</div>
    </article>'''


cards = "\n".join(card(row) for row in rows)
page = r'''<!doctype html>
<html lang="fr"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
<title>NegletFix | Veille recherche · 25 septembre 2026</title>
<style>
:root{--bg:#10191d;--band:#19272b;--line:#34474d;--ink:#f5f4ef;--muted:#b8c6c6;--cyan:#80d6db;--amber:#f0bd70;--coral:#e9947c;--green:#90d2ae}
*{box-sizing:border-box}html{scroll-behavior:smooth}body{margin:0;color:var(--ink);background:var(--bg);font:15px/1.48 -apple-system,BlinkMacSystemFont,"Segoe UI",sans-serif;letter-spacing:0}button,input{font:inherit}a{color:var(--cyan)}:focus-visible{outline:3px solid var(--cyan);outline-offset:2px}.wrap{width:min(100% - 36px,1220px);margin:auto}
header{border-bottom:1px solid var(--line);background:#152228}header .wrap{display:flex;align-items:center;justify-content:space-between;min-height:74px;gap:16px}.brand{font-weight:800;font-size:16px}.brand span{color:var(--cyan)}nav{display:flex;gap:18px;flex-wrap:wrap}nav a{color:var(--muted);text-decoration:none;font-size:13px}nav a:hover{color:var(--ink)}
.hero{padding:44px 0 46px;border-bottom:1px solid var(--line)}.eyebrow{margin:0 0 8px;color:var(--cyan);font-size:12px;font-weight:800;text-transform:uppercase;letter-spacing:.1em}h1{font-size:52px;line-height:1.06;margin:0;max-width:900px}h2{font-size:27px;line-height:1.2;margin:0 0 18px}h3{font-size:17px;line-height:1.25;margin:0 0 8px}.hero p{max-width:820px;color:var(--muted);font-size:17px}.verdict{border-left:4px solid var(--green);padding:12px 18px;background:#1d3131;max-width:860px;font-weight:700}.summary{display:grid;grid-template-columns:repeat(4,1fr);gap:1px;background:var(--line);border:1px solid var(--line);margin-top:29px}.metric{background:#18272c;padding:16px 18px}.metric strong{display:block;font-size:30px;line-height:1;color:var(--ink)}.metric span{font-size:12px;color:var(--muted)}
section.band{padding:35px 0;border-bottom:1px solid var(--line)}.band.alt{background:var(--band)}.lead-grid{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:18px 28px}.lead{border-top:2px solid var(--line);padding:15px 0 0}.lead .tag{font-size:12px;font-weight:800;color:var(--amber);text-transform:uppercase}.lead p{color:var(--muted);margin:7px 0}.lead strong{color:var(--ink)}.note{color:var(--muted);max-width:940px}.local{border-left:3px solid var(--amber);padding-left:20px}.local p{color:var(--muted)}
.filters{display:flex;gap:8px;align-items:center;flex-wrap:wrap;margin:22px 0}.filters input{min-width:260px;max-width:420px;width:100%;background:#213238;border:1px solid #5b7074;color:var(--ink);border-radius:5px;padding:9px 11px}.filters button{background:#213238;color:var(--muted);border:1px solid #52676e;border-radius:5px;padding:9px 12px;cursor:pointer}.filters button[aria-pressed=true]{background:#345259;color:var(--ink);border-color:var(--cyan)}#count{color:var(--muted);font-size:13px;margin-left:auto}.trial-grid{display:grid;grid-template-columns:repeat(3,minmax(0,1fr));gap:11px}.trial{background:#1b292e;border:1px solid #3b5056;border-radius:6px;padding:15px;min-width:0}.trial[hidden]{display:none}.trial-top{display:flex;gap:9px;align-items:center;flex-wrap:wrap;margin-bottom:14px;font-size:12px;font-weight:800}.id{color:var(--muted)}.trial-top a{white-space:nowrap}.state{margin-left:auto;color:var(--amber)}.state.completed{color:var(--green)}.state.recruiting{color:var(--cyan)}.trial p{margin:0 0 8px;color:var(--muted);font-size:13px}.trial .intervention{color:var(--ink)}.role{border-top:1px solid var(--line);padding-top:10px;margin-top:13px;color:var(--muted);font-size:13px}.impact{margin-top:10px;color:var(--green);font-size:12px}.foot{padding:30px 0 48px;color:var(--muted);font-size:13px}.foot p{max-width:950px}li{margin:7px 0}
@media(max-width:900px){.trial-grid{grid-template-columns:repeat(2,minmax(0,1fr))}.summary{grid-template-columns:repeat(2,1fr)}}@media(max-width:650px){header .wrap{display:block;padding:14px 0}nav{margin-top:7px}h1{font-size:37px}h2{font-size:23px}.lead-grid,.trial-grid{grid-template-columns:1fr}.summary{grid-template-columns:repeat(2,1fr)}.filters input{min-width:100%}#count{margin:0}}
</style></head><body>
<header><div class="wrap"><div class="brand">Neglet<span>Fix</span> / Veille</div><nav><a href="#nouveau">Nouveautés</a><a href="#paris">Paris</a><a href="#essais">36 essais</a><a href="#sources">Sources</a></nav></div></header>
<main><div class="hero"><div class="wrap"><p class="eyebrow">Mise à jour du 25 septembre 2026 · depuis le 13 août</p><h1>La recherche avance. Le protocole reste stable.</h1><p>Un nouvel essai français sur la lecture hémianopsique, deux essais suivis désormais terminés, et deux articles utiles pour cadrer les attentes et les mesures. La Pitié-Salpêtrière demeure une piste de contact scientifique, pas un essai actuellement ouvert à l'inclusion.</p><div class="verdict">Aucun résultat nouveau ne justifie de modifier l'entraînement audiovisuel Quest avant le retour du casque et une reprise mesurée.</div><div class="summary"><div class="metric"><strong>36</strong><span>essais dans la veille</span></div><div class="metric"><strong>1</strong><span>nouvel essai enregistré</span></div><div class="metric"><strong>2</strong><span>essais passés à « terminé »</span></div><div class="metric"><strong>0</strong><span>changement de protocole</span></div></div></div></div>
<section class="band" id="nouveau"><div class="wrap"><h2>Ce qui mérite l'attention</h2><div class="lead-grid">
<div class="lead"><span class="tag">France · nouvel essai</span><h3><a href="https://clinicaltrials.gov/study/NCT07830745">Stimulation optocinétique et lecture ↗</a></h3><p>Première fiche publiée le 21 septembre. Clinique Les Trois Soleils, Boissise-le-Roi. Étude croisée de stimulation visuelle pour l'alexie hémianopsique après AVC, <strong>pas encore en recrutement</strong>. Comparateur fonctionnel, pas validation de notre AV Quest.</p></div>
<div class="lead"><span class="tag">Registre · deux clôtures</span><h3>Scanning et prismes</h3><p><a href="https://clinicaltrials.gov/study/NCT06136169">NCT06136169</a> et <a href="https://clinicaltrials.gov/study/NCT04827147">NCT04827147</a> sont maintenant marqués « Completed » avec résultats déposés. Ils concernent la compensation et l'adaptation, non la restauration audiovisuelle.</p></div>
<div class="lead"><span class="tag">Synthèse · retrouvée dans cette passe</span><h3><a href="https://pubmed.ncbi.nlm.nih.gov/42652963/">Revue des interventions pour pertes de champ visuel ↗</a></h3><p>Publiée fin juillet mais indexée après notre précédente passe. La revue distingue mieux les gains fonctionnels de compensation et la preuve plus variable d'une expansion du champ.</p></div>
<div class="lead"><span class="tag">Mesure · septembre</span><h3><a href="https://pubmed.ncbi.nlm.nih.gov/42684868/">Périmétrie cinétique par eye-tracking ↗</a></h3><p>Validation d'une méthode de mesure sur 37 participants, dont 17 avec lésions rétrochiasmatiques. Le seuil suggéré de changement significatif est propre à cet instrument : il ne s'applique pas directement aux mesures Quest.</p></div>
</div></div></section>
<section class="band alt" id="paris"><div class="wrap"><h2>Paris : que reste-t-il de la piste Salpêtrière ?</h2><div class="local"><p><strong>HEMIANOTACS / <a href="https://clinicaltrials.gov/study/NCT04043689">NCT04043689</a></strong> : essai tACS de l'AP-HP/Pitié-Salpêtrière sur l'hémianopsie chronique. L'AP-HP indique « Suivi terminé » ; le registre indique « Completed », sans résultats déposés. Cela n'est donc pas une place ouverte dans cet essai.</p><p>Correction importante : l'article <a href="https://pubmed.ncbi.nlm.nih.gov/41243213/">Raffin et al. (2025)</a> est une étude de tACS distincte ; aucune source primaire vérifiée ne permet de l'attribuer à HEMIANOTACS. La bonne démarche locale reste une consultation de <a href="https://pitiesalpetriere.aphp.fr/consultation/83183/">neuro-ophtalmologie à la Pitié-Salpêtrière</a>, en apportant le bilan visuel et les comptes rendus, puis la question des équipes/études adaptées.</p><p><a href="https://clinicaltrials.gov/study/NCT06636994">Rothschild NCT06636994</a> reste actif mais ne recrute plus ; l'IRON/LMC2 reste une piste sociale sans publication primaire identifiée dans cette passe.</p></div></div></section>
<section class="band" id="essais"><div class="wrap"><h2>Registre suivi</h2><p class="note">Recherche et filtres locaux sur le cliché ClinicalTrials.gov du 25 septembre. Statut de registre ≠ efficacité démontrée ni éligibilité personnelle.</p><div class="filters"><input id="search" type="search" placeholder="Rechercher un NCT, une modalité, un statut…" aria-label="Rechercher un essai"><button type="button" data-filter="all" aria-pressed="true">Tous</button><button type="button" data-filter="recruiting" aria-pressed="false">Recrutement</button><button type="button" data-filter="completed" aria-pressed="false">Terminés</button><button type="button" data-filter="watch" aria-pressed="false">Autres</button><span id="count" aria-live="polite"></span></div><div class="trial-grid" id="trial-grid">__CARDS__</div></div></section>
<section class="band alt" id="sources"><div class="wrap"><h2>Sources et méthode</h2><ul><li><a href="https://clinicaltrials.gov/">ClinicalTrials.gov</a>, fiches API des 35 essais suivis plus recherche large « hemianopia » ; statuts vérifiés le 25 septembre 2026.</li><li><a href="https://www.aphp.fr/registre-des-essais-cliniques/stimulation-transcranienne-par-courant-electrique-alternatif-tacs">AP-HP, fiche HEMIANOTACS</a> ; <a href="https://pitiesalpetriere.aphp.fr/consultation/83183/">consultation de neuro-ophtalmologie</a>.</li><li><a href="https://pubmed.ncbi.nlm.nih.gov/42652963/">PMID 42652963</a> et <a href="https://pubmed.ncbi.nlm.nih.gov/42684868/">PMID 42684868</a>, récupérés par PubMed E-utilities.</li><li><a href="https://arxiv.org/abs/2606.14957v5">Neuro-JEPA v5</a> : veille méthodes d'IRM uniquement, aucune preuve de rééducation du champ visuel.</li></ul><p class="note">YouTube, X, LinkedIn, Mayo et GitHub ont été parcourus comme voies de découverte ; aucun signal primaire vérifié n'a modifié la décision. Détails, limites et pistes de prochaine vérification : <a href="research-refresh-2026-09-25.md">note de recherche</a> et <a href="clinical-trials-watchlist-2026-09-25.csv">CSV source</a>.</p></div></section></main><footer class="foot"><div class="wrap"><p>NegletFix · veille documentaire, pas un avis médical. Comparer les études avec un clinicien avant toute décision de traitement ou de stimulation. Prochaine étape produit : retour du Quest, reprise du protocole mesuré, sondes -5° / -8° et essais de contrôle.</p></div></footer>
<script>const cards=[...document.querySelectorAll('.trial')],search=document.getElementById('search'),count=document.getElementById('count');let filter='all';function update(){const q=search.value.trim().toLowerCase();let n=0;for(const card of cards){const show=(filter==='all'||card.dataset.state===filter)&&card.dataset.search.includes(q);card.hidden=!show;if(show)n++}count.textContent=n+' / '+cards.length+' essais';}search.addEventListener('input',update);document.querySelectorAll('[data-filter]').forEach(button=>button.addEventListener('click',()=>{filter=button.dataset.filter;document.querySelectorAll('[data-filter]').forEach(b=>b.setAttribute('aria-pressed',String(b===button)));update()}));update();</script></body></html>'''
MONITOR.write_text(page.replace("__CARDS__", cards), encoding="utf-8")
print(f"Wrote {NEW.relative_to(ROOT)} and {MONITOR.relative_to(ROOT)} ({len(rows)} trials)")
