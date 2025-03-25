using System.Numerics;
using Raylib_cs;

namespace SharkEngine.Gameplay
{
    public enum NodeState
    {
        Inactive,
        Igniting,
        Active,
        Dead
    }

    public abstract class LightNode
    {
        public Vector2 Position { get; private set; }
        public NodeType Type { get; protected set; }
        public float ParentRadius { get; private set; }
        public LightNode? Parent { get; private set; }
        public List<LightNode> Children { get; } = new();
        public int Descendants { get; set; }
        public NodeState CurrentState { get; protected set; } = NodeState.Inactive;
        public Color CurrentColor => StateColors.TryGetValue(CurrentState, out var color)
            ? color
            : Color.LightGray;

        public Dictionary<NodeState, Color> StateColors { get; protected set; } = new();

        public float EnergyReserve { get; protected set; }
        public float IgnitionCost { get; protected set; }

        public float AnimatedRadius { get; protected set; }
        public float BaseRadius { get; protected set; }
        public float PulseTime { get; protected set; }
        public float BeamPulseTime { get; protected set; }
        public float BeamAlpha { get; protected set; }
        public float BeamThickness { get; protected set; }
        public float CurrentOrbitRadius { get; protected set; } = 100f;
        protected const float OrbitStep  = 50f;

        public LightNode(LightNode? parent = null)
        {
            Parent = parent;
            parent?.Children.Add(this);
            ParentRadius = parent?.GetChildRadius(this.Type) ?? 100f;
        }
        public void SetPosition(Vector2 newPos)
        {
            Position = newPos;
        }

        public bool IsConnectedToRoot()
        {
            return CurrentState == NodeState.Active && (Parent == null || Parent.IsConnectedToRoot());
        }

        public void Ignite()
        {
            if (CurrentState == NodeState.Inactive && (Parent == null || Parent.CurrentState == NodeState.Active))
            {
                CurrentState = NodeState.Igniting;
                EnergyReserve = 0f;
            }
        }

        public virtual void Update(float deltaTime)
        {
            PulseTime += deltaTime;
            BeamPulseTime += deltaTime;

            AnimatedRadius = BaseRadius + 20f * MathF.Sin(PulseTime * 2f);
            BeamAlpha = 0.5f + 0.5f * MathF.Sin(BeamPulseTime * 4f);
            BeamThickness = 4f + 2f * MathF.Sin(BeamPulseTime * 4f);

            if (CurrentState == NodeState.Igniting)
            {
                float input = Parent == null
                    ? GetEnergyFromSelf(deltaTime)
                    : Parent.GetEnergyOutput(this, deltaTime);

                EnergyReserve += input;

                if (EnergyReserve >= IgnitionCost)
                {
                    EnergyReserve = 0f;
                    CurrentState = NodeState.Active;
                }
            }
        }

        public virtual void Destroy()
        {
            CurrentState = NodeState.Dead;
            foreach (var child in Children)
                child.Destroy();
            Children.Clear();
        }
        public abstract void RepositionChildren();
        public virtual bool IsActive() {
            return CurrentState == NodeState.Active;
        }
        protected virtual bool CanGenerate()
        {
            return Parent == null || CurrentState == NodeState.Active;
        }
        public virtual float GetChildRadius(NodeType childType) => 100f;
        public abstract float GetEnergyFromSelf(float deltaTime);
        public abstract float GetEnergyOutput(LightNode target, float deltaTime);
        public abstract void Draw();
    }
}
