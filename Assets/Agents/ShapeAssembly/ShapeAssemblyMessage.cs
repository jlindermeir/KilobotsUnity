using UnityEngine;

namespace Agents.ShapeAssembly
{
    public struct ShapeAssemblyMessage
    {
        public ShapeAssemblyMessage(int gradient, ShapeAssemblyAgent.State state, Vector2 position, float id)
        {
            Gradient = gradient;
            State = state;
            Position = position;
            ID = id;
        }

        public int Gradient;
        public ShapeAssemblyAgent.State State;
        public Vector2 Position;
        public float ID;
    }
}
