using UnityEditor;
using UnityEngine;
using MyLib.Shared.Database;

namespace MyLib.EditorTools
{
    /// <summary>
    /// A base database inspector to inherit when creating database editors.
    /// </summary>
    /// <typeparam name="S">The database file type.</typeparam>
    /// <typeparam name="U">The type of asset the database holds.</typeparam>
    public abstract class DatabaseInspectorBase<S, U> : Editor
        where S : ScriptableObject, IDatabaseFile
        where U : UnityEngine.Object
    {
        protected DatabaseWindowEditor<S,U>  pEditorGUI;
        protected S pThisDatabase;

        /// <summary>
        /// Reloads the gui of this inspector.
        /// </summary>
        public abstract void ReloadGUI();

        public override void OnInspectorGUI()
        {
            if (pThisDatabase != target || pEditorGUI == null)
            {
                pThisDatabase = target as S;
                //Changing to a new database, Reset editor and load new values
                ReloadGUI();
            }

            pEditorGUI.CurDatabase = pThisDatabase;

            int selectionId16 = pEditorGUI.DrawLeftColumn<U>(new Rect(0, 0, EditorGUIUtility.currentViewWidth - 56, 256));

            if (selectionId16 != -1)
                pEditorGUI.CurAsset = pThisDatabase.DatabaseData.GetAsset((short)selectionId16);


            pEditorGUI.DrawRightColumn(new Rect(0, 0, EditorGUIUtility.currentViewWidth - 56f, 480f));

            pEditorGUI.ProcessDragAssets();
        }

        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            if (pEditorGUI != null && pThisDatabase != null)
                pEditorGUI.DrawPreviewGUI(r, background, pThisDatabase);
        }
    }
}