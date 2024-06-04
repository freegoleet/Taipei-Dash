using System.Collections.Generic;

public class EntityManager
{
    public List<Entity> Entities { get; private set; } = new List<Entity>();
    public Dictionary<ulong, Entity> EntityById { get; private set; } = new Dictionary<ulong, Entity>();
    public Dictionary<ulong, Entity> EntityByPlayerId { get; private set; } = new Dictionary<ulong, Entity>();

    public void Tick(float dt)
    {
        foreach (var entity in Entities)
        {
            entity.Tick(dt);
        }
    }

    public void Clear()
    {
        Entities.Clear();
        EntityById.Clear();
        EntityByPlayerId.Clear();
    }

    public void AddNewEntity(Entity entity)
    {
        if (EntityById.ContainsKey(entity.Id))
        {
            return;
        }

        Entities.Add(entity);
        EntityById.Add(entity.Id, entity);
    }

    public void RemoveEntity(Entity entity)
    {
        if (entity == null) { return; }
        Entities.Remove(entity);
        EntityById.Remove(entity.Id);
    }

    public void RemoveEntity(ulong id)
    {
        if (GetEntityById(id) is Entity entity == false)
        {
            return;
        }

        Entities.Remove(entity);
        EntityById.Remove(id);
    }

    public Entity GetEntityById(ulong id)
    {
        return EntityById.ContainsKey(id) ? EntityById[id] : null;
    }
}