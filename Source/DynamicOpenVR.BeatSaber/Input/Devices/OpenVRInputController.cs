// <copyright file="OpenVRInputController.cs" company="Nicolas Gnyra">
// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
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

using DynamicOpenVR.BeatSaber.InputCollections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

namespace DynamicOpenVR.BeatSaber.Input.Devices
{
    [InputControlLayout(stateType = typeof(OpenVRInputControllerState))]
    internal class OpenVRInputController : XRController, IInputUpdateCallbackReceiver
    {
        private UnityXRActionsHand _actions;

        public void OnUpdate()
        {
            bool isTracked = _actions.pose.isTracking;
            var state = new OpenVRInputControllerState()
            {
                isTracked = isTracked,
                trackingState = isTracked ? InputTrackingState.Position | InputTrackingState.Rotation | InputTrackingState.Velocity | InputTrackingState.AngularVelocity : InputTrackingState.None,
                devicePosition = _actions.pose.position,
                deviceRotation = _actions.pose.rotation,
            };

            InputSystem.QueueStateEvent(this, state);
        }

        public override void FinishSetup()
        {
            base.FinishSetup();
            var descriptor = XRDeviceDescriptor.FromJson(description.capabilities);
            if (descriptor != null)
            {
                if ((descriptor.characteristics & InputDeviceCharacteristics.Left) != 0)
                {
                    _actions = Plugin.unityXRActions.left;
                }
                else if ((descriptor.characteristics & InputDeviceCharacteristics.Right) != 0)
                {
                    _actions = Plugin.unityXRActions.right;
                }
            }
        }
    }
}
