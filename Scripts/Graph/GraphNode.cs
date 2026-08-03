using System;
using System.Collections.Generic;
using UnityEngine;

namespace tora.graph
{
    [Serializable]
    public abstract class GraphNode
    {
        public string Id;
        public Vector2 Position;
        public List<string> ChildIds = new List<string>();
    }
}
