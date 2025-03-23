namespace SharkEngine.Core
{
    public abstract class Scene
    {
        public virtual void Load() { }
        public virtual void Unload() { }
        public abstract void Update(float deltaTime);
        public abstract void Draw();
    }
}
