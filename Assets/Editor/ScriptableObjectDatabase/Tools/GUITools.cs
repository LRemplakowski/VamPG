using UnityEditor;
using UnityEngine;

namespace MyLib.EditorTools.Tools
{
    public class GUITools
    {
        #region Separators

        public static void DrawVerticalSeparator(int xOffset, int yOffset, int height, int thickness, Color color)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(thickness));
            {
                GUILayout.Space(0);
                Rect lastRect = GUILayoutUtility.GetLastRect();
                lastRect = new Rect(lastRect.xMax + xOffset, lastRect.yMin + yOffset, thickness, height);

                GUI.color = color;
                GUI.Box(lastRect, new GUIContent());
                GUI.color = Color.white;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2f);
        }

        public static void DrawHorizontalSeparator(int xOffset, int yOffset, int width, int thickness, Color color)
        {
            GUILayout.BeginVertical(GUILayout.Height(thickness));
            {
                GUILayout.Space(0f);
                Rect lastRect = GUILayoutUtility.GetLastRect();
                lastRect = new Rect(lastRect.xMin + xOffset, lastRect.yMax + yOffset, width, thickness);

                GUI.color = color;
                GUI.Box(lastRect, new GUIContent());
                GUI.color = Color.white;
            }
            GUILayout.EndVertical();
            GUILayout.Space(2f);
        }

        public static void DrawHorizontalSeparator(int thickness, Color color)
        {
            GUILayout.BeginHorizontal(GUILayout.Height(thickness));
            {
                GUILayout.FlexibleSpace();
                Rect lastRect = GUILayoutUtility.GetLastRect();
                lastRect = new Rect(lastRect.xMin, lastRect.yMax, lastRect.width, thickness);

                GUI.color = color;
                GUI.Box(lastRect, new GUIContent());
                GUI.color = Color.white;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2f);
        }

        #endregion Separators

        #region Headers
        public static void DrawHeader(string text)
        {
            if (!GUILayout.Toggle(true, "<b><size=11>" + text + "</size></b>", "dragtab")) { };
        }

        public static void DrawHeader(string text, float width)
        {
            if (!GUILayout.Toggle(true, "<b><size=11>" + text + "</size></b>", "dragtab", GUILayout.Width(width))) { };
        }

        public static void DrawHeader(Rect position, string text)
        {
            if (!GUI.Toggle(position, true, "<b><size=11>" + text + "</size></b>", "dragtab")) { };
        }

        #endregion Headers

        #region Contents

        /// <summary>
        /// Draws an accent box around BeginContents and EndContents
        /// </summary>
        public static void BeginContents(float width)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(width));
            GUILayout.Space(4f);
            EditorGUILayout.BeginHorizontal("TextArea", GUILayout.MinHeight(10f));
            GUILayout.BeginVertical();
            GUILayout.Space(2f);
        }

        public static void BeginContents()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);
            EditorGUILayout.BeginHorizontal("TextArea", GUILayout.MinHeight(10f));
            GUILayout.BeginVertical();
            GUILayout.Space(2f);
        }

        public static void EndContents()
        {
            GUILayout.Space(3f);
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }

        /// <summary>
        /// Draws an accent box around BeginContents and EndContents using a desired style
        /// </summary>
        /// <param name="style">Desired style to use.</param>
        public static void BeginContents(string style)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(style);
            GUILayout.BeginVertical();
        }

        public static void EndContents(float spacing)
        {
            GUILayout.Space(spacing);
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
        }

        public static Rect DrawContainer(Rect position, int padding)
        {
            GUI.Box(position, "", "TextArea");
            return new Rect(position.x + padding, position.y + padding, position.width - (padding * 2), position.height - padding * 2);
        }

        #endregion Contents

        #region Misc

        public static void WebLinkButton(string link)
        {
            WebLinkButton(link, link, null);
        }

        public static void WebLinkButton(string label, string link)
        {
            WebLinkButton(label, link, null);
        }

        public static void WebLinkButton(string label, string link, string style)
        {
            if (string.IsNullOrEmpty(style))
            {
                if (GUILayout.Button(label))
                {
                    Application.OpenURL(link);
                }
            }
            else
            {
                if (GUILayout.Button(label, style))
                {
                    Application.OpenURL(link);
                }
            }
        }


        public static string SearchBar(string searchText, EditorWindow window, bool isSearching)
        {
            EditorGUILayout.BeginHorizontal();
            GUI.SetNextControlName("SearchField");
            string searchValue = EditorGUILayout.TextField(searchText, (GUIStyle)"SearchTextField");

            if (isSearching)
            {
                if (GUILayout.Button("", (GUIStyle)"SearchCancelButton", GUILayout.Width(17)))
                {
                    searchValue = ""; // Reset
                    if (window != null)
                        window.Repaint();
                }
            }
            else
            {
                GUILayout.Button("", (GUIStyle)"SearchCancelButtonEmpty", GUILayout.Width(17));
            }

            EditorGUILayout.EndHorizontal();

            return searchValue;
        }

        #endregion Msic

        #region Sliders and Toggles

        /// <summary>
        /// Shows a slider for changing percentages and add label under the slider.
        /// </summary>
        /// <param name="value">The value to represent.</param>
        /// <param name="label">The label to show under the slider.</param>
        /// <returns></returns>
        public static float PercentageSlider(float value, string label)
        {
            BeginContents("Button");
            GUILayout.BeginHorizontal();
            {
                //slider
                value = GUILayout.HorizontalSlider(value, 0, 1f);
                Rect mLastRect = GUILayoutUtility.GetLastRect();
                mLastRect.y += 8;

                //label
                GUI.Label(mLastRect, "<b><size=8>" + label + "</size> " + (value * 100).ToString("000") + "%</b>", "ObjectFieldThumbOverlay");
            }
            GUILayout.EndHorizontal();
            EndContents(0);
            return value;
        }

        /// <summary>
        /// An advanced int slider with label built in.
        /// </summary>
        /// <param name="label">The label to show.</param>
        /// <param name="value">The value to represent.</param>
        /// <param name="leftValue">The left most value of the slider.</param>
        /// <param name="rightValue">The right most value of the slider.</param>
        /// <returns></returns>
        public static int IntSlider(string label, int value, int leftValue, int rightValue, bool hideValueLabel)
        {
            BeginContents("Button");
            GUILayout.BeginHorizontal();
            {
                //slider
                value = (int)GUILayout.HorizontalSlider(value, leftValue, rightValue);
                Rect lRect = GUILayoutUtility.GetLastRect();
                //label
                lRect = new Rect(lRect.x, lRect.y + 8f, lRect.width - 40f, lRect.height - 2f);
                GUI.Label(lRect, "<b><size=8>" + label + "</size></b>", "ObjectFieldThumbOverlay");
            }
            GUILayout.EndHorizontal();
            EndContents(2f);
            return value;
        }

        /// <summary>
        /// A toggle button using a little plus and minus symbol to denote toggle on or off
        /// </summary>
        /// <param name="value">The value the toggle should show.</param>
        /// <param name="toolTip">The tool tip displayed when hovering over the toggle.</param>
        /// <returns></returns>
        public static bool ToggleOLHeader(string header, string toolTip, bool value)
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(new GUIContent("", toolTip), (!value ? "OL Plus" : "OL Minus"), GUILayout.Width(16)))
                    value = !value;

                DrawHeader(header);
            }
            GUILayout.EndHorizontal();

            return value;
        }

        public static bool ToggleHeader(Rect rect, string header, string toolTip, bool value)
        {
            GUI.color = value ? Color.green : Color.yellow;
            if (GUI.Button(rect, new GUIContent(header, toolTip), "Button"))
                value = !value;
            GUI.color = Color.white;

            return value;
        }

        public static bool ToggleExpandHeader(Rect rect, string header, string toolTip, bool value)
        {
            GUI.color = value ? Color.white : Color.gray;
            value = GUI.Toggle(new Rect(rect.x, rect.y, 16f, 16f), value, new GUIContent("", toolTip), (!value ? "OL Plus" : "OL Minus"));
            GUI.color = Color.white;
            GUI.Toggle(new Rect(rect.x + 18f, rect.y, rect.width - 18f, rect.height), true, new GUIContent(header, toolTip), "dragtab");

            return value;
        }

        public static bool ToggleExpandableHeader(string header, string toolTip, bool value)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(8f);
                value = (GUILayout.Toggle(value, new GUIContent("", toolTip), "TL Playhead", GUILayout.Width(14)));
                GUITools.DrawHeader(header);
            }
            GUILayout.EndHorizontal();

            return value;
        }

        #endregion Sliders and Toggles

        #region DragButtons

        public static Vector2 Drag2D(Vector2 scrollPosition, Rect position)
        {
            int sliderHash = "Slider".GetHashCode();
            int controlID = GUIUtility.GetControlID(sliderHash, FocusType.Passive);
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && (position.width > 50f))
                    {
                        GUIUtility.hotControl = controlID;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    return scrollPosition;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                    }
                    EditorGUIUtility.SetWantsMouseJumping(0);
                    return scrollPosition;

                case EventType.MouseMove:
                    return scrollPosition;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        scrollPosition -= (Vector2)(((current.delta * (!current.shift ? ((float)1) : ((float)3))) / Mathf.Min(position.width, position.height)) * 140f);
                        scrollPosition.y = Mathf.Clamp(scrollPosition.y, -90f, 90f);
                        current.Use();
                        GUI.changed = true;
                    }
                    return scrollPosition;
            }
            return scrollPosition;
        }

        public static bool DragButton(Rect buttonRect, bool buttonPressed)
        {
            if (buttonRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    buttonPressed = true;
                }

                if (Event.current.type == EventType.MouseUp)
                {
                    buttonPressed = false;
                }
            }

            if (buttonPressed && Event.current.type == EventType.MouseDrag)
            {
                buttonRect.x += Event.current.delta.x;
                buttonRect.y += Event.current.delta.y;
            }

            GUI.Button(buttonRect, "Draggable Button");

            return buttonPressed;
        }

        public static bool DragButton<T>(Rect buttonRect, bool buttonPressed, T dataToDrag)
        {
            Event inputEvent = Event.current;

            if ((inputEvent.type == EventType.MouseUp) || (inputEvent.type == EventType.MouseDown))
            {
                // Clear our drag info in DragAndDrop so that we know that we are not dragging
                DragAndDrop.SetGenericData(typeof(T).ToString(), null);
            }

            if (buttonRect.Contains(inputEvent.mousePosition))
            {
                if (inputEvent.type == EventType.MouseDown)
                {
                    buttonPressed = true;
                    DragAndDrop.SetGenericData(typeof(T).ToString(), dataToDrag);
                }

                if (inputEvent.type == EventType.MouseUp)
                {
                    buttonPressed = false;
                }
            }

            if (buttonPressed && inputEvent.type == EventType.MouseDrag)
            {
                buttonRect.x += inputEvent.delta.x;
                buttonRect.y += inputEvent.delta.y;
            }

            GUI.Button(buttonRect, "Draggable Button");

            return buttonPressed;
        }

        public static bool DragButton<T>(Rect buttonRect, ref bool isDragging, T dataToDrag, string iconName, string tooltip)
        where T : UnityEngine.Object
        {
            bool pressed = false;
            Event inputEvent = Event.current;

            if (buttonRect.Contains(Event.current.mousePosition))
            {
                if (inputEvent.type == EventType.MouseDrag && !isDragging)
                {
                    isDragging = true;

                    DragAndDrop.PrepareStartDrag();

                    // Set up what we want to drag
                    DragAndDrop.SetGenericData(typeof(T).ToString(), dataToDrag);

                    DragAndDrop.paths = null;
                    DragAndDrop.objectReferences = new T[1] { dataToDrag };
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    // Start the actual drag (don't know what the name is for yet)
                    DragAndDrop.StartDrag("Drag Behaviour");

                    // Use the event, else the drag won't start
                    inputEvent.Use();
                }

                if (inputEvent.type == EventType.MouseUp)
                {
                    if (!isDragging)
                        pressed = true;
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.None;
                        isDragging = false;
                    }

                    DragAndDrop.PrepareStartDrag();
                }
            }

            if ((inputEvent.type == EventType.MouseUp || inputEvent.type == EventType.MouseDown))
            {
                isDragging = false;
                DragAndDrop.visualMode = DragAndDropVisualMode.None;
                DragAndDrop.PrepareStartDrag();
            }

            GUI.Label(buttonRect, EditorGUIUtility.IconContent(iconName, ""));
            GUI.Button(buttonRect, new GUIContent("", tooltip), "MiniLabel");

            return pressed;
        }
        #endregion DragButtons

        #region Object Buttons

        public static void DrawCustomObjectField(Rect rect, SerializedProperty property, float spacing, ref bool isDragging)
        {
            EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width - 32, spacing), property);
            DrawObjectButton(new Rect(rect.xMax - 32, rect.y, 16, rect.height), property.objectReferenceValue, ref isDragging);

            if (DragButton(new Rect(rect.xMax - 16, rect.y, 16, rect.height),
                ref isDragging, property.objectReferenceValue, "cs Script Icon", "Select or drag Script"))
            {
                CustomEditorTools.InspectTarget<UnityEngine.Object>(property.objectReferenceValue);
            }
        }

        public static void DrawCustomPropertyField(Rect rect, SerializedProperty property, float spacing, ref bool isDragging)
        {
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 32, spacing), property, false);
            DrawObjectButton(new Rect(rect.xMax - 32, rect.y, 16, rect.height), property.objectReferenceValue, ref isDragging);
            if (DragButton(new Rect(rect.xMax - 16, rect.y, 16, rect.height),
                ref isDragging, property.objectReferenceValue, "cs Script Icon", "Select or drag Object"))
            {
                CustomEditorTools.InspectTarget<UnityEngine.Object>(property.objectReferenceValue);
            }

        }

        public static void DrawObjectButton(Rect rect, UnityEngine.Object objectReference)
        {
            if (objectReference == null)
                return;

            PrefabAssetType pType = PrefabUtility.GetPrefabAssetType(objectReference);

            string objectIcon;

            switch (pType)
            {
                case PrefabAssetType.Regular:
                    objectIcon = "Prefab Icon";
                    break;
                case PrefabAssetType.Model:
                    objectIcon = "PrefabModel Icon";
                    break;
                case PrefabAssetType.Variant:
                    objectIcon = "PrefabVariant Icon";
                    break;
                default:
                    objectIcon = (objectReference is ScriptableObject)
                        ? "ScriptableObject Icon" : "GameObject Icon";
                    break;
            }

            rect.width = 18f;
            rect.height = 18f;

            GUI.Label(rect, EditorGUIUtility.IconContent(objectIcon, ""));
            if (GUI.Button(rect, new GUIContent("", "Select Object"), "MiniLabel"))
            {
                if (objectReference is MonoBehaviour)
                {
                    GameObject gObject = ((MonoBehaviour)objectReference).gameObject;
                    Selection.objects = new UnityEngine.GameObject[1] { gObject };
                    Selection.activeObject = gObject;
                }
                else
                {
                    Selection.objects = new UnityEngine.Object[1] { objectReference };
                    Selection.activeObject = objectReference;
                }
            }
        }

        public static bool DrawObjectButton(Rect rect, UnityEngine.Object objectReference, ref bool isDragging)
        {
            if (objectReference == null)
                return false;

            PrefabAssetType pType = PrefabUtility.GetPrefabAssetType(objectReference);

            string objectIcon;

            switch (pType)
            {
                case PrefabAssetType.Regular:
                    objectIcon = "Prefab Icon";
                    break;
                case PrefabAssetType.Model:
                    objectIcon = "PrefabModel Icon";
                    break;
                case PrefabAssetType.Variant:
                    objectIcon = "PrefabVariant Icon";
                    break;
                default:
                    objectIcon = (objectReference is ScriptableObject)
                        ? "ScriptableObject Icon" : "GameObject Icon";
                    break;
            }

            if (DragButton(rect, ref isDragging, objectReference, 
                objectIcon, "Select or drag GameObject"))
            {
                if (objectReference is MonoBehaviour)
                {
                    GameObject gObject = ((MonoBehaviour)objectReference).gameObject;
                    Selection.objects = new UnityEngine.GameObject[1] { gObject };
                    Selection.activeObject = gObject;
                }
                else
                {
                    Selection.objects = new UnityEngine.Object[1] { objectReference };
                    Selection.activeObject = objectReference;
                }

                return true;
            }

            return false;
        }

        public static void DrawSerializedObject(SerializedObject sObject)
        {
            if (sObject == null || sObject.targetObject == null)
                return;

            EditorGUI.BeginChangeCheck();

            GUI.color = new Color(0.5770294f, 0.6585662f, 0.853f);
            BeginContents();
            {
                GUI.color = Color.white;

                SerializedProperty prop = sObject.GetIterator();

                if (prop.NextVisible(true))
                {
                    do
                    {
                        if (prop.name == "m_Script")
                            continue;

                        if (prop.prefabOverride)
                            EditorGUILayout.LabelField("!", (GUIStyle)"AssetLabel", GUILayout.Width(8f));

                        if (prop.isArray && prop.arrayElementType != "char")
                        {
                            int oldIndent = EditorGUI.indentLevel;
                            EditorGUI.indentLevel = 0;
                            GUILayout.BeginHorizontal();
                            prop.arraySize = EditorGUILayout.IntField(prop.arraySize, GUILayout.Width(32f));
                            prop.isExpanded = ToggleOLHeader(prop.displayName, "", prop.isExpanded);
                            GUILayout.EndHorizontal();

                            if (prop.isExpanded && prop.arraySize > 0)
                            {
                                GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.25f);
                                BeginContents();
                                {
                                    GUI.color = Color.white;
                                    var enumerator = prop.GetEnumerator();
                                    int counter = 0;
                                    while (enumerator.MoveNext())
                                    {
                                        EditorGUILayout.PropertyField(enumerator.Current as SerializedProperty, new GUIContent("Elelment #" + counter), false);
                                        DrawHorizontalSeparator(2, Color.gray);
                                        counter++;
                                    }
                                }
                                EndContents();
                            }

                            EditorGUI.indentLevel = oldIndent;
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(prop, false);
                        }
                    }
                    while (prop.NextVisible(false));
                }
            }
            EndContents();

            if (EditorGUI.EndChangeCheck())
                sObject.ApplyModifiedProperties();

            sObject.Update();
        }

        #endregion Object Buttons

        public static Rect ClipTextureForAspect(Rect rect, Vector2 ratio, Vector2 textureDimensions)
        {
            float scaleX = rect.width / ratio.x;
            float scaleY = rect.height / ratio.y;

            float aspect = (scaleY / scaleX) / (textureDimensions.y / textureDimensions.x);
            Rect clipRect = rect;

            if (aspect != 1f)
            {
                if (aspect < 1f)
                {
                    float padding = rect.width * (1f - aspect) * 0.5f;
                    clipRect.xMin += padding;
                    clipRect.xMax -= padding;
                }
                else
                {
                    float padding = rect.height * (1f - 1f / aspect) * 0.5f;
                    clipRect.yMin += padding;
                    clipRect.yMax -= padding;
                }
            }
            return clipRect;
        }

        /// <summary>
        /// Checks to see if an area of the screen with a pixel buffer contains the mouse.
        /// </summary>
        /// <param name="screenRect">Screen area to check.</param>
        /// <param name="pixelBuffer">A padded buffer around to Rect.</param>
        /// <returns></returns>
        public static bool RectContainsMouseWithBuffer(Rect screenRect, float pixelBuffer)
        {
            //Extend rect with buffer
            screenRect.y -= pixelBuffer;
            screenRect.height += pixelBuffer * 2;
            screenRect.x -= pixelBuffer;
            screenRect.width += pixelBuffer * 2;
            if (screenRect.Contains(Event.current.mousePosition))
                return true;
            return false;
        }
    }
}