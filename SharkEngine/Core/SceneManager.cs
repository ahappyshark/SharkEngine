namespace SharkEngine.Core
{
    public class SceneManager
    {
        private Scene currentScene;

        public void ChangeScene(Scene newScene)
        {
            currentScene?.Unload();
            currentScene = newScene;
            currentScene.Load();
        }

        public void Update(float deltaTime)
        {
            currentScene?.Update(deltaTime);
        }

        public void Draw()
        {
            currentScene?.Draw();
        }
    }
}
