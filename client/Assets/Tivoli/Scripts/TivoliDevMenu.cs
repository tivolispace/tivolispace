#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tivoli.Scripts
{
    public static class TivoliEditorPrefs
    {
        public const string OverrideApiUrl = "TivoliOverrideApiUrl";
        public const string OverridePlayMode = "TivoliOverridePlayMode";
        public const string PlayInVR = "TivoliPlayInVr";
    }

    public static class TivoliDefaultEditorPrefs
    {
        public const string OverrideApiUrl = "http://127.0.0.1:3000";
        public const bool OverridePlayMode = true;
        public const bool PlayInVR = false;
    }

    public class TivoliDevMenu : EditorWindow
    {
        [MenuItem("Tivoli/Open Dev Menu")]
        public static void OpenTivoliDevMenu()
        {
            var window = GetWindow<TivoliDevMenu>();
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Tivoli/Editor/tivoli-dev-menu.png");
            window.titleContent = new GUIContent("Tivoli Dev Menu", icon);
        }

        private const float Padding = 8f;

        private void DrawTitle(string text)
        {
            rootVisualElement.Add(new TextElement
            {
                text = text,
                style =
                {
                    fontSize = 24,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginBottom = Padding
                }
            });
        }

        private void DrawSubtitle(string text)
        {
            rootVisualElement.Add(new TextElement
            {
                text = text,
                style =
                {
                    fontSize = 16,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginTop = Padding,
                    marginBottom = Padding
                }
            });
        }

        private void DrawOverrideApiUrl()
        {
            var flexbox = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    paddingBottom = Padding
                }
            };

            rootVisualElement.Add(flexbox);

            string[] apiUrls =
            {
                "https://tivoli.space",
                "http://127.0.0.1:3000"
            };


            var textField = new TextField
            {
                value =
                    EditorPrefs.GetString(TivoliEditorPrefs.OverrideApiUrl, TivoliDefaultEditorPrefs.OverrideApiUrl),
                style =
                {
                    width = 192,
                    marginRight = Padding
                }
            };

            textField.RegisterCallback<ChangeEvent<string>>((e) =>
            {
                EditorPrefs.SetString(TivoliEditorPrefs.OverrideApiUrl, e.newValue);
            });

            flexbox.Add(textField);

            foreach (var apiUrl in apiUrls)
            {
                var button = new Button
                {
                    text = apiUrl,
                    style =
                    {
                        marginRight = Padding
                    }
                };

                button.clicked += () =>
                {
                    EditorPrefs.SetString(TivoliEditorPrefs.OverrideApiUrl, apiUrl);
                    textField.value = apiUrl;
                };

                flexbox.Add(button);
            }
        }

        private void DrawOverridePlayMode()
        {
            var overridePlayModeText = new Func<string>(() =>
                EditorPrefs.GetBool(TivoliEditorPrefs.OverridePlayMode, TivoliDefaultEditorPrefs.OverridePlayMode)
                    ? "Tivoli \"Initialize\" scene"
                    : "current scene");

            var labelFlexbox = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    paddingBottom = Padding
                }
            };

            rootVisualElement.Add(labelFlexbox);

            var preLabel = new Label
            {
                text = "Play button will take you to ",
            };

            labelFlexbox.Add(preLabel);

            var label = new Label
            {
                text = overridePlayModeText(),
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            };

            labelFlexbox.Add(label);

            var buttonsFlexbox = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    paddingBottom = Padding
                }
            };

            rootVisualElement.Add(buttonsFlexbox);

            var initializeButton = new Button
            {
                text = "Tivoli \"Initialize\" scene",
                style =
                {
                    marginRight = Padding
                }
            };

            initializeButton.clicked += () =>
            {
                EditorPrefs.SetBool(TivoliEditorPrefs.OverridePlayMode, true);
                label.text = overridePlayModeText();
            };

            buttonsFlexbox.Add(initializeButton);

            var currentButton = new Button
            {
                text = "Current scene",
                style =
                {
                    marginRight = Padding
                }
            };

            currentButton.clicked += () =>
            {
                EditorPrefs.SetBool(TivoliEditorPrefs.OverridePlayMode, false);
                label.text = overridePlayModeText();
            };

            buttonsFlexbox.Add(currentButton);
        }

        private void DrawPlayInVR()
        {
            var playInVRText = new Func<string>(() =>
                EditorPrefs.GetBool(TivoliEditorPrefs.PlayInVR, TivoliDefaultEditorPrefs.PlayInVR)
                    ? "VR mode"
                    : "desktop mode");

            var labelFlexbox = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    paddingBottom = Padding
                }
            };

            rootVisualElement.Add(labelFlexbox);

            var preLabel = new Label
            {
                text = "Will start in  ",
            };

            labelFlexbox.Add(preLabel);

            var label = new Label
            {
                text = playInVRText(),
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            };

            labelFlexbox.Add(label);

            var buttonsFlexbox = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    paddingBottom = Padding
                }
            };

            rootVisualElement.Add(buttonsFlexbox);

            var playInVRButton = new Button
            {
                text = "Play in VR",
                style =
                {
                    marginRight = Padding
                }
            };

            playInVRButton.clicked += () =>
            {
                EditorPrefs.SetBool(TivoliEditorPrefs.PlayInVR, true);
                label.text = playInVRText();
            };

            buttonsFlexbox.Add(playInVRButton);

            var playInDesktop = new Button
            {
                text = "Play in desktop",
                style =
                {
                    marginRight = Padding
                }
            };

            playInDesktop.clicked += () =>
            {
                EditorPrefs.SetBool(TivoliEditorPrefs.PlayInVR, false);
                label.text = playInVRText();
            };

            buttonsFlexbox.Add(playInDesktop);
        }

        public void CreateGUI()
        {
            rootVisualElement.style.paddingTop = Padding;
            rootVisualElement.style.paddingRight = Padding;
            rootVisualElement.style.paddingBottom = Padding;
            rootVisualElement.style.paddingLeft = Padding;

            DrawTitle("Tivoli Dev Menu");

            DrawSubtitle("Override API url");
            DrawOverrideApiUrl();

            DrawSubtitle("Override play mode");
            DrawOverridePlayMode();
            
            DrawSubtitle("Play in VR");
            DrawPlayInVR();
        }
    }
}
#endif