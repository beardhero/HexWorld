using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputMode {none, Keyboard, OpenVR}

public class InputManager : MonoBehaviour {

	public InputMode inputMode = InputMode.Keyboard;

	public delegate void InputAction();
	
	public static event InputAction OnChatToggle, OnBack;

	public void Initialize(){
		
	}

	void Update()
	{
		if (inputMode == InputMode.Keyboard){

			// Because Return and Escape key events are consumed by the textfield, we have to access the event system directly to poll them

			if (GUIManager.currentEvent != null && GUIManager.currentEvent.isKey){
				if (GUIManager.currentEvent.keyCode == KeyCode.Return)
					OnChatToggle();
				else if (GUIManager.currentEvent.keyCode == KeyCode.Escape)
					OnBack();
			} 
		}
	}
}
