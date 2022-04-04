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
        private float _gas;
        
        private Vector2 _previousMotionDirection = Vector2.up;
        private float _previousEnergy = 0;
        private static float _movementGasCost = 0.04f;
        private static float _gasGain = 0.1f;

        public PotentialMinimizationAgent(State initialState = State.Idle)
        {
            _currentState = initialState;
            _gas = Random.value;
        }

        public PotentialMinimizationMessage GetMessage()
        {
            return new PotentialMinimizationMessage(_currentState, _totalEnergy, _gas);
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
            // Idle, no movement
            Tuple<Vector2, float> returnVal = new Tuple<Vector2, float>(Vector2.zero, 0);
            
            // Add gas
            _gas += _gasGain * Time.deltaTime * (1 - _gas);
            
            // If no other bot is moving and has higher gas, switch to moving
            foreach (var (_, message) in messageList)
            {
                // Skip fixed bots
                if (message.State == State.Fixed)
                {
                    continue;
                }
                
                if (message.Gas > _gas | message.State == State.Moving)
                {
                    return returnVal;
                }
            }

            _currentState = State.Moving;
            return returnVal;
        }
        
        private Tuple<Vector2, float> ProcessMoving(List<Tuple<float, PotentialMinimizationMessage>> messageList)
        {
            // If the gas is empty, switch to idle
            if (_gas <= 0)
            {
                _gas = 0;
                _currentState = State.Idle;
                return new Tuple<Vector2, float>(Vector2.zero, 0);
            } 
            
            // If the previous energy is higher than the current one, keep moving in the same direction, else move randomly
            Vector2 direction = (_previousEnergy > _totalEnergy) ? _previousMotionDirection : Random.insideUnitCircle;
            
            // Store energy and direction
            _previousEnergy = _totalEnergy;
            _previousMotionDirection = direction;
            
            // Deduct gas costs
            _gas -= _movementGasCost * Time.deltaTime;
            
            return new Tuple<Vector2, float>(direction, 0);
        }

        private static float PotentialFunction(float distance)
        {
            return -20f * (1 / distance - 1 / (distance * distance));
        }

        private void CalculateTotalPotentialEnergy(List<Tuple<float, PotentialMinimizationMessage>> messageList)
        {
            _totalEnergy = 0;

            int nNeighbors = messageList.Count;

            if (nNeighbors == 0)
            {
                return;
            }

            foreach ((float distance, _) in messageList)
            {
                _totalEnergy += PotentialFunction(distance);
            }
        }

        public bool PositionEstimated => false;
        public Vector2 PositionEstimate { get; } = Vector2.zero;
        public Color GetStateColor()
        {
            Color stateColor = StateColor[_currentState];
            return new Color(stateColor.r, stateColor.g, stateColor.b, (1 + _gas) / 2);
        }

        public string GetDisplayText()
        {
            return String.Format("", _gas);
        }
    }
}