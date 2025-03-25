using System.Numerics;
using Raylib_cs;

namespace SharkEngine.Gameplay
{
    public class CapacitorNode : LightNode
    {
        public float MaxStorage { get; set; } = 20f;
        public float RechargeRate { get; set; } = 1f;
        public CapacitorNode(LightNode? parent = null)
            : base(parent)
        {
            Type = NodeType.Capacitor;
            BaseRadius = 50f;
            EnergyReserve = 0f;
            IgnitionCost = 5f;
            StateColors = new Dictionary<NodeState, Color>
            {
                { NodeState.Inactive, Color.DarkPurple },
                { NodeState.Igniting, Raylib.Fade(Color.Purple, 0.6f) },
                { NodeState.Active, Color.Purple },
                { NodeState.Dead, Color.Gray }
            };
        }
        public override float GetChildRadius(NodeType childType) => 50f;
        public override float GetEnergyFromSelf(float deltaTime)
        {
            // Capacitor doesn't generate energy
            return 0f;
        }
        public override float GetEnergyOutput(LightNode target, float deltaTime)
        {
            float request = RechargeRate * deltaTime;
            float provided = MathF.Min(EnergyReserve, request);
            EnergyReserve -= provided;
            return provided;
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (CurrentState == NodeState.Active && Parent != null)
            {
                // Pull energy from parent slowly
                float incoming = Parent.GetEnergyOutput(this, deltaTime);
                EnergyReserve = MathF.Min(EnergyReserve + incoming, MaxStorage);
            }
        }
        public override void Draw()
        {
            Raylib.DrawCircleGradient((int)Position.X, (int)Position.Y, AnimatedRadius, CurrentColor, Color.Blank);

            if (Parent != null)
            {
                Color blended = BlendColors(Parent.CurrentColor, CurrentColor, 0.5f);
                Color beamColor = Raylib.Fade(blended, BeamAlpha);

                Raylib.DrawLineEx(Parent.Position, Position, BeamThickness, beamColor);
            }
        }
        private Color BlendColors(Color a, Color b, float t)
        {
            return new Color(
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t),
                (int)(a.A + (b.A - a.A) * t)
            );
        }
        public override void RepositionChildren()
        {
            throw new NotImplementedException();
        }
    }
}
