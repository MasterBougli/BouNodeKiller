# BouNodeKiller

BouNodeKiller est un utilitaire Windows pour repérer les processus Node.js, comprendre ce qu'ils exécutent, puis en fermer un, plusieurs ou tous si besoin.

## Liens rapides

- [English](README.en.md)
- [Español](README.es.md)
- [Dépôt GitHub](https://github.com/MasterBougli/BouNodeKiller)
- [Faire un don via Streamlabs](https://streamlabs.com/bouglitv)

## Fonctionnalités

- liste les processus `node.exe` et `nodejs.exe`
- affiche la ligne de commande complète
- identifie le script ou la cible exécutée
- affiche l'utilisateur, le PID, le parent et l'heure de lancement
- montre le répertoire de lancement détecté
- permet de tuer le processus sélectionné
- permet de tuer tous les processus Node d'un coup

## État du projet

- application desktop Windows basée sur WPF
- version courante : `1.0.4`
- CI GitHub Actions à chaque push et pull request
- build de release automatique sur les tags commençant par `v`

## Publication

Les releases sont publiées automatiquement à partir des tags `v*`.

Exemple :

```bash
git tag v1.0.4
git push origin v1.0.4
```

## Dans l'application

L'interface inclut :

- une table des processus avec recherche
- une colonne pour le répertoire de lancement
- un affichage de version
- une fenêtre "À propos" avec les liens du dépôt et des releases

