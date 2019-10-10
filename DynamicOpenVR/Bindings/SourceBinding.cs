namespace DynamicOpenVR.Bindings
{
    internal class SourceBinding : OVRBinding
    {
        internal string Mode { get; }
        internal string Input { get; }

        internal SourceBinding(string path, string mode, string input) : base("sources", path)
        {
            Mode = mode;
            Input = input;
        }
    }
}
