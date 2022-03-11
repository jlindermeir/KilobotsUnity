using System;
using System.Collections.Generic;
using UnityEngine;

public class KilobotAgent
{
    public enum State
    {
        Start,
        WaitToMove,
        MoveWhileOutside,
        MoveWhileInside,
        JoinedShape
    }
    public State CurrentState;
    public Vector2 PositionEstimate;
    public int Gradient;

    private Dictionary<State, Color> _stateColor = new Dictionary<State, Color>()
    {
        {State.Start, Color.gray},
        {State.WaitToMove, Color.black},
        {State.MoveWhileOutside, Color.cyan},
        {State.MoveWhileInside, Color.blue},
        {State.JoinedShape, Color.green}
    };
    
    public KilobotAgent(State initialState = State.Start)
    {
        CurrentState = initialState;
    }

    public Color GetStateColor()
    {
        return _stateColor[CurrentState];
    }

    public Tuple<Vector2, KilobotMessage> Act(List<Tuple<float, KilobotMessage>> messageList)
    {
        Vector2 motionDirection = Vector2.zero;

        switch (CurrentState)
        {
            case State.JoinedShape:
                break;
        }
        
        return new Tuple<Vector2, KilobotMessage>(motionDirection, GetMessage());
    }

    private KilobotMessage GetMessage()
    {
        return new KilobotMessage(Gradient, CurrentState);
    }
}
