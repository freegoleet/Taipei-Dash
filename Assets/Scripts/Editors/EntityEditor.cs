using UnityEngine;

public class EntityEditor : MonoBehaviour
{
    public TileGameplay SelectedTile { get; private set; } = null;
    public TileGameplay TargetTile { get; private set; } = null;
    public Entity Entity { get; private set; } = null;

    private void OnEnable() {
        
    }

    public void Tick(float dt) {


    }

    public void SelectTile(TileGameplay tile) {
        if (SelectedTile != null) {
            UnselectTiles();
            return;
        }
        if(Entity == null) {
            GameObject go = new GameObject();
            Entity = go.AddComponent<Entity>();
        }
        Entity.Initialize(new SO_Entity());
        SelectedTile = tile; 
        SelectedTile.ToggleHighlight(true);
        Entity.Tile = SelectedTile;
    }

    public void SetTargetTile(TileGameplay tile) {
        TargetTile = tile;
        FindPath();
    }

    public void UnselectTiles() {
        SelectedTile.ToggleHighlight(false);
        SelectedTile = null;
        TargetTile = null;
        Entity.Tile = null;
        Entity.Pathfinding.ResetPath();
    }

    public void FindPath() {
        if (TargetTile == null) {
            return;
        }

        Entity.Pathfinding.SetPath(PathfindingUtilities.GetPathTiles(SelectedTile, TargetTile, SO_Entity.ePathfindType.Car, GameService.Instance.GridManager));
    }
}
