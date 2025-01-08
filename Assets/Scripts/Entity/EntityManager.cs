using System.Collections.Generic;

public static class EntityManager
{
    public static Dictionary<ulong, Entity> EntityById { get; private set; } = new Dictionary<ulong, Entity>();
    public static Dictionary<ulong, Entity> EntityByPlayerId { get; private set; } = new Dictionary<ulong, Entity>();

    public static void Tick(float dt) {
        foreach (var entity in EntityById) {
            if (entity.Value == null) {
                RemoveEntity(entity.Key);
                break;
            }
            entity.Value.Tick(dt);
        }
    }

    public static void Clear() {
        EntityById.Clear();
        EntityByPlayerId.Clear();
    }

    public static void AddNewEntity(Entity entity) {
        if (EntityById.ContainsKey(entity.Id)) {
            return;
        }
        EntityById.Add(entity.Id, entity);
    }

    public static void RemoveEntity(Entity entity) {
        if (entity == null) { return; }
        EntityById.Remove(entity.Id);
    }

    public static void RemoveEntity(ulong id) {
        if (GetEntityById(id) is Entity entity == false) {
            return;
        }
        EntityById.Remove(id);
    }

    public static Entity GetEntityById(ulong id) {
        return EntityById.ContainsKey(id) ? EntityById[id] : null;
    }
}