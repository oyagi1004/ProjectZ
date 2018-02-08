using nightowl.DepthMap;
using UnityEngine;

namespace nightowl.WaterShader
{
	[ExecuteInEditMode]
	public class ConnectHeightMap : MonoBehaviour 
	{
		// Refs
		public AbstractHeightMapGenerator generator;
		public WaterShader waterScript;

		// Mono
		void Update ()
		{
			if (generator == null || waterScript == null)
			{
				Debug.LogWarning("ConnectHeightMap not setup: HeightMapGenerator " + (generator == null ? "null" : "ok") + ", LavaScript " + (waterScript == null ? "null" : "ok"));
				return;
			}
			waterScript.heightTexture = generator.GetHeightMap();
		}
	}
}
