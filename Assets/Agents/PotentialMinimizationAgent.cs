using System;
using System.Collections.Generic;
using UnityEngine;

namespace Agents
{
    public class PotentialMinimizationAgent : IAgentInterface
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

        public KilobotMessage GetMessage()
        {
            throw new NotImplementedException();
        }

        public Tuple<Vector2, float, KilobotMessage> Act(List<Tuple<float, KilobotMessage>> messageList)
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