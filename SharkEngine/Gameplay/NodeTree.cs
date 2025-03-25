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
            return true;
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
        // public LightNode? FindNearestActiveNode(Vector2 targetPos)
        // {
        //     LightNode? best = null;

        //     void Recurse(LightNode current)
        //     {
        //         if (current.IsActive())
        //         {
        //             float dist = Vector2.Distance(current.Position, targetPos);
        //             if (dist <= bestDist)
        //             {
        //                 best = current;
        //                 bestDist = dist;
        //             }
        //         }

        //         foreach (var child in current.Children)
        //             Recurse(child);
        //     }
        //     Recurse(rootNode);
        //     return best;
        // }
        public bool IsTooClose(Vector2 targetPos, float minSpacing) {
            return IsTooCloseToExistingNode(rootNode, targetPos, minSpacing);
        }
        public bool IsTooCloseToExistingNode(LightNode node, Vector2 targetPos, float minSpacing)
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
    }
}