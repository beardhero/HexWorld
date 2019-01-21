using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class HexTile
{
  public int index;
  public int plate = -1;
  public int generation = 0;
  public float height = 1;
  string terrainType;
  public Hexagon hexagon;
  public TileType type = TileType.Gray;
  public TileType typeToSet;
  public int objectToPlace = -1;
  public List<int> neighbors;
  public bool boundary;
  public bool passable = true;
  public bool oceanTile = false;
  public bool[] rules;
  public bool flip;
  public bool placeObject;
  public int antPasses = 0;
  public int state;
	//Each tile has it's own rule set, rule changes turn these bools on and off accordingly
	public bool os1, os2, os3, os4, os5, os6, ob1, ob2, ob3, ob4, ob5, ob6,
	ws1, ws2, ws3, ws4, ws5, ws6, wb1, wb2, wb3, wb4, wb5, wb6,
	fs1, fs2, fs3, fs4, fs5, fs6, fb1, fb2, fb3, fb4, fb5, fb6, 
	es1, es2, es3, es4, es5, es6, eb1, eb2, eb3, eb4, eb5, eb6,
	ss1, ss2, ss3, ss4, ss5, ss6, sb1, sb2, sb3, sb4, sb5, sb6,
	as1, as2, as3, as4, as5, as6, ab1, ab2, ab3, ab4, ab5, ab6,
	ns1, ns2, ns3, ns4, ns5, ns6, nb1, nb2, nb3, nb4, nb5, nb6;

	public void SetRule(bool[] b){
		//bool[84]
		os1 = os2 = os3 = os4 = os5 = os6 = ob1 = ob2 = ob3 = ob4 = ob5 = ob6 =
			ws1 = ws2 = ws3 = ws4 = ws5 = ws6 = wb1 = wb2 = wb3 = wb4 = wb5 = wb6 =
				fs1 = fs2 = fs3 = fs4 = fs5 = fs6 = fb1 = fb2 = fb3 = fb4 = fb5 = fb6 =
					es1 = es2 = es3 = es4 = es5 = es6 = eb1 = eb2 = eb3 = eb4 = eb5 = eb6 =
						ss1 = ss2 = ss3 = ss4 = ss5 = ss6 = sb1 = sb2 = sb3 = sb4 = sb5 = sb6 =
							as1 = as2 = as3 = as4 = as5 = as6 = ab1 = ab2 = ab3 = ab4 = ab5 = ab6 =
								ns1 = ns2 = ns3 = ns4 = ns5 = ns6 = nb1 = nb2 = nb3 = nb4 = nb5 = nb6 = false;
		if (b [0])
			os1 = true;
		if (b [1])
			os2 = true;
		if (b [2])
			os3 = true;
		if (b [3])
			os4 = true;
		if (b [4])
			os5 = true;
		if (b [5])
			os6 = true;
		if (b [6])
			ob1 = true;
		if (b [7])
			ob2 = true;
		if (b [8])
			ob3 = true;
		if (b [9])
			ob4 = true;
		if (b [10])
			ob5 = true;
		if (b [11])
			ob6 = true;
		if (b [12])
			ws1 = true;
		if (b [13])
			ws2 = true;
		if (b [14])
			ws3 = true;
		if (b [15])
			ws4 = true;
		if (b [16])
			ws5 = true;
		if (b [17])
			ws6 = true;
		if (b [18])
			wb1 = true;
		if (b [19])
			wb2 = true;
		if (b [20])
			wb3 = true;
		if (b [21])
			wb4 = true;
		if (b [22])
			wb5 = true;
		if (b [23])
			wb6 = true;
		if (b [24])
			fs1 = true;
		if (b [25])
			fs2 = true;
		if (b [26])
			fs3 = true;
		if (b [27])
			fs4 = true;
		if (b [28])
			fs5 = true;
		if (b [29])
			fs6 = true;
		if (b [30])
			fb1 = true;
		if (b [31])
			fb2 = true;
		if (b [32])
			fb3 = true;
		if (b [33])
			fb4 = true;
		if (b [34])
			fb5= true;
		if (b [35])
			fb6 = true;
		if (b [36])
			es1 = true;
		if (b [37])
			es2 = true;
		if (b [38])
			es3 = true;
		if (b [39])
			es4 = true;
		if (b [40])
			es5 = true;
		if (b [41])
			es6 = true;
		if (b [42])
			eb1 = true;
		if (b [43])
			eb2 = true;
		if (b [44])
			eb3 = true;
		if (b [45])
			eb4 = true;
		if (b [46])
			eb5 = true;
		if (b [47])
			eb6 = true;
		if (b [48])
			ss1 = true;
		if (b [49])
			ss2 = true;
		if (b [50])
			ss3 = true;
		if (b [51])
			ss4 = true;
		if (b [52])
			ss5 = true;
		if (b [53])
			ss6 = true;
		if (b [54])
			sb1 = true;
		if (b [55])
			sb2 = true;
		if (b [56])
			sb3 = true;
		if (b [57])
			sb4 = true;
		if (b [58])
			sb5 = true;
		if (b [59])
			sb6 = true;
		if (b [60])
			as1 = true;
		if (b [61])
			as2 = true;
		if (b [62])
			as3 = true;
		if (b [63])
			as4 = true;
		if (b [64])
			as5 = true;
		if (b [65])
			as6 = true;
		if (b [66])
			ab1 = true;
		if (b [67])
			ab2 = true;
		if (b [68])
			ab3 = true;
		if (b [69])
			ab4 = true;
		if (b [70])
			ab5 = true;
		if (b [71])
			ab6 = true;
		if (b [72])
			ns1 = true;
		if (b [73])
			ns2 = true;
		if (b [74])
			ns3 = true;
		if (b [75])
			ns4 = true;
		if (b [76])
			ns5 = true;
		if (b [77])
			ns6 = true;
		if (b [78])
			nb1 = true;
		if (b [79])
			nb2 = true;
		if (b [80])
			nb3 = true;
		if (b [81])
			nb4 = true;
		if (b [82])
			nb5 = true;
		if (b [83])
			nb6 = true;
	}

  //Tectonics
  public float pressure, shear, scale, temp, humidity;

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
  public HexTile() { }

  public HexTile(Hexagon h)
  {
    index = h.index;
    hexagon = h;
  }
  public HexTile(Hexagon h, int p, List<int> neighbs, bool b, float hi, TileType t)
  {
    type = t;
    index = h.index;
    hexagon = h;
    plate = p;
    boundary = b;
    neighbors = new List<int>(neighbs);
    height = hi;
  }
  public void ChangeType(TileType t)
  {
    type = t;
	state = (int)t;
	GameObject aW = GameObject.Find ("World");
	GameObject gOWM = GameObject.Find ("WorldManager");
	WorldManager wM = gOWM.GetComponent<WorldManager> ();
	//World aWorld = wM.activeWorld;
	MeshCollider[] meshC = aW.transform.GetComponentsInChildren<MeshCollider>();
	Mesh mesh = meshC [plate].sharedMesh;
	IntCoord newCoord = wM.regularTileSet.GetUVForType(t);
	newCoord.y = generation;
	Vector2 newOffset = new Vector2((newCoord.x * WorldRenderer.uvTileWidth), (newCoord.y * WorldRenderer.uvTileHeight));
	Vector2[] uvs = mesh.uv;
	try{
	uvs [hexagon.uv0i] = WorldRenderer.uv0 + newOffset;
	uvs [hexagon.uv1i] = WorldRenderer.uv1 + newOffset;
	uvs [hexagon.uv2i] = WorldRenderer.uv2 + newOffset;
	uvs [hexagon.uv3i] = WorldRenderer.uv3 + newOffset;
	uvs [hexagon.uv4i] = WorldRenderer.uv4 + newOffset;
	uvs [hexagon.uv5i] = WorldRenderer.uv5 + newOffset;
	uvs [hexagon.uv6i] = WorldRenderer.uv6 + newOffset;
	mesh.uv = uvs;
	}
	catch(Exception e){Debug.Log(" bad tile: " + index + " uv0: " + hexagon.uv0i + " error: " + e);}
  }

  public void MoveHighlight()
  {
	GameObject aW = GameObject.Find ("World");
	GameObject gOWM = GameObject.Find ("WorldManager");
	WorldManager wM = gOWM.GetComponent<WorldManager> ();
	//World aWorld = wM.activeWorld;
	MeshCollider[] meshC = aW.transform.GetComponentsInChildren<MeshCollider>();
	Mesh mesh = meshC [plate].sharedMesh;
	IntCoord newCoord = wM.regularTileSet.GetUVForType(type);
	newCoord.y += 21; //highlight
	//newCoord.y = generation;
	Vector2 newOffset = new Vector2((newCoord.x * WorldRenderer.uvTileWidth), (newCoord.y * WorldRenderer.uvTileHeight));
	Vector2[] uvs = mesh.uv;
	try{
	uvs [hexagon.uv0i] = WorldRenderer.uv0 + newOffset;
	uvs [hexagon.uv1i] = WorldRenderer.uv1 + newOffset;
	uvs [hexagon.uv2i] = WorldRenderer.uv2 + newOffset;
	uvs [hexagon.uv3i] = WorldRenderer.uv3 + newOffset;
	uvs [hexagon.uv4i] = WorldRenderer.uv4 + newOffset;
	uvs [hexagon.uv5i] = WorldRenderer.uv5 + newOffset;
	uvs [hexagon.uv6i] = WorldRenderer.uv6 + newOffset;
	mesh.uv = uvs;
	}
	catch(Exception e){Debug.Log(" bad tile: " + index + " uv0: " + hexagon.uv0i + " error: " + e);}
  }
  public void MoveUnhighlight()
  {
	GameObject aW = GameObject.Find ("World");
	GameObject gOWM = GameObject.Find ("WorldManager");
	WorldManager wM = gOWM.GetComponent<WorldManager> ();
	//World aWorld = wM.activeWorld;
	MeshCollider[] meshC = aW.transform.GetComponentsInChildren<MeshCollider>();
	Mesh mesh = meshC [plate].sharedMesh;
	IntCoord newCoord = wM.regularTileSet.GetUVForType(type);
	newCoord.y -= 21; //unhighlight
	//newCoord.y = generation;
	Vector2 newOffset = new Vector2((newCoord.x * WorldRenderer.uvTileWidth), (newCoord.y * WorldRenderer.uvTileHeight));
	Vector2[] uvs = mesh.uv;
	try{
	uvs [hexagon.uv0i] = WorldRenderer.uv0 + newOffset;
	uvs [hexagon.uv1i] = WorldRenderer.uv1 + newOffset;
	uvs [hexagon.uv2i] = WorldRenderer.uv2 + newOffset;
	uvs [hexagon.uv3i] = WorldRenderer.uv3 + newOffset;
	uvs [hexagon.uv4i] = WorldRenderer.uv4 + newOffset;
	uvs [hexagon.uv5i] = WorldRenderer.uv5 + newOffset;
	uvs [hexagon.uv6i] = WorldRenderer.uv6 + newOffset;
	mesh.uv = uvs;
	}
	catch(Exception e){Debug.Log(" bad tile: " + index + " uv0: " + hexagon.uv0i + " error: " + e);}
  }

	public void ChangeRule(bool[] bs)
	{
		rules = bs;
	}

	public TileType[] GetOpposingElements()
	{
		TileType[] opp = new TileType[4];
		switch(type)
		{
			case TileType.Water: opp[0] = TileType.Fire; opp[1] = TileType.Air; opp[2] = TileType.Light; opp[3] = TileType.Sol; break;
			case TileType.Earth: opp[0] = TileType.Air; opp[1] = TileType.Fire; opp[2] = TileType.Light; opp[3] = TileType.Sol; break;
			case TileType.Dark: opp[0] = TileType.Light; opp[1] = TileType.Air; opp[2] = TileType.Fire; opp[3] = TileType.Sol; break; 
			case TileType.Luna:  opp[0] = TileType.Sol; opp[1] = TileType.Air; opp[2] = TileType.Light; opp[3] = TileType.Fire; break;
			case TileType.Fire: opp[0] = TileType.Water; opp[1] = TileType.Earth; opp[2] = TileType.Dark; opp[3] = TileType.Luna; break;
			case TileType.Air: opp[0] = TileType.Earth; opp[1] = TileType.Water; opp[2] = TileType.Dark; opp[3] = TileType.Luna; break;
			case TileType.Light: opp[0] = TileType.Dark; opp[1] = TileType.Earth; opp[2] = TileType.Water; opp[3] = TileType.Luna; break;
			case TileType.Sol: opp[0] = TileType.Luna; opp[1] = TileType.Earth; opp[2] = TileType.Dark; opp[3] = TileType.Water; break;
			default: break;
		}
		return opp;
	}

	public void HexLifeShift(HexTile[] neighbs)
	{
		//count neighbors
		int[] ec = new int[7];
		foreach (HexTile n in neighbs) {
			if (n.type == TileType.Gray) {
				ec[0]++;
			}
			if(n.type == TileType.Water){
				ec[1]++;
			}
			if(n.type == TileType.Fire){
				ec[2]++;
			}
			if(n.type == TileType.Earth){
				ec[3]++;
			}
			if(n.type == TileType.Sol)
			{
				ec[4]++;
			}
			if(n.type == TileType.Luna){
				ec[5]++;
			}
			if (n.type == TileType.Air) {
				ec[6]++;
			}
		}
		TileType tset = TileType.Gray;
		/*
		if ((ob1 && ec [0] == 1) || (ob2 && ec [0] == 2) || (ob3 && ec [0] == 3) || (ob4 && ec [0] == 4) || (ob5 && ec [0] == 5) || (ob6 && ec [0] == 6)) {
			tset = TileType.Gray;
		}
		if ((os1 && ec [0] == 1) || (os2 && ec [0] == 2) || (os3 && ec [0] == 3) || (os4 && ec [0] == 4) || (os5 && ec [0] == 5) || (os6 && ec [0] == 6)) {
			if (this.type == TileType.Gray) {
		tset = this.type;
				generation++;
			}
		}
		*/
		//if solar	
		if ((ec [1] + ec [3] + ec [5]) < (ec [2] + ec [4] + ec [6])) {
			if(ec[2] > ec[4] && ec[2] > ec[6]){
			if ((fb1 && ec [2] == 1) || (fb2 && ec [2] == 2) || (fb3 && ec [2] == 3) || (fb4 && ec [2] == 4) || (fb5 && ec [2] == 5) || (fb6 && ec [2] == 6)) {
				tset = TileType.Fire;
			}
				if ((fs1 && ec [2] == 1) || (fs2 && ec [2] == 2) || (fs3 && ec [2] == 3) || (fs4 && ec [2] == 4) || (fs5 && ec [2] == 5) || (fs6 && ec [2] == 6)) {
				
					tset = this.type;
				}
			}
			if(ec[6] > ec[2] && ec[6] > ec[4]){
			if ((sb1 && ec [6] == 1) || (sb2 && ec [6] == 2) || (sb3 && ec [6] == 3) || (sb4 && ec [6] == 4) || (sb5 && ec [6] == 5) || (sb6 && ec [6] == 6)) {
				tset = TileType.Air;
			}
				if ((ss1 && ec [6] == 1) || (ss2 && ec [6] == 2) || (ss3 && ec [6] == 3) || (ss4 && ec [6] == 4) || (ss5 && ec [6] == 5) || (ss6 && ec [6] == 6)) {

					tset = this.type;
				}
			}
			if (ec [4] > ec [2] && ec [4] > ec [6]) {
				if ((ab1 && ec [4] == 1) || (ab2 && ec [4] == 2) || (ab3 && ec [4] == 3) || (ab4 && ec [4] == 4) || (ab5 && ec [4] == 5) || (ab6 && ec [4] == 6)) {
					tset = TileType.Sol;
				}
				if ((as1 && ec [4] == 1) || (as2 && ec [4] == 2) || (as3 && ec [4] == 3) || (as4 && ec [4] == 4) || (as5 && ec [4] == 5) || (as6 && ec [4] == 6)) {
					tset = this.type;
				}
			}
		}
		//if lunar
			if ((ec [1] + ec [3] + ec [5]) > (ec [2] + ec [4] + ec [6])) {
			if (ec [3] > ec [5] && ec [3] > ec [1]) {
				if ((eb1 && ec [3] == 1) || (eb2 && ec [3] == 2) || (eb3 && ec [3] == 3) || (eb4 && ec [3] == 4) || (eb5 && ec [3] == 5) || (eb6 && ec [3] == 6)) {
					tset = TileType.Earth;
				}
				if ((es1 && ec [3] == 1) || (es2 && ec [3] == 2) || (es3 && ec [3] == 3) || (es4 && ec [3] == 4) || (es5 && ec [3] == 5) || (es6 && ec [3] == 6)) {

					tset = this.type;
				}
			}
			if(ec[5] > ec[3] && ec[5] > ec[1]){	
				if ((nb1 && ec [5] == 1) || (nb2 && ec [5] == 2) || (nb3 && ec [5] == 3) || (nb4 && ec [5] == 4) || (nb5 && ec [5] == 5) || (nb6 && ec [5] == 6)) {
					tset = TileType.Luna;
				}
				if ((ns1 && ec [5] == 1) || (ns2 && ec [5] == 2) || (ns3 && ec [5] == 3) || (ns4 && ec [5] == 4) || (ns5 && ec [5] == 5) || (ns6 && ec [5] == 6)) {
					tset = this.type;
					}
				}
			if (ec [1] > ec [5] && ec [1] > ec [3]) {
				if ((wb1 && ec [1] == 1) || (wb2 && ec [1] == 2) || (wb3 && ec [1] == 3) || (wb4 && ec [1] == 4) || (wb5 && ec [1] == 5) || (wb6 && ec [1] == 6)) {
					tset = TileType.Water;
				}
				if ((ws1 && ec [1] == 1) || (ws2 && ec [1] == 2) || (ws3 && ec [1] == 3) || (ws4 && ec [1] == 4) || (ws5 && ec [1] == 5) || (ws6 && ec [1] == 6)) {
					tset = this.type;
				}
			}
		}
		//if equal persist
		if ((ec [1] + ec [3] + ec [5]) == (ec [2] + ec [4] + ec [6])) {
			tset = this.type;
		}
		/*
		if (this.type == TileType.Water && ecounts [2] > 0) {
				
		}  
		int test = 0;
		int x = 0;
		TileType ttset = TileType.None;
		for (int i = 0; i < 7; i++) {
			if (ecounts [i] > test) {
				test = ecounts [i];
				x = i;
			}
			ttset = types [x];
		}
		*/

		if (tset == this.type) {
			if (!flip) {
				generation++;
			}
			if (flip && this.type != TileType.Gray) {
				generation--;
			}
			if (generation >= 6) {
				flip = true;
			}
			if (generation <= 0) {
				flip = false;
			}
		} else {
			generation = 0;
		}
		this.ChangeType (tset);
	}


  public int GetNeighborID(int dir)
  {
    return hexagon.neighbors[dir];
  }

  public HexTile GetNeighbor(List<HexTile> tiles, int dir)
  {
    return tiles[hexagon.neighbors[dir]];
  }

  public virtual void OnUnitEnter() { }
}

public class HexTile_Grass : HexTile
{ 
  public override void OnUnitEnter()
  {
    Debug.Log("The grass rustles as a unit enters.");
    // Some custom tile logic here
  }
}