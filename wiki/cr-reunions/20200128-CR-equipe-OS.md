# PI - Groupe OS | Compte-rendu de réunion - 28/01/2020





- **Type  :**  Séance dédiée #02

- **Date  :**  Mardi 28 janvier 2020

- **Heure :**  13h30 -> 17h30 (4h)

- **Lieu  :**  UFR, Salle T24

- **Membres présents :** Tout le monde


*Note : Dans ce compte-rendu et les suivants, certaines abréviations seront utilisées :*

  * CR pour "compte-rendu"
  * CDC pour "cahier des charges"
  * IG pour "interface graphique"







------------------------------------------------------
## [Note : Mini-réunion du 22/01]



Nous nous sommes tous vus le mercredi 22/01 de 13h à 14h15\~ afin de jouer ensemble une partie du jeu que nous avons choisi d'adapter, Les Colons de Catane, afin que nous puissions nous familiariser avec les règles et voir ce que donne une partie "réelle".

Cette réunion n'ayant pas donné lieu à d'autres discussions, progressions ou décisions relatives au projet, nous n'avons pas jugé utile d'en faire un CR à part.

Nous en faisons donc mention brièvement dans ce CR.






------------------------------------------------------
## 1. Déroulement de la réunion





### 1.1. Démarrage de la séance (13h30)


- Récapitulatif de l'état actuel de l'avancement et présentation par Joren du "plan" de la séance : ce qu'il faut faire, les points à discuter ; l'objectif étant d'avancer dans le CDC qui est à rendre la semaine du 03/02.

- Accord sur une date pour la prochaine réunion : jeudi 30/01, à l'UFR, en début d'après-midi (tout le monde est libre jusqu'à 15h30).




---------------------------
### 1.2. Discussions autour du cahier des charges et de son contenu (\~ 13h40)


- Prise en compte des informations données par M. Decor par rapport au contenu demandé, description des points importants et de ceux qui le sont moins


- Choix d'un plan pour le CDC, détail du contenu de chaque partie


- Idée importante retenue pour le CDC : récapituler les priorités sous la forme d'un tableau. Ce format sera en effet plus clair et lisible qu'une succession de paragraphes.

Chaque ligne correspondra à une fonctionnalité, et les fonctionnalités seront regroupées, au sein du tableau, en fonction de leur catégorie (réseau, IG...)

Pour chaque ligne :

  * nom de la fonctionnalité
  * priorité (3 : indispensable | 2 : envisageable | 1 : peut-être, idée émise mais à voir | ...)
  * catégorie
  * description




---------------------------
### 1.3. Discussions de plusieurs points importants du projet (\~ 14h15)


- **Fonctionnalités envisagées**

  * re-*listing*

  * IA : la priorité sera donnée à une IA "*useless*" dans un premier temps

  * local/multi : une partie sera possible :

    - en local/solo  - partie avec un joueur solo contre des *bots*
    - en local/multi - partie avec plusieurs joueurs sur un même PC
    - en ligne/multi - partie avec plusieurs joueurs, chacun sur son PC, connectés à un même serveur

  * traduction : cette phase sera à priori assez rapide et se fera vers la fin du projet

  * mode en ligne : un serveur "central" servira à host les parties en ligne, la BDD

  * système de compte : permettra aux joueurs en disposant d'un de personnaliser leur pseudo, leur avatar notamment


- Choix de faire un **schéma global de l'architecture** du jeu (maquette globale en UML)


- Première ébauche de **diagramme d'activité du jeu**


- Jeu en ligne : **les parties en ligne seront hébergées sur un serveur** (et pas chez l'utilisateur ayant créé la partie)


- Autres fonctionnalités éventuellement envisageables : extension à 6 joueurs ?
/!\ Ce sera à voir lors de la spécification des modules au départ, de manière à ce qu'une telle extension, si le choix est fait de l'implémenter, soit compatible avec le code qui sera déjà écrit à ce moment là.





---------------------------
### 1.4. Discussions et description des écrans du jeu (\~ 15h00)


Pour chaque écran du jeu : réflexions sur les éléments importants qui doivent être inclus, choix d'emplacements logiques et cohérents, schéma rapide de l'écran.
Sur la plupart des écrans, présence d'un bouton "retour", permettant de revenir au menu précédent. Sa position devra être cohérente d'un menu à l'autre.



##### Accueil

Écran principal au démarrage, permettant de lancer une partie locale, une partie en ligne, d'accéder aux crédits, options, règles, et de quitter

/!\ Éventuellement : lancement d'un tutoriel au premier démarrage du jeu, ou bien affichage des règles (à voir).



##### Règles

Écran permettant de :

  * consulter les règles de l'implémentation du jeu, en prenant en compte les particularités de l'adaptation en jeu vidéo par rapport au jeu de plateau original
  * connaître les différentes possibilités offertes par ce jeu (plateformes supportées, langues disponibles...)



##### Options

Menu permettant de paramétrer plusieurs aspects du logiciel : réglage du volume et des effets sonores, langue du jeu, mode plein écran...
Les changements s'appliquent directement à chaque modification.
Présence d'un bouton "par défaut" permettant de *reset* les options à des valeurs par défaut.

/!\ À voir comment gérer les changements de langue (si les modifications peuvent se faire directement ou non)

/!\ À voir comment gérer une telle section sur mobile/web.



##### Crédits

Écran permettant de consulter plusieurs informations

  * le(s) créateur(s) du jeu original et des extensions (si certaines sont ajoutées)
  * le contexte de l'adaptation en jeu-vidéo
  * l'équipe de développement, et qui a travaillé sur quelle partie
  * les crédits des éventuelles musiques/images/assets utilisés



##### Jeu local

Cet écran permet de créer une partie en local, sur le PC de l'utilisateur.
Il est possible de régler le nombre de joueurs souhaités, le nombre de bots (si l'utilisateur souhaite qu'il y en ai), et éventuellement d'autres paramètres de la partie.



##### Jeu en ligne : menu général


Écran récapitulant les différentes parties disponibles, regroupées au sein d'une sorte de *scroll box* au centre de l'écran, avec affichage pour chaque partie disponible du nom de la partie, du nombre de joueurs actuellement en attente, du statut du salon (public ou privé).
Un salon "public" est rejoignable par quiconque veut s'y connecter ; alors que la tentative de connexion à un salon privé nécessite la saisie d'un mot de passe permettant de rejoindre le salon.


L'utilisateur peut directement choisir le salon qu'il souhaite rejoindre en le sélectionnant dans la *scroll box*.
Différents boutons permettent en outre de :

  * créer un salon
  * rejoindre un salon, de manière aléatoire
  * rejoindre un salon spécifique


Cet écran permet aussi d'accéder au système de compte : l'utilisateur peut depuis cet endroit créer un compte ou se connecter au sien.
S'il est connecté, l'utilisateur peut consulter ses scores/classements, gérer son compte (pseudo, avatar...) et aussi se déconnecter lorsqu'il le souhaite.
Cette partie est située en haut à droite de l'écran, et les différentes possibilités sont accessibles depuis un bouton (avec une icône d'utilisateur par exemple).

Qu'il soit connecté ou non, tout joueur peut rejoindre une partie en ligne. Un joueur non connecté se verra toutefois attribuer un pseudo et un avatar automatiquement.



##### Jeu en ligne : créer un salon


Depuis cet écran, pour créer une partie en ligne, un utilisateur doit choisir le nom du salon, sélectionner le nombre de joueurs qui joueront ainsi que le nombre de bots (s'il souhaite qu'il y en ai), choisir le statut du salon (public ou privé) et choisir le mot de passe du salon (si le choix "privé" a été fait).

Le nom de la partie doit être composé exclusivement de caratères alphanumériques, ainsi que de tirets ou underscore (à voir) ; il en est de même pour le mot de passe du salon. Le nom de la partie comme le mot de passe ne peuvent pas être "vides".


Pour qu'une partie en ligne puisse avoir lieu, il faut obligatoirement la présence de deux joueurs "humains" (une partie en ligne seul et avec 3 bots, par exemple, ne sera pas autorisée).

En cas de partie à deux joueurs "humains" + deux bots, par exemple, si l'un des deux joueurs est déconnecté ou quitte la partie, l'autre joueur aura la possibilité de continuer la partie avec les bots (les deux bots de départ + le bot qui aura pris le relais du joueur déconnecté), ou de revendiquer la victoire.



##### Jeu en ligne : rejoindre un salon aléatoire

En cliquant sur ce bouton depuis l'écran général (jeu en ligne), l'utilisateur rejoint un salon public choisi aléatoirement parmi les salons publics disponibles où il reste de la place.



##### Jeu en ligne : rejoindre un salon spécifique

En cliquant sur ce bouton depuis l'écran général (jeu en ligne), une sorte de fenêtre apparaît sur l'écran. L'utilisateur peut alors saisir le nom du salon qu'il souhaite rejoindre. Si le salon dispose d'un mot de passe, ce dernier lui est demandé. Si le nom et le mot de passe (le cas échéant) sont corrects, l'utilisateur rejoint le salon.



##### Jeu en ligne : attente dans un salon


L'utilisateur, qu'il crée un salon, rejoigne un salon aléatoire, ou rejoigne un salon spécifique, se retouve sur cet écran.

Cet écran affiche simplement le statut actuel du salon : quels sont les joueurs en attente et combien de joueurs sont encore attendus avant de lancer le jeu.

Un bouton "lancer la partie" permet au créateur du salon de lancer la partie quand suffisament de joueurs sont présents : il est "grisé"" jusqu'à ce que le nombre requis soit atteint, et devient cliquable lorsque suffisament de joueurs sont présents.
Lorsque la partie est lancée, tous les joueurs du salon se retrouvent en jeu.

Un bouton laisse également à tout utilisateur la possibilité de se déconnecter du salon dans lequel il attend.



##### Partie *in-game* (en ligne et en local)

/!\ Cette interface sera décrite lors de la prochaine réunion.




---------------------------
### 1.5. Dernières réflexions et fin de séance (\~ 17h00)


- **Réflexions concernant le *medium* de jeu**

  * La souris semble être l'outil le plus adapté pour un tel jeu : sélection de tuiles sur le plateau, placement de constructions... Une partie à la souris assurera à priori une expérience de jeu plus amusante et intuitive

  * Même si cela reste moins prioritaire, il est envisageable de laisser la possibilité à l'utilisateur de jouer au clavier et au *pad* (ex : manette de X360). Dans ces configurations, à minima, les touches directionnelles (sur un clavier) ou le stick analogique (sur une manette) permettraient de déplacer le curseur, et donc de "simuler" un déplacement de souris. Des assignations "touche (sur clavier) / bouton (sur manette) <=> action" viendraient compléter ce fonctionnement. 
  Ce ne serait ainsi pas la *config* la plus ergonomique, mais de cette manière, les utilisateurs souhaitant jouer au clavier ou à la manette exclusivement ne seraient pas laissés sur le carreau.


- Réflexions concernant les **interactions entre joueurs au cours d'une partie en ligne**

  * Un chat par écrit, en réseau, nous semble indispensable

  * Un chat vocal est envisageable : ce serait une fonctionnalité potentiellement très utile et intéressante, améliorant l'aspect fun du jeu et facilitant la vie aux joueurs n'ayant pas envie de tout dire par écrit.
  /!\ À voir, en fonction de la difficulté et du temps nécessaire pour ajouter une telle fonctionnalité.


- Prise de notes / récapitulatif par Joren des souhaits d'affectation de chaque membre de l'équipe pour les différents modules de développement, en vue de la répartition qui sera faite prochainement.


- Récapitulatif de ce qui est à faire pour notre prochaine réunion, jeudi 30/01.






------------------------------------------------------
## 2. Bilan





### 2.1. Ce qui a été fait, où nous en sommes


Cette deuxième séance dédiée de PI s'est bien passée et a été globalement productive pendant les 4 heures.

Plusieurs points importants ont été discutés et nous avons avancé nos réflexions concernant le jeu et le cahier des charges :

  - plan et contenu des parties du cahier des charges détaillé ;
  - discussions concernant le jeu : fonctionnalités, diagramme d'activité ;
  - description et mise au point des écrans de jeu (sauf pour le *in-game*) ;
  - réflexions sur le *medium* pour joueur, les interactions entre joueurs en cours de partie, début des réflexions pour la répartition des membres pour le développement.


Le système consistant à débuter la séance en exposant une "feuille de route", récapitulant l'état actuel du projet, ce qui a été fait dernièrement ainsi que ce qui est prévu pour la séance (et éventuellement ultérieurement) semble être positif et permet de commencer tout de suite la séance, sans s'égarer et sans perdre du temps à se demander quelles sont les priorités.

Cette phase permet aussi de prendre en compte les questions, remarques et avis de tout le monde concernant l'avancement, la gestion du projet, les prochaines échéances...




---------------------------
### 2.2. Difficultés rencontrées


Nous n'avons pas rencontré de difficulté particulière, si ce n'est peut-être un manque de temps pour parler de tous les sujets dont nous avions prévu de parler. Ceux-ci ont ainsi été reportés à la prochaine réunion (*cf*. section 3.2.)

La préparation des prochaines "feuilles de route" devra :
  
  * nécessiter une estimation plus juste du temps nécessaire à chaque discussion au cours de la séance ;
  * veiller à ce que le contenu du plan de séance ne soit pas "trop chargé".







------------------------------------------------------
## 3. À venir : ce qui est prévu




### 3.1. D'ici la prochaine réunion



Afin que la prochaine réunion soit efficace, nous prévoyons de travailler sur différents points d'ici là :

- réfléchir à l'interface encore non discutée : la partie *in-game* :

  * affichage, emplacement des informations (en prenant en compte les situations pouvant avoir lieu en cours de partie)
  * type de partie (en ligne ou en local) et incidence que ça aura sur le jeu

- réfléchir à la durée que pourraient prendre les tâches du projet et la répartition des membres ;

- (réfléchir éventuellement aux structures de données, et comment communiqueraient les différents modules entre eux)




---------------------------
### 3.2. À la prochaine réunion (jeudi 30 janvier, en présentiel)


- La prochaine réunion est prévue pour le **jeudi 30/01, de 13h30\~ jusqu'à 15h30** : ce créneau horaire est libre pour tout le monde. De nombreuses salles dans l'UFR seront libres à ce moment.


- L'objectif de cette réunion sera d'avoir un **CDC globalement très avancé**, avec un contenu de chaque section clair pour tout le monde.
Nous aborderons donc de nombreux points deslesquels il nous faut discuter afin que tout soit clair entre nous - cela est indispensable pour mener le projet à bien, mais également pour que ce soit inscrit dans le CDC. La plupart de ces points ont déjà été plus ou moins discutés entre nous, mais ce sera en tout cas l'occasion d'être au clair :

  * éléments importants pour l'adaptation du jeu et des règles sur ordinateur ;

  * menus et interface (wireframing) incluant les fonctionnalités envisagées ;

  * ordre et durée des tâches pour le développement (Diagramme de Gantt) ;

  * répartition prévisionnelle des membres ;

  * choix des technologies et langages : passer en revue pour les fonctionnalités, noter les aspects importants, les avantages/inconvénients ;

  * rôle du chef de projet ;

  * plateformes utilisées et rôle de chacune (+ outil pour "remplir les heures").
