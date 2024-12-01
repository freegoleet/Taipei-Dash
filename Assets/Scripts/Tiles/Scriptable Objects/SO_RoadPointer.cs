using UnityEngine;

namespace Traffic
{
    public enum RoadPointer
    {
        Forward,
        BigRight,
        SmallRight
    }

    [CreateAssetMenu(fileName = "Road Pointer", menuName = "ScriptableObjects/Tiles/Road Pointer", order = 1)]

    public class SO_RoadPointer : ScriptableObject
    {
        [SerializeField]
        private Sprite m_Sprite = null;
        [SerializeField]
        private RoadPointer m_PointerType = new();

        public Sprite Sprite { get => m_Sprite; }
        public RoadPointer PointerType { get => m_PointerType; }
    }
}