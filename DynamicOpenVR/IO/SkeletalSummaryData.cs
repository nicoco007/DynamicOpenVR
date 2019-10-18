// DynamicOpenVR - Unity scripts to allow dynamic creation of OpenVR actions at runtime.
// Copyright © 2019 Nicolas Gnyra

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.

// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.

using Valve.VR;

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
