using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
//public enum Targetting{Single, AoE, Life}
public abstract class RuneHex{ // : Rune{
	public List<int> castedTiles;
	public float amplitude;
	//public int generations;
	public float castTime;
	public float cooldown;
	public int manaCost;
	public GameObject effect;

	public abstract void Initialize();
	public abstract void Cast(int origin);

}

public abstract class SingleTargetHex : RuneHex
{
	public abstract override void Initialize();
	public abstract override void Cast(int origin);
}

public abstract class AreaTargetHex : RuneHex
{
	public abstract override void Initialize();
	public abstract override void Cast(int origin);
}

public abstract class LifeHex : RuneHex
{
	public int generations;
	public abstract override void Initialize();
	public abstract override void Cast(int origin);
}

public class WaterAttackI : SingleTargetHex{
	public override void Initialize()
	{
		effect = (GameObject)Resources.Load("Effects/Water/Worb");
		castedTiles = new List<int>();
		amplitude = 1;
		castTime = 0.2f;
		cooldown = 1;
		manaCost = 1;
	}
	public override void Cast(int origin)
	{
		castedTiles.Add(origin);
		Vector3 tileVec = CombatManager.activeWorld.tiles[origin].hexagon.center;
		Vector3 tileVecNorm = tileVec.normalized;
		Vector3 startPos = tileVec + tileVecNorm; //above the tile by 1 unit 
		Vector3 startScale = new Vector3(3f,3f,3f);
		effect.transform.localScale = startScale;
		GameObject.Instantiate(effect, startPos, Quaternion.identity);
	}
}
