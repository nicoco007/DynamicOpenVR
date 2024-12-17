// <copyright file="OpenVRLoaderWithInputSystem.cs" company="Nicolas Gnyra">
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

using System;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR.Hands;
using Valve.VR;

namespace DynamicOpenVR.BeatSaber.Input
{
    internal class OpenVRLoaderWithInputSystem : OpenVRLoader
    {
        private readonly TrackedDevicePose_t[] _renderPoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
        private readonly TrackedDevicePose_t[] _gamePoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];

        private XRHandSubsystem _handSubsystem;

        internal static TrackedDevicePose_t[] currentPoses { get; private set; } = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];

        public override bool Initialize()
        {
            Plugin.harmony.PatchCategory(Plugin.kOpenVRLoaderHarmonyCategory);

            List<XRHandSubsystemDescriptor> list = new();
            SubsystemManager.GetSubsystemDescriptors(list);
            CreateSubsystem<XRHandSubsystemDescriptor, XRHandSubsystem>(list, OpenVRHandProvider.id);
            _handSubsystem = GetLoadedSubsystem<XRHandSubsystem>();

            return base.Initialize();
        }

        public override bool Start()
        {
            if (!base.Start())
            {
                return false;
            }

            try
            {
                OpenVRActionManager.instance.Start();
                OpenVRInput.RegisterLayoutsAndAddDevices();

                StartSubsystem<XRHandSubsystem>();

                InputSystem.onBeforeUpdate += OnBeforeUpdate;

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }

        public override bool Stop()
        {
            bool result;

            try
            {
                InputSystem.onBeforeUpdate -= OnBeforeUpdate;

                StopSubsystem<XRHandSubsystem>();

                OpenVRInput.RemoveLayouts();
                OpenVRActionManager.instance.Stop();

                result = true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                result = false;
            }

            return base.Stop() && result;
        }

        public override bool Deinitialize()
        {
            Plugin.harmony.UnpatchCategory(Plugin.kOpenVRLoaderHarmonyCategory);
            return base.Deinitialize();
        }

        private void OnBeforeUpdate()
        {
            InputUpdateType updateType = InputState.currentUpdateType;

            if (updateType == InputUpdateType.BeforeRender)
            {
                // only need to do this once per frame
                OpenVR.Compositor.GetLastPoses(_renderPoses, _gamePoses);
            }

            currentPoses = updateType == InputUpdateType.BeforeRender ? _renderPoses : _gamePoses;

            OpenVRActionManager.instance.Update(); // TODO: pass updateType

            _handSubsystem.TryUpdateHands(updateType == InputUpdateType.BeforeRender ? XRHandSubsystem.UpdateType.BeforeRender : XRHandSubsystem.UpdateType.Dynamic);
        }
    }
}
