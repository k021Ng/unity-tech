using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace XGraph
{
    public class GraphData : ISerializationCallbackReceiver
    {
        
        [NonSerialized]
        Dictionary<Guid, List<IGroupItem>> m_GroupItems = new Dictionary<Guid, List<IGroupItem>>();

        public GraphData()
        {
            m_GroupItems[Guid.Empty] = new List<IGroupItem>();
        }

        public void OnBeforeSerialize()
        {
            // m_SerializableNodes = SerializationHelper.Serialize(GetNodes<AbstractMaterialNode>());
            // m_SerializableEdges = SerializationHelper.Serialize<IEdge>(m_Edges);
            // m_SerializedProperties = SerializationHelper.Serialize<AbstractShaderProperty>(m_Properties);
            // m_SerializedKeywords = SerializationHelper.Serialize<ShaderKeyword>(m_Keywords);
            // m_ActiveOutputNodeGuidSerialized = m_ActiveOutputNodeGuid == Guid.Empty ? null : m_ActiveOutputNodeGuid.ToString();
        }

        public void OnAfterDeserialize()
        {
            // have to deserialize 'globals' before nodes
            // m_Properties = SerializationHelper.Deserialize<AbstractShaderProperty>(m_SerializedProperties, GraphUtil.GetLegacyTypeRemapping());
            // m_Keywords = SerializationHelper.Deserialize<ShaderKeyword>(m_SerializedKeywords, GraphUtil.GetLegacyTypeRemapping());

            // var nodes = SerializationHelper.Deserialize<AbstractMaterialNode>(m_SerializableNodes, GraphUtil.GetLegacyTypeRemapping());

            // m_Nodes = new List<AbstractMaterialNode>(nodes.Count);
            // m_NodeDictionary = new Dictionary<Guid, AbstractMaterialNode>(nodes.Count);

            // foreach (var group in m_Groups)
            // {
            //     m_GroupItems.Add(group.guid, new List<IGroupItem>());
            // }

            // foreach (var node in nodes)
            // {
            //     node.owner = this;
            //     node.UpdateNodeAfterDeserialization();
            //     node.tempId = new Identifier(m_Nodes.Count);
            //     m_Nodes.Add(node);
            //     m_NodeDictionary.Add(node.guid, node);
            //     m_GroupItems[node.groupGuid].Add(node);
            // }

            // foreach (var stickyNote in m_StickyNotes)
            // {
            //     m_GroupItems[stickyNote.groupGuid].Add(stickyNote);
            // }

            // m_SerializableNodes = null;

            // m_Edges = SerializationHelper.Deserialize<IEdge>(m_SerializableEdges, GraphUtil.GetLegacyTypeRemapping());
            // m_SerializableEdges = null;
            // foreach (var edge in m_Edges)
            //     AddEdgeToNodeEdges(edge);

            // m_OutputNode = null;

            // if (!isSubGraph)
            // {
            //     if (string.IsNullOrEmpty(m_ActiveOutputNodeGuidSerialized))
            //     {
            //         var node = (AbstractMaterialNode)GetNodes<IMasterNode>().FirstOrDefault();
            //         if (node != null)
            //         {
            //             m_ActiveOutputNodeGuid = node.guid;
            //         }
            //     }
            //     else
            //     {
            //         m_ActiveOutputNodeGuid = new Guid(m_ActiveOutputNodeGuidSerialized);
            //     }
            // }
        }

        public void ClearChanges()
        {
            // m_AddedNodes.Clear();
            // m_RemovedNodes.Clear();
            // m_PastedNodes.Clear();
            // m_ParentGroupChanges.Clear();
            // m_AddedGroups.Clear();
            // m_RemovedGroups.Clear();
            // m_PastedGroups.Clear();
            // m_AddedEdges.Clear();
            // m_RemovedEdges.Clear();
            // m_AddedInputs.Clear();
            // m_RemovedInputs.Clear();
            // m_MovedInputs.Clear();
            // m_AddedStickyNotes.Clear();
            // m_RemovedNotes.Clear();
            // m_PastedStickyNotes.Clear();
            // m_MostRecentlyCreatedGroup = null;
            // didActiveOutputNodeChange = false;
        }

        public void AddNode(BaseNode node)
        {
            // check

            AddNodeNoValidate(node);

            // If adding a Sub Graph node whose asset contains Keywords
            // Need to restest Keywords against the variant limit
            // if(node is SubGraphNode subGraphNode &&
            //     subGraphNode.asset != null && 
            //     subGraphNode.asset.keywords.Count > 0)
            // {
            //     OnKeywordChangedNoValidate();
            // }

            ValidateGraph();
        }

        [NonSerialized]
        Stack<Identifier> m_FreeNodeTempIds = new Stack<Identifier>();
                [NonSerialized]
        List<BaseNode> m_Nodes = new List<BaseNode>();
        [NonSerialized]
        Dictionary<Guid, BaseNode> m_NodeDictionary = new Dictionary<Guid, BaseNode>();
                [NonSerialized]
        List<BaseNode> m_AddedNodes = new List<BaseNode>();

        private void AddNodeNoValidate(BaseNode node)
        {
            if (node.groupGuid != Guid.Empty && !m_GroupItems.ContainsKey(node.groupGuid))
            {
                throw new InvalidOperationException("Cannot add a node whose group doesn't exist.");
            }
            node.owner = this;
            if (m_FreeNodeTempIds.Any())
            {
                var id = m_FreeNodeTempIds.Pop();
                id.IncrementVersion();
                node.tempId = id;
                m_Nodes[id.index] = node;
            }
            else
            {
                var id = new Identifier(m_Nodes.Count);
                node.tempId = id;
                m_Nodes.Add(node);
            }
            m_NodeDictionary.Add(node.guid, node);
            m_AddedNodes.Add(node);
            m_GroupItems[node.groupGuid].Add(node);
        }

          public void ValidateGraph()
        {
            // var propertyNodes = GetNodes<PropertyNode>().Where(n => !m_Properties.Any(p => p.guid == n.propertyGuid)).ToArray();
            // foreach (var pNode in propertyNodes)
            //     ReplacePropertyNodeWithConcreteNodeNoValidate(pNode);

            // messageManager?.ClearAllFromProvider(this);
            // //First validate edges, remove any
            // //orphans. This can happen if a user
            // //manually modifies serialized data
            // //of if they delete a node in the inspector
            // //debug view.
            // foreach (var edge in edges.ToArray())
            // {
            //     var outputNode = GetNodeFromGuid(edge.outputSlot.nodeGuid);
            //     var inputNode = GetNodeFromGuid(edge.inputSlot.nodeGuid);

            //     MaterialSlot outputSlot = null;
            //     MaterialSlot inputSlot = null;
            //     if (outputNode != null && inputNode != null)
            //     {
            //         outputSlot = outputNode.FindOutputSlot<MaterialSlot>(edge.outputSlot.slotId);
            //         inputSlot = inputNode.FindInputSlot<MaterialSlot>(edge.inputSlot.slotId);
            //     }

            //     if (outputNode == null
            //         || inputNode == null
            //         || outputSlot == null
            //         || inputSlot == null)
            //     {
            //         //orphaned edge
            //         RemoveEdgeNoValidate(edge);
            //     }
            // }

            // var temporaryMarks = IndexSetPool.Get();
            // var permanentMarks = IndexSetPool.Get();
            // var slots = ListPool<MaterialSlot>.Get();

            // // Make sure we process a node's children before the node itself.
            // var stack = StackPool<AbstractMaterialNode>.Get();
            // foreach (var node in GetNodes<AbstractMaterialNode>())
            // {
            //     stack.Push(node);
            // }
            // while (stack.Count > 0)
            // {
            //     var node = stack.Pop();
            //     if (permanentMarks.Contains(node.tempId.index))
            //     {
            //         continue;
            //     }

            //     if (temporaryMarks.Contains(node.tempId.index))
            //     {
            //         node.ValidateNode();
            //         permanentMarks.Add(node.tempId.index);
            //     }
            //     else
            //     {
            //         temporaryMarks.Add(node.tempId.index);
            //         stack.Push(node);
            //         node.GetInputSlots(slots);
            //         foreach (var inputSlot in slots)
            //         {
            //             var nodeEdges = GetEdges(inputSlot.slotReference);
            //             foreach (var edge in nodeEdges)
            //             {
            //                 var fromSocketRef = edge.outputSlot;
            //                 var childNode = GetNodeFromGuid(fromSocketRef.nodeGuid);
            //                 if (childNode != null)
            //                 {
            //                     stack.Push(childNode);
            //                 }
            //             }
            //         }
            //         slots.Clear();
            //     }
            // }

            // StackPool<AbstractMaterialNode>.Release(stack);
            // ListPool<MaterialSlot>.Release(slots);
            // IndexSetPool.Release(temporaryMarks);
            // IndexSetPool.Release(permanentMarks);

            // foreach (var edge in m_AddedEdges.ToList())
            // {
            //     if (!ContainsNodeGuid(edge.outputSlot.nodeGuid) || !ContainsNodeGuid(edge.inputSlot.nodeGuid))
            //     {
            //         Debug.LogWarningFormat("Added edge is invalid: {0} -> {1}\n{2}", edge.outputSlot.nodeGuid, edge.inputSlot.nodeGuid, Environment.StackTrace);
            //         m_AddedEdges.Remove(edge);
            //     }
            // }

            // foreach (var groupChange in m_ParentGroupChanges.ToList())
            // {
            //     if (groupChange.groupItem is AbstractMaterialNode node && !ContainsNodeGuid(node.guid))
            //     {
            //         m_ParentGroupChanges.Remove(groupChange);
            //     }

            //     if (groupChange.groupItem is StickyNoteData stickyNote && !m_StickyNotes.Contains(stickyNote))
            //     {
            //         m_ParentGroupChanges.Remove(groupChange);
            //     }
            // }
        }
    }
}