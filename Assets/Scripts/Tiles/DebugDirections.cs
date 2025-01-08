using Traffic;
using UnityEngine;

public class DebugDirections : MonoBehaviour
{
    [SerializeField]
    private DebugDirection m_FromUp = null;
    [SerializeField]
    private DebugDirection m_FromDown = null;
    [SerializeField]
    private DebugDirection m_FromLeft = null;
    [SerializeField]
    private DebugDirection m_FromRight = null;

    public DebugDirection FromUp { get => m_FromUp; }
    public DebugDirection FromDown { get => m_FromDown; }
    public DebugDirection FromLeft { get => m_FromLeft; }
    public DebugDirection FromRight { get => m_FromRight; }

    public void ToggleShowDebugDirections(bool show) {
        FromUp.gameObject.SetActive(show);
        FromDown.gameObject.SetActive(show);
        FromLeft.gameObject.SetActive(show);
        FromRight.gameObject.SetActive(show);
    }

    public void ToggleDebugDirection(Direction from, Direction to, bool show) {
        if(from == to) {
            return;
        }

        //if(show == true) {
        //    if(from == Direction.Right) {
        //        if(to == Direction.Up) {
        //            Debug.Log("he");
        //        }
        //    }
        //}

        Direction newdir = TrafficUtilities.NormalizeRotation(from, to);
        switch (from) {
            case Direction.Up:                FromUp.ToggleDirection(newdir, show);
                break;
            case Direction.Right:
                FromRight.ToggleDirection(newdir, show);
                break;
            case Direction.Down:
                FromDown.ToggleDirection(newdir, show);
                break;
            case Direction.Left:
                FromLeft.ToggleDirection(newdir, show);
                break;
        }
    }
}
