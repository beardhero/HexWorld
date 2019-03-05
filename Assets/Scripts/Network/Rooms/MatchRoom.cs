using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Colyseus;

public class MatchRoom : MonoBehaviour {

	void OnPlayerMove (DataChange change)
  	{
//    Debug.Log ("OnPlayerMove");
//    Debug.Log ("playerId: " + change.path["id"] + ", Axis: " + change.path["axis"]);
//    Debug.Log (change.value);

    	GameObject cube;
    	//players.TryGetValue (change.path ["id"], out cube);

	    //cube.transform.Translate (new Vector3 (Convert.ToSingle(change.value), 0, 0));
	}
}
