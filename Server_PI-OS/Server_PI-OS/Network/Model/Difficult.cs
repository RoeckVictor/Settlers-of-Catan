using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Model
{
    class Difficult: IA
    {
        public Difficult(int idDB, Guid idGame, int idInGame, string name)
            : base(idDB, idGame, idInGame, ClientType.DifficultAI, name) { }
    }
}
