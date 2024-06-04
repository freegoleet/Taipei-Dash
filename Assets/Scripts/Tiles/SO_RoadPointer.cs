using System.Collections.Generic;
using UnityEngine;

namespace Traffic
{
    [CreateAssetMenu(fileName = "Road Pointer", menuName = "ScriptableObjects/Tiles/Road Pointer", order = 1)]

    public class SO_RoadPointer : ScriptableObject
    {
        [SerializeField]
        private Texture2D m_Texture2D = null;
        [SerializeField]
        private Directions m_Direction = Directions.Up;

        public Texture2D Texture2D { get => m_Texture2D; }
        public Directions Direction { get => m_Direction; }

        public Dictionary<Directions, Texture2D> GetTexture() {
            return new Dictionary<Directions, Texture2D>() { { Direction, m_Texture2D } };
        }
    }
}