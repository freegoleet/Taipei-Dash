using UnityEngine;

public class TileUtilities
{
    private TilemapService m_TilemapService = null;

    public TileUtilities()
    {
        m_TilemapService = GameServices.Instance.TilemapService;
    }

    public bool HoverTile(Tile hoveredTile)
    {
        if (hoveredTile == null)
        {
            return false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            //if (m_PlayerEntity != null)
            //{
            //    m_PlayerEntity.gameObject.SetActive(true);
            //    m_PlayerEntity.transform.position = m_HoverTile.transform.position;
            //    m_StartTile = m_HoverTile;
            //}
        }

        if (hoveredTile == m_TilemapService.PreviousHoverTile)
        {
            return false;
        }

        m_TilemapService.PreviousHoverTile = hoveredTile;

        return true;
    }
}
