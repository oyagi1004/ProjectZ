using UnityEngine;

namespace nightowl.WaterShader
{
	public class CodeDemo2 : MonoBehaviour
	{
		// Refs
		public Material Material;
		public Texture Texture;

		// Mono
		void Update()
		{
			if (Material.HasProperty("_NormalTex"))
			{
				Material.SetTexture("_NormalTex", CodeDemoHelper.HelperTimeSin > 0 ? Texture : null);
			}
		}
	}
}