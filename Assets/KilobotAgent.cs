using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public Vector2 PositionEstimate = Vector2.zero;
    public bool PositionSeed = false;
    public int Gradient;
    public bool GradientSeed = false;

    private const float GradientDistance = 3f;

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
        // Update the gradient value
        UpdateGradient(messageList);
        
        // Update the position estimate
        EstimatePosition(messageList);
        
        Vector2 motionDirection = Vector2.zero;

        switch (CurrentState)
        {
            case State.Start:
                motionDirection = Random.insideUnitCircle;
                break;
            case State.JoinedShape:
                break;
        }
        
        return new Tuple<Vector2, KilobotMessage>(motionDirection, GetMessage());
    }

    public KilobotMessage GetMessage()
    {
        return new KilobotMessage(Gradient, CurrentState, PositionEstimate);
    }

    private void UpdateGradient(List<Tuple<float, KilobotMessage>> messageList)
    {
        // If the kilobot seeds the gradient computation, set the gradient to 0
        if (GradientSeed)
        {
            Gradient = 0;
            return;
        }
        
        // Determine the minimum gradient of neighbours within GradientDistance
        Gradient = Int32.MaxValue;
        bool isAnyInRange = false;
        foreach ((float distance, KilobotMessage message) in messageList)
        {
            if (distance < GradientDistance && message.Gradient <= Gradient)
            {
                Gradient = message.Gradient;
                isAnyInRange = true;
            }
        }
        
        // If at least on other bot was in range, set the own gradient as the minimum + 1
        if (isAnyInRange)
        {
            Gradient++;
            return;
        }

        Gradient = 0;
    }

    private void EstimatePosition(List<Tuple<float, KilobotMessage>> messageList)
    {
        // If the kilobot is a position seed, the estimate is already precise
        if (PositionSeed)
        {
            return;
        }
        
        // Generate a list of stationary neighbors
        List<Tuple<float, Vector2>> statNeighbours = new List<Tuple<float, Vector2>>();
        foreach ((float distance, KilobotMessage message) in messageList)
        {
            if (message.State == State.JoinedShape)
            {
                statNeighbours.Add(new Tuple<float, Vector2>(distance, message.Position));
            }
        }
        
        // Improve the position estimate, only try if there are 3 or more neighbors
        Debug.Log(statNeighbours.Count);
        if (statNeighbours.Count < 3)
        {
            return;
        }

        foreach ((float distance, Vector2 neighborPosition) in statNeighbours)
        {
            Vector2 directionToNeighbor = (PositionEstimate - neighborPosition).normalized;
            Vector2 newPosition = neighborPosition + distance * directionToNeighbor;
            PositionEstimate -= (PositionEstimate - newPosition) / 4;
        }
    }
}
