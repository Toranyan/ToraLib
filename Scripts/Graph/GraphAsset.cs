using System.Collections.Generic;
using UnityEngine;

namespace tora.graph
{
    public abstract class GraphAsset<TNode> : ScriptableObject where TNode : GraphNode
    {
        /// <summary>[SerializeReference] so TNode can be abstract with multiple concrete node
        /// kinds as elements (e.g. DialogueGraph's TextNode/ChoiceNode/...). Works the same
        /// for a single concrete TNode (e.g. StoryNode) with only one runtime type in play.</summary>
        [SerializeReference]
        public List<TNode> Nodes = new List<TNode>();

        public TNode GetNode(string id)
        {
            foreach (var node in Nodes)
            {
                if (node.Id == id)
                {
                    return node;
                }
            }
            return null;
        }
    }
}
