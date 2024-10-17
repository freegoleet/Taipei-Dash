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

        private TileType CurrentCursorTileType { get; set; }

        public void UpdateCursorPos(Tile tile) {
            HoveredTileGridIndex = tile.GridPosition;
            transform.position = tile.transform.position;
        }

        public Tile GetCursorTile() {
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
                    break;
            }
            return null;
        }

        public void SelectNewTileType(SO_Tile data) {
            TileType type = data.GetTileType();
            if (CurrentTile != null) {
                var tile = GetCursorTile();
                if (tile.TileType != data.GetTileType()) {
                    CurrentTile.gameObject.SetActive(false);
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

            ToggleShowCursorTile(true, type);
        }

        public void ToggleShowCursorTile(bool show, TileType tileType = TileType.None) {
            Cursor.gameObject.SetActive(show);

            if (tileType == CurrentCursorTileType) {
                return;
            }

            switch (CurrentCursorTileType) {
                case TileType.Road:
                    RoadTile.gameObject.SetActive(false);
                    break;
                case TileType.Autofit:
                    SidewalkTile.gameObject.SetActive(false);
                    break;
                case TileType.Deco:
                    DecoTile.gameObject.SetActive(false);
                    break;
                case TileType.Gameplay:
                    GameplayTile.gameObject.SetActive(false);
                    break;
                case TileType.None:
                    break;
            }

            if (show == false) {
                return;
            }

            switch (tileType) {
                case TileType.Road:
                    RoadTile.gameObject.SetActive(true);
                    break;
                case TileType.Autofit:
                    SidewalkTile.gameObject.SetActive(true);
                    break;
                case TileType.Deco:
                    DecoTile.gameObject.SetActive(true);
                    break;
                case TileType.Gameplay:
                    GameplayTile.gameObject.SetActive(true);
                    break;
                case TileType.None:
                    return;
            }

            CurrentCursorTileType = tileType;
        }
    }
}