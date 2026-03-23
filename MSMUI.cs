using Il2Cpp;
using MelonLoader;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Il2Cpp.CommonButtonHandler;

//*****
// This file contains all the backend of the mod, and builds everything UI, using the methods from Utilities
// This includes setting up the mod menu, and calling the delayed builders to populate it
//*****

namespace ModSettingsMenu
{
    public static class MSMUI
    {
        private static Transform _modsPanel;
        private static Transform _settingsPanel;
        private static Transform _modsListRoot;
        private static Transform _settingsListRoot;
        private static Transform _leftPanel;
        private static Transform _rightPanel;

        //*****
        // Class initialization, adds the button to access the MSM
        //*****
        public static void SettingsMenuInit(OptionsPopup popup)
        {
            AddModSettingsButton(popup);
        }

        //*****
        // Adds the button to open the MSM to the OptionsPopup
        //*****
        public static void AddModSettingsButton(OptionsPopup popup)
        {
            Utilities.BuildButton(popup.ExportLogButton.transform.parent, "Mod Settings Menu", (Action)(() =>
            {
                CreateModSettingsWindow(popup);
                BuildModlist();
                popup.gameObject.SetActive(false);
            }), CommonButtonColorType.Alternative, CommonButtonHandler.CommonButtonSizeEnum.Smaller);
        }

        //*****
        // This clones OptionsPopup, cleans it, creating the MSM Popup and its expected behavior
        //*****
        public static void CreateModSettingsWindow(OptionsPopup originalPopup)
        {
            if (originalPopup == null)
            {
                MelonLogger.Warning("Original OptionsPopup not provided!");
                return;
            }

            // Clone the original window
            var _modWindow = UnityEngine.Object.Instantiate(originalPopup.gameObject, originalPopup.transform.parent);
            _modWindow.name = "ModSettingsWindow";

            // Clear its content while keeping layout
            _modsPanel = _modWindow.transform.Find("Area 1");
            _modsPanel.SetName("Mods Panel");
            _settingsPanel = _modWindow.transform.Find("Area 2");
            _settingsPanel.SetName("Settings Panel");
            Utilities.ClearChildren(_modsPanel);
            Utilities.ClearChildren(_settingsPanel);
            Utilities.SetChildText(_modWindow, "Title", "Mod Settings Menu");

            var closeBtn = _modWindow.transform.Find("Close Button").GetComponent<BetterButton>();
            closeBtn.onClick.RemoveAllListeners();
            closeBtn.onClick.AddListener((Action)(() =>
            {
                // Close our window and reopen the options popup
                UnityEngine.Object.Destroy(_modWindow);
                originalPopup.gameObject.SetActive(true);
            }));
            
            // Modify the window's size & Setting Panel
            var modWindowRT = _modWindow.GetComponent<RectTransform>();
            modWindowRT.offsetMax = new Vector2(750f, 400f);
            modWindowRT.offsetMin = new Vector2(-750f, -400f);
            

            _settingsPanel.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(60, -10);
            
            // Build the ScrollLists
            Utilities.BuildScrollList(_modsPanel, "ModsScrollList", out _modsListRoot, 740, 300);

            VerticalLayoutGroup vlg = _modsPanel.GetComponent<VerticalLayoutGroup>();
            vlg.childControlWidth = false;
            vlg.childForceExpandWidth = false;
            vlg.childControlHeight = false;
            vlg.childForceExpandHeight = false;
            vlg.spacing = 10f;
            vlg.childAlignment = TextAnchor.MiddleLeft;
            
            GameObject settingsScrollList = Utilities.BuildScrollList(_settingsPanel, "SettingsScrollList", out _settingsListRoot, 740, 1130);
            
            vlg = _settingsListRoot.GetComponent<VerticalLayoutGroup>();
            vlg.childControlWidth = false;
            vlg.childForceExpandWidth = false;
            vlg.childControlHeight = false;
            vlg.childForceExpandHeight = false;
            vlg.spacing = 10f;
            vlg.childAlignment = TextAnchor.MiddleLeft;
            vlg.padding.left = 10;   // left padding
            vlg.padding.right = 0;
            vlg.padding.top = 5;     // top padding
            vlg.padding.bottom = 0;

            // *** Splitting the Settings List into 2 panels ***
            // Adds a child with horizontal layout to Holder, adds two new "holders" under it with vertical layout

            // Make sure the content root controls height but NOT width
            VerticalLayoutGroup rootVLG = _settingsListRoot.GetComponent<VerticalLayoutGroup>();
            rootVLG.childControlWidth = true;
            rootVLG.childForceExpandWidth = true;
            rootVLG.childControlHeight = false;
            rootVLG.childForceExpandHeight = false;

            // Create ColumnsRoot inside content root
            GameObject columnsRoot = new GameObject("ColumnsRoot");
            UnityEngine.Object.Destroy(columnsRoot.transform);
            RectTransform columnsRT = columnsRoot.AddComponent<RectTransform>();
            columnsRoot.transform.SetParent(_settingsListRoot, false);

            // Stretch horizontally, stick to top
            columnsRT.anchorMin = new Vector2(0, 1);
            columnsRT.anchorMax = new Vector2(1, 1);
            columnsRT.pivot = new Vector2(0.5f, 1);
            columnsRT.anchoredPosition = Vector2.zero;
            columnsRT.sizeDelta = new Vector2(0, 0);

            // Horizontal layout to split left/right
            HorizontalLayoutGroup hlg = columnsRoot.AddComponent<HorizontalLayoutGroup>();
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = false;
            hlg.childAlignment = TextAnchor.UpperLeft;
            hlg.spacing = 20;
            hlg.padding.left = 10;
            hlg.padding.right = 10;

            // Ensure ColumnsRoot participates in vertical sizing
            ContentSizeFitter columnsCSF = columnsRoot.AddComponent<ContentSizeFitter>();
            columnsCSF.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            columnsCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Left Panel
            GameObject leftPanelGO = new GameObject("Left Panel");
            UnityEngine.Object.Destroy(leftPanelGO.transform);
            RectTransform leftRT = leftPanelGO.AddComponent<RectTransform>();
            leftPanelGO.transform.SetParent(columnsRoot.transform, false);

            VerticalLayoutGroup leftVLG = leftPanelGO.AddComponent<VerticalLayoutGroup>();
            leftVLG.childControlWidth = false;
            leftVLG.childForceExpandWidth = false;
            leftVLG.childControlHeight = false;
            leftVLG.childForceExpandHeight = false;
            leftVLG.childAlignment = TextAnchor.UpperLeft;
            leftVLG.spacing = 10;

            LayoutElement leftLE = leftPanelGO.AddComponent<LayoutElement>();
            leftLE.flexibleWidth = 1;

            _leftPanel = leftPanelGO.transform;


            // Right Panel
            GameObject rightPanelGO = new GameObject("Right Panel");
            UnityEngine.Object.Destroy(rightPanelGO.transform);
            RectTransform rightRT = rightPanelGO.AddComponent<RectTransform>();
            rightPanelGO.transform.SetParent(columnsRoot.transform, false);

            VerticalLayoutGroup rightVLG = rightPanelGO.AddComponent<VerticalLayoutGroup>();
            rightVLG.childControlWidth = false;
            rightVLG.childForceExpandWidth = false;
            rightVLG.childControlHeight = false;
            rightVLG.childForceExpandHeight = false;
            rightVLG.childAlignment = TextAnchor.UpperLeft;
            rightVLG.spacing = 10;

            LayoutElement rightLE = rightPanelGO.AddComponent<LayoutElement>();
            rightLE.flexibleWidth = 1;

            _rightPanel = rightPanelGO.transform;

            // Add some placeholder text
            Utilities.BuildLabel(_leftPanel, "PlaceholderTitle", "Mod Settings Menu", Utilities.FontSize.Large);
            Utilities.BuildLabel(_rightPanel, "PlaceholderCredit", "by Echo", Utilities.FontSize.Small);
            Utilities.BuildPadding(_leftPanel, 100);
            Utilities.BuildLabel(_leftPanel, "PlaceholderText", "Click on a mod on the left side to display its settings.", Utilities.FontSize.Small);

            _modWindow.SetActive(true);
        }

        //*****
        // Refreshes the mod's settings, updating the UI
        //*****
        public static void RefreshDisplay()
        {
            _settingsPanel.gameObject.SetActive(false);
            _settingsPanel.gameObject.SetActive(true);
        }


        //******************************************
        //         Delayed builders logic
        //******************************************

        //*****
        // Delayed builder for ModList
        //*****
        public static void BuildModlist()
        {
            foreach (var mod in MSM._mods.Values
                .OrderBy(m => m.DisplayName, StringComparer.OrdinalIgnoreCase))
            {
                try
                {
                    Utilities.BuildButton(_modsListRoot, mod.DisplayName, (Action)(() =>
                    {
                        Utilities.ClearChildren(_leftPanel);
                        Utilities.ClearChildren(_rightPanel);
                        Utilities.BuildLabel(_leftPanel, "Mod Title", mod.DisplayName, Utilities.FontSize.Large);
                        Utilities.BuildPadding(_leftPanel, 0);
                        Utilities.BuildLabel(_rightPanel, "Empty Space", " ", Utilities.FontSize.Large);
                        Utilities.BuildPadding(_rightPanel, 0);
                        BuildModSettings(mod.ModId);
                    }), CommonButtonColorType.Alternative);
                }
                catch (Exception ex)
                {
                    MelonLogger.Error("Deferred builder error: " + ex);
                }
            }
        }

        //*****
        // Delayed builder for Mod Settings
        //*****
        public static void BuildModSettings(string modid)
        {
            int settingID = 0;
            Transform panel;

            foreach (var setting in MSM._mods[modid].Settings)
            {
                try
                {
                    if (setting.Panel == PanelSide.LeftPanel)
                        panel = _leftPanel;
                    else panel = _rightPanel;

                    switch (setting.Type)
                    {
                        case SettingType.Label:
                            Utilities.BuildLabel(panel, modid+settingID+"Label", setting.Label, setting.LabelSize);
                            break;

                        case SettingType.Button:
                            Utilities.BuildButton(panel, setting.Label, setting.ButtonAction, BtnColorConvert(setting.ButtonColor), BtnSizeConvert(setting.ButtonSize));
                            break;

                        case SettingType.Checkbox:
                            Utilities.BuildToggle(panel, modid+settingID+"Checkbox", setting.Label, setting.StartValueB, setting.CheckboxAction);
                            break;

                        case SettingType.Slider:
                            Utilities.BuildLabel(panel, modid+settingID +"Label", setting.Label, Utilities.FontSize.Small);
                            Utilities.BuildScrollbar(panel, modid+settingID+"Slider", setting.Min, setting.Max, setting.StartValueF, setting.Steps, setting.SliderAction);
                            break;
                        case SettingType.Padding:
                            Utilities.BuildPadding(panel, 30);
                            break;
                        case SettingType.Dropdown:
                            Utilities.BuildLabel(panel, modid + settingID + "Label", setting.Label, Utilities.FontSize.Small);
                            Utilities.BuildDropdown(panel, modid + settingID + "Dropdown", setting.DropdownOptions, setting.DropdownIndex, setting.DropdownAction);
                            break;
                    }
                    settingID++;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error("Deferred builder error: " + ex);
                }
            }
        }

        // Converts enums to Il2Cpp, for ButtonColor
        internal static CommonButtonColorType BtnColorConvert(ButtonColor color)
        {
            try
            {
                switch (color)
                {
                    case ButtonColor.Default:
                        return (CommonButtonColorType.Normal);

                    case ButtonColor.Blue:
                        return (CommonButtonColorType.Alternative);

                    case ButtonColor.Red:
                        return (CommonButtonColorType.Red);

                    case ButtonColor.Green:
                        return (CommonButtonColorType.Green);
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error("Deferred builder error: " + ex);
            }
            // Fallback return if something unexpected happens
            return CommonButtonColorType.Alternative;
        }

        // Converts enums to Il2Cpp, for ButtonSize
        internal static CommonButtonSizeEnum BtnSizeConvert(ButtonSize size)
        {
            try
            {
                switch (size)
                {
                    case ButtonSize.Normal:
                        return (CommonButtonSizeEnum.Normal);

                    case ButtonSize.Smaller:
                        return (CommonButtonSizeEnum.Smaller);
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error("Deferred builder error: " + ex);
            }
            // Fallback return if something unexpected happens
            return CommonButtonSizeEnum.Normal;
        }
    }
}
