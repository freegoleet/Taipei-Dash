using System.Collections.Generic;
using Traffic;
using UnityEngine;

public class TileSidewalk : TileAutofit
{
    [Header("Sidewalk")]
    [SerializeField]
    private SpriteRenderer m_ImgCurb1 = null;
    [SerializeField]
    private SpriteRenderer m_ImgCurb2 = null;
    [SerializeField]
    private SpriteRenderer m_ImgCurb3 = null;
    [SerializeField]
    private SpriteRenderer m_ImgCurb4 = null;
    [SerializeField]
    private Transform m_SpriteMask1 = null;
    [SerializeField]
    private Transform m_SpriteMask2 = null;
    [SerializeField]
    private Transform m_SpriteMask3 = null;
    [SerializeField]
    private Transform m_SpriteMask4 = null;

    public SO_TileSidewalk SidewalkData { get; private set; }

    public SpriteRenderer ImgCurb1 { get => m_ImgCurb1; }
    public SpriteRenderer ImgCurb2 { get => m_ImgCurb2; }
    public SpriteRenderer ImgCurb3 { get => m_ImgCurb3; }
    public SpriteRenderer ImgCurb4 { get => m_ImgCurb4; }
    public Transform SpriteMask1 { get => m_SpriteMask1; }
    public Transform SpriteMask2 { get => m_SpriteMask2; }
    public Transform SpriteMask3 { get => m_SpriteMask3; }
    public Transform SpriteMask4 { get => m_SpriteMask4; }

    private Dictionary<int, SpriteRenderer> Curbs = null;
    private Dictionary<int, Transform> Masks = null;

    public override void Initialize(SO_Tile data, Vector2Int gridPos, bool cursor = false) {
        base.Initialize(data, gridPos, cursor);
        if (Data is SO_TileSidewalk swData) {
            SidewalkData = swData;
        }

        Curbs = new Dictionary<int, SpriteRenderer>() {
            { 0, ImgCurb1 },
            { 1, ImgCurb2 },
            { 2, ImgCurb3 },
            { 3, ImgCurb4 },
        };

        Masks = new Dictionary<int, Transform>() {
            { 0, SpriteMask1 },
            { 1, SpriteMask2 },
            { 2, SpriteMask3 },
            { 3, SpriteMask4 },
        };
    }

    public override void SetFacing(Direction facing) {
        base.SetFacing(facing);

        switch (facing) {
            case Direction.Up:
                ImgCurb1.transform.localEulerAngles = new Vector3(0, 0, 0);
                ImgCurb2.transform.localEulerAngles = new Vector3(0, 0, 180);
                break;
            case Direction.Right:
                ImgCurb1.transform.localEulerAngles = new Vector3(0, 0, 270);
                ImgCurb2.transform.localEulerAngles = new Vector3(0, 0, 90);
                break;
            case Direction.Down:
                ImgCurb1.transform.localEulerAngles = new Vector3(0, 0, 180);
                ImgCurb2.transform.localEulerAngles = new Vector3(0, 0, 0);
                break;
            case Direction.Left:
                ImgCurb1.transform.localEulerAngles = new Vector3(0, 0, 90);
                ImgCurb2.transform.localEulerAngles = new Vector3(0, 0, 270);
                break;
        }
    }

    public override void SetAutofitType(AutofitType autofitType) {
        base.SetAutofitType(autofitType);
        SetupCurbs(autofitType);
    }

    /// <summary>
    /// 1 Up-Left, 2 Up-Right, 3 Down-Left, 4 Down-Right
    /// </summary>
    /// <param name="innerCorners"></param>
    /// <param name="autofitType"></param>
    private void SetupCurbs(AutofitType autofitType) {
        int numberOfCurbs = 0;
        List<Direction> sidesToIgnore = new List<Direction>();
        bool canHaveInnerCorner = true;

        if (autofitType != AutofitType.Middle) {
            sidesToIgnore.Add(Facing);
        }

        switch (autofitType) {
            case AutofitType.Side:
                ImgCurb1.sprite = SidewalkData.CurbSide;
                SpriteMask1.localScale = new Vector3(1f, 0.25f, 0f);
                SpriteMask1.localPosition = new Vector3(0f, 15f, 0f);
                ImgFeature.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                numberOfCurbs++;
                break;
            case AutofitType.Corner:
                ImgCurb1.sprite = SidewalkData.CurbCorner;
                SpriteMask1.localScale = new Vector3(1f, 0.25f, 0f);
                SpriteMask1.localPosition = new Vector3(0f, 15f, 0f);
                SpriteMask2.localScale = new Vector3(0.25f, 1f, 0f);
                SpriteMask2.localPosition = new Vector3(15f, 0f, 0f);
                ImgFeature.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                numberOfCurbs += 2;
                break;
            case AutofitType.Bridge:
                ImgCurb1.sprite = SidewalkData.CurbSide;
                ImgCurb2.sprite = SidewalkData.CurbSide;
                SpriteMask1.localScale = new Vector3(1f, 0.25f, 0f);
                SpriteMask1.localPosition = new Vector3(0f, 15f, 0f);
                SpriteMask2.localScale = new Vector3(1f, 0.25f, 0f);
                SpriteMask2.localPosition = new Vector3(0f, -15f, 0f);
                ImgFeature.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                if (Facing == Direction.Up || Facing == Direction.Down) {
                    SpriteMask1.parent.localEulerAngles = new Vector3(0f, 0f, 0f);
                    SpriteMask2.parent.localEulerAngles = new Vector3(0f, 0f, 0f);
                }
                else if (Facing == Direction.Left || Facing == Direction.Right) {
                    SpriteMask1.parent.localEulerAngles = new Vector3(0f, 0f, 90f);
                    SpriteMask2.parent.localEulerAngles = new Vector3(0f, 0f, 90f);
                }
                numberOfCurbs += 2;
                SetActiveCurbs(numberOfCurbs);
                canHaveInnerCorner = false;
                break;
            case AutofitType.DeadEnd:
                ImgCurb1.sprite = SidewalkData.CurbDeadEnd;
                SpriteMask1.localScale = new Vector3(0.5f, 0.75f, 0f);
                SpriteMask1.localPosition = new Vector3(0f, -5f, 0f);
                ImgFeature.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                numberOfCurbs++;
                SetActiveCurbs(numberOfCurbs);
                canHaveInnerCorner = false;
                break;
        }

        if (autofitType != AutofitType.Bridge) {
            switch (Facing) {
                case Direction.Up:
                    if (numberOfCurbs > 0) {
                        SpriteMask1.parent.localEulerAngles = new Vector3(0f, 0f, 0f);
                    }
                    if (numberOfCurbs > 1) {
                        SpriteMask2.parent.localEulerAngles = new Vector3(0f, 0f, 0f);
                    }
                    if (autofitType == AutofitType.Corner) {
                        sidesToIgnore.Add(Direction.Right);
                    }
                    break;
                case Direction.Right:
                    if (numberOfCurbs > 0) {
                        SpriteMask1.parent.localEulerAngles = new Vector3(0f, 0f, -90f);
                    }
                    if (numberOfCurbs > 1) {
                        SpriteMask2.parent.localEulerAngles = new Vector3(0f, 0f, -90f);
                    }
                    if (autofitType == AutofitType.Corner) {
                        sidesToIgnore.Add(Direction.Down);
                    }
                    break;
                case Direction.Down:
                    if (numberOfCurbs > 0) {
                        SpriteMask1.parent.localEulerAngles = new Vector3(0f, 0f, -180f);
                    }
                    if (numberOfCurbs > 1) {
                        SpriteMask2.parent.localEulerAngles = new Vector3(0f, 0f, -180f);
                    }
                    if (autofitType == AutofitType.Corner) {
                        sidesToIgnore.Add(Direction.Left);
                    }
                    break;
                case Direction.Left:
                    if (numberOfCurbs > 0) {
                        SpriteMask1.parent.localEulerAngles = new Vector3(0f, 0f, -270f);
                    }
                    if (numberOfCurbs > 1) {
                        SpriteMask2.parent.localEulerAngles = new Vector3(0f, 0f, -270f);
                    }
                    if (autofitType == AutofitType.Corner) {
                        sidesToIgnore.Add(Direction.Up);
                    }
                    break;
                case Direction.None:
                    break;
            }
        }

        // Inner Corner Curbs
        foreach (Direction fitAdjacents in NeighborSystem.GetAllFittableAdjacentDirections()) {
            if (canHaveInnerCorner == false) {
                break;
            }

            foreach ((Direction, Direction) unfitDiagonals in NeighborSystem.GetAllUnfittableDiagonalDirections()) {

                if (sidesToIgnore.Contains(unfitDiagonals.Item1)) {
                    continue;
                }
                if (unfitDiagonals.Item1 == fitAdjacents && autofitType != AutofitType.Corner) {
                    continue;
                }
                if (sidesToIgnore.Contains(unfitDiagonals.Item2)) {
                    continue;
                }
                if (unfitDiagonals.Item2 == fitAdjacents && autofitType != AutofitType.Corner) {
                    continue;
                }
                if (numberOfCurbs >= Curbs.Count) {
                    break;
                }
                switch (unfitDiagonals) {
                    case (Direction.Up, Direction.Left):
                        Curbs[numberOfCurbs].transform.localEulerAngles = new Vector3(0f, 0f, 90f);
                        Masks[numberOfCurbs].parent.transform.localEulerAngles = new Vector3(0, 0, 90f);
                        break;
                    case (Direction.Up, Direction.Right):
                        Curbs[numberOfCurbs].transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                        Masks[numberOfCurbs].parent.transform.localEulerAngles = new Vector3(0, 0, 0f);
                        break;
                    case (Direction.Down, Direction.Left):
                        Curbs[numberOfCurbs].transform.localEulerAngles = new Vector3(0f, 0f, 180f);
                        Masks[numberOfCurbs].parent.transform.localEulerAngles = new Vector3(0, 0, 180f);
                        break;
                    case (Direction.Down, Direction.Right):
                        Curbs[numberOfCurbs].transform.localEulerAngles = new Vector3(0f, 0f, 270f);
                        Masks[numberOfCurbs].parent.transform.localEulerAngles = new Vector3(0, 0, 270f);
                        break;
                }

                Curbs[numberOfCurbs].sprite = SidewalkData.CurbInnerCorner;
                Masks[numberOfCurbs].transform.localPosition = new Vector3(15f, 15f, 0f);

                Masks[numberOfCurbs].transform.localScale = new Vector3(0.25f, 0.25f, 0f);
                numberOfCurbs++;
            }
        }

        SetActiveCurbs(numberOfCurbs);
    }

    private void SetActiveCurbs(int numberOfCurbs) {
        for (int i = 0; i < Curbs.Count; i++) {
            bool active = i < numberOfCurbs ? true : false;
            Curbs[i].gameObject.SetActive(i == 1 && AutofitType == AutofitType.Corner ? false : active);
            Masks[i].gameObject.SetActive(active);
        }
    }
}
