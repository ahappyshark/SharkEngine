using System.Numerics;
using Raylib_cs;
using SharkEngine.Core;
using SharkEngine.Gameplay;

namespace SharkEngine.Scenes
{
    public class LightingScene : Scene
    {
        private LightNode rootNode;
        private Camera2D camera;
        private float cameraSpeed = 300f;

        private RenderTexture2D sceneTexture;
        private RenderTexture2D lightMap;

        private Vector2 originPosition;

        private float pulseTime = 0f;
        private float beamPulseTime = 0f;

        private const float MaxIgniteDistance = 200f;
        private const float MinNodeSpacing = 75f;
        private float baseRadius = 100f;

        // 🔍 Preview state
        private Vector2 previewPosition;
        private bool previewValid = false;

        public override void Load()
        {
            sceneTexture = Raylib.LoadRenderTexture(GameConfig.ScreenWidth, GameConfig.ScreenHeight);
            lightMap = Raylib.LoadRenderTexture(GameConfig.ScreenWidth, GameConfig.ScreenHeight);

            originPosition = new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f);

            rootNode = new LightNode(originPosition);
            rootNode.Ignite();
            rootNode.Update(999);

            var nodeA = new LightNode(new Vector2(originPosition.X + 200, originPosition.Y - 100), rootNode);
            var nodeB = new LightNode(new Vector2(originPosition.X + 300, originPosition.Y + 150), rootNode);
            var nodeC = new LightNode(new Vector2(originPosition.X - 250, originPosition.Y + 200), rootNode);
            nodeA.Ignite(); nodeA.Update(999);
            nodeB.Ignite(); nodeB.Update(999);
            nodeC.Ignite(); nodeC.Update(999);

            camera = new Camera2D
            {
                Offset = new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f),
                Target = originPosition,
                Rotation = 0.0f,
                Zoom = 1.0f
            };
        }

        public override void Unload()
        {
            Raylib.UnloadRenderTexture(sceneTexture);
            Raylib.UnloadRenderTexture(lightMap);
        }

        public override void Update(float deltaTime)
        {
            pulseTime += deltaTime;
            beamPulseTime += deltaTime;

            HandleCameraMovement(deltaTime);
            UpdateNodeTree(rootNode, deltaTime);

            Vector2 mouseWorld = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), camera);
            previewPosition = mouseWorld;

            LightNode? nearest = FindNearestActiveNode(rootNode, mouseWorld, MaxIgniteDistance);
            bool notTooClose = !IsTooCloseToExistingNode(rootNode, mouseWorld, MinNodeSpacing);
            previewValid = nearest != null && notTooClose;
            Console.WriteLine($"Preview valid: {previewValid} | Nearest null? {nearest == null} | Too close? {!notTooClose}");


            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                Console.WriteLine("Mouse clicked.");
                TryIgniteNodeAt(mouseWorld);
            }
        }

        private void TryIgniteNodeAt(Vector2 targetPos)
        {
            LightNode? nearestActive = FindNearestActiveNode(rootNode, targetPos, MaxIgniteDistance);

            if (nearestActive != null && !IsTooCloseToExistingNode(rootNode, targetPos, MinNodeSpacing))
            {
                var newNode = new LightNode(targetPos, nearestActive);
                newNode.Ignite();
            }
        }

        private void UpdateNodeTree(LightNode node, float deltaTime)
        {
            node.Update(deltaTime);
            foreach (var child in node.Children)
                UpdateNodeTree(child, deltaTime);
        }

        private bool IsTooCloseToExistingNode(LightNode node, Vector2 targetPos, float minSpacing)
        {
            if (Vector2.Distance(node.Position, targetPos) < minSpacing)
                return true;

            foreach (var child in node.Children)
            {
                if (IsTooCloseToExistingNode(child, targetPos, minSpacing))
                    return true;
            }

            return false;
        }

        private LightNode? FindNearestActiveNode(LightNode node, Vector2 targetPos, float maxDistance)
        {
            LightNode? best = null;
            float bestDist = maxDistance;

            void Recurse(LightNode current)
            {
                if (current.State == NodeState.Active)
                {
                    float dist = Vector2.Distance(current.Position, targetPos);
                    if (dist <= bestDist)
                    {
                        best = current;
                        bestDist = dist;
                    }
                }

                foreach (var child in current.Children)
                    Recurse(child);
            }
            Recurse(node);
            return best;
        }
        private float GetTotalEnergy(LightNode node)
        {
            float total = node.EnergyReserve;

            foreach (var child in node.Children)
                total += GetTotalEnergy(child);

            return total;
        }
        private void HandleCameraMovement(float deltaTime)
        {
            Vector2 cameraMove = Vector2.Zero;

            if (Raylib.IsKeyDown(KeyboardKey.Right) || Raylib.IsKeyDown(KeyboardKey.D)) cameraMove.X += 1;
            if (Raylib.IsKeyDown(KeyboardKey.Left) || Raylib.IsKeyDown(KeyboardKey.A)) cameraMove.X -= 1;
            if (Raylib.IsKeyDown(KeyboardKey.Down) || Raylib.IsKeyDown(KeyboardKey.S)) cameraMove.Y += 1;
            if (Raylib.IsKeyDown(KeyboardKey.Up) || Raylib.IsKeyDown(KeyboardKey.W)) cameraMove.Y -= 1;

            if (cameraMove != Vector2.Zero)
                camera.Target += cameraMove * cameraSpeed * deltaTime;
        }

        public override void Draw()
        {
            float totalEnergy = GetTotalEnergy(rootNode);

            // 🎨 Draw world background and screen-space UI
            Raylib.BeginTextureMode(sceneTexture);
            Raylib.EndTextureMode();

            // 💡 Draw light map and world content
            Raylib.BeginTextureMode(lightMap);
            Raylib.ClearBackground(Color.Black);

            Raylib.BeginMode2D(camera);

            // Draw active nodes, beams, and radiance
            DrawNodeTree(rootNode);

            // 🔍 Draw node placement preview
            DrawPlacementPreview();

            Raylib.EndMode2D();
            Raylib.EndTextureMode();

            // 🧩 Composite final scene to screen            
            Renderer.DrawRenderTexture(lightMap, 0, 0, Color.White);
            Renderer.DrawRenderTexture(sceneTexture, 0, 0, Color.White);
            Raylib.DrawText($"Total Energy: {totalEnergy:F1}", 10, 40, 20, Color.White);
        }
        private void DrawPlacementPreview()
        {
            Color previewColor = previewValid
                ? Raylib.Fade(Color.White, 0.4f)
                : Raylib.Fade(Color.Red, 0.3f);

            Raylib.DrawCircle((int)previewPosition.X, (int)previewPosition.Y, 50, previewColor);
        }

        private void DrawNodeTree(LightNode node)
        {
            float animatedRadius = baseRadius + 20f * MathF.Sin(pulseTime * 2f);
            float beamAlpha = 0.5f + 0.5f * MathF.Sin(beamPulseTime * 4f);
            float beamThickness = 4f + 2f * MathF.Sin(beamPulseTime * 4f);
            Color beamColor = Raylib.Fade(Color.White, beamAlpha);

            if (node.Parent != null)
            {
                Raylib.DrawLineEx(node.Parent.Position, node.Position, beamThickness, beamColor);
            }

            Color coreColor = node.State switch
            {
                NodeState.Inactive => Color.Gray,
                NodeState.Igniting => Color.Yellow,
                NodeState.Active => Color.White,
                NodeState.Dead => Color.DarkGray,
                _ => Color.LightGray
            };

            Raylib.DrawCircleGradient((int)node.Position.X, (int)node.Position.Y, animatedRadius, coreColor, Color.Blank);

            foreach (var child in node.Children)
                DrawNodeTree(child);
        }
    }
}
