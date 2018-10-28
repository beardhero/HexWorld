using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PolySphere
{
  public static Random rnd = new Random();
  //Initial icosahedron coords
  public static List<Vector3> icoCoords;
  public GameObject go; //Using this transform to rotate around centers of hexes
  public Vector3 origin;
  public int subdivisions;
  public float scale = 24;
  public float avgTileHeight;
  public float tileCount;
  public TileType element;

  public List<Triangle> icosahedronTris;
  public List<List<Triangle>> subdividedTris;
  public List<Triangle> finalTris;    // The finest level of subdivided tris
  public List<HexTile> hexTiles;
  //public List<HexTile> orbitTiles;
  public List<TriTile> triTiles;
  public List<SphereTile> sTiles; //the tiles on this sphere
  public List<List<SphereTile>> tPlates;
  public List<Plate> plates;
  public int numberOfPlates;
  public int maxTilesPerPlate = 6500;
  public float initialHeight;
  public float oceanProb = 0.6f; //redacted
  
  public PolySphere(){}

  public PolySphere(Vector3 o, float s, int d)
  {
    origin = o;
    scale = s;
    subdivisions = d;

    icoCoords = new List<Vector3>();
    icosahedronTris = Icosahedron(scale);
 
    SubdivideAndDuals(); //Builds SphereTiles
    
    
    //SimplexHeights();
    //SimplexHeights();
    //SimplexHeights();
    //AvgHeight();
    //CorrectSunkenTiles();
    TectonicPlates(); //Populates plates and creates stress forces between them.
    //RandomPlateAttunement();
	  
    //CacheTris();
    CacheHexes(); //Converts to HexTiles for serialization
    
    //avgTileHeight = AvgHexHeight();
  
    //HeightSeed();
  }
 
  void RandomPlateAttunement()
  {
    TileType t;
    TileType[] tta = new TileType[6] {
				TileType.Luna,
				TileType.Sol,
				TileType.Water,
				TileType.Air,
				TileType.Earth,
				TileType.Fire,
			};
    foreach(Plate p in plates)
    {
      t = tta[Random.Range(0,5)];
      foreach(SphereTile st in p.tiles)
      {
        st.type = t;
      }
    }
  }
  
  void Flatten(){
		foreach (HexTile t in hexTiles) {
			t.hexagon.scale = scale ;
		}		
  }
  void HeightSeed()
  {
    foreach (HexTile ht in hexTiles)
    {
      ht.hexagon.scale = scale;
    }
    Debug.Log(hexTiles[0].hexagon.scale);
  }

  public float AvgHexHeight()
  {
    foreach (HexTile ht in hexTiles)
    {
      avgTileHeight += ht.hexagon.scale;
    }
    avgTileHeight /= hexTiles.Count;
    return avgTileHeight;
  }
  void AvgHeight()
  {
    foreach (SphereTile st in sTiles)
    {
      avgTileHeight += st.height;
      tileCount++;
    }
    avgTileHeight /= tileCount;
  }

  void CorrectSunkenTiles()
  {
    foreach (SphereTile st in sTiles)
    {
      if (st.height < avgTileHeight)
      {
        st.height = avgTileHeight + 0.1f;
      }
    }
  }

  void CacheTris()
  {
    triTiles = new List<TriTile>();
    //index
    int ind = 0;
    foreach (SphereTile st in sTiles)
    {
      foreach(TriTile tt in st.ToTriTiles())
      {
        tt.index = ind;
        ind++;
        triTiles.Add(tt);
      }
    }
  }
  
  void CacheHexes()
  {
    hexTiles = new List<HexTile>();
    
    foreach (SphereTile st in sTiles)
    {
      // Cache neighbors in list
      foreach (SphereTile t in st.neighborList)
      {
        if (!st.neighborDict.ContainsKey(t.index))
          st.neighborDict.Add(t.index, t);
      }
    }
    
    // === Cache unit hexagons ===
    foreach (SphereTile st in sTiles)
    {
      hexTiles.Add(st.ToHexTile()); //plate index passed along to hextiles, these are not units anymore
    }
    // === Assign neighbors to unit hexes ===
    List<Hexagon> toNei = new List<Hexagon>();
    foreach (HexTile ht in hexTiles)
    {
      toNei.Add(ht.hexagon);
    }
    //RecursiveNeighbors(toNei);
    TraverseAndAssignNeighbors(toNei, sTiles);
  }

  void SubdivideAndDuals()
  {
    List<Triangle> currentTris;
    List<Triangle> nextTris = new List<Triangle>(icosahedronTris); //Original icosahedron
    List<List<Triangle>> subdividedTris = new List<List<Triangle>>();

    sTiles = new List<SphereTile>();
    
    // Subdivide icosahedron
    for (int i = 0; i < subdivisions; i++)
    {
      currentTris = new List<Triangle>(nextTris);
      nextTris = new List<Triangle>();
      //triforces = new List<Triforce>();

      foreach (Triangle tri in currentTris)
      {
        //Bisect
        Vector3 v1 = (tri.v1+tri.v2)/2.0f;
        Vector3 v2 = (tri.v2+tri.v3)/2.0f;
        Vector3 v3 = (tri.v3+tri.v1)/2.0f;
        
        //Project onto sphere
        v1 *= (float)(1.902084 / v1.magnitude) * scale; //golden rectangle sphere radius 1.902084
        v2 *= (float)(1.902084 / v2.magnitude) * scale;
        v3 *= (float)(1.902084 / v3.magnitude) * scale;

        //Add the four new triangles
        Triangle mid = new Triangle(v1, v2, v3, tri, TriforcePosition.Mid, subdivisions);
        nextTris.Add(mid);   // Center of triforce

        Triangle top = new Triangle(tri.v1, v1, v3, tri, TriforcePosition.Top, subdivisions);
        nextTris.Add(top);

        Triangle right = new Triangle(v1, tri.v2, v2, tri, TriforcePosition.Right, subdivisions);
        nextTris.Add(right);

        Triangle left = new Triangle(v3, v2, tri.v3, tri, TriforcePosition.Left, subdivisions);
        nextTris.Add(left);

        tri.AssignChildren(mid, top, left, right);
      }
      
      //Set Neighbors
      foreach (Triangle tri in currentTris)
      {
        tri.childMid.AssignNeighbors(tri.childTop, tri.childRight, tri.childLeft);
        tri.childTop.AssignNeighbors(tri.NeighborOne(tri.childTop), tri.childMid, tri.NeighborTwo(tri.childTop));
        tri.childRight.AssignNeighbors(tri.NeighborOne(tri.childRight), tri.NeighborTwo(tri.childRight), tri.childMid);
        tri.childLeft.AssignNeighbors(tri.childMid, tri.NeighborOne(tri.childLeft), tri.NeighborTwo(tri.childLeft));
      }    

      //Save our subdivided levels
      subdividedTris.Add(nextTris); 
    }
    finalTris = new List<Triangle>(nextTris);
    //Create SphereTiles
    foreach (Triangle tri in finalTris)
    {
      
      //Tiles to assign
      SphereTile st1 = null, 
                 st2 = null, 
                 st3 = null;
      
      //Create empty SphereTiles, or, if we've already created a SphereTile at this point just reference it
      foreach (SphereTile st in sTiles)
      {
        if ((Vector3)st.center == (Vector3)tri.v1)
        {
          st1 = st;
        }
        if ((Vector3)st.center == (Vector3)tri.v2)
        {
          st2 = st;
        }
        if ((Vector3)st.center == (Vector3)tri.v3)
        {
          st3 = st;
        }
      }
      if (st1 == null)
      {
        st1 = new SphereTile(tri.v1, origin);
        sTiles.Add(st1);
      }
      if (st2 == null)
      {
        st2 = new SphereTile(tri.v2, origin);
        sTiles.Add(st2);
      }
      if (st3 == null)
      {
        st3 = new SphereTile(tri.v3, origin);
        sTiles.Add(st3);
      }


      //Add in the new neighbors from this triangle
        st1.neighborList.Add(st2);
        st1.neighborList.Add(st3);

        st2.neighborList.Add(st1);
        st2.neighborList.Add(st3);

        st3.neighborList.Add(st1);
        st3.neighborList.Add(st2);

      //Add this triangle as an inital triangle in each spheretile
      st1.subTriangles.Add(tri);
      st2.subTriangles.Add(tri);
      st3.subTriangles.Add(tri);
    }
    
    // --- Number sphere tiles ---
    int count = 0;
    //Build the SphereTiles!
    foreach(SphereTile st in sTiles)
    {
      st.index = count;
      count++;

      //st.scale *= scale;
      st.Build();
    }
  }
  void TectonicPlates()
  {
    //Give each spheretile (and hextile) a plate index
    //make plates based on index
    tPlates = new List<List<SphereTile>>();
    
    //Start at some random points across the sphere
    //each tile will have a chance to be assigned its own plate
    Debug.Log("tiles per plate: " + maxTilesPerPlate + " tile count: " + sTiles.Count);
    numberOfPlates = (int)(1 + sTiles.Count/maxTilesPerPlate);//Random.Range(minPlates, maxPlates);
    Debug.Log("plate count: " + numberOfPlates);
    for (int f = 0; f < numberOfPlates; f++)
    {
      //pick origin tiles for each plate
      int rand = Random.Range(0, sTiles.Count);
      if (!sTiles[rand].plateOrigin)//&& tPlates.Count < maxPlates)
      {
        tPlates.Add(new List<SphereTile>());
        tPlates[f].Add(sTiles[rand]);
        //makes this spheretile an origin of a plate, which we will build the plates from with a flood fill
        sTiles[rand].plateOrigin = true;
      }
      else
      {
        if(f > 0){f--;}
      }
    }
    //Fill in neighbors, fill in neighbors of neighbors, repeat until filled
    FloodFill();
    //Debug.Log(plates.Count);
    //Make the plates!
    BuildPlates();
    //Create stress forces between plates
    //Tectonics();
  }

  void Tectonics()//List<SphereTile> setting)
  {
	  foreach (Plate p in plates)
	  {
      //Debug.Log(" plate " + p.index);
      foreach (SphereTile st in p.boundary)
      {
        //Debug.Log("plate " + p.index + "tile" + st.plate);
        st.distanceFromBoundary = 0;
        foreach (SphereTile stn in st.neighborList)
        {
          if(stn.plate != st.plate)
          {
            st.hSet = true;
            Plate neighbor = plates[stn.plate];
            //find relative force between tiles and determine height of boundary
            Vector3 pressure = st.drift - stn.drift; //overall direction of drift, heights will shift in this direction
                                                      //(a dot b)/|b|, a in direction of b
            float pressureOnTile = Vector3.Dot(pressure, st.center - stn.center) / st.center.magnitude;
            //shear, the component of pressure perpendicular to st.center-stn.center
            Vector3 perp = Quaternion.AngleAxis(-90, st.center) * (st.center - stn.center);
            float shear = Vector3.Dot(pressure, perp) / perp.magnitude;
            //Give this tile a new drift so the next set of tiles can use it
            st.drift += (pressure);

            if (pressureOnTile == 0)
            {
              Debug.Log("perpendicular");
            }
            //collide plates
            if (!p.oceanic && !neighbor.oceanic)
            {
              //land
              //max height and adjust height with drift component in center direction, (a dot b)/|b|
              st.type = TileType.Gray;
              st.height += (pressureOnTile);
            }
            if (!p.oceanic && neighbor.oceanic)
            {
              //subducted, add a little more to the land at first then drop off
              st.type = TileType.Gray;
              st.height = avgTileHeight + .02f;
              p.subducted = true;
              st.height += (pressureOnTile);
            }
            if (p.oceanic)
            {
              //Debug.Log(p.oceanic + " second " + p.index);
              //ocean
              float rand = Random.Range(0, 1.0f);
              if (rand < oceanProb)
              {
                st.type = TileType.Water;
                st.height = avgTileHeight;
              }
              else
              {
                st.type = TileType.Water;
                st.height = avgTileHeight;
              }
            }
            break; //one neighbor for each tile
          }   
        }
      }
	  }
    //Boundaries done, now fill the plates
    foreach (Plate p in plates)
    {
      int dFromB = 0;
      FillPlate(p.boundary, dFromB, p);
    }
  }
  void FillPlate(List<SphereTile> tiles, int dFromB, Plate p) //recursive
  {
    dFromB++;
    List<SphereTile> next = new List<SphereTile>();
    foreach (SphereTile st in tiles)
    {
      st.distanceFromBoundary = dFromB;
      foreach (SphereTile stn in st.neighborList)
      {
        if (stn.plate == st.plate && !stn.hSet) //same plate index
        {
          stn.hSet = true;
          next.Add(stn);
          //Repeated relation calculation
          Vector3 pressure = st.drift - stn.drift; //overall direction of drift, heights will shift in this direction
                                                   //(a dot b)/|b|, a in direction of b
          float pressureOnTile = Vector3.Dot(pressure, st.center - stn.center) / st.center.magnitude;
          //shear, the component of pressure perpendicular to st.center-stn.center
          Vector3 perp = Quaternion.AngleAxis(-90, st.center) * (st.center - stn.center);
          float shear = Vector3.Dot(pressure, perp) / perp.magnitude;
          //adjust drift for next set
          st.drift += (pressure);
          
          if (p.oceanic)
          {
            //Debug.Log(p.oceanic);
            stn.type = TileType.Water;
            stn.height = avgTileHeight;
          }
          if (!p.oceanic)
          {
            stn.type = TileType.Gray;
            stn.height += (pressureOnTile);
          }
          break; //one neighbor per tile
        }
      }
    }
    if (next.Count != 0)
    {
      FillPlate(next, dFromB, p);
    }
  }
  void FloodFill() //Recursive
  {
    bool go = false;
    List<SphereTile> toAssign = new List<SphereTile>();
    for(int i = 0; i < tPlates.Count; i++)
    {
      foreach (SphereTile st in tPlates[i])
      {
        foreach (SphereTile nst in st.neighborList)
        {
          if (nst.plate == -1)
          {
            toAssign.Add(nst);
            nst.plate = i;
          }
        }
      }
      foreach (SphereTile s in toAssign)
      {
        tPlates[i].Add(s);
      }
    }
    //check if we've done all the tiles
    foreach (SphereTile s in sTiles)
    {
      if (s.plate == -1)
      {
        go = true;
      }
    }

    if (go) { FloodFill(); } 
  }

  void BuildPlates()
  {
    plates = new List<Plate>();
    SphereTile toOrigin = new SphereTile();
    //first get toPlate and toOrigin then make the plates
    for (int i = 0; i < tPlates.Count; i++)
    {
      List<SphereTile> toPlate = new List<SphereTile>();
      foreach (SphereTile st in sTiles)
      {
        if (st.plate == i)
        {
          toPlate.Add(st);
        }
      }
      int x = 0;
      foreach (SphereTile st in toPlate)
      {
        if (st.plateOrigin)
        {
          toOrigin = st;
          x++;
          break;
        }
        if(x>1)
        { Debug.Log("same plate origin: " + x); }
      }
      plates.Add(new Plate(toPlate, toOrigin, i));
      //toPlate.Clear();
    }
    //now get boundary by looking at neighbors
    //Debug.Log(plates.Count);
    foreach (Plate p in plates)
    {
      List<SphereTile> toBoundary = new List<SphereTile>();
      foreach (SphereTile st in p.tiles)
      {
        foreach (SphereTile stn in st.neighborList)
        {
          //Debug.Log("parent " + st.plate + " neighbor " + stn.plate);
          if (stn.plate != st.plate)
          {
            //If a neighbor has a plate index other than the parent's, add it to the boundary
            toBoundary.Add(st);
            st.boundary = true;
            break;
          }
        }
      }
      //set boundary
      p.boundary = toBoundary;
      //toBoundary.Clear(); 
    }
    //Boundaries are defined, next tile heights are set by Tectonics()
  }



  List<Triangle> Icosahedron(float scale)
  {
    if (scale <= 0)
      Debug.LogError("NO SCALE?!");
    if (scale > 50)
      Debug.LogError("Why so big?");
      
    List<Triangle> output = new List<Triangle>();
    List<Vector3> vertices = new List<Vector3>();

    float goldRat = 1.618f;

    //Icosahedron coords
    Vector3 origin = Vector3.zero,
            xy1 = new Vector3(1, goldRat, 0) * scale,
            xy2 = new Vector3(1, -goldRat, 0) * scale,
            xy3 = new Vector3(-1, -goldRat, 0) * scale,
            xy4 = new Vector3(-1, goldRat, 0) * scale,
            xz1 = new Vector3(goldRat, 0, 1) * scale,
            xz2 = new Vector3(goldRat, 0, -1) * scale,
            xz3 = new Vector3(-goldRat, 0, -1) * scale,
            xz4 = new Vector3(-goldRat, 0, 1) * scale,
            zy1 = new Vector3(0, 1, goldRat) * scale,
            zy2 = new Vector3(0, 1, -goldRat) * scale,
            zy3 = new Vector3(0, -1, -goldRat) * scale,
            zy4 = new Vector3(0, -1, goldRat) * scale;
    icoCoords.Add(xy1);
    icoCoords.Add(xy2);
    icoCoords.Add(xy3);
    icoCoords.Add(xy4);
    icoCoords.Add(xz1);
    icoCoords.Add(xz2);
    icoCoords.Add(xz3);
    icoCoords.Add(xz4);
    icoCoords.Add(zy1);
    icoCoords.Add(zy2);
    icoCoords.Add(zy3);
    icoCoords.Add(zy4);


    //Debug.Log(xz4.magnitude);
    vertices.Add(origin);         // 0
    vertices.Add(origin + xy1);   // 1
    vertices.Add(origin + xy2);   // 2
    vertices.Add(origin + xy3);   // 3
    vertices.Add(origin + xy4);   // 4
    vertices.Add(origin + xz1);   // 5
    vertices.Add(origin + xz2);   // 6
    vertices.Add(origin + xz3);   // 7
    vertices.Add(origin + xz4);   // 8
    vertices.Add(origin + zy1);   // 9
    vertices.Add(origin + zy2);   // 10
    vertices.Add(origin + zy3);   // 11
    vertices.Add(origin + zy4);   // 12

    // === Faces of the Original 5 Triforces ===
    output.Add(new Triangle(vertices[1], vertices[6], vertices[10]));   // 0
    output.Add(new Triangle(vertices[1], vertices[10], vertices[4]));   // 1
    output.Add(new Triangle(vertices[1], vertices[4], vertices[9]));    // 2
    output.Add(new Triangle(vertices[1], vertices[9], vertices[5]));    // 3
    output.Add(new Triangle(vertices[1], vertices[5], vertices[6]));    // 4
    
    output.Add(new Triangle(vertices[3], vertices[7], vertices[11]));   // 5
    output.Add(new Triangle(vertices[3], vertices[11], vertices[2]));   // 6
    output.Add(new Triangle(vertices[3], vertices[2], vertices[12]));   // 7
    output.Add(new Triangle(vertices[3], vertices[12], vertices[8]));   // 8
    output.Add(new Triangle(vertices[3], vertices[8], vertices[7]));    // 9

    output.Add(new Triangle(vertices[10], vertices[7], vertices[4]));   // 10
    output.Add(new Triangle(vertices[4], vertices[7], vertices[8]));    // 11
    output.Add(new Triangle(vertices[4], vertices[8], vertices[9]));    // 12
    output.Add(new Triangle(vertices[9], vertices[8], vertices[12]));   // 13
    output.Add(new Triangle(vertices[9], vertices[12], vertices[5]));   // 14
    output.Add(new Triangle(vertices[5], vertices[12], vertices[2]));   // 15
    output.Add(new Triangle(vertices[5], vertices[2], vertices[6]));    // 16
    output.Add(new Triangle(vertices[6], vertices[2], vertices[11]));   // 17
    output.Add(new Triangle(vertices[11], vertices[10], vertices[6]));  // 18
    output.Add(new Triangle(vertices[10], vertices[11], vertices[7]));  // 19

    // Assign initial neighbors
    output[0].AssignNeighbors(output[1],  output[4], output[18]);
    output[1].AssignNeighbors(output[2],  output[0], output[10]);
    output[2].AssignNeighbors(output[1],  output[12],output[3]);
    output[3].AssignNeighbors(output[2],  output[14],output[4]);
    output[4].AssignNeighbors(output[3],  output[16],output[0]);
    output[5].AssignNeighbors(output[19], output[6], output[9]);
    output[6].AssignNeighbors(output[5],  output[17],output[7]);
    output[7].AssignNeighbors(output[6],  output[15],output[8]);
    output[8].AssignNeighbors(output[7],  output[13],output[9]);
    output[9].AssignNeighbors(output[8],  output[11],output[5]);
    output[10].AssignNeighbors(output[1], output[19],output[11]);
    output[11].AssignNeighbors(output[10],output[9], output[12]);
    output[12].AssignNeighbors(output[11],output[13],output[2]);
    output[13].AssignNeighbors(output[12],output[8], output[14]);
    output[14].AssignNeighbors(output[13],output[15],output[3]);
    output[15].AssignNeighbors(output[14],output[7], output[16]);
    output[16].AssignNeighbors(output[15],output[17],output[4]);
    output[17].AssignNeighbors(output[16],output[6], output[18]);
    output[18].AssignNeighbors(output[17],output[19],output[0]);
    output[19].AssignNeighbors(output[18],output[5], output[10]);

    // --- Number tris ---
    int count = 0;
    foreach (Triangle t in output)
    {
      t.index = count;
      count++;
    }

    return output;
  }

  void RecursiveNeighbors(List<Hexagon> hexes)
  {
    //tests pentagon indices, there shouldn't be any here
    foreach (Hexagon h in hexes)
    {
      if (h.isPentagon)
      Debug.Log(h.index);
    }
    bool[] tilesDefined = new bool[hexes.Count];

    // === Set initial seed hex neighbors ===
    hexes[0].neighbors = GetLeftHexagonInitialNeighborsAtSubdivion(subdivisions);
    tilesDefined[0] = true;

    int r = hexes[0].neighbors[Direction.NegY];
    hexes[r].neighbors = GetRightHexagonInitialNeighborsAtSubdivion(subdivisions);
    tilesDefined[r] = true;
    //Now that we have a single hexagon defined, we can define the rest on the sphere based on it
    RecursiveAssign(hexes, tilesDefined);  //This goes into the list of hexes as many times as it takes to assign directions to all hexes
  }

  void RecursiveAssign(List<Hexagon> hexes, bool[] tilesDefined)
  {
    int it = 0;
    List<int> definedIndices = new List<int>();
    //Figure out the indices of tiles which are already defined, increment "it" if we find any that aren't defined so we go back into the function
    for (int i = 0; i < tilesDefined.Length; i++)
    {
      if (tilesDefined[i])
      {
        definedIndices.Add(i);
      }
      else
      {
        it++;
      }
    }
    //For each tile that has been defined, define its neighbors (if they aren't already defined)
    foreach (int i in definedIndices)
    {
      //So we are working with each defined hexagon, hexes[i]
      //Walk to its neighbors one at a time and assign their neighbors
      if (!hexes[i].isPentagon)
      { 
        for (int w = 0; w < hexes[i].neighbors.Length; w++)
        {
          if(!tilesDefined[hexes[i].neighbors[w]]) //if one of my neighbors isn't defined
          {
            DefineNeighborsFromNeighbors(hexes, hexes[i].neighbors[w], hexes[i].index);
            tilesDefined[hexes[i].neighbors[w]] = true;
          }
        }
      }
    }
    if (it > 0)
    {
      RecursiveAssign(hexes, tilesDefined);
    }
  }
  //Using RecursiveNeighbors instead
  void TraverseAndAssignNeighbors(List<Hexagon> hexes, List<SphereTile> sTiles)
  { 
    bool[] tilesDefined = new bool[hexes.Count];

    // === Set initial seed hex neighbors ===
    hexes[0].neighbors = GetLeftHexagonInitialNeighborsAtSubdivion(subdivisions);
    tilesDefined[0] = true;

    int r = hexes[0].neighbors[Direction.NegY];
    hexes[r].neighbors = GetRightHexagonInitialNeighborsAtSubdivion(subdivisions);
    tilesDefined[r] = true;

    // @TODO: Damien come here
    // My idea is to first define the three rings (this example shows y-ring)
    //  then to do the traversal as we discussed.
    //  I beleive the three rings must be defined first 

    //Damien believes that only one tile needs to be defined, and all subsequent tiles can be done by walking to them

    // === Define remaining neighbors ===
    int previous = 0, direction = Direction.Y;
    int current = hexes[0].neighbors[direction];
    int timer = 100;    // @TODO: increase lol

    while (timer > 0)
    {
      timer --;

      if (!tilesDefined[current])
      {
        DefineNeighborsFromNeighbors(hexes, current, previous);
        tilesDefined[current] = true;
      }
      
      previous = current;
      current = hexes[current].neighbors[direction];
     }
  }

  int HasACompleteNeighbor(List<Hexagon> hexes, int index)
  {
    int found = -1;

    for (int i=0; i<hexes[index].neighbors.Length; i++)
    {
      if (NeighborsDefined(hexes, hexes[index].neighbors[i]))
      {
        found = hexes[index].neighbors[i];
      }
    }

    return found;
  }

  bool NeighborsDefined(List<Hexagon> hexes, int i)
  {
    if (i >= hexes.Count)
    {
      Debug.LogError("index:"+i+" hexes.Count:"+hexes.Count);
      return false;
    }
    if (i==-1)
      return false;
    else if (hexes[i].neighbors.Length > 5)
    {
      if (i==1)
      {

      }
      Debug.Log("Y:" + (hexes[i].neighbors[Direction.Y] != -1) +
            " XY:"+(hexes[i].neighbors[Direction.XY] != -1)+
            " X:"+(hexes[i].neighbors[Direction.X] != -1)+
            " -Y:"+(hexes[i].neighbors[Direction.NegY] != -1)+
            " -XY:"+(hexes[i].neighbors[Direction.NegXY] != -1 )+
            " -X:"+(hexes[i].neighbors[Direction.NegX] != -1));

      return hexes[i].neighbors[Direction.Y] != -1 &&
            hexes[i].neighbors[Direction.XY] != -1 &&
            hexes[i].neighbors[Direction.X] != -1 &&
            hexes[i].neighbors[Direction.NegY] != -1 &&
            hexes[i].neighbors[Direction.NegXY] != -1 &&
            hexes[i].neighbors[Direction.NegX] != -1;
    }
    else
    {
      return hexes[i].neighbors[Direction.Y] != -1 &&
            hexes[i].neighbors[Direction.XY] != -1 &&
            hexes[i].neighbors[Direction.X] != -1 &&
            hexes[i].neighbors[Direction.NegY] != -1 &&
            hexes[i].neighbors[Direction.NegX] != -1;
    }
  }

  void DefineNeighborsFromNeighbors(List<Hexagon> hexes, int i, int knownDefined = -1)
  {
    
    // Here we assume there are two adjacent tiles, i and knownDefined
    if (i==-1)
    {
      Debug.LogError("Attempting to define a null index");
      return;
    }

    if (knownDefined == -1)
    {
      knownDefined = i-1;
    }
    
    List<SphereTile> potentialNeighbors;

    try
    {
      potentialNeighbors = sTiles[i].neighborDict.Values.ToList();
    }
    catch (System.Exception e)
    {
      Debug.LogError(" index:"+i+" sTiles.Count:"+sTiles.Count);
      return;
    }

    for (int dir = 0; dir<hexes[i].neighbors.Length; dir++)
    {
      //Debug.Log(i+": Defining "+Direction.ToString(dir)+"-neighbor with reference "+knownDefined+"->"+hexes[knownDefined].neighbors[dir]);

      if (hexes[i].neighbors[dir] != -1)
      {
        continue;
      }

      Vector3 direction = hexes[knownDefined].center -
        hexes[ hexes[knownDefined].neighbors[dir] ].center;

      int n = FindNeighbor(hexes[i].center, direction, potentialNeighbors);
      hexes[i].neighbors[dir] = n;
    } 
  }

  int FindNeighbor(Vector3 center, Vector3 direction, List<SphereTile> potentialNeighbors)
  {
    float winningAngle = 190, angle=9999;
    int winningNeighborIndex = -1;

    for (int i=0; i<potentialNeighbors.Count; i++)
    {
      angle = Vector3.Angle(center-potentialNeighbors[i].center, direction);

      if (angle < winningAngle)
      {
        winningAngle = angle;
        winningNeighborIndex = i;
      }
    }

    return potentialNeighbors[winningNeighborIndex].index;
  }

  public static int[] GetLeftHexagonInitialNeighborsAtSubdivion(int subdivisionLevel)
  {
    int[] output = new int[6];

    switch (subdivisionLevel)
    {
                      // Y, XY, X, -Y, -XY -X
      case 1:
        output[Direction.Y]     = 1;
        output[Direction.XY]    = 2;
        output[Direction.X]     = 3;
        output[Direction.NegY]  = 13;
        output[Direction.NegXY] = 15;
        output[Direction.NegX]  = 4;
        return output;
      case 2:
        return new int[]{1,2,3,9,10,4};
      default:
        return new int[6];
    }
  }

  public static int[] GetRightHexagonInitialNeighborsAtSubdivion(int subdivisionLevel)
  {
    switch (subdivisionLevel)
    {
                      // Y, XY, X, -Y, -XY -X
      case 1:
        return new int[]{0,3,10,12,14,15};
      case 2:
        return new int[]{0,3,46,50,11,10};
      default:
        return new int[6];
    }
  }
}
