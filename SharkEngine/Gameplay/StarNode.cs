using System.Numerics;
using Raylib_cs;

namespace SharkEngine.Gameplay
{
    public class StarNode : LightNode
    {
        public float EnergyProductionRate { get; set; } = 2f;
        public float MaxStorage { get; set; } = 10f;

        public StarNode(LightNode? parent = null)
            : base(parent)
        {
            Type = NodeType.Star;
            BaseRadius = 100f;
            EnergyReserve = 0f;
            IgnitionCost = 10f;
            // Define this node’s color palette
            StateColors = new Dictionary<NodeState, Color>
            {
                { NodeState.Inactive, Color.Gray },
                { NodeState.Igniting, Color.Yellow },
                { NodeState.Active, Color.White },
                { NodeState.Dead, Color.DarkGray }
            };
        }
        public override float GetChildRadius(NodeType childType) => 100f;
        

        public override float GetEnergyFromSelf(float deltaTime)
        {
            if (!CanGenerate()) return 0f;

            float produced = EnergyProductionRate * deltaTime;
            EnergyReserve = MathF.Min(EnergyReserve + produced, MaxStorage);
            return 0f;
        }

        public override float GetEnergyOutput(LightNode target, float deltaTime)
        {
            if (CurrentState != NodeState.Active) return 0f;

            float amount = EnergyProductionRate * deltaTime;
            EnergyReserve = MathF.Max(0f, EnergyReserve - amount);
            return amount;
        }

        public override void Draw()
        {
            // Node body
            Raylib.DrawCircleGradient(
                (int)Position.X,
                (int)Position.Y,
                AnimatedRadius,
                CurrentColor,
                Color.Blank
            );

            // Beam to parent
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
            var starChildren = Children.Where(c => c.Type == NodeType.Star).ToList();
            int count = starChildren.Count;
            if (count == 0) return;

            if (Parent is LightNode parent)
            {
                var directionToParent = Vector2.Normalize(Position - parent.Position);                
                float distanceFromParent = BaseRadius + OrbitStep * MathF.Max(1, Descendants);                
                SetPosition(parent.Position + directionToParent * distanceFromParent);
            }
            
            float baseAngle = 0f;
            if (Parent is LightNode p)
            {
                Vector2 toParent = Vector2.Normalize(p.Position - Position);
                baseAngle = MathF.Atan2(toParent.Y, toParent.X); // 180° opposite of parent
            }

            for (int i = 0; i < count; i++)
            {
                float angle = baseAngle + MathF.PI * 2f * (i + 1) / (count + 1);
                Vector2 offset = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * starChildren[i].BaseRadius;
                starChildren[i].SetPosition(Position + offset);
            }

            foreach (var child in Children)
                child.RepositionChildren();
        }
    }
}
