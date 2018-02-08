using UnityEngine;
using System.Collections;

namespace nightowl.WaterShader
{
	[ExecuteInEditMode]
	public class WaterShader : MonoBehaviour
	{
		public int ReflectionTextureSize = 1024;
		public float ClipPlaneOffset = 0.07f;
		public LayerMask ReflectLayers = -1;
		public bool reflectSky = true;
		public Texture heightTexture = null;
		//public Texture flowTexture = null;
		public float timeOffset = 120f;

		private Hashtable reflectionCameras = new Hashtable();
		private RenderTexture reflectionTexture = null;
		private int oldReflectionTextureSize = 0;
		private static bool insideRendering = false;


		// Mono
		void OnWillRenderObject()
		{
			UpdateRender();
		}

		void OnDisable()
		{
			Clear();
		}

		// WaterShader
		private void UpdateRender()
		{
			Material[] materials = GetComponent<Renderer>().sharedMaterials;
			foreach (Material mat in materials)
			{
				if (mat.HasProperty("_UVOffset"))
				{
					Vector4 movement = mat.GetVector("_Movement");
					Vector4 offset = movement;
					float time = Time.time + timeOffset;
					offset += (Mathf.Sin(time)*0.2f + time)*new Vector4(movement.x, movement.y, 0, 0);
					offset += (Mathf.Cos(time)*0.2f + time)*new Vector4(0, 0, movement.z, movement.w);
					//offset = movement;
					mat.SetVector("_UVOffset", offset);
				}
				if (mat.HasProperty("_ShoreFoamMovement"))
				{
					Vector4 vector = mat.GetVector("_ShoreFoamMovement");

					Vector2 movement = new Vector2(vector.x, vector.y);
					Vector2 offset = movement*Time.time*0.05f;
					vector.z = offset.x;
					vector.w = offset.y;
					mat.SetVector("_ShoreFoamMovement", vector);
				}
			}

			Camera cam = Camera.current;
			if (!cam)
				return;

			//cam.depth = 24.0f;
			cam.depthTextureMode = DepthTextureMode.Depth;

			if (!enabled || !GetComponent<Renderer>() || !GetComponent<Renderer>().sharedMaterial ||
			    !GetComponent<Renderer>().enabled)
				return;

			if (insideRendering)
				return;
			insideRendering = true;

			Camera reflectionCamera;
			CreateSurfaceObjects(cam, out reflectionCamera);

			Vector3 pos = transform.position;
			Vector3 normal = GetComponent<MeshFilter>() != null
				? transform.TransformDirection(GetComponent<MeshFilter>().sharedMesh.normals[0])
				: transform.up;

			int oldPixelLightCount = QualitySettings.pixelLightCount;
			QualitySettings.pixelLightCount = 0;

			UpdateCameraModes(cam, reflectionCamera);

			float d = -Vector3.Dot(normal, pos) - ClipPlaneOffset;
			Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

			Matrix4x4 reflection = Matrix4x4.zero;
			CalculateReflectionMatrix(ref reflection, reflectionPlane);
			Vector3 oldpos = cam.transform.position;
			Vector3 newpos = reflection.MultiplyPoint(oldpos);
			reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix*reflection;

			Vector4 clipPlane = CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);
			Matrix4x4 projection = cam.projectionMatrix;
			CalculateObliqueMatrix(ref projection, clipPlane);
			reflectionCamera.projectionMatrix = projection;

			reflectionCamera.cullingMask = ~(1 << 4) & ReflectLayers.value; //never render water layer
			reflectionCamera.targetTexture = reflectionTexture;
			GL.invertCulling = true;
			reflectionCamera.transform.position = newpos;
			Vector3 euler = cam.transform.eulerAngles;
			reflectionCamera.transform.eulerAngles = new Vector3(0, euler.y, euler.z);
			reflectionCamera.Render();
			reflectionCamera.transform.position = oldpos;
			GL.invertCulling = false;
			foreach (Material mat in materials)
			{
				if (mat.HasProperty("_ReflectionTex"))
					mat.SetTexture("_ReflectionTex", reflectionTexture);
				if (mat.HasProperty("_EnvironmentTex"))
					mat.SetTexture("_EnvironmentTex", heightTexture);
				//if (mat.HasProperty("_FlowTex"))
				//	mat.SetTexture("_FlowTex", flowTexture);
			}

			Matrix4x4 scaleOffset = Matrix4x4.TRS(
				new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));
			Vector3 scale = transform.lossyScale;
			Matrix4x4 mtx = transform.localToWorldMatrix*Matrix4x4.Scale(new Vector3(1.0f/scale.x, 1.0f/scale.y, 1.0f/scale.z));
			mtx = scaleOffset*cam.projectionMatrix*cam.worldToCameraMatrix*mtx;
			foreach (Material mat in materials)
				mat.SetMatrix("_ProjMatrix", mtx);

			QualitySettings.pixelLightCount = oldPixelLightCount;

			insideRendering = false;
		}

		private void UpdateCameraModes(Camera src, Camera dest)
		{
			if (dest == null)
				return;

			dest.clearFlags = reflectSky ? src.clearFlags : CameraClearFlags.Color;
			dest.backgroundColor = src.backgroundColor;


			if (src.clearFlags == CameraClearFlags.Skybox && reflectSky)
			{
				Skybox sky = src.GetComponent(typeof (Skybox)) as Skybox;
				Skybox mysky = dest.GetComponent(typeof (Skybox)) as Skybox;
				if (!sky || !sky.material)
				{
					mysky.enabled = false;
				}
				else
				{
					mysky.enabled = true;
					mysky.material = sky.material;
				}
			}

			dest.farClipPlane = src.farClipPlane;
			dest.nearClipPlane = src.nearClipPlane;
			dest.orthographic = src.orthographic;
			dest.fieldOfView = src.fieldOfView;
			dest.aspect = src.aspect;
			dest.orthographicSize = src.orthographicSize;
		}

		private void CreateSurfaceObjects(Camera currentCamera, out Camera reflectionCamera)
		{
			if (!reflectionTexture || oldReflectionTextureSize != ReflectionTextureSize)
			{
				if (reflectionTexture)
					DestroyImmediate(reflectionTexture);
				reflectionTexture = new RenderTexture(ReflectionTextureSize, ReflectionTextureSize, 24);
				reflectionTexture.name = "__SurfaceReflection" + GetInstanceID();
				reflectionTexture.isPowerOfTwo = true;
				reflectionTexture.hideFlags = HideFlags.HideAndDontSave;
				oldReflectionTextureSize = ReflectionTextureSize;
			}

			reflectionCamera = reflectionCameras[currentCamera] as Camera;
			if (!reflectionCamera)
			{
				GameObject go = new GameObject(
					"Surface Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof (Camera),
					typeof (Skybox));
				reflectionCamera = go.GetComponent<Camera>();
				reflectionCamera.enabled = false;
				reflectionCamera.transform.position = transform.position;
				reflectionCamera.transform.rotation = transform.rotation;
				go.hideFlags = HideFlags.HideAndDontSave;
				reflectionCameras[currentCamera] = reflectionCamera;
			}
		}

		private static float sgn(float a)
		{
			if (a > 0.0f) return 1.0f;
			if (a < 0.0f) return -1.0f;
			return 0.0f;
		}

		private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
		{
			Vector3 offsetPos = pos + normal*ClipPlaneOffset;
			Matrix4x4 m = cam.worldToCameraMatrix;
			Vector3 cpos = m.MultiplyPoint(offsetPos);
			Vector3 cnormal = m.MultiplyVector(normal).normalized*sideSign;
			return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
		}

		private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
		{
			Vector4 q = projection.inverse*new Vector4(
				sgn(clipPlane.x),
				sgn(clipPlane.y),
				1.0f,
				1.0f
				);
			Vector4 c = clipPlane*(2.0F/(Vector4.Dot(clipPlane, q)));
			projection[2] = c.x - projection[3];
			projection[6] = c.y - projection[7];
			projection[10] = c.z - projection[11];
			projection[14] = c.w - projection[15];
		}

		private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
		{
			reflectionMat.m00 = (1F - 2F*plane[0]*plane[0]);
			reflectionMat.m01 = (-2F*plane[0]*plane[1]);
			reflectionMat.m02 = (-2F*plane[0]*plane[2]);
			reflectionMat.m03 = (-2F*plane[3]*plane[0]);

			reflectionMat.m10 = (-2F*plane[1]*plane[0]);
			reflectionMat.m11 = (1F - 2F*plane[1]*plane[1]);
			reflectionMat.m12 = (-2F*plane[1]*plane[2]);
			reflectionMat.m13 = (-2F*plane[3]*plane[1]);

			reflectionMat.m20 = (-2F*plane[2]*plane[0]);
			reflectionMat.m21 = (-2F*plane[2]*plane[1]);
			reflectionMat.m22 = (1F - 2F*plane[2]*plane[2]);
			reflectionMat.m23 = (-2F*plane[3]*plane[2]);

			reflectionMat.m30 = 0F;
			reflectionMat.m31 = 0F;
			reflectionMat.m32 = 0F;
			reflectionMat.m33 = 1F;
		}

		private void Clear()
		{
			ClearReflectionTexture();
			ClearCameras();
		}

		private void ClearReflectionTexture()
		{
			if (reflectionTexture)
			{
				DestroyImmediate(reflectionTexture);
				reflectionTexture = null;
			}
		}

		private void ClearCameras()
		{
			foreach (DictionaryEntry kvp in reflectionCameras)
				DestroyImmediate(((Camera) kvp.Value).gameObject);
			reflectionCameras.Clear();
		}
	}
}