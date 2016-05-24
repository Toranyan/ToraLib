using UnityEngine;
using System.Collections;

namespace tora.assetbundle {

	public class CAssetBundleConfigs {

		public const string ASSET_BUNDLE_LOCAL_PATH = "/AssetBundles/"; //from datapath


		public static string GetAssetBundleBaseURL() {
			return "file://" + Application.dataPath + ASSET_BUNDLE_LOCAL_PATH;
		}
		
	}

}