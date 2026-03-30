# Mod Settings Menu (MSM)
**Download**: [Latest Release](https://github.com/Echo5Dev/MDRG-ModSettingsMenu/releases/)

MSM is a framework mod for MDRG that allows mod developers to easily add a configurable settings menu for their mod.<br>
The MSM menu is accessed via a new button in the Options menu.

### Preview
![](https://raw.githubusercontent.com/Echo5Dev/MDRG-ModSettingsMenu/refs/heads/main/MSMPreview.png)

### Features
* Easy to use API
* Shared menu for all the mods using the MSM
* Individual settings menu for each mod
* All vanilla-like, customizable settings UI elements, including: Labels, Checkboxes, Sliders, Dropdown Menus, Buttons

### Compatibility
* MelonLoader v0.7.2+
* MDRG v0.95
> Also works on v0.90, and should hopefully be compatible with future versions too

## For Users
1. Ensure you have MelonLoader v0.7.2+ installed in your game: [/LavaGang/MelonLoader/releases](https://github.com/LavaGang/MelonLoader/releases)
2. Download `ModSettingsMenu.zip` from the [releases](https://github.com/Echo5Dev/MDRG-ModSettingsMenu/releases/)
3. Open the .zip file and drop the mod file (`ModSettingsMenu.dll`) into your game `Mods` folder.
4. Open the MSM menu using the new button in the Options menu.


## For Modders

**IMPORTANT**: to avoid compatibility issues and more, please do NOT redistribute a modified version of the MSM. <br>
Feel free to request changes or features here, or by reaching out on the MDRG Discord server. <br> <br>
**Full API documentation**: TODO documentation link

### How to use
1. Download the `ModSettingsMenu.dll` and reference it in your mod project.
2. Add `using ModSettingsMenu;`.
3. Register your mod by using `MSM.RegisterMod(string modId, string displayName)`.<br>
4. Declare your settings / request UI elements using the provided API.<br>

### Notes
* Call the MSM during `OnInitializeMelon()`
* When registering your mod, the modID must be unique, and displayName is what users will see in the MSM menu
* The MSM offers two panels (left/right) to organize your settings
* Order matters: the UI elements will be created in the order you declared them, per panel (left/right)
* MSM does not save values by itself. You must handle saving/loading yourself (eg. MelonPreferences)
* All logic such as saving values should be handled inside the provided actions (`onClick`, `onChanged`, etc.)
* Supported UI elements:
  * Labels
  * Checkboxes
  * Sliders
  * Dropdown Menus
  * Buttons
  * Padding (for visual)
 * The "Example Mods" seen on the preview can be found on the project [here](https://github.com/Echo5Dev/MDRG-ModSettingsMenu/blob/main/ExampleModMSM/ExampleModMSM.cs) 

### Minimal Example
```
public class ExampleModMSM : MelonMod
    {
        private static MelonPreferences_Category _exampleCategory;
        private static MelonPreferences_Entry<bool> _exampleSetting;

        public override void OnInitializeMelon()
        {
            // Load settings
            _exampleCategory = MelonPreferences.CreateCategory("CategoryName");
            _exampleSetting = _exampleCategory.CreateEntry("Enabled", true, "Enable Setting");
            _exampleCategory.SaveToFile(true);

            // Initialize MSM
            InitMSM();
        }

        public static void InitMSM()
        {
            string modId = "exampleMod";
            var left = PanelSide.LeftPanel;
            var right = PanelSide.RightPanel;
            Action<bool> exampleToggleAction = (Action<bool>)(value =>
            { 
                _exampleSetting.Value = value;
                _exampleCategory.SaveToFile(true);
            });

            MSM.RegisterMod(modId, "Example Mod");
            MSM.AddCheckbox(modId, left, "Checkbox Setting Name", () => _exampleSetting.Value, exampleToggleAction); // () => _exampleSetting.Value is the starting value
        }
    }
```
