using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.IO;
public enum RelativityState {None, Caching, MainMenu, WorldMap, ZoneMap, WorldDuel};

public class GameManager : MonoBehaviour
{
  // === Const & Inspector Cache ===
  public RelativityState beginningState = RelativityState.WorldMap;
  public const string gameSeed = "doesthisneedtobemorethaneightchars";

  // === Static Cache ===
  static RelativityState state;
  public static Transform myTrans;
  public static RelativityState State {get{return state;} set{}}
  public static Camera cam;
  public static MainUI mainUI;

  //For World
  public static World currentWorld;
  public static GameObject worldManagerObj;
  public static WorldManager worldManager;
  static CreateWorldCache worldCacher;

  public static List<GameObject> currentZoneObjects;
  public static ZoneViewCamera zoneCameraControls;

  //Zone
  public static ZoneManager zoneManager;
  public static ZoneRenderer zoneRenderer;
  public static Zone currentZone;

  // For combat
  public static GameObject combatManagerObj;
  public static CombatManager combatManager;
  public static RoundManager roundManager;
  
  void Update()
  {
    if(Input.GetKeyDown(KeyCode.Return))
    {
      CapturePNG();
    }
  }
  // *** Main Initializer ***
  void Awake ()
  {
    myTrans = transform;
    cam = Camera.main;
    if (Camera.main)
      zoneCameraControls = Camera.main.GetComponent<ZoneViewCamera>();

    // @TODO: Make these a singleton pattern
    //currentZone = new Zone(1); // Required so Hex doesn't null ref currentZone
    Hex.Initialize();

    // Ideally, the only place state is manually set.
    state = beginningState;
    bool loading;
    switch (state)
    {
      case RelativityState.WorldDuel:
        loading = false; //@TODO: still caching on duel
        InitializeWorld(loading);

        InitializeCombat();
        combatManager.BeginDuel();
      break;

      case RelativityState.WorldMap:
        loading = true;
        InitializeWorld(loading);
      break;

      case RelativityState.ZoneMap:
        InitializeZone();
      break;

      case RelativityState.Caching:
        Debug.Log("got to game manager caching");
        loading = false;
        InitializeWorld(loading);
        Debug.Log("advanced past initialize");
        worldCacher = worldManagerObj.GetComponent<CreateWorldCache>();
        worldCacher.BuildCache(worldManager.activeWorld);
      break;

      default:
        Debug.LogError("Please set a state in GameManager.beginningState before playing.");
      break;
    }
  }

  void InitializeWorld(bool loading)
  {
    worldManagerObj = GameObject.FindWithTag("World Manager");
    worldManager = worldManagerObj.GetComponent<WorldManager>();
    currentWorld = worldManager.Initialize(loading);
  }

  void InitializeCombat()
  {
    combatManagerObj = GameObject.FindWithTag("Combat Manager");
    combatManager = combatManagerObj.GetComponent<CombatManager>();
    combatManager.Initialize(currentWorld);
  }
	
  void InitializeZone()
  {
    zoneManager = GameObject.FindWithTag("Zone Manager").GetComponent<ZoneManager>();
    zoneRenderer = zoneManager.GetComponent<ZoneRenderer>();

    // --- Input

    // --- Network

    // --- Zone
    if (currentZoneObjects != null && currentZoneObjects.Count > 0)
    {
      foreach (GameObject g in currentZoneObjects)
        Destroy (g);
    }
    int safety = 100;
    bool buildingZone = true;
    int minimumSize = 50;

    Triangle tri = new Triangle(new Vector3(0, 0, 0), new Vector3(18, 0, 24), new Vector3(0, 0, 36));

    while (buildingZone)
    {
      currentZone = new Zone(tri);

      if (currentZone.landArea > minimumSize)
      {
        Debug.Log("Zone generated with a land mass of "+currentZone.landArea+" hex.");
        buildingZone = false;
      }
      else if (currentZone.landArea>0)
      {
        Debug.Log("Land mass is too low. New level being generated....");
      }
      else
      {
        Debug.Log("Underwater level detected. New level being generated....");
      }

      safety--;
      if (safety < 0)
        break;
    }

    currentZoneObjects = zoneRenderer.RenderZone(currentZone, zoneManager.regularTileSet);
    //zoneManager.Initialize(currentZone);
    //CapturePNG();
  }

  public void CapturePNG()
  {
    RenderTexture.active = Camera.main.targetTexture;
    
    //GameObject selection = GameObject.Find ("Zone Prefab(Clone)");
        Camera.main.backgroundColor = Color.clear;
        int width = 1024;
        int height = 1024;
        Texture2D tex = new Texture2D (width, height, TextureFormat.ARGB32, false);
        Rect sel = new Rect ();
        sel.width = width;
        sel.height = height;
        //sel.position = new Vector2(0,0);

        tex.ReadPixels (sel, 0, 0);
        /* 
        for(int x = 0; x < tex.width; x++)
        {
          for(int y = 0; y < tex.height; y++)
          {
            if(tex.GetPixel(x,y) == Color.clear)
            {
              
            }
          }
        }
        */
        byte[] bytes = tex.EncodeToPNG ();



        File.WriteAllBytes ("Assets/Cards/card.png", bytes);
  }

  void OnGUI()
  {
    //mainUI.OnMainGUI(); TURN BACK ON LATER
  }

  public static void OnTapInput(Vector2 tap)
  {
    switch (state)
    {
      case RelativityState.ZoneMap:
        roundManager.OnTapInput(tap);
      break;
    }
  }
}