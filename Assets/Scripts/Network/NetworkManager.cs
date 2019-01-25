using UnityEngine;

[RequireComponent(typeof(ColyseusClient))]
public class NetworkManager : MonoBehaviour
{
	ColyseusClient client;

	public void Initialize(){
		client = GetComponent<ColyseusClient>();
		// Initialize takes a callback, which here is an anonymous delegate
		StartCoroutine(client.Initialize( (msg) => Debug.Log(msg) ));
	}
}