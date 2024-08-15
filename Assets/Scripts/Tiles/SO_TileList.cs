using System.Collections.Generic;
using UnityEngine;

namespace Traffic
{
    [CreateAssetMenu(fileName = "Tile List", menuName = "ScriptableObjects/Tiles/Tile List", order = 1)]
    public class SOTile_List : ScriptableObject
    {
        [Header("Prefabs")]
        [SerializeField]
        private GameObject m_GameplayTile = null;
        [SerializeField]
        private GameObject m_RoadTile = null;
        [SerializeField]
        private GameObject m_DecoTile = null;

        [Header("Lists")]
        [SerializeField]
        private List<SO_TileGameplay> m_Tiles = new List<SO_TileGameplay>();
        [SerializeField]
        private List<SO_TileRoad> m_RoadTiles = new List<SO_TileRoad>();
        [SerializeField]
        private List<SO_TileDecorative> m_DecoTiles = new List<SO_TileDecorative>();
        [SerializeField]
        private List<SO_RoadPointer> m_Pointers = new List<SO_RoadPointer>();
        [SerializeField]
        private List<SO_RoadLine> m_Lines = new List<SO_RoadLine>();

        public GameObject GameplayTile { get => m_GameplayTile; set => m_GameplayTile = value; }
        public GameObject RoadTile { get => m_RoadTile; set => m_RoadTile = value; }
        public GameObject DecoTile { get => m_DecoTile; set => m_DecoTile = value; }

        public List<SO_TileGameplay> Tiles { get { return m_Tiles; } }
        public List<SO_TileRoad> RoadTiles { get => m_RoadTiles; set => m_RoadTiles = value; }
        public List<SO_TileDecorative> DecoTiles { get => m_DecoTiles; set => m_DecoTiles = value; }
        public List<SO_RoadPointer> Pointers { get { return m_Pointers; } }
        public List<SO_RoadLine> Lines { get { return m_Lines; } }

        public Dictionary<List<Directions>, SO_RoadPointer> GetPointers() {
            Dictionary<List<Directions>, SO_RoadPointer> dict = new();

            foreach (var pointer in Pointers) {
                dict.Add(pointer.Directions, pointer);
            }

            return dict;
        }

        public Dictionary<LineType, SO_RoadLine> GetRoadlines() {
            Dictionary<LineType, SO_RoadLine> dict = new();

            foreach (var item in Lines) {
                dict.Add(item.LineType, item);
            }

            return dict;
        }
    }
}