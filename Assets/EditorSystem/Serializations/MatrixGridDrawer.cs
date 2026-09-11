using UnityEditor;
using UnityEngine;

namespace Kosha82.EditorSystem.Drawers
{
    public class MatrixGridDrawer
    {
        private int selectedIndex = 0;

        private SerializedProperty gridSizeProp;
        private SerializedProperty arrayProp;

        private string subelementName;
        private string iconPropertyName;

        public MatrixGridDrawer(SerializedObject serializedObject, string arrayName = "ingredients", string sizeName = "gridSize",  string iconPropertyName = null, string subelementName = null)
        {
            gridSizeProp = serializedObject.FindProperty(sizeName);
            arrayProp = serializedObject.FindProperty(arrayName);
            this.subelementName = subelementName;
            this.iconPropertyName = iconPropertyName;
        }

        public void Draw()
        {
            if (gridSizeProp == null || arrayProp == null)
            {
                EditorGUILayout.HelpBox($"Cannot find properties! Check if '{gridSizeProp?.name}' and '{arrayProp?.name}' exist in your script.", MessageType.Error);
                return;
            }

            Vector2Int size = gridSizeProp.vector2IntValue;
            arrayProp.arraySize = size.x * size.y;

            EditorGUILayout.LabelField("Recipe Pattern", EditorStyles.boldLabel);
            DrawSizeField();
            EditorGUILayout.Space();
            DrawGrid(size);
            DrawSelectedSlot(size);
        }

        private void DrawSizeField()
        {
            EditorGUILayout.PropertyField(gridSizeProp, new GUIContent("Grid Size"));
        }

        private void DrawGrid(Vector2Int size)
        {
            GUILayout.BeginVertical("box");
            for (int y = 0; y < size.y; y++)
            {
                GUILayout.BeginHorizontal();
                for (int x = 0; x < size.x; x++)
                {
                    int index = x + (y * size.x);
                    DrawCell(index);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void DrawCell(int index)
        {
            SerializedProperty elementProp = arrayProp.GetArrayElementAtIndex(index);
            
            bool hasData = true;
            if (!string.IsNullOrEmpty(subelementName))
            {
                SerializedProperty dataProp = elementProp.FindPropertyRelative(subelementName);
                if (dataProp != null)
                {
                    if (dataProp.propertyType == SerializedPropertyType.ObjectReference) 
                        hasData = dataProp.objectReferenceValue != null;
                }
                else
                {
                    hasData = false;
                }
            }

            Color defaultColor = GUI.backgroundColor;
            if (selectedIndex == index) GUI.backgroundColor = Color.green;
            else if (hasData) GUI.backgroundColor = new Color(0.7f, 0.9f, 1f);

            bool hasIcon = false;
            GUIContent buttonContent = new GUIContent(hasData ? "[ Data ]" : "[ Empty ]");

            if (hasData && !string.IsNullOrEmpty(iconPropertyName))
            {
                SerializedProperty iconProp = elementProp.FindPropertyRelative(iconPropertyName);
                if (iconProp != null && iconProp.propertyType == SerializedPropertyType.ObjectReference && iconProp.objectReferenceValue != null)
                {
                    Texture2D icon = iconProp.objectReferenceValue as Texture2D;
                    if (icon != null)
                    {
                        buttonContent = new GUIContent("", icon);
                        hasIcon = true;
                    }
                }
            }

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = 40,
                imagePosition = hasIcon ? ImagePosition.ImageOnly : ImagePosition.TextOnly
            };

            if (GUILayout.Button(buttonContent, buttonStyle, GUILayout.Width(40), GUILayout.Height(40)))
            {
                selectedIndex = index;
            }

            GUI.backgroundColor = defaultColor;
        }

        private void DrawSelectedSlot(Vector2Int size)
        {
            if (selectedIndex >= arrayProp.arraySize) return;

            int selX = selectedIndex % size.x;
            int selY = selectedIndex / size.x;

            GUILayout.BeginVertical("helpbox");
            EditorGUILayout.LabelField($"Editing Slot ({selX}, {selY})", EditorStyles.boldLabel);

            SerializedProperty selectedProp = arrayProp.GetArrayElementAtIndex(selectedIndex);
            EditorGUILayout.PropertyField(selectedProp, true);

            GUILayout.EndVertical();
        }
    }
}