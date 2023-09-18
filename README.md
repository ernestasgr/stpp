# Sistemos paskirtis:
Projekto tikslas - sukurti kino teatro informacinę sistemą.

Kuriamą platformą sudaro klientinė dalis, serverinė dalis ir duomenų bazė. Klientas naudos klientinę sistemos dalį per grafinę naudotojo sąsają, kuri su serverine dalimi komunikuos per REST principu sudarytą API. Duomenys kaupiami duomenų bazėje.

Sistemoje egzistuos trys kliento rolės:

    - Svečias

    - Narys

    - Administratorius

Sistema turi tarpusavyje susietus hierarchiniu ryšiu objektus (filmas -> seansas -> bilietas), kuriems bus realizuojami API metodai, tarp jų ir hierarchiniai (pvz. API metodas gauti tam tikro filmo seansus).

Sistemoje bus realizuota autentifikacija ir autorizacija su OAuth2/JWT.

Sistema bus pasiekiama saityne, tam panaudojant debesų technologijas.

# Funkciniai reikalavimai:
## Svečias galės:

    - Peržiūrėti informaciją apie filmus ir jų seansus.

    - Peržiūrėti tam tikro seanso bilietų informaciją.

    - Užsiregistruoti

## Naudotojas galės:

    - Prisijungti / atsijungti

    - Valdyti savo paskyrą

    - Peržiūrėti informaciją apie filmus ir jų seansus.

    - Peržiūrėti tam tikro seanso bilietų informaciją.

    - Nusipirkti bilietą

    - Peržiūrėti savo bilietus, juos redaguoti (pvz: keisti seanso laiką, bilieto tipą) ar naikinti (jei įmanoma)

    - Parsisiųsti bilietą PDF formatu

    - Reitinguoti matytus filmus

## Administratorius galės:

    - Tą patį kaip ir naudotojas

    - Sukurti naujus filmus sistemoje, juos redaguoti ir ištrinti

    - Sukurti tam tikro filmo seansus, juos redaugoti ir ištrinti

    - Nustatyti vietų informaciją tam tikram seansui

    - Valdyti nupirktus bilietus
