namespace DynamicOpenVR.Bindings
{
    public abstract class OVRBinding
    {
        internal string Type { get; }
        internal string Path { get; }

        protected OVRBinding(string type, string path)
        {
            Type = type;
            Path = path;
        }
    }
}
