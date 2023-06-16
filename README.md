# MyPrescription
## Secure digital healthcare project 2022-2023

The objective of the project was to implement some safety aspects seen during the lessons, the following points had to be respected:
- HTTPS network data exchange
- Graphicalinterface
- Data encryption
- MFA (multi-factor auth) and authorization

The project reproduces a scenario in which:
- The doctor creates a new prescription for the patient
- The patient receives a notification informing him that he can view the prescription in his dashboard
- The patient can see the monouse code that he will have to communicate to the pharmacist to collect the drug
- The patient can regenerate a new monouse code if he wants
- The pharmacist can scan the qrcode or enter the monouse code
- The patient receives a notification as soon as the prescription has been delivered

### Security aspects
1. The password is saved encrypted in the database
2. 2FA authentication is enabled by default
3. The pharmacist sees only the information he needs to do his job

### Participants
- Davide Albiero
- Matteo Tossuto
