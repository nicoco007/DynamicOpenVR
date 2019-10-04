namespace BeatSaber.OpenVR.IO
{
	public class HapticVibrationOutput : OVRAction
	{
		public HapticVibrationOutput(string name, OVRActionRequirement requirement = OVRActionRequirement.Suggested) : base(name, requirement, "vibration", "out") { }

		public void TriggerHapticVibration(float durationSeconds, float amplitude, float frequency = 150f)
		{
			OpenVRApi.TriggerHapticVibrationAction(Handle, 0, durationSeconds, frequency, amplitude);
		}
	}
}
