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

using DynamicOpenVR.Bindings;

namespace DynamicOpenVR.IO
{
	public class BooleanInput : Input
	{
		public BooleanInput(string name, OVRActionRequirement requirement = OVRActionRequirement.Suggested) : base(name, requirement, "boolean") { }

        /// <summary>
        /// Is set to True if this action is bound to an input source that is present in the system and is in an action set that is active.
        /// </summary>
        public override bool IsActive()
        {
            return GetActionData().bActive;
        }

        /// <summary>
        /// The current state of this digital action. True means the user wants to perform this action.
        /// </summary>
		public bool GetState()
		{
            return GetActionData().bState;
		}

        /// <summary>
        /// If the state changed from disabled to enabled since it was last checked.
        /// </summary>
		public bool GetActiveChange()
		{
            InputDigitalActionData_t actionData = GetActionData();
			return actionData.bState && actionData.bChanged;
		}

        /// <summary>
        /// If the state changed from enabled to disabled since it was last checked.
        /// </summary>
        public bool GetInactiveChange()
        {
            InputDigitalActionData_t actionData = GetActionData();
			return !actionData.bState && actionData.bChanged;
		}

        public void AddBinding(string path, string mode, string input)
        {
            bindings.Add(path, new SourceBinding(path, mode, input));
        }

        private InputDigitalActionData_t GetActionData()
        {
            return OpenVRApi.GetDigitalActionData(Handle);
        }
	}
}
