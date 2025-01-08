//using System;
//using Traffic;
//using UnityEngine;

//public static class TilemapEditorUtilities
//{
//    private static SO_Tile m_CurrentTileData = null;
//    private static UI_Item_Button_Tile m_CurrentButton = null;

//    public static Action<UI_Item_Button_Tile> OnUITilePressed = null;

//    static TilemapEditorUtilities()
//    {
//        OnUITilePressed = UITilePressed;
//    }

//    public static bool HoverTile(TileGameplay hoveredTile)
//    {
//        if (hoveredTile == null)
//        {
//            return false;
//        }

//        if (Input.GetMouseButtonDown(0) == true)
//        {
//            if (hoveredTile == null)
//            {
//                return false;
//            }

//            if (m_CurrentTileData != null)
//            {
//                ReplaceTile(hoveredTile);
//                return true;
//            }

//            m_CurrentTileData = hoveredTile.Data;
//        }

//        if (hoveredTile == TilemapUtilities.PreviousHoverTile)
//        {
//            return false;
//        }

//        TilemapUtilities.PreviousHoverTile = hoveredTile;

//        return false;
//    }

//    private static void UITilePressed(UI_Item_Button_Tile button)
//    {
//        if (m_CurrentButton != null)
//        {
//            m_CurrentButton.OnDeselect();
//        }

//        m_CurrentButton = button;
//        m_CurrentTileData = button.Data;
//    }

//    private static void ReplaceTile(TileGameplay tileToReplace)
//    {
//        if (tileToReplace.Data == m_CurrentTileData)
//        {
//            return;
//        }

//        if (tileToReplace.GetComponent<UI_Item_Button<SO_Tile>>() == true)
//        {
//            return;
//        }

//        if (tileToReplace.Entity != null)
//        {
//            //if (m_CurrentTileData.Traversable.Equals(SO_Tile.eTraversable.Untraversable))
//            //{
//                //GameServices.Instance.EntityEditorUtilities.DeletePlayerEntity(tileToReplace);
//            //}
//        }

//        tileToReplace.Initialize(m_CurrentTileData as SO_TileGameplay);
//    }
//}
