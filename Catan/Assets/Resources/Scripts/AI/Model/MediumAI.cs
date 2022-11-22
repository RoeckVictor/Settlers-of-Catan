namespace AI.Model
{
    class MediumAI : AbstractIntermediateAI
    {
        /// <summary>
        /// Crée une instance de AbstractIntermediateAI avec une probabilité d'appel aux méthodes de DifficultAI de 0.8
        /// </summary>
        public MediumAI()
        {
            level = 0.8;
        }
    }
}
