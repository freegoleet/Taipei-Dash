using System.Collections.Generic;
using Traffic;
using UnityEngine;

public class EntityEditor : MonoBehaviour
{
    [SerializeField]
    private SO_CarData m_CarData = null;
    public TileGameplay SelectedStartTile { get; private set; } = null;
    public TileGameplay TargetTile { get; private set; } = null;
    public Entity Entity { get; private set; } = null;
    public CarEntity Car { get; private set; }
    public List<TileGameplay> Path { get; private set; } = new();

    public void Tick(float dt) {
    }

    public void SelectStartTile(TileGameplay tile) {
        if (SelectedStartTile != null) {
            UnselectTiles();
            return;
        }
        if (Entity == null) {
            //GameObject go = new GameObject();
            //Entity = go.AddComponent<Entity>();
        }
        if (tile is TileRoad tr) {
            Car = Instantiate(m_CarData.CarPrefab);
            //Car.Initialize();
            Car.SetupCar(m_CarData);
            Entity = Car;
        }

        Entity.Initialize();
        EntityManager.AddNewEntity(Entity);
        SelectedStartTile = tile;
        SelectedStartTile.ToggleHighlight(true);
        Entity.Tile = SelectedStartTile;
        Entity.transform.position = SelectedStartTile.transform.position;
        Entity.transform.rotation = Quaternion.Euler(TrafficUtilities.GetDirectionAsQuaternion(SelectedStartTile.Facing));
    }

    public void SelectTargetTile(TileGameplay tile) {
        if(TargetTile == null) {
            return;
        }

        Entity.EntityControllerNPC.MoveEntity(Entity.Pathfinding.CurrentPathTiles);
    }

    public void ShowPathToHoveredTile(TileGameplay tile) {
        TargetTile = tile;
        FindPath();
    }

    public void UnselectTiles() {
        SelectedStartTile.ToggleHighlight(false);
        SelectedStartTile = null;
        TargetTile = null;
        Entity.Tile = null;
        Entity.Pathfinding.ResetPath();
        EntityManager.RemoveEntity(Entity);
        DestroyImmediate(Entity.gameObject);
        Entity = null;

    }

    public void FindPath() {
        if (TargetTile == null) {
            return;
        }

        Entity.Pathfinding.SetPath(PathfindingUtilities.GetPathTiles(SelectedStartTile, TargetTile, PathfindingUtilities.PathfindType.Car, GameService.Instance.GridManager));
    }
}
