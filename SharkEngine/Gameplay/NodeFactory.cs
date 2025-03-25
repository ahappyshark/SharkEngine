using System.Numerics;

namespace SharkEngine.Gameplay
{
    public enum NodeType
    {
        Root,
        Star,
        Capacitor,
        Weapon,
        Auxiliary
    }

    public static class NodeFactory
    {
        public static LightNode CreateNode(NodeType type, LightNode? parent = null)
        {
            return type switch
            {
                NodeType.Star => new StarNode(parent),
                NodeType.Capacitor => new CapacitorNode(parent),
                NodeType.Weapon => new WeaponNode(parent),
                NodeType.Root => new RootNode(parent),
                // NodeType.Auxiliary => new AuxiliaryNode(position, parent),
                _ => throw new ArgumentException($"Unsupported node type: {type}")
            };
        }
        public static bool IsValidParent(NodeType childType, NodeType parentType)
        {
            return childType switch
            {
                NodeType.Star => parentType is NodeType.Star or NodeType.Root,
                NodeType.Capacitor => parentType is NodeType.Star,
                NodeType.Weapon => parentType == NodeType.Star,
                _ => false
            };
        }
    }
}
