namespace DynamicOpenVR.IO
{
    public abstract class AnalogInput : Input
    {
        protected AnalogInput(string name, OVRActionRequirement requirement, string type) : base(name, requirement, type) { }

        /// <summary>
        /// Is set to True if this action is bound to an input source that is present in the system and is in an action set that is active.
        /// </summary>
        public override bool IsActive()
        {
            return GetActionData().bActive;
        }

        protected InputAnalogActionData_t GetActionData()
        {
            return OpenVRApi.GetAnalogActionData(Handle);
        }
    }
}
