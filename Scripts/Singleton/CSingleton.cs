using UnityEngine;
using System.Collections;

namespace tora.singleton {

	/// <summary>
	/// Standard Monobehaviour singleton
	/// extend monobehaviours that use the singleton pattern with this class
	/// </summary>
	public class CSingleton<T> : MonoBehaviour where T : Component {

		protected static T m_instance;

		public static T Instance {
			get {
				if(m_instance == null) {
					Instantiate();
				}
				return m_instance;
			}
		}

		public static T Instantiate() {
			if(m_instance == null) {
				m_instance = new GameObject(typeof(T).Name).AddComponent<T>();
			}
			return m_instance;
		}

		public static void Destroy() {
			if(m_instance == null) {
				//warn
				Debug.LogWarning("Attempt to destroy non-instantiated singleton");
				return;
			}
			GameObject.Destroy(m_instance.gameObject);
		}

		
	}

}