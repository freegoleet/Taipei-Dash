using UnityEngine;

[CreateAssetMenu(fileName = "Tile Options", menuName = "ScriptableObjects/Tile Options", order = 1)]
public class SOTileSettings : ScriptableObject
{
    public enum eHighlightType
    {
        Path,
        PathCantReach,
    }

    [Header("Highlight Colors")]
    [SerializeField]
    private Color m_Path = Color.blue;
    [SerializeField]
    private Color m_PathCantReach = Color.yellow;

    public Color Path { get { return m_Path; } }
    public Color PathCantReach { get { return m_PathCantReach; } }

    public Color? GetColor(eHighlightType color)
    {
        switch (color)
        {
            case eHighlightType.Path:
                return m_Path;
            case eHighlightType.PathCantReach:
                return m_PathCantReach;
        }

        return null;
    }
}
