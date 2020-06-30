namespace DynamicOpenVR.BeatSaber.Native
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
}
