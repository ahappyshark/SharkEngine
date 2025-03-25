using System.Numerics;
using Raylib_cs;
using SharkEngine.Core;

namespace SharkEngine.Gameplay
{
    public class NodeTree
    {
        private LightNode rootNode;
        public float TotalEnergy { 
            get {
                return GetTotalEnergy(rootNode);
            }
        }
        private Vector2 originPosition;
        public NodeTree() {
            originPosition = new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f);
            rootNode = NodeFactory.CreateNode(NodeType.Root);
            rootNode.SetPosition(new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f));
            rootNode.Ignite();
            rootNode.Update(999);
        }
        public void Update(float deltaTime) {
            UpdateNodeTree(rootNode, deltaTime);
        }
        private void UpdateNodeTree(LightNode node, float deltaTime)
        {
            node.Update(deltaTime);
            foreach (var child in node.Children)
                UpdateNodeTree(child, deltaTime);
        }
        public void Draw() {
            DrawNodeTree(rootNode);
        }
        private void DrawNodeTree(LightNode node)
        {
            node.Draw();
            foreach (var child in node.Children)
                DrawNodeTree(child);
        }
        
        private float GetTotalEnergy(LightNode node)
        {
            float total = node.EnergyReserve;

            foreach (var child in node.Children)
                total += GetTotalEnergy(child);

            return total;
        }
        public bool TryAddNode(Vector2 clickPos, NodeType newType)
        {
            LightNode? parent = GetClickedNode(clickPos);
            if (parent == null || !parent.IsActive()) return false;

            if (!NodeFactory.IsValidParent(newType, parent.Type)) return false;

            var node = NodeFactory.CreateNode(newType, parent);
            node.Ignite();
            RecalculateAll();
            return true;
        }
        public void RecalculateAll()
        {
            CountStarDescendants(rootNode);
            rootNode.RepositionChildren(); // layout from root outward
        }
        public int CountStarDescendants(LightNode node)
        {
            int count = 0;

            foreach (var child in node.Children)
            {
                int childDescendants = CountStarDescendants(child);
                count += childDescendants;

                // Only count this node if it's a Star
                if (child.Type == NodeType.Star)
                    count += 1;
            }

            node.Descendants = count;
            return count;
        }
        public LightNode? GetClickedNode(Vector2 mouseWorldPos, float clickRadius = 30f) 
        {
            LightNode? clickedNode = null;
            void Recurse(LightNode current)
            {
                if(Vector2.Distance(current.Position, mouseWorldPos) < clickRadius)
                    clickedNode = current;
                foreach (var child in current.Children)
                    Recurse(child);
            }
            Recurse(rootNode);
            return clickedNode;
        }
    }
}