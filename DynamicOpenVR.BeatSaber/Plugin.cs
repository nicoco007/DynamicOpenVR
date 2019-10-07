using DynamicOpenVR.IO;
using Harmony;
using IPA;
using IPA.Logging;
using UnityEngine.SceneManagement;

namespace DynamicOpenVR.BeatSaber
{
    public class Plugin : IBeatSaberPlugin
    {
        public const string ActionSetName = "main";
        public const string LeftTriggerValueAction = "LeftTriggerValue";
        public const string RightTriggerValueAction = "RightTriggerValue";
        public const string MenuAction = "Menu";
        public const string LeftSliceAction = "LeftSlice";
        public const string RightSliceAction = "RightSlice";

        public static string Name => typeof(Plugin).Namespace;

        public static Logger Logger { get; private set; }

        private HarmonyInstance harmonyInstance;

        public void Init(Logger logger)
        {
            Logger = logger;

            Logger.Info("Starting " + Name);

            if (!OpenVRApi.IsRunning)
            {
                Logger.Warn($"OpenVR not detected. {Name} will not be activated.");
                return;
            }

            RegisterActionSet();
            ApplyHarmonyPatches();
        }

        private void RegisterActionSet()
        {
            Logger.Info("Registering actions");

            OpenVRActionManager manager = OpenVRActionManager.Instance;

            OVRActionSet actionSet = new OVRActionSet(ActionSetName, OVRActionSetUsage.LeftRight).AddTranslation("en_us", "Beat Saber");

            actionSet.RegisterAction(new VectorInput(LeftTriggerValueAction, OVRActionRequirement.Mandatory).AddTranslation("en_us", "Left Trigger Pull").AddTranslation("fr_fr", "Gâchette gauche (tirer)"));
            actionSet.RegisterAction(new VectorInput(RightTriggerValueAction, OVRActionRequirement.Mandatory).AddTranslation("en_us", "Right Trigger Pull").AddTranslation("fr_fr", "Gâchette droite (tirer)"));
            actionSet.RegisterAction(new ButtonInput(MenuAction, OVRActionRequirement.Mandatory).AddTranslation("en_us", "Menu Button").AddTranslation("fr_fr", "Bouton Menu"));
            actionSet.RegisterAction(new HapticVibrationOutput(LeftSliceAction).AddTranslation("en_us", "Left Slice Haptic Feedback").AddTranslation("fr_fr", "Retour haptique pour coupe de gauche"));
            actionSet.RegisterAction(new HapticVibrationOutput(RightSliceAction).AddTranslation("en_us", "Left Slice Haptic Feedback").AddTranslation("fr_fr", "Retour haptique pour coupe de droite"));

            manager.RegisterActionSet(actionSet);
        }

        private void ApplyHarmonyPatches()
        {
            Logger.Info("Applying OpenVR patches");

            harmonyInstance = HarmonyInstance.Create("com.DynamicOpenVR");
            harmonyInstance.PatchAll();
        }

        public void OnApplicationStart() { }
        public void OnApplicationQuit() { }
        public void OnFixedUpdate() { }
        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) { }
        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene) { }
        public void OnSceneUnloaded(Scene scene) { }
        public void OnUpdate() { }
    }
}
