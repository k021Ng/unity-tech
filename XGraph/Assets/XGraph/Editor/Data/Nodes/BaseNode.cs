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
    abstract class BaseNode : ISerializationCallbackReceiver, IGroupItem
    {
        [NonSerialized]
        Guid m_GroupGuid;

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