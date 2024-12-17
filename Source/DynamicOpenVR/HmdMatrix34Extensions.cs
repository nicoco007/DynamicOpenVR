// <copyright file="HmdMatrix34Extensions.cs" company="Nicolas Gnyra">
// DynamicOpenVR - Unity scripts to allow dynamic creation of OpenVR actions at runtime.
// Copyright © 2019-2024 Nicolas Gnyra
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

#if OPENVR_XR_API
using UnityEngine;
using Valve.VR;

namespace DynamicOpenVR
{
    public static class HmdMatrix34Extensions
    {
        public static Vector3 GetPosition(this HmdMatrix34_t matrix)
        {
            return new Vector3(matrix.m3, matrix.m7, -matrix.m11);
        }

        public static bool IsRotationValid(this HmdMatrix34_t matrix)
        {
            return (matrix.m2 != 0 || matrix.m6 != 0 || matrix.m10 != 0) && (matrix.m1 != 0 || matrix.m5 != 0 || matrix.m9 != 0);
        }

        public static Quaternion GetRotation(this HmdMatrix34_t matrix)
        {
            if (matrix.IsRotationValid())
            {
                float w = Mathf.Sqrt(Mathf.Max(0, 1 + matrix.m0 + matrix.m5 + matrix.m10)) / 2;
                float x = Mathf.Sqrt(Mathf.Max(0, 1 + matrix.m0 - matrix.m5 - matrix.m10)) / 2;
                float y = Mathf.Sqrt(Mathf.Max(0, 1 - matrix.m0 + matrix.m5 - matrix.m10)) / 2;
                float z = Mathf.Sqrt(Mathf.Max(0, 1 - matrix.m0 - matrix.m5 + matrix.m10)) / 2;

                CopySign(ref x, -matrix.m9 - -matrix.m6);
                CopySign(ref y, -matrix.m2 - -matrix.m8);
                CopySign(ref z, matrix.m4 - matrix.m1);

                return new Quaternion(x, y, z, w);
            }

            return Quaternion.identity;
        }

        private static void CopySign(ref float sizeval, float signval)
        {
            if (signval > 0 != sizeval > 0)
            {
                sizeval = -sizeval;
            }
        }
    }
}
#endif
