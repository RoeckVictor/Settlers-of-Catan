# PI - Groupe OS | Compte-rendu de réunion - 30/01/2020





- **Type  :**  Réunion hors-séance, en présentiel

- **Date  :**  Jeudi 30 janvier 2020

- **Heure :**  13h10 -> 15h20 (2h10)

- **Lieu  :**  UFR, Salle C9

- **Membres présents :** Tout le monde







------------------------------------------------------
## 1. Déroulement de la réunion





### 1.1. Description de l'interface en cours de jeu (*in-game*) (13h10)



Nous avons poursuivi ce qui avait été commencé à la réunion du mardi 28/01, en détaillant le contenu de l'écran, l'affichage et l'emplacement des éléments en cours de partie.



##### Plateau de jeu

Affichage du plateau (la "carte" de l'île) au centre de l'écran



##### Chat

Présence d'un chat sur la droite de l'écran. Le chat reste toujours accessible quoiqu'il arrive, que l'utilisateur consulte les options, les règles, soit en train de faire du commerce ou joue tout simplement. De cette manière, chaque joueur peut continuer de discuter et de consulter le chat à tout moment.

/!\ Fonctionnalité envisageable : afficher, dans le chat, l'historique des actions qui ont eu lieu au cours de la partie (lors d'un échange, d'une construction...).
Cela pourrait être intéressant, nous en rediscuterons.



##### "Menu" au-dessus du chat

Toujours sur la droite, au-dessus du chat, une sorte de *header* affiche à l'utilisateur certaines informations (à déterminer) et des boutons lui permettent (de gauche à droite) d'accéder aux options, de consulter les règles ou de quitter la partie.

Le menu d'options comme celui des règles s'affiche par-dessus la partie en cours (laissant le chat et le *header* toujours consultables).

Le fait de vouloir quitter la partie demande à l'utilisateur de confirmer avant de réellement quitter le jeu.



##### Affichage des informations du joueur

Les informations du joueur (pseudo, ressources, points de victoire, avatar...) sont affichées sur la gauche de l'écran, sous la forme d'icônes représentant les différentes ressources disponibles ainsi que le nombre actuel dont dispose l'utilisateur.

Lors de l'attribution des ressources, le compteur de chaque ressource "acquise" sera incrémenté. *(dans l'idéal, une animation / transition permettra à l'utilisateur de voir plus clairement les nouvelles ressources acquises)*


/!\ Point important décidé : quoiqu'il arrive, le nombre d'une ressource donnée dont  l'utilisateur dispose sera toujours affiché au moyen de 2 chiffres (ex: x01, ... x09, x10, ...). Cela nous permettra d'emblée d'être sûrs d'avoir un affichage des valeurs toujours cohérent et d'éviter d'éventuels soucis de positionnement.
Les ressources seront peut-être limitées à 99 pour éviter tout souci, même si la possibilité d'atteindre un tel nombre paraît peut probable. Nous en reparlerons.



##### Affichage des informations des autres joueurs

À proximité de l'affichage des informations du joueur se trouvent plusieurs icônes, représentant les avatars des autres joueurs dans la partie : en survolant l'icône de l'avatar d'un joueur avec sa souris, les informations du joueur dont il s'agit seront présentées à l'utilisateur "par-dessus" l'écran.
De cette manière, les informations des autres joueurs (nombre de cartes, points de victoire...) resteront accessibles à tout moment sans prendre trop de place pour autant.



##### Construction

Pour la construction, des icônes sur la gauche, vers le haut de l'écran seront disponibles, pour construire une colonie, une ville, une route... Ces boutons sont grisés si l'utilisateur n'a pas les ressources nécessaires ; autrement, ils sont cliquables.

Lorsque l'utilisateur place sa souris sur l'icône, au bout d'un court laps de temps, une info-bulle apparaît et présente à l'utilisateur des informations sur la construction : les ressources nécessaires, les points de victoire apportés...

En cliquant sur une des icônes, les positions possibles pour la construction demandée sont affichées en évidence sur le plateau de jeu. L'utilisateur peut alors sélectionner la zone où construire, et cliquer sur celle de son choix.

Il peut alors confirmer vouloir construire à cet endroit, en cliquant sur un bouton spécifique. S'il le souhaite, l'utilisateur peut également construire ailleurs avant de confirmer. Enfin, en re-cliquant sur l'icône de construction, il peut annuler la construction en cours.
En revanche, une fois la construction confirmée, celle-ci est réalisée : l'utilisateur se voit retirer les ressources nécessaires et il ne peut revenir sur sa décision.



##### Cartes développement

- **Achat** : une icône se trouve sur la gauche, vers le haut de l'écran, à côté des icônes de construction. En cliquant dessus, une confirmation est demandée à l'utilisateur s'il veut acquérir une carte développement. En cas de validation, l'utilisateur gagne une de ces cartes.

- **Utilisation** : les icônes des cartes développement jouables sont situées près des autres boutons d'action (construction, ...). L'utilisateur souhaitant jouer une carte développement a le choix parmi celles dont il dispose.

- **Consultation** : les icônes des cartes développement qu'a l'utilisateur sont situées près de l'affichage des autres ressources dont il dispose



##### Commerce (pour une partie en ligne)

Une icône, probablement vers la gauche, permettra de lancer une session d'échange entre le joueur dont c'est le tour et tous les autres joueurs.

La session d'échange dispose d'une fenêtre spécifique qui se place par dessus le plateau de jeu, laissant le chat (sur la droite) toujours accessible pour que les joueurs puissent communiquer au cours des échanges.

Dans cette fenêtre, chaque utilisateur peut proposer des ressources à l'utilisateur dont c'est le tour. Cet utilisateur peut lui aussi proposer des ressources en échange.
Une fois qu'un des utilisateurs et l'utilisateur dont c'est le tour se sont mis d'accord, ils peuvent confirmer les ressources qui vont être échangées. Si les deux parties acceptent, l'échange est réalisé et la session se finit.

Si aucune offre ne convient, l'utilisateur ayant lancé la session peut l'interrompre (à rediscuter).

En "globalisant" les échanges de cette manière, cela évite d'avoir des sessions seulement entre deux joueurs, ce qui ralentirait nettement le rythme de la partie et nécessiterait autant de sessions d'échanges que de joueurs souhaitant faire du commerce.
De cette manière, l'échange a lieu entre tous les joueurs au même moment, tout le monde peut voir ce qui se passe et communiquer.



##### Brigand et défausse

Lorsque le brigand est "activé" (lors d'un 7 aux dés), chaque joueur ayant 8 ressources (ou plus) doit défausser la moitié d'entre elles : il peut sélectionner, parmi ses ressources, lesquelles il veut "jeter". Une zone spécifique de l'écran lui permet alors de voir ce qu'il met dans la défausse.

Lorsque l'utilisateur clique sur une des ressources dont il veut se débarasser, cette ressource est placée dans la défausse. Cette action décrémente le compteur de la ressource associée pour l'utilisateur et incrémente le compteur de cartes demandées dans la défausse.

En cliquant sur cette ressource dans la défausse, celle-ci est remise dans ses ressources : cette action décrémente le compteur de cartes demandées dans la défausse et incrémente le compteur de la ressource associée pour l'utilisateur.

Ainsi, l'utilisateur peut sélectionner comme bon lui semble les ressources dont il veut se débarasser et peut revenir sur certains de ses choix.

Une fois que le nombre de cartes à "jeter" est atteint, l'utilisateur peut confirmer les cartes choisies : alors, c'est seulement à ce moment que les ressources dans la défausse sont réellement supprimées.



##### Lancer de dés

En début de tour, pour le lancer de dés : des dés sont représentés quelque part sur l'écran, et un clic de l'utilisateur dont c'est le tour déclenche un lancer de dés aléatoire. De cette manière, cela donne dans une certaine mesure l'illusion à l'utilisateur que c'est lui qui lance les dés, et que ce n'est pas simplement une génération de chiffres aléatoire réalisée par l'ordinateur.




##### Spécificité d'une partie en local

Une partie de jeu en local à plusieurs présente certaines différences à prendre en compte :
  
  * présence du chat ?
  * gestion des échanges (ressources de tout le monde en public, accords faits à l'oral) ?
  * position des cartes et affichages des ressources ?

Ces questions n'étant pas prioritaires, nous en reparlerons à la prochaine séance de mardi.





---------------------------
### 1.2. Ordre et durée estimée des tâches pour réaliser le projet (\~ 14h50)



Nous avons discuté entre nous de l'organisation du projet sur la durée du semestre : l'ordre dans lequel doivent être réalisées les tâches et leur durée estimée.


- **Prioritairement :**
  
  * spécification du noyau, du réseau (prototype des fonctions, classes...), de la BDD, de l'IA + communication entre les modules
  [10 à 15 jours]

  * développement du noyau

  * développement de l'IG, des menus, de la partie

  * développement du réseau, mise en place de la partie serveur
  [pour le noyau/IG/réseau : 4 à 5 semaines]

  * développement de la BDD et du système de compte
  [1 à 2 semaines]

  * développement de l'IA (avec prise en charge du réseau)
  [2 semaines]

  * *(remarque : le développement de certains modules, par exemple noyau/réseau/IG, se fera en parallèle, en essayant d'intégrer les différents modules entre eux au fur et à mesure)*


- **Dans un second temps :**

  * jeu sur web / mobile
  [pour le web et les tests associés : 1 semaine]

  * traduction
  [moins d'1 semaine]

  * améliorations graphiques, animations, transitions, finitions
  [le temps qu'il nous restera]

  * autres fonctionnalités envisagées mais bien moins prioritaires
  [le temps qu'il nous restera]


- **Vers la fin du projet :**

  * préparation du dossier de communication
  [commencer environ 1 mois avant]

  * réaliser le rapport final
  [commencer plus d'1 mois avant, et dès février, voir ce qui sera attendu]


- **Sans oublier, tout au long du projet :**

  * tests

  * comptes-rendus successifs

  * réalisation de la documentation technique
  [au fur et à mesure, avec des ajouts éventuels - en fonction de ce qui est attendu - à partir de début mars]



Nous avons aussi tâché de prendre en compte nos obligations au cours des 3 mois qui vont suivre : si le début du semestre est plus léger (peu de TPs, pas de CCs), nous sommes conscients que plus nous avancerons dans le semestre, plus nous auront de choses à faire à côté du PI (examens, projets...).
Nous avons essayé d'en tenir compte dans notre planification, en faisant le plus de choses possibles dès le début du projet et en essayant d'avoir constamment une vue globale de l'avancement.

Par ailleurs, nous sommes conscients qu'il y aura forcément des imprévus, des aléas techniques et/ou humains, ou des retards : c'est pourquoi nous allons faire en sorte de finir notre projet pour le 21 avril, soit deux semaines avant le rendu. Cela permet de "nous laisser" un peu de marge.

Aussi, du fait que nous devrons produire en cours de projet des prototypes jouables, il a été jugé important d'"intégrer" les différents modules entre eux au fur et à mesure, afin de ne pas commencer à vouloir faire communiquer les modules entre eux à quelques semaines de la fin.

Tout cela sera récapitulé de manière plus claire au sein d'un diagramme de Gantt.









------------------------------------------------------
## 2. Bilan





### 2.1. Ce qui a été fait, où nous en sommes


Nous avons bien avancé dans nos réflexions et précisé des points importants :

  * l'écran, l'interface en cours de jeu (*in-game*) avec l'emplacement des éléments, l'affichage des informations (et un début de réflexion sur les spécificités propre aux parties en local / en ligne)

  * l'ordre et la durée estimée des tâches pour réaliser le projet


Les quelques points restant à aborder le seront à la réunion du vendredi 31/01, après quoi tous les points importants nécessaires pour réaliser le CDC et pour bien débuter le développement auront été abordés, discutés ensemble et précisés.




---------------------------
### 2.2. Difficultés rencontrées


Pas de difficulté particulière à souligner pour cette réunion, nous avons été productifs et les réflexions ont été fructueuses.







------------------------------------------------------
## 3. À venir : ce qui est prévu




### 3.1. À la prochaine réunion (vendredi 31 janvier, sur Discord)


Discuter des points restants :

- Répartition prévisionnelle des membres pour le développement

- Choix des technologies et langages : passer en revue pour les fonctionnalités, noter les aspects importants, les avantages/inconvénients

- Rôle du chef de projet

- Plateformes utilisées et rôle de chacune (+ outil pour "remplir les heures")

- Répartition des tâches pour la rédaction du CDC
