public class UI_Item_Button_Tile : UI_Item_Button<SO_Tile>
{
    public override void Initialize(SO_Tile data)
    {
        m_Data = data;
        m_Img.sprite = data.Sprite;
    }

    public override void ButtonPressed()
    {
        base.ButtonPressed();
        GameServices.Instance.MapEditor.OnUITilePressed.Invoke(this);
    }
}
