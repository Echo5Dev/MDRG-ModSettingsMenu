using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Reflection;

[assembly: MelonInfo(typeof(ModSettingsMenu.MSM), "ModSettingsMenu", "1.0.0", "Echo")]
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
        public static void AddLabel(string modId, PanelSide side, string text, Utilities.FontSize fontSize = Utilities.FontSize.Medium)
        {
            if (!_mods.TryGetValue(modId, out var mod))
            {
                MelonLogger.Error($"MSM: Mod '{modId}' not registered.");
                return;
            }
            _mods[modId].Settings.Add(new SettingDefinition(side, SettingType.Label, text, fontSize));
        }

        //*****
        // Adds a new Button with the given parameters to the given mod
        //*****
        public static void AddButton(string modId, PanelSide side, string label, Action onClick, ButtonColor buttonColor = ButtonColor.Blue, ButtonSize buttonSize = ButtonSize.Normal)
        {
            if (!_mods.TryGetValue(modId, out var mod))
            {
                MelonLogger.Error($"MSM: Mod '{modId}' not registered.");
                return;
            }
            _mods[modId].Settings.Add(new SettingDefinition(side, SettingType.Button, label, onClick, buttonColor, buttonSize));
        }

        //*****
        // Adds a new Checkbox with the given parameters to the given mod
        //*****
        public static void AddCheckbox(string modId, PanelSide side, string label, bool startValue, Action<bool> onClick)
        {
            if (!_mods.TryGetValue(modId, out var mod))
            {
                MelonLogger.Error($"MSM: Mod '{modId}' not registered.");
                return;
            }
            _mods[modId].Settings.Add(new SettingDefinition(side, SettingType.Checkbox, label, startValue, onClick));
        }

        //*****
        // Adds a new Slider (float) with the given parameters to the given mod
        //*****
        public static void AddSlider(string modId, PanelSide side, string label, float min, float max, float startValue, int steps, Action<float> onChanged)
        {
            if (!_mods.TryGetValue(modId, out var mod))
            {
                MelonLogger.Error($"MSM: Mod '{modId}' not registered.");
                return;
            }
            _mods[modId].Settings.Add(new SettingDefinition(side, SettingType.Slider, label, min, max, startValue, steps, onChanged));
        }

        //*****
        // Adds a new Slider (int) with the given parameters to the given mod
        //*****
        public static void AddSlider(string modId, PanelSide side, string label, int min, int max, int startValue, Action<int> onChanged)
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

            _mods[modId].Settings.Add(new SettingDefinition(side, SettingType.Slider, label, (float)min, (float)max, (float)startValue, steps, wrappedAction));
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
        public static void AddDropdown(string modId, PanelSide side, string label, List<string> options, string defaultValue, Action<string> onChanged)
        {
            if (!_mods.TryGetValue(modId, out var mod))
            {
                MelonLogger.Error($"MSM: Mod '{modId}' not registered.");
                return;
            }

            int index = options.IndexOf(defaultValue);
            if (index < 0) index = 0;

            _mods[modId].Settings.Add(new SettingDefinition(side, SettingType.Dropdown, label, options, index, onChanged));
        }

        //*****
        // Refreshes the mod's settings, updating the UI with potentially changed values
        // Usefull in the case of a "Restore defaults" button
        //*****
        public static void RefreshDisplay()
        {
            MSMUI.RefreshDisplay();
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
        public bool StartValueB;

        // Sliders stuff
        public Action<float> SliderAction;
        public float Min;
        public float Max;
        public float StartValueF;
        public int Steps;

        // Dropdowns stuff
        public Action<string> DropdownAction;
        public List<string> DropdownOptions;
        public int DropdownIndex;


        //ctor Label
        public SettingDefinition(PanelSide panel, SettingType type, string label, Utilities.FontSize labelSize)
        {
            Panel = panel;
            Type = type;
            Label = label;
            LabelSize = labelSize;
        }

        //ctor Button
        public SettingDefinition(PanelSide panel, SettingType type, string label, Action onClick, ButtonColor buttonColor, ButtonSize buttonSize)
        {
            Panel = panel;
            Type = type;
            Label = label;
            ButtonAction = onClick;
            ButtonColor = buttonColor;
            ButtonSize = buttonSize;
        }

        //ctor Checkbox
        public SettingDefinition(PanelSide panel, SettingType type, string label, bool startValue, Action<bool> onClick)
        {
            Panel = panel;
            Type = type;
            Label = label;
            CheckboxAction = onClick;
            StartValueB = startValue;
        }

        //ctor Slider
        public SettingDefinition(PanelSide panel, SettingType type, string label, float min, float max, float startValue, int steps, Action<float> onChanged)
        {
            Panel = panel;
            Type = type;
            Label = label;
            Min = min;
            Max = max;
            StartValueF = startValue;
            Steps = steps;
            SliderAction = onChanged;
        }

        //ctor Padding
        public SettingDefinition(PanelSide panel, SettingType type)
        {
            Panel = panel;
            Type = type;
        }

        //ctor Dropdown
        public SettingDefinition(PanelSide panel, SettingType type, string label, List<string> options, int defaultIndex, Action<string> onChanged)
        {
            Panel = panel;
            Type = type;
            Label = label;
            DropdownOptions = options;
            DropdownIndex = defaultIndex;
            DropdownAction = onChanged;
        }
    }
}
