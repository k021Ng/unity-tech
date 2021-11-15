using System;
using System.Collections.Generic;
using UnityEngine;

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

        public void AddNode(AbstractMaterialNode node)
        {
            if (node is AbstractMaterialNode materialNode)
            {
                if (isSubGraph && !materialNode.allowedInSubGraph)
                {
                    Debug.LogWarningFormat("Attempting to add {0} to Sub Graph. This is not allowed.", materialNode.GetType());
                    return;
                }

                AddNodeNoValidate(materialNode);

                // If adding a Sub Graph node whose asset contains Keywords
                // Need to restest Keywords against the variant limit
                if(node is SubGraphNode subGraphNode &&
                    subGraphNode.asset != null && 
                    subGraphNode.asset.keywords.Count > 0)
                {
                    OnKeywordChangedNoValidate();
                }

                ValidateGraph();
            }
            else
            {
                Debug.LogWarningFormat("Trying to add node {0} to Material graph, but it is not a {1}", node, typeof(AbstractMaterialNode));
            }
        }
    }
}