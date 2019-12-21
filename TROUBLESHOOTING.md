# Troubleshooting
## Buttons and/or tracking aren't working properly
*If this happens after uninstalling DynamicOpenVR, please see [Completely Removing DynamicOpenVR](TROUBLESHOOTING.md#completely-removing-dynamicopenvr).*

Input or tracking not working are usually a symptom of missing or broken input bindings. Follow the instructions below to diagnose this kind of issue.

### Diagnosing issues with bindings
1. Start Beat Saber.
2. Open Controller Settings by clicking on the SteamVR menu button and choosing Devices > Controller Settings.
   
   ![](Documentation/Images/controller-settings.png)

3. Click on Controllers, then Manage Controller Bindings.
   
   ![](Documentation/Images/manage-controller-bindings.png)

4. Make sure Beat Saber is the selected game at the top of the window, and click on Custom next to Active Controller Binding.

   ![](Documentation/Images/manage-bindings-default.png)

5. You should now see which binding is currently being used by Beat Saber.
   * If it is called "Default Beat Saber Bindings," your game should technically be working. Try completely closing SteamVR and starting Beat Saber again. If input still doesn't work, please [open an issue](https://github.com/nicoco007/DynamicOpenVR/issues).
   * If it is called "Default bindings for legacy applications for &lt;Your Controllers&gt;," DynamicOpenVR did not register properly. Try completely closing SteamVR and starting Beat Saber again. If the same binding is still there, please [open an issue](https://github.com/nicoco007/DynamicOpenVR/issues).
   * If the window is stuck on "Loading current binding..." or something else, then there are most likely no default bindings for the controllers you are currently using.

   If you can see the "Edit this Binding" button, continue to step 8. If not, continue to the next step.

   ![](Documentation/Images/binding-possibilities.png)

6. Close the binding management popup and click "Show" under "Advanced Settings" in the bottom left corner. You should now see the "Show Old Binding UI" button; press it.

   ![](Documentation/Images/enable-advanced-settings.png)

7. In the window that opens, select "Beat Saber." Since you have the game already open, it should be at the top of the list.

   ![](Documentation/Images/select-game.png)

8. Make sure the controllers you are currently using are selected.

   ![](Documentation/Images/select-right-controller.png)

9. Scroll all the way down until you see the "Create New Binding" button and press it.

   ![](Documentation/Images/create-binding.png)

10. You should now see the screen below. If you can see all 3 buttons, skip to step 14. If you only see the "Add Chords" button (or if any of the buttons are missing), proceed to the next step.

    ![](Documentation/Images/verify-buttons.png)

11. Press the back button in the top right corner and select one of the following controllers:
    * Index Controller
    * Oculus Touch
    * Vive Controller
   
    You **must** choose one of the above controllers because they come with default bindings. For this guide, we will choose the Index Controller.

    ![](Documentation/Images/select-different-controller.png)

12. Press the "Edit" button under "Current Binding."

    ![](Documentation/Images/edit-existing-binding.png)

13. Immediately go back, choose your own controller again, and scroll down to press the "Create New Binding" button once more.

    ![](Documentation/Images/go-back.png)

    ![](Documentation/Images/select-right-controller.png)

    ![](Documentation/Images/create-binding.png)

14. Start configuring the binding. Start with buttons and triggers.
    * "Menu Button" &rarr; Menu Click
    * "Left Trigger Pull" &rarr; Left Trigger Pull
    * "Right Trigger Pull" &rarr; Right Trigger Pull

    Below is an example of adding the Left Trigger Pull action.

    1. Press the "+" button next to the input to which you want to bind an action. For this example, we will add the action to the left trigger.

        ![](Documentation/Images/plus.png)

    2. Next, select the mode in which the input will be used. Since we want to use the trigger as, well, a trigger, select "Trigger."

       ![](Documentation/Images/use-input-as.png)

    3. Select the value to be used for the action. Since we want the trigger's value between 0 and 1, select "Pull."

       ![](Documentation/Images/pull.png)

    4. Finally, select the action to bind to the value. Since this is the left hand trigger, select "Left Trigger Pull."

       ![](Documentation/Images/vector1-actions.png)
    
    Repeat these steps for the right trigger and the menu button. Note that for the menu button, you should be choosing "Button" as the mode and "Click" as the value to be used.

15. Once you are done adding the button bindings, your screen should look like this.

    ![](Documentation/Images/bindings.png)

16. After you've done the buttons, add the following action poses by pressing the "Edit Action Poses" button.

    * "Left Hand Pose" &rarr; Left Hand Raw
    * "Right Hand Pose" &rarr; Right Hand Raw

    ![](Documentation/Images/poses.png)

17. Finally, you can add the haptics.
    
    * "Left Slice Haptic Feedback" &rarr; "Left Hand Haptics"
    * "Right Slice Haptic Feedback" &rarr; "Right Hand Haptics"

    ![](Documentation/Images/haptics.png)

18. Once you are done, you can save your bindings by pressing "Save Personal Binding" at the bottom of the screen, entering a name and a description, and pressing the "Save" button. Note that it may take a few seconds for the binding to save and for the popup to close (don't press cancel!).

    ![](Documentation/Images/save.png)

19. And there you go! Your bindings are now configured and you should be able to use in-game input properly. You can now close the settings windows and enjoy your game! If you would like to submit the bindings you just created to be included by default in DynamicOpenVR, continue on to the section below.

    ![](Documentation/Images/done.png)

### Exporting bindings so they can be included by default in DynamicOpenVR

1. Open Controller Settings and make sure Advanced Settings are set to show (see above if necessary). Select "Developer" in the left-hand pane, turn "Enable debugging options in the input binding user interface" to ON, and press the "Restart SteamVR" button that appears afterwards.

    ![](Documentation/Images/enable-debug-input.png)

2. After SteamVR has restarted, go once again to Controller Settings, select Controllers, press "Manage Controller Bindings."

   ![](Documentation/Images/manage-controller-bindings.png)

3. Make sure Beat Saber is selected, press "Custom," and press "Edit Binding."

   ![](Documentation/Images/manage-bindings-advanced.png)

4. You should now once again see your binding. Press the "Replace Default Binding" that has appeared at the bottom of the screen.

    ![](Documentation/Images/replace-default-binding.png)

5. A new file whose name starts with `steam_app_620980_binding_` followed by your controller name have been created in the `DynamicOpenVR` folder in your Beat Saber installation folder (usually `C:\Program Files (x86)\Steam\steamapps\common\Beat Saber`). Attach that file to an issue, and you're good to go!

   ![](Documentation/Images/files.png)
   

## Completely Removing DynamicOpenVR
DynamicOpenVR generates files specific to your computer to improve user experience in SteamVR's interface. These files may cause issues after partially removing DynamicOpenVR. To completely uninstall DynamicOpenVR, make sure these files/folders no longer exist in Beat Saber's installation folder:

* Libs\DynamicOpenVR.dll
* Plugins\DynamicOpenVR.BeatSaber.dll
* DynamicOpenVR\
* beatsaber.vrmanifest

If you're still having issues after removing all of these, please [open an issue](https://github.com/nicoco007/DynamicOpenVR/issues).
