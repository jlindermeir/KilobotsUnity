using System;
using System.Collections.Generic;
using UnityEngine;

namespace Agents
{
    public interface IAgentInterface<TMessage>
    {
        TMessage GetMessage();
        Tuple<Vector2, float, TMessage> Act(List<Tuple<float, TMessage>> messageList);
        bool PositionEstimated { get; }
        Vector2 PositionEstimate { get; }
        Color GetStateColor();
        String GetDisplayText();
    }
}