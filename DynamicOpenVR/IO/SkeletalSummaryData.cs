namespace DynamicOpenVR.IO
{
	public class SkeletalSummaryData
	{
		public float ThumbCurl { get; }
		public float IndexCurl { get; }
		public float MiddleCurl { get; }
		public float RingCurl { get; }
		public float LittleCurl { get; }

		public float ThumbIndexSplay { get; }
		public float IndexMiddleSplay { get; }
		public float MiddleRingSplay { get; }
		public float RingLittleSplay { get; }

		internal SkeletalSummaryData(VRSkeletalSummaryData_t summaryDataStruct)
		{
			ThumbCurl  = summaryDataStruct.flFingerCurl0;
			IndexCurl  = summaryDataStruct.flFingerCurl1;
			MiddleCurl = summaryDataStruct.flFingerCurl2;
			RingCurl   = summaryDataStruct.flFingerCurl3;
			LittleCurl = summaryDataStruct.flFingerCurl4;

			ThumbIndexSplay  = summaryDataStruct.flFingerSplay0;
			IndexMiddleSplay = summaryDataStruct.flFingerSplay1;
			MiddleRingSplay  = summaryDataStruct.flFingerSplay2;
			RingLittleSplay  = summaryDataStruct.flFingerSplay3;
		}
	}
}
