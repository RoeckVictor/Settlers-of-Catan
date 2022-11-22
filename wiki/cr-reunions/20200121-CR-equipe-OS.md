# PI - Groupe OS | Compte-rendu de réunion - 21/01/2020





- **Type  :**  Séance dédiée #01

- **Date  :**  Mardi 21 janvier 2020

- **Heure :**  13h30 -> 17h30 (4h)

- **Lieu  :**  UFR, Salle T24

- **Membres présents :** Tous les huit membres de l'équipe
  *(Victoria ne fera pas partie de notre équipe car elle a abandonné la L3 Informatique. Nous avons eu une confirmation de sa part à ce sujet.)*







------------------------------------------------------
## 1. Déroulement de la réunion





### 1.1. Démarrage de la séance (13h30)


**Présentation de chacun des membres** de l'équipe : qui est qui, nos prénoms,
compétences, points forts/faibles...


**Choix du chef de projet** : Joren se propose et est accepté par les membres du groupe comme chef de projet




---------------------------
### 1.2. Premières discussions concernant les sujets importants (\~ 13h50)


**Choix du jeu de plateau** que nous souhaitons adapter

- Discussion autour des jeux éventuels

- Réflexion sur les points importants à prendre en compte dans le choix : aspect multijoueur, renouvellement d'un partie à l'autre, aspect ludique, jeu complet et pas trop simpliste...

- Décision, après réflexion, d'adapter le jeu **Les Colons de Catane**, jeu comportant des aspects de gestion, réflexion, décision et comportant une part de hasard. 


**Fonctionnalités envisagées**, en vrac :

- multijoueur en ligne ;
- possibilité de jouer à plusieurs en local ;
- IA ;
- traduction (Anglais, possibilité en Allemand, Arabe) ;
- système de compte personnel : profil, score, classements...


**Cahier des charges** : discussions et obtention d'informations sur le contenu du document à préparer. Ce dernier devra contenir, en vrac :

- concernant le jeu :
  * une présentation du jeu choisi (de manière relativement simple) ;
  * l'architecture envisagée (noyau, réseau, client, serveur...) ;
  * les technologies choisies ainsi que la justification de ces choix.

- concernant notre organisation et la gestion du projet :
  * l'organisation et la gestion de l'équipe ;
  * le rôle et les attributions du chef de projet ;
  * les outils/plateformes utilisées ;
  * le diagramme de Gantt prévisionnel.


**Technologies, langages, et outils/plateformes** envisagés, dans un premier temps, ainsi que les plateformes sur lesquelles chaque membre développe

- En termes de technos/langages, choix de **Unity/C#** pour le développement du jeu en lui-même.
Ce choix n'est pas encore entériné et sera à nouveau abordé lors de la prochaine séance. Toutefois, la possibilité avec Unity de créer des jeux sur différentes plateformes, le fait qu'une licence gratuite soit disponible, la relative simplicité de plusieurs éléments par rapport à d'autres outils de création de jeu et le fait que certains membres de l'équipe ont déjà travaillé avec ce moteur sont des points positifs non négligeables.

- Dans cette optique, choix de la **version d'Unity** que tous les membres utiliseront (le choix d'une seule et même version évitera d'emblée d'éventuels soucis liés à des versions différentes) :
**Unity version 2019.2.18f1**

- Bref échange sur les plateformes sur lesquelles chaque membre développe : Windows pour certains, Linux pour d'autres.

- Concernant les **outils/plateformes collaboratives**, décision de mettre en place :
  * un **serveur Discord**, pour faciliter la communication entre nous ;
  * un **Trello**, outil en ligne permettant de gérer et d'organiser un projet ;
  * un **Git Unistra**, pour le développement du jeu à proprement parler.

Ces trois plateformes nous semblent, pour le moment, suffisantes sans être redondantes : le Discord nous permettra d'échanger régulièrement et simplement, le Trello facilitera l'organisation et la vision globale de nos avancées dans le projet et le Git nous permettra d'héberger le code de notre application.




---------------------------
### 1.3. Début de mise en place des outils/plateformes (\~ 15h10)


**Création des outils/plateformes** qui seront utilisés entre les membres du groupe pour communiquer et développer ensemble le projet :

* serveur Discord (créé par Aymeric) ;
* Trello (créé par Khalida) ;
* groupe et *repository* Git Unistra (créés par Joren).

Ajout de notre tuteur, M. DECOR, sur chacune de ces plateformes, et de MM. BENOIT et CATELOIN sur le Git Unistra.


Partie de la séance où **chacun est davantage "de son côté", travaillant sur des aspects différents** :

- début de configuration des outils/plateformes (salons Discord, prochaines échéances sur Trello, .gitignore et gestion du Git au sein du groupe) ;

- début des réflexions sur le cahier des charges (création d'un GoogleDoc afin de le rédiger) ;

- "familiarisation" avec le jeu choisi : vidéo d'une partie, lecture des règles ;

- installation de la version choisie d'Unity...




---------------------------
### 1.4. Discussions additionnelles (\~ 16h50)


Mise en commun des options S6 choisies par chaque membre du groupe, afin d'avoir une idée de nos EDT pour envisager plus facilement des réunions hors "séance banalisée" au cours du semestre


Priorités des prochains jours et de la prochaine séance *(cf. section 3. de ce compte-rendu)*


Mise au point "en gros" sur les tâches importantes qu'il faudra effectuer pour mener le projet à bien, ainsi que leur importance et l'ordre de réalisation envisagé pour le développement :

- spécification du noyau et du réseau, développement du noyau, menus et interface graphique, réseau, comptes et BDD, IA ;
- puis ensuite seulement : web & mobile, finitions graphiques, traductions, autre fonctionnalités ;
- sans oublier : tests au fur et à mesure du développement, réalisation de compte-rendus et d'une documentation technique, d'un dossier de communication et d'un rapport général.


Estimation "grossière" et durée des tâches qui composeront ce projet, prévision d'une marge suffisante pour le rendu


Idée rapide de répartition des tâches entre nous, dans un premier temps, au début du projet : qui aimerait davantage faire quoi (pour avoir une idée)


Brève discussion autour du rôle de chef de projet


**Les derniers points ci-dessus ont été abordés assez rapidement et seront détaillés et approfondis lors de la prochaine séance.**






------------------------------------------------------
## 2. Bilan





### 2.1. Ce qui a été fait, où nous en sommes


Cette première séance a été globalement productive et bon nombre de discussions sur divers sujets ont eu lieu au sein du groupe.

Une fois les présentations faites, nous avons avancé sur plusieurs points :

- choix d'un jeu, "Les Colons de Catane" ;
- discussions autour des fonctionnalités envisagées pour le moment ;
- premiers éléments de réflexion au sujet du cahier des charges ;
- discussions sur les technologies que nous utiliseront (Unity notamment) ;
- choix et création des outils pour la gestion du projet (Discord, Trello , Git Unistra) ;
- discussions diverses sur des points importants du projet : nous en reparlerons à la prochaine séance.




---------------------------
### 2.2. Difficultés rencontrées


Pour cette première séance, nous n'avons pas rencontré de réelle difficulté, que ce soit au niveau technique ou au niveau humain.

Nous avons rapidement beaucoup échangé sur de nombreux points, nous avons plutôt bien avancé et il n'y a pas eu de mésentente au cours de la séance.


Le point ardu a surtout consisté à "lister" et réfléchir à tout ce dont nous devions parler concernant les débuts du projet, les éléments importants étant nombreux.

Malgré la quantité d'informations échangées, une prise de notes au fur et à mesure du déroulement de la séance (par Joren) a permis de rédiger un compte-rendu relativement détaillé. Les points importants dont il nous faudra discuter lors des réunions à venir figurent également dans un paragraphe dédié du compte-rendu *(cf. section 3.)*.







------------------------------------------------------
## 3. À venir : ce qui est prévu




### 3.1. Dans les prochains jours, pour chacun d'entre nous


- Bien comprendre le déroulement du jeu ainsi que les règles "en profondeur"

- Essayer de faire une partie du jeu "réel" "Les Colons de Catane" entre nous, afin de nous familiariser avec les règles et de voir ce que donne une partie du jeu

- Noter les éventuelles réflexions et idées qui nous viennent concernant le jeu, les règles : les points importants à ne pas négliger, ceux qui pourraient poser problème lors de l'adaptation en jeu vidéo...

- De la même manière, si un point qui nous semble important nous vient en tête, concernant notamment l'organisation, le planning, le cahier des charges ou autre : le noter afin d'en discuter à la prochaine séance ou en faire part directement sur le Discord.



---------------------------
### 3.2. À la prochaine séance (séance #02, mardi 28 janvier)


- Discuter tous ensemble des réflexions, idées ou points à soulever que chacun a eu au cours des jours précédents

- Revenir sur certains points abordés lors de la séance #01 :
  * ordre et durée des tâches pour le développement ;
  * répartition des membres de l'équipe pour le développement ;
  * fonctionnalités du jeu envisagées ;
  * choix des technologies et langages ;
  * rôle et attributions du chef de projet.

- Dans l'idéal, faire une autre partie du jeu afin de voir à nouveau ce qui serait judicieux, ou problématique, pour notre adaptation en jeu vidéo

- Débuter un wireframing des menus et de l'interface du jeu

- Faire un planning prévisionnel plus précis

- Principalement : **progresser dans le cahier des charges** : faire un plan logique et adapté, détailler le contenu, approfondir et détailler chacun des points qui y seront abordés
*(les points précédents de ce paragraphe seront utiles pour cela)*
