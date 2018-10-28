using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(WorldManager))]
public class WorldEditor : Editor {
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		WorldManager wm = (WorldManager)target;
		
		if(GUILayout.Button("Capture PNG"))
		{
			wm.CapturePNG();
		}
	}
}
