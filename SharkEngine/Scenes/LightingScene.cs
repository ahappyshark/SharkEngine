using System.Numerics;
using Raylib_cs;
using SharkEngine.Core;
using SharkEngine.Gameplay;

namespace SharkEngine.Scenes
{
    public class LightingScene : Scene
    {
        private LightNode rootNode;
        private GameCamera gameCamera;

        private RenderTexture2D sceneTexture;
        private RenderTexture2D lightMap;

        private Vector2 originPosition;


        private const float MaxIgniteDistance = 200f;
        private const float MinNodeSpacing = 75f;
        private NodeType currentSelectedType = NodeType.Star;

        // 🔍 Preview state
        private Vector2 previewPosition;
        private bool previewValid = false;

        public override void Load()
        {
            sceneTexture = Raylib.LoadRenderTexture(GameConfig.ScreenWidth, GameConfig.ScreenHeight);
            lightMap = Raylib.LoadRenderTexture(GameConfig.ScreenWidth, GameConfig.ScreenHeight);

            originPosition = new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f);

            gameCamera = new GameCamera();

            rootNode = NodeFactory.CreateNode(NodeType.Star, originPosition);
            rootNode.Ignite();
            rootNode.Update(999);
        }

        public override void Unload()
        {
            Raylib.UnloadRenderTexture(sceneTexture);
            Raylib.UnloadRenderTexture(lightMap);
        }

        public override void Update(float deltaTime)
        {
            gameCamera.Update(deltaTime);
            UpdateNodeTree(rootNode, deltaTime);

            Vector2 mouseWorld = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), gameCamera.Camera);
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
            if (Raylib.IsKeyPressed(KeyboardKey.One)) currentSelectedType = NodeType.Star;
            if (Raylib.IsKeyPressed(KeyboardKey.Two)) currentSelectedType = NodeType.Capacitor;
            if (Raylib.IsKeyPressed(KeyboardKey.Three)) currentSelectedType = NodeType.Weapon;
        }

        private void TryIgniteNodeAt(Vector2 targetPos)
        {
            LightNode? nearestActive = FindNearestActiveNode(rootNode, targetPos, MaxIgniteDistance);

            if (nearestActive != null && !IsTooCloseToExistingNode(rootNode, targetPos, MinNodeSpacing))
            {
                var newNode = NodeFactory.CreateNode(currentSelectedType, targetPos, nearestActive);
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
                if (current.IsActive())
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
        

        public override void Draw()
        {
            float totalEnergy = GetTotalEnergy(rootNode);

            // 🎨 Draw world background and screen-space UI
            Raylib.BeginTextureMode(sceneTexture);
            Raylib.EndTextureMode();

            // 💡 Draw light map and world content
            Raylib.BeginTextureMode(lightMap);
            Raylib.ClearBackground(Color.Black);

            Raylib.BeginMode2D(gameCamera.Camera);

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
            Raylib.DrawText($"Node Type: {currentSelectedType}", 10, 70, 20, Color.White);
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
            node.Draw();
            foreach (var child in node.Children)
                DrawNodeTree(child);
        }
    }
}
