using MelonLoader;
using ModSettingsMenu;
using System;
using System.Collections.Generic;

[assembly: MelonInfo(typeof(ExampleModMSM.ExampleModMSM), "ExampleModMSM", "1.0.0", "Echo")]
[assembly: MelonGame("IncontinentCell", "My Dystopian Robot Girlfriend")]

namespace ExampleModMSM
{
    public class ExampleModMSM : MelonMod
    {
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("ExampleModMSM initializing...");

            ExampleModA.InitMod();
            ExampleModB.InitMod();

            MelonLogger.Msg("ExampleModMSM loaded.");
        }
    }

    /* ####################
     * #### Quick example on how to use an Action to change your MelonPreferences ####
     * ####################
     * 
        *****
        // MelonLoader stored Settings
        *****

        // private static MelonPreferences_Category _exampleCategory;
        // private static MelonPreferences_Entry<bool> _exampleSetting;

        public static void Load()
        {
            _exampleCategory = MelonPreferences.CreateCategory("CocktractsShowRep");

            _exampleSetting = _exampleCategory.CreateEntry("Enabled", true, "Enable Module");

            _exampleCategory.SaveToFile(true);
        }

        *****
        // Setter Logic 
        *****

        public static void SetEnabled(bool value)
        {
            _exampleSetting.Value = value;
            _exampleCategory.SaveToFile(true);
        }

        *****
        // Action Logic 
        *****
        
        Action<bool> dummyToggleAction = (Action<bool>)(value => { SetEnabled(value); });
      *
      */

    class ExampleModA
    {
        public static void InitMod()
        {
            string modId = "exampleModA";
            var left = PanelSide.LeftPanel;
            var right = PanelSide.RightPanel;

            Action<bool> dummyToggleAction = (Action<bool>)(value => { MelonLogger.Msg("Toggle clicked!"); });
            Action<int> dummySliderAction = (Action<int>)(value => { MelonLogger.Msg("Preset: " + value); });
            Action dummyButtonAction = (Action)(() => { });

            MSM.RegisterMod(modId, "Example Mod A");
            MSM.AddLabel(modId, left, "Settings Category A");
            MSM.AddCheckbox(modId, left, "Checkbox Setting A", false, dummyToggleAction);
            MSM.AddCheckbox(modId, left, "Checkbox Setting B", false, dummyToggleAction);

            MSM.AddPadding(modId, left);
            MSM.AddLabel(modId, left, "Settings Category B");
            MSM.AddSlider(modId, left, "Slider Setting A", 0, 4, 2, dummySliderAction);
            MSM.AddSlider(modId, left, "Slider Setting B", 0, 100, 25, dummySliderAction);

            MSM.AddPadding(modId, left);
            MSM.AddButton(modId, left, "Some Button", dummyButtonAction, ButtonColor.Blue);
            MSM.AddButton(modId, left, "Some Small Red Button", dummyButtonAction, ButtonColor.Red, ButtonSize.Smaller);
        }
    }

    class ExampleModB
    {
        public static void InitMod()
        {
            string modId = "exampleModB";
            var left = PanelSide.LeftPanel;
            var right = PanelSide.RightPanel;

            Action<bool> dummyToggleAction = (Action<bool>)(value => { MelonLogger.Msg("Toggle clicked!"); });

            Action dummyButtonAction = (Action)(() => { });

            Action<int> dummySliderAction = (Action<int>)(value => { MelonLogger.Msg("Preset: " + value); });

            Action<string> dummyDropdownAction = (Action<string>)(value => { MelonLogger.Msg("Dropdown option changed: " + value); });
            List<string> dropdownOptions = new List<string> { "Option A", "Option B", "Option C" };
            string dropdownDefault = dropdownOptions[0];

            MSM.RegisterMod(modId, "Example Mod B");
            MSM.AddLabel(modId, left, "Settings Category A");
            MSM.AddCheckbox(modId, left, "Checkbox Setting A", true, dummyToggleAction);
            MSM.AddCheckbox(modId, left, "Checkbox Setting B", false, dummyToggleAction);
            MSM.AddPadding(modId, right);
            MSM.AddCheckbox(modId, right, "Checkbox Setting C", false, dummyToggleAction);
            MSM.AddCheckbox(modId, right, "Checkbox Setting D", true, dummyToggleAction);

            MSM.AddPadding(modId, left);
            MSM.AddLabel(modId, left, "Settings Category B");
            MSM.AddSlider(modId, left, "Slider Setting A", 0, 4, 2, dummySliderAction);
            MSM.AddSlider(modId, left, "Slider Setting B", 0, 100, 25, dummySliderAction);
            MSM.AddPadding(modId, right);
            MSM.AddPadding(modId, right);
            MSM.AddSlider(modId, right, "Slider Setting C", 0, 10, 7, dummySliderAction);
            MSM.AddSlider(modId, right, "Slider Setting D", 0, 100, 61, dummySliderAction);

            MSM.AddPadding(modId, left);
            MSM.AddLabel(modId, left, "Button's explanation", Utilities.FontSize.Small);
            MSM.AddButton(modId, left, "Some Button", dummyButtonAction, ButtonColor.Blue);
            MSM.AddPadding(modId, right);
            MSM.AddLabel(modId, right, "Other button's explanation", Utilities.FontSize.Small);
            MSM.AddButton(modId, right, "Some Small Red Button", dummyButtonAction, ButtonColor.Red, ButtonSize.Smaller);

            MSM.AddPadding(modId, left);
            MSM.AddDropdown(modId, left, "Dropdown Setting", dropdownOptions, dropdownDefault, dummyDropdownAction);

            MSM.AddPadding(modId, left);
            MSM.AddLabel(modId, left, "Some disclaimer or explanation about something", Utilities.FontSize.VerySmall);
        }
    }
}
