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


        public virtual void OnBeforeSerialize()
        {
            // m_GuidSerialized = m_Guid.ToString();
            // m_GroupGuidSerialized = m_GroupGuid.ToString();
            // m_SerializableSlots = SerializationHelper.Serialize<ISlot>(m_Slots);
        }

        public virtual void OnAfterDeserialize()
        {
            // if (!string.IsNullOrEmpty(m_GuidSerialized))
            //     m_Guid = new Guid(m_GuidSerialized);
            // else
            //     m_Guid = Guid.NewGuid();

            // if (m_NodeVersion != GetCompiledNodeVersion())
            // {
            //     UpgradeNodeWithVersion(m_NodeVersion, GetCompiledNodeVersion());
            //     m_NodeVersion = GetCompiledNodeVersion();
            // }

            // if (!string.IsNullOrEmpty(m_GroupGuidSerialized))
            //     m_GroupGuid = new Guid(m_GroupGuidSerialized);
            // else
            //     m_GroupGuid = Guid.Empty;

            // m_Slots = SerializationHelper.Deserialize<ISlot>(m_SerializableSlots, GraphUtil.GetLegacyTypeRemapping());
            // m_SerializableSlots = null;
            // foreach (var s in m_Slots)
            //     s.owner = this;

            UpdateNodeAfterDeserialization();
        }

        public virtual void UpdateNodeAfterDeserialization()
        {}

 // Nodes that want to have a preview area can override this and return true
        public virtual bool hasPreview
        {
            get { return false; }
        }

        public virtual PreviewMode previewMode
        {
            get { return PreviewMode.Preview2D; }
        }

        public virtual bool allowedInSubGraph
        {
            get { return true; }
        }

        public virtual bool allowedInMainGraph
        {
            get { return true; }
        }

        public virtual bool allowedInLayerGraph
        {
            get { return true; }
        }

        [NonSerialized]
        bool m_HasError;
        public virtual bool hasError
        {
            get { return m_HasError; }
            protected set { m_HasError = value; }
        }

        OnNodeModified m_OnModified;
        public void Dirty(ModificationScope scope)
        {
            if (m_OnModified != null)
                m_OnModified(this, scope);
        }

        [NonSerialized]
        private List<ISlot> m_Slots = new List<ISlot>();
       public void GetSlots<T>(List<T> foundSlots) where T : ISlot
        {
            foreach (var slot in m_Slots)
            {
                if (slot is T)
                    foundSlots.Add((T)slot);
            }
        }

    }
}