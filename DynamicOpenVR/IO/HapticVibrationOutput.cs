namespace DynamicOpenVR.IO
{
	public class HapticVibrationOutput : OVRAction
	{
		public HapticVibrationOutput(string name, OVRActionRequirement requirement = OVRActionRequirement.Suggested) : base(name, requirement, "vibration", "out") { }

        /// <summary>
        /// Triggers a haptic vibration action.
        /// </summary>
        /// <param name="durationSeconds">How long to trigger the haptic event for.</param>
        /// <param name="magnitude">The magnitude of the haptic event. This value must be between 0.0 and 1.0.</param>
        /// <param name="frequency">The frequency in cycles per second of the haptic event.</param>
		public void TriggerHapticVibration(float durationSeconds, float magnitude, float frequency = 150f)
		{
			OpenVRApi.TriggerHapticVibrationAction(Handle, 0, durationSeconds, frequency, magnitude);
		}
	}
}
