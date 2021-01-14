// DynamicOpenVR - Unity scripts to allow dynamic creation of OpenVR actions at runtime.
// Copyright © 2019-2021 Nicolas Gnyra

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
		public float thumbCurl { get; }
		public float indexCurl { get; }
		public float middleCurl { get; }
		public float ringCurl { get; }
		public float littleCurl { get; }

		public float thumbIndexSplay { get; }
		public float indexMiddleSplay { get; }
		public float middleRingSplay { get; }
		public float ringLittleSplay { get; }

		internal SkeletalSummaryData(VRSkeletalSummaryData_t summaryDataStruct)
		{
			thumbCurl  = summaryDataStruct.flFingerCurl0;
			indexCurl  = summaryDataStruct.flFingerCurl1;
			middleCurl = summaryDataStruct.flFingerCurl2;
			ringCurl   = summaryDataStruct.flFingerCurl3;
			littleCurl = summaryDataStruct.flFingerCurl4;

			thumbIndexSplay  = summaryDataStruct.flFingerSplay0;
			indexMiddleSplay = summaryDataStruct.flFingerSplay1;
			middleRingSplay  = summaryDataStruct.flFingerSplay2;
			ringLittleSplay  = summaryDataStruct.flFingerSplay3;
		}
	}
}
