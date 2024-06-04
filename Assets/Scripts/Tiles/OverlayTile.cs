using UnityEngine;

public class OverlayTile : MonoBehaviour
{
    public void ToggleShow(bool show)
    {
        gameObject.SetActive(show);
    }
}
