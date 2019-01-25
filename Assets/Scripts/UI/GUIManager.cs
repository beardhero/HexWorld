using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour {

	List<Action> activeUIs = new List<Action>();

	void Initialize(){
		Debug.Log("initializing ui scripts");
		activeUIs.Add( () => ChatWindow.Render() );
	}

	void OnGUI(){
		foreach (Action a in activeUIs) {
	    	a.Invoke();
		}
	}
}
