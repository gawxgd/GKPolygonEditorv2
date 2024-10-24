 # Polygon Editor
 ## Ekran startowy 
 Na ekranie startowym wyświetla się scena początkowa wyświetlająca przykładwowy wielokąt (z ograniczeniem i dwoma segmentami beziera)
 ## Top Menu
 ### Change Line Algorithm
 Przycisk pozwalający na przełączanie się między bibliotecznym algorytmem rysowania krawędzi między wierzchołkami, a własną implementacją algorytmu bresenhama.
 ### Draw New Polygon
 Przycisk przełącza na tryb rysowania nowego wielokąta, po wciśnięciu usuwany jest stary wielokąt, czyszczone jest Canvas a następnie zostaje wyświetlony MessageBox z instrukcją 
 jak rysować nowy wielokąt. W celu narysowania wielokąta należy wcisnąć lewy przycisk myszy w miejscu w którym chcemy dodać wierzchołek (krawędzie dodawana są automatycznie), 
 a na koniec prawy żeby zatwierdzić operacje i dodać 
 krawędź między ostatnimi dwoma wierzchołkami (nie połączynymi).
 ### Delete Selected Vertex
 Żeby zaznaczyć wierzchołek należy kliknąć go prawym przyciskiem myszy. Poprawne zaznaczenie jest indykowane zmianą koloru wierzchołka na czerwone.
 Po wciśnięciu Delete Selected Vertex zostanie usnięty wierzchołek i sąsiadujące z nim krawędzie, a następnie zostanie dodana krawędź pomiędzy sąsiadami usuniętego wierzchołka.
 ### Move Polygon
 Po włączeniu trybu Move Polygon możliwe jest przesuwanie całego wielokąta. 
 W celu przesunięcia wielokąta wystarczy wcisnąć lewy przycisk myszy w obszarze ograniczonym przez wielokąt i przesunąć mysz w żądane miejsce (cały czas trzymając wciśnięty przycisk).
 Po włączeniu trybu zostanie wyświetlony MessageBox polygon drawing enabled, 
 w celu wyłączenia trybu należy ponownie wcisnąć przycisk Move Polygon, zmiana trybu zostanie potwierdzona odpowiednim MessageBoxem.
 ## Operacje na wielokącie 
 ### Przesuwanie wierzchołka / punktu kontrolnego
 Należy przytrzymać lewy przycik myszy na wybranym wierzchołku i przesunąć go ruchem myszki.
 ### Zaznaczanie wierzchołka
 Należy kliknąc wybrany wierzchołek prawym przyciskiem myszy. Poprawne zaznaczenie jest potwierdzone zmianą koloru wierzchołka na czerwony.
 ### Zaznaczanie krawędzi / segmentu beziera
 Należy kliknąć wybraną krawędź / segment prawym przyciskiem myszy. Poprawne zaznaczenie jest potwierdzone zmianą koloru krawędzi na czerwony.
 ### Dodawnie wierzchołka
 Należy zaznaczyć krawędź na której chcemy dodać wierzchołek a następnie z menu kontekstowgo wybrać opcję Add Vertex, wierzchołek zostanie dodany w połowie krawędzi, 
 z krawędzi zostaną usunięte ograniczenia.
 ### Nadawanie ograniczeń krawędziom
 Należy zaznaczyć wybraną krawędź prawym przyciskiem myszy, zostanie wyświetlone menu kontekstowe z którego można wybrać żądane ograniczenie:
 1. Pozioma krawędź - Make horizontal
 2. Pionowa krawędź - Make vertical
 3. Krawędź o ograniczonej długości - Set distance. W przypadku wybrania tego ograniczenia wyświetlone zostanie okienko popup z polem tekstowym w którym należy wpisać żądaną długość krawędzi, domyślna wartość 200

**Uwaga:** w celu poprawnego nadania ograniczenia należy upewnić się że krawędź nie ma ustawionych ograniczeń (można je usunąć opcją Remove Constraints), nie jest segmentem beziera 
(można przełączyć na zwykła przy użyciu Switch Bezier)
, a także w przypadku ograniczenia poziomego lub pionowego że sąsiednie krawędzie nie mają ograniczeń tego samego typu. 
Poprawne wyświetlenie ograniczenia może nastąpić dopiero po przesunięciu dowolnego wierchołka przez użytkownika
### Usuwanie ograniczeń
W celu usunięcia ograniczeń trzeba upewnić się że krawędź posiada ograniczenie, a następnie zaznaczyć ją i wybrać z menu kontekstowego opcję Remove Constraints.
### Przełączanie segmentu beziera
Należy upewnić się że krawędź nie ma ograniczeń (wpp. usunąć ograniczenia), a następnie zaznaczyć krawędź i z wybrać z menu kontekstowego opcje Switch Bezier, 
w celu przełączenia w zwykła krawędź należy postąpić analogicznie.

**Uwaga** po ustawieniu segmentu beziera wierzchołkom należącym do krawędzi zostanie ustawiona ciągłość G1 (nawet jeśli wierzchołki już miały ustawione jakąś klasę ciągłości), 
w przypadku przełączenia segmentu w krawędź wierzchołki zostaną pozbawione klasy ciągłości.
### Nadawnanie ciągłości wierzchołkom
W celu nadania ciągłości wierzchołkowi należy upewnić się że należy do segmentu beziera, a następnie zaznaczyć go i z menu kontekstowego wybrać odpowiednią klasę ciągłości:
1. G0
2. G1
3. C1



  
 
