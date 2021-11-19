using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor;

namespace XGraph
{
    [ScriptedImporter(31, Extension, 3)]
    public class XGraphImporter : ScriptedImporter
    {
        public const string Extension = "xgh";

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var oldShader = AssetDatabase.LoadAssetAtPath<Shader>(ctx.assetPath);
            if (oldShader != null)
                ShaderUtil.ClearShaderMessages(oldShader);

            List<PropertyCollector.TextureInfo> configuredTextures;
            string path = ctx.assetPath;
            var sourceAssetDependencyPaths = new List<string>();

            UnityEngine.Object mainObject;

            var textGraph = File.ReadAllText(path, Encoding.UTF8);
            GraphData graph = JsonUtility.FromJson<GraphData>(textGraph);
            graph.messageManager = new MessageManager();
            graph.assetGuid = AssetDatabase.AssetPathToGUID(path);
            graph.OnEnable();
            graph.ValidateGraph();

            if (graph.outputNode is VfxMasterNode vfxMasterNode)
            {
                var vfxAsset = GenerateVfxShaderGraphAsset(vfxMasterNode);
                
                mainObject = vfxAsset;
            }
            else
            {
                var text = GetShaderText(path, out configuredTextures, sourceAssetDependencyPaths,graph);
                var shader = ShaderUtil.CreateShaderAsset(text, false);

                if (graph != null && graph.messageManager.nodeMessagesChanged)
                {
                    foreach (var pair in graph.messageManager.GetNodeMessages())
                    {
                        var node = graph.GetNodeFromTempId(pair.Key);
                        MessageManager.Log(node, path, pair.Value.First(), shader);
                    }
                }

                EditorMaterialUtility.SetShaderDefaults(
                    shader,
                    configuredTextures.Where(x => x.modifiable).Select(x => x.name).ToArray(),
                    configuredTextures.Where(x => x.modifiable).Select(x => EditorUtility.InstanceIDToObject(x.textureId) as Texture).ToArray());
                EditorMaterialUtility.SetShaderNonModifiableDefaults(
                    shader,
                    configuredTextures.Where(x => !x.modifiable).Select(x => x.name).ToArray(),
                    configuredTextures.Where(x => !x.modifiable).Select(x => EditorUtility.InstanceIDToObject(x.textureId) as Texture).ToArray());

                mainObject = shader;
            }
            Texture2D texture = Resources.Load<Texture2D>("Icons/sg_graph_icon@64");
            ctx.AddObjectToAsset("MainAsset", mainObject, texture);
            ctx.SetMainObject(mainObject);

            var metadata = ScriptableObject.CreateInstance<ShaderGraphMetadata>();
            metadata.hideFlags = HideFlags.HideInHierarchy;
            if (graph != null)
            {
                metadata.outputNodeTypeName = graph.outputNode.GetType().FullName;
            }
            ctx.AddObjectToAsset("Metadata", metadata);

            foreach (var sourceAssetDependencyPath in sourceAssetDependencyPaths.Distinct())
            {
                // Ensure that dependency path is relative to project
                if (!sourceAssetDependencyPath.StartsWith("Packages/") && !sourceAssetDependencyPath.StartsWith("Assets/"))
                {
                    Debug.LogWarning($"Invalid dependency path: {sourceAssetDependencyPath}", mainObject);
                    continue;
                }

                ctx.DependsOnSourceAsset(sourceAssetDependencyPath);
            }
        }
    }
}