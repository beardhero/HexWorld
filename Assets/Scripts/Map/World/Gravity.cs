using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour {

	public Rigidbody rb;
	public Transform playerTrans;
	public WorldManager worldManager;
	public Vector3 worldOrigin;
	public Vector3 gravityDirection;
	public float gravityScale = 4;

	void Start(){
		playerTrans = this.gameObject.transform;
		
		rb = GetComponent<Rigidbody>();
		rb.useGravity = false;
		rb.freezeRotation = true;
		
		worldManager = GameObject.Find("WorldManager").GetComponent<WorldManager>();
		World activeWorld = worldManager.activeWorld;
		worldOrigin = activeWorld.origin;
		playerTrans.position = activeWorld.tiles[0].hexagon.center;// * 1.1f;
	}
	// Update is called once per frame
	void FixedUpdate () {
		gravityDirection = (worldOrigin - playerTrans.position).normalized; 
		playerTrans.rotation = Quaternion.FromToRotation(playerTrans.up, -gravityDirection) * playerTrans.rotation;
		rb.AddForce(gravityDirection * gravityScale * rb.mass, ForceMode.Acceleration);
	}
}
