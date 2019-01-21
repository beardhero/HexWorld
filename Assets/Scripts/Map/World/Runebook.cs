using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Runebook {

	public byte[] ID;
	[HideInInspector]public List<Rune> runes;
	public TileSet tileset;
	//public GameObject runebookgo;
	public Runebook(){}
	
	public Runebook(byte[] id)
	{
		ID = id;
		//Make all the runes
		runes = new List<Rune>();
		for (int i = 0; i < 32; i++)
		{
			UnityEngine.Random.InitState(ID[i]);
			byte[] newRuneID = new byte[32];
			for(int a = 0; a < 32; a++)
			{
				newRuneID[i] = (byte)UnityEngine.Random.Range(0,256);
			}
			runes.Add(new Rune(newRuneID));
		}
	}
	
	public List<GameObject> RunesGO()
	{
		List<GameObject> l = new List<GameObject>();
		foreach(Rune r in runes)
		{
			l.Add(r.RuneGO());
		}
		return l;
	}
	public GameObject RunebookGO()
	{
		Zone zone = new Zone(7);
		int i = 0;
		for (int x = 0; x < 7; x++)
		{
			for (int y = 0; y < 7; y++)
			{
				if(zone.tiles[x,y].type != TileType.None)
				{
					if(i < 32)
					{
						zone.tiles[x,y] = runes[i].tile;
						i++;
					}
				}
			}
		}
		GameObject rr = GameObject.Find("RuneRenderer");
		ZoneRenderer zr = rr.GetComponent<ZoneRenderer>();
		ZoneManager zm = rr.GetComponent<ZoneManager>();
		//GameObject output = zr.RenderZone(zone, zm.regularTileSet)[0];
		return zr.RenderZone(zone, zm.regularTileSet)[0];
	}
}
