#!/bin/bash
# Script per pushare su GitHub - Copia e incolla nel Terminale

# 1. Inizializza il repository
git init

# 2. Aggiungi tutti i file
git add .

# 3. Fai il primo commit
git commit -m "✨ Initial commit: Xbox IPTV Smarters con credenziali preconfigurate

- Credenziali: M99SVERR @ xxczsmart.com
- Build automatico via GitHub Actions
- Supporta Live TV, VOD e Serie TV
- Player video full-screen con EPG"

# 4. Configura il remote repository
git remote add origin https://github.com/maxmorrison74/xbox-iptv-smarters.git

# 5. Rinomina il branch in 'main' (se necessario)
git branch -M main

# 6. Push su GitHub
git push -u origin main

echo ""
echo "=========================================="
echo "✅ Progetto pubblicato su GitHub!"
echo "=========================================="
echo ""
echo "Prossimi step:"
echo "1. Vai su: https://github.com/maxmorrison74/xbox-iptv-smarters/actions"
echo "2. Clicca su 'Run workflow' (o aspetta il trigger automatico)"
echo "3. Attendi 3-5 minuti che GitHub buildi il progetto"
echo "4. Scarica XboxIptvSmarters_ARM64.zip (consigliato per Xbox Series X|S)"
echo "5. Segui le istruzioni di installazione nel README"
echo ""
echo "🎮 Buona visione!"
echo "=========================================="
