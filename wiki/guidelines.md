# Guidelines / Conventions pour le projet




------------------------------------------------------

## 1. Code




### 1.1. Langue


- Langue du **code** : **anglais**


- Langue des **commentaires** : **français**


- Langue de la **docs** : **français**


---------------------------
### 1.2. Formatage / Casse du texte


- **Variables** en *camelCase* (`variableForSomething`)


- **Fonctions** en *PascalCase* (`FunctionForSomethingElse`)


- **Constantes** en majuscules (`CONSTANT_FOR_ANOTHER_THING`)




---------------------------
### 1.3. Mise en forme du code


- **Indentation**

    * pas de tabulations mais des **espaces** (*convert tabs to spaces*)

    * longueur : **4 espaces**


- **Accolades** en **début de ligne**







------------------------------------------------------

## 2. Documentation


- **Commentaires** : format `/// ...`


- Documentation via **Doxygen**
[ utilisation de balises XML possible, puisque Doxygen supporte les commentaires en XML ]







------------------------------------------------------

## 3. GitLab


- La **gestion des éventuels conflits** au moment des merges se fera en fonction de la situation et du problème, entre les membres / équipes concernées et avec le chef de projet


- **Conventions pour les commits** : **indiquer la partie concernée par le commit** (ex: noyau, IG...) en début de nom **et** faire suivre par **la modification effectuée** (ex: ajout d'une fonctionnalité, résolution de bug...)
Exemple : *[Noyau] Ajout logique distribution des cartes*


- **Principes de précautions** : récupérer les modifications avant d'en push de nouvelles ; ne pas travailler sur un même document au même moment ; lors d'un merge, en cas de conflit : annonce aux membres des équipes concernées...
