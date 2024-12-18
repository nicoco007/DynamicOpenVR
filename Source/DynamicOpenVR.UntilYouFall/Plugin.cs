using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using SG.Claymore;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.Rendering;

namespace DynamicOpenVR.UntilYouFall
{
    [BepInPlugin("com.nicoco007.until-you-fall.dynamicopenvr", "DynamicOpenVR.UntilYouFall", "1.0.0")]
    public class Plugin : BasePlugin
    {
        internal static Plugin instance { get; private set; }

        public Plugin()
            : base()
        {
            instance = this;
        }

        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            try
            {
                OpenVRUtilities.Init(false);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to initialize OpenVR API; DynamicOpenVR will not run");
                Debug.LogError(ex?.ToString());
                return;
            }

            Logging.Logger.handler = new BepInExLoggerWrapper(Log);
            OpenVRActionManager.instance.Configure("Until You Fall", Path.Combine(Paths.GameRootPath, "actions.json"));
            OpenVRActionManager.instance.Start();
        }

        private class OpenVRActionManagerUpdate : Il2CppSystem.Object { }

        [HarmonyPatch(typeof(ClaymorePlayerLoopInitializer), nameof(ClaymorePlayerLoopInitializer.Init))]
        private static class ClaymorePlayerLoopInitializer_Init
        {
            private static void Postfix()
            {
                ClassInjector.RegisterTypeInIl2Cpp<OpenVRActionManagerUpdate>();

                var mySystem = new PlayerLoopSystem
                {
                    type = Il2CppType.Of<OpenVRActionManagerUpdate>(),
                    updateDelegate = (PlayerLoopSystem.UpdateFunction)(() => OpenVRActionManager.instance.Update(UpdateType.Dynamic)),
                };

                PlayerLoopSystem playerLoop = PlayerLoop.GetCurrentPlayerLoop();
                PlayerLoopSystem[] systems = playerLoop.subSystemList;
                int index = Array.FindIndex(systems, s => s.type == Il2CppType.Of<UnityEngine.PlayerLoop.EarlyUpdate>());

                List<PlayerLoopSystem> subsystems = new(systems[index].subSystemList);
                subsystems.Insert(0, mySystem);

                systems[index].subSystemList = subsystems.ToArray();
                playerLoop.subSystemList = systems;

                PlayerLoop.SetPlayerLoop(playerLoop);

                RenderPipelineManager.beginCameraRendering += new Action<ScriptableRenderContext, Camera>((ctx, camera) => OpenVRActionManager.instance.Update(UpdateType.BeforeRender));
            }
        }

        private class BepInExLoggerWrapper : Logging.ILogHandler
        {
            private readonly ManualLogSource _manualLogSource;

            public BepInExLoggerWrapper(ManualLogSource manualLogSource)
            {
                _manualLogSource = manualLogSource;
            }

            public void Critical(object message)
            {
                _manualLogSource.LogError(message);
            }

            public void Debug(object message)
            {
                _manualLogSource.LogDebug(message);
            }

            public void Error(object message)
            {
                _manualLogSource.LogError(message);
            }

            public void Info(object message)
            {
                _manualLogSource.LogInfo(message);
            }

            public void Notice(object message)
            {
                _manualLogSource.LogInfo(message);
            }

            public void Trace(object message)
            {
                _manualLogSource.LogDebug(message);
            }

            public void Warn(object message)
            {
                _manualLogSource.LogWarning(message);
            }
        }
    }
}
