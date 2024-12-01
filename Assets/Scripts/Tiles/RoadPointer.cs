using Traffic;
using UnityEngine;

public class RoadPointer : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer m_ImgForward = null;
    [SerializeField]
    private SpriteRenderer m_ImgLeft = null;
    [SerializeField]
    private SpriteRenderer m_ImgRight = null;

    public void ToggleShow(bool show) {
        m_ImgForward.gameObject.SetActive(show);
        m_ImgLeft.gameObject.SetActive(show);
        m_ImgRight.gameObject.SetActive(show);
    }

    public void ToggleShow(Direction dir, bool show) {
        switch (dir) {
            case Direction.Up:
                m_ImgForward.gameObject.SetActive(show);
                break;
            case Direction.Right:
                m_ImgRight.gameObject.SetActive(show);
                break;
            case Direction.Left:
                m_ImgLeft.gameObject.SetActive(show);
                break;

        }
    }

    public void SetPointer(Direction dir, SO_RoadPointer roadPointer) {
        if (dir == Direction.Down) {
            return;
        }
        switch (dir) {
            case Direction.Up:
                m_ImgForward.sprite = roadPointer.Sprite;
                break;
            case Direction.Left:
                m_ImgLeft.sprite = roadPointer.Sprite;
                break;
            case Direction.Right:
                m_ImgRight.sprite = roadPointer.Sprite;
                break;
        }
    }

}
