/*
 * Copyright (c) 2015 Colin James Currie.
 * All rights reserved.
 * Contact: cj@cjcurrie.net
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Hex
{
  private static float width, halfWidth, height, rowHeight;

  public const float edge = .25f;
  public static float root3, halfSide, bisect, doubleHeight, sideAndAHalf, gridHeight, gridWidth;

  public static void Initialize()
  {
    height = 2 * edge;
    rowHeight = 1.5f * edge;
    halfWidth = (float)Mathf.Sqrt((edge * edge) - ((edge / 2) * (edge / 2)));
    width = 2 * halfWidth;

    root3 = Mathf.Sqrt(3);
    halfSide = edge/2;
    bisect = root3 * edge / 2;
    doubleHeight = bisect*2;
    sideAndAHalf = halfSide * 3;

    gridHeight = 8 * edge / 3;
    gridWidth = bisect * 2;
  }

  public static Vector3 TileOrigin(IntCoord tileCoordinate)
  {
    return new Vector3(
              (tileCoordinate.x * width) + ((tileCoordinate.y % 2 == 1) ? halfWidth : 0),
           //   GameManager.currentZone.tiles[tileCoordinate.x,tileCoordinate.y].height,
              tileCoordinate.y * rowHeight);
  }

  public static Vector3 TileCenter(IntCoord tileCoordinate)
  {
    return TileOrigin(tileCoordinate) + new Vector3(halfWidth, 0, height/2);
  }

  public static int RotateDirection(int direction, int amount)
  {
    //Let's make sure our directions stay within the enumerated values.
    if (direction < Direction.XY ||
        direction > Direction.Y ||
        Mathf.Abs(amount) > (int)Direction.Y)
    {
        throw new InvalidOperationException("Directions out of range.");
    }
   direction += amount;
   //Now we need to make sure direction stays within the proper range.
   //C# does not allow modulus operations on enums, so we have to convert to and from int.

   int n_dir = direction % Direction.Count;

   if (n_dir < 0) n_dir = Direction.Count + n_dir;
       direction = n_dir;

   return direction;
  }

  public static int Opposite(int direction)
  {
    return RotateDirection(direction, 3);
  }

  public static Vector2 Neighbor(Vector2 tile, int direction)
  {
    if (tile.y % 2 == 0) //Is this row even?
    {
      switch(direction)
      {
        case Direction.XY : tile.y += 1; break;
        case Direction.X : tile.x += 1; break;
        case Direction.NegY: tile.y -= 1; break;
        case Direction.NegXY: tile.y -= 1; tile.x -= 1; break;
        case Direction.NegX: tile.x -= 1; break;
        case Direction.Y: tile.x -= 1; tile.y += 1; break;
        default: throw new InvalidOperationException("Invalid direction");
      }
    }
    else //This is an odd row.
    {
      switch (direction)
      {
        case Direction.XY: tile.x += 1;  tile.y += 1; break;
        case Direction.X: tile.x += 1; break;
        case Direction.NegY: tile.x += 1; tile.y -= 1; break;
        case Direction.NegXY: tile.y -= 1;; break;
        case Direction.NegX: tile.x -= 1; break;
        case Direction.Y: tile.y += 1; break;
        default: throw new InvalidOperationException("Invalid direction");
      }
    }

    return tile;
  }

  public static Vector2 TileAt(Vector3 worldCoordinate)
  {
    float rise = height - rowHeight;
    float slope = rise / halfWidth;

    int X = (int)Math.Floor(worldCoordinate.x / width);
    int Y = (int)Math.Floor(worldCoordinate.y / rowHeight);

    Vector2 offset = new Vector2(worldCoordinate.x - X * width, worldCoordinate.y - Y * rowHeight);

    if (Y % 2 == 0) //Is this an even row?
    {
      //Section type A
      //Point is below left line; inside SouthWest neighbor
      if (offset.y < (-slope * offset.x + rise))
      {
        X -= 1;
        Y -= 1;
      }
      //Point is below right line; inside SouthEast neighbor
      else if (offset.y < (slope * offset.x - rise))
      {
        Y -= 1;
      }
    }
    else
    {
      //Section type B
      if (offset.x >= halfWidth) //Is the point on the right side?
      {
          if (offset.y < (-slope * offset.x + rise * 2.0f))
        //Point is below bottom line; inside SouthWest neighbor.
              Y -= 1;
      }
      else //Point is on the left side
      {
        if (offset.x < (slope * offset.y))
          //Point is below the bottom line; inside SouthWest neighbor.
          Y -= 1;
        else //Point is above the bottom line; inside West neighbor.
          X -= 1;
      }
    }

    return new Vector2(X, Y);
  }
}