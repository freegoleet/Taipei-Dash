using Traffic;
using UnityEngine;

public class DebugDirection : MonoBehaviour
{
    [SerializeField]
    private Transform m_Forward = null;
    [SerializeField]
    private Transform m_Left = null;
    [SerializeField]
    private Transform m_Right = null;

    public Transform Forward { get => m_Forward; }
    public Transform Left { get => m_Left; }
    public Transform Right { get => m_Right; }

    public void ToggleDirection(Direction to, bool show) {
        switch (to) {
            case Direction.Up:
                Forward.gameObject.SetActive(show);
                break;
            case Direction.Right:
                Right.gameObject.SetActive(show);
                break;
            case Direction.Left:
                Left.gameObject.SetActive(show);
                break;
        }
    }
}
