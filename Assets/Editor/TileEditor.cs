using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SO_Tile))]
public class TileEditor : Editor
{
    private SO_Tile m_Tile = null; 

    private void OnEnable()
    {
        m_Tile = target as SO_Tile;
    }

    public override void OnInspectorGUI()
    {
        if(m_Tile.Background != null)
        {
            Texture2D texture = AssetPreview.GetAssetPreview(m_Tile.Background);
            GUILayout.Label("", GUILayout.Height(32), GUILayout.Width(32));
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
        }

        base.OnInspectorGUI();
    }
}
