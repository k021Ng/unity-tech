using System;
using UnityEditor.ShaderGraph;

namespace XGraph
{
    public interface ISlot : IEquatable<ISlot>
    {
        int id { get; }
        string displayName { get; set; }
        bool isInputSlot { get; }
        bool isOutputSlot { get; }
        int priority { get; set; }
        SlotReference slotReference { get; }
        BaseNode owner { get; set; }
        bool hidden { get; set; }
    }
}
