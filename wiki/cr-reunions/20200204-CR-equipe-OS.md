# PI - Groupe OS | Compte-rendu de réunion - 04/02/2020





- **Type  :**  Séance dédiée #03

- **Date  :**  Mardi 04 février 2020

- **Heure :**  13h30 -> 17h30 (4h)

- **Lieu  :**  UFR, Salle T24

- **Membres présents :** Tout le monde à l'exception de Choaïb
                         (qui était malade)







------------------------------------------------------
## 1. Déroulement de la réunion





### 1.1. Retour sur le cahier des charges (13h30)


Discussions sur le **cahier des charges et l'avis** de M. DECOR : notre projet est validé et nous allons pouvoir commencer dans la "phase active", en débutant la spécification et le développement.

Retour sur les remarques de M. DECOR, les points bien faits et ceux à améliorer.



**Concernant les points "mal faits" / à revoir** qui ont été soulignés dans cet avis, nous avons ainsi eu une rediscussion à ce sujet dont voici un bref bilan.


- **Choix d'adaptation des mécaniques / fonctionnalités sur support informatique par rapport à la version "physique" du jeu de plateau** : nous en avions parlé mais n'avons effectivement pas mentionnés ce point dans notre compte-rendu. Certaines fonctionnalités sont en effet adaptées différement :

    * **Lancer de dés** : en début de tour, des dés sont représentés quelque part sur l'écran, et un clic de l'utilisateur dont c'est le tour déclenche un lancer de dés aléatoire. De cette manière, cela donne dans une certaine mesure l'illusion à l'utilisateur que c'est lui qui lance les dés, et que ce n'est pas simplement une génération de chiffres aléatoire réalisée par l'ordinateur.

    * **Affichage des ressources et des informations** : une ressource n'est pas matérialisée par une carte mais par une valeur associée à une icône (par exemple, du bois ou de la pierre). Ainsi, une ressource supplémentaire ne se traduit pas par une carte en plus mais par une incrémentation du compteur associé à la ressource nouvellement acquise.

    * **Commerce** : là aussi, cette facette du jeu diffère pour son adaptation lors d'une partie en ligne, les joueurs n'étant pas "physiquement" présents dans la même pièce. Ainsi, les sessions d'échanges sont lancées via une icône, probablement vers la gauche de l'interface : cette session a lieu entre le joueur dont c'est le tour et tous les autres joueurs.
    La session d'échange dispose d'une fenêtre spécifique qui se place par dessus le plateau de jeu, laissant le chat (sur la droite) toujours accessible pour que les joueurs puissent communiquer au cours des échanges. Dans cette fenêtre, chaque utilisateur peut proposer des ressources à l'utilisateur dont c'est le tour. Cet utilisateur peut lui aussi proposer des ressources en échange.
    Une fois qu'un des utilisateurs et l'utilisateur dont c'est le tour se sont mis d'accord, ils peuvent confirmer les ressources qui vont être échangées. Si les deux parties acceptent, l'échange est réalisé et la session se finit.
    Si aucune offre ne convient, l'utilisateur ayant lancé la session peut l'interrompre


- **Priorisation des tâches** : il est vrai que le niveau de priorité de certaines tâches aurait pu être réfléchi davantage, notamment les "réglages sonores" au niveau 3, qui pourraient être au niveau 2. Concernant le niveau 2 attribué à l'implémentation web/mobile, il s'agit d'une erreur de notre part, étant donné que, comme cela est souligné dans l'avis rendu, il s'agit de la "phase 2" du projet. Enfin, certaines fonctionnalités ont été mises au niveau 3, en raison de leur court temps de mise en place au regard de ce que cela peut apporter (comme la traduction), ou en raison de leur importance afin que le jeu soit jouable et procure une expérience intéressante (c'est le cas du *chat*).


- **Architecture** : chaque client aura en effet sa copie du jeu. Dans le cas d'une partie en local, solo ou multi, cela ne pose pas de problème, puisque la partie est seulement gérée du côté d'un client en particulier. Par ailleurs, lors d'une partie en ligne, la partie sera gérée du côté serveur, et non par chaque client localement. C'est donc le serveur (plus exactement, un module noyau, côté réseau) qui s'occupe de gérer toute partie en ligne, de manière "centrale".


- **Justification de la difficulté** : il est vrai que nous aurions pu donner davantage de détails sur cet aspect. Pour ce qui est de Unity3D, quatre membres de l'équipe (Antoine, Khalida, Louis, Victor) ont déjà eu une expérience avec cet outil, et un autre (Choaïb) a déjà de bonnes notions en C#. Aymeric, Hugo, Joren vont donc se former sur ces technologies : part eux-mêmes mais aussi par les autres membres du groupe qui ont déjà eu une expérience avec ces outils. Pour ce qui est du réseau, peu de membres du groupe se sentent réellement compétents dans ce domaine ; mais là encore, certains ont des notions plus avancées et mettront leur connaissances en commun avec le reste de l'équipe afin que la formation soit plus rapide et efficace.


- **Outils pour la gestion du travail et concurrence** : l'avis envisage la possibilité que les *issues* sur GitLab entre en concurrence avec les notes sur Trello. Il ne devrait pas y avoir de souci à ce niveau : en effet, Trello est exclusivement dédié à la gestion du projet (ce qui est à faire, qui doit le faire, les taĉhes en cours et celles prévues...) ; cet outil ne sera pas directement utilisé pour *report* des erreurs, ou des *bugs*, et c'est bien les *issues* GitLab qui serviront à cela.
Nous avions déjà discuté dans une réunion précédente des plateformes et de leur rôles, afin d'éviter, justement, d'éventuelles redondances.


- Enfin, concernant **l'organisation et la répartition en groupes** : nous avons conscience que faire de la répartition un paramètre ajustable est à double tranchant, comme il est écrit dans l'avis rendu. L'ensemble des membres, et plus particulièrement le chef de projet, porteront une attention particulière à tout changement d'équipe d'un membre et chaque ré-affectation sera réfléchie, afin, justement, de garder le projet sur la bonne voie et de garder une bonne gestion d'équipe.




---------------------------
### 1.2. Discussions rapides sur quelques points (\~ 13h55)


- Discussion et définition des conventions et *guidelines* que nous utiliseront pour le projet

- Réflexions sur la documentation : Doxygen ou autre ? Nous y réfléchirons dans les jours à venir.

- Récapitulatif de l'organisation pour les prochaines semaines et des prochaines tâches à réaliser (début de l'IG, spécification noyau + réseau).

- Retour rapide sur quelques points qu'il fallait préciser (options à implémenter, gestion des changements de la langue ou du son avec effet immédiat : tout est ok)




---------------------------
### 1.3. Débuts de spécifications, réflexions concernant les structures de données et les échanges entre modules (\~ 14h10)


La suite de cette séance a principalement été axée sur nos tâches prioritaires pour le développement du projet : à savoir, la **spécification du noyau et du réseau, les structures de données et les échanges entre modules**.

Nous avons donc beaucoup réfléchi et discuté à ce niveau, notamment concernant la structure de données du plateau de jeu, et son implémentation. Plusieurs hypothèses de structures ont été envisagées (tableaux, graphes) ainsi que diverses représentations possibles (notamment les "cube coordinates" pour la représentation de grilles hexagonales).

Le travail sur ces points a bien avancé pendant cette réunion, il continuera cette semaine et à la prochaine séance.




---------------------------
### 1.4. Travail en équipes (\~ 16h15)


Jusqu'à la fin de la séance, nous avons ensuite **travaillé en équipes**, selon la répartition que nous avions déterminé (noyau, IG, réseau).

Le travail a donc, entre autres, porté sur les structures de données, la recherche d'assets, la réflexion sur la partie réseau et serveur...

Joren a quant à lui consulté les différentes équipes pour voir le travail réalisé et participer aux réflexions.


Au cours de ces travaux, plusieurs ressources et liens éventuellement utiles ont été trouvés / cités. Les voici pour mémoire :

- réseau : *SocketWeaver*

- adaptation web/mobile : *Flutter*

- assets : *itch.io*






------------------------------------------------------
## 2. Bilan





### 2.1. Ce qui a été fait, où nous en sommes


Cette séance nous a permis de **revenir sur le cahier des charges** : celui-ci a été jugé valide et nous pouvons commencer "réellement" notre projet. Certains points du CDC avaient été peu développés, ou mal faits : nous avons rediscuté de ces éléments et avons pu les préciser (cf section 1.1. de ce CR).

Nous avons ensuite entamé la **spécification du noyau et du réseau**, et réfléchi aux **structures de données et échanges entre modules**. Ces points ont bien avancé (notamment concernant la structure associée au plateau de jeu), et nous allons continuer à travailler dessus cette semaine et à la prochaine séance.




---------------------------
### 2.2. Difficultés rencontrées


Pas de difficulté particulière pour cette séance. Le sujet de réflexion ardu a surtout concerné la structure du plateau de jeu et son implémentation, la difficulté provenant de la forme du plateau (composé de "tuiles") et des problèmes qui en découlent : représentation en structure de données, calcul des intersections, des sommets et arêtes...

En dehors de cela, le travail a bien avancé.






------------------------------------------------------
## 3. À venir : ce qui est prévu




### 3.1. D'ici à la prochaine réunion



- Réfléchir à la documentation (Doxygen ? autre ?), format des commentaires ?

- Consulter les recommendations / standards liés au C Sharp et à Unity (bonnes pratiques de prog, etc...)

- Réfléchir à certains aspects du GitLab (organisation spécifique des répertoires pour les projets en Unity ? gestion des conflits et des merge requests ? conventions pour les commits / les merges)

- Mettre en place l'outil pour assurer le suivi du travail de chacun (le "remplissage" des heures), et "remplir" nos heures pour les trois premières semaines de travail



---------------------------
### 3.2. À la prochaine réunion (séance #04, mardi 11 février)


- Rediscuter des points de réflexion mentionnées ci-dessus (cf section 3.1.), et de ceux restant qui n'ont pas été abordés lors de cette séance (cf section 3.3. du CR du 31/01/2020)

- Récapituler le travail réalisé jusqu'à présent + les avancées

- Continuer les spécifications, le détail des structures de données et les communications entre les modules : c'est la tâche importante actuelle.
