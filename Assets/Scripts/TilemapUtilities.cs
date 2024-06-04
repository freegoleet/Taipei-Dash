public enum eTilemapMode
{
    Pathfinding,
    LineOfSight
}

public class TilemapUtilities
{
    private TilemapService m_TilemapService = null;
    private eTilemapMode m_CurrentTilemapMode = eTilemapMode.Pathfinding;

    public TilemapUtilities()
    {
        m_TilemapService = GameServices.Instance.TilemapService;
    }

    public virtual bool HoverTile(Tile hoveredTile)
    {
        if (hoveredTile == null)
        {
            return false;
        }

        if (m_TilemapService.HoverTile != hoveredTile)
        {
            m_TilemapService.HoverTile = hoveredTile;
            m_TilemapService.HoverGO.transform.position = m_TilemapService.HoverTile.transform.position;
        }

        if (hoveredTile == m_TilemapService.PreviousHoverTile)
        {
            return true;
        }

        m_TilemapService.PreviousHoverTile = hoveredTile;

        HoverNewTile();

        return true;
    }

    public void HoverNewTile()
    {
        switch (m_CurrentTilemapMode)
        {
            case eTilemapMode.Pathfinding:
                break;
            case eTilemapMode.LineOfSight:
                break;
        }
    }
}
