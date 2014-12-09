using UnityEngine;
using System.Collections;

public class cameraControlTest : MonoBehaviour {

	public float speed = 10.0F;
	public GameObject camera;
	void Update() {
		Vector3 movement= (speed*(new Vector3(Input.GetAxis("LA_h"),Input.GetAxis("RA_v"),Input.GetAxis("LA_v"))));
		float rotation = Input.GetAxis("RA_h")*100;
		movement *= Time.deltaTime;
		rotation *= Time.deltaTime;
		Vector3 newVec=camera.transform.rotation*movement+transform.position;
		transform.position=newVec;
		transform.Rotate(0, rotation, 0);
	}
}
