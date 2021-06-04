// <copyright file="Vector3Input.cs" company="Nicolas Gnyra">
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

using UnityEngine;

namespace DynamicOpenVR.IO
{
    public class Vector3Input : AnalogInput
    {
        public Vector3Input(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the current state of this axis of the analog action.
        /// </summary>
        public Vector3 vector => new Vector3(_actionData.x, _actionData.y, _actionData.z);

        /// <summary>
        /// Gets the change in this axis for this action since the previous frame.
        /// </summary>
        public Vector3 delta => new Vector3(_actionData.deltaX, _actionData.deltaY, _actionData.deltaZ);
    }
}
