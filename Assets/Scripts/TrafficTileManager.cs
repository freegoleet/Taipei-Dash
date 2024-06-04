using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class TrafficTileManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap m_Tilemap = null;
    [SerializeField]
    private GameObject m_Arrow = null;
    [SerializeField]
    private Transform m_Canvas = null;

    private List<GameObject> m_Arrows = new List<GameObject>();
    private bool m_ShowArrows = false;

    public void ToggleDirections() {
        if (m_ShowArrows == true) {
            ShowDirections();
            m_ShowArrows = false;
        }
        else {
            HideDirections();
            m_ShowArrows = true;
        }
    }

    public void ShowDirections() {
        var rect = m_Arrow.GetComponent<RectTransform>().rect;
        Vector3 arrowOffset = new Vector3(rect.width / 2, rect.height / 2, 0);

        var bounds = m_Tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++) {
            for (int y = bounds.yMin; y < bounds.yMax; y++) {
                Vector3Int localPlace = new Vector3Int(x, y, (int)m_Tilemap.transform.position.y);
                Quaternion angle = m_Tilemap.GetTransformMatrix(localPlace).rotation;
                Vector3 place = m_Tilemap.CellToWorld(localPlace);

                if (m_Tilemap.HasTile(localPlace)) {
                    var arrow = Instantiate(m_Arrow, place + arrowOffset, angle);
                    arrow.transform.SetParent(m_Canvas);
                    m_Arrows.Add(arrow);
                }
                else {
                    //No tile at "place"
                }
            }
        }
    }

    public void HideDirections() {
        for (int i = 0; i < m_Arrows.Count; i++) {
            DestroyImmediate(m_Arrows[i]);
        }

        m_Arrows.Clear();
    }
}
