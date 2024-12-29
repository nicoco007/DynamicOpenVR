// <copyright file="OpenVRInputController.cs" company="Nicolas Gnyra">
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
using DynamicOpenVR.BeatSaber.InputCollections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

namespace DynamicOpenVR.BeatSaber.Input.Devices
{
    [InputControlLayout(stateType = typeof(OpenVRInputControllerState), commonUsages = new string[] { "LeftHand", "RightHand" }, updateBeforeRender = true, canRunInBackground = true)]
    internal class OpenVRInputController : XRController, IInputUpdateCallbackReceiver
    {
        private UnityXRActionsHand _actions;

        public ButtonControl system { get; private set; }

        public ButtonControl systemTouched { get; private set; }

        public ButtonControl primaryButton { get; private set; }

        public ButtonControl primaryTouched { get; private set; }

        public ButtonControl secondaryButton { get; private set; }

        public ButtonControl secondaryTouched { get; private set; }

        public AxisControl grip { get; private set; }

        public ButtonControl gripPressed { get; private set; }

        public AxisControl gripForce { get; private set; }

        public AxisControl trigger { get; private set; }

        public ButtonControl triggerPressed { get; private set; }

        public ButtonControl triggerTouched { get; private set; }

        public Vector2Control thumbstick { get; private set; }

        public ButtonControl thumbstickClicked { get; private set; }

        public ButtonControl thumbstickTouched { get; private set; }

        public Vector2Control trackpad { get; private set; }

        public ButtonControl trackpadClicked { get; private set; }

        public ButtonControl trackpadTouched { get; private set; }

        public AxisControl trackpadForce { get; private set; }

        public PoseControl devicePose { get; private set; }

        public PoseControl pointer { get; private set; }

        public Vector3Control pointerPosition { get; private set; }

        public QuaternionControl pointerRotation { get; private set; }

        public void OnUpdate()
        {
            bool isTracked = _actions.devicePose.isTracking;
            InputTrackingState trackingState = isTracked ? InputTrackingState.Position | InputTrackingState.Rotation | InputTrackingState.Velocity | InputTrackingState.AngularVelocity : InputTrackingState.None;
            var state = new OpenVRInputControllerState()
            {
                system = _actions.system.state,
                systemTouched = _actions.systemTouched.state,
                select = _actions.select.state,
                menu = _actions.menu.state,
                primaryButton = _actions.primaryButton.state,
                primaryTouched = _actions.primaryTouched.state,
                secondaryButton = _actions.secondaryButton.state,
                secondaryTouched = _actions.secondaryTouched.state,
                grip = _actions.grip.value,
                gripPressed = _actions.gripPressed.state,
                gripForce = _actions.gripForce.value,
                trigger = _actions.trigger.value,
                triggerPressed = _actions.triggerPressed.state,
                triggerTouched = _actions.triggerTouched.state,
                thumbstick = _actions.thumbstick.vector,
                thumbstickClicked = _actions.thumbstickClicked.state,
                thumbstickTouched = _actions.thumbstickTouched.state,
                trackpad = _actions.trackpad.vector,
                trackpadClicked = _actions.trackpadClicked.state,
                trackpadTouched = _actions.trackpadTouched.state,
                trackpadForce = _actions.trackpadForce.value,
                devicePose = new OpenVRInputControllerState.PoseState(isTracked, trackingState, _actions.devicePose.position, _actions.devicePose.rotation, _actions.devicePose.velocity, _actions.devicePose.angularVelocity),
                pointer = new OpenVRInputControllerState.PoseState(isTracked, trackingState, _actions.pointer.position, _actions.pointer.rotation, _actions.pointer.velocity, _actions.pointer.angularVelocity),
                isTracked = isTracked,
                trackingState = trackingState,
                devicePosition = _actions.devicePose.position,
                deviceRotation = _actions.devicePose.rotation,
                pointerPosition = _actions.pointer.position,
                pointerRotation = _actions.pointer.rotation,
            };

            InputSystem.QueueStateEvent(this, state);
        }

        public override void FinishSetup()
        {
            base.FinishSetup();

            XRDeviceDescriptor descriptor = XRDeviceDescriptor.FromJson(description.capabilities) ?? throw new ArgumentException("Device must have capabilities defined");

            InputDeviceCharacteristics characteristics = descriptor.characteristics;

            if (characteristics.HasFlag(InputDeviceCharacteristics.Left))
            {
                _actions = Plugin.unityXRActions.left;
            }
            else if (characteristics.HasFlag(InputDeviceCharacteristics.Right))
            {
                _actions = Plugin.unityXRActions.right;
            }
            else
            {
                throw new ArgumentException("Controller must have the Left or Right characteristic");
            }

            system = GetChildControl<ButtonControl>("system");
            systemTouched = GetChildControl<ButtonControl>("systemTouched");
            system = GetChildControl<ButtonControl>("select");
            system = GetChildControl<ButtonControl>("menu");
            primaryButton = GetChildControl<ButtonControl>("primaryButton");
            primaryTouched = GetChildControl<ButtonControl>("primaryTouched");
            secondaryButton = GetChildControl<ButtonControl>("secondaryButton");
            secondaryTouched = GetChildControl<ButtonControl>("secondaryTouched");
            grip = GetChildControl<AxisControl>("grip");
            gripPressed = GetChildControl<ButtonControl>("gripPressed");
            gripForce = GetChildControl<AxisControl>("gripForce");
            trigger = GetChildControl<AxisControl>("trigger");
            triggerPressed = GetChildControl<ButtonControl>("triggerPressed");
            triggerTouched = GetChildControl<ButtonControl>("triggerTouched");
            thumbstick = GetChildControl<Vector2Control>("thumbstick");
            thumbstickClicked = GetChildControl<ButtonControl>("thumbstickClicked");
            thumbstickTouched = GetChildControl<ButtonControl>("thumbstickTouched");
            trackpad = GetChildControl<Vector2Control>("trackpad");
            trackpadClicked = GetChildControl<ButtonControl>("trackpadClicked");
            trackpadTouched = GetChildControl<ButtonControl>("trackpadTouched");
            trackpadForce = GetChildControl<AxisControl>("trackpadForce");
            devicePose = GetChildControl<PoseControl>("devicePose");
            pointer = GetChildControl<PoseControl>("pointer");
            pointerPosition = GetChildControl<Vector3Control>("pointerPosition");
            pointerRotation = GetChildControl<QuaternionControl>("pointerRotation");
        }
    }
}
