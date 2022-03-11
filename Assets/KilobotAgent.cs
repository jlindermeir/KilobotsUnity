using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class KilobotAgent
{
    public enum State
    {
        Movement,
        FixedPosition
    }
    public State CurrentState;
    public Vector2 PositionEstimate;

    private Dictionary<State, Color> _stateColor = new Dictionary<State, Color>()
    {
        {State.Movement, Color.red},
        {State.FixedPosition, Color.green}
    };
    
    public KilobotAgent(State initialState = State.Movement)
    {
        CurrentState = initialState;
    }

    public Color GetStateColor()
    {
        return _stateColor[CurrentState];
    }

    public Tuple<Vector2, KilobotMessage> Act(List<Tuple<float, KilobotMessage>> messageList)
    {
        // TODO
        return new Tuple<Vector2, KilobotMessage>(Random.insideUnitCircle, new KilobotMessage(0, CurrentState));
    }
}
