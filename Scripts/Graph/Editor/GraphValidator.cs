using System.Collections.Generic;
using System.Linq;

namespace tora.graph.editor
{
    public class GraphValidationIssue
    {
        public readonly string NodeId;
        public readonly string Message;

        public GraphValidationIssue(string nodeId, string message)
        {
            NodeId = nodeId;
            Message = message;
        }
    }

    /// <summary>Structural checks shared by every graph type built on GraphAsset&lt;TNode&gt;
    /// (StoryGraph today, DialogueGraph later) - see Docs/Design/GraphFramework.md#validation.
    /// "Missing assets" isn't included yet: no node type references external assets to check
    /// until an action like UnlockConversation exists.</summary>
    public static class GraphValidator<TNode> where TNode : GraphNode
    {
        public static List<GraphValidationIssue> Validate(GraphAsset<TNode> graph)
        {
            var issues = new List<GraphValidationIssue>();
            if (graph == null)
            {
                return issues;
            }

            CheckDuplicateIds(graph, issues);
            CheckMissingLinks(graph, issues);
            CheckCycles(graph, issues);
            CheckOrphans(graph, issues);

            return issues;
        }

        private static void CheckDuplicateIds(GraphAsset<TNode> graph, List<GraphValidationIssue> issues)
        {
            var seen = new HashSet<string>();
            foreach (var node in graph.Nodes)
            {
                if (string.IsNullOrEmpty(node.Id))
                {
                    issues.Add(new GraphValidationIssue(node.Id, "Node has an empty Id."));
                    continue;
                }

                if (!seen.Add(node.Id))
                {
                    issues.Add(new GraphValidationIssue(node.Id, $"Duplicate Id '{node.Id}'."));
                }
            }
        }

        private static void CheckMissingLinks(GraphAsset<TNode> graph, List<GraphValidationIssue> issues)
        {
            var ids = new HashSet<string>(graph.Nodes.Select(n => n.Id));
            foreach (var node in graph.Nodes)
            {
                foreach (var childId in node.ChildIds)
                {
                    if (!ids.Contains(childId))
                    {
                        issues.Add(new GraphValidationIssue(node.Id, $"Links to missing node Id '{childId}'."));
                    }
                }
            }
        }

        private static void CheckCycles(GraphAsset<TNode> graph, List<GraphValidationIssue> issues)
        {
            var nodesById = new Dictionary<string, TNode>();
            foreach (var node in graph.Nodes)
            {
                if (!string.IsNullOrEmpty(node.Id) && !nodesById.ContainsKey(node.Id))
                {
                    nodesById[node.Id] = node;
                }
            }

            var visiting = new HashSet<string>();
            var visited = new HashSet<string>();

            foreach (var node in graph.Nodes)
            {
                if (!visited.Contains(node.Id))
                {
                    DetectCycle(node.Id, nodesById, visiting, visited, issues);
                }
            }
        }

        private static void DetectCycle(string nodeId, Dictionary<string, TNode> nodesById, HashSet<string> visiting, HashSet<string> visited, List<GraphValidationIssue> issues)
        {
            if (!nodesById.TryGetValue(nodeId, out var node))
            {
                return;
            }

            visiting.Add(nodeId);

            foreach (var childId in node.ChildIds)
            {
                if (visiting.Contains(childId))
                {
                    issues.Add(new GraphValidationIssue(nodeId, $"Cycle detected involving '{nodeId}' -> '{childId}'."));
                }
                else if (!visited.Contains(childId))
                {
                    DetectCycle(childId, nodesById, visiting, visited, issues);
                }
            }

            visiting.Remove(nodeId);
            visited.Add(nodeId);
        }

        private static void CheckOrphans(GraphAsset<TNode> graph, List<GraphValidationIssue> issues)
        {
            var hasIncoming = new HashSet<string>();
            foreach (var node in graph.Nodes)
            {
                foreach (var childId in node.ChildIds)
                {
                    hasIncoming.Add(childId);
                }
            }

            foreach (var node in graph.Nodes)
            {
                if (!hasIncoming.Contains(node.Id) && node.ChildIds.Count == 0)
                {
                    issues.Add(new GraphValidationIssue(node.Id, "Orphan node: no incoming or outgoing links."));
                }
            }
        }
    }
}
