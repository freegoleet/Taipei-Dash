using System.Collections.Generic;

public static class EntityManager
{
    public static List<Entity> Entities { get; private set; } = new List<Entity>();
    public static Dictionary<ulong, Entity> EntityById { get; private set; } = new Dictionary<ulong, Entity>();
    public static Dictionary<ulong, Entity> EntityByPlayerId { get; private set; } = new Dictionary<ulong, Entity>();

    public static void Tick(float dt)
    {
        foreach (var entity in Entities)
        {
            entity.Tick(dt);
        }
    }

    public static void Clear()
    {
        Entities.Clear();
        EntityById.Clear();
        EntityByPlayerId.Clear();
    }

    public static void AddNewEntity(Entity entity)
    {
        if (EntityById.ContainsKey(entity.Id))
        {
            return;
        }

        Entities.Add(entity);
        EntityById.Add(entity.Id, entity);
    }

    public static void RemoveEntity(Entity entity)
    {
        if (entity == null) { return; }
        Entities.Remove(entity);
        EntityById.Remove(entity.Id);
    }

    public static void RemoveEntity(ulong id)
    {
        if (GetEntityById(id) is Entity entity == false)
        {
            return;
        }

        Entities.Remove(entity);
        EntityById.Remove(id);
    }

    public static Entity GetEntityById(ulong id)
    {
        return EntityById.ContainsKey(id) ? EntityById[id] : null;
    }
}