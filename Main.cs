using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Reflection;

[assembly: MelonInfo(typeof(ModSettingsMenu.MSM), "ModSettingsMenu", "1.1.0", "Echo")]
[assembly: MelonGame("IncontinentCell", "My Dystopian Robot Girlfriend")]
[assembly: MelonPriority(int.MinValue)]

//*****
// This file contains the Melon mod itself, the Harmony patching, and anything API for the other mods to use
// The full list of "commands" should be in the guide
//*****

namespace ModSettingsMenu
{
    public class MSM : MelonMod
    {
        public static MSM Instance { get; private set; }
        public static readonly Dictionary<string, ModEntry> _mods = new Dictionary<string, ModEntry>();
        private HarmonyLib.Harmony _harmony;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("ModSettingsMenu initializing...");
            Instance = this;
            _harmony = new HarmonyLib.Harmony("ModSettingsMenu");

            // Manual patches to avoid duplication because of Il2Cpp
            // PatchAll() sees the same methods twice
            TryPatchMainMenu();
            TryPatchOptionsPopup();

            MelonLogger.Msg("ModSettingsMenu loaded.");
        }

        //*****
        // Patch injector at the Main Menu.
        //*****
        private void TryPatchMainMenu()
        {
            var target = typeof(MainMenuScene).GetMethod("Prepare", BindingFlags.Instance | BindingFlags.Public);
            var postfix = AccessTools.Method(typeof(PatchMainMenu), nameof(PatchMainMenu.Postfix));
            // Only patch if not already patched
            var patches = HarmonyLib.Harmony.GetPatchInfo(target);
            _harmony.Patch(target, postfix: new HarmonyMethod(postfix));
        }

        //*****
        // Patch for anything to run at startup.
        //*****
        public static class PatchMainMenu
        {
            static bool _initialized = false;
            public static void Postfix()
            {
                if (!_initialized)
                {
                    Utilities.InitializeTemplates();
                    _initialized = true;
                }

            }
        }

        //*****
        // Patch injector for the mod settings menu.
        //*****
        private void TryPatchOptionsPopup()
        {
            try
            {
                var target = typeof(OptionsPopup).GetMethod("InitRemaining", BindingFlags.Instance | BindingFlags.Public);
                var postfix = AccessTools.Method(typeof(SettingsInjectPatch), nameof(SettingsInjectPatch.Postfix));

                // Only patch if not already patched
                var patches = HarmonyLib.Harmony.GetPatchInfo(target);
                _harmony.Patch(target, postfix: new HarmonyMethod(postfix));

                MelonLogger.Msg("[ModSettingsMenu] Successfully patched OptionsPopup.InitRemaining.");
            }
            catch (Exception ex)
            {
                MelonLogger.Error("[ModSettingsMenu] OptionsPopup patch error: " + ex);
            }
        }

        //*****
        // Patch for the mod settings menu.
        //*****
        public static class SettingsInjectPatch
        {
            public static void Postfix(OptionsPopup __instance)
            {
                MSMUI.SettingsMenuInit(__instance);
            }
        }

        //******************************************
        //           API Stuff: commands
        //******************************************

        //*****
        // Registers a new mod, with a given ID and display name, stores it in the _mods dictionary
        //*****
        public static void RegisterMod(string modId, string displayName)
        {
            _mods[modId] = (new ModEntry(modId, displayName));
        }

        //*****
        // Adds a new Label with the given parameters to the given mod
        //*****
        public static void AddLabel(string modId, PanelSide side, string text, Utilities.FontSize fontSize = Utilities.FontSize.Medium, string tooltip = "")
        {
            if (!_mods.TryGetValue(modId, out var mod))
            {
                MelonLogger.Error($"MSM: Mod '{modId}' not registered.");
                return;
            }
            _mods[modId].Settings.Add(new SettingDefinition(side, SettingType.Label, text, fontSize, tooltip));
        }

        //*****
        // Adds a new Button with the given parameters to the given mod
        //*****
        public static void AddButton(string modId, PanelSide side, string label, Action onClick, ButtonColor buttonColor = ButtonColor.Blue, ButtonSize buttonSize = ButtonSize.Normal, string tooltip = "")
        {
            if (!_mods.TryGetValue(modId, out var mod))
            {
                MelonLogger.Error($"MSM: Mod '{modId}' not registered.");
                return;
            }
            _mods[modId].Settings.Add(new SettingDefinition(side, SettingType.Button, label, onClick, buttonColor, buttonSize, tooltip));
        }

        //*****
        // Adds a new Checkbox with the given parameters to the given mod
        //*****
        public static void AddCheckbox(string modId, PanelSide side, string label, Func<bool> startValue, Action<bool> onClick, string tooltip = "")
        {
            if (!_mods.TryGetValue(modId, out var mod))
            {
                MelonLogger.Error($"MSM: Mod '{modId}' not registered.");
                return;
            }
            _mods[modId].Settings.Add(new SettingDefinition(side, SettingType.Checkbox, label, startValue, onClick, tooltip));
        }

        //*****
        // Adds a new Slider (float) with the given parameters to the given mod
        //*****
        public static void AddSlider(string modId, PanelSide side, string label, float min, float max, Func<float> startValue, int steps, Action<float> onChanged, string tooltip = "")
        {
            if (!_mods.TryGetValue(modId, out var mod))
            {
                MelonLogger.Error($"MSM: Mod '{modId}' not registered.");
                return;
            }
            _mods[modId].Settings.Add(new SettingDefinition(side, SettingType.Slider, label, min, max, startValue, steps, onChanged, tooltip));
        }

        //*****
        // Adds a new Slider (int) with the given parameters to the given mod
        //*****
        public static void AddSlider(string modId, PanelSide side, string label, int min, int max, Func<int> startValue, Action<int> onChanged, string tooltip = "")
        {
            if (!_mods.TryGetValue(modId, out var mod))
            {
                MelonLogger.Error($"MSM: Mod '{modId}' not registered.");
                return;
            }

            int steps = (max - min) + 1;

            Action<float> wrappedAction = value =>
            {
                int intValue = Mathf.RoundToInt(value);
                onChanged(intValue);
            };

            _mods[modId].Settings.Add(new SettingDefinition(side, SettingType.Slider, label, (float)min, (float)max, () => (float)startValue(), steps, wrappedAction, tooltip));
        }

        //*****
        // Adds a new Padding with the given parameters to the given mod
        //*****
        public static void AddPadding(string modId, PanelSide side)
        {
            if (!_mods.TryGetValue(modId, out var mod))
            {
                MelonLogger.Error($"MSM: Mod '{modId}' not registered.");
                return;
            }
            _mods[modId].Settings.Add(new SettingDefinition(side, SettingType.Label));
        }

        //*****
        // Adds a new Dropdown with the given parameters to the given mod
        //*****
        public static void AddDropdown(string modId, PanelSide side, string label, List<string> options, Func<string> defaultValue, Action<string> onChanged, string tooltip = "")
        {
            if (!_mods.TryGetValue(modId, out var mod))
            {
                MelonLogger.Error($"MSM: Mod '{modId}' not registered.");
                return;
            }

            Func<int> getIndex = () =>
            {
                int idx = options.IndexOf(defaultValue());
                return idx >= 0 ? idx : 0;
            };

            _mods[modId].Settings.Add(new SettingDefinition(side, SettingType.Dropdown, label, options, getIndex, onChanged, tooltip));
        }

        //*****
        // Refreshes the UI
        //*****
        public static void RefreshSettings(string modId)
        {
            MSMUI.RebuildWindow(modId);
        }

    }

    //******************************************
    //           API Stuff: structures
    //******************************************

    //*****
    // Class that contains everything relative to a registered mod
    // To be stored in the _mods dictionary
    //*****
    public class ModEntry
    {
        public string ModId;
        public string DisplayName;
        public List<SettingDefinition> Settings = new List<SettingDefinition>();

        public ModEntry(string modId, string displayName)
        {
            ModId = modId;
            DisplayName = displayName;
        }
    }

    public enum SettingType
    {
        Label,
        Button,
        Checkbox,
        Slider,
        Padding,
        Dropdown
    }

    public enum PanelSide
    {
        LeftPanel,
        RightPanel
    }

    public enum ButtonColor
    {
        Default,
        Blue,
        Red,
        Green
    }

    public enum ButtonSize
    {
        Normal,
        Smaller
    }

    //*****
    // Class that contains everything relative to a setting UI element
    // To be stored in the ModEntry's Settings list
    //*****
    public class SettingDefinition
    {
        public SettingType Type;    // Type of Setting UI element
        public PanelSide Panel;     // On which side of the Mod's settings menu this element should appear

        public string Tooltip;

        // *** Optional fields depending on Type
        // Labels stuff
        public string Label;
        public Utilities.FontSize LabelSize;

        // Buttons stuff
        public Action ButtonAction;
        public ButtonColor ButtonColor;
        public ButtonSize ButtonSize;

        // Checkboxes stuff
        public Action<bool> CheckboxAction;
        public Func<bool> StartValueB;

        // Sliders stuff
        public Action<float> SliderAction;
        public float Min;
        public float Max;
        public Func<float> StartValueF;
        public int Steps;

        // Dropdowns stuff
        public Action<string> DropdownAction;
        public List<string> DropdownOptions;
        public Func<int> DropdownIndex;


        //ctor Label
        public SettingDefinition(PanelSide panel, SettingType type, string label, Utilities.FontSize labelSize, string tooltip)
        {
            Panel = panel;
            Type = type;
            Label = label;
            LabelSize = labelSize;
            Tooltip = tooltip;
        }

        //ctor Button
        public SettingDefinition(PanelSide panel, SettingType type, string label, Action onClick, ButtonColor buttonColor, ButtonSize buttonSize, string tooltip)
        {
            Panel = panel;
            Type = type;
            Label = label;
            ButtonAction = onClick;
            ButtonColor = buttonColor;
            ButtonSize = buttonSize;
            Tooltip = tooltip;
        }

        //ctor Checkbox
        public SettingDefinition(PanelSide panel, SettingType type, string label, Func<bool> startValue, Action<bool> onClick, string tooltip)
        {
            Panel = panel;
            Type = type;
            Label = label;
            CheckboxAction = onClick;
            StartValueB = startValue;
            Tooltip = tooltip;
        }

        //ctor Slider
        public SettingDefinition(PanelSide panel, SettingType type, string label, float min, float max, Func<float> startValue, int steps, Action<float> onChanged, string tooltip)
        {
            Panel = panel;
            Type = type;
            Label = label;
            Min = min;
            Max = max;
            StartValueF = startValue;
            Steps = steps;
            SliderAction = onChanged;
            Tooltip = tooltip;
        }

        //ctor Dropdown
        public SettingDefinition(PanelSide panel, SettingType type, string label, List<string> options, Func<int> defaultIndex, Action<string> onChanged, string tooltip)
        {
            Panel = panel;
            Type = type;
            Label = label;
            DropdownOptions = options;
            DropdownIndex = defaultIndex;
            DropdownAction = onChanged;
            Tooltip = tooltip;
        }

        //ctor Padding
        public SettingDefinition(PanelSide panel, SettingType type)
        {
            Panel = panel;
            Type = type;
        }
    }
}
