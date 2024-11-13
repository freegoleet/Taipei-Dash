using System.Collections.Generic;
using Traffic;
using UnityEngine;

public class SO_RoadLine_List : MonoBehaviour
{
    private SO_RoadLine RoadLines = null;

    private Dictionary<LineType, SO_RoadLine> RoadLinesByType { get; set; } = null;
}
