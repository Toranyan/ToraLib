using UnityEngine;
using System.Collections;

namespace tora.camera {

	/// <summary>
	/// C clone camera.
	/// Copies another camera
	/// </summary>
	public class CCloneCamera : MonoBehaviour {

		public Camera targetCamera;

		public float scale;
		
		// Update is called once per frame
		void Update () {

			transform.localRotation = targetCamera.transform.localRotation;
			transform.localPosition = targetCamera.transform.localPosition * scale;
		
		}
	}


}