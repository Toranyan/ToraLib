using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace tora.graph.editor
{
    public class GraphNodeView<TNode> : Node where TNode : GraphNode
    {
        public TNode Data { get; }
        public Port InputPort { get; }
        public Port OutputPort { get; }

        public Action<GraphNodeView<TNode>> Selected;
        public Action<GraphNodeView<TNode>> Unselected;
        public Action<GraphNodeView<TNode>> Moved;

        public GraphNodeView(TNode data, string title)
        {
            Data = data;
            this.title = title;
            viewDataKey = data.Id;

            InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            InputPort.portName = "In";
            inputContainer.Add(InputPort);

            OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            OutputPort.portName = "Out";
            outputContainer.Add(OutputPort);

            RefreshExpandedState();
            RefreshPorts();

            SetPosition(new Rect(data.Position, new Vector2(160, 120)));
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Data.Position = newPos.position;
            Moved?.Invoke(this);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            Selected?.Invoke(this);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            Unselected?.Invoke(this);
        }
    }
}
