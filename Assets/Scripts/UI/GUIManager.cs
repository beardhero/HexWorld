using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIFocus {None, Chat};

public class GUIManager : MonoBehaviour {

	public static ChatWindow chatWindow;

	public static Event currentEvent;

	List<Window> activeUIs = new List<Window>();

	public UIFocus focus;

	// --- Temp ---
	bool showLookingForMatch, showInMatchmaking;
	// --- /Temp ---

	public void Initialize(){
		chatWindow = new ChatWindow(this);

		// Note: we use a list of active ui's in order to be able to add and remove interface elements as they become or cease to be relevant.
		activeUIs.Add(chatWindow);

		// Temp
		showLookingForMatch = true;
	}

	void Update(){
		foreach (Window w in activeUIs)
			w.Update();
	}

	void OnGUI(){
		currentEvent = Event.current;
		
		foreach (Window w in activeUIs) {
	    	w.Render();
		}

		GUILayout.BeginArea(new Rect(0,0,100,40));
		if (showLookingForMatch)
		{
			
			if (GUILayout.Button("Click to join matchmaking")){
				NetworkManager.JoinMatchmakingQueue();
				showLookingForMatch = false;
				showInMatchmaking = true;
			}
		}
		else if (showInMatchmaking){
			GUILayout.Label("Looking for match....");
		}
		GUILayout.EndArea();
	}
}
