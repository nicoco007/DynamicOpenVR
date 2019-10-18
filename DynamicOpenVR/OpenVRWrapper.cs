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

using System;
using System.Runtime.InteropServices;
using Valve.VR;

namespace DynamicOpenVR
{
	internal static class OpenVRWrapper
	{
		internal static bool IsRunning => OpenVR.IsRuntimeInstalled();

		internal static void SetActionManifestPath(string manifestPath)
		{
			EVRInputError error = OpenVR.Input.SetActionManifestPath(manifestPath);

			if (error != EVRInputError.None)
			{
				throw new Exception(error.ToString());
			}
		}

		internal static ulong GetActionSetHandle(string actionSetName)
		{
			ulong handle = default;

			EVRInputError error = OpenVR.Input.GetActionSetHandle(actionSetName, ref handle);

			if (error != EVRInputError.None)
			{
				throw new Exception(error.ToString());
			}

			return handle;
		}

		internal static ulong GetActionHandle(string actionName)
		{
			ulong handle = default;

			EVRInputError error = OpenVR.Input.GetActionHandle(actionName, ref handle);

			if (error != EVRInputError.None)
			{
				throw new Exception(error.ToString());
			}

			return handle;
		}

		internal static void UpdateActionState(ulong[] handles)
		{
			VRActiveActionSet_t[] activeActionSets = new VRActiveActionSet_t[handles.Length];

			for (int i = 0; i < handles.Length; i++)
			{
				activeActionSets[i] = new VRActiveActionSet_t
				{
					ulActionSet = handles[i],
					ulRestrictedToDevice = OpenVR.k_ulInvalidInputValueHandle
				};
			}

			EVRInputError error = OpenVR.Input.UpdateActionState(activeActionSets, (uint)Marshal.SizeOf(typeof(VRActiveActionSet_t)));

			if (error != EVRInputError.None)
			{
				throw new Exception(error.ToString());
			}
		}

		internal static InputAnalogActionData_t GetAnalogActionData(ulong actionHandle)
		{
			InputAnalogActionData_t actionData = default;

			EVRInputError error = OpenVR.Input.GetAnalogActionData(actionHandle, ref actionData, (uint)Marshal.SizeOf(typeof(InputAnalogActionData_t)), OpenVR.k_ulInvalidInputValueHandle);

			if (error != EVRInputError.None)
			{
				throw new Exception(error.ToString());
			}

			return actionData;
		}

        internal static InputDigitalActionData_t GetDigitalActionData(ulong actionHandle)
        {
            InputDigitalActionData_t actionData = default;

            EVRInputError error = OpenVR.Input.GetDigitalActionData(actionHandle, ref actionData, (uint)Marshal.SizeOf(typeof(InputDigitalActionData_t)), OpenVR.k_ulInvalidInputValueHandle);

            if (error != EVRInputError.None)
            {
                throw new Exception(error.ToString());
            }

            return actionData;
        }

        internal static InputSkeletalActionData_t GetSkeletalActionData(ulong actionHandle)
        {
            InputSkeletalActionData_t actionData = default;

            EVRInputError error = OpenVR.Input.GetSkeletalActionData(actionHandle, ref actionData, (uint)Marshal.SizeOf(typeof(InputDigitalActionData_t)));

            if (error != EVRInputError.None)
            {
                throw new Exception(error.ToString());
            }

            return actionData;
        }

        internal static InputPoseActionData_t GetPoseActionDataForNextFrame(ulong actionHandle, ETrackingUniverseOrigin origin = ETrackingUniverseOrigin.TrackingUniverseStanding)
        {
            InputPoseActionData_t actionData = default;

            EVRInputError error = OpenVR.Input.GetPoseActionDataForNextFrame(actionHandle, origin, ref actionData, (uint)Marshal.SizeOf(typeof(InputPoseActionData_t)), OpenVR.k_ulInvalidInputValueHandle);

            if (error != EVRInputError.None)
            {
                throw new Exception(error.ToString());
            }

            return actionData;
        }

		internal static VRSkeletalSummaryData_t GetSkeletalSummaryData(ulong actionHandle, EVRSummaryType summaryType = EVRSummaryType.FromDevice)
		{
			VRSkeletalSummaryData_t summaryData = default;

			EVRInputError error = OpenVR.Input.GetSkeletalSummaryData(actionHandle, summaryType, ref summaryData);

			if (error != EVRInputError.None)
			{
				throw new Exception(error.ToString());
			}

			return summaryData;
		}

        internal static void TriggerHapticVibrationAction(ulong actionHandle, float startSecondsFromNow, float durationSeconds, float frequency, float amplitude)
        {
            EVRInputError error = OpenVR.Input.TriggerHapticVibrationAction(actionHandle, startSecondsFromNow, durationSeconds, frequency, amplitude, OpenVR.k_ulInvalidInputValueHandle);

            if (error != EVRInputError.None)
            {
                throw new Exception(error.ToString());
            }
        }
	}
}
