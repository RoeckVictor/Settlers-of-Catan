using System;
using Util.View;

namespace Network.Model
{
    class Easy : IA
    {
        public Easy(int idDB, Guid idGame, int idInGame, string name)
            : base(idDB, idGame, idInGame, ClientType.EasyAI, name) { }
    }
}
