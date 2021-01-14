// DynamicOpenVR - Unity scripts to allow dynamic creation of OpenVR actions at runtime.
// Copyright ?2019-2021 Nicolas Gnyra

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

using DynamicOpenVR.Logging;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicOpenVR.Exceptions;
using Valve.VR;

namespace DynamicOpenVR
{
	internal static class OpenVRFacade
	{
		internal static bool IsRuntimeInstalled()
        {
			return OpenVR.IsRuntimeInstalled();
        }

		internal static void SetActionManifestPath(string manifestPath)
		{
			Logger.Info($"Setting action manifest path to '{manifestPath}'");

			EVRInputError error = OpenVR.Input.SetActionManifestPath(manifestPath);

			if (error != EVRInputError.None)
			{
				throw new OpenVRInputException($"Could not set action manifest path: {error}", error);
			}
		}

		internal static ulong GetActionSetHandle(string actionSetName)
		{
			ulong handle = default;

			EVRInputError error = OpenVR.Input.GetActionSetHandle(actionSetName, ref handle);

			if (error != EVRInputError.None)
			{
				throw new OpenVRInputException($"Could not get handle for action set '{actionSetName}': {error}", error);
			}

			return handle;
		}

		internal static ulong GetActionHandle(string actionName)
		{
			ulong handle = default;

			EVRInputError error = OpenVR.Input.GetActionHandle(actionName, ref handle);

			if (error != EVRInputError.None)
			{
				throw new OpenVRInputException($"Could not get handle for action '{actionName}': {error}", error);
			}

			return handle;
		}

		internal static void UpdateActionState(List<ulong> handles)
		{
			VRActiveActionSet_t[] activeActionSets = new VRActiveActionSet_t[handles.Count];

			for (int i = 0; i < handles.Count; i++)
			{
				activeActionSets[i] = new VRActiveActionSet_t
				{
					ulActionSet = handles[i],
					ulRestrictedToDevice = OpenVR.k_ulInvalidInputValueHandle
				};
			}

			EVRInputError error = OpenVR.Input.UpdateActionState(activeActionSets, (uint)Marshal.SizeOf(typeof(VRActiveActionSet_t)));

			if (error != EVRInputError.None && error != EVRInputError.NoData)
			{
				throw new OpenVRInputException($"Could not update action states: {error}", error);
			}
		}

		internal static InputAnalogActionData_t GetAnalogActionData(ulong actionHandle)
		{
			InputAnalogActionData_t actionData = default;

			EVRInputError error = OpenVR.Input.GetAnalogActionData(actionHandle, ref actionData, (uint)Marshal.SizeOf(typeof(InputAnalogActionData_t)), OpenVR.k_ulInvalidInputValueHandle);

			if (error != EVRInputError.None && error != EVRInputError.NoData)
			{
				throw new OpenVRInputException($"Could not get analog data for action with handle {actionHandle}: {error}", error);
			}

			return actionData;
		}

        internal static InputDigitalActionData_t GetDigitalActionData(ulong actionHandle)
        {
            InputDigitalActionData_t actionData = default;

            EVRInputError error = OpenVR.Input.GetDigitalActionData(actionHandle, ref actionData, (uint)Marshal.SizeOf(typeof(InputDigitalActionData_t)), OpenVR.k_ulInvalidInputValueHandle);

            if (error != EVRInputError.None && error != EVRInputError.NoData)
            {
                throw new OpenVRInputException($"Could not get digital data for action with handle {actionHandle}: {error}", error);
            }

            return actionData;
        }

        internal static InputSkeletalActionData_t GetSkeletalActionData(ulong actionHandle)
        {
            InputSkeletalActionData_t actionData = default;

            EVRInputError error = OpenVR.Input.GetSkeletalActionData(actionHandle, ref actionData, (uint)Marshal.SizeOf(typeof(InputSkeletalActionData_t)));

            if (error != EVRInputError.None && error != EVRInputError.NoData)
            {
                throw new OpenVRInputException($"Could not get skeletal data for action with handle {actionHandle}: {error}", error);
            }

            return actionData;
        }

		internal static InputPoseActionData_t GetPoseActionData(ulong actionHandle, ETrackingUniverseOrigin origin = ETrackingUniverseOrigin.TrackingUniverseStanding)
		{
            InputPoseActionData_t actionData = default;

            EVRInputError error = OpenVR.Input.GetPoseActionDataRelativeToNow(actionHandle, origin, 0, ref actionData, (uint)Marshal.SizeOf(typeof(InputPoseActionData_t)), OpenVR.k_ulInvalidInputValueHandle);
            if (error != EVRInputError.None && error != EVRInputError.NoData)
            {
                throw new OpenVRInputException($"Could not get pose data for action with handle {actionHandle}: {error}", error);
            }

            return actionData;
		}

		internal static VRSkeletalSummaryData_t GetSkeletalSummaryData(ulong actionHandle)
		{
			VRSkeletalSummaryData_t summaryData = default;

			EVRInputError error = OpenVR.Input.GetSkeletalSummaryData(actionHandle, EVRSummaryType.FromDevice, ref summaryData);

			if (error != EVRInputError.None && error != EVRInputError.NoData)
			{
				throw new OpenVRInputException($"Could not get skeletal summary data for action with handle {actionHandle}: {error}", error);
			}

			return summaryData;
		}

        internal static void TriggerHapticVibrationAction(ulong actionHandle, float startSecondsFromNow, float durationSeconds, float frequency, float amplitude)
        {
            EVRInputError error = OpenVR.Input.TriggerHapticVibrationAction(actionHandle, startSecondsFromNow, durationSeconds, frequency, amplitude, OpenVR.k_ulInvalidInputValueHandle);

            if (error != EVRInputError.None && error != EVRInputError.NoData)
            {
                throw new OpenVRInputException($"Failed to trigger haptic feedback vibration for action with handle {actionHandle}: {error}", error);
            }
        }
	}
}
