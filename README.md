# IPTV Smarters per Xbox Dev Mode

## 📋 Prerequisiti

1. **Xbox Series X|S** con modalità **Dev Mode** attivata
2. **Account GitHub** (gratuito)
3. **Connessione internet** per GitHub Actions
4. Credenziali Xtream Codes (username, password, server URL)

**Nota:** Non serve Windows o Visual Studio! La build avviene automaticamente su GitHub.

---

## 🚀 Guida all'installazione su Mac M2 (senza Windows!)

### 1️⃣ Configura GitHub Actions

1. **Crea un repository GitHub**
   ```bash
   # Dal terminale, nella cartella del progetto
   cd /Users/utente/Documents/xbox-iptv-smarters
   
   # Inizializza git (se non hai gia')
   git init
   git add .
   git commit -m "Initial commit: Xbox IPTV Smarters"
   
   # Crea repo su GitHub e collega (dal browser: https://github.com/new)
   git remote add origin https://github.com/TUO-USERNAME/xbox-iptv-smarters.git
   git branch -M main
   git push -u origin main
   ```

2. **Attiva GitHub Actions**
   - Vai sul repository GitHub: `https://github.com/TUO-USERNAME/xbox-iptv-smarters/actions`
   - La workflow `build-xbox.yml` parte automaticamente a ogni `push`!
   - Puoi anche triggerarla manualmente: **Run workflow button**

3. **Attendi il build** (ci vogliono ~3-5 minuti)
   - GitHub compila il progetto su Windows-latest
   - Ti crea due pacchetti: `x64` e `ARM64`
   - Scarica il file `.zip` appropriato per la tua Xbox

### 2️⃣ Configura Dev Mode su Xbox

1. Sull'Xbox, vai a **My games & apps** → **Apps**
2. Cerca e apri **Remote Development** (se non c'è, installarlo dall'Microsoft Store)
3. In **Settings** → **System** → **Developer** → **Activate Dev Mode**
4. Segui le istruzioni per collegare l'Xbox al PC (o al network)
5. **Prendi nota dell'IP address** della Xbox (appare a schermo)

### 3️⃣ Deploy su Xbox (senza Visual Studio!)

#### Metodo A: Browser Xbox (più semplice!)

1. Dalla Xbox, apri **Microsoft Edge**
2. Vai a `http://XBOX-IP:11807/` (sostituisci XBOX-IP con l'IP preso sopra)
3. Clicca su **Browse Files**
4. Carica **TUTTI i file** estratti dal `.zip` di GitHub Actions
5. Torna indietro e clicca su **Install**
6. Wait 30-60 secondi, poi **Launch**

#### Metodo B: PowerShell remoto

```powershell
# Da un PC Windows (o da Parallels)
# Usa SSH o WinRM per connetterti alla Xbox
# PowerShell remoto: Connect-WSMan -ComputerName XBOX-IP
# Installazione: Add-AppxPackage -Path "C:\percorso\XboxIptvSmarters.appx"
```

#### Metodo C: USB (se il browser non funziona)

1. Estrai il `.zip` su una chiavetta USB
2. Inserisci la chiavetta nella Xbox
3. Dalla Dev Mode App, scegli **Install da USB**
4. Segui le istruzioni

---

## 🎮 Navigazione Xbox

### Controlli:

| Tasto | Azione |
|-------|--------|
| **A** | Conferma / Seleziona |
| **B** | Indietro |
| **D-Pad** | Navigazione su/giù/sinistra/destra |
| **Menu** | Menu contestuale / Opzioni |
| **View** | Info / EPG |

### Player video:

| Tasto | Azione |
|-------|--------|
| **A doppio** | Fullscreen / Torna alla lista |
| **A** | Play / Pausa |
| **B** | Esci dal player |
| **Trigger** | Volume |

---

## 📖 Caratteristiche

### ✅ Implementate

- **Login Xtream Codes** con credenziali salvate
- **Canali Live** con EPG integrato
- **VOD (Film)** con ricerca per genere
- **Serie TV** con navigazione stagioni/episodi
- **Player video** a schermo intero con controlli
- **Persistenza dati** locale (credenziali, impostazioni)
- **UI ottimizzata per Xbox** con focus navigazione

### ⚠️ Da migliorare

- Cache immagini per velocità
- Filtro ricerca per nome canale/film
- Lista "ultimi riprodotti"
- Preferiti / Segnalibri
- Supporto multi-profilo

---

## 🔧 Sviluppo

### Struttura del progetto:

```
xbox-iptv-smarters/
├── Models/           # Modelli di dati Xtream
├── Services/         # Logica API e persistence
│   ├── XtreamApiService.cs
│   ├── SettingsService.cs
│   └── VideoPlayerService.cs
├── Views/            # Pagine della UI
│   ├── LoginPage.xaml
│   ├── MainPage.xaml
│   ├── LiveChannelsPage.xaml
│   ├── VodPage.xaml
│   ├── SeriesPage.xaml
│   ├── PlayerPage.xaml
│   └── SettingsPage.xaml
├── App.xaml          # Entry point
└── XboxIptvSmarters.csproj
```

### Test locale:

```bash
dotnet run
# Oppure F5 in Visual Studio senza Xbox
```

---

## 🐛 Risoluzione problemi

### Problema: "Nessuna categoria trovata"
**Soluzione**: Verifica che le credenziali Xtream siano corrette e che il provider abbia canali abilitati.

### Problema: "Stream non riprodotto"
**Soluzione**: 
1. Verifica che il provider includa stream `.m3u8`
2. Controlla che il firewall non blocchi la porta 80/443
3. Prova su un altro canale/film

### Problema: Lentezza nel caricamento
**Soluzione**: Aggiungere cache per le immagini e i dati EPG.

---

## 📄 Licenza

Questa è un'applicazione dimostrativa sviluppata per scopi educativi. L'utente è responsabile per l'uso dei contenuti streaming.

---

## 🎉 Ringraziamenti

- **Xbox Dev Mode** per permettere lo sviluppo domestico
- **Xtream Codes** per l'API semplice e documentata
- **Community** per il supporto

---

**Versione**: 1.0.0  
**Compatibilità**: Xbox Series X|S (Dev Mode), Windows 10 19041+  
**Ultimo aggiornamento**: 2026-10-26
