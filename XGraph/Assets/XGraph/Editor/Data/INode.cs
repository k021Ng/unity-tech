using System;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;

namespace XGraph
{
    public enum ModificationScope
    {
        Nothing = 0,
        Node = 1,
        Graph = 2,
        Topological = 3,
        Layout = 4
    }

    delegate void OnNodeModified(BaseNode node, ModificationScope scope);

    static class NodeExtensions
    {
        public static IEnumerable<T> GetSlots<T>(this BaseNode node) where T : ISlot
        {
            var slots = new List<T>();
            node.GetSlots(slots);
            return slots;
        }

        public static IEnumerable<T> GetInputSlots<T>(this BaseNode node) where T : ISlot
        {
            var slots = new List<T>();
            node.GetInputSlots(slots);
            return slots;
        }

        public static IEnumerable<T> GetOutputSlots<T>(this BaseNode node) where T : ISlot
        {
            var slots = new List<T>();
            node.GetOutputSlots(slots);
            return slots;
        }
    }
}
