using System;

namespace Spirit604.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute
#if !ODIN_INSPECTOR
        : AttributeBase
#else
        : Sirenix.OdinInspector.ButtonAttribute
#endif
    {
#if !ODIN_INSPECTOR
        public string Label { get; private set; }

        public ButtonAttribute(string label)
        {
            Label = label;
        }

        public ButtonAttribute()
        {
            Label = string.Empty;
        }
#else
        public ButtonAttribute() : base() { }
        public ButtonAttribute(string label) : base(label) { }
#endif
    }
}