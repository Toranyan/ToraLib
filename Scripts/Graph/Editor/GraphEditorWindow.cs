using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace tora.graph.editor
{
    /// <summary>MVP graph editor: create/delete nodes, drag connections, default inspector,
    /// save. Pan/zoom is free via GraphView. See Docs/Design/GraphFramework.md.</summary>
    public abstract class GraphEditorWindow<TGraph, TNode> : EditorWindow
        where TGraph : GraphAsset<TNode>
        where TNode : GraphNode
    {
        protected TGraph Graph { get; private set; }

        private SerializedObject _serializedGraph;
        private GraphView _graphView;
        private VisualElement _inspectorContainer;
        private readonly Dictionary<string, GraphNodeView<TNode>> _nodeViews = new Dictionary<string, GraphNodeView<TNode>>();
        private TNode _selectedNode;

        protected abstract string GetNodeTitle(TNode node);

        /// <summary>Field name -> base interface/abstract type, for [SerializeReference] list
        /// fields on TNode that need an explicit "pick a type then add" control (see
        /// BuildSerializeReferenceListField). Unity's built-in Add-button type picker for
        /// SerializeReference lists is not reliably discoverable through the default list
        /// drawer in practice, so fields listed here get a custom dropdown + Add/Remove UI
        /// instead of a plain PropertyField.</summary>
        protected virtual Dictionary<string, Type> GetSerializeReferenceListFields()
        {
            return new Dictionary<string, Type>();
        }

        public void Load(TGraph graph)
        {
            Graph = graph;
            _serializedGraph = new SerializedObject(graph);
            titleContent = new GUIContent(graph.name);

            rootVisualElement.Clear();
            BuildLayout();
            BuildGraphView();
        }

        private void BuildLayout()
        {
            var toolbar = new Toolbar();
            toolbar.Add(new ToolbarButton(Save) { text = "Save" });
            toolbar.Add(new ToolbarButton(Validate) { text = "Validate" });
            rootVisualElement.Add(toolbar);

            var body = new VisualElement { style = { flexDirection = FlexDirection.Row, flexGrow = 1 } };
            rootVisualElement.Add(body);

            _graphView = new ConcreteGraphView { style = { flexGrow = 1 } };
            _graphView.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            _graphView.AddManipulator(new ContentDragger());
            _graphView.AddManipulator(new SelectionDragger());
            _graphView.AddManipulator(new RectangleSelector());
            _graphView.AddManipulator(new ContextualMenuManipulator(BuildContextMenu));
            _graphView.graphViewChanged = OnGraphViewChanged;

            var grid = new GridBackground();
            grid.StretchToParentSize();
            _graphView.Insert(0, grid);

            body.Add(_graphView);

            var inspectorScroll = new ScrollView { style = { width = 320, borderLeftWidth = 1 } };
            _inspectorContainer = new VisualElement { style = { paddingLeft = 4, paddingRight = 4 } };
            inspectorScroll.Add(_inspectorContainer);
            body.Add(inspectorScroll);

            RefreshInspector();
        }

        private void BuildGraphView()
        {
            _nodeViews.Clear();

            foreach (var node in Graph.Nodes)
            {
                AddNodeView(node);
            }

            foreach (var node in Graph.Nodes)
            {
                if (!_nodeViews.TryGetValue(node.Id, out var outputView))
                {
                    continue;
                }

                foreach (var childId in node.ChildIds)
                {
                    if (_nodeViews.TryGetValue(childId, out var inputView))
                    {
                        _graphView.AddElement(outputView.OutputPort.ConnectTo(inputView.InputPort));
                    }
                }
            }
        }

        private void AddNodeView(TNode node)
        {
            var view = new GraphNodeView<TNode>(node, GetNodeTitle(node))
            {
                Selected = v =>
                {
                    _selectedNode = v.Data;
                    RefreshInspector();
                },
                Unselected = v =>
                {
                    if (_selectedNode == v.Data)
                    {
                        _selectedNode = null;
                        RefreshInspector();
                    }
                },
                Moved = _ => MarkDirty()
            };

            _nodeViews[node.Id] = view;
            _graphView.AddElement(view);
        }

        /// <summary>Every concrete node type creatable in this graph: TNode itself, if it's
        /// concrete (StoryGraph's case - exactly one node kind), plus every non-abstract type
        /// deriving from it (DialogueGraph's case - TNode is abstract, one entry per kind).</summary>
        private List<Type> GetCreatableNodeTypes()
        {
            var types = new List<Type>();

            if (!typeof(TNode).IsAbstract)
            {
                types.Add(typeof(TNode));
            }

            types.AddRange(TypeCache.GetTypesDerivedFrom<TNode>().Where(t => !t.IsAbstract));

            return types.OrderBy(t => t.Name).ToList();
        }

        private void BuildContextMenu(ContextualMenuPopulateEvent evt)
        {
            var position = _graphView.contentViewContainer.WorldToLocal(evt.mousePosition);
            var creatableTypes = GetCreatableNodeTypes();

            if (creatableTypes.Count == 1)
            {
                var onlyType = creatableTypes[0];
                evt.menu.AppendAction("Create Node", _ => CreateNode(onlyType, position));
                return;
            }

            foreach (var nodeType in creatableTypes)
            {
                var capturedType = nodeType;
                evt.menu.AppendAction($"Create Node/{ObjectNames.NicifyVariableName(nodeType.Name)}", _ => CreateNode(capturedType, position));
            }
        }

        private void CreateNode(Type nodeType, Vector2 position)
        {
            var node = (TNode)Activator.CreateInstance(nodeType);
            node.Id = GenerateUniqueId();
            node.Position = position;

            Graph.Nodes.Add(node);
            AddNodeView(node);
            MarkDirty();
        }

        private string GenerateUniqueId()
        {
            var existing = new HashSet<string>(Graph.Nodes.Select(n => n.Id));
            var index = existing.Count + 1;
            string id;
            do
            {
                id = $"node_{index++}";
            } while (existing.Contains(id));
            return id;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            if (change.edgesToCreate != null)
            {
                foreach (var edge in change.edgesToCreate)
                {
                    if (edge.output.node is GraphNodeView<TNode> outputNode &&
                        edge.input.node is GraphNodeView<TNode> inputNode &&
                        !outputNode.Data.ChildIds.Contains(inputNode.Data.Id))
                    {
                        outputNode.Data.ChildIds.Add(inputNode.Data.Id);
                    }
                }
            }

            if (change.elementsToRemove != null)
            {
                foreach (var element in change.elementsToRemove)
                {
                    if (element is Edge edge &&
                        edge.output.node is GraphNodeView<TNode> edgeOutputNode &&
                        edge.input.node is GraphNodeView<TNode> edgeInputNode)
                    {
                        edgeOutputNode.Data.ChildIds.Remove(edgeInputNode.Data.Id);
                    }
                    else if (element is GraphNodeView<TNode> nodeView)
                    {
                        Graph.Nodes.Remove(nodeView.Data);
                        _nodeViews.Remove(nodeView.Data.Id);
                        if (_selectedNode == nodeView.Data)
                        {
                            _selectedNode = null;
                            RefreshInspector();
                        }
                    }
                }
            }

            MarkDirty();
            return change;
        }

        /// <summary>Rebuilds the inspector as native UI Toolkit PropertyFields bound to the
        /// SerializedObject, rather than IMGUI. This matters specifically for the Conditions/
        /// Actions lists: their [SerializeReference] "pick a concrete type" dropdown on the
        /// list's Add button only renders reliably through the bound PropertyField/list-view
        /// path, not through manual IMGUI property iteration in a narrow container.</summary>
        private void RefreshInspector()
        {
            _inspectorContainer.Clear();

            if (_serializedGraph == null || _selectedNode == null)
            {
                _inspectorContainer.Add(new Label("Select a node to edit its properties."));
                return;
            }

            var index = Graph.Nodes.IndexOf(_selectedNode);
            if (index < 0)
            {
                return;
            }

            _serializedGraph.Update();

            var serializeReferenceListFields = GetSerializeReferenceListFields();

            var nodeProperty = _serializedGraph.FindProperty("Nodes").GetArrayElementAtIndex(index);
            var end = nodeProperty.GetEndProperty();
            var iterator = nodeProperty.Copy();
            var enterChildren = true;
            var plainFields = new List<PropertyField>();

            while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, end))
            {
                if (serializeReferenceListFields.TryGetValue(iterator.name, out var baseType))
                {
                    _inspectorContainer.Add(BuildSerializeReferenceListField(iterator.Copy(), baseType));
                }
                else
                {
                    var field = new PropertyField(iterator.Copy());
                    plainFields.Add(field);
                    _inspectorContainer.Add(field);
                }

                enterChildren = false;
            }

            // Only the plain fields go through the standard bind path - the custom
            // SerializeReference list fields above apply their own edits directly.
            foreach (var field in plainFields)
            {
                field.Bind(_serializedGraph);
            }
        }

        /// <summary>Custom "type dropdown + Add" / per-element "Remove" UI for a
        /// [SerializeReference] List&lt;T&gt; field, replacing Unity's built-in list Add button
        /// (whose type picker isn't reliably discoverable) with an explicit, always-visible one.</summary>
        private VisualElement BuildSerializeReferenceListField(SerializedProperty listProperty, Type baseType)
        {
            var root = new VisualElement();
            var foldout = new Foldout { text = listProperty.displayName, value = true };
            root.Add(foldout);

            var elementsContainer = new VisualElement();
            foldout.Add(elementsContainer);

            var concreteTypes = TypeCache.GetTypesDerivedFrom(baseType)
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .OrderBy(t => t.Name)
                .ToList();

            var typeNames = concreteTypes.Select(t => t.Name).ToList();
            var typeDropdown = new DropdownField(typeNames, typeNames.Count > 0 ? 0 : -1);

            void Rebuild()
            {
                elementsContainer.Clear();

                for (var i = 0; i < listProperty.arraySize; i++)
                {
                    var index = i;
                    var elementProperty = listProperty.GetArrayElementAtIndex(index);

                    var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };
                    var elementField = new PropertyField(elementProperty) { style = { flexGrow = 1 } };
                    elementField.Bind(listProperty.serializedObject);

                    var removeButton = new Button(() =>
                    {
                        listProperty.DeleteArrayElementAtIndex(index);
                        listProperty.serializedObject.ApplyModifiedProperties();
                        MarkDirty();
                        Rebuild();
                    })
                    { text = "-" };

                    row.Add(elementField);
                    row.Add(removeButton);
                    elementsContainer.Add(row);
                }
            }

            var addRow = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            var addButton = new Button(() =>
            {
                if (typeDropdown.index < 0)
                {
                    return;
                }

                var selectedType = concreteTypes[typeDropdown.index];
                listProperty.arraySize++;
                var newElement = listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1);
                newElement.managedReferenceValue = Activator.CreateInstance(selectedType);
                listProperty.serializedObject.ApplyModifiedProperties();
                MarkDirty();
                Rebuild();
            })
            { text = "+" };

            addRow.Add(typeDropdown);
            addRow.Add(addButton);
            root.Add(addRow);

            Rebuild();
            return root;
        }

        private void MarkDirty()
        {
            if (Graph != null)
            {
                EditorUtility.SetDirty(Graph);
            }
        }

        private void Save()
        {
            MarkDirty();
            AssetDatabase.SaveAssets();
        }

        private void Validate()
        {
            var issues = GraphValidator<TNode>.Validate(Graph);

            if (issues.Count == 0)
            {
                Debug.Log($"[{Graph.name}] Validation passed - no issues found.");
                return;
            }

            foreach (var issue in issues)
            {
                Debug.LogWarning($"[{Graph.name}] Node '{issue.NodeId}': {issue.Message}");
            }
        }

        /// <summary>GraphView itself is abstract - also overrides GetCompatiblePorts, since the
        /// default implementation relies on registered NodeAdapter conversions we never set up
        /// and rejects every drop even when both ports carry the same placeholder port type.</summary>
        private sealed class ConcreteGraphView : GraphView
        {
            public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
            {
                var compatible = new List<Port>();
                ports.ForEach(port =>
                {
                    if (port.direction != startPort.direction && port.node != startPort.node)
                    {
                        compatible.Add(port);
                    }
                });
                return compatible;
            }
        }
    }
}
