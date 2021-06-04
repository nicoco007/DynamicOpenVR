﻿// <copyright file="AppConfigConfirmationModal.cs" company="Nicolas Gnyra">
// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
// Copyright © 2019-2021 Nicolas Gnyra
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

using System.IO;
using System.Linq;
using HMUI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace DynamicOpenVR.BeatSaber
{
    internal class AppConfigConfirmationModal : MonoBehaviour
    {
        private ModalView _modalView;
        private MainMenuViewController _mainMenuViewController;
        private JObject _appConfig;

        public static AppConfigConfirmationModal Create(JObject updatedAppConfig)
        {
            Transform viewControllers = GameObject.Find("/ViewControllers").transform;
            DiContainer container = Resources.FindObjectsOfTypeAll<SceneContext>().First(sc => sc.gameObject.scene.name == "MainMenu").Container;
            GameObject modalViewObject = container.InstantiatePrefab(viewControllers.Find("GameplaySetupViewController/ColorsOverrideSettings/Settings/Detail/ColorSchemeDropDown/DropdownTableView").gameObject);
            modalViewObject.name = "DynamicOpenVR Modal";

            DestroyImmediate(modalViewObject.GetComponent<TableView>());
            DestroyImmediate(modalViewObject.GetComponent("ScrollRect"));
            DestroyImmediate(modalViewObject.GetComponent<ScrollView>());
            DestroyImmediate(modalViewObject.GetComponent<EventSystemListener>());

            foreach (RectTransform child in modalViewObject.transform)
            {
                if (child.name == "BG")
                {
                    child.anchoredPosition = Vector2.zero;
                    child.sizeDelta = Vector2.zero;
                }
                else
                {
                    Destroy(child.gameObject);
                }
            }

            Transform mainMenuViewControllerTransform = viewControllers.Find("MainMenuViewController");
            MainMenuViewController mainMenuViewController = mainMenuViewControllerTransform.GetComponent<MainMenuViewController>();

            var rectTransform = (RectTransform)modalViewObject.transform;
            rectTransform.anchorMin = Vector2.one * 0.5f;
            rectTransform.anchorMax = Vector2.one * 0.5f;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(110, 65);
            rectTransform.SetParent(mainMenuViewControllerTransform, false);

            Material fontMaterial = Resources.FindObjectsOfTypeAll<Material>().First(m => m.name == "Teko-Medium SDF Curved Softer");
            var textObject = new GameObject("Text");
            CurvedTextMeshPro text = textObject.AddComponent<CurvedTextMeshPro>();
            text.text = "DynamicOpenVR.BeatSaber has created a .vrmanifest file in your game's root folder and would like to permanently register it within SteamVR. " +
                        $"The file has been created at \"{Plugin.kManifestPath}\" and will be added to the global SteamVR app configuration at \"{Plugin.kAppConfigPath}\".\n\n" +
                        "Doing this allows SteamVR to properly recognize that the game is now using the new input system when the game is not running. " +
                        "However, it may cause issues on certain systems. You can opt to skip this temporarily and run the game as-is to confirm that " +
                        "everything works as expected, and you will be prompted with this message again the next time you start the game.\n\n" +
                        "Can DynamicOpenVR.BeatSaber proceed with the changes?";

            text.fontMaterial = fontMaterial;
            text.fontSize = 3;
            text.alignment = TMPro.TextAlignmentOptions.Left;
            text.overflowMode = TMPro.TextOverflowModes.Truncate;
            ((RectTransform)text.transform).sizeDelta = new Vector2(100, 50);
            ((RectTransform)text.transform).anchoredPosition = new Vector2(0, 5);
            textObject.transform.SetParent(rectTransform, false);

            var noButtonTransform = (RectTransform)Instantiate(viewControllers.Find("SettingsViewController/BottomPanel/CancelButton"));
            noButtonTransform.anchorMin = new Vector2(0.5f, 0);
            noButtonTransform.anchorMax = new Vector2(0.5f, 0);
            noButtonTransform.anchoredPosition = new Vector2(-20, 8);
            noButtonTransform.SetParent(rectTransform, false);
            Button noButton = noButtonTransform.GetComponent<Button>();
            noButton.onClick.RemoveAllListeners();
            Transform noButtonTextTransform = noButtonTransform.Find("Content/Text");
            Destroy(noButtonTextTransform.GetComponent("LocalizedTextMeshProUGUI"));
            CurvedTextMeshPro noButtonText = noButtonTextTransform.GetComponent<CurvedTextMeshPro>();
            noButtonText.text = "No";

            var yesButtonTransform = (RectTransform)Instantiate(viewControllers.Find("SettingsViewController/BottomPanel/OkButton"));
            yesButtonTransform.anchorMin = new Vector2(0.5f, 0);
            yesButtonTransform.anchorMax = new Vector2(0.5f, 0);
            yesButtonTransform.anchoredPosition = new Vector2(20, 8);
            yesButtonTransform.SetParent(rectTransform, false);
            Button yesButton = yesButtonTransform.GetComponent<Button>();
            yesButton.onClick.RemoveAllListeners();
            Transform yesButtonTextTransform = yesButtonTransform.Find("Content/Text");
            Destroy(yesButtonTextTransform.GetComponent("LocalizedTextMeshProUGUI"));
            CurvedTextMeshPro yesButtonText = yesButtonTextTransform.GetComponent<CurvedTextMeshPro>();
            yesButtonText.text = "Yes";

            AppConfigConfirmationModal modal = modalViewObject.AddComponent<AppConfigConfirmationModal>();
            modal._modalView = modalViewObject.GetComponent<ModalView>();
            modal._mainMenuViewController = mainMenuViewController;
            modal._appConfig = updatedAppConfig;
            noButton.onClick.AddListener(modal.OnNoButtonClicked);
            yesButton.onClick.AddListener(modal.OnYesButtonClicked);
            mainMenuViewController.didActivateEvent += modal.OnMainMenuViewControllerActivated;

            return modal;
        }

        private void OnMainMenuViewControllerActivated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            _modalView.Show(false);
            _mainMenuViewController.didActivateEvent -= OnMainMenuViewControllerActivated;
        }

        private void OnYesButtonClicked()
        {
            _modalView.Hide(true);
            WriteAppConfig(Plugin.kAppConfigPath, _appConfig);
        }

        private void OnNoButtonClicked()
        {
            _modalView.Hide(true);
        }

        private void WriteAppConfig(string configPath, JObject appConfig)
        {
            using (var writer = new StreamWriter(configPath))
            {
                writer.Write(JsonConvert.SerializeObject(appConfig, Formatting.Indented));
            }
        }
    }
}
