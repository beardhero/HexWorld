using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
public enum HexDirection{R, P, L, S, B, F}; // right port left starboard back front

[RequireComponent(typeof(WorldRenderer))]
public class WorldManager : MonoBehaviour
{
  // === Public ===
  public string worldCaptureName;
  public RenderTexture activeTex;
  [HideInInspector] public World activeWorld;
  public TileSet regularTileSet;
  public float worldScale = 1;
  public int worldSubdivisions = 1;
  public static int uvWidth = 100;
  public static int uvHeight;
  public bool b;
  public bool randomAnt;
  public TileType element = TileType.Gray;
  public byte[] seed;
  
  // === Private ===
  bool labelDirections;
  
  private int octaves, multiplier;
  private float amplitude, lacunarity, dAmplitude;

  // === Properties ===
  private float _averageScale;
  public float averageScale
  {
    get {
      _averageScale = 0;
      foreach (HexTile tt in activeWorld.tiles)
      {
        _averageScale += tt.height;
      }
      _averageScale /= activeWorld.tiles.Count;
      return _averageScale; }
    set { _averageScale = value; }
  }
  		
  // === Cache ===
  WorldRenderer worldRenderer;
  GameObject currentWorldObject;
  Transform currentWorldTrans;
  //int layermask; @TODO: stuff

  //for type changer
  [HideInInspector]public Ray ray;
  [HideInInspector]public RaycastHit hit;
  public TileType switchToType;
  public float heightToSet;
  public int frameDelay = 60;
  [HideInInspector]public int fB;
  [HideInInspector]public TileType sT;
  [HideInInspector]public int r;
  [HideInInspector]public bool[] hla;
  //float uvTileWidth = regularTileSet.tileWidth / texWidth;
  //float uvTileHeight = regularTileSet.tileWidth / texHeight;

  //Langston's ant
  public string sequence;
  public float antSpeed;
  public bool track;
  float lerpTime = 1f;
  float currentLerpTime;
  public float hDiffTolerance = .1f;
  public int maxElementals = 12;
  public int eleSpawnMin = 60;
  public int eleSpawnMax = 360;
  public bool activeElementals = true;
  Object[] elementals;
  Object[] AirElementals;
  Object[] EarthElementals;
  Object[] WaterElementals;
  Object[] FireElementals;
  Object[] LightElementals;
  Object[] DarkElementals;
  List<GameObject> livingElementals;
  List<IEnumerator> eleCoroutines;
  GameObject newAnt;
  HexTile startingTile;
  HexTile forwardTile;

  public World Initialize(bool loadWorld = false)
  {
    currentWorldObject = new GameObject("World");
    currentWorldTrans = currentWorldObject.transform;
    //activeWorld = new World();
    
    if (loadWorld)
    {
      activeWorld = LoadWorld();

      //random world test
      seed = new byte[32];
      for(int i = 0; i < 32; i++)
      {
        seed[i] = (byte)UnityEngine.Random.Range(0,256);
      }
      activeWorld.Populate(seed);

      //place objects in biomes
      Object[] airBiome = Resources.LoadAll("Air/");
      Object[] earthBiome = Resources.LoadAll("Earth/");
      Object[] waterBiome = Resources.LoadAll("Water/");
      Object[] fireBiome = Resources.LoadAll("Fire/");
      Object[] darkBiome = Resources.LoadAll("Dark/");
      Object[] lightBiome = Resources.LoadAll("Light/");
      Object[] misc = Resources.LoadAll("Misc/");
      Transform p = GameObject.Find("WorldObjects").transform;
      //p.parent = currentWorldTrans;

      AirElementals = Resources.LoadAll("AirElementals/");//,typeof(GameObject)).Cast<GameObject>().ToArray();
      EarthElementals = Resources.LoadAll("EarthElementals/");
      WaterElementals = Resources.LoadAll("WaterElementals/");
      FireElementals = Resources.LoadAll("FireElementals/");
      LightElementals = Resources.LoadAll("LightElementals/");
      DarkElementals = Resources.LoadAll("DarkElementals/");
      elementals = Resources.LoadAll("wfchars/");
      livingElementals = new List<GameObject>();
      
      foreach(HexTile ht in activeWorld.tiles)
      {
        if(ht.objectToPlace != -1)
        {
          Object g = new Object();
          Vector3 v =  ht.hexagon.center - activeWorld.origin;
          
        switch(ht.type)
          {
            case TileType.Gray:
              g = misc[ht.objectToPlace]; 
             Instantiate(g,ht.hexagon.center,Quaternion.FromToRotation(Vector3.up, v), p); break;
              //g.transform.rotation = Quaternion.FromToRotation(g.transform.up, ht.hexagon.center - activeWorld.origin); break;
            case TileType.Water: 
              g = waterBiome[ht.objectToPlace];
              Instantiate(g,ht.hexagon.center,Quaternion.FromToRotation(Vector3.up, v), p); break;
              //g.transform.rotation = Quaternion.FromToRotation(g.transform.up, ht.hexagon.center - activeWorld.origin); break;
           case TileType.Fire: 
              g = fireBiome[ht.objectToPlace];
              Instantiate(g,ht.hexagon.center,Quaternion.FromToRotation(Vector3.up, v), p); break;
             //g.transform.rotation = Quaternion.FromToRotation(g.transform.up, ht.hexagon.center - activeWorld.origin); break;
          case TileType.Earth: 
              g = earthBiome[ht.objectToPlace];
              Instantiate(g,ht.hexagon.center,Quaternion.FromToRotation(Vector3.up, v), p); break;
              //g.transform.rotation = Quaternion.FromToRotation(g.transform.up, ht.hexagon.center - activeWorld.origin); break;
            case TileType.Air: 
              g = airBiome[ht.objectToPlace];
              Instantiate(g,ht.hexagon.center,Quaternion.FromToRotation(Vector3.up, v), p); break;
              //g.transform.rotation = Quaternion.FromToRotation(g.transform.up, ht.hexagon.center - activeWorld.origin); break;
           case TileType.Dark: 
             g = darkBiome[ht.objectToPlace];
             Instantiate(g,ht.hexagon.center,Quaternion.FromToRotation(Vector3.up, v), p); break;
             //g.transform.rotation = Quaternion.FromToRotation(g.transform.up, ht.hexagon.center - activeWorld.origin); break;
           case TileType.Light: 
             g = lightBiome[ht.objectToPlace];
             Instantiate(g,ht.hexagon.center,Quaternion.FromToRotation(Vector3.up, v), p); break;
             //g.transform.rotation = Quaternion.FromToRotation(g.transform.up, ht.hexagon.center - activeWorld.origin); break;
           default: break;
          }
        }
      }
    }
    else
    {
      activeWorld = new World();
      activeWorld.PrepForCache(worldScale, worldSubdivisions); //only being used for base planets right now
    }

    worldRenderer = GetComponent<WorldRenderer>();
    //changed this to run TriPlates instead of HexPlates
    foreach (GameObject g in worldRenderer.HexPlates(activeWorld, regularTileSet))
    {
      g.transform.parent = currentWorldTrans;
    }
    Debug.Log(activeWorld.tiles.Count);
     
    foreach(HexTile ht in activeWorld.tiles)
    {
      ht.ChangeType(ht.type);
    }
    
    if(loadWorld)
    {
       StartCoroutine(SpawnElementals());
    }
    /* 
    foreach(HexTile ht in activeWorld.tiles)
    {
      ht.ChangeType(TileType.Sol);
    }
    */
    //layermask = 1 << 8;   // Layer 8 is set up as "Chunk" in the Tags & Layers manager

    //labelDirections = true;

    //DrawHexIndices();

    return activeWorld;
  }

  void Update()
  {
    if(Input.GetKeyDown(KeyCode.G))
    {
      int[] s = CalculateGenerationalJoeLife(activeWorld.tiles, 1);
      for(int y = 0; y < activeWorld.tiles.Count; y++)
      {
        HexTile t = activeWorld.tiles[y];
        t.generation = s[y];
        t.ChangeType(t.type);
      }
    }
    if(Input.GetKeyDown(KeyCode.T))
    {
      track = !track;
    }
    //cyclical hex life
		if(Input.GetKeyDown(KeyCode.Return))
		{
      b = !b;
      if(b)
      {
        if(randomAnt){sequence = RandomAnt();}
        GameObject ant = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        StartCoroutine(LangstonsHex0(sequence, ant));
      }
			fB = 0;
		}	
    
		fB++;
		if (b) 
    {
		  //HLShift ();
      //CyclicalHexLife();
      //JoeLife();
      //TheDualityOfLife();
      //RandomWorldState();
      
		  fB = 0;
    }

    if(Input.GetKeyDown(KeyCode.M))
    {
      byte[] id = new byte[32];
      
      for (int i = 0; i < 32; i++)
      {
          id[i] = (byte)i;
      }

      MONTest(id);
    }

    if(Input.GetKeyDown(KeyCode.N))
    {
      if(randomAnt){sequence = RandomAnt();}
        GameObject ant = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        StartCoroutine(LangstonsHex0(sequence, ant));
    }
    /* 
    if (Input.GetKeyDown(KeyCode.Mouse0))
    {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if(Physics.Raycast(ray, out hit, 100.0f))
      { 
        Debug.Log("casted");
        int from = GetHitTile(hit);
        Debug.Log(from);
        StartCoroutine(TestDistance(from));
      }
    }
    */
  }
  
  void MONTest(byte[] id)
  {
    Mon mon = new Mon(id);
  }

  public void RandomWorldState()
  {
    int[] st = new int[activeWorld.tiles.Count];
    for(int i = 0; i < activeWorld.tiles.Count; i++)
    {
      st[i] = UnityEngine.Random.Range(0,8);
    }
    activeWorld.SetState(st);
  }
  public IEnumerator LangstonsHex0(string seq, GameObject ant)
  {
    Debug.Log("Select starting tile");
    HexTile hitTile = new HexTile();
    bool selecting = true;
    while(selecting)
    {
      if(Input.GetKeyDown(KeyCode.Mouse0))
      {
        r++;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
	        GameObject plateO = hit.transform.gameObject;
	        Vector3 c = new Vector3 ();
	        Vector3 h = hit.point;
	        float test;
	        float dist = 9999999;
	        foreach (HexTile ht in activeWorld.tiles) 
	        {
			      c = ht.hexagon.center;
			      test = (c - h).sqrMagnitude;
			      if (test < dist) 
            {
			    	  dist = test;
				      hitTile = ht;
			      }
	        }
          selecting = false;
        } 
      }
      yield return null;
    }
    Debug.Log(hitTile.index);
    StartCoroutine(LangstonsHex1(seq, ant, hitTile));
    yield return null;
  }
  public IEnumerator LangstonsHex1(string seq, GameObject ant, HexTile onTile)
  {
    Debug.Log("Select forward tile");
    HexTile hitTile = new HexTile();
    bool selecting = true;
    while(selecting)
    {
      if(Input.GetKeyDown(KeyCode.Mouse0))
      {
        r++;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
	        GameObject plateO = hit.transform.gameObject;
	        Vector3 c = new Vector3 ();
	        Vector3 h = hit.point;
	        float test;
	        float dist = 9999999;
	        foreach (HexTile ht in activeWorld.tiles) 
	        {
            foreach(int i in onTile.neighbors)
            {
              if(ht.index == i)
              {
                 c = ht.hexagon.center;
			          test = (c - h).sqrMagnitude;
			          if (test < dist) 
                {
			    	      dist = test;
				          hitTile = ht;
			          }
                selecting = false;
              }
            }
	        }
        } 
      }
      yield return null;
    }
    Debug.Log(hitTile.index);
    StartCoroutine(LangstonsHex2(seq, ant, onTile, hitTile));
    yield return null;
  }
  public IEnumerator LangstonsHex2(string seq, GameObject ant, HexTile onTile, HexTile forwardTile)
  {
    Debug.Log(seq);
    int back, forward, right, left, port, starboard;
    back = forward = right = left = port = starboard = 0;
    char[] dna = seq.ToCharArray();
    //HexTile tOut = new HexTile();
    HexTile nextTile = new HexTile();
    int toSet = 1;
    Vector3 o = activeWorld.origin;
    Vector3 ve = Camera.main.transform.position - o;
    float camMag = ve.magnitude *.4f;
    Vector3 lVel = Vector3.zero;
    float smoothTime = 1;
    float dist;
    Vector3 from;
    Vector3 to;
    //Instantiate(ant, onTile.hexagon.center, Quaternion.identity);
    Transform antTrans = ant.transform;
    antTrans.position = onTile.hexagon.center; //@TODO: figure out correct scaling
    
    //Set up first tile
    forward = forwardTile.index;
    if(!onTile.hexagon.isPentagon)
      {
        Vector3 fVec = forwardTile.hexagon.center - onTile.hexagon.center; 
        Vector3 rotationAxis = onTile.hexagon.center - activeWorld.origin;
        for(int i = 0; i < 5; i++)
        {
          Vector3 nextVec = Quaternion.AngleAxis(60*(i+1), rotationAxis) * fVec;
          float test = 99999;
          int nextNei = 0;
          foreach(int nei in onTile.neighbors)
          {
            Vector3 v = activeWorld.tiles[nei].hexagon.center - onTile.hexagon.center;
            float tV = (v - nextVec).sqrMagnitude;
            if(tV < test)
            {
              nextNei = nei;
              test = tV;
            }
          }
          switch(i)
          {
            case 0: right = activeWorld.tiles[nextNei].index; break;
            case 1: starboard = activeWorld.tiles[nextNei].index; break;
            case 2: back = activeWorld.tiles[nextNei].index; break;
            case 3: port = activeWorld.tiles[nextNei].index; break;
            case 4: left = activeWorld.tiles[nextNei].index; break;
          }
        }
      }
        else
        {
          back = forward;
          Vector3 backVec = forwardTile.hexagon.center - onTile.hexagon.center; 
          Vector3 rotationAxis = onTile.hexagon.center - activeWorld.origin;
          for(int i = 0; i < 4; i++)
          {
            Vector3 nextVec = Quaternion.AngleAxis(72*(i+1), rotationAxis) * backVec;
            float testF = 99999;
            int nextNei = 0;
            foreach(int nei in onTile.neighbors)
            {
              Vector3 v = activeWorld.tiles[nei].hexagon.center - onTile.hexagon.center;
              float tV = (v - nextVec).sqrMagnitude;
              if(tV < testF)
              {
                nextNei = nei;
                testF = tV;
              }
            }
            switch(i)
            {
                case 0: port = activeWorld.tiles[nextNei].index; break;
                case 1: left = activeWorld.tiles[nextNei].index; break;
                case 2: right = activeWorld.tiles[nextNei].index; break;
                case 3: starboard = activeWorld.tiles[nextNei].index; break;
            }
          }
        }
    /*
    port = onTile.neighbors[1];
    left = onTile.neighbors[2];
    forward = onTile.neighbors[4];
    right = onTile.neighbors[5];
    starboard = onTile.neighbors[3];
    */
    /* 
    activeWorld.tiles[back].ChangeType(TileType.Dark);
    activeWorld.tiles[port].ChangeType(TileType.Fire);
    activeWorld.tiles[left].ChangeType(TileType.Earth);
    activeWorld.tiles[forward].ChangeType(TileType.Light);
    activeWorld.tiles[right].ChangeType(TileType.Water);
    activeWorld.tiles[starboard].ChangeType(TileType.Air);
    */
    
    while(true)
    {
      /* 
      currentLerpTime += Time.deltaTime;
      if (currentLerpTime > lerpTime) {
          currentLerpTime = lerpTime;
      }
      float lerp = currentLerpTime / lerpTime;
      */
      /*Camera
      if(track){
        from = Camera.main.transform.position;
        to = (onTile.hexagon.center - activeWorld.origin)*2.4f;
        dist = (to - from).sqrMagnitude;
        smoothTime = .24f;//+(0.95f/dist);
        Camera.main.transform.position = Vector3.SmoothDamp(from, to, ref lVel, smoothTime);
        Camera.main.transform.LookAt(currentWorldTrans);
      }*/
      //Switch to next color
      if(onTile.type == TileType.None)
      {
        Debug.Log(onTile.index);
      }
      toSet = (int)onTile.type + 1;
      if(toSet > 7){toSet = 1;}
      if(sequence.Length < 6 && toSet > sequence.Length)
      {
        toSet = 1;
      }
      onTile.ChangeType((TileType)toSet);
      
      //Make next movement based on dna
      char seqChar = dna[onTile.antPasses];
      onTile.antPasses += 1;
      if(onTile.antPasses > dna.Length - 1)
      {
        onTile.antPasses = 0;
      }
      switch(seqChar) 
      {
        case 'B': nextTile = activeWorld.tiles[back]; break;
        case 'P': nextTile = activeWorld.tiles[port]; break;
        case 'L': nextTile = activeWorld.tiles[left]; break;
        case 'F': nextTile = activeWorld.tiles[forward]; break;
        case 'R': nextTile = activeWorld.tiles[right]; break;
        case 'S': nextTile = activeWorld.tiles[starboard]; break;
        default: Debug.Log("Invalid char" + dna[onTile.antPasses]); break;
      }

      if(!nextTile.hexagon.isPentagon)
      {
        back = onTile.index;
        Vector3 backVec = onTile.hexagon.center - nextTile.hexagon.center; 
        Vector3 rotationAxis = nextTile.hexagon.center - activeWorld.origin;
        for(int i = 0; i < 5; i++)
        {
          Vector3 nextVec = Quaternion.AngleAxis(60*(i+1), rotationAxis) * backVec;
          float test = 99999;
          int nextNei = 0;
          foreach(int nei in nextTile.neighbors)
          {
            Vector3 v = activeWorld.tiles[nei].hexagon.center - nextTile.hexagon.center;
            float tV = (v - nextVec).sqrMagnitude;
            if(tV < test)
            {
              nextNei = nei;
              test = tV;
            }
          }
          switch(i)
          {
            case 0: port = activeWorld.tiles[nextNei].index; break;
            case 1: left = activeWorld.tiles[nextNei].index; break;
            case 2: forward = activeWorld.tiles[nextNei].index; break;
            case 3: right = activeWorld.tiles[nextNei].index; break;
            case 4: starboard = activeWorld.tiles[nextNei].index; break;
          }
        }
      }
        else
        {
          //Debug.Log("Registered pentagon: " + nextTile.index);
          back = onTile.index;
          forward = onTile.index;
          Vector3 backVec = onTile.hexagon.center - nextTile.hexagon.center; 
          Vector3 rotationAxis = onTile.hexagon.center - activeWorld.origin;
          for(int i = 0; i < 4; i++)
          {
            Vector3 nextVec = Quaternion.AngleAxis(72*(i+1), rotationAxis) * backVec;
            float testF = 9999;
            int nextNei = 0;
            foreach(int nei in nextTile.neighbors)
            {
              Vector3 v = activeWorld.tiles[nei].hexagon.center - nextTile.hexagon.center;
              float tV = (v - nextVec).sqrMagnitude;
              if(tV < testF)
              {
                nextNei = nei;
                testF = tV;
              }
            }
            switch(i)
            {
                case 0: port = activeWorld.tiles[nextNei].index; break;
                case 1: left = activeWorld.tiles[nextNei].index; break;
                case 2: right = activeWorld.tiles[nextNei].index; break;
                case 3: starboard = activeWorld.tiles[nextNei].index; break;
            }
          }
        }
      antTrans.position = nextTile.hexagon.center * 1.03f;
      //antTrans.up = nextTile.hexagon.center - activeWorld.origin;
      //Debug.DrawLine((onTile.hexagon.center-activeWorld.origin)*1.1f, (nextTile.hexagon.center-activeWorld.origin)*1.1f, Color.blue, 1000.0f, false);
      onTile = nextTile;
   
      if(antSpeed > 0)
      {
        yield return new WaitForSeconds(antSpeed);
      }
      yield return null;
    }
    foreach(HexTile ht in activeWorld.tiles)
    {
      ht.ChangeType(TileType.Gray);
      ht.antPasses = 0;
      ht.generation = 0;
    }
    Debug.Log("Ant stopped");
    
    yield return null;
  }
  public IEnumerator LangstonsElementals(string seq, GameObject antGO, HexTile onTile, HexTile forwardTile)
  {
    if(antGO == null)
    {yield break;}
    Debug.Log(seq);
    int back, forward, right, left, port, starboard;
    back = forward = right = left = port = starboard = 0;
    TileType ele = onTile.type;
    char[] dna = seq.ToCharArray();
    //HexTile tOut = new HexTile();
    HexTile nextTile = new HexTile();
    int toSet = 1;
    Vector3 o = activeWorld.origin;
    Vector3 ve = Camera.main.transform.position - o;
    float camMag = ve.magnitude *.4f;
    Vector3 lVel = Vector3.zero;
    float smoothTime = 1;
    float dist;
    Vector3 from;
    Vector3 to;
    GameObject ant = Instantiate(antGO, onTile.hexagon.center, Quaternion.identity);
    livingElementals.Add(ant);
    Transform antTrans = ant.transform;
    antTrans.position = onTile.hexagon.center; //@TODO: figure out correct scaling
    
    //Set up first tile
    forward = forwardTile.index;
    if(!onTile.hexagon.isPentagon)
      {
        Vector3 fVec = forwardTile.hexagon.center - onTile.hexagon.center; 
        Vector3 rotationAxis = onTile.hexagon.center - activeWorld.origin;
        for(int i = 0; i < 5; i++)
        {
          Vector3 nextVec = Quaternion.AngleAxis(60*(i+1), rotationAxis) * fVec;
          float test = 99999;
          int nextNei = 0;
          foreach(int nei in onTile.neighbors)
          {
            Vector3 v = activeWorld.tiles[nei].hexagon.center - onTile.hexagon.center;
            float tV = (v - nextVec).sqrMagnitude;
            if(tV < test)
            {
              nextNei = nei;
              test = tV;
            }
          }
          switch(i)
          {
            case 0: right = activeWorld.tiles[nextNei].index; break;
            case 1: starboard = activeWorld.tiles[nextNei].index; break;
            case 2: back = activeWorld.tiles[nextNei].index; break;
            case 3: port = activeWorld.tiles[nextNei].index; break;
            case 4: left = activeWorld.tiles[nextNei].index; break;
          }
        }
      }
        else
        {
          back = forward;
          Vector3 backVec = forwardTile.hexagon.center - onTile.hexagon.center; 
          Vector3 rotationAxis = onTile.hexagon.center - activeWorld.origin;
          for(int i = 0; i < 4; i++)
          {
            Vector3 nextVec = Quaternion.AngleAxis(72*(i+1), rotationAxis) * backVec;
            float testF = 99999;
            int nextNei = 0;
            foreach(int nei in onTile.neighbors)
            {
              Vector3 v = activeWorld.tiles[nei].hexagon.center - onTile.hexagon.center;
              float tV = (v - nextVec).sqrMagnitude;
              if(tV < testF)
              {
                nextNei = nei;
                testF = tV;
              }
            }
            switch(i)
            {
                case 0: port = activeWorld.tiles[nextNei].index; break;
                case 1: left = activeWorld.tiles[nextNei].index; break;
                case 2: right = activeWorld.tiles[nextNei].index; break;
                case 3: starboard = activeWorld.tiles[nextNei].index; break;
            }
          }
        }
    if(ant != null){StartCoroutine(ElementalRandomRotation(ant));}
    while(ant != null)
    {
      //Switch to next color
      foreach(TileType tt in onTile.GetOpposingElements())
      {
        if(ele == tt)
        {
          GameObject.Destroy(ant);
          livingElementals.Remove(ant);
          yield break;
        }
      }
      /*TileType ts = TileType.Gray;
      if(onTile.type == ts)
      {
        ts = ele;
      }
      */
      if(onTile.generation == World.zeroState)
      {
        onTile.generation = World.oneState;
        onTile.ChangeType(onTile.type);
      }
      else if(onTile.generation == World.oneState)
      {
        onTile.generation = World.zeroState;
        onTile.ChangeType(onTile.type);
      }
      

      //Make next movement based on dna
      if(onTile.antPasses > dna.Length - 1)
      {
        onTile.antPasses = 0;
      }
      char seqChar = dna[onTile.antPasses];
      onTile.antPasses += 1;
      if(onTile.antPasses > dna.Length - 1)
      {
        onTile.antPasses = 0;
      }
      switch(seqChar) 
      {
        case 'B': nextTile = activeWorld.tiles[back]; break;
        case 'P': nextTile = activeWorld.tiles[port]; break;
        case 'L': nextTile = activeWorld.tiles[left]; break;
        case 'F': nextTile = activeWorld.tiles[forward]; break;
        case 'R': nextTile = activeWorld.tiles[right]; break;
        case 'S': nextTile = activeWorld.tiles[starboard]; break;
        default: Debug.Log("Invalid char" + dna[onTile.antPasses]); break;
      }
      if(Mathf.Abs(nextTile.hexagon.scale - onTile.hexagon.scale) > hDiffTolerance || nextTile.passable == false) //height tolerance & passability
      {
        nextTile = onTile;
      }
      else{
      if(!nextTile.hexagon.isPentagon)
      {
        back = onTile.index;
        Vector3 backVec = onTile.hexagon.center - nextTile.hexagon.center; 
        Vector3 rotationAxis = nextTile.hexagon.center - activeWorld.origin;
        for(int i = 0; i < 5; i++)
        {
          Vector3 nextVec = Quaternion.AngleAxis(60*(i+1), rotationAxis) * backVec;
          float test = 99999;
          int nextNei = 0;
          foreach(int nei in nextTile.neighbors)
          {
              Vector3 v = activeWorld.tiles[nei].hexagon.center - nextTile.hexagon.center;
              float tV = (v - nextVec).sqrMagnitude;
              if(tV < test)
              {
                nextNei = nei;
                test = tV;
              }
          }
          switch(i)
          {
            case 0: port = activeWorld.tiles[nextNei].index; break;
            case 1: left = activeWorld.tiles[nextNei].index; break;
            case 2: forward = activeWorld.tiles[nextNei].index; break;
            case 3: right = activeWorld.tiles[nextNei].index; break;
            case 4: starboard = activeWorld.tiles[nextNei].index; break;
          }
        }
      }
      else
      {
          //Debug.Log("Registered pentagon: " + nextTile.index);
          back = onTile.index;
          forward = onTile.index;
          Vector3 backVec = onTile.hexagon.center - nextTile.hexagon.center; 
          Vector3 rotationAxis = onTile.hexagon.center - activeWorld.origin;
          for(int i = 0; i < 4; i++)
          {
            Vector3 nextVec = Quaternion.AngleAxis(72*(i+1), rotationAxis) * backVec;
            float testF = 9999;
            int nextNei = 0;
            foreach(int nei in nextTile.neighbors)
            {
                Vector3 v = activeWorld.tiles[nei].hexagon.center - nextTile.hexagon.center;
                float tV = (v - nextVec).sqrMagnitude;
                if(tV < testF)
                {
                  nextNei = nei;
                  testF = tV;
                }
            }
            switch(i)
            {
                case 0: port = activeWorld.tiles[nextNei].index; break;
                case 1: left = activeWorld.tiles[nextNei].index; break;
                case 2: right = activeWorld.tiles[nextNei].index; break;
                case 3: starboard = activeWorld.tiles[nextNei].index; break;
            }
          }
        }
      }
      if(onTile != nextTile)
      {
        //onTile.passable = true; @TODO: passability of occupied tile
        //nextTile.passable = false;
        if(ant != null)
        {
          StartCoroutine(MoveElemental(nextTile.hexagon.center * 1.03f, ant));
        }
      }
      onTile = nextTile;
      //onTile.passable = false;
      //Debug.DrawLine((onTile.hexagon.center-activeWorld.origin)*1.1f, (nextTile.hexagon.center-activeWorld.origin)*1.1f, Color.blue, 1000.0f, false);
      if(antSpeed > 0)
      {
        yield return new WaitForSeconds(antSpeed);
      }
      yield return null;
    }
    Debug.Log("Ant stopped");
    
    yield return null;
  }
  
  public IEnumerator ElementalRandomRotation(GameObject go)
  {
    while(go != null)
    {
      Vector3 axis = Random.onUnitSphere;
      float startTime = Time.time;
      float end = Random.Range(.5f,1.5f);
      Quaternion g = go.transform.rotation;
      Quaternion rotation = Random.rotation;
      while(Time.time < startTime + end && go != null)
      {
        go.transform.rotation = Quaternion.Lerp(g, rotation, (Time.time - startTime)/end);
        yield return null;
      }
      if(go != null){go.transform.rotation = rotation;}
    }
  }
  public IEnumerator MoveElemental(Vector3 to, GameObject go)
  {
    if(go==null)
    {yield break;}
    float startTime = Time.time;
    Vector3 from = go.transform.position;
    float t = antSpeed * .24f;
    while(Time.time < startTime + t && go != null)
    {
      go.transform.position = Vector3.Lerp(from, to, (Time.time - startTime)/t);
      yield return null;
    }
    if(go != null)
    {
      go.transform.position = to;
    }
  }

  public string RandomAnt()
  {
    int length = Random.Range(3,7);
    string[] ant = new string[length];
    string s = "";
    for(int i = 0; i < length; i++)
    {
      switch(Random.Range(0,6))
      {
        case 0: ant[i] = "R"; break;
        case 1: ant[i] = "L"; break;
        case 2: ant[i] = "P"; break;
        case 3: ant[i] = "S"; break;
        case 4: ant[i] = "F"; break;
        case 5: ant[i] = "B"; break;
      }
      s += ant[i];
    }
    return s;
  }
  public void TheDualityOfLife()
  {
    foreach(HexTile ht in activeWorld.tiles)
    {
      ht.typeToSet = TileType.Gray;//ht.type;
      int s = 0;
      foreach(int i in ht.neighbors)
      {
        switch (activeWorld.tiles[i].type)
        {
            case TileType.Fire:
              s += 1;
              break;
            case TileType.Water:
              s -= 1;
              break;
            case TileType.Air:
              s += 2;
              break;
            case TileType.Earth:
              s -= 2;
              break;
            case TileType.Light:
              s += 3;
              break;
            case TileType.Dark:
              s -= 3;
              break;
            default: break;
        }
      }
      if(ht.type == TileType.Water || ht.type == TileType.Earth || ht.type == TileType.Dark)
      {
        if(s > 0)
        {
         s = s % 3;
          switch(s)
          {
           case 0: ht.typeToSet = TileType.Fire; break;
           case 1: ht.typeToSet = TileType.Air; break;
           case 2: ht.typeToSet = TileType.Light; break;
           default: break;
         }
       }
       if(s<0)
       {
        // ht.typeToSet = ht.type;
       }
      }
      if(ht.type == TileType.Fire || ht.type == TileType.Air || ht.type == TileType.Light)
      if(s < 0)
      {
        s = (s % 3)*-1;
        switch(s)
        {
          case 0: ht.typeToSet = TileType.Water; break;
          case 1: ht.typeToSet = TileType.Earth; break;
          case 2: ht.typeToSet = TileType.Dark; break;
          default: break;
        }
      }
      if(s>0)
      {
       // ht.typeToSet = ht.type;
      }
    }
    foreach(HexTile ht in activeWorld.tiles)
    {
      ht.ChangeType(ht.typeToSet);
    }
  }
  public int[] CalculateGenerationalJoeLife(List<HexTile> tiles, int generations)
  {
    int toSet;
    int numTiles = tiles.Count;
    int[] state = new int[numTiles];
    for(int st = 0; st < numTiles; st++)
    {
      state[st] = tiles[st].generation;
    }
    for(int i = 0; i < generations; i++)
    {
      //find the state
      for(int t = 0; t < numTiles; t++)
      {
        toSet = 0; 
        int s = 0;
        HexTile ht = tiles[t];
        foreach(int n in ht.neighbors)
        {
          HexTile nei = activeWorld.tiles[n];
          if((nei.hexagon.scale - ht.hexagon.scale) < hDiffTolerance && nei.type == ht.type)
          {s += nei.generation;}
        }
        if(ht.generation == 0 && s == 4)
        {
          toSet = 1;
        }
        if(ht.generation == 1 && s != 0 && s != 5 && s <= 6)
        {
          toSet = 2;
        }
        if(ht.generation == 2 && (s == 1 || s == 2))
        {
          toSet = 2;
        }
        if(ht.generation == 2 && s == 4)
        {
          toSet = 1;
        }
        state[t] = toSet;
      }
      //set the state
      for(int x = 0; x > numTiles; x++)
      {
        tiles[x].generation = state[x];
      }  
    }
    return state;
  }
  public void JoeLife()
  {
     foreach(HexTile ht in activeWorld.tiles)
    {
      ht.typeToSet = TileType.Light;
      TileType nextTile = ht.type;
      int s = 0;
      foreach(int i in ht.neighbors)
      {
        switch (activeWorld.tiles[i].type)
        {
            case TileType.Fire:
              s += 1;
              break;
           case TileType.Air:
              s += 2;
              break;
            default: break;
        }
      }
      if((ht.type == TileType.Light || ht.type == TileType.Gray) && s == 4)
      {
        ht.typeToSet = TileType.Fire;
      }
      if(ht.type == TileType.Fire && s != 0 && s != 5 && s <= 6)
      {
        ht.typeToSet = TileType.Air;
      }
      if(ht.type == TileType.Air && (s == 1 || s == 2))
      {
        ht.typeToSet = TileType.Air;
      }
      if(ht.type == TileType.Air && s == 4)
      {
        ht.typeToSet = TileType.Fire;
      }
    }
    foreach(HexTile ht in activeWorld.tiles)
    {
      ht.ChangeType(ht.typeToSet);
    }
  }

  public void CyclicalHexLife()
  {
    foreach(HexTile ht in activeWorld.tiles)
    {
      TileType nextTile = ht.type;
    
      switch (ht.type)
      {
          case TileType.Water:
            nextTile = TileType.Dark;
            break;
          case TileType.Dark:
            nextTile = TileType.Earth;
            break;
          case TileType.Earth:
            nextTile = TileType.Water;
            break;
          default: break;
      }
      foreach(int nei in ht.neighbors)
      {
        if(activeWorld.tiles[nei].type == nextTile)
        {
          ht.typeToSet = nextTile;
        }
      }
    }
    foreach(HexTile ht in activeWorld.tiles)
    {
      if(ht.type != ht.typeToSet)
      {
        ht.ChangeType(ht.typeToSet);
      }
    }
  }


  public IEnumerator SpawnElementals()
  {
    while(activeElementals)
    {
      if(livingElementals.Count < maxElementals) 
      {
        HexTile t = activeWorld.tiles[Random.Range(0,activeWorld.tiles.Count)];
        /* 
        while(t.type == TileType.Gray || t.passable == false)
        {
          t = activeWorld.tiles[Random.Range(0,activeWorld.tiles.Count)];
        }
        */
        if(t.type != TileType.Gray && t.passable == true)
        {
          GameObject g = elementals[Random.Range(0,elementals.Length)] as GameObject;
          //Instantiate(g, Vector3.zero, Quaternion.identity);
          StartCoroutine(LangstonsElementals(RandomAnt(), g,t, activeWorld.tiles[t.neighbors[0]]));
          livingElementals.Add(g);
        }
      }   
      yield return new WaitForSeconds(Random.Range(eleSpawnMin, eleSpawnMax));
    }
  }
  public IEnumerator Test(RaycastHit hit)
  {
    Debug.Log(hit.point);
    yield return null;
  }
  public int GetHitTile(RaycastHit hit)
  {
    HexTile hitTile = new HexTile();
	  GameObject plateO = hit.transform.gameObject;
	  Vector3 c = new Vector3 ();
	  Vector3 h = hit.point;
	  float test;
	  float dist = 9999*worldScale;
	  foreach (HexTile ht in activeWorld.tiles) 
	  {
		  c = ht.hexagon.center;
			test = (c - h).sqrMagnitude;
		  if (test < dist) 
      {
			  dist = test;
				hitTile = ht;
		  }
	  }
    return hitTile.index;
  }

  World LoadWorld()
  {
    return BinaryHandler.ReadData<World>(World.cachePath);
  }
   
  public IEnumerator TestDistance(int from)
  {
    bool selecting = true;
    Debug.Log("select tile 'to'");
    while(selecting)
    {
      if(Input.GetKeyDown(KeyCode.Mouse1))
      {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, 100.0f))
        {
          int to = GetHitTile(hit);
          Debug.Log("to: " + to);
          int dist = activeWorld.TileDistanceFromTo(from,to);
          Debug.Log("Distance between " + from + " and " + to + ": " + dist);
          selecting = false;
        }
      }
      yield return null;
    }
  }
  void SetHeights() //@TODO we should be reading heights from hextile (based on the worldseed?)
  {
    foreach (TriTile tt in activeWorld.triTiles)
    {
	    //Debug.Log (ht.height);
      tt.height = 1f + tt.height;
    }
  }
  
  public void CapturePNG()
  {
    //GameObject selection = GameObject.Find ("Zone Prefab(Clone)");
		RenderTexture.active = activeTex;
    	//Camera cam = GameObject.Find("Dragon Camera").camera;
        int width = activeTex.width;
        int height = activeTex.height;
        Texture2D tex = new Texture2D (width, height, TextureFormat.ARGB32, false);
        Rect sel = new Rect ();
        sel.width = width;
        sel.height = height;
        sel.x = 0;
        sel.y = 0;//selection.transform.position.y;

        tex.ReadPixels (sel, 0, 0);
 
        byte[] bytes = tex.EncodeToPNG ();
        File.WriteAllBytes ("Assets/Dragons/" + worldCaptureName + ".png", bytes);
  }


  //@TODO: This is preliminary, it sets the ocean tiles using average scale 
  //by making any tile close to the average or below blue, then scaling the blue tiles up to the average.
	/*
  void CreateOcean()
  {
    foreach (TriTile ht in activeWorld.triTiles)
    {
      ht.type = TileType.Blue;
    }
    TileType typeToSet = TileType.Tan;
    foreach (TriTile ht in activeWorld.triTiles)
    {
      float rand = Random.Range(0, 1f);
      //@TODO: this is just a preliminary variation of the land types
      if (rand <= 0.4f)
        typeToSet = TileType.Brown;
      if (rand > 0.4f)
        typeToSet = TileType.Red;
      if (ht.height >= averageScale*0.99f)
      {
        ht.type = typeToSet;
      }
    }
    foreach (TriTile ht in activeWorld.triTiles)
    {
      if (ht.type == TileType.Blue)
      {
        ht.height *= (averageScale*0.99f / ht.height);
      }
    }
  }
  
  
  void DrawAxes()
  {
    if (!labelDirections || activeWorld.tiles.Count == 0)
      return;

    //int currentTileX = 13, currentTileY = 0, currentTileXY = 0;  

    // === Draw axes on all tiles ===
    for (int i=0; i<activeWorld.tiles.Count; i++)
    {
      DrawHexAxes(activeWorld.tiles, activeWorld.origin, i);
    }

    /*
    // === Draw Bands Only ===
    // Y-band
    for (int y=0; y<activeWorld.circumferenceInTiles; y++)
    {
      if (currentTileY != -1)
      {
        DrawHexAxes(activeWorld.tiles, activeWorld.origin, currentTileY);
        currentTileY = activeWorld.tiles[currentTileY].GetNeighborID(Direction.Y);
      }
    }
    // XY-band
    for (int xy=0; xy<activeWorld.circumferenceInTiles; xy++)
    {
      if (currentTileXY != -1)
      {
        DrawHexAxes(activeWorld.tiles, activeWorld.origin, currentTileXY);
        currentTileXY = activeWorld.tiles[currentTileXY].GetNeighborID(Direction.XY);
      }
    }
    // X-band
    for (int x=0; x<activeWorld.circumferenceInTiles; x++)
    {
      if (currentTileX != -1)
      {
        DrawHexAxes(activeWorld.tiles, activeWorld.origin, currentTileX);
        currentTileX = activeWorld.tiles[currentTileX].GetNeighborID(Direction.X);
      }
    }
    */
 /* }
   
  void DrawHexAxes(List<HexTile> tiles, Vector3 worldOrigin, int index, float scale = .1f, bool suppressWarnings = true)
  {
    if (index == -1)
    {
      Debug.LogError("Invalid index: -1");
      return;
    }

    SerializableVector3 origin = new SerializableVector3();
    try
    {
      origin = (tiles[index].hexagon.center + (SerializableVector3)worldOrigin) * 1.05f;
    }
    catch(System.Exception e)
    {
      Debug.LogError("Error accessing tile "+ index+": "+e);
      return;
    }

    for (int dir = 0; dir<Direction.Count && dir<tiles[index].hexagon.neighbors.Length; dir++)
    {
      int y = tiles[index].GetNeighborID(dir);
      if (y != -1)
      {
        Gizmos.color = Direction.ToColor(dir);
        SerializableVector3 direction = tiles[tiles[index].GetNeighborID(dir)].hexagon.center - tiles[index].hexagon.center;

        float finalScale = scale;
        if (dir == Direction.X || dir == Direction.Y || dir == Direction.NegXY)   // Prime directions
          finalScale *= 2;

        Gizmos.DrawRay((Vector3)origin, (Vector3)direction*finalScale);
      }
    }
  }

  void DrawHexIndices()
  {
    foreach (HexTile ht in activeWorld.tiles)
    {
      Transform t = (Transform)Instantiate(textMeshPrefab, (ht.hexagon.center-activeWorld.origin)*1.01f, Quaternion.LookRotation(activeWorld.origin-ht.hexagon.center));
      TextMesh x = t.GetComponent<TextMesh>();
      x.text = ht.index.ToString();
      t.parent = currentWorldTrans;
    }
  }
  
  public void HLShift ()
	{
		foreach (HexTile ht in activeWorld.tiles) {
			List<HexTile> htl = new List<HexTile>();
			foreach (int n in ht.neighbors) {
				htl.Add(activeWorld.tiles[n]);
			}
			HexTile[] hta = new HexTile[htl.Count];
			for (int i = 0; i < htl.Count; i++) {
				hta [i] = htl [i];
			}
			ht.HexLifeShift (hta);
		}
	}/*
  public void HL() //automata for 2 types
	{
		bool[] ba = new bool[activeWorld.tiles.Count];
		//List<HexTile> tiles = new List<HexTile>(activeWorld.tiles);
		for (int i = 0; i < activeWorld.tiles.Count; i++) {
			int sol = 0;
			int luna = 0;
			HexTile ht = activeWorld.tiles [i];
			foreach (int n in ht.neighbors) {
				if (activeWorld.tiles [n].type == TileType.Sol) {
					sol++;
				}
					
					if (activeWorld.tiles [i].type == TileType.Luna)
					{
						luna++;
					}
					 
				}
			//survive
			if (s1 && sol ==  1) {
				if (ht.type == TileType.Sol) {
					ht.generation++;
					ba [i] = true;
				}
			}
			if (s2 && sol == 2) {
				if (ht.type == TileType.Sol) {
					ht.generation++;
					ba [i] = true;
				}
			}
			if (s3 && sol == 3) {
				if (ht.type == TileType.Sol) {
					ht.generation++;
					ba [i] = true;
				}
			} 
			if (s4 && sol == 4) {
				if (ht.type == TileType.Sol) {
					ht.generation++;
					ba [i] = true;
				}
			}
			if (s5 && sol == 5) {
				if (ht.type == TileType.Sol) {
					ht.generation++;
					ba [i] = true;
				}
			}
			if (s6 && sol == 6) {
				if (ht.type == TileType.Sol) {
					ht.generation++;
					ba [i] = true;
				}
			}
			//born
			if (b1 && sol == 1) {
				if (ht.type != TileType.Sol) {
					ba [i] = true;
				}
			}
			if (b2 && sol == 2) {
				if (ht.type != TileType.Sol) {
					ba [i] = true;
				}
			}
			if (b3 && sol == 3) {
				if (ht.type != TileType.Sol) {
					ba [i] = true;
				}
			}
			if (b4 && sol == 4) {
				if (ht.type != TileType.Sol) {
					ba [i] = true;
				}
			}
			if (b5 && sol == 5) {
				if (ht.type != TileType.Sol) {
					ba [i] = true;
				}
			}
			if (b6 && sol == 6) {
				if (ht.type != TileType.Sol) {
					ba [i] = true;
				}
			}
		}
		//now change the types
		for (int i = 0; i < activeWorld.tiles.Count; i++)
		{
			HexTile ht = activeWorld.tiles [i];
			if (ba [i]) {
				ht.ChangeType (TileType.Sol);
			} 
			else 
			{
				ht.ChangeType (TileType.Luna);
			}
		}
	}     */
}
