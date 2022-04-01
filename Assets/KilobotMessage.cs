using Agents;
using UnityEngine;

public struct KilobotMessage
{
    public KilobotMessage(int gradient, ShapeAssemblyAgent.State state, Vector2 position, float id)
    {
        Gradient = gradient;
        State = state;
        Position = position;
        ID = id;
    }

    public int Gradient;
    public ShapeAssemblyAgent.State State;
    public Vector2 Position;
    public float ID;
}
