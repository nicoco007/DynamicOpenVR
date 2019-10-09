namespace DynamicOpenVR.IO
{
	public class VectorInput : Input
	{
		public VectorInput(string name, OVRActionRequirement requirement = OVRActionRequirement.Suggested) : base(name, requirement, "vector1") { }

		public float GetAxisRaw()
		{
			return OpenVRApi.GetAnalogActionData(Handle).x;
		}
	}
}
