using UnityEngine;
using System.Collections;

using UnityEditor;


namespace tora.assetbundle {

	public class CAssetBundleGUI : EditorWindow {

		[MenuItem("Tora/Asset Bundles/Asset Bundle Build GUI")]
		public static void Init() {
			CAssetBundleGUI window = (CAssetBundleGUI)EditorWindow.GetWindow (typeof (CAssetBundleGUI));
			window.Show();
		}

		public void OnGUI() {
			if(GUILayout.Button("Build Asset Bundles")) {
				CAssetBundleBuilder.BuildAllAssetBundles();
			}
		}

	}

}