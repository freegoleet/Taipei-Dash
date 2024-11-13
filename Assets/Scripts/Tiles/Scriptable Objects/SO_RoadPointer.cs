using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Traffic
{
    [CreateAssetMenu(fileName = "Road Pointer", menuName = "ScriptableObjects/Tiles/Road Pointer", order = 1)]

    public class SO_RoadPointer : ScriptableObject
    {
        [SerializeField]
        private Sprite m_Sprite = null;
        [SerializeField]
        private List<Direction> m_Directions = new();

        public Sprite Sprite { get => m_Sprite; }
        public HashSet<Direction> Directions { get => m_Directions.ToHashSet(); }

        public Dictionary<HashSet<Direction>, Sprite> GetTexture() {
            return new Dictionary<HashSet<Direction>, Sprite>() { { Directions, m_Sprite } };
        }
    }
}