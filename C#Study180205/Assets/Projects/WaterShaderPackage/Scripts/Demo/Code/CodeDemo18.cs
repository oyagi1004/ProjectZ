using UnityEngine;

namespace nightowl.WaterShader
{
	public class CodeDemo18 : MonoBehaviour
	{
		// Refs
		public WaterShader WaterShaderScript;

		// Mono
		void Update()
		{
			var layersDefault = 1;
			var layersReflect = LayerMask.GetMask("ReflectionOnly");
			WaterShaderScript.ReflectLayers = CodeDemoHelper.HelperTimeSin < 0 ? layersDefault : layersReflect;
		}
	}
}