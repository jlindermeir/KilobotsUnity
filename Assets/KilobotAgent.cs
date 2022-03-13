using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


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
    public bool PositionEstimated = false;
    public bool PositionSeed = false;
    public int Gradient;
    public bool GradientSeed = false;
    public float RandomID;

    private const float GradientDistance = 3f;
    private const float EdgeDistance = 1.75f;
    private float prevNearestNeighborDistance = float.MaxValue;
    private int StartupTime = 15;

    private Dictionary<State, Color> _stateColor = new Dictionary<State, Color>()
    {
        {State.Start, Color.gray},
        {State.WaitToMove, Color.magenta},
        {State.MoveWhileOutside, Color.cyan},
        {State.MoveWhileInside, Color.blue},
        {State.JoinedShape, Color.green}
    };
    
    public KilobotAgent(State initialState = State.Start)
    {
        CurrentState = initialState;
        Random rng = new Random();
        RandomID = (float)rng.NextDouble();
    }

    public Color GetStateColor()
    {
        return _stateColor[CurrentState];
    }

    public Tuple<Vector2, float, KilobotMessage> Act(List<Tuple<float, KilobotMessage>> messageList)
    {
        // Update the gradient value
        UpdateGradient(messageList);
        
        // Update the position estimate
        EstimatePosition(messageList);
        
        // Determine the movement within the state machine
        Vector2 motionDirection = Vector2.zero;
        float torque = 0f;
        switch (CurrentState)
        {
            case State.Start:
                (motionDirection, torque) = ProcessStart();
                break;
            case State.WaitToMove:
                (motionDirection, torque) = ProcessWaitToMove(messageList);
                break;
            case State.MoveWhileOutside:
                (motionDirection, torque) = ProcessMoveOutside(messageList);
                break;
            case State.MoveWhileInside:
                (motionDirection, torque) = ProcessMoveInside(messageList);
                break;
            case State.JoinedShape:
                break;
        }
        
        return new Tuple<Vector2, float, KilobotMessage>(motionDirection, torque, GetMessage());
    }

    public KilobotMessage GetMessage()
    {
        return new KilobotMessage(Gradient, CurrentState, PositionEstimate, RandomID);
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
            PositionEstimated = true;
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
        if (statNeighbours.Count < 3)
        {
            PositionEstimated = false;
            return;
        }

        foreach ((float distance, Vector2 neighborPosition) in statNeighbours)
        {
            Vector2 directionToNeighbor = (PositionEstimate - neighborPosition).normalized;
            Vector2 newPosition = neighborPosition + distance * directionToNeighbor;
            PositionEstimate -= (PositionEstimate - newPosition) / 4;
            PositionEstimated = true;
        }
    }

    private Tuple<Vector2, float> ProcessStart()
    {
        if (StartupTime < 0)
        {
            CurrentState = State.WaitToMove;
        }
        else
        {
            StartupTime--;
        }

        return new Tuple<Vector2, float>(Vector2.zero, 0);
    }

    private Tuple<Vector2, float> ProcessWaitToMove(List<Tuple<float, KilobotMessage>> messageList)
    {
        Tuple<Vector2, float> action = new Tuple<Vector2, float>(Vector2.zero, 0);
        
        // Check if other visible bots are moving. If so, stay in this state
        foreach ((_, KilobotMessage message) in messageList)
        {
            if (message.State == State.MoveWhileInside | message.State == State.MoveWhileOutside)
            {
                return action;
            }
        }
        
        
        // Determine the maximum gradient of neighbors
        int maximumGradient = 0;
        foreach ((_, KilobotMessage message) in messageList)
        {
            if (message.Gradient > maximumGradient)
            {
                maximumGradient = message.Gradient;
            }
        }
        
        // If our gradient is higher than the neighbour's, switch state
        if (maximumGradient < Gradient)
        {
            CurrentState = State.MoveWhileOutside;
            return action;
        }
        
        // If our gradient is equal the highest one, check who has the higher random ID
        if (maximumGradient == Gradient)
        {
            foreach ((_, KilobotMessage message) in messageList)
            {
                if (message.Gradient == maximumGradient && message.ID > RandomID)
                {
                    return action;
                }
            }
            CurrentState = State.MoveWhileOutside;
        }

        return action;
    }

    private Tuple<Vector2, float> ProcessMoveOutside(List<Tuple<float, KilobotMessage>> messageList)
    {
        if (IsInShape())
        {
            CurrentState = State.MoveWhileInside;
        }
        return FollowEdge(messageList);
    }
    
    private Tuple<Vector2, float> ProcessMoveInside(List<Tuple<float, KilobotMessage>> messageList)
    {
        if (PositionEstimated && !IsInShape())
        {
            CurrentState = State.JoinedShape;
            return new Tuple<Vector2, float>(Vector2.zero, 0);
        }

        float closestNeighborDistance = float.MaxValue;
        int closestNeighborGradient = Int32.MaxValue;
        foreach ((float distance, KilobotMessage message) in messageList)
        {
            if (distance < closestNeighborDistance)
            {
                closestNeighborDistance = distance;
                closestNeighborGradient = message.Gradient;
            }
        }

        if (closestNeighborGradient >= Gradient)
        {
            CurrentState = State.JoinedShape;
            return new Tuple<Vector2, float>(Vector2.zero, 0);
        }

        return FollowEdge(messageList);
    }

    private Tuple<Vector2, float> FollowEdge(List<Tuple<float, KilobotMessage>> messageList)
    {
        float currentNearestNeighborDistance = float.MaxValue;
        foreach ((float distance, _) in messageList)
        {
            if (distance < currentNearestNeighborDistance)
            {
                currentNearestNeighborDistance = distance;
            }
        }

        float torque = 0f; 
        if (currentNearestNeighborDistance < EdgeDistance)
        {
            if (prevNearestNeighborDistance > currentNearestNeighborDistance)
            {
                torque = 1f;
            }
        }
        else
        {
            if (prevNearestNeighborDistance < currentNearestNeighborDistance)
            {
                torque = -1f;
            }
        }
        
        prevNearestNeighborDistance = currentNearestNeighborDistance;
        return new Tuple<Vector2, float>(Vector2.up, torque);
    }

    private bool IsInShape()
    {
        if (!PositionEstimated)
        {
            return false;
        }

        Vector2 center = new Vector2(0, 8.5f);
        const float radius = 7.5f;

        if ((PositionEstimate - center).magnitude < radius)
        {
            return true;
        }

        return false;
    }
}
