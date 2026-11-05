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

\* Filtrul implementat: \*\*\\\[ ]\*\*
\* Interogarea , cu filtrul aplicat:

```sql
\\\[lipiți aici]
```

## S4 · Folosirea AI

\* \*\*Instrumente folosite:\*\* \\\[nume, sau „nu am folosit”]
\* \*\*La ce pași:\*\* \\\[ ]
\* \*\*Un exemplu de cerere trimisă:\*\*

> \\\[textul cererii]

\* \*\*O situație în care am corectat sau respins un răspuns AI\*\* (sau, dacă nu am folosit AI, o eroare întâlnită și cum am găsit-o):

  \* Ce a propus AI-ul / care era eroarea: \\\[ ]
  \* Cum mi-am dat seama că e greșit sau ineficient: \\\[ ]
  \* Codul inițial:

```csharp
\\\[ ]
```

\* Codul final:

```csharp
\\\[ ]
```

\\---

## Ce nu am reușit (opțional)

\\\[Ce nu funcționează sau nu este complet și de ce. Este mai bine scris aici decât descoperit la corectare.]


