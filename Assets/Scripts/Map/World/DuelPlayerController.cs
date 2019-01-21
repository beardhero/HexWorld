using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelPlayerController : MonoBehaviour {
	Rigidbody rigbody;
	Transform trans;
	GameObject player;
	Transform head;
	Vector3 gravityDir;
	Vector3 moveDir;
	WorldManager wM;
	World aW;
	Vector3 origin;
	Animator animator;
	Camera cam;
	Actor actor;
	[HideInInspector]public Ray ray;
  	[HideInInspector]public RaycastHit hit;
	public int spawnTile = 0;
	public bool move;
	public bool cast;
	public RuneHex runeHex;
	// Use this for initialization
	void Start () {
		actor = GetComponent<Actor>();
		player = this.gameObject;
		trans = player.transform;
		//head = GameObject.Find("Head").transform;
		rigbody = GetComponent<Rigidbody>();
		rigbody.useGravity = false;
		rigbody.freezeRotation = true;
		cam = Camera.main;
		wM = GameObject.Find("WorldManager").GetComponent<WorldManager>();
		aW = wM.activeWorld;
		trans.position = aW.tiles[spawnTile].hexagon.center;
		origin = new Vector3(aW.origin.x, aW.origin.y, aW.origin.z);
		animator = player.GetComponent<Animator>();
		animator.enabled = true;
		animator.Play("Idle");

		//runehex test
		runeHex = new WaterAttackI();
		runeHex.Initialize();
		//move = false;
		//cast = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Mouse0))
    	{
			
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      		if(Physics.Raycast(ray, out hit, 100.0f))
      		{ 
        		Debug.Log("casted");
        		int to = wM.GetHitTile(hit);
				Debug.Log("hit tile: " + to);
				StartCoroutine(actor.Move(to));
     		}
    	}
		//cast test
		if (Input.GetKeyDown(KeyCode.Mouse1))
    	{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      		if(Physics.Raycast(ray, out hit, 100.0f))
      		{ 
        		Debug.Log("casted");
        		int to = wM.GetHitTile(hit);
				Debug.Log("hit tile: " + to);
				runeHex.Cast(to);
     		}
    	}
	}
}
