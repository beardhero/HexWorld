/*
 * Copyright (c) 2015 Colin James Currie.
 * All rights reserved.
 * Contact: cj@cjcurrie.net
 */

 // @INFO: This script is responsible for rendering zone data and performing simulation at the zone level

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ZoneManager : MonoBehaviour
{
  // === Public ===
  public TileSet regularTileSet;
	
  // === Private ===
  Transform boardHolder;
  List<Vector2> gridPositions;
  List<Vector2> topTilePositions;

  // === Cache ===
  //LayerMask layermask;

  public void Initialize (Zone z)
  {
    //layermask = 1<<8;   // Layer 8 is set up as "Chunk" in the Tags & Layers manager
    //GameManager.zoneRenderer.RenderZone(z, regularTileSet);
    CapturePNG();
  }
  public void CapturePNG()
  {
    GameObject selection = GameObject.Find ("Zone Prefab(Clone)");
    Camera cam = Camera.main;
        int width = 128;
        int height = 128;
        Texture2D tex = new Texture2D (width, height, TextureFormat.ARGB32, false);
        Rect sel = new Rect ();
        sel.width = width;
        sel.height = height;
        sel.x = 1.618f;
        sel.y = 1.1265f;//selection.transform.position.y;

        tex.ReadPixels (sel, 0, 0);
 
        byte[] bytes = tex.EncodeToPNG ();
        File.WriteAllBytes ("Assets/Test/picture.png", bytes);
  }
  public void OnTapInput(Vector2 tap)
  {
    
  }
}