using System;
using UnityEngine;

public class TilemapEditorUtilities
{
    private SO_Tile m_CurrentTileData = null;
    private UI_Item_Button_Tile m_CurrentButton = null;
    private TilemapService m_TilemapService = null;

    public Action<UI_Item_Button_Tile> OnUITilePressed = null;

    public TilemapEditorUtilities()
    {
        m_TilemapService = GameServices.Instance.TilemapService;
        OnUITilePressed = UITilePressed;
    }

    public bool HoverTile(Tile hoveredTile)
    {
        if (hoveredTile == null)
        {
            return false;
        }

        if (Input.GetMouseButtonDown(0) == true)
        {
            if (hoveredTile == null)
            {
                return false;
            }

            if (m_CurrentTileData != null)
            {
                ReplaceTile(hoveredTile);
                return true;
            }

            m_CurrentTileData = hoveredTile.Data;
        }

        if (hoveredTile == m_TilemapService.PreviousHoverTile)
        {
            return false;
        }

        m_TilemapService.PreviousHoverTile = hoveredTile;

        return false;
    }

    private void UITilePressed(UI_Item_Button_Tile button)
    {
        if (m_CurrentButton != null)
        {
            m_CurrentButton.OnDeselect();
        }

        m_CurrentButton = button;
        m_CurrentTileData = button.Data;
    }

    private void ReplaceTile(Tile tileToReplace)
    {
        if (tileToReplace.Data == m_CurrentTileData)
        {
            return;
        }

        if (tileToReplace.GetComponent<UI_Item_Button<SO_Tile>>() == true)
        {
            return;
        }

        if (tileToReplace.Entity != null)
        {
            //if (m_CurrentTileData.Traversable.Equals(SO_Tile.eTraversable.Untraversable))
            //{
                //GameServices.Instance.EntityEditorUtilities.DeletePlayerEntity(tileToReplace);
            //}
        }

        tileToReplace.Initialize(m_CurrentTileData);
    }
}
