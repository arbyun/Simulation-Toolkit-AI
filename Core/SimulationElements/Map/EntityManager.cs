using RogueSharp;
using SimArena.Core.Entities;

namespace SimArena.Core.SimulationElements.Map;

public class EntityManager
{
    private Dictionary<Entity, Cell> _entities;

    public EntityManager()
    {
        _entities = new Dictionary<Entity, Cell>();
    }
    
    public Entity Get(Cell cell)
    {
        return _entities.FirstOrDefault(x => x.Value == cell).Key;
    }

    public IEnumerable<T> GetEntities<T>() where T : Entity?
    {
        return _entities.Where(x => x.Key is T).Select(x => (T)x.Key);
    }
    
    public void Set(Entity entity, Cell cell)
    {
        _entities[entity] = cell;
    }

    public void Remove(Entity entity)
    {
        _entities.Remove(entity);
    }
    
    public void Clear()
    {
        _entities.Clear();
    }

    public Entity? Get(Guid entityId)
    {
        return _entities.Keys.FirstOrDefault(x => x.Id.Equals(entityId));
    }
}