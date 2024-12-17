// <copyright file="OpenVRInput.cs" company="Nicolas Gnyra">
// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
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

using DynamicOpenVR.BeatSaber.Input.Devices;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

namespace DynamicOpenVR.BeatSaber.Input
{
    internal static class OpenVRInput
    {
        private const string kInterfaceName = nameof(DynamicOpenVR);
        private const string kHMDProductName = "OpenVR Input HMD";
        private const string kControllerProductName = "OpenVR Input Controller";
        private const string kTrackerProductName = "OpenVR Tracker";

        internal static void RegisterLayoutsAndAddDevices()
        {
            InputSystem.RegisterLayout<OpenVRInputHMD>(nameof(OpenVRInputHMD), default(InputDeviceMatcher).WithInterface(kInterfaceName).WithProduct(kHMDProductName));
            RegisterHMD();

            InputSystem.RegisterLayout<OpenVRInputController>(nameof(OpenVRInputController), default(InputDeviceMatcher).WithInterface(kInterfaceName).WithProduct(kControllerProductName));
            RegisterController(InputDeviceCharacteristics.Left, "Left");
            RegisterController(InputDeviceCharacteristics.Right, "Right");

            InputSystem.RegisterLayout<XRTracker>(nameof(XRTracker));
            InputSystem.RegisterLayout<OpenVRTracker>(nameof(OpenVRTracker), default(InputDeviceMatcher).WithInterface(kInterfaceName).WithProduct(kTrackerProductName));
            RegisterTracker(InputDeviceTrackerCharacteristics.TrackerLeftFoot, "Left Foot");
            RegisterTracker(InputDeviceTrackerCharacteristics.TrackerRightFoot, "Right Foot");
            RegisterTracker(InputDeviceTrackerCharacteristics.TrackerLeftShoulder, "Left Shoulder");
            RegisterTracker(InputDeviceTrackerCharacteristics.TrackerRightShoulder, "Right Shoulder");
            RegisterTracker(InputDeviceTrackerCharacteristics.TrackerLeftElbow, "Left Elbow");
            RegisterTracker(InputDeviceTrackerCharacteristics.TrackerRightElbow, "Right Elbow");
            RegisterTracker(InputDeviceTrackerCharacteristics.TrackerLeftKnee, "Left Knee");
            RegisterTracker(InputDeviceTrackerCharacteristics.TrackerRightKnee, "Right Knee");
            RegisterTracker(InputDeviceTrackerCharacteristics.TrackerWaist, "Waist");
            RegisterTracker(InputDeviceTrackerCharacteristics.TrackerChest, "Chest");
            RegisterTracker(InputDeviceTrackerCharacteristics.TrackerCamera, "Camera");
            RegisterTracker(InputDeviceTrackerCharacteristics.TrackerKeyboard, "Keyboard");
        }

        internal static void RemoveLayouts()
        {
            // RemoveLayout will recursively remove devices as well
            InputSystem.RemoveLayout(nameof(OpenVRInputHMD));
            InputSystem.RemoveLayout(nameof(OpenVRInputController));
            InputSystem.RemoveLayout(nameof(OpenVRTracker));
            InputSystem.RemoveLayout(nameof(XRTracker));
        }

        private static void RegisterHMD()
        {
            InputSystem.s_Manager.AddDevice(
                new InputDeviceDescription()
                {
                    interfaceName = kInterfaceName,
                    product = kHMDProductName,
                    capabilities = new XRDeviceDescriptor()
                    {
                        characteristics = InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeadMounted,
                    }.ToJson(),
                },
                true,
                kHMDProductName);
        }

        private static void RegisterController(InputDeviceCharacteristics characteristics, string name)
        {
            InputSystem.s_Manager.AddDevice(
                new InputDeviceDescription()
                {
                    interfaceName = kInterfaceName,
                    product = kControllerProductName,
                    capabilities = new XRDeviceDescriptor()
                    {
                        characteristics = InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | characteristics,
                    }.ToJson(),
                },
                true,
                $"{kControllerProductName} ({name})");
        }

        private static void RegisterTracker(InputDeviceTrackerCharacteristics characteristics, string name)
        {
            InputSystem.s_Manager.AddDevice(
                new InputDeviceDescription()
                {
                    interfaceName = kInterfaceName,
                    product = kTrackerProductName,
                    capabilities = new XRDeviceDescriptor()
                    {
                        characteristics = InputDeviceCharacteristics.TrackedDevice | (InputDeviceCharacteristics)characteristics,
                    }.ToJson(),
                },
                true,
                $"{kTrackerProductName} ({name})");
        }
    }
}
