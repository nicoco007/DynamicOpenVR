namespace DynamicOpenVR.IO
{
    public abstract class Input : OVRAction
    {
        protected Input(string name, OVRActionRequirement requirement, string type) : base(name, requirement, type, "in") { }
    }
}
