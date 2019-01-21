using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LibNoise.Unity.Generator;
using System.Linq;
public enum WorldSize {None, Small, Medium, Large};
public enum WorldType {None, Verdant, Frigid, Oceanic, Barren, Volcanic, Radiant, Gaseous};
public enum Season {None, Spring, Summer, Fall, Winter};
public enum AxisTilt { None, Slight, Moderate, Severe };      // Affects intensity of difficulty scaling during seasons


[System.Serializable]
public class World
{
  public int[] state;
  public const string cachePath = "currentWorld.save";
  public string name;
  public WorldSize size;
  public WorldType type;
  public Season season;
  public AxisTilt tilt;
  public TileType element;
  public float glyphProb = 0.006f; //distribution of glyphs
  public float populationProb = 0.42f;
  public int maxObjects = 2400;
  public static int zeroState = 3;
  public static int oneState = 4;
  [HideInInspector] public SerializableVector3 origin;
  [HideInInspector] public int circumferenceInTiles;
  [HideInInspector] public int distanceBetweenTiles;
  [HideInInspector] public float circumference, radius;
  [HideInInspector] public int numberOfPlates; //Set by polysphere on cache
  [HideInInspector] public float seaLevel = 0;
  [HideInInspector] public List<HexTile> tiles;
  [HideInInspector] public List<TriTile> triTiles;
  [HideInInspector] public List<HexTile> pentagons;
  [HideInInspector] public List<Rune> runes;
  //[HideInInspector] public List<Plate> plates;
  [HideInInspector] public Dictionary<int, int> tileToPlate; //key hextile.index, value plate index

  private bool neighborInit;
  private List<List<HexTile>> _neighbors;
  public List<List<HexTile>> neighbors{
    get{
      if (!neighborInit)
      {
        if (tiles.Count < 1)
          Debug.LogError("Making neighbor list from null tiles");

        neighborInit = true;
        _neighbors = new List<List<HexTile>>();

        foreach (HexTile t in tiles)
        {
          List<HexTile> neighbs = new List<HexTile>();

          for (int i=0; i<t.hexagon.neighbors.Length; i++)
          {
            try
            {
              neighbs.Add(tiles[t.hexagon.neighbors[i]]);
            }
            catch (System.Exception e)
            {
              //Debug.LogError("tile "+t.index+"'s "+Direction.ToString(i)+" neighbor is bad: "+t.hexagon.neighbors[i]);
            }
          }
          _neighbors.Add(neighbs);
        }
      }

      return _neighbors;
    }
    set{}
  }

  public World()
  {
    origin = Vector3.zero;
  }

  public World(WorldSize s, WorldType t, Season se, AxisTilt at)
  {
    size = s;
    type = t;
    season = se;
    tilt = at;
    origin = Vector3.zero;
  }
  
  public void Populate(byte[] seed)
  {
    Object[] airBiome = Resources.LoadAll("Air/");
    Object[] earthBiome = Resources.LoadAll("Earth/");
    Object[] waterBiome = Resources.LoadAll("Water/");
    Object[] fireBiome = Resources.LoadAll("Fire/");
    Object[] darkBiome = Resources.LoadAll("Dark/");
    Object[] lightBiome = Resources.LoadAll("Light/");
    Object[] misc = Resources.LoadAll("Misc/");

    Perlin perlin = new Perlin();
    double f = 0;
    double p = 0;
    double l = 0;
    int o = 0;
    int pSeed = 0;
    float sc = 0; //99.0f
    float amplitude = 42; //42
    int h = 0;
    int iterations = 0;
    runes = new List<Rune>();
    //set generation to 3, the 0 state for life casting
    foreach(HexTile ht in tiles)
    {
      ht.generation = zeroState;
    }

    sc = 99; //Random.Range(99f,111f);
    f = 0.0000024; //(double)Random.Range(.0000014618f,.000001918f); //(double)Random.Range(0.000000618f,0.000000918f);//.01618;// * Random.Range(0.5f,1.5f);// * i; //.0000024
    l = 2.4; //(double)Random.Range(2.24f,4.42f);// * Random.Range(0.5f,1.5f);//2.4;
    p = .2; //(double)Random.Range(.16f,.191f);// * Random.Range(0.5f,1.5f); //.24
    o = 6; //Random.Range(3,7);// + i;
    amplitude = 42; //Random.Range(24,42);
    glyphProb /= 64;
    for(int i = 0; i < seed.Length; i++)
    {
      UnityEngine.Random.InitState(seed[i]);
      perlin.Seed = seed[i];

      float stepHeight = Random.Range(0.1f,0.5f);
      PerlinPopulate(perlin,f,l,p,o,amplitude,sc,stepHeight);
    }
    
      //Heightmap perlin seed
      //UnityEngine.Random.InitState(randseed);
      //perlin.Seed = perlinseed;
      //pSeed += seed[i];
      
      //iterations = 32;//Random.Range(2,7);

    //Populate with params found in seed
    //Normalize perlin values
    //pSeed /= seed.Length;
    //sc /= seed.Length;
    //f /= seed.Length;
    //l /= seed.Length;
    //p /= seed.Length;
    //iterations = (iterations % 12) + 1; //seed.Length;
    //o = (o % 32) + 3;

    //glyphProb /= iterations;
    //populationProb /= iterations;
    //Debug.Log("octave: " + o + " sc: " + sc + " freq: " + f + " lac: " + l + " pers: " + p + " iterations: " + iterations);
  
    

    //PerlinPopulate(perlin,pSeed,f*6,l,p,o,amplitude,sc);
    //PerlinPopulate(perlin,pSeed,f,l,p,o,amplitude*10,sc);
    
    
    
    //biomes and ocean
  
    int water = 0; 
    int fire = 0;
    int vapor = 0;
    int crystal = 0;

    seaLevel = AverageTileHeight() - 1;// + 0.1f;
    Debug.Log("sea level: " + seaLevel);
    //water world
    foreach(HexTile ht in tiles)
    {
      if(ht.type == TileType.Water){water++;}
      if(ht.type == TileType.Fire){fire++;}
      if(ht.type == TileType.Vapor){vapor++;}
      if(ht.type == TileType.Crystal){crystal++;}
    }

    if(water >= fire && water >= vapor && water >= crystal)
    {
      element = TileType.Water;
      foreach(HexTile ht in tiles)
      {
        if((ht.type == TileType.Water))//&& ht.hexagon.scale < seaLevel)
        {
          if(ht.hexagon.scale > seaLevel)
          {
            ht.type = TileType.Earth;
          }
        }
        
      }
      LightToDark();
    }
    //fire world
    if(fire > water && fire > vapor && fire > crystal)
    {
      element = TileType.Fire;
      foreach(HexTile ht in tiles)
      {
        
        if((ht.type == TileType.Water || ht.type == TileType.Fire))// && ht.hexagon.scale < seaLevel)
        {
          //ht.hexagon.scale = seaLevel;
        }
        
      }
      DarkToLight();
    }
    //vapor
    if(vapor >= water && vapor >= fire && vapor >= crystal)
    {
      element = TileType.Vapor;
      //foreach(HexTile ht in tiles)
      //{
        /* 
        if(ht.type == TileType.Vapor || ht.type == TileType.Crystal)
        {
          ht.hexagon.scale = seaLevel;
        }
        */
      //}
      //DarkToLight();
    }
    //crystal
    if(crystal > fire && crystal > vapor && crystal > water)
    {
      element = TileType.Crystal;
      //foreach(HexTile ht in tiles)
      //{
        /* 
        if(ht.type == TileType.Vapor || ht.type == TileType.Crystal)
        {
          ht.hexagon.scale = seaLevel;
        }
        */
      //}
      //LightToDark();
    }
     
    foreach(HexTile ht in tiles)
    {
      if(element == TileType.Water || element == TileType.Fire)
      {    
        if(ht.hexagon.scale <= seaLevel)
        {
          ht.type = element;
          ht.oceanTile = true;
          //ht.passable = false;
          ht.hexagon.scale = seaLevel;
        }
      }
    }
    
    int numObjects = 0;
    //biome objects
    foreach(HexTile ht in tiles)
    {
      //if surrounded by ocean tiles, become oceantile
      int x = 0;
      foreach(int n in ht.neighbors)
      {
        if(tiles[n].oceanTile)
        {
          x++;
        }
      }
      if(x == ht.neighbors.Count)
      {
        ht.generation = zeroState;
        ht.hexagon.scale = seaLevel;
      }

      //choose which object to place
      if(ht.placeObject && numObjects <= maxObjects && !ht.oceanTile)
      {
        ht.passable = false;
        numObjects++;
        switch(ht.type)
        {
          case TileType.Gray: ht.objectToPlace = Random.Range(0,misc.Length); break;
          case TileType.Water: ht.objectToPlace = Random.Range(0,waterBiome.Length); break;
          case TileType.Fire: ht.objectToPlace = Random.Range(0,fireBiome.Length); break;
          case TileType.Earth: ht.objectToPlace = Random.Range(0,earthBiome.Length); break;
          case TileType.Air: ht.objectToPlace = Random.Range(0,airBiome.Length); break;
          case TileType.Dark: ht.objectToPlace = Random.Range(0,darkBiome.Length); break;
          case TileType.Light: ht.objectToPlace = Random.Range(0,lightBiome.Length); break;

          case TileType.Ice: ht.objectToPlace = Random.Range(0,waterBiome.Length); break;
          case TileType.Metal: ht.objectToPlace = Random.Range(0,fireBiome.Length); break;
          case TileType.Arbor: ht.objectToPlace = Random.Range(0,earthBiome.Length); break;
          case TileType.Astral: ht.objectToPlace = Random.Range(0,airBiome.Length); break;
          case TileType.Crystal: ht.objectToPlace = Random.Range(0,darkBiome.Length); break;
          case TileType.Vapor: ht.objectToPlace = Random.Range(0,lightBiome.Length); break;
          default: break;
        }
      }
    }
  }
  public void PerlinPopulate(Perlin perlin, double frequency, double lacunarity, double persistence, int octave, float amplitude, float scale, float stepHeight)
  {
    //Random.InitState(seed);
    //perlin.Seed = seed;
    perlin.Frequency = frequency;
    perlin.Lacunarity = lacunarity;
    perlin.Persistence = persistence;
    perlin.OctaveCount = octave;
    int typeShift = Random.Range(0,21);
    foreach(HexTile ht in tiles)
    {
      //Get next height
      double perlinVal = perlin.GetValue(ht.hexagon.center.x * scale, ht.hexagon.center.y * scale, ht.hexagon.center.z * scale);
      double v1 = perlinVal*amplitude;//*i; 
      int h = (int)v1;
      //Debug.Log(v1);
      ht.hexagon.scale += h/(1+(stepHeight/2f));//1.5f;
      if(ht.generation == zeroState) //keep glyphs
      {
        int v = Mathf.Abs((int)ht.type + h + typeShift);
        int t = (v % 12) + 1; //using 12 types
        ht.type = (TileType)t;
      }
      float gP = Random.Range(0f,1.0f);
      if(glyphProb > gP)
      {
        byte[] newID = new byte[32];
        for(int b = 0; b < 32; b++)
        {
          newID[b] = (byte)Random.Range(0,256);
        }
        //Create new rune on world (locked in)
        Rune newRune = new Rune(newID);
        ht.generation = newRune.tile.uvy;
        ht.placeObject = false;
        newRune.hexTile = ht.index;
        runes.Add(newRune);
      } 

      float r = Random.Range(0,1.0f);
      if(r < 0.24f && ht.generation == zeroState)// populationProb)// && ht.generation == zeroState)
      {
        //populate, unless too many neighbors are populated
        int neighborPopulation = 0;
        foreach(int htn in ht.neighbors)
        {
          if(tiles[htn].placeObject)
          {
            neighborPopulation++;
          }
        }
        if(neighborPopulation < 1)
        {
          ht.placeObject = true;
          //ht.passable = false;
        }
      }
    }
  }
  public void ReadState()
  {
    //state of tiletypes
    state = new int[tiles.Count];
    for (int i = 0; i < tiles.Count; i++)
    {
      state[i] = (int)tiles[i].type;
    }
  }

  public void SetState(int[] st)
  {
    state = st;
    foreach(HexTile ht in tiles)
    {
      ht.ChangeType((TileType)state[ht.index]);
    }
  }

  public void Clear()
  {
    foreach(HexTile ht in tiles)
    {
      if(ht.type != TileType.Gray){ht.ChangeType(TileType.Gray);}
      ht.antPasses = 0;
      ht.generation = 0;
    }
  }
  
/*
  public void Imbue(int[] glyph, HexTile origin)
  {

  }
*/
/*  an imprecise but working solution
public List<int> GetTilesInRadius(float radius, int origin)
{
  List<int> output = new List<int>();
  foreach(HexTile ht in tiles)
  {
    if((ht.hexagon.center - tiles[origin].hexagon.center).magnitude <= radius)
    {
      output.Add(ht.index);
    }
  }
  return output;
}*/

  public List<int> GetTilesInRadius(int radius, int origin)
  {
    List<int> returnedTiles = new List<int>();
    List<int> tilesToAdd = new List<int>();
    returnedTiles.Add(origin);
    for(int r = 0; r < radius; r++)
    {
      foreach(int t in returnedTiles)
      {
        foreach(int n in tiles[t].neighbors)
        {
          tilesToAdd.Add(n);
        }
      }
      foreach(int a in tilesToAdd)
      {
        returnedTiles.Add(a);
      }
      returnedTiles = returnedTiles.Distinct().ToList();
    }
    return returnedTiles;
  }
  
  public int TileDistanceFromTo(int from, int to)
  { 
    //Just brute force it with GetTilesInRadius, expensive but simple solution
    for(int r = 0; r <= 1000; r++)
    {
      List<int> t = GetTilesInRadius(r,from);
      foreach(int i in t)
      {
        if(to == i)
        {
          return r;
        }
      }
    }
    Debug.Log("Couldn't find distance between " + from + " and " + to);
    return 1000;
  }
  
  public float AverageTileHeight()
  {
    float h = 0;
    foreach(HexTile ht in tiles)
    {
      h += ht.hexagon.scale;
    }
    h /= tiles.Count;
    return h;
  }

  public void PrepForCache(float scale, int subdivisions)
  {
    if (tiles == null || tiles.Count == 0)
    {
      Debug.Log("creating new world cache. Scale:" + scale + " Subs: " + subdivisions);
      neighborInit = false;
      PolySphere sphere = new PolySphere(Vector3.zero, scale, subdivisions);
      //make the tileToPlate dict
      numberOfPlates = sphere.numberOfPlates;
      //tileToPlate = new Dictionary<int, int>();
      CacheHexes(sphere);
      //CacheTriangles(sphere);
    }
    else
      Debug.Log("tiles not null during cache prep");
  }
  public void CacheTriangles(PolySphere s)
  {
    triTiles = new List<TriTile>(s.triTiles);
  }
  public void CacheHexes(PolySphere s)  // Executed by the cacher.  @CHANGE: Now directly converting spheretiles to hextiles
  { 
    tiles = new List<HexTile>(s.hexTiles);
    Debug.Log(tiles[0].hexagon.scale);
    neighborInit = false;
  }

  public void LightToDark()
  {
    foreach(HexTile ht in tiles)
    {
      if(ht.type == TileType.Light){ht.type = TileType.Dark;}
      if(ht.type == TileType.Metal){ht.type = TileType.Ice;}
      if(ht.type == TileType.Fire){ht.type = TileType.Water;}
      if(ht.type == TileType.Astral){ht.type = TileType.Arbor;}
      if(ht.type == TileType.Air){ht.type = TileType.Earth;}
      if(ht.type == TileType.Crystal){ht.type = TileType.Vapor;}
      if(ht.type == TileType.Sol){ht.type = TileType.Luna;}
    }
  }
  public void DarkToLight()
  {
    foreach(HexTile ht in tiles)
     {
      if(ht.type == TileType.Dark){ht.type = TileType.Light;}
      if(ht.type == TileType.Ice){ht.type = TileType.Metal;}
      if(ht.type == TileType.Water){ht.type = TileType.Fire;}
      if(ht.type == TileType.Arbor){ht.type = TileType.Astral;}
      if(ht.type == TileType.Earth){ht.type = TileType.Air;}
      if(ht.type == TileType.Vapor){ht.type = TileType.Crystal;}
      if(ht.type == TileType.Luna){ht.type = TileType.Sol;}
     }
  }   
}
   
