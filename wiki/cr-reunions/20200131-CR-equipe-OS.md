# PI - Groupe OS | Compte-rendu de réunion - 31/01/2020





- **Type  :**  Réunion hors-séance, à distance (vocal sur Discord)

- **Date  :**  Vendredi 31 janvier 2020

- **Heure :**  21h15 -> 23h15 (2h)

- **Lieu  :**  *(chacun chez soi)*

- **Membres présents :** Tout le monde à l'exception de Choaïb et Aymeric







------------------------------------------------------
## 1. Déroulement de la réunion





### 1.1. Répartition prévisionnelle des membres pour le développement (21h20)


Pour le début du développement, nous avons mis en place **3 équipes principales** : une pour le noyau, une pour le réseau, une pour l'interface graphique et les menus.

Après discussion, nous sommes arrivés à une **répartition "initiale"** des membres au sein de chaque sous-équipe principale :

  * **noyau** : Choaïb°, Louis
  * **IG et menus** : Victor°, Hugo, Aymeric
  * **réseau** : Khalida°, Antoine


Nous avons également mis en place un **"responsable" d'équipe** pour chaque équipe, qui sera au coeur de cette équipe. (il s'agit des personnes suivies du signe '°')

Ce système évite que tous les membres se retrouvents "multi-équipe" : dans l'idée, en cas de ré-affectation de membre d'une équipe à une autre, ce ne sera pas lui qui changera. 
Un responsable pour chaque équipe permet d'avoir une personne qui sera, à priori, toujours membre de cette équipe et par conséquent connaîtra vraiment bien sa partie, son module.


En tant que **chef de projet**, Joren sera amené à travailler avec les différentes équipes, l'objectif étant qu'il ait une bonne connaissance de chaque partie et qu'il puisse faire le lien entre les équipes.
De cette manière, il pourra à tout moment disposer d'une vision globale du projet, de son organisation et de son avancement, afin qu'il puisse faire les bons choix en conséquence (réorganisation des équipes, changement dans le planning, décisions pour le développement...).

Par ailleurs, Joren s'occupera tout au long du projet de la réalisation des CR (de réunions, et d'échanges avec M. Decor le cas échéant), et initiera la réalisation des documents finaux (dossier de communication, rapport final).


La répartition pour l'IA et la BDD restent à définir, mais étant donné que l'un comme l'autre auront un lien avec le réseau, ce seront, à priori, les membres du module réseau qui seront affectés à ce module prioritairement.

L'organisation des tests sera aussi ré-abordée à la prochaine réunion.




---------------------------
### 1.2. Discussions sur le rôle du chef de projet (\~ 22h00)


Joren a décrit sa vision du **rôle du chef de projet et de ses fonctions** :
  
  * gestion de l'équipe (organisation, cohérence, répartition) et la communication entre tous les membres mais aussi entre le tuteur, M. Decor, et les membres de l'équipe

  * vision globale du projet, de son organisation, de son avancement, suivi des tâches ; afin de faire les bons choix en conséquence (réorganisation des équipes, changement dans le planning, décisions pour le développement)

  * travail avec les différentes équipes, pour avoir une bonne connaissance de chaque partie et faire le lien entre les équipes

  * prise en compte des remarques des membres de l'équipe (concernant l'organisation, le développement...)

  * gestion des éventuels conflits ou difficultés, qu'elles soient techniques ou humaines

  * préparation pour chaque réunion d'une "feuille de route" récapitulant où nous en sommes (l'état actuel du projet), ce qui a été fait dernièrement ainsi que ce qui est prévu pour la réunion (et éventuellement ultérieurement)

  * réalisation des CR (de réunions, et d'échanges avec M. Decor le cas échéant) : ces CR seront détaillés, de manière à ce que nous disposions d'une trace de tous les choix pris (de plus, la rédaction des CR aide à mémoriser les choix faits et permet un meilleur suivi des tâches)

  * réflexions, dès le mois de février, sur le rapport final, la documentation technique et le dossier de communication (attentes, contenu...), et réalisation du rapport final

  * gestion des rendus (sur Moodle ou Seafile)

Nous en avons discuté et nous nous sommes rejoint sur ces points.



---------------------------
### 1.3. Récapitulatif des choix des technologies et langages (\~ 22h10)


Nous utiliserons principalement :

  * C# avec Unity
  * SQLite
  * (langages web classiques)
  * (éventuellement d'autres langages en fonction des fonctionnalités)

Plusieurs membres ont déjà des compétences en Unity : Antoine, Louis, Victor, et aussi Khalida. Choaïb a des compétences en C#.

Nous avons récapitulé les points importants à ne pas négliger pour le développement sous Unity, notamment pour les novices :

  * le fonctionnement des scripts et l'utilisation de l'inspecteur
  * l'accès à des composants d'autres objets
  * la position des boutons par rapport au *canvas*
  * *aussi, la facilité de développement plus faible sous Linux est à retenir*



---------------------------
### 1.4. Discussions sur les plateformes utilisées et le rôle de chacune (\~ 22h30)


- Le **GitLab Unistra** : il regroupera le code du jeu ainsi que les *guidelines* et documents de production (CR, CDC, docs finaux). Il permettra aussi de gérer les *issues* (suivi des bugs/avancées).

- Le **Discord** : il servira à la communication globale, et permettra de nous échanger les infos, avec notamment des salons dédiés aux différents modules et éventuellement un système de rôles si beaucoup de changements d'équipes ont lieu, pour savoir qui travaille sur quoi.

- Le **Trello** : cet outil nous permettra d'assurer la gestion de notre planning et de gérer l'organisation globale du projet, ainsi que les tâches et leurs attributions.

- Un **outil permettra de "remplir les heures"** de chacun des membres : qui a fait quoi, quand, avec éventuellement les numéros des commits associés, pour du code. Il s'agira probablement d'un *Google Docs*.




---------------------------
### 1.5. Répartition des tâches pour la rédaction du CDC (\~ 22h50)


Avant de mettre fin à la réunion, nous sommes répartis les tâches pour la rédaction des différentes parties du cahier des charges.






------------------------------------------------------
## 2. Bilan





### 2.1. Ce qui a été fait, où nous en sommes


Nous avons abordé tous les points dont il nous fallait discuter :

  * Répartition prévisionnelle des membres pour le développement

  * Rôle du chef de projet

  * Récapitulatif des choix des technologies et langages

  * Plateformes utilisées et rôle de chacune (+ outil pour "remplir les heures")

  * Répartition des tâches pour la rédaction du CDC

Tous les points importants nécessaires pour réaliser le CDC et pour bien débuter le développement ont été abordés, discutés ensemble et précisés.


---------------------------
### 2.2. Difficultés rencontrées


Pas de difficulté particulière pour cette réunion. Malgré l'absence d'Aymeric et de Choaïb, nous avons bien avancé.

Les choix et décisions concernant Aymeric et Choaïb (place dans les équipes, partie à rédiger pour le CDC), qui ne pouvaient pas être présents, leurs ont été communiqué, afin qu'ils puissent valider ou non ces décisions qui ont été prise en leur absence.







------------------------------------------------------
## 3. À venir : ce qui est prévu




### 3.1. Le week-end du 1er et du 02 février


- Finir la rédaction du cahier des charges : rédiger les parties qui n'ont pas encore été faites

- Joindre les annexes (diagramme d'activité, UML, diagramme de Gantt, wireframes)

- Produire un document PDF du CDC et voir comment y joindre les annexes (soit en documents à part, soit au sein du CDC). Assurer une mise en page claire, propre et une mise en forme cohérente au sein du document.



---------------------------
### 3.2. À la prochaine séance (séance #03, mardi 04 février)


- Discuter du cahier des charges avec notre tuteur, M. Decor, afin qu'il le valide ou non.


- Mettre en place un outil permettant de "remplir les heures" (pour décrire, pendant toute la durée du projet, qui a fait quoi + quand + le n° des commits associés, s'il s'agit de code).
Il s'agira, à priori, d'un *Google Docs* mis en commun, qui nous permettrait une mise à jour pratique tout en assurant un suivi précis. Le document sera mis en forme d'une manière spécifique, avec éventuellement la mise en place d'un code couleur.


- Récapituler qui a fait quoi pour le cahier des charges (pour le suivi du travail) et demander s'il faut compléter le "remplissage des heures" pour les deux semaines qui viennent de s'écouler.


- Mettre en place les conventions que nous utiliserons pour le code. Tout ceci, une fois défini, sera regroupé dans un fichier récapitulatif. En respectant ces conventions, nous pourrons dès le début du développement nous assurer d'avoir un code homogène et cohérent dans sa globalité. S'accorder sur :

  * langue du code
  * langue pour les commentaires
  * "casse" des mots (variables, fonctions, constantes... + standards C# ?)
  * conventions pour la mise en forme (accolades, indentation...)


- Voir comment gérer la documentation (Doxygen, génération de document...)


- Concernant les plateformes utilisées, voir ensemble comment nous allons organiser :

  *  le Trello (gestion globale, fonctionnement, organisation des tâches, mise à jour, rôles ?,...)

  * le Discord (salon, rôles ?, ...)

  * le GitLab Unistra (gestion des commits, pull/push, merge, conflits, + ne pas oublier de récupérer les ajouts du dépôt central... + // expériences)


- Discuter de l'organisation des prochaines semaines + (// Joren) du rôle du chef de projet : vision globale, développement des connaissances et compétences dans chaque domaine du développement, début de réflexion + obtention d'informations sur les docs à produire (doc technique, dossier de communication, rapport final) et sur les attentes de l'oral (durée, déroulement)


- Voir comment nous organiserons nos tests au fur et à mesure du développement, qui s'en chargera et à quel moment


- Discuter des points restant à préciser (cf section 3.3.) avant de pouvoir démarrer vraiment la spécification


- Débuter la spécification du noyau, du réseau, et de la communication entre les modules, qui nous occupera pour les jours à venir.




---------------------------
### 3.3. Points à rediscuter à la prochaine séance


- Préciser certains points dans les menus : flèche retour d'un écran à un autre (+ emplacement homogène) ? boutons grisés/non-grisés ?


- Rediscuter de l'IA : commencer par une IA "useless" ? plusieurs niveaux de difficulté ? comment gérer la prise en charge de la partie d'un joueur lorsqu'il se déconnecte ?


- Rediscuter des extensions envisageables (ex : 6 joueurs), et de ce que ça aurait comme conséquences sur le temps de développement, la spécification du noyau...


- Pour les options

  * gestion des changements de langue et du son (les modifications peuvent se faire directement ou non ?)

  * autres options nécessaires ?

  * gestion des options lorsqu'on passera sur web/mobile


- Pour le réseau

  * préciser comment sera gérée cette partie (hébergement des parties en ligne, des comptes, de l'IA...)

  * revenir sur l'éventuel chat vocal : comment gérer ça ? tutoriel ? idées ? temps de développement et difficulté VS potentiel


- À la création d'une partie de jeu en local :

  * choix du niveau de difficulté des bots (// IA) ?

  * choix explicite d'une partie locale solo ou à plusieurs, ou choix fait automatiquement en fonction du nombre de joueurs sélectionné ? (1 VS >1)


- Concernant une partie *in-game*, en cours de jeu, revenir sur plusieurs points :

  * fonctionnalité permettant d'afficher, dans le chat, l'historique des actions qui ont eu lieu au cours de la partie (lors d'un échange, d'une construction...) : affichage de quelles informations ? difficulté ? potentiel, intérêt ? comment gérer ça ?

  * chat : code couleur pour chaque joueur + couleur spécifique pour l'historique des actions, si implémenté ? choix laissé libre au joueur ou attribution aléatoire ?

  * nombre de chaque ressource possible : limitation à 99 ? (// décision d'afficher les ressources toujours avec 2 chiffres)

  * points de victoire : affichage, ou non, et comment les calculer (prise en compte de certaines cartes ou non)

  * cartes ressources et développement des autres joueurs : affichage ou non ? cartes cachées tout le temps ?

  * constructions restantes : affichage ou non ? limitations ? (juste les notres ou celles de tout le monde ?)

  * commerce pour une partie en ligne ET en local (modélisation d'une session d'échange ; + qui peut proposer un échange, + qui peut fermer la session, comment et à quel moment)

  * "orientation" (sens) du plateau de jeu ; rotation possible ? (si non, veiller à garder des icônes de construction "symétriques", "centrées")

  * spécificités d'une partie de jeu en local à plusieurs

    - chat laissé quand même (vide, ou pour l'historique des actions) ?
    - gestion des échanges (ressources de tout le monde en public, accords faits à l'oral) ?
    - position des cartes et affichages des ressources
