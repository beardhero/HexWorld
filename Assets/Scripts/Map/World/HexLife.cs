using UnityEngine;
using System.Collections;
using System;

[Serializable]
public enum Ruleset {
	None, HexagonalDuality
};

public class HexLife{ //cyclical hex life
		/* 
		foreach (HexTile ht in GetComponentInParent<WorldManager>().activeWorld.tiles)
		{
			ht.ChangeType(TileType.Luna);
		} 
		*/
}

public class Ruleset_HexagonalDuality : HexLife
{

}
