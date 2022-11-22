namespace AI.Model
{
    class EasyAI : AbstractIntermediateAI
    {
        /// <summary>
        /// Crée une instance de AbstractIntermediateAI avec une probabilité d'appel aux méthodes de DifficultAI de 0.6
        /// </summary>
        public EasyAI()
        {
            level = 0.0; // 0.6;
        }
    }
}