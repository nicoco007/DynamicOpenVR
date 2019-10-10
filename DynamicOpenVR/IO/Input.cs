namespace DynamicOpenVR.IO
{
    public abstract class Input : OVRAction
    {
        protected Input(string name, OVRActionRequirement requirement, string type) : base(name, requirement, type, "in") { }

        /// <summary>
        /// Is set to True if this action is bound to an input source that is present in the system and is in an action set that is active.
        /// </summary>
        public abstract bool IsActive();
    }
}
