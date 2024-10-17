using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Neighbors : MonoBehaviour
{
    [SerializeField]
    private RawImage m_UpLeft = null;
    [SerializeField]
    private RawImage m_Up = null;
    [SerializeField]
    private RawImage m_UpRight = null;
    [SerializeField]
    private RawImage m_Left = null;
    [SerializeField]
    private RawImage m_Right = null;
    [SerializeField]
    private RawImage m_DownLeft = null;
    [SerializeField]
    private RawImage m_Down = null;
    [SerializeField]
    private RawImage m_DownRight = null;

    private Dictionary<Vector2Int, RawImage> m_Images = null;

    public void Initialize() {
        m_Images = new Dictionary<Vector2Int, RawImage>() {
            { new Vector2Int(-1, 1),  m_UpLeft },
            { new Vector2Int(0, 1),  m_Up },
            { new Vector2Int(1, 1),  m_UpRight },
            { new Vector2Int(-1, 0),  m_Left },
            { new Vector2Int(1, 0),  m_Right },
            { new Vector2Int(-1, -1),  m_DownLeft },
            { new Vector2Int(0, -1),  m_Down },
            { new Vector2Int(1, -1),  m_DownRight },
        };
    }


    public void ShowNeighbors(List<Vector2Int> directions) {
        foreach (KeyValuePair<Vector2Int, RawImage> kvp in m_Images) {
            if(directions.Contains(kvp.Key)) {
                m_Images[kvp.Key].color = Color.green;
                continue;
            }
            m_Images[kvp.Key].color = Color.red;
            continue;
        }
    }
}
