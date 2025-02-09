## ⛓️‍💥 KoBlockchain.P2P
Serwis po odpaleniu którego startuje tzw. node w sieci blockchain. Zasada działania kazdego blockchaina opiera się na p2p (peer-to-peer) co oznacza ze każdy węzeł (node), pełni rolę klienta i serwera jednocześnie.
Jak to wygląda w moim przypadku?  
![image](https://github.com/user-attachments/assets/a18b8504-bed7-43ca-a526-6ecb2b404286)

- Czyli node to np. komputer osoby gdzie jest uruchomiony ten serwis.
- Po startowaniu serwisu, tworzy sie podreczna baza danych SQLite oraz tzw. **Genesis block**
- Rowniez sa uruchamiane 3 joby: Generowanie losowych transakcji, Generowanie blokow na podstawie transakcji, Synchronizacja blokow z pozostalymi node'ami
- Do tworzenia hash'u blocku uzyty mechanizm SHA-512

## Uzyte technologie
- ASP.NET 8 Web API - jako host calego serwisu
- Dapper - micro-orm zeby prosciej bylo komunikowac sie z baza
- SQLite - podreczna baza nie wymagajaca postawionego serwera
- Hangfire - do uruchamiania background jobów
