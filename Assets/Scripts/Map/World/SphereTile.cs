using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using LibNoise.Unity;
using LibNoise.Unity.Generator;
using LibNoise.Unity.Operator;
//This is where you get to use things that you don't want to serialize (vectors, etc.)
//Remember to add things to ToHexTile() and HexTile if you want them to be serialized
public class SphereTile
{
  public int index;    // The index of the tile in our map. Translates into HexTile.id [set by PolySphere]
  public int[] neighbors;   // Indexes of the surrounding sphere tiles in our map [set by PolySphere] in array form for serialization
  public int plate = -1; //Polysphere
  public int distanceFromBoundary; //Polysphere
  public float height; //passed to HexTile
  public Dictionary<int, SphereTile> neighborDict;    // A list of unique neighbors
  public List<SphereTile> neighborList;   // This is the first raw list, which will contain duplicates
  public bool boundary; //plate boundary
  public bool plateOrigin;
  public bool colliding; //OnCollisionStay
  public bool hSet;
  public TileType type;
  //The inital triangles from the subdivided polysphere which we will use to build the spheretile
  public List<Triangle> subTriangles;
  //The triangles that make up this piece of the entire dual polygon
  public List<Triangle> faceTris;
  public List<Triangle> sideTris;
  //Checking equality with the center vertex 
  public Vector3 center;
  public Vector3 origin;
  public Vector3 drift;

  //Tectonics Properties
  private float _elevation;
  public float elevation
  {
    get { return _elevation; }
    set { _elevation = value; }
  }
  private float _heat;

  public float heat
  {
    get { return _heat; }
    set { _heat = value; }
  }
  private float _precipitation;

  public float precipitation
  {
    get { return _precipitation; }
    set { _precipitation = value; }
  }

  //Scaling property
  private float _scale = 24;
  public float scale
  {
    get { return _scale; }
    set
    {
      _scale = value;
      center.Normalize();
      center *= scale;
      foreach (Triangle t in faceTris)
      {
        t.v1.Normalize();
        t.v1 *= scale;
        t.v2.Normalize();
        t.v2 *= scale;
        t.v3.Normalize();
        t.v3 *= scale;
        t.center.Normalize();
        t.center *= scale;
      }
      foreach (Triangle t in sideTris)
      {
        t.v1.Normalize();
        t.v1 *= scale;
        t.v2.Normalize();
        t.v2 *= scale;
        t.v3.Normalize();
        t.v3 *= scale;
        t.center.Normalize();
        t.center *= scale;
      }
    }
  }

  //Unit SphereTile
  public SphereTile() { }

  public SphereTile(Vector3 c, Vector3 o)
  {
    center = c;
    origin = o;
    faceTris = new List<Triangle>();
    neighbors = new int[7];
    neighborDict = new Dictionary<int, SphereTile>();
    neighborList = new List<SphereTile>();
    sideTris = new List<Triangle>();
    subTriangles = new List<Triangle>();

  }
  //Given the subdivided triangles, build the spheretile, which is made up of 12 triangles. 6 face, 6 side
  public void Build()
  {
    //simplex = new SimplexNoise(GameManager.gameSeed);
    List<Triangle> subCopies = new List<Triangle>(subTriangles);
    //unit vector in the direction of subCopies[0].center. Each vector will have the same scaling factor, so we only need to do this once
    Vector3 dir = subCopies[0].center / subCopies[0].center.magnitude;
    //center gives us both a point and normal for our face plane
    //Parameter (D) determined by plane equation
    float planeParam = -center.x * center.x - center.y * center.y - center.z * center.z;
    //Scaling factor determined by plane equation parameters -> s(Ax + By + Cz) + D = 0
    float s = (-planeParam) / (center.x * dir.x + center.y * dir.y + center.z * dir.z);
    //Scale our vectors to be on the face plane
    foreach (Triangle t in subCopies)
    {
      t.center = (t.center.normalized * s); 
    }

    Triangle startingAt = subCopies[0];
    Triangle triCopy = new Triangle(subCopies[0].v1, subCopies[0].v2, subCopies[0].v3);
    Transform triCopyTrans = GameManager.myTrans;
    triCopyTrans.rotation = Quaternion.identity;
    triCopyTrans.position = triCopy.center;

    List<float> subs = new List<float>();


    for (int z=0;z < 6;z++) //Make our 12 triangles (the pentagons apparently work) 
    {
      //Rotate our tester to where we want it, check for subTriangle here
      triCopyTrans.RotateAround(center, center, 60);

      //Get the list of triCopyTrans distance vectors and sort it to find the smallest(?)
      subs.Clear();
      foreach (Triangle t in subCopies)
      {
        subs.Add(((Vector3)t.center - triCopyTrans.position).sqrMagnitude);
      }
      subs.Sort();
      foreach (Triangle t in subCopies)
      {
        //Debug.Log(subs.Count);
        //Debug.Log(((Vector3)t.center - triCopyTrans.position).sqrMagnitude);
        //If this center corresponds to the smallest value in subs
        if (((Vector3)t.center - triCopyTrans.position).sqrMagnitude == subs[0])
        {
          Triangle faceTri = new Triangle(center, startingAt.center, t.center);
          Triangle sideTri = new Triangle(Vector3.zero, faceTri.v3, faceTri.v2);
          faceTris.Add(faceTri);
          sideTris.Add(sideTri);
          startingAt = t;
        }
      }
    }
  }

  void OnCollisionStay()
  {
    colliding = true;
  }
  public List<TriTile> ToTriTiles()
  {
    List<TriTile> tiles = new List<TriTile>();
    foreach (Triangle t in subTriangles)
    {
      List<int> neighbors = new List<int>();
      neighbors.Add(t.top.index);
      neighbors.Add(t.right.index);
      neighbors.Add(t.left.index);
      tiles.Add(new TriTile(t, plate, neighbors, boundary, height, type));
    }
    return tiles;
  }
  public HexTile ToHexTile()
  {
    Vector3[] verts = new Vector3[1];
    verts = new Vector3[]{faceTris[0].v2, faceTris[0].v3, faceTris[1].v3, faceTris[2].v3, faceTris[3].v3, faceTris[4].v3};
    Hexagon hex = new Hexagon(index, center, verts, origin);
    //Dictionary<Vector3, SphereTile> neighbs = new Dictionary<Vector3, SphereTile>();
    //convert neighbors to index list
    List<int> neig = new List<int>();
    foreach (SphereTile st in neighborDict.Values.ToList())
    {
      neig.Add(st.index);
    }
    return new HexTile(hex, plate, neig, boundary, height, type);
  }
}

