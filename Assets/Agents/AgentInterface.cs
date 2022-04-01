using System;
using System.Collections.Generic;
using UnityEngine;

namespace Agents
{
    public interface IAgentInterface
    {
        KilobotMessage GetMessage();
        Tuple<Vector2, float, KilobotMessage> Act(List<Tuple<float, KilobotMessage>> messageList);
        bool PositionEstimated { get; }
        Vector2 PositionEstimate { get; }
        Color GetStateColor();
        String GetDisplayText();
    }
}