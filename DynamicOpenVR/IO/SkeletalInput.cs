namespace DynamicOpenVR.IO
{
	public class SkeletalInput : Input
	{
		public string Skeleton { get; }

		public SkeletalInput(string name, string skeleton, OVRActionRequirement requirement = OVRActionRequirement.Suggested) : base(name, requirement, "skeleton")
		{
			Skeleton = skeleton;
		}

        /// <summary>
        /// Is set to True if this action is bound to an input source that is present in the system and is in an action set that is active.
        /// </summary>
        public override bool IsActive()
        {
            return GetActionData().bActive;
        }

        /// <summary>
        /// Retrieves the summary data of the skeleton (finger curl and splay).
        /// </summary>
		public SkeletalSummaryData GetSummaryData(EVRSummaryType summaryType = EVRSummaryType.FromDevice)
		{
			return new SkeletalSummaryData(OpenVRApi.GetSkeletalSummaryData(Handle, summaryType));
		}

        private InputSkeletalActionData_t GetActionData()
        {
            return OpenVRApi.GetSkeletalActionData(Handle);
        }
	}
}
