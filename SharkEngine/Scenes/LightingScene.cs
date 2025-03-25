using System.Numerics;
using Raylib_cs;
using SharkEngine.Core;
using SharkEngine.Gameplay;

namespace SharkEngine.Scenes
{
    public class LightingScene : Scene
    {
        private GameCamera gameCamera;
        private NodeTree nodeTree;
        private RenderTexture2D sceneTexture;
        private RenderTexture2D lightMap;
        private NodeType currentSelectedType = NodeType.Star;
        // 🔍 Preview state
        private Vector2 previewPosition;
        private bool previewValid = false;

        public override void Load()
        {
            sceneTexture = Raylib.LoadRenderTexture(GameConfig.ScreenWidth, GameConfig.ScreenHeight);
            lightMap = Raylib.LoadRenderTexture(GameConfig.ScreenWidth, GameConfig.ScreenHeight);

            gameCamera = new GameCamera();
            nodeTree = new NodeTree();
        }

        public override void Unload()
        {
            Raylib.UnloadRenderTexture(sceneTexture);
            Raylib.UnloadRenderTexture(lightMap);
        }

        public override void Update(float deltaTime)
        {
            gameCamera.Update(deltaTime);
            nodeTree.Update(deltaTime);

            Vector2 mouseWorld = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), gameCamera.Camera);

            if (Raylib.IsKeyPressed(KeyboardKey.One)) currentSelectedType = NodeType.Star;
            if (Raylib.IsKeyPressed(KeyboardKey.Two)) currentSelectedType = NodeType.Capacitor;
            if (Raylib.IsKeyPressed(KeyboardKey.Three)) currentSelectedType = NodeType.Weapon;
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                Console.WriteLine("Mouse clicked.");
                if (nodeTree.TryAddNode(mouseWorld, currentSelectedType)) {
                    Console.WriteLine("Node placed.");
                }
            }            
        }
        public override void Draw()
        {
            float totalEnergy = nodeTree.TotalEnergy;

            // 🎨 Draw world background and screen-space UI
            Raylib.BeginTextureMode(sceneTexture);
            Raylib.EndTextureMode();

            // 💡 Draw light map and world content
            Raylib.BeginTextureMode(lightMap);
            Raylib.ClearBackground(Color.Black);

            Raylib.BeginMode2D(gameCamera.Camera);

            // Draw active nodes, beams, and radiance
            nodeTree.Draw();

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
    }
}
