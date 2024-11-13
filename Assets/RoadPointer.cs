using Traffic;
using UnityEngine;

public class RoadPointer : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer m_ImgPointer = null;

    public void ToggleShow(bool show) {
        m_ImgPointer.gameObject.SetActive(show);
    }

    public void SetDirections(SO_RoadPointer roadPointer) {
        m_ImgPointer.sprite = roadPointer.Sprite;
    }

}
