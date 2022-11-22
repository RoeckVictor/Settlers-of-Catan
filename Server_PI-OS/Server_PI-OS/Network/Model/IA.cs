using System;
using Util.View;

namespace Network.Model
{
    class IA : Player
    {
        protected IA(int idDB, Guid idGame, int idInGame, ClientType type, string name)
            : base(idDB, idGame, idInGame, type, name,0,true) { } //
    }
}
