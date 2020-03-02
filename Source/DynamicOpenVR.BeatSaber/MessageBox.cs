using System;

namespace DynamicOpenVR.BeatSaber
{
    internal enum MessageBoxButtons : uint
    {
        /// <summary>
        /// The message box contains three push buttons: Abort, Retry, and Ignore. 
        /// </summary>
        AbortRetryIgnore = 0x00000002u,

        /// <summary>
        /// The message box contains three push buttons: Cancel, Try Again, Continue. Use this message box type instead of MB_ABORTRETRYIGNORE. 
        /// </summary>
        CancelTryContinue = 0x00000006u,

        /// <summary>
        /// Adds a Help button to the message box. When the user clicks the Help button or presses F1, the system sends a WM_HELP message to the owner. 
        /// </summary>
        Help = 0x00004000u,

        /// <summary>
        /// The message box contains one push button: OK. This is the default. 
        /// </summary>
        Ok = 0x00000000u,

        /// <summary>
        /// The message box contains two push buttons: OK and Cancel. 
        /// </summary>
        OkCancel = 0x00000001u,

        /// <summary>
        /// The message box contains two push buttons: Retry and Cancel. 
        /// </summary>
        RetryCancel = 0x00000005u,

        /// <summary>
        /// The message box contains two push buttons: Yes and No. 
        /// </summary>
        YesNo = 0x00000004u,

        /// <summary>
        /// The message box contains three push buttons: Yes, No, and Cancel. 
        /// </summary>
        YesNoCancel = 0x00000003u
    }

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

    internal class MessageBox
    {
        internal static DialogResult Show(string message, string title, MessageBoxButtons buttons = MessageBoxButtons.Ok, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            uint type = (uint)buttons | (uint)icon;

            return (DialogResult) NativeMethods.MessageBox(IntPtr.Zero, message, title, type);
        }
    }
}
