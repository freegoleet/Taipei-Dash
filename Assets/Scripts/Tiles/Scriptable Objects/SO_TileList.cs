using System.Collections.Generic;
using UnityEngine;

namespace Traffic
{
    [CreateAssetMenu(fileName = "Tile List", menuName = "ScriptableObjects/Tiles/Tile List", order = 1)]
    public class SOTile_List : ScriptableObject
    {
        [Header("Prefabs")]
        [SerializeField]
        private TileSidewalk m_SidewalkTile = null;
        [SerializeField]
        private TileRoad m_RoadTile = null;
        [SerializeField]
        private TileDeco m_DecoTile = null;
        [SerializeField]
        private TileGameplay m_GameplayTile = null;
        [SerializeField]
        private TrafficLight m_TrafficLight = null;

        [Header("Lists")]
        [SerializeField]
        private List<SO_TileGameplay> m_Tiles = new List<SO_TileGameplay>();
        [SerializeField]
        private List<SO_TileDecorative> m_DecoTiles = new List<SO_TileDecorative>();
        [SerializeField]
        private List<SO_RoadPointer> m_Pointers = new List<SO_RoadPointer>();
        [SerializeField]
        private List<SO_RoadLine> m_Lines = new List<SO_RoadLine>();
        [SerializeField]
        private SO_TileRoad m_SOTileRoad = null;
        [SerializeField]
        private SO_TileSidewalk m_SOTileSidewalk = null;


        public TileSidewalk SidewalkTile { get => m_SidewalkTile; }
        public TileRoad RoadTile { get => m_RoadTile; }
        public TileDeco DecoTile { get => m_DecoTile; }
        public TileGameplay GameplayTile { get => m_GameplayTile; }
        public TrafficLight TrafficLight { get => m_TrafficLight; }

        public List<SO_TileGameplay> GameplayTiles { get { return m_Tiles; } }
        public List<SO_TileDecorative> DecoTiles { get => m_DecoTiles; }
        public List<SO_RoadPointer> Pointers { get { return m_Pointers; } }
        public List<SO_RoadLine> Lines { get { return m_Lines; } }

        public Dictionary<LineType, SO_RoadLine> RoadLines = null;

        public Dictionary<RoadPointer, SO_RoadPointer> GetPointers() {
            Dictionary<RoadPointer, SO_RoadPointer> pointers = new();
            foreach (var pointer in Pointers) {
                pointers.TryAdd(pointer.PointerType, pointer);
            }

            return pointers;
        }

        public Dictionary<LineType, SO_RoadLine> GetRoadlines() {
            if(RoadLines != null) {
                return RoadLines;
            }
            RoadLines = new();
            foreach (var item in Lines) {
                RoadLines.Add(item.LineType, item);
            }
            return RoadLines;
        }

        public NodeBase GetTilePrefab(TileType tileType) {
            switch (tileType) {
                case TileType.Road:
                    return RoadTile;
                case TileType.Autofit:
                    return SidewalkTile;
                case TileType.Deco:
                    return DecoTile;
                case TileType.Gameplay:
                    return GameplayTile;
                case TileType.None:
                    return null;
            }
            return null;
        }

        public List<SO_Tile> GetSOTileListByType(TileType tileType) {
            List<SO_Tile> list = new List<SO_Tile>();
            for (var i = 0; i < GameplayTiles.Count; i++) {
                if (GameplayTiles[i].TileType != tileType) {
                    continue;
                }
                list.Add(GameplayTiles[i]);
            }
            return list;
        }

        public SO_Tile GetSOByType(TileType tileType) {
            switch (tileType) {
                case TileType.Gameplay:
                    break;
                case TileType.Road:
                    return m_SOTileRoad;
                case TileType.Autofit:
                    return m_SOTileSidewalk;
                case TileType.Deco:
                    break;
                case TileType.None:
                    break;
                default:
                    break;
            }
            return null;
        }
    }
}