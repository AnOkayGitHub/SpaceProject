using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public static class World
{
    public static bool canHideRooms = false;
    public static bool waitForGeneration = false;
    public static GameObject mainCam;
    public static List<GameObject> discoveredRooms = new List<GameObject>();
    public static bool readyToPlay = false;
    public static int coins = 0;
    public static int level = 1;
    public static GameObject player;
    public static Dictionary<string, Sprite> items = new Dictionary<string, Sprite>();
    public static float timeBeforeEnemySpawn = 0f;
    public static Room currentRoom;
    public static int currentEnemyCount = 999;
    public static bool currentRoomCleared = false;
    public static Level levelManager;
    public static Vector2 startLocation;
    public static Image bossHealthbar;
    public static TextMeshProUGUI bossName;
    public static GameObject bossHUD;
    public static Light2D globalLight;
    public static string[] bossNames = new string[] { "Bionic Transportation Cyborg",
                                                    "Universal Expedition Droid",
                                                    "Highpowered Automaton",
                                                    "Robotic Travel Juggernaut",
                                                    "Nullification Golem",
                                                    "00111110 00111010 00101001" };
}
