using nightowl.DepthMap;
using UnityEngine;

namespace nightowl.WaterShader
{
	public class CodeDemo3 : MonoBehaviour
	{
		// Refs
		public WaterShader WaterShaderScript;
		public HeightMapRenderer TextureRenderer;

		// Mono
		void Update()
		{
			WaterShaderScript.heightTexture = CodeDemoHelper.HelperTimeSin > 0 ? TextureRenderer.HeightTexture : null;
		}
	}
}