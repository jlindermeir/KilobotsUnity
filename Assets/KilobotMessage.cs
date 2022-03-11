public struct KilobotMessage
{
    public KilobotMessage(int gradient, KilobotAgent.State state)
    {
        Gradient = gradient;
        State = state;
    }

    public static KilobotMessage Empty()
    {
        return new KilobotMessage(0, KilobotAgent.State.Start);
    }
    
    public int Gradient;
    public KilobotAgent.State State;
}
