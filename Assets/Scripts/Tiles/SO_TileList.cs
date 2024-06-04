using System.Collections.Generic;
using UnityEngine;

namespace Traffic
{
    [CreateAssetMenu(fileName = "Tile List", menuName = "ScriptableObjects/Tiles/Tile List", order = 1)]
    public class SOTile_List : ScriptableObject
    {
        [SerializeField]
        private List<SO_Tile> m_Tiles = new List<SO_Tile>();
        public List<SO_Tile> Tiles { get { return m_Tiles; } }

        [SerializeField]
        private List<SO_RoadPointer> m_Pointers = new List<SO_RoadPointer>();
        public List<SO_RoadPointer> Pointers { get { return m_Pointers; } }

        public Dictionary<Directions, SO_RoadPointer> GetPointers() {
            Dictionary<Directions, SO_RoadPointer> dict = new();

            foreach (var item in Pointers) {
                dict.Add(item.Direction, item);
            }

            return dict;
        }
    }
}