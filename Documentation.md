# API Documentation

## Table of Contents

* [General Notes](#general-notes)
* [RegisterMod](#registermod)
* [AddLabel](#addlabel)
* [AddButton](#addbutton)
* [AddCheckbox](#addcheckbox)
* [AddSlider(float)](#addslider-(float))
* [AddSlider(int)](#addslider-(int))
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
| `text`       | `string`       | Button text.                                            |
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

WIP
