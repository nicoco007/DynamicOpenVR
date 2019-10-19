using System;
using Valve.VR;

namespace DynamicOpenVR
{
    public class OpenVRInputException : Exception
    {
        public EVRInputError Error { get; }

        internal OpenVRInputException(string message, EVRInputError error) : base(message)
        {
            Error = error;
        }
    }
}
