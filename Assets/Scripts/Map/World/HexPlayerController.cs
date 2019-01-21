using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexPlayerController : MonoBehaviour {
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
	float currentHeight = 0;
	float testH = 0;
	public float gravityScale = 4f;
	public float walkSpeed = 1.33f;
	public float runSpeed = 1.33f;
	public float rotateSpeed = 2.4f;
	public float jumpHeight = 24f;
	public bool canJump;
	public bool jumped;
	public int numberOfJumps;
	public int maxJumps = 2;
	public Camera cam;
	public float zoomMax = 10.0f;
	public float zoomMin = 0.5f;
	public float camZoomStep = .1f;
	//public float camZoomStep = .3f;
	public float camRotateSpeed = 4.2f;
	public float camSens = .5f;
	public int spawnTile = 0;
	public Runebook runeBook;
	// Use this for initialization
	void Start () {
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
		//runebook test
		byte[] b = new byte[32];
		for(int i = 0; i < 32; i++)
		{
			b[i] = (byte)Random.Range(0,256);
		}
		runeBook = new Runebook(b);
		GameObject runebook = Instantiate(runeBook.RunebookGO());
		/* 
		Transform runebookTrans = runebook.transform;
		runebookTrans.parent = cam.transform;
		runebookTrans.position = cam.transform.position + cam.transform.forward;
		runebookTrans.LookAt(cam.transform);
		*/
		/*
		foreach(Rune r in runeBook.runes)
		{
			Instantiate(r.RuneGO());
		}
		*/
	}
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space) && numberOfJumps < maxJumps)
		{
			numberOfJumps++;
			jumped = true;
		}
		
		float f = Input.GetAxis("Mouse ScrollWheel");
		Vector3 v = cam.transform.position - trans.position;
		if(v.magnitude <= zoomMax && f < 0){cam.transform.position -= f*v*camZoomStep;}
		if(v.magnitude >= zoomMin && f > 0){cam.transform.position -= f*v*camZoomStep;}
	}
	// Update is called once per frame
	void FixedUpdate () { 
		gravityDir = (origin - trans.position).normalized;
		trans.rotation = Quaternion.FromToRotation(trans.up, -gravityDir) * trans.rotation;
		rigbody.AddForce(gravityDir * gravityScale * rigbody.mass, ForceMode.Acceleration);
		/* 
		if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
		{
			if(Input.GetKey(KeyCode.LeftShift))
			{
				animator.Play("Run");
			}
			else
			{
				animator.Play("Walk");
			}
		}
		*/
		if(rigbody.velocity.sqrMagnitude == 0)
		{
			animator.Play("Idle");
		}
		
		float vert = Input.GetAxis("Vertical");
		if(vert != 0)
		{
			rigbody.velocity += trans.forward * vert * walkSpeed;
			animator.Play("Walk");
		}

		float horz = Input.GetAxis("Horizontal");
		if(horz != 0)
		{
			//rigbody.velocity += -trans.right * vert * walkSpeed;
			trans.RotateAround(trans.position, gravityDir, -horz * rotateSpeed);
			animator.Play("Walk");
		}
		
		/* 
		if(Input.GetKey(KeyCode.W))
		{
			//animator.enabled = true;
			if(Input.GetKey(KeyCode.LeftShift))
			{
				rigbody.velocity += trans.forward * runSpeed;
				//{animator.Play("Run");}
			}
			else
			{
				rigbody.velocity += trans.forward * walkSpeed;
				//{animator.Play("Walk");}
			}
		}
		if(Input.GetKey(KeyCode.S))
		{
			//animator.enabled = true;
			if(Input.GetKey(KeyCode.LeftShift))
			{
				rigbody.velocity += -trans.forward * runSpeed;
				//{animator.Play("Run");}
			}
			else{
				rigbody.velocity += -trans.forward * walkSpeed;
				//{animator.Play("Walk");}
			}
		}
		
		if(Input.GetKey(KeyCode.D))
		{
			trans.RotateAround(trans.position, gravityDir, -rotateSpeed);
			//animator.Play("Walk");
		}
		if(Input.GetKey(KeyCode.A))
		{
			trans.RotateAround(trans.position, gravityDir, rotateSpeed);
			//animator.Play("Walk");
		}
		*/

		if(jumped && canJump)
		{
			animator.Play("Levitate");
			rigbody.AddForce(-gravityDir * jumpHeight);
			jumped = false;
		}
		if(numberOfJumps >= maxJumps)
		{
			canJump = false;
		}
		if(Input.GetKey(KeyCode.Mouse1))
		{
			cam.transform.RotateAround(trans.position, gravityDir, -camRotateSpeed*Input.GetAxis("Mouse X"));
			cam.transform.RotateAround(cam.transform.position, cam.transform.right, -camRotateSpeed*Input.GetAxis("Mouse Y"));
			cam.transform.RotateAround(trans.position, cam.transform.right, -camRotateSpeed*Input.GetAxis("Mouse Y"));
		}
	}
	void OnCollisionEnter(Collision collision)
	{
		if(numberOfJumps > 0)
		{
			numberOfJumps = 0;
			canJump = true;
		}
	}      
	void OnCollisionStay(Collision collision)
	{
		canJump = true;
	}
}
