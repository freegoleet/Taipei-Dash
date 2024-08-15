using System.Collections.Generic;
using UnityEngine;

namespace Traffic
{
    [CreateAssetMenu(fileName = "Road Pointer", menuName = "ScriptableObjects/Tiles/Road Pointer", order = 1)]

    public class SO_RoadPointer : ScriptableObject
    {
        [SerializeField]
        private Sprite m_Sprite = null;
        [SerializeField]
        private List<Directions> m_Directions = new();

        public Sprite Texture2D { get => m_Sprite; }
        public List<Directions> Directions { get => m_Directions; }

        public Dictionary<List<Directions>, Sprite> GetTexture() {
            return new Dictionary<List<Directions>, Sprite>() { { Directions, m_Sprite } };
        }
    }
}