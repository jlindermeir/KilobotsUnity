using System;
using System.Collections.Generic;
using Agents.ShapeAssembly;
using UnityEngine;

namespace Agents
{
    public class PotentialMinimizationAgent : IAgentInterface<ShapeAssemblyMessage>
    {
        private enum State
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

        public ShapeAssemblyMessage GetMessage()
        {
            throw new NotImplementedException();
        }

        public Tuple<Vector2, float, ShapeAssemblyMessage> Act(List<Tuple<float, ShapeAssemblyMessage>> messageList)
        {
            throw new NotImplementedException();
        }

        public bool PositionEstimated => false;
        public Vector2 PositionEstimate { get; } = Vector2.zero;
        public Color GetStateColor()
        {
            return StateColor[_currentState];
        }

        public string GetDisplayText()
        {
            throw new NotImplementedException();
        }
    }
}