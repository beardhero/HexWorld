using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Zone {

  public Tile[,] tiles;
  public List<Tile[,]> continents;
  public int width;
  public Triangle triangle;
  public float root3 = Mathf.Sqrt(3);
  public float waterHeight;
  public int landArea;
  public static float sideLength;
  public List<Tile> tilesList;
  //SimplexNoise simplex;

  //ZoneRelationship[] neighbors;

  public Zone(){}
  
  public Zone(Triangle tri)  //equilateral triangle zone
  {
    /*
    triangle = tri;
    Vector3 p = tri.v1 - tri.v3;
    sideLength = p.magnitude;
    width = (int)sideLength;
    */
    width = 7;
    //waterHeight = .5f;
    tiles = new Tile[width, width];
    //simplex = new SimplexNoise(GameManager.gameSeed);
    // 1st pass: random seed noise in Perlin
    
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < width; y++)
      {
        // Perlin noise tiles
        //tiles[x, y] = new Tile(x, y, width, .1f, .5f, startingHeight);
        
        // All gray same-height method
        tiles[x,y] = new Tile(1f);
      }
    }
    AnalyzeTiles();
    //Cut();
  }
  
  public Zone(int w)
  {
    width = w;
    waterHeight = .5f;
    tiles = new Tile[width, width];
    //simplex = new SimplexNoise(GameManager.gameSeed);

    //int randX = Random.Range(-99999,99999);
    //int randY = Random.Range(-99999,99999);
    
    float maxHeight = 10;   // Should be an int
    float startingHeight = Random.Range(maxHeight*.1f,maxHeight*.5f);

    // 1st pass: random seed noise in Perlin

    for (int x=0; x<width; x++)
    {
      for (int y=0; y<width; y++)
      {
        // Perlin noise tiles
        tiles[x,y] = new Tile();//x,y,width, .1f, .5f, startingHeight);

        // All Grass same-height method
        //tiles[x,y] = new Tile(startingHeight);
      }
    }

    //RandomWaterHeight(.2f, .4f);
    /* 
    // 2nd pass: Spread ground
    SpreadGround(4, TileType.Earth);

    //3rd: Refine ground
    RefineGround();

    //4th SetHeights
    AddPerlinHeight(maxHeight*2, 1);
    AddPerlinHeight(maxHeight*.5f, 4);
    AddPerlinHeight(maxHeight*.25f, 8);
    */
//    SetTypeByHeight(maxHeight);

    //AnalyzeTiles();
    if(width == 7)
    {
      CutIntoRunebook();
    }
  }

  void RandomWaterHeight(float dryLandWeight)
  {
    waterHeight = Random.Range(-dryLandWeight,4.3f);
  }

  void AnalyzeTiles()
  {
    landArea = 0;

    for (int x=0; x<width; x++)
    {
      for (int y=0; y<width; y++)
      {
        if (tiles[x,y].height< waterHeight)
          tiles[x,y].type = TileType.Water;
        else
        {
          switch (tiles[x,y].type)
          {
            case TileType.Water:

            break;
            default:
              landArea++;
            break;
          }
        }
      }
    }
  }
  void CutIntoRunebook() //width 7
  {
    tilesList = new List<Tile>();
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < width; y++)
      {
        if ((y == 0 && (x <= 1 || x > 5)) || (y == 1 && (x == 0 || x == 6)) || (y == 2 && (x == 0)) || (y == 4 && (x == 0)) || (y == 5 && (x == 0 || x == 6)) || (y==6) && (x <= 1 || x > 5) )
        {
          tiles[x, y].type = TileType.None;
        }
        else
        {
          tilesList.Add(tiles[x,y]);
        }
      }
    }
  }
	/*
  void SetTypeByHeight(float maxHeight)
  {
    int rx = Random.Range(-100,100), ry = Random.Range(-100,100);
    float perlinScale = 10;

    for (int x=0; x<width; x++)
    {
      for (int y=0; y<width; y++)
      {
        if (tiles[x,y].type == TileType.None)
          continue;

        float xs = (float)x/width*5, ys = (float)y/width*5;
        float perlin = Mathf.PerlinNoise((xs+rx)*perlinScale,(ys+ry)*perlinScale);
        float height = tiles[x,y].height;
        float sum = height;

        if (sum < waterHeight)
        {
          if (sum < maxHeight*.1f)
            tiles[x,y].type = TileType.DeepWater;
          else
            tiles[x,y].type = TileType.Water;
        }
        else if (sum > maxHeight*.8f)
          tiles[x,y].type = TileType.Snow;
        else if (sum > maxHeight * .6f)
        {
          if (sum > maxHeight*.75f)
          {
            if (perlin > .5f)
              tiles[x,y].type = TileType.MossyRoad;
            else
              tiles[x,y].type = TileType.Road;
          }
          else
            tiles[x,y].type = TileType.Stone;
        }
        else if (sum > maxHeight*.5f)
        {
          if (perlin < .2f)
            tiles[x,y].type = TileType.Mud;
          else
            tiles[x,y].type = TileType.Grass;
        }
        else if (sum > maxHeight*.3f)
          tiles[x,y].type = TileType.Dirt;
        else if (sum > maxHeight*.2f)
          tiles[x,y].type = TileType.Mud;
        else if (sum > maxHeight*.15f)
        {
          if (perlin > .8f)
            tiles[x,y].type = TileType.PinkSand;
          else
            tiles[x,y].type = TileType.Sand;
        }
      }
    }
    */

  public void SimulateLife(){

  }

  void AddPerlinHeight(float scale, int lacunarity)
  {
    float seedx = Random.Range(-1.0f,1.0f);
    float seedy = Random.Range(-1.0f,1.0f);
    float num = Random.Range(1.5f, 3f);
    float den = Random.Range(2f, 3f);
    float wi = width;

    for (int x=0; x<wi; x++)
    {
      for (int y=0; y<wi; y++)
      {
        if (tiles[x,y].type == TileType.None)
          continue;

        float height = Mathf.PerlinNoise( (x/wi+seedx)*lacunarity, (y/wi+seedy)*lacunarity);
        height -= .5f;    // So that subtractions can happen

        tiles[x,y].height += (int)(height * scale * num) / den;
      }
    }
  }
  /*
  void AddSimplexHeight(float scale, int lacunarity)
  {
    float seedx = Random.Range(-1.0f,1.0f);
    float seedy = Random.Range(-1.0f,1.0f);
    float wi = width;

    for (int x=0; x<wi; x++)
    {
      for (int y=0; y<wi; y++)
      {
        if (tiles[x,y].type == TileType.None)
          continue;

        float fractal = simplex.coherentNoise(x, y,
          2, 5, 2, 4);
        //Debug.Log(fractal);
      }
    }
  }
  */
  public void SpreadGround(int iterations, TileType groundType)
  { 
    Tile[,] oldTiles = new Tile[width,width];
    for(int i = 1; i < iterations; i++)  //Increase i bounds to iterate the grid more times
    {
      for (int x=0; x<width; x++)
      {
        for (int y=0; y<width; y++)
        {
          oldTiles[x,y] = tiles[x,y];
        }
      }

      for (int x=0; x<width; x++)
      {
        for (int y=0; y<width; y++)
        {
          // count neighbors
          int neighborGroundCount = 0;
          int neighborEmptyCount = 0;
          for (int xNeighb=-1; xNeighb<=1; xNeighb++)
          {
            for (int yNeighb=-1; yNeighb<=1; yNeighb++)
            {
              if ((x+xNeighb < 0 || x+xNeighb >= width || y+yNeighb < 0 || y+yNeighb >= width) )
                continue;     

              if (oldTiles[x+xNeighb,y+yNeighb].type != TileType.None)
              {
                neighborGroundCount++;
              }

              if (oldTiles[x + xNeighb, y + yNeighb].type == TileType.None)
              {
                  neighborEmptyCount++;
              }
            }
          }
      
          //How about this: if your neighbor grass count is higher than neighbor empty count, become a grass  
          // If we have >5 neighbors, become wall
          if (oldTiles[x,y].type == TileType.Earth || neighborGroundCount >= 5)
          {
              tiles[x,y].type = groundType;
          }

          if(neighborGroundCount <= 2)
          {
              tiles[x,y].type = TileType.None;
          }
       }
     }
    }
   }
    public void RefineGround()
    {
        for (int i = 1; i < 6; i++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    int neighborGroundCount = 0;
                    int neighborEmptyCount = 0;
                    for (int xNeighb = -1; xNeighb <= 1; xNeighb++)
                    {
                        for (int yNeighb = -1; yNeighb <= 1; yNeighb++)
                        {
                            if ((x + xNeighb < 0 || x + xNeighb > width - 1 || y + yNeighb < 0 || y + yNeighb > width - 1) || (yNeighb == 1 && xNeighb == 1) || (yNeighb == -1 && xNeighb == -1))
                                continue;
                            if (tiles[x + xNeighb, y + yNeighb].type == TileType.None)
                                neighborEmptyCount++;
                            if (tiles[x + xNeighb, y + yNeighb].type != TileType.None)
                                neighborGroundCount++;
                        }
                    }
                    if (neighborGroundCount < neighborEmptyCount)
                    {
                        tiles[x, y].type = TileType.None;
                    }
                }
            }
        }

        //Now take out tiles with one or less ground neighbor.        
        int neighborCount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                neighborCount = 0;
                for (int xNeighb = -1; xNeighb <= 1; xNeighb++)
                {
                    for (int yNeighb = -1; yNeighb <= 1; yNeighb++)
                    {
                        if (y % 2 == 0) //even
                        {
                            if ((yNeighb == 1 && xNeighb == 1) || (yNeighb == -1 && xNeighb == 1))
                            {
                                continue;
                            }
                        }
                        else //odd
                        {
                            if ((yNeighb == 1 && xNeighb == -1) || (yNeighb == -1 && xNeighb == -1))
                            {
                                continue;
                            }
                        }
                        if ((x + xNeighb < 0 || x + xNeighb > width - 1 || y + yNeighb < 0 || y + yNeighb > width - 1))
                        {
                            //tiles[x, y].type = TileType.None;
                            continue;
                        }
                        if (tiles[x + xNeighb, y + yNeighb].type != TileType.None)
                            neighborCount++;
                    }
                }
                if (neighborCount <= 1)
                {
                    tiles[x, y].type = TileType.None;
                }
            }
        }
    }
    
    public void SetBorders()
    {
        int neighborCount = 0;
 
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                // count empty neighbors (6)
              Vector2 v = new Vector2(x, y);
                neighborCount = 0;
                Vector2 e = Hex.Neighbor(v, Direction.X),
                        ne = Hex.Neighbor(v, Direction.XY),
                        se = Hex.Neighbor(v, Direction.NegY),
                        w = Hex.Neighbor(v, Direction.NegX),
                        nw = Hex.Neighbor(v, Direction.Y),
                        sw = Hex.Neighbor(v, Direction.NegXY); 

                if (tiles[(int)e.x, (int)e.y].type == TileType.None || tiles[(int)ne.x, (int)ne.y].type == TileType.None ||
                    tiles[(int)se.x, (int)se.y].type == TileType.None || tiles[(int)w.x, (int)w.y].type == TileType.None ||
                    tiles[(int)nw.x, (int)nw.y].type == TileType.None || tiles[(int)sw.x, (int)sw.y].type == TileType.None )
                {
                    neighborCount++;
                }
                if (neighborCount > 0)
                {
                    tiles[x, y].border = true;
                //    tiles[x, y].type = TileType.Abyss;
                }
            }
        }
                          
    }
    //needs to make the continents between borders and put each land segment into its own array, in order to compare size later and keep the biggest one or few
    public void FillBorders()
    {
        //Get an empty tile array which we will populate the next array in continents with
        //Tile[,] tilesToSave = new Tile[width, width];
       
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                if (tiles[x, y].posBorderCheck)
                    continue;
                if (tiles[x, y].border) //we're only going to do this for borders we haven't done this for yet
                {
                    RecursiveFill(x,y);
                }
            }
        }
    }

    bool walk = false;
    public void RecursiveFill(int x, int y)
    {
      walk = false;
      while (walk)
      {

      }
      for (int xNeighb = -1; xNeighb <= 1; xNeighb++)
      {
          for (int yNeighb = -1; yNeighb <= 1; yNeighb++)
          {
              if ((x + xNeighb < 0 || x + xNeighb > width - 1 || y + yNeighb < 0 || y + yNeighb > width - 1))
                  continue;
              if (y % 2 == 0) //even
              {
                  if ((yNeighb == 1 && xNeighb == 1) || (yNeighb == -1 && xNeighb == 1))
                  {
                      continue;
                  }
              }
              else //odd
              {
                  if ((yNeighb == 1 && xNeighb == -1) || (yNeighb == -1 && xNeighb == -1))
                  {
                      continue;
                  }
              }
          }
      }

      // count empty neighbors (6)
      int neighborCount = 0;

      for (int xNeighb = -1; xNeighb <= 1; xNeighb++)
      {
        for (int yNeighb = -1; yNeighb <= 1; yNeighb++)
        {
          if ((x+xNeighb < 0 || x+xNeighb > width - 1 || y + yNeighb < 0 || y + yNeighb > width - 1))
              continue;
          
          if (tiles[x+xNeighb, y+yNeighb].type == TileType.None)
          {
            neighborCount++;
          }
          if (tiles[x + xNeighb, y + yNeighb].border == true && !(tiles[x + xNeighb, y + yNeighb].type == TileType.None))
          {
            
          }
        }   
      }
      if (neighborCount > 0)
      {
        tiles[x, y].border = true;
      }
    }

  //Making this a seperate function so we can add complexity more easily
  float Height(float x, float y)
  {
    return Mathf.PerlinNoise(x,y);
  }
}

public class ZoneRelationship
{
  Zone other;
  Vector3 otherDirection;   // This is used to calculate apprx. where the door to the other zone should be placed
}