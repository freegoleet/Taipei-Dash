using UnityEditor;
using UnityEngine;

namespace Traffic
{
    [CustomEditor(typeof(TilemapEditor))]
    public class TilemapWindowEditor : Editor
    {
        private TilemapEditor window = null;
        private Tile m_HoveredTile = null;

        private bool m_ShowPointers = true;
        private bool m_ShowPointersOld = true;

        private int m_LayerToEdit = 0;
        private int m_LayerToEditOld = 1;
        private string[] m_LayerNames = new string[] { "Tiles", "Deco 1", "Deco 2", "Deco 3" };

        private int m_CursorTileIndex = 0;
        private int m_CursorTileIndexOld = 1;

        private bool[] m_LayersToShow = new bool[] { true, true, true };

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            window = (TilemapEditor)target;
        }

    }
}