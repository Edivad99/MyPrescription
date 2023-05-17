# Features
- Registrazione: Email, password, nome, cognome, data di nascita. Facciamo scegliere se essere dottore, farmacista o paziente.
- Login: Email, password, codice autenticazione

## Dottore
- Lista pazienti che segue (o tutti i pazienti)
- Per ogni paziente può visualizzare la sua scheda:
    - informazioni paziente
    - storico ricette
    - rinnovo di una vecchia ricetta
    - Link che porta ad aggiungere una ricetta direttamente al paziente
- Pagina per creare una ricetta:
    - seleziona da una select il paziente
    - nome del farmaco
    - esente dal pagamento
    - data della creazione

## Paziente
- Storico delle ricette
- Dettaglio ricetta:
    - Vede il nome del farmaco
    - Se è esente dal pagamento
    - Data della creazione
    - Possibilità di generare un codice univoco da esibire al farmacista

## Farmacista
- Dato il codice del paziente:
    - Vede se la ricetta è già usata
    - Vede il nome del farmaco
    - Se è esente dal pagamento
    - Data della creazione
    - Può segnare una ricetta come consegnata


