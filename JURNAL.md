<!--
  ȘABLON · JURNAL.md · Laborator Întâlnirea 4 
  Cum îl folosiți:
  1. Copiați fișierul în folderul rădăcina al solutiei voastre (lângă fișierul .slnx).
  2. Înlocuiți tot ce este între \\\[paranteze drepte] 
 

# Laboratorul Î4 · \\\[Bodea Joshua] · nr. matricol \\\[121200050] · varianta \\\[A0 · B1]

|Parametru|Valoarea mea|
|-|-|
|A (ultima cifră a nr. matricol: \\\[0)|\\\[A0]|
|B (penultima cifră a nr. matricol: \\\[5])|\\\[B1]|
|V1 · cărți pe pagină|\\\[4]|
|V2 · sortarea implicită a catalogului|\\\[preț descrescator]|
|V3 · filtrul din S3|\\\[gen]|
|V4 · indicatorul din S2|\\\[preț mediu]|

## S1 · Comenzile

\* Protecția la overposting aleasă: \*\*\\\[ViewModel], [Atributul Bind pe acțiunile Create și Edit]\*\*
\* De ce am ales-o: \\\[ Pentru a optimiza interogările bazei de date și a afișa direct datele necesare în catalog (BookListItem)], [Pentru a specifica în mod explicit o listă albă (whitelist) cu proprietățile entității Order ce pot fi preluate din cererile HTTP POST, prevenind astfel modificarea neautorizată a datelor prin atacuri de tip overposting.]

## S2 · Pagina unui autor și sortarea după autor

\* Indicatorul V4 (\\\[numărul de comenzi pe carte / `Orders.Count`]) este calculat cu interogarea:

```sql
\\\[SELECT b.ID, b.Title, a.FirstName, a.LastName, g.Name, b.Price, COUNT(o.ID) AS OrdersCount
FROM Book AS b
LEFT JOIN Author AS a ON b.AuthorID = a.ID
LEFT JOIN Genre AS g ON b.GenreID = g.ID
LEFT JOIN [Order] AS o ON b.ID = o.BookID
GROUP BY b.ID, b.Title, a.FirstName, a.LastName, g.Name, b.Price;]
```

\* De ce `OrderBy(b => b.Author.FullName)` aruncă o excepție? \\\[Deoarece FullName este o proprietate calculată doar în codul C# (prin concatenarea prenumelui și numelui), nefiind o coloană fizică în tabelul bazei de date pe care motorul SQL să poată face sortarea direct]
\* Cum am rezolvat: \\\[Am sortat folosind proprietățile reale ]



## S3 · Filtrul variantei mele (V3)

\* Filtrul implementat: \*\*\\\[ gen]\*\*
\* Interogarea , cu filtrul aplicat:

```sql
\\\[SELECT *
FROM Book
WHERE GenreID = 2;]
```

## S4 · Folosirea AI

\* \*\*Instrumente folosite:\*\* \\\[gemini]
\* \*\*La ce pași:\*\* \\\[unde am intampinat erori, mai ales in dezvoltare ]
\* \*\*Un exemplu de cerere trimisă:\*\*

> \\\[Cum integrez filtrul pe bază de gen și parametrul int? genreID în metoda Index din BooksController?]

\* \*\*O situație în care am corectat sau respins un răspuns AI\*\* (sau, dacă nu am folosit AI, o eroare întâlnită și cum am găsit-o):

  \* Ce a propus AI-ul / care era eroarea: \\\[ o interogare în `AuthorsController` care căuta după o proprietate inexistentă (CustomerID) pe entitatea Author, ceea ce a generat eroarea de compilare CS1061]
  \* Cum mi-am dat seama că e greșit sau ineficient: \\\[Prin verificarea ferestrei de Error List care a indicat explicit ca entitatea Author nu conține o definiție pentru CustomerID]
  \* Codul inițial:

```csharp
\\\[var author = await _context.Author
            .FirstOrDefaultAsync(m => m.CustomerID == id); ]
```

\* Codul final:

```csharp
\\\[var author = await _context.Author
            .FirstOrDefaultAsync(m => m.ID == id); ]
```

\\---

## Ce nu am reușit (opțional)

\\\[nu am reusit sa indeplinesc sarcinile 3 si 4 deoarece am intamplinat multe erori pe parcursul dezvoltarii si a trebuit sa le rezolv. Timpul a fost problema.]


Laboratorul 5:
Pasul A1 - import ratings
A fost importat fișierul `book-ratings.csv`.
Număr de rânduri/evaluări importate în baza de date 6599 

Pasul A2:
Deschideți /Recommendations?userId=5 după reantrenare, fără să reporniți aplicația.
Rezultat:
Recomandări pentru cititorul 5
 
Domain-Driven Design (scor 4.51)
The Pragmatic Programmer (scor 3.97)
Câinele din Baskerville (scor 3.96)
Fata cu un tatuaj de dragon (scor 3.93)
Designing Data-Intensive Applications (scor 3.91)

Pasul A3:
Testare API Recomandări (Cold Start vs Model ML)
-cititorul 5 (Utilizator existent): Primește recomandări personalizate calculate prin ML (fallback: false), bazate pe scorurile prezise de model (ex. Fata cu un tatuaj de dragon, scor ~3.93). Parametrul top=3 limiteaza corect numarul rezultatelor
-Cititorul 999 (Cold Start -utilizator necunoscut): Declanșează mecanismul de fallback (fallback: true), primind cele mai populare și bine cotate cărți din baza de date pe baza mediilor de evaluare

Pasul A4:
MicroAccuracy: 74.3%
MacroAccuracy: 74.2%
De ce difera?
MicroAccuracy calculeaza acuratetea globală raportând numarul de preziceri corecte la numărul total de exemple din intregul set de test. Din acest motiv, ea manipulata de clasele majore (genurile care conțin cele mai multe exemple)
MacroAccuracy calculează acuratețea pentru fiecare clasa în mod individual, dupa care face media aritmetică simpla a acestora, si trateaza fiecare gen cu aceeași pondere (indiferent dacă are multe ex sau doar câteva)
difera pentru ca clasele cu mai puține exemple au un volum redus de date de antrenare, modelul învața mai greu specificul lor și obține adesea o acuratețe mai mică pe ele
Citiți matricea de confuzie: ce genuri se confundă cel mai des? Este logic?
Poezie-Nuvelă se confunda. Ambele sunt forme literare de dimensiuni mai reduse comparativ cu un roman amplu
Confuziile sunt logice, deoarece unele genuri literare au vocabular și caracteristici comune. Cele mai bune rezultate sunt la Fantasy recall 0.9310, iar cele mai slabe la Programare  0.6176 și Nuvelă 0.6250.

 Testați cu descrierea unei cărți din books.csv (de exemplu, Dune). Ghicește genul?
 Da, Gen sugerat: Fantasy (Încredere: 84.2%) pentru descrierea O aventură epică într-o lume fantastică plină de elfi, vrăjitori, orci și un inel puternic care trebuie distrus. (am generat eu un alt books.csv pentru ca nu l-am mai avut)

 Testați cu o descriere ambiguă (de exemplu, „un detectiv pe o stație spațială”). Ce încredere
afișează?
Gen sugerat: SF (Încredere: 81.3%)

PARTEA B SARCINI
S1. Jurnalul Recomandărilor
  -creat entitatea `RecommendationLog` (reține `ReaderId`, `CreatedAt` și `BookIds`) și am configurat salvarea automată a acesteia în baza de date la fiecare apel al API-ului de recomandări.
  -implementat pagina de administrare Admin/Log, am folosit clasa `PaginatedList`
  -adăugat funcționalități de filtrare după ID-ul cititorului și interval de date, precum și sortare după data apelului.

S2. Dashboard:
-implementat pagina de administrare Admin/Dashboard incluzând carduri pentru statistici cheie (numărul total de evaluări, numărul de cititori distincți și media generală).
-integrat biblioteca Chart.js pentru a afișa două grafice interactive:
    1. Un grafic cu bare pentru distribuția notelor (1–5 stele).
    2. Un al doilea grafic pentru media evaluărilor pe genuri literare (Varianta B0).