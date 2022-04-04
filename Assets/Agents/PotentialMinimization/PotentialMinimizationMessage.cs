namespace Agents.PotentialMinimization
{
    public struct PotentialMinimizationMessage
    {
        public PotentialMinimizationMessage(PotentialMinimizationAgent.State state, float potentialEnergy, float gas)
        {
            State = state;
            PotentialEnergy = potentialEnergy;
            Gas = gas;
        }
        
        public PotentialMinimizationAgent.State State;
        public float PotentialEnergy;
        public float Gas;
    }
}