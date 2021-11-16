using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing.Colors;
using UnityEditor.ShaderGraph.Internal;

namespace XGraph
{
    [Serializable]
    public abstract class BaseNode : ISerializationCallbackReceiver, IGroupItem
    {
        [NonSerialized]
        Guid m_GroupGuid;

        public Identifier tempId { get; set; }
        public GraphData owner { get; set; }

        [NonSerialized]
        private Guid m_Guid;

        public Guid guid
        {
            get { return m_Guid; }
        }

        public Guid groupGuid
        {
            get { return m_GroupGuid; }
            set { m_GroupGuid = value; }
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
        }
    }
}