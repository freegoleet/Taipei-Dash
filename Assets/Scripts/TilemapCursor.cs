using UnityEditor;
using UnityEngine;

namespace Traffic
{
    public class TilemapCursor : MonoBehaviour
    {
        [SerializeField]
        private TileRoad m_RoadTile = null;
        [SerializeField]
        private TileSidewalk m_SidewalkTile = null;
        [SerializeField]
        private TileDeco m_DecoTile = null;
        [SerializeField]
        private TileGameplay m_GameplayTile = null;
        [SerializeField]
        private Transform m_Cursor = null;

        public Vector2Int HoveredTileGridIndex = new Vector2Int();
        public Tile CurrentTile { get; private set; }
        public TileRoad RoadTile { get => m_RoadTile; }
        public TileSidewalk SidewalkTile { get => m_SidewalkTile; }
        public TileDeco DecoTile { get => m_DecoTile; }
        public TileGameplay GameplayTile { get => m_GameplayTile; }
        public Transform Cursor { get => m_Cursor; }

        private TileType CurrentCursorTileType { get; set; } = TileType.None;

        public void UpdateCursorPos(Tile tile) {
            HoveredTileGridIndex = tile.GridPosition;
            transform.position = tile.transform.position;
        }

        public Tile GetCurrentCursorTile() {
            switch (CurrentCursorTileType) {
                case TileType.Road:
                    return m_RoadTile;
                case TileType.Autofit:
                    return m_SidewalkTile;
                case TileType.Deco:
                    return m_DecoTile;
                case TileType.Gameplay:
                    return m_GameplayTile;
                case TileType.None:
                    return null;
            }
            return null;
        }

        public void SelectNewTileType(SO_Tile data) {
            TileType type = data.GetTileType();
            if (CurrentTile != null) {
                var cursorTile = GetCurrentCursorTile();
                if(cursorTile != null) {
                    if (cursorTile.TileType != data.GetTileType()) {
                        CurrentTile.gameObject.SetActive(false);
                    }
                }
            }

            switch (type) {
                case TileType.Road:
                    RoadTile.Initialize((SO_TileRoad)data, new Vector2Int(-1, -1), true);
                    CurrentTile = RoadTile;
                    break;
                case TileType.Autofit:
                    SidewalkTile.Initialize((SO_TileSidewalk)data, new Vector2Int(-1, -1), true);
                    CurrentTile = SidewalkTile;
                    break;
                case TileType.Deco:
                    DecoTile.Initialize((SO_TileDecorative)data, new Vector2Int(-1, -1), true);
                    CurrentTile = DecoTile;
                    break;
                case TileType.Gameplay:
                    GameplayTile.Initialize((SO_TileGameplay)data, new Vector2Int(-1, -1), true);
                    CurrentTile = GameplayTile;
                    break;
                case TileType.None:
                    break;
            }

            if (data.Rotatable == false) {
                RoadTile.SetFacing(Direction.Up);
                SidewalkTile.SetFacing(Direction.Up);
                DecoTile.SetFacing(Direction.Up);
                GameplayTile.SetFacing(Direction.Up);
            }

            CurrentCursorTileType = type;
            ToggleShowCursorTile(true, type);
        }

        public void ToggleShowCursor(bool show) {
            Cursor.gameObject.SetActive(show);
        }

        public GameObject GetTileByType(TileType tileType) {
            switch (tileType) {
                case TileType.Road:
                    return RoadTile.gameObject;
                case TileType.Autofit:
                    return SidewalkTile.gameObject;
                case TileType.Deco:
                    return DecoTile.gameObject;
                case TileType.Gameplay:
                    return GameplayTile.gameObject;
            }
            return null;
        }

        public void ToggleShowCursorTile(bool show, TileType tileType = TileType.None) {
            if(CurrentCursorTileType == TileType.None) {
                return;
            }
            if(tileType == TileType.None) {
                GetTileByType(CurrentCursorTileType).SetActive(show);
                return;
            }
            GetTileByType(CurrentCursorTileType).SetActive(!show);
            GetTileByType(tileType).SetActive(show);
            CurrentCursorTileType = tileType;
        }
    }
}