# Petit Makefile pour gérer plus simplement la documentation.

# La documentation est séparée en deux répertoires :
#	- `client/`, qui prend en compte le code source du jeu côté client 
#	  (répertoires correspondants dans `pi/Catan/Assets/Resources/Scripts/`) ;
#	- `server/`, qui prend en compte le code source du jeu côté serveur
#	  (répertoires correspondants dans `pi/Server_PI-OS/Server_PI-OS/`).
# Les deux fichiers Doxyfile utilisés sont respectivement placés dans 
# les dossiers `client/` et `server/`.
# La liste des commandes est disponible via `make help`.

# NOTE : Il est recommandé d'utiliser ce Makefile, dans le répertoire 
#		 `doc-code-source/`, pour générer la documentation, puisque les
# 		 Doxyfile utilisent des chemins relatifs (pour gérer les INPUT 
# 	     notamment) relativement à l'emplacement depuis lequel la commande 
#		 `doxygen` est lancée.


CLIENT_REP = client/
SERVER_REP = server/


.PHONY: all

all: doc-all


##### générer la documentation

doc-all: doc-client doc-server

doc-client:
	doxygen $(CLIENT_REP)Doxyfile
	@echo "[1m   ==>  Documentation du code client générée dans $(CLIENT_REP)[0m"

doc-server:
	doxygen $(SERVER_REP)Doxyfile
	@echo "[1m   ==>  Documentation du code serveur générée dans $(SERVER_REP)[0m"


##### supprimer la documentation

clean-all: clean-client clean-server

clean-client:
	rm -rf $(CLIENT_REP)html $(CLIENT_REP)latex $(CLIENT_REP)xml
	@echo "[1m   ==>  Documentation du code client supprimée[0m"

clean-server:
	rm -rf $(SERVER_REP)html $(SERVER_REP)latex $(SERVER_REP)xml
	@echo "[1m   ==>  Documentation du code serveur supprimée[0m"



# afficher les cibles possibles

help:
	@echo "--> Commandes du Makefile pour gérer la documentation"
	@echo "  * Générer la documentation:    make [doc-all|doc-client|doc-server] "
	@echo "  * Supprimer la documentation:  make [clean-all|clean-client|clean-server] "
