using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using System.Linq;
using System.Text.RegularExpressions;

namespace XGraph
{
    public class XGraphUtil
    {
        [MenuItem("Tools/XGraph", false, 208)]
        public static void CreateUnlitMasterMaterialGraph()
        {
            XGraphUtil.CreateNewGraph(new UnlitMasterNode());
        }

        public static void CreateNewGraph(BaseNode node)
        {
            var graphItem = ScriptableObject.CreateInstance<NewGraphAction>();
            graphItem.node = node;
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, graphItem,
                string.Format("New Shader Graph.{0}", XGraphImporter.Extension), null, null);
        }

        static Dictionary<SerializationHelper.TypeSerializationInfo, SerializationHelper.TypeSerializationInfo> s_LegacyTypeRemapping;
        public static Dictionary<SerializationHelper.TypeSerializationInfo, SerializationHelper.TypeSerializationInfo> GetLegacyTypeRemapping()
        {
            if (s_LegacyTypeRemapping == null)
            {
                s_LegacyTypeRemapping = new Dictionary<SerializationHelper.TypeSerializationInfo, SerializationHelper.TypeSerializationInfo>();
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in assembly.GetTypesOrNothing())
                    {
                        if (type.IsAbstract)
                            continue;
                        foreach (var attribute in type.GetCustomAttributes(typeof(FormerNameAttribute), false))
                        {
                            var legacyAttribute = (FormerNameAttribute)attribute;
                            var serializationInfo = new SerializationHelper.TypeSerializationInfo { fullName = legacyAttribute.fullName };
                            s_LegacyTypeRemapping[serializationInfo] = SerializationHelper.GetTypeSerializableAsString(type);
                        }
                    }
                }
            }

            return s_LegacyTypeRemapping;
        }
    }

    class NewGraphAction : EndNameEditAction
    {
        BaseNode m_Node;
        public BaseNode node
        {
            get { return m_Node; }
            set { m_Node = value; }
        }

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var graph = new GraphData();
            graph.AddNode(node);
            graph.path = "Shader Graphs";
            FileUtilities.WriteShaderGraphToDisk(pathName, graph);
            AssetDatabase.Refresh();

            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<Shader>(pathName);
            Selection.activeObject = obj;
        }
    }
}
