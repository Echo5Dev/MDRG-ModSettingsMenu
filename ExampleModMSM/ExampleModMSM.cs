using MelonLoader;
using ModSettingsMenu;
using System;
using System.Collections.Generic;

[assembly: MelonInfo(typeof(MDRG_MSMExampleMod.MSMExampleMod), "ExampleModMSM", "1.0.0", "Echo")]
[assembly: MelonGame("IncontinentCell", "My Dystopian Robot Girlfriend")]

namespace MDRG_MSMExampleMod
{
    public class MSMExampleMod : MelonMod
    {
        private static MelonPreferences_Category _exampleCategory;
        private static MelonPreferences_Entry<bool> _exampleCheckboxSetting1;
        private static MelonPreferences_Entry<bool> _exampleCheckboxSetting2;
        private static MelonPreferences_Entry<int> _exampleSliderSetting1;
        private static MelonPreferences_Entry<int> _exampleSliderSetting2;
        private static MelonPreferences_Entry<string> _exampleDropdownSetting;

        public override void OnInitializeMelon()
        {
            // Load settings
            _exampleCategory = MelonPreferences.CreateCategory("MSM Example Mod");
            _exampleCheckboxSetting1 = _exampleCategory.CreateEntry("CheckboxSetting1", true);
            _exampleCheckboxSetting2 = _exampleCategory.CreateEntry("CheckboxSetting2", true);
            _exampleSliderSetting1 = _exampleCategory.CreateEntry("SliderSetting1", 5);
            _exampleSliderSetting2 = _exampleCategory.CreateEntry("SliderSetting2", 50);
            _exampleDropdownSetting = _exampleCategory.CreateEntry("DropdownSetting", "Option A");
            _exampleCategory.SaveToFile(true);

            // Initialize MSM
            InitMSM();
        }

        public static void InitMSM()
        {
            string modId = "exampleMod";
            var left = PanelSide.LeftPanel;
            var right = PanelSide.RightPanel;

            // Some actions to save a checkbox setting
            Action<bool> exampleToggleAction1 = (Action<bool>)(value =>
            {
                _exampleCheckboxSetting1.Value = value;
                _exampleCategory.SaveToFile(true);
            });

            Action<bool> exampleToggleAction2 = (Action<bool>)(value =>
            {
                _exampleCheckboxSetting2.Value = value;
                _exampleCategory.SaveToFile(true);
            });

            // A dummy action for the buttons to write text in the console
            Action exampleButtonAction = (Action)(() => { MelonLogger.Msg("Button clicked!"); });

            // Some actions to save a slider setting
            Action<int> exampleSliderAction1 = (Action<int>)(value =>
            { 
                MelonLogger.Msg("Preset: " + value);
                _exampleSliderSetting1.Value = value;
                _exampleCategory.SaveToFile(true);
            });
            Action<int> exampleSliderAction2 = (Action<int>)(value =>
            {
                MelonLogger.Msg("Preset: " + value);
                _exampleSliderSetting2.Value = value;
                _exampleCategory.SaveToFile(true);
            });

            // Dropdown action to save setting & list of options
            Action<string> exampleDropdownAction = (Action<string>)(value =>
            {
                MelonLogger.Msg("Dropdown option changed: " + value);
                _exampleDropdownSetting.Value = value;
                _exampleCategory.SaveToFile(true);
            });
            List<string> dropdownOptions = new List<string> { "Option A", "Option B", "Option C" };

            // Registering the mod
            MSM.RegisterMod(modId, "Example Mod");

            // Building the settings
            MSM.AddLabel(modId, left, "Settings Category A");
            MSM.AddCheckbox(modId, left, "Checkbox Setting A", () => _exampleCheckboxSetting1.Value, exampleToggleAction1);
            MSM.AddCheckbox(modId, left, "Checkbox Setting B", () => _exampleCheckboxSetting2.Value, exampleToggleAction2);
            MSM.AddPadding(modId, right);
            MSM.AddCheckbox(modId, right, "Checkbox Setting C", () => _exampleCheckboxSetting2.Value, exampleToggleAction2);
            MSM.AddCheckbox(modId, right, "Checkbox Setting D", () => _exampleCheckboxSetting1.Value, exampleToggleAction1);

            MSM.AddPadding(modId, left);
            MSM.AddLabel(modId, left, "Settings Category B");
            MSM.AddSlider(modId, left, "Slider Setting A", 0, 10, () => _exampleSliderSetting1.Value, exampleSliderAction1);
            MSM.AddSlider(modId, left, "Slider Setting B", 0, 100, () => _exampleSliderSetting2.Value, exampleSliderAction1);
            MSM.AddPadding(modId, right);
            MSM.AddPadding(modId, right);
            MSM.AddSlider(modId, right, "Slider Setting C", 0, 10, () => _exampleSliderSetting1.Value, exampleSliderAction2);
            MSM.AddSlider(modId, right, "Slider Setting D", 0, 100, () => _exampleSliderSetting2.Value, exampleSliderAction2);

            MSM.AddPadding(modId, left);
            MSM.AddLabel(modId, left, "Button's explanation", Utilities.FontSize.Small);
            MSM.AddButton(modId, left, "Some Button", exampleButtonAction, ButtonColor.Blue);
            MSM.AddPadding(modId, right);
            MSM.AddLabel(modId, right, "Other button's explanation", Utilities.FontSize.Small);
            MSM.AddButton(modId, right, "Some Small Red Button", exampleButtonAction, ButtonColor.Red, ButtonSize.Smaller);

            MSM.AddPadding(modId, left);
            MSM.AddDropdown(modId, left, "Dropdown Setting", dropdownOptions, () => _exampleDropdownSetting.Value, exampleDropdownAction);

            MSM.AddPadding(modId, left);
            MSM.AddLabel(modId, left, "Some disclaimer or explanation about something", Utilities.FontSize.VerySmall);
        }
    }
}
