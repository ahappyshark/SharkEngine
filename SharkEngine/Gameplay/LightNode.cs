using System.Numerics;

namespace SharkEngine.Gameplay
{
    public enum NodeState
    {
        Inactive,
        Igniting,
        Active,
        Dead
    }

    public class LightNode
    {
        public Vector2 Position { get; }
        public LightNode? Parent { get; private set; }
        public List<LightNode> Children { get; } = new();

        public NodeState State { get; private set; } = NodeState.Inactive;

        public float EnergyProduction { get; private set; } = 1f;
        public float EnergyReserve { get; private set; } = 0f;
        public float IgnitionCost { get; } = 5f;

        public LightNode(Vector2 position, LightNode? parent = null)
        {
            Position = position;
            Parent = parent;
            parent?.Children.Add(this);
        }

        public bool IsConnectedToRoot()
        {
            return State == NodeState.Active && (Parent == null || Parent.IsConnectedToRoot());
        }

        public void Ignite()
        {
            if (State == NodeState.Inactive && (Parent == null || Parent.State == NodeState.Active))
            {
                State = NodeState.Igniting;
                EnergyReserve = IgnitionCost;
            }
        }

        public void Update(float deltaTime)
        {
            if (State == NodeState.Igniting)
            {
                if (Parent == null)
                {
                    EnergyReserve += EnergyProduction * deltaTime;
                }
                else
                {
                    EnergyReserve += Parent.EnergyProduction * deltaTime;
                }


                if (EnergyReserve >= IgnitionCost)
                {
                    EnergyReserve = 0f;
                    State = NodeState.Active;
                }
            }
            else if (State == NodeState.Active)
            {
                EnergyReserve += EnergyProduction * deltaTime;
                if (EnergyReserve > 10f) { EnergyReserve = 10f; }
                // Cap or decay logic can go here
            }

        }

        public void Destroy()
        {
            State = NodeState.Dead;
            foreach (var child in Children)
            {
                child.Destroy();
            }
            Children.Clear();
        }
    }
}
