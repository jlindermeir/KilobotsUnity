namespace Agents.PotentialMinimization
{
    public struct PotentialMinimizationMessage
    {
        public PotentialMinimizationMessage(PotentialMinimizationAgent.State state, float potentialEnergy)
        {
            State = state;
            PotentialEnergy = potentialEnergy;
        }
        
        public PotentialMinimizationAgent.State State;
        public float PotentialEnergy;
    }
}