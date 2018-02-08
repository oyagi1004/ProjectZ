using UnityEngine;

namespace nightowl.WaterShader
{
	public class CodeDemo22 : MonoBehaviour
	{
		// Refs
		public Material Material;
		public Color MainColor;
		public Color CounterColor;

		// Mono
		void Update()
		{
			if (Material.HasProperty("_MainColor"))
			{
				Material.SetColor("_MainColor",
					CodeDemoHelper.HelperTimeNormalized*MainColor + (1 - CodeDemoHelper.HelperTimeNormalized)*CounterColor);
			}
		}
	}
}