# BOU-NodeJSKiller

Application Windows pour repérer les processus Node.js, voir ce qu'ils exécutent, puis en fermer un, plusieurs, ou tous.

## Base retenue

La première version part sur **WPF**, pour aller vite avec une interface native et un accès simple aux processus Windows.

## Ce que l'application fera

- lister les processus `node.exe` et `nodejs.exe`
- afficher la ligne de commande complète
- identifier le script ou la cible lancée
- montrer l'utilisateur, le PID, le parent et l'heure de lancement
- tuer un processus sélectionné
- tuer tous les processus Node d'un coup

## Pistes alternatives

1. WPF
2. WinUI 3
3. Electron

La version courante part sur le choix 1 pour le démarrage.

