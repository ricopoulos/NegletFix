#!/usr/bin/env python3
"""Build the October 5 research snapshot and self-contained HTML monitor."""

import csv
import html
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
RESEARCH = ROOT / "docs/research"
SOURCE = RESEARCH / "clinical-trials-watchlist-2026-09-25.csv"
SNAPSHOT = RESEARCH / "clinical-trials-watchlist-2026-10-05.csv"
MONITOR = RESEARCH / "research-monitor-2026-10-05.html"

with SOURCE.open(newline="", encoding="utf-8") as source:
    reader = csv.DictReader(source)
    fields = reader.fieldnames
    rows = list(reader)

for row in rows:
    row["last_checked"] = "2026-10-05"
    if row["id"] == "CTG-026":
        row["status"] = "Suspended"
        row["evidence_role"] = "Eye-tracking biofeedback versus home reading comparator; registry suspended for funding on 2026-09-25"
        row["next_check"] = "Watch for funding resolution and restart; no enrollment while suspended"
    elif row["id"] == "CTG-031":
        row["status"] = "Not yet recruiting"
        row["evidence_role"] = "Fondation Rothschild Paris observational blindsight/fMRI study; October 1 registry update now says not yet recruiting"
        row["next_check"] = "Verify actual recruitment opening and publications; status correction is not an efficacy result"

rows.append({
    "id": "CTG-037",
    "nct_id": "NCT07849582",
    "priority": "Medium",
    "track": "Individualized neurovisual rehabilitation/early ABI",
    "status": "Not yet recruiting",
    "last_checked": "2026-10-05",
    "condition": "Acquired visual impairment after stroke, TBI, or brain tumor",
    "intervention": "Five therapist-led sessions plus self-training at least five days weekly",
    "linked_pubmed": "",
    "evidence_role": "New Prague single-group 30-person feasibility pilot, first posted 2026-09-30; visual scanning, fixation, gaze and compensation",
    "protocol_impact": "Early-window care comparator only; 4 weeks to 12 months after injury excludes Eric's 2021 stroke; no sham/control group",
    "next_check": "Recruitment start and feasibility outcomes; do not infer efficacy or personal eligibility",
})

assert fields is not None
assert len(rows) == 37
assert len({row["id"] for row in rows}) == len(rows)
assert len({row["nct_id"] for row in rows}) == len(rows)
with SNAPSHOT.open("w", newline="", encoding="utf-8") as output:
    writer = csv.DictWriter(output, fieldnames=fields, lineterminator="\n")
    writer.writeheader()
    writer.writerows(rows)


def trial_card(row):
    esc = html.escape
    state = ("recruiting" if row["status"] == "Recruiting" else
             "completed" if row["status"].startswith("Completed") else "watch")
    search = esc(" ".join(row.values()).lower(), quote=True)
    return f'''<article class="trial" data-search="{search}" data-state="{state}">
  <div class="trial-top"><span>{esc(row['id'])}</span><a href="https://clinicaltrials.gov/study/{esc(row['nct_id'])}" target="_blank" rel="noopener">{esc(row['nct_id'])} ↗</a><span class="state {state}">{esc(row['status'])}</span></div>
  <h3>{esc(row['track'])}</h3><p>{esc(row['condition'])}</p><p class="intervention">{esc(row['intervention'])}</p>
  <p class="role">{esc(row['evidence_role'])}</p><p class="impact">{esc(row['protocol_impact'])}</p>
</article>'''


cards = "\n".join(trial_card(row) for row in rows)
page = '''<!doctype html>
<html lang="fr"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
<title>NegletFix | Veille recherche · 5 octobre 2026</title>
<style>
:root{--bg:#10191d;--band:#19272b;--line:#34474d;--ink:#f5f4ef;--muted:#b8c6c6;--cyan:#80d6db;--amber:#f0bd70;--coral:#e9947c;--green:#90d2ae}
*{box-sizing:border-box}html{scroll-behavior:smooth}body{margin:0;color:var(--ink);background:var(--bg);font:15px/1.48 -apple-system,BlinkMacSystemFont,"Segoe UI",sans-serif;letter-spacing:0}button,input{font:inherit}a{color:var(--cyan)}:focus-visible{outline:3px solid var(--cyan);outline-offset:2px}.wrap{width:min(100% - 36px,1220px);margin:auto}
header{border-bottom:1px solid var(--line);background:#152228}header .wrap{display:flex;align-items:center;justify-content:space-between;min-height:74px;gap:16px}.brand{font-weight:800;font-size:16px}.brand span{color:var(--cyan)}nav{display:flex;gap:18px;flex-wrap:wrap}nav a{color:var(--muted);text-decoration:none;font-size:13px}nav a:hover{color:var(--ink)}
.hero{padding:42px 0 45px;border-bottom:1px solid var(--line)}.eyebrow{margin:0 0 8px;color:var(--cyan);font-size:12px;font-weight:800;text-transform:uppercase;letter-spacing:0}h1{font-size:48px;line-height:1.08;margin:0;max-width:900px}h2{font-size:27px;line-height:1.2;margin:0 0 18px}h3{font-size:17px;line-height:1.25;margin:0 0 8px}.hero p{max-width:850px;color:var(--muted);font-size:17px}.verdict{border-left:4px solid var(--green);padding:12px 18px;background:#1d3131;max-width:900px;font-weight:700}.summary{display:grid;grid-template-columns:repeat(4,1fr);gap:1px;background:var(--line);border:1px solid var(--line);margin-top:29px}.metric{background:#18272c;padding:16px 18px}.metric strong{display:block;font-size:30px;line-height:1;color:var(--ink)}.metric span{font-size:12px;color:var(--muted)}
section.band{padding:35px 0;border-bottom:1px solid var(--line)}.band.alt{background:var(--band)}.lead-grid{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:18px 28px}.lead{border-top:2px solid var(--line);padding:15px 0 0}.lead .tag{font-size:12px;font-weight:800;color:var(--amber);text-transform:uppercase}.lead p{color:var(--muted);margin:7px 0}.lead strong{color:var(--ink)}.note{color:var(--muted);max-width:940px}.local{border-left:3px solid var(--amber);padding-left:20px}.local p{color:var(--muted)}
.filters{display:flex;gap:8px;align-items:center;flex-wrap:wrap;margin:22px 0}.filters input{min-width:260px;max-width:420px;width:100%;background:#213238;border:1px solid #5b7074;color:var(--ink);border-radius:5px;padding:9px 11px}.filters button{background:#213238;color:var(--muted);border:1px solid #52676e;border-radius:5px;padding:9px 12px;cursor:pointer}.filters button[aria-pressed=true]{background:#345259;color:var(--ink);border-color:var(--cyan)}#count{color:var(--muted);font-size:13px;margin-left:auto}.trial-grid{display:grid;grid-template-columns:repeat(3,minmax(0,1fr));gap:11px}.trial{background:#1b292e;border:1px solid #3b5056;border-radius:6px;padding:15px;min-width:0}.trial[hidden]{display:none}.trial-top{display:flex;gap:9px;align-items:center;flex-wrap:wrap;margin-bottom:14px;font-size:12px;font-weight:800}.trial-top a{white-space:nowrap}.state{margin-left:auto;color:var(--amber)}.state.completed{color:var(--green)}.state.recruiting{color:var(--cyan)}.trial p{margin:0 0 8px;color:var(--muted);font-size:13px}.trial .intervention{color:var(--ink)}.role{border-top:1px solid var(--line);padding-top:10px;margin-top:13px}.trial .impact{margin-top:10px;color:var(--green);font-size:12px}.foot{padding:30px 0 48px;color:var(--muted);font-size:13px}.foot p{max-width:950px}li{margin:7px 0}
@media(max-width:900px){.trial-grid{grid-template-columns:repeat(2,minmax(0,1fr))}.summary{grid-template-columns:repeat(2,1fr)}}@media(max-width:650px){header .wrap{display:block;padding:14px 0}nav{margin-top:7px}h1{font-size:36px}h2{font-size:23px}.lead-grid,.trial-grid{grid-template-columns:1fr}.summary{grid-template-columns:repeat(2,1fr)}.filters input{min-width:100%}#count{margin:0}}
</style></head><body>
<header><div class="wrap"><div class="brand">Neglet<span>Fix</span> / Veille</div><nav><a href="#nouveau">Nouveautés</a><a href="#paris">Paris</a><a href="#essais">37 essais</a><a href="#sources">Sources</a></nav></div></header>
<main><div class="hero"><div class="wrap"><p class="eyebrow">Mise à jour du 5 octobre 2026 · depuis le 25 septembre</p><h1>Quelques mouvements, aucun signal pour changer le Quest.</h1><p>Un essai pilote ajouté, deux statuts de registre corrigés et un article d'imagerie paru depuis notre dernière veille. Ce sont des pistes de comparaison ou de mécanisme, pas des preuves de récupération visuelle.</p><div class="verdict">Protocole inchangé : attendre le Quest, puis reprendre les mesures avec cibles -5° / -8°, essais de contrôle et escalier de contraste borné.</div><div class="summary"><div class="metric"><strong>37</strong><span>essais dans la veille</span></div><div class="metric"><strong>1</strong><span>nouvel essai enregistré</span></div><div class="metric"><strong>2</strong><span>statuts corrigés</span></div><div class="metric"><strong>0</strong><span>changement de protocole</span></div></div></div></div>
<section class="band" id="nouveau"><div class="wrap"><h2>Ce qui a changé</h2><div class="lead-grid">
<div class="lead"><span class="tag">Prague · nouvel essai</span><h3><a href="https://clinicaltrials.gov/study/NCT07849582">Rééducation neurovisuelle individualisée ↗</a></h3><p>Pilote à groupe unique de 30 personnes, cinq séances encadrées et exercices à domicile. <strong>Pas encore en recrutement.</strong> Inclusion limitée à 4 semaines–12 mois après la lésion : l'AVC d'Eric en 2021 ne correspond pas à cette fenêtre. Comparateur de prise en charge, pas preuve d'efficacité.</p></div>
<div class="lead"><span class="tag">Registre · deux statuts</span><h3>Lecture et étude parisienne</h3><p><a href="https://clinicaltrials.gov/study/NCT06638619">NCT06638619</a> est suspendu pour financement ; <a href="https://clinicaltrials.gov/study/NCT06636994">NCT06636994</a>, étude d'imagerie de Rothschild à Paris, est maintenant noté « pas encore en recrutement ». Ces changements ne sont pas des résultats.</p></div>
<div class="lead"><span class="tag">IRM · publication du 26 septembre</span><h3><a href="https://pubmed.ncbi.nlm.nih.gov/42799918/">Strie de Gennari et déficit du champ visuel ↗</a></h3><p>Étude à 7 T sur l'intégrité d'une couche de V1 après perte d'apport visuel. Elle porte sur des lésions entre le chiasma et V1, <strong>pas sur une reconstruction de tissu occipital lésé</strong>. Piste mécanistique uniquement.</p></div>
<div class="lead"><span class="tag">Photobiomodulation · vérification</span><h3><a href="https://www.vielight.com/research/">Vielight reste en veille ↗</a></h3><p>Des effets EEG et cognitifs préliminaires existent dans d'autres populations ; aucune preuve spécifique de restauration du champ visuel chronique ne justifie d'ajouter lumière infrarouge, « Alpha » ou Vagus au protocole Quest. Voir la <a href="https://pubmed.ncbi.nlm.nih.gov/41768981/">petite étude post-Covid</a>, publiée en janvier, et notre note critique.</p></div>
</div></div></section>
<section class="band alt" id="paris"><div class="wrap"><h2>Paris : la piste Salpêtrière</h2><div class="local"><p><strong>HEMIANOTACS / <a href="https://clinicaltrials.gov/study/NCT04043689">NCT04043689</a></strong> reste terminé, sans résultats déposés au registre. La <a href="https://www.aphp.fr/registre-des-essais-cliniques/stimulation-transcranienne-par-courant-electrique-alternatif-tacs">fiche AP-HP</a> indique toujours « Suivi terminé » : ce n'est pas un essai ouvert.</p><p>La <a href="https://pitiesalpetriere.aphp.fr/consultation/83183/">consultation de neuro-ophtalmologie</a> reste une piste concrète pour examiner le dossier et demander quelles études ou prises en charge actuelles conviennent. À Rothschild, le statut de l'étude blindsight/IRM est désormais « pas encore en recrutement » ; aucune inclusion n'est à présumer.</p></div></div></section>
<section class="band" id="essais"><div class="wrap"><h2>Registre suivi</h2><p class="note">Recherche locale dans le cliché ClinicalTrials.gov du 5 octobre. Statut de registre ≠ efficacité démontrée ni éligibilité personnelle.</p><div class="filters"><input id="search" type="search" placeholder="Rechercher un NCT, une modalité, un statut…" aria-label="Rechercher un essai"><button type="button" data-filter="all" aria-pressed="true">Tous</button><button type="button" data-filter="recruiting" aria-pressed="false">Recrutement</button><button type="button" data-filter="completed" aria-pressed="false">Terminés</button><button type="button" data-filter="watch" aria-pressed="false">Autres</button><span id="count" aria-live="polite"></span></div><div class="trial-grid" id="trial-grid">__CARDS__</div></div></section>
<section class="band alt" id="sources"><div class="wrap"><h2>Sources et méthode</h2><ul><li><a href="https://clinicaltrials.gov/data-api/api">ClinicalTrials.gov API</a> : 36 fiches suivies revérifiées le 5 octobre, plus recherche des nouveaux enregistrements du 26 septembre au 5 octobre.</li><li><a href="https://pubmed.ncbi.nlm.nih.gov/42799918/">PMID 42799918</a> ; recherche PubMed E-utilities par date d'entrée sur l'hémianopsie et la perte du champ visuel.</li><li><a href="https://www.aphp.fr/registre-des-essais-cliniques/stimulation-transcranienne-par-courant-electrique-alternatif-tacs">AP-HP HEMIANOTACS</a> et <a href="https://www.vielight.com/research/">bibliothèque Vielight</a>, recoupée avec les publications originales pertinentes.</li></ul><p class="note">Les annonces institutionnelles et sociales ont servi à repérer des pistes, sans remplacer les registres et articles. Détails et limites : <a href="research-refresh-2026-10-05.md">note de recherche</a> et <a href="clinical-trials-watchlist-2026-10-05.csv">CSV source</a>.</p></div></section></main><footer class="foot"><div class="wrap"><p>NegletFix · veille documentaire, pas un avis médical. Toute stimulation ou traitement nouveau relève d'une discussion clinique. Prochaine étape produit : retour du Quest et reprise du protocole mesuré.</p></div></footer>
<script>const cards=[...document.querySelectorAll('.trial')],search=document.getElementById('search'),count=document.getElementById('count');let filter='all';function update(){const q=search.value.trim().toLowerCase();let n=0;for(const card of cards){const show=(filter==='all'||card.dataset.state===filter)&&card.dataset.search.includes(q);card.hidden=!show;if(show)n++}count.textContent=n+' / '+cards.length+' essais';}search.addEventListener('input',update);document.querySelectorAll('[data-filter]').forEach(button=>button.addEventListener('click',()=>{filter=button.dataset.filter;document.querySelectorAll('[data-filter]').forEach(b=>b.setAttribute('aria-pressed',String(b===button)));update()}));update();</script></body></html>'''

MONITOR.write_text(page.replace("__CARDS__", cards), encoding="utf-8")
print(f"Wrote {SNAPSHOT.relative_to(ROOT)} and {MONITOR.relative_to(ROOT)} ({len(rows)} trials)")
