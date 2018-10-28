using UnityEngine;
using System.Collections;

public class ZoneViewCamera : MonoBehaviour {

  float tapRadius = 15;

  float tapRadiusSquared;
  Transform myTrans;
  float dragSpeed = .01f;
  float zoomSpeed = 30;
  bool dragging;
  Vector2 dragStartPos;

	public void Initialize ()
  {
    myTrans = transform;
    tapRadiusSquared = tapRadius * tapRadius;
	}
	
	void Update ()
  {
    // Begin touch/click
    if (Input.GetMouseButtonDown(0))
    {
      dragging = true;
      dragStartPos = Input.mousePosition;
    }

    // Dragging
    if (dragging && Input.GetMouseButton(0))
    {  
      Vector2 touchDeltaPosition = (Vector2)(Input.mousePosition) - dragStartPos;

      if (touchDeltaPosition.sqrMagnitude > tapRadiusSquared)
      {
        myTrans.Translate(-touchDeltaPosition.x * dragSpeed, 0, -touchDeltaPosition.y * dragSpeed, Space.World);
      }
    }

    // Releasing
    if (Input.GetMouseButtonUp(0))
    {
      dragging = false; 
      Vector2 currentPos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
      Vector2 touchDeltaPosition = currentPos - dragStartPos;
      
      // Interpret as tap
      if (touchDeltaPosition.sqrMagnitude < tapRadiusSquared)
        GameManager.OnTapInput(currentPos);
    }

    // Zooming in/out
    float scroll = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
    if (scroll != 0)
    {
      myTrans.Translate(Vector3.forward * scroll * Time.deltaTime);
    }
	}
}
