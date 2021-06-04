// <copyright file="BooleanInput.cs" company="Nicolas Gnyra">
// DynamicOpenVR - Unity scripts to allow dynamic creation of OpenVR actions at runtime.
// Copyright © 2019-2021 Nicolas Gnyra
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>

using Valve.VR;

namespace DynamicOpenVR.IO
{
    public class BooleanInput : OVRInput
    {
        private InputDigitalActionData_t _actionData;

        public BooleanInput(string name)
            : base(name)
        {
        }

        /// <inheritdoc/>
        public override bool isActive => _actionData.bActive;

        /// <summary>
        /// Gets a value indicating whether the user wants to perform this action or not.
        /// </summary>
        public bool state => _actionData.bState;

        /// <summary>
        /// Gets a value indicating whether the state changed from disabled to enabled since it was last checked or not.
        /// </summary>
        public bool activeChange => _actionData.bState && _actionData.bChanged;

        /// <summary>
        /// Gets a value indicating whether the state changed from enabled to disabled since it was last checked or not.
        /// </summary>
        public bool inactiveChange => !_actionData.bState && _actionData.bChanged;

        /// <inheritdoc/>
        internal override void UpdateData()
        {
            _actionData = OpenVRFacade.GetDigitalActionData(handle);
        }
    }
}
