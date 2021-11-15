using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class World
{
    public static bool canHideRooms = false;
    public static bool waitForGeneration = false;
    public static GameObject mainCam;
    public static List<GameObject> discoveredRooms = new List<GameObject>();
    public static bool readyToPlay = false;
}
