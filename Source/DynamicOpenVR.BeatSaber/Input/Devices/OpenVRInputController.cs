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

using DynamicOpenVR.BeatSaber.InputCollections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

namespace DynamicOpenVR.BeatSaber.Input.Devices
{
    [InputControlLayout(stateType = typeof(OpenVRInputControllerState), commonUsages = new string[] { "LeftHand", "RightHand" })]
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

        public ButtonControl trackpadTouched { get; private set; }

        public AxisControl trackpadForce { get; private set; }

        public PoseControl devicePose { get; private set; }

        public PoseControl pointer { get; private set; }

        public new ButtonControl isTracked { get; private set; }

        public new IntegerControl trackingState { get; private set; }

        public new Vector3Control devicePosition { get; private set; }

        public new QuaternionControl deviceRotation { get; private set; }

        public Vector3Control pointerPosition { get; private set; }

        public QuaternionControl pointerRotation { get; private set; }

        public void OnUpdate()
        {
            bool isTracked = _actions.pose.isTracking;
            InputTrackingState trackingState = isTracked ? InputTrackingState.Position | InputTrackingState.Rotation | InputTrackingState.Velocity | InputTrackingState.AngularVelocity : InputTrackingState.None;
            var state = new OpenVRInputControllerState()
            {
                system = default,
                systemTouched = default,
                primaryButton = _actions.primaryButton.state,
                primaryTouched = _actions.primaryTouch.state,
                secondaryButton = _actions.secondaryButton.state,
                secondaryTouched = _actions.secondaryTouch.state,
                grip = _actions.grip.value,
                gripPressed = _actions.gripButton.state,
                gripForce = default,
                trigger = _actions.trigger.value,
                triggerPressed = _actions.triggerButton.state,
                triggerTouched = default,
                thumbstick = _actions.primary2DAxis.vector,
                thumbstickClicked = _actions.primary2DAxisClick.state,
                thumbstickTouched = _actions.primary2DAxisTouch.state,
                trackpad = default,
                trackpadTouched = default,
                trackpadForce = default,
                devicePose = new PoseState()
                {
                    isTracked = isTracked,
                    trackingState = trackingState,
                    position = _actions.pose.position,
                    rotation = _actions.pose.rotation,
                    velocity = _actions.pose.velocity,
                    angularVelocity = _actions.pose.angularVelocity,
                },
                pointer = default,
                isTracked = isTracked,
                trackingState = trackingState,
                devicePosition = _actions.pose.position,
                deviceRotation = _actions.pose.rotation,
                pointerPosition = default,
                pointerRotation = default,
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

            system = GetChildControl<ButtonControl>("system");
            systemTouched = GetChildControl<ButtonControl>("systemTouched");
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
            trackpadTouched = GetChildControl<ButtonControl>("trackpadTouched");
            trackpadForce = GetChildControl<AxisControl>("trackpadForce");
            devicePose = GetChildControl<PoseControl>("devicePose");
            pointer = GetChildControl<PoseControl>("pointer");
            pointerPosition = GetChildControl<Vector3Control>("pointerPosition");
            pointerRotation = GetChildControl<QuaternionControl>("pointerRotation");
        }
    }
}
