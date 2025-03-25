using System.Numerics;
using Raylib_cs;

namespace SharkEngine.Gameplay
{
    public class WeaponNode : LightNode
    {
        public float FireCooldown = 2f;
        public float EnergyCostPerShot = 3f;

        private float timeSinceLastFire = 0f;

        public WeaponNode(LightNode? parent = null)
            : base(parent)
        {
            Type = NodeType.Weapon;
            BaseRadius = 50f;
            EnergyReserve = 0f;
            IgnitionCost = 5f;
            StateColors = new Dictionary<NodeState, Color>
            {
                { NodeState.Inactive, Color.DarkPurple },
                { NodeState.Igniting, Raylib.Fade(Color.Orange, 0.6f) },
                { NodeState.Active, Color.Red },
                { NodeState.Dead, Color.Gray }
            };
        }
        public override float GetChildRadius(NodeType childType) => 50f;

        public override float GetEnergyFromSelf(float deltaTime)
        {
            return 0f; // Doesn't generate energy
        }

        public override float GetEnergyOutput(LightNode target, float deltaTime)
        {
            return 0f; // Doesn't share energy with others (for now)
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (CurrentState == NodeState.Active)
            {
                timeSinceLastFire += deltaTime;

                if (timeSinceLastFire >= FireCooldown)
                {
                    float drawn = Parent?.GetEnergyOutput(this, deltaTime) ?? 0f;

                    if (drawn >= EnergyCostPerShot)
                    {
                        timeSinceLastFire = 0f;
                        // 🔫 FIRE EVENT: Simulate attack
                        // Eventually this would spawn a projectile or trigger damage
                    }
                }
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
    }
}
