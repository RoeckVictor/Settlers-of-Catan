using System;
using Util.View;

namespace Network.Model
{
    class Medium : IA
    {
        public Medium(int idDB, Guid idGame, int idInGame, string name)
            : base(idDB, idGame, idInGame, ClientType.MediumAI, name) { }
    }
}
