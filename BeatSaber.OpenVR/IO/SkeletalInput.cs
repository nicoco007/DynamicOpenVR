namespace BeatSaber.OpenVR.IO
{
	public class SkeletalInput : OVRAction
	{
		public string Skeleton { get; }

		public SkeletalInput(string name, string skeleton, OVRActionRequirement requirement = OVRActionRequirement.Suggested) : base(name, requirement, "skeleton", "in")
		{
			Skeleton = skeleton;
		}

		public SkeletalSummaryData GetSummaryData(EVRSummaryType summaryType = EVRSummaryType.FromDevice)
		{
			return new SkeletalSummaryData(OpenVRApi.GetSkeletalSummaryData(Handle, summaryType));
		}
	}
}
