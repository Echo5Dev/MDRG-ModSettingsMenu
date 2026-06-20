# API Documentation

## Table of Contents

* [General Notes](#general-notes)
* [RegisterMod](#registermod)
* [AddLabel](#addlabel)
* [AddButton](#addbutton)
* [AddCheckbox](#addcheckbox)
* [AddSlider (int)](#addslider-int)
* [AddSlider (float)](#addslider-float)
* [AddPadding](#addpadding)
* [AddDropdown](#adddropdown)
* [RefreshSettings](#refreshsettings)
  
## General Notes
### Common Parameters
- `modID`: your mod's unique identifier, can be any <string>
- `side`: of <PanelSide> type, determines which side of the Mod Settings UI this setting will appear. Possible values are `PanelSide.LeftPanel` and `PanelSide.RightPanel`

### Examples
In the following examples, we will assume that the following lines exist:
```
private static MelonPreferences_Entry<bool> _exampleSetting;
```
Or the corresponding type of variable, and:
```
string modId = "exampleMod";
```

## RegisterMod
Mandatory first step. Registers your mod with the settings menu.

### Syntax
```
MSM.RegisterMod(
    string modId,
    string displayName,
);
```

### Parameters
| Parameter     | Type          | Description                                             |
| ------------- | ------------- | ------------------------------------------------------- |
| `modId`      | `string`       | Registered mod ID.                                      |
| `displayName`| `string`       | The name of your mod as it should appear in the Mod Settings Menu |

### Example
```
MSM.AddLabel(
    modId,
    "Example Mod"
);
```

## AddLabel
Adds a label UI element to a registered mod's settings menu.

### Syntax
```
MSM.AddLabel(
    string modId,
    PanelSide side,
    string text,
    Utilities.FontSize fontSize = Utilities.FontSize.Medium
    string tooltip = ""
);
```

### Parameters
| Parameter     | Type          | Description                                             |
| ------------- | ------------- | ------------------------------------------------------- |
| `modId`      | `string`       | Registered mod ID.                                      |
| `side`       | `PanelSide`    | The panel where the setting should appear.              |
| `text`       | `string`       | Label text.                                            |
| `fontSize`   | `Utilities.FontSize`| Optional. Font size. Defaults to `Medium`. (see note 1) |
| `tooltip`     | `string`      | Optional tooltip text.                                  |


### Notes
1. Valid values for `fontSize`: `Large`(28px), `Medium`(24px), `Small`(20px), `Very Small`(16px), `Tiny`(10px)

### Example
```
MSM.AddLabel(
    modId,
    PanelSide.LeftPanel,
    "Label text here",
    Utilities.FontSize.Large
);
```

## AddButton
Adds a button UI element to a registered mod's settings menu.

### Syntax
```
MSM.AddButton(
    string modId,
    PanelSide side,
    string label,
    Action onClick,
    ButtonColor buttonColor = ButtonColor.Blue,
    ButtonSize buttonSize = ButtonSize.Normal,
    string tooltip = ""
);
```

### Parameters
| Parameter     | Type          | Description                                             |
| ------------- | ------------- | ------------------------------------------------------- |
| `modId`      | `string`       | Registered mod ID.                                      |
| `side`       | `PanelSide`    | The panel where the setting should appear.              |
| `label`       | `string`      | Button text.                                            |
| `onClick`     | `Action`      | Invoked when the button is pressed.                     |
| `buttonColor` | `ButtonColor` | Optional. Button color. Defaults to `ButtonColor.Blue`. (see note 1) |
| `buttonSize`  | `ButtonSize`  | Optional. Button size. Defaults to `ButtonSize.Normal`. (see note 2) |
| `tooltip`     | `string`      | Optional tooltip text.                                  |


### Notes
1. Valid values for `buttonColor`: `default`, `Blue`, `Red`, `Green`
2. Valid values for `buttonSize`: `Normal` or `Smaller`

### Example
```
MSM.AddButton(
    modId,
    PanelSide.LeftPanel,
    "Button Name",
    exampleButtonAction,
    ButtonColor.Red,
    ButtonSize.Large,
    "Explanation on what the button does."
);
```
Where:
```
Action exampleButtonAction = (Action)(() =>
            {
              //Actions on button click
            });
```

## AddCheckbox
Adds a checkbox UI element to a registered mod's settings menu.

### Syntax
```
MSM.AddCheckbox(
    string modId,
    PanelSide side,
    string label,
    Func<bool> startValue,
    Action<bool> onClick,
    string tooltip = ""
);
```

### Parameters
| Parameter    | Type           | Description                                                           |
| ------------ | -------------- | --------------------------------------------------------------------- |
| `modId`      | `string`       | Registered mod ID.                                                    |
| `side`       | `PanelSide`    | The panel where the setting should appear.                            |
| `label`      | `string`       | The text displayed next to the checkbox.                              |
| `startValue` | `Func<bool>`   | The current value of the setting.                                     |
| `onClick`    | `Action<bool>` | Called whenever the checkbox state changes. (see note 1)              |
| `tooltip`    | `string`       | Optional tooltip displayed when hovering the setting.                 |

### Notes
1. There you must handle changing the currently stored value, and save it to your MelonPreferences or config file

### Example
```
MSM.AddCheckbox(
    modId,
    PanelSide.LeftPanel,
    "Checkbox Setting Name",
    () => _exampleSetting.Value,
    exampleToggleAction,
    "Explanation on what the checkbox does."
);
```
Where:
```
Action<bool> exampleToggleAction = (Action<bool>)(value =>
            { 
                _exampleSetting.Value = value;
                _exampleCategory.SaveToFile(true);
            });
```

## AddSlider (int)
Adds a Slider UI element to a registered mod's settings menu, with increments of 1.

### Syntax
```
MSM.AddSlider(
    string modId,
    PanelSide side,
    string label,
    int min,
    int max,
    Func<int> startValue,
    Action<int> onchanged,
    string tooltip = ""
);
```

### Parameters
| Parameter    | Type           | Description                                                           |
| ------------ | -------------- | --------------------------------------------------------------------- |
| `modId`      | `string`       | Registered mod ID.                                                    |
| `side`       | `PanelSide`    | The panel where the setting should appear.                            |
| `label`      | `string`       | The text displayed above the slider.                                  |
| `min`        | `int`          | The minimum value of the slider.                                      |
| `max`        | `int`          | The maximum value of the slider.                                      |
| `startValue` | `Func<int>`    | The current value of the setting.                                     |
| `onClick`    | `Action<int>`  | Called whenever the slider state changes. (see note 1)                |
| `tooltip`    | `string`       | Optional tooltip displayed when hovering the setting.                 |

### Notes
1. There you must handle changing the currently stored value, and save it to your MelonPreferences or config file

### Example
```
MSM.AddSlider(
    modId,
    PanelSide.LeftPanel,
    "Slider Setting Name",
    0,
    10,
    () => _exampleSetting.Value,
    exampleSliderAction1,
    string tooltip = "Exaplanation on what the slider does."
);
```
Where:
```
Action<int> exampleSliderAction1 = (Action<int>)(value =>
            { 
                MelonLogger.Msg("Preset: " + value);
                _exampleSliderSetting1.Value = value;
                _exampleCategory.SaveToFile(true);
            });
```

## AddSlider (float)
Adds a Slider UI element to a registered mod's settings menu, float version with custom increments.

### Syntax
```
MSM.AddSlider(
    string modId,
    PanelSide side,
    string label,
    float min,
    float max,
    Func<float> startValue,
    int steps,
    Action<int> onchanged,
    string tooltip = ""
);
```

### Parameters
| Parameter    | Type           | Description                                                           |
| ------------ | -------------- | --------------------------------------------------------------------- |
| `modId`      | `string`       | Registered mod ID.                                                    |
| `side`       | `PanelSide`    | The panel where the setting should appear.                            |
| `label`      | `string`       | The text displayed above the slider.                                  |
| `float`      | `int`          | The minimum value of the slider.                                      |
| `float`      | `int`          | The maximum value of the slider.                                      |
| `startValue` | `Func<float>`  | The current value of the setting.                                     |
| `steps`      | `int`          | Number of steps. (see note 1)                                         |
| `onClick`    | `Action<float>`| Called whenever the slider state changes. (see note 2)                |
| `tooltip`    | `string`       | Optional tooltip displayed when hovering the setting.                 |

### Notes
1. If min = 0 and max = 100; Steps = 2 gives you only 0 and 100 as possible values. Steps 3 gives you 0, 50 and 100. Steps 5 gives you 0, 25, 50, 75, 100, etc...
2. There you must handle changing the currently stored value, and save it to your MelonPreferences or config file

### Example
```
MSM.AddSlider(
    modId,
    PanelSide.LeftPanel,
    "Slider Setting Name",
    0,
    100,
    () => _exampleSetting.Value,
    5,
    exampleSliderAction1,
    string tooltip = "Explanation on what the slider does."
);
```
Where:
```
Action<float> exampleSliderAction1 = (Action<float>)(value =>
            { 
                MelonLogger.Msg("Preset: " + value);
                _exampleSliderSetting1.Value = value;
                _exampleCategory.SaveToFile(true);
            });
```

## Addpadding
Adds a padding/margin, 30px.

### Syntax
```
MSM.AddPadding(
    string modId,
    PanelSide side,
);
```

### Parameters
| Parameter    | Type           | Description                                                           |
| ------------ | -------------- | --------------------------------------------------------------------- |
| `modId`      | `string`       | Registered mod ID.                                                    |
| `side`       | `PanelSide`    | The panel where the setting should appear.                            |

### Example
```
MSM.AddPadding(
    modId,
    PanelSide.LeftPanel,
);
```

## AddDropdown
Adds a Dropdown menu UI element to a registered mod's settings menu.

### Syntax
```
MSM.AddDropdown(
    string modId,
    PanelSide side,
    string label,
    List<string> options,
    Func<string> defaultValue,
    Action<string> onChanged,
    string tooltip = ""
);
```

### Parameters
| Parameter    | Type           | Description                                                           |
| ------------ | -------------- | --------------------------------------------------------------------- |
| `modId`      | `string`       | Registered mod ID.                                                    |
| `side`       | `PanelSide`    | The panel where the setting should appear.                            |
| `label`      | `string`       | The text displayed above the slider.                                  |
| `options`    | `List<string>` | The list of available string options.                                 |
| `defaultValue`| `Func<string>`| The current value.                                                    |
| `onChanged`  | `Action<string>`| Called whenever the dropdown state changes. (see note 1)             |
| `tooltip`    | `string`       | Optional tooltip displayed when hovering the setting.                 |

### Notes
1. There you must handle changing the currently stored value, and save it to your MelonPreferences or config file

### Example
```
MSM.AddDropdown(
    modId,
    PanelSide.LeftPanel,
    "Dropdown Setting Name",
    dropdownOptions,
    () => _exampleSetting.Value,
    exampleDropdownAction,
    string tooltip = "Explanation on what the dropdown menu does."
);
```

With
```
List<string> dropdownOptions = new List<string> { "Option A", "Option B", "Option C" };

Action<string> exampleDropdownAction = (Action<string>)(value =>
            {
                MelonLogger.Msg("Dropdown option changed: " + value);
                _exampleDropdownSetting.Value = value;
                _exampleCategory.SaveToFile(true);
            });
```

## RefreshSettings
Refreshes the mod's settings page. Same as closing and reopening the MSM.
Requested feature, used to make a "restore settings to defaults" button.

### Syntax
```
MSM.RefreshSettings(
    string modId,
);
```

### Parameters
| Parameter    | Type           | Description                                                           |
| ------------ | -------------- | --------------------------------------------------------------------- |
| `modId`      | `string`       | Registered mod ID.                                                    |

### Example
If used as part of a button action to refresh the page.
```
Action exampleButtonAction = (Action)(() =>
            {
              MSM.RefreshSettings(modId,);
            });
```
