using BeatSaber.OpenVR.Model;
using Newtonsoft.Json;

namespace BeatSaber.OpenVR.IO
{
	public class SkeletalInput : OVRAction
	{
		[JsonProperty(PropertyName = "skeleton", Order = 3)] public string Skeleton;

		public SkeletalInput(string name, string skeleton, OVRActionRequirement requirement = OVRActionRequirement.Suggested) : base(name, requirement, "skeleton", "in")
		{
			this.Skeleton = skeleton;
		}

		public SkeletalSummaryData GetSummaryData(EVRSummaryType summaryType = EVRSummaryType.FromDevice)
		{
			return new SkeletalSummaryData(OpenVRApi.GetSkeletalSummaryData(Handle, summaryType));
		}
	}
}
