# Troubleshooting
## Buttons and/or tracking isn't working properly
If this happens after uninstalling DynamicOpenVR, please see [Removing DynamicOpenVR](https://github.com/nicoco007/DynamicOpenVR/blob/master/TROUBLESHOOTING.md#removing-dynamicopenvr).

Make sure the right controller bindings are selected by going into your controller settings and selecting 
the bindings called "Default Beat Saber Bindings &ndash; Action Bindings for Beat Saber". Follow the steps below to do so. Please note that you may encounter error messages when opening Controller Settings &ndash; this might happen the first time you open the window, but should no longer be an issue afterewards. If you are constantly getting an error message, please [open an issue](https://github.com/nicoco007/DynamicOpenVR/issues).

1. Make sure you are running the game.
2. Open the controller settings window.

![](https://i.imgur.com/hmnHmIV.png)

3. Select Beat Saber.

![](https://i.imgur.com/embdQYO.png)

4. Make sure you have the right binding under Current Binding and the right controller type under Current Controller. If the current binding is anything else, Beat Saber may not work properly.

![](https://i.imgur.com/FJmjP7C.png)

## Removing DynamicOpenVR
DynamicOpenVR generates files specific to your computer to improve user experience in SteamVR's interface. These files may cause issues after partially removing DynamicOpenVR. To completely uninstall DynamicOpenVR, make sure these files/folders no longer exist in Beat Saber's installation folder:

* Libs\DynamicOpenVR.dll
* Plugins\DynamicOpenVR.BeatSaber.dll
* DynamicOpenVR\
* beatsaber.vrmanifest

If you're still having issues after removing all of these, please [open an issue](https://github.com/nicoco007/DynamicOpenVR/issues).
