using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Agents.PotentialMinimization
{
    public class PotentialMinimizationAgent : IAgentInterface<PotentialMinimizationMessage>
    {
        public enum State
        {
            Fixed,
            Idle,
            Moving
        }
        
        private static readonly Dictionary<State, Color> StateColor = new Dictionary<State, Color>()
        {
            {State.Fixed, Color.gray},
            {State.Idle, Color.magenta},
            {State.Moving, Color.green}
        };

        private State _currentState;
        private float _totalEnergy = 0;

        public PotentialMinimizationAgent(State initialState = State.Idle)
        {
            _currentState = initialState;
        }

        public PotentialMinimizationMessage GetMessage()
        {
            return new PotentialMinimizationMessage(_currentState, _totalEnergy);
        }

        public Tuple<Vector2, float, PotentialMinimizationMessage> Act(List<Tuple<float, PotentialMinimizationMessage>> messageList)
        {
            CalculateTotalPotentialEnergy(messageList);

            float torque = 0;
            Vector2 motionDirection = Vector2.zero;

            switch (_currentState)
            {
                case State.Fixed:
                    break;
                case State.Idle:
                    (motionDirection, torque) = ProcessIdle(messageList);
                    break;
                case State.Moving:
                    (motionDirection, torque) = ProcessMoving(messageList);
                    break;
            }

            return new Tuple<Vector2, float, PotentialMinimizationMessage>(motionDirection, torque, GetMessage());
        }

        private Tuple<Vector2, float> ProcessIdle(List<Tuple<float, PotentialMinimizationMessage>> messageList)
        {
            Tuple<Vector2, float> returnVal = new Tuple<Vector2, float>(Vector2.zero, 0);
            
            // If no other bot is moving and has higher potential energy, switch to moving
            foreach (var (_, message) in messageList)
            {
                if (message.PotentialEnergy > _totalEnergy | message.State == State.Moving)
                {
                    return returnVal;
                }
            }

            _currentState = State.Moving;
            return returnVal;
        }
        
        private Tuple<Vector2, float> ProcessMoving(List<Tuple<float, PotentialMinimizationMessage>> messageList)
        {
            // If other bots have higher potential energy, switch to idle
            foreach (var (_, message) in messageList)
            {
                if (message.PotentialEnergy > _totalEnergy)
                {
                    _currentState = State.Idle;
                    return new Tuple<Vector2, float>(Vector2.zero, 0);
                }
            }

            return new Tuple<Vector2, float>(Random.insideUnitCircle, 0);
        }

        private static float PotentialFunction(float distance)
        {
            return 1 / distance;
        }

        private void CalculateTotalPotentialEnergy(List<Tuple<float, PotentialMinimizationMessage>> messageList)
        {
            _totalEnergy = 0;

            foreach ((float distance, _) in messageList)
            {
                _totalEnergy += PotentialFunction(distance);
            }
        }

        public bool PositionEstimated => false;
        public Vector2 PositionEstimate { get; } = Vector2.zero;
        public Color GetStateColor()
        {
            return StateColor[_currentState];
        }

        public string GetDisplayText()
        {
            return String.Format("{0:0.00}", _totalEnergy);
        }
    }
}