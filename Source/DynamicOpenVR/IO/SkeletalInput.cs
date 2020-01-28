// DynamicOpenVR - Unity scripts to allow dynamic creation of OpenVR actions at runtime.
// Copyright © 2019-2020 Nicolas Gnyra

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
	public class SkeletalInput : OVRInput
    {
        private InputSkeletalActionData_t _actionData;
        private VRSkeletalSummaryData_t _summaryData;

        public SkeletalInput(string name) : base(name) { }

        /// <summary>
        /// Is set to True if this action is bound to an input source that is present in the system and is in an action set that is active.
        /// </summary>
        public override bool isActive => _actionData.bActive;

        /// <summary>
        /// Retrieves the summary data of the skeleton (finger curl and splay).
        /// </summary>
        public SkeletalSummaryData summaryData => new SkeletalSummaryData(_summaryData);

        internal override void UpdateData()
        {
            _actionData = OpenVRWrapper.GetSkeletalActionData(handle);
            _summaryData = OpenVRWrapper.GetSkeletalSummaryData(handle);
        }
    }
}
