using System.Numerics;

namespace SharkEngine.Gameplay
{
    public enum NodeType
    {
        Star,
        Capacitor,
        Weapon,
        Auxiliary
    }

    public static class NodeFactory
    {
        public static LightNode CreateNode(NodeType type, Vector2 position, LightNode? parent = null)
        {
            return type switch
            {
                NodeType.Star => new StarNode(position, parent),
                NodeType.Capacitor => new CapacitorNode(position, parent),
                NodeType.Weapon => new WeaponNode(position, parent),
                // NodeType.Auxiliary => new AuxiliaryNode(position, parent),
                _ => throw new ArgumentException($"Unsupported node type: {type}")
            };
        }
    }
}
