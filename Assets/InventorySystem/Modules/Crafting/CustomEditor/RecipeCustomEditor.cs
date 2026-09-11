#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Kosha82.EditorSystem.Drawers;

namespace Kosha82.InventorySystem.Crafting.Editor
{
    [CustomEditor(typeof(Recipe))]
    public class CraftingRecipeEditor : UnityEditor.Editor
    {
        private MatrixGridDrawer gridDrawer;

        private void OnEnable()
        {
            gridDrawer = new MatrixGridDrawer(serializedObject, "ingredients", "gridSize", "icon", "criterias");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            gridDrawer.Draw();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif