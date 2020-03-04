using System;
using System.Runtime.InteropServices;

namespace DynamicOpenVR.BeatSaber
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true, CharSet= CharSet.Auto)]
        internal static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
    }

    internal enum MessageBoxType : uint
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
        YesNoCancel = 0x00000003u,
        
        /// <summary>
        /// An exclamation-point icon appears in the message box.
        /// </summary>
        IconExclamation = 0x00000030u,

        /// <summary>
        /// An exclamation-point icon appears in the message box.
        /// </summary>
        IconWarning = 0x00000030u,

        /// <summary>
        /// An icon consisting of a lowercase letter i in a circle appears in the message box.
        /// </summary>
        IconInformation = 0x00000040u,

        /// <summary>
        /// An icon consisting of a lowercase letter i in a circle appears in the message box.
        /// </summary>
        IconAsterisk = 0x00000040u,

        /// <summary>
        /// A question-mark icon appears in the message box. The question-mark message icon is no longer recommended because it does not clearly represent a specific type of message and because the phrasing of a message as a question could apply to any message type. In addition, users can confuse the message symbol question mark with Help information. Therefore, do not use this question mark message symbol in your message boxes. The system continues to support its inclusion only for backward compatibility.
        /// </summary>
        IconQuestion = 0x00000020u,

        /// <summary>
        /// A stop-sign icon appears in the message box.
        /// </summary>
        IconStop = 0x00000010u,

        /// <summary>
        /// A stop-sign icon appears in the message box.
        /// </summary>
        IconError = 0x00000010u,

        /// <summary>
        /// A stop-sign icon appears in the message box.
        /// </summary>
        IconHand = 0x00000010u,

        /// <summary>
        /// The first button is the default button. DefaultButton1 is the default unless DefaultButton2, DefaultButton3, or DefaultButton4 is specified.
        /// </summary>
        DefaultButton1 = 0x00000000u,

        /// <summary>
        /// The second button is the default button.
        /// </summary>
        DefaultButton2 = 0x00000100u,

        /// <summary>
        /// The third button is the default button.
        /// </summary>
        DefaultButton3 = 0x00000200u,

        /// <summary>
        /// The fourth button is the default button.
        /// </summary>
        DefaultButton4 = 0x00000300u
    }

    internal enum DialogResult
    {
        Abort = 3,
        Cancel = 2,
        Continue = 11,
        Ignore = 5,
        No = 7,
        Ok = 1,
        Retry = 4,
        TryAgain = 10,
        Yes = 6
    }
}
