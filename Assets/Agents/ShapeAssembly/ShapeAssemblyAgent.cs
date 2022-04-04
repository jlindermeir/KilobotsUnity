using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


namespace Agents.ShapeAssembly
{
    public class ShapeAssemblyAgent : IAgentInterface<ShapeAssemblyMessage>
    {
        public enum State
        {
            Start,
            WaitToMove,
            MoveWhileOutside,
            MoveWhileInside,
            JoinedShape
        }
        
        private static readonly Dictionary<State, Color> StateColor = new Dictionary<State, Color>()
        {
            {State.Start, Color.gray},
            {State.WaitToMove, Color.magenta},
            {State.MoveWhileOutside, Color.cyan},
            {State.MoveWhileInside, Color.blue},
            {State.JoinedShape, Color.green}
        };
        
        private readonly bool _positionSeed = false;
        private readonly bool _gradientSeed = false;
        private readonly float _randomID;
        private readonly Collider2D _targetShape;
        
        public bool PositionEstimated { get; private set; }
        public Vector2 PositionEstimate { get; set; }
        private State _currentState;
        private int _gradient;

        private const float GradientDistance = 3f;
        private const float EdgeDistance = 1.75f;
        private float _prevNearestNeighborDistance = float.MaxValue;
        private int _startupTime = 15;
        
        public ShapeAssemblyAgent(Collider2D targetShape, Vector2 initialPositionEstimate, bool isPositionSeed = false, bool isGradientSeed = false)
        {
            // Store whether the agent is a position or gradient seed
            _gradientSeed = isGradientSeed;
            _positionSeed = isPositionSeed;
            
            // Assign the target shape
            _targetShape = targetShape;
            
            // Set the initial state
            _currentState = isPositionSeed ? State.JoinedShape : State.Start;
            
            // Set the initial position estimate
            PositionEstimate = initialPositionEstimate;
            
            // Set the random ID
            Random rng = new Random();
            _randomID = (float)rng.NextDouble();
        }

        public Color GetStateColor()
        {
            return StateColor[_currentState];
        }

        public string GetDisplayText()
        {
            return _gradient.ToString();
        }

        public Tuple<Vector2, float, ShapeAssemblyMessage> Act(List<Tuple<float, ShapeAssemblyMessage>> messageList)
        {
            // Update the gradient value
            UpdateGradient(messageList);
        
            // Update the position estimate
            EstimatePosition(messageList);
        
            // Determine the movement within the state machine
            Vector2 motionDirection = Vector2.zero;
            float torque = 0f;
            switch (_currentState)
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
        
            return new Tuple<Vector2, float, ShapeAssemblyMessage>(motionDirection, torque, GetMessage());
        }

        public ShapeAssemblyMessage GetMessage()
        {
            return new ShapeAssemblyMessage(_gradient, _currentState, PositionEstimate, _randomID);
        }

        private void UpdateGradient(List<Tuple<float, ShapeAssemblyMessage>> messageList)
        {
            // If the kilobot seeds the gradient computation, set the gradient to 0
            if (_gradientSeed)
            {
                _gradient = 0;
                return;
            }
        
            // Determine the minimum gradient of neighbours within GradientDistance
            _gradient = Int32.MaxValue;
            bool isAnyInRange = false;
            foreach ((float distance, ShapeAssemblyMessage message) in messageList)
            {
                if (distance < GradientDistance && message.Gradient <= _gradient)
                {
                    _gradient = message.Gradient;
                    isAnyInRange = true;
                }
            }
        
            // If at least on other bot was in range, set the own gradient as the minimum + 1
            if (isAnyInRange)
            {
                _gradient++;
                return;
            }

            _gradient = 0;
        }

        private void EstimatePosition(List<Tuple<float, ShapeAssemblyMessage>> messageList)
        {
            // If the kilobot is a position seed, the estimate is already precise
            if (_positionSeed)
            {
                PositionEstimated = true;
                return;
            }
        
            // Generate a list of stationary neighbors
            List<Tuple<float, Vector2>> statNeighbours = new List<Tuple<float, Vector2>>();
            foreach ((float distance, ShapeAssemblyMessage message) in messageList)
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
            if (_startupTime < 0)
            {
                _currentState = State.WaitToMove;
            }
            else
            {
                _startupTime--;
            }

            return new Tuple<Vector2, float>(Vector2.zero, 0);
        }

        private Tuple<Vector2, float> ProcessWaitToMove(List<Tuple<float, ShapeAssemblyMessage>> messageList)
        {
            Tuple<Vector2, float> action = new Tuple<Vector2, float>(Vector2.zero, 0);
        
            // Check if other visible bots are moving. If so, stay in this state
            foreach ((_, ShapeAssemblyMessage message) in messageList)
            {
                if (message.State == State.MoveWhileInside | message.State == State.MoveWhileOutside)
                {
                    return action;
                }
            }
        
        
            // Determine the maximum gradient of neighbors that are outside of the shape
            int maximumGradient = 0;
            foreach ((_, ShapeAssemblyMessage message) in messageList)
            {
                if (message.State != State.JoinedShape && message.Gradient > maximumGradient)
                {
                    maximumGradient = message.Gradient;
                }
            }
        
            // If our gradient is higher than the neighbour's, switch state
            if (maximumGradient < _gradient)
            {
                _currentState = State.MoveWhileOutside;
                return action;
            }
        
            // If our gradient is equal the highest one, check who has the higher random ID
            if (maximumGradient == _gradient)
            {
                foreach ((_, ShapeAssemblyMessage message) in messageList)
                {
                    if (message.Gradient == maximumGradient && message.ID > _randomID)
                    {
                        return action;
                    }
                }
                _currentState = State.MoveWhileOutside;
            }

            return action;
        }

        private Tuple<Vector2, float> ProcessMoveOutside(List<Tuple<float, ShapeAssemblyMessage>> messageList)
        {
            if (IsInShape())
            {
                _currentState = State.MoveWhileInside;
            }
            return FollowEdge(messageList);
        }
    
        private Tuple<Vector2, float> ProcessMoveInside(List<Tuple<float, ShapeAssemblyMessage>> messageList)
        {
            if (PositionEstimated && !IsInShape())
            {
                _currentState = State.JoinedShape;
                return new Tuple<Vector2, float>(Vector2.zero, 0);
            }

            float closestNeighborDistance = float.MaxValue;
            int closestNeighborGradient = Int32.MaxValue;
            foreach ((float distance, ShapeAssemblyMessage message) in messageList)
            {
                if (distance < closestNeighborDistance)
                {
                    closestNeighborDistance = distance;
                    closestNeighborGradient = message.Gradient;
                }
            }

            if (closestNeighborGradient >= _gradient)
            {
                _currentState = State.JoinedShape;
                return new Tuple<Vector2, float>(Vector2.zero, 0);
            }

            return FollowEdge(messageList);
        }

        private Tuple<Vector2, float> FollowEdge(List<Tuple<float, ShapeAssemblyMessage>> messageList)
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
                if (_prevNearestNeighborDistance > currentNearestNeighborDistance)
                {
                    torque = 1f;
                }
            }
            else
            {
                if (_prevNearestNeighborDistance < currentNearestNeighborDistance)
                {
                    torque = -1f;
                }
            }
        
            _prevNearestNeighborDistance = currentNearestNeighborDistance;
            return new Tuple<Vector2, float>(Vector2.up, torque);
        }

        private bool IsInShape()
        {
            if (!PositionEstimated)
            {
                return false;
            }

            Vector2 closestPoint = _targetShape.ClosestPoint(PositionEstimate);
            return closestPoint == PositionEstimate;
        }
    }
}
