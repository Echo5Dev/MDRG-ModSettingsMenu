using Il2Cpp;
using Il2CppTMPro;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.PropertyVariants;
using UnityEngine.UI;

//*****
// This file contains all the utilities used by the different mod modules
// This includes the UI building stuff
//*****

namespace ModSettingsMenu
{
    public static class Utilities
    {
        private static GameObject _templateRoot; //UI Templates lib

        public static GameObject ToggleTemplate; //From OptionsPopup
        public static GameObject SimpleLabel; //From OptionsPopup
        public static GameObject SliderTemplate; //From OptionsPopup
        public static GameObject ScrollListTemplate; //From ModsPopup
        public static GameObject DropdownTemplate; //From OptionsPopup


        //******************************************
        //         UI Templates Preparation
        //******************************************

        //*****
        // Creates clean UI stuff templates for later
        //*****
        public static void InitializeTemplates()
        {
            if (_templateRoot != null)
                return;

            _templateRoot = new GameObject("UI_Templates");
            UnityEngine.Object.DontDestroyOnLoad(_templateRoot);
            _templateRoot.SetActive(false);

            //** Templates from the OptionsMenu **
            var optionsMenu = UiOverlay.Instance._optionsPopupRef?.Load();
            if (optionsMenu == null)
                return;

            //ToggleTemplate
            ToggleTemplate = UnityEngine.Object.Instantiate(optionsMenu.fullscreenToggle.gameObject, _templateRoot.transform);
            ToggleTemplate.gameObject.name = "TemplateToggle";
            Toggle toggle = ToggleTemplate.GetComponent<Toggle>();
            SetChildText(ToggleTemplate.gameObject, "Label", "Template Toggle");
            toggle.onValueChanged.m_PersistentCalls.Clear();
            toggle.onValueChanged.m_Calls.Clear();
            toggle.onValueChanged.RemoveAllListeners();
            ToggleTemplate.SetActive(false);

            //SimpleLabel
            SimpleLabel = UnityEngine.Object.Instantiate(optionsMenu.transform.Find("Area 1/Screen label").gameObject, _templateRoot.transform);
            SimpleLabel.gameObject.name = "TemplateLabel";
            SimpleLabel.GetComponent<TextMeshProUGUI>().SetText("Sample text");
            UnityEngine.Object.DestroyImmediate(SimpleLabel.GetComponent<LocalizeTextMeshProFont>());
            UnityEngine.Object.DestroyImmediate(SimpleLabel.GetComponent<GameObjectLocalizer>());
            SimpleLabel.SetActive(false);

            //SliderTemplate
            SliderTemplate = UnityEngine.Object.Instantiate(optionsMenu.transform.Find("Area 2/Master Scrollbar").gameObject, _templateRoot.transform);
            SliderTemplate.gameObject.name = "TemplateSlider";
            Scrollbar sb = SliderTemplate.GetComponent<Scrollbar>();
            sb.onValueChanged.m_PersistentCalls.Clear();
            sb.onValueChanged.m_Calls.Clear();
            sb.onValueChanged.RemoveAllListeners();
            SliderTemplate.SetActive(false);

            //DropdownTemplate
            DropdownTemplate = UnityEngine.Object.Instantiate(optionsMenu.transform.Find("Area 1/Language Dropdown").gameObject, _templateRoot.transform);
            DropdownTemplate.name = "TemplateDropdown";
            TMP_Dropdown tmpDropdown = DropdownTemplate.GetComponent<TMP_Dropdown>();
            tmpDropdown.options.Clear();
            tmpDropdown.captionText.text = "Placeholder";
            tmpDropdown.onValueChanged.RemoveAllListeners();
            tmpDropdown.template.gameObject.SetActive(false);

            //** Templates from the modsPopup **
            //ScrollList
            CreateScrollListTemplate();
        }

        //*****
        // Template preparation for ScrollList, because more complex
        //*****
        public static void CreateScrollListTemplate()
        {
            var modsPopup = UiOverlay.Instance._modsPopupRef?.Load();
            if (modsPopup == null)
                return;
            var scrollRect = modsPopup.GetComponentInChildren<ScrollRect>(true);
            if (scrollRect == null)
            {
                MelonLogger.Error("MSM: No ScrollRect found in ModsPopup!");
                return;
            }

            var original = scrollRect.gameObject;

            ScrollListTemplate = UnityEngine.Object.Instantiate(original, _templateRoot.transform);
            ScrollListTemplate.name = "TemplateScrollList";

            // Clean root
            var layout = ScrollListTemplate.GetComponent<LayoutElement>();
            if (layout == null)
                layout = ScrollListTemplate.AddComponent<LayoutElement>();

            layout.flexibleHeight = 1f;
            layout.minHeight = 100f;

            var bg = ScrollListTemplate.GetComponent<Image>();
            if (bg == null)
                bg = ScrollListTemplate.AddComponent<Image>();

            bg.color = new Color32(0, 0, 0, 60);

            // Clean Holder
            var holder = ScrollListTemplate.transform.Find("Mask/Holder");

            var advanced = holder.GetComponent<SortableItemList>();
            if (advanced != null)
                UnityEngine.Object.DestroyImmediate(advanced);

            // Remove any existing children
            for (int i = holder.childCount - 1; i >= 0; i--)
                UnityEngine.Object.DestroyImmediate(holder.GetChild(i).gameObject);

            // Remove old layout components
            // FULL CLEAN of holder (except Transform / RectTransform)
            var components = holder.GetComponents<Component>();

            foreach (var comp in components)
            {
                if (comp == null)
                    continue;
                // Keep transform only
                if (comp is Transform)
                    continue;
                if (comp.GetType().Name.Contains("RectTransform"))
                    continue;

                UnityEngine.Object.DestroyImmediate(comp);
            }

            // Add a clean layout system
            var fitter = holder.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            var vlg = holder.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.childControlHeight = true;
            vlg.childForceExpandHeight = false;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.spacing = 6f;

            ScrollListTemplate.SetActive(false);
        }

        //******************************************
        //           UI Elems Builders
        //******************************************

        //*****
        // Builds a new Padding of desired height, returns the GameObject
        //*****
        public static GameObject BuildPadding(Transform parent, int height)
        {
            GameObject padding = new GameObject("padding");
            UnityEngine.Object.Destroy(padding.transform);
            RectTransform paddingRT = padding.AddComponent<RectTransform>();
            padding.transform.SetParent(parent, false);
            paddingRT.sizeDelta = new Vector2(0, height);
            return padding;
        }

        //*****
        // Builds a new Label from template with desired parameters, returns the GameObject
        //*****
        public static GameObject BuildLabel(Transform parent, string GOname, string text, FontSize fSize = FontSize.Medium)
        {
            GameObject labelGO = UnityEngine.Object.Instantiate(SimpleLabel, parent);
            labelGO.name = GOname;
            labelGO.GetComponent<TextMeshProUGUI>().SetText(text);
            labelGO.GetComponent<TextMeshProUGUI>().fontSize = (int)fSize;
            labelGO.SetActive(true);
            return labelGO;
        }

        public enum FontSize
        {
            Large = 28,
            Medium = 24,
            Small = 20,
            VerySmall = 16,
            Tiny = 10
        }

        //*****
        // Builds a new Toggle from template with desired parameters, returns the GameObject
        //*****
        public static GameObject BuildToggle(Transform parent, string GOname, string label, bool startValue, Action<bool> listener)
        {
            GameObject toggleGO = UnityEngine.Object.Instantiate(ToggleTemplate, parent);
            toggleGO.name = GOname;
            toggleGO.SetActive(true);

            SetChildText(toggleGO, "Label", label);

            Toggle toggle = toggleGO.GetComponent<Toggle>();

            toggle.isOn = startValue;
            toggle.onValueChanged.AddListener((Action<bool>)(value => listener(value)));

            return toggleGO;
        }

        //*****
        // Builds a new Scrollbar slider from template with desired parameters, adds a value label next to it, returns the GameObject
        //*****
        public static GameObject BuildScrollbar(Transform parent, string name, float min, float max, float startValue, int steps, Action<float> listener)
        {
            GameObject holder = new GameObject(name + "_holder");
            holder.transform.SetParent(parent, false);

            RectTransform holderRT = holder.AddComponent<RectTransform>();
            holderRT.sizeDelta = new Vector2(0, 16);

            HorizontalLayoutGroup hlg = holder.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = false;
            hlg.childControlHeight = false;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;
            hlg.spacing = 8f;

            GameObject scrollbarGO = UnityEngine.Object.Instantiate(SliderTemplate, holder.transform);
            scrollbarGO.name = name;
            scrollbarGO.SetActive(true);

            Scrollbar sb = scrollbarGO.GetComponent<Scrollbar>();

            // Preserve original prefab size
            RectTransform sliderRT = scrollbarGO.GetComponent<RectTransform>();
            float width = sliderRT.sizeDelta.x;   // 300
            float height = sliderRT.sizeDelta.y;  // 16

            LayoutElement sliderLE = scrollbarGO.GetComponent<LayoutElement>();
            if (sliderLE == null)
                sliderLE = scrollbarGO.AddComponent<LayoutElement>();

            sliderLE.minWidth = width;
            sliderLE.preferredWidth = width;
            sliderLE.minHeight = height;
            sliderLE.preferredHeight = height;
            sliderLE.flexibleWidth = 0;
            sliderLE.flexibleHeight = 0;

            sb.numberOfSteps = steps;
            sb.value = (startValue - min) / (max - min);

            // Value Label
            GameObject valueLabel = BuildLabel(holder.transform, name + "_Value", "");
            TextMeshProUGUI valueTMP = valueLabel.GetComponent<TextMeshProUGUI>();

            valueTMP.enableWordWrapping = false;
            valueTMP.overflowMode = TextOverflowModes.Overflow;

            LayoutElement labelLE = valueLabel.GetComponent<LayoutElement>();
            if (labelLE == null)
                labelLE = valueLabel.AddComponent<LayoutElement>();

            labelLE.minWidth = 40;
            labelLE.preferredWidth = 60;
            labelLE.flexibleWidth = 0;

            // Value Updating
            void UpdateLabel(float realValue)
            {
                if (steps > 1)
                    valueTMP.SetText(Mathf.RoundToInt(realValue).ToString());
                else
                    valueTMP.SetText(realValue.ToString("0.##"));
            }

            float initialValue = min + sb.value * (max - min);
            if (steps > 1)
                initialValue = Mathf.Round(initialValue);

            UpdateLabel(initialValue);

            sb.onValueChanged.AddListener((Action<float>)(normalized =>
            {
                float realValue = min + normalized * (max - min);

                if (steps > 1)
                    realValue = Mathf.Round(realValue);

                UpdateLabel(realValue);
                listener(realValue);
            }));

            return scrollbarGO;
        }

        //*****
        // Builds a new button using the native method with desired parameters, returns the GameObject
        //*****
        public static GameObject BuildButton(Transform parent, string text, Action listener, CommonButtonColorType colorType = CommonButtonColorType.Normal, CommonButtonHandler.CommonButtonSizeEnum size = CommonButtonHandler.CommonButtonSizeEnum.Normal)
        {
            var button = UiOverlay.Instance.commonButton.GetFromPool(parent);

            button.ButtonText.text = text;
            button.SetOnClick((Action)(() => listener()));
            button.SetColorType(colorType);
            button.SetSize(size);

            return button.gameObject;
        }

        //*****
        // Builds a new ScrollList from template with desired parameters, returns the GameObject
        //*****
        public static GameObject BuildScrollList(Transform parent, string name, out Transform contentRoot, float height = 100, float width = 100)
        {
            GameObject scrollGO = UnityEngine.Object.Instantiate(ScrollListTemplate, parent);

            scrollGO.name = name;
            scrollGO.SetActive(true);

            contentRoot = scrollGO.transform.Find("Mask/Holder");

            RectTransform rt = scrollGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(width, height);

            return scrollGO;
        }

        //*****
        // Builds a new Dropdown from template with desired parameters, returns the GameObject
        //*****
        public static GameObject BuildDropdown(Transform parent, string name, List<string> options, int defaultIndex, Action<string> onChanged)
        {
            GameObject go = UnityEngine.Object.Instantiate(DropdownTemplate, parent);
            go.name = name;
            go.SetActive(true);

            var dropdown = go.GetComponent<TMP_Dropdown>();

            foreach (var opt in options)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData(opt));
            }

            dropdown.value = defaultIndex;
            dropdown.RefreshShownValue();

            dropdown.onValueChanged.AddListener((UnityEngine.Events.UnityAction<int>)(index =>
            {
                onChanged?.Invoke(dropdown.options[index].text);
            }));

            return go;
        }


        //******************************************
        //         Other/Debug Utilities
        //******************************************

        //*****
        // Changes the displayed text of the Label child of a GameObject (used for the toggle at least)
        //*****
        public static bool SetChildText(GameObject root, string childName, string newText)
        {
            try
            {
                if (root == null) return false;

                var t = root.transform;
                for (int i = 0; i < t.childCount; i++)
                {
                    var child = t.GetChild(i);

                    if (child.name == childName)
                    {
                        var tmp = child.GetComponent<TextMeshProUGUI>();
                        if (tmp != null)
                        {
                            tmp.SetText(newText);
                            EnforceLabel(root, newText);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[Utilities] TrySetLabelText error: " + ex);
                return false;
            }
        }

        //*****
        // The following reapplies a label change for a few frames, because I can't find what method resets the damn labels
        //*****
        private static void EnforceLabel(GameObject root, string desired)
        {
            MelonCoroutines.Start(EnforceLabelCoroutine(root, desired));
        }

        private static IEnumerator EnforceLabelCoroutine(GameObject root, string desired)
        {
            if (root == null) yield break;
            int maxFrames = 3;
            int frames = 0;
            while (frames++ < maxFrames)
            {
                var t = root.transform;
                for (int c = 0; c < t.childCount; c++)
                {
                    var child = t.GetChild(c);
                    var tmp = child.GetComponent<TextMeshProUGUI>();
                    if (tmp != null)
                    {
                        if (tmp.text != desired)
                        {
                            tmp.SetText(desired);
                            tmp.ForceMeshUpdate();
                        }
                    }
                }
                yield return null;
            }
        }

        //*****
        //The following deletes all children from a passed Transform (e.g. for mod settings popup)
        //*****
        public static void ClearChildren(Transform parent)
        {
            if (parent == null) return;
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i);
                UnityEngine.Object.Destroy(child.gameObject);
            }
        }

    }
}
