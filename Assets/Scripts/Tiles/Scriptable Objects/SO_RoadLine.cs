using UnityEngine;

namespace Traffic
{
    public enum LineType
    {
        LaneSeparatorWhole,
        LaneSeparatorDotted,
        DirectionSeparatorWhole,
        DirectionSeparatorDotted,
        Stop
    }

    [CreateAssetMenu(fileName = "Road Line", menuName = "ScriptableObjects/Tiles/Road Line", order = 1)]
    public class SO_RoadLine : ScriptableObject
    {
        [SerializeField]
        private Sprite m_Sprite = null;
        [SerializeField]
        private Color m_Color = Color.white;
        [SerializeField]
        private LineType m_LineType = LineType.LaneSeparatorWhole;

        public Sprite Sprite { get => m_Sprite; }
        public Color Color { get => m_Color; set => m_Color = value; }
        public LineType LineType { get => m_LineType; }
    }
}