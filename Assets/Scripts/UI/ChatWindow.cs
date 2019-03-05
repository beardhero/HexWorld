using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatWindow : Window {

	public bool chatOpen;

	Rect chatArea,
		chatDisplayArea,
		chatInputArea;

	string textInput;

	bool flag_focusChat, flag_clearFocus;	// These are flags for triggering GUI focus from outside of OnGUI

	int initTimer;		// This stupid workaround for GUI.SetNextControlName requiring multiple calls to init

	public ChatWindow(){}
	public ChatWindow(GUIManager m){
		manager = m;	// inherited from Window
		chatArea = new Rect(0, Screen.height-(Screen.height/4f), Screen.width/2f, Screen.height/4f);
		chatInputArea = new Rect(0, chatArea.height-20, chatArea.width, 20);
		chatDisplayArea = new Rect(0, 0, chatArea.width, chatArea.height-chatInputArea.height);

		// Input hooks
		InputManager.OnChatToggle += () => {
			if (manager.focus == UIFocus.Chat){
				Message();
				manager.focus = UIFocus.None;
			}
			else {
				chatOpen = true;
				manager.focus = UIFocus.Chat;
				flag_focusChat = true;
			}
		};
		InputManager.OnBack += () => {
			if (manager.focus == UIFocus.Chat){
				manager.focus = UIFocus.None;
				textInput = "";
				flag_clearFocus = true;
			}
			else if (chatOpen)
				chatOpen = false;
		};

		initTimer = 5;
	}

	public override void Update(){

	}

	public override void Render(){
		// --- Must use this dumb flagging system in order to control focus from outside of OnGUI loop
		if (flag_focusChat){
			GUI.FocusControl("chatInputControl");
			flag_focusChat = false;
		}

		if (flag_clearFocus){
			GUI.FocusControl(null);
			flag_clearFocus = false;
		}
		// --- End dumb flagging system

		if (initTimer > 0)
		{
			chatOpen = true;
			if (initTimer == 1){
				chatOpen = false;
			}
			initTimer --;
		}

		if (chatOpen){

			GUI.BeginGroup(chatArea);
				GUI.Box(chatDisplayArea, "Received chats here");
				GUI.SetNextControlName("chatInputControl");
				textInput = GUI.TextField(chatInputArea, textInput);
			GUI.EndGroup();
		}
	}

	void Message(){
		NetworkManager.SendMessage(textInput);
		textInput = "";
	}
}
