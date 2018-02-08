using UnityEngine;

namespace nightowl.WaterShader
{
	public class CodeDemo23 : MonoBehaviour
	{
		// Refs
		public Material Material;

		// Mono
		void Update()
		{
			if (Material.HasProperty("_Transparency"))
			{
				Material.SetFloat("_Transparency", CodeDemoHelper.HelperTimeNormalized);
			}
		}
	}
}