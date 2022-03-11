using UnityEngine;

public struct KilobotMessage
{
    public KilobotMessage(int gradient, KilobotAgent.State state, Vector2 position, int id)
    {
        Gradient = gradient;
        State = state;
        Position = position;
        ID = id;
    }

    public int Gradient;
    public KilobotAgent.State State;
    public Vector2 Position;
    public int ID;
}
