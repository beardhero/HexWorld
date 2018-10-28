using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
			Random.InitState(ID[i]);
			byte[] newRuneID = new byte[32];
			for(int a = 0; a < 32; a++)
			{
				newRuneID[i] = (byte)Random.Range(0,256);
			}
			runes.Add(new Rune(newRuneID));
		}
	}
	
	public List<GameObject> RunebookGO()
	{
		List<GameObject> l = new List<GameObject>();
		foreach(Rune r in runes)
		{
			l.Add(r.RuneGO());
		}
		return l;
	}
}
