using UnityEngine;

public struct KilobotMessage
{
    public KilobotMessage(int gradient, KilobotAgent.State state, Vector2 position)
    {
        Gradient = gradient;
        State = state;
        Position = position;
    }

    public static KilobotMessage Empty()
    {
        return new KilobotMessage(0, KilobotAgent.State.Start, Vector2.zero);
    }
    
    public int Gradient;
    public KilobotAgent.State State;
    public Vector2 Position;
}
