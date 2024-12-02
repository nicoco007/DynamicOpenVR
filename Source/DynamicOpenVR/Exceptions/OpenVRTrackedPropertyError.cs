// <copyright file="OpenVRTrackedPropertyError.cs" company="Nicolas Gnyra">
// DynamicOpenVR - Unity scripts to allow dynamic creation of OpenVR actions at runtime.
// Copyright © 2019-2023 Nicolas Gnyra
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

#pragma warning disable IDE1006
namespace DynamicOpenVR.Exceptions
{
    public class OpenVRTrackedPropertyError : OpenVRException
    {
        internal OpenVRTrackedPropertyError(string message, uint unDeviceIndex, ETrackedPropertyError error)
            : base(message)
        {
            DeviceIndex = unDeviceIndex;
            Error = error;
        }

        public uint DeviceIndex { get; }

        public ETrackedPropertyError Error { get; }
    }
}
