﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tivoli.Scripts
{
    public static class TivoliEditorPrefs
    {
        public const string OverrideApiUrl = "TivoliOverrideApiUrl";
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

            string[] apiUrls = {
                "https://tivoli.space",
                "http://127.0.0.1:3000"
            };

            if (!EditorPrefs.HasKey(TivoliEditorPrefs.OverrideApiUrl))
            {
                EditorPrefs.SetString(TivoliEditorPrefs.OverrideApiUrl, apiUrls[0]);
            }
            
            var textField = new TextField
            {
                value = EditorPrefs.GetString(TivoliEditorPrefs.OverrideApiUrl),
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
                    style = {
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

        public void CreateGUI()
        {
            rootVisualElement.style.paddingTop = Padding;
            rootVisualElement.style.paddingRight = Padding;
            rootVisualElement.style.paddingBottom = Padding;
            rootVisualElement.style.paddingLeft = Padding;

            DrawTitle("Tivoli Dev Menu");

            DrawSubtitle("Override API url");
            DrawOverrideApiUrl();
        }
    }
}
#endif