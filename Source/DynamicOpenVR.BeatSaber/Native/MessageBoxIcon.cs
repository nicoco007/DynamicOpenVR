namespace DynamicOpenVR.BeatSaber.Native
{
    internal enum MessageBoxIcon : uint
    {
        /// <summary>
        /// No icon is displayed.
        /// </summary>
        None = 0x00000000u,

        /// <summary>
        /// An exclamation-point icon appears in the message box.
        /// </summary>
        Exclamation = 0x00000030u,

        /// <summary>
        /// An exclamation-point icon appears in the message box.
        /// </summary>
        Warning = 0x00000030u,

        /// <summary>
        /// An icon consisting of a lowercase letter i in a circle appears in the message box.
        /// </summary>
        Information = 0x00000040u,

        /// <summary>
        /// An icon consisting of a lowercase letter i in a circle appears in the message box.
        /// </summary>
        Asterisk = 0x00000040u,

        /// <summary>
        /// A question-mark icon appears in the message box. The question-mark message icon is no longer recommended because it does not clearly represent a specific type of message and because the phrasing of a message as a question could apply to any message type. In addition, users can confuse the message symbol question mark with Help information. Therefore, do not use this question mark message symbol in your message boxes. The system continues to support its inclusion only for backward compatibility.
        /// </summary>
        Question = 0x00000020u,

        /// <summary>
        /// A stop-sign icon appears in the message box.
        /// </summary>
        Stop = 0x00000010u,

        /// <summary>
        /// A stop-sign icon appears in the message box.
        /// </summary>
        Error = 0x00000010u,

        /// <summary>
        /// A stop-sign icon appears in the message box.
        /// </summary>
        Hand = 0x00000010u
    }
}
