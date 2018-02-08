using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using LunarCatsStudio.SuperCombiner;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Main class of Super Combiner asset.
/// </summary>
public class SuperCombiner : MonoBehaviour {

	public enum CombineStatesList {Uncombined, Combining, Combined}
	public List<int> TextureAtlasSizes = new List<int>() { 
		32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 
	};
	public List<string> TextureAtlasSizesNames = new List<string>() { 
		"32", "64", "128", "256", "512", "1024", "2048", "4096", "8192"
	};

	public CombineStatesList combiningState = CombineStatesList.Uncombined;
	public TexturePacker texturePacker = new TexturePacker();
	public MeshCombiner meshCombiner = new MeshCombiner();

	// Editable Parameters
	public int textureAtlasSize = 1024;
	public bool combineMaterials = true;
	public bool combineMeshes = false;
	public int meshOutput;
	public int maxVerticesCount = 65534;
	public string sessionName = "combinedSession";
	public bool combineAtRuntime = false;

	// Saving options
	public bool savePrefabs = false;
	public bool saveMeshObj = false;
	public bool saveMeshFbx = false;
	public bool saveMaterials = false;
	public bool saveTextures = false;
	public string folderDestination = "Assets/SuperCombiner/Combined";

	// Internal combine process variables
	public List<MeshRenderer> meshList = new List<MeshRenderer>();				// List of all MeshRenderer in all children
	public List<SkinnedMeshRenderer> skinnedMeshList = new List<SkinnedMeshRenderer>();	// List of SkinnedMeshRenderer in all children
	public List<Rect> materialUVBounds = new List<Rect>();						// List of UV Bound for each mesh, after material transformation (offset and scale)
	public Dictionary<int, string> originalMeshMaterialId = new Dictionary<int, string>();	// List of copied meshes instancesId associated with their original sharedMesh and sharedMaterial instanceId. This is usefull not to save duplicated mesh when exporting
	public Dictionary<int, string> copyMeshId = new Dictionary<int, string>();
	public List<GameObject> originalObjectList = new List<GameObject>();		// List of original game objects used for combining
	public List<GameObject> modifiedObjectList = new List<GameObject>();		// List of transformed game objects
	public List<GameObject> modifiedSkinnedObjectList = new List<GameObject>();	// List of transformed skinned meshes game objects
	public List<GameObject> toSavePrefabList = new List<GameObject>();			// List of transformed game objects for prefab saving
	public List<MeshRenderer> toSaveObjectList = new List<MeshRenderer>();		// List of transformed game objects for saving purpose
	public List<Mesh> toSaveMeshList = new List<Mesh>();						// List of meshes to save
	public List<SkinnedMeshRenderer> toSaveSkinnedObjectList = new List<SkinnedMeshRenderer>();// List of transformed skinned game objects for saving purpose
	public List<MeshFilter> originalMeshesList = new List<MeshFilter>();		// List of original mesh filters
	public List<SkinnedMeshRenderer> originalSkinnedMeshesList = new List<SkinnedMeshRenderer>();// List of original skinned mesh renderer
	public List<int> combinedTextureIndex = new List<int>();					// CombinedGameObjects[i] will use uvs[combinedTextureIndex[i]]
	public Material savedMaterial;												// 

	private GameObject modifiedParent;		// Each combined game object will be a children of this object

	private SuperCombiner() {
		// Nothing to do here
	}

	void Start() {
		if (combineAtRuntime) {
			Combine ();
		}
	}

	// Combine process
	public void Combine()
	{
		combiningState = CombineStatesList.Combining;

		DateTime timeStart = DateTime.Now;

		Debug.Log("[Super Combiner] Start processing...");

		#if UNITY_EDITOR
		// UI Progress bar display in Editor
		EditorUtility.DisplayProgressBar("Super Combiner", "Meshes listing...", 0.1f);
		#endif

		// Getting the list of meshes ...
		meshList = FindEnabledMeshes (transform, true);
		skinnedMeshList = FindEnabledSkinnedMeshes (transform, true);

		#if UNITY_EDITOR
		// UI Progress bar display in Editor
		EditorUtility.DisplayProgressBar("Super Combiner", "Materials and textures listing...", 0.1f);
		#endif
		// Getting list of materials and textures
		texturePacker.materialList = FindEnabledMaterials (meshList, skinnedMeshList);

		// Check if there is at least 2 meshes in the current gameobject
		if (combineMeshes) {
			if (meshList.Count + skinnedMeshList.Count < 1) {
				if (meshList.Count == 0) {
					#if UNITY_EDITOR
					EditorUtility.DisplayDialog ("Super Combiner", "Zero meshes found.\nUnable to proceed without at least 1 mesh.", "Ok");
					#endif
					UnCombine ();
				}
				return;
			}
		}
		if (combineMaterials) {
			if (texturePacker.materialList == null) {
				return;
			} else if(texturePacker.materialList.Count < 1) {
				#if UNITY_EDITOR
				EditorUtility.DisplayDialog ("Super Combiner", "You need at least 1 material to proceed combine", "Ok");
				#endif
				UnCombine ();
				return;
			}
		}

		#if UNITY_EDITOR
		// UI Progress bar display in Editor
		EditorUtility.DisplayProgressBar("Super Combiner", "Packing textures...", 0f);
		#endif

		// Pack the textures
		texturePacker.PackTextures (textureAtlasSize, combineMaterials, sessionName);

		// Create the parent Game object
		modifiedParent = new GameObject (sessionName);
		modifiedParent.transform.parent = this.transform;
		modifiedParent.transform.localPosition = Vector3.zero;

		#if UNITY_EDITOR
		// UI Progress bar display in Editor
		EditorUtility.DisplayProgressBar("Super Combiner", "Creating combined GameObjects and generating UVs...", 0.1f);
		#endif

		// Parametrize MeshCombiner
		meshCombiner.SetParameters(maxVerticesCount, texturePacker, sessionName);

		// Combine process
		if (combineMeshes) {
			modifiedObjectList = meshCombiner.GenerateCombinedTransformedGameObjects (modifiedParent.transform, originalMeshesList, originalSkinnedMeshesList, (MeshOutput) meshOutput);
			if (modifiedObjectList.Count == 0) {
				UnCombine ();
				return;
			}
			foreach (GameObject go in modifiedObjectList) {
				// Add the copy mesh instanceId with its original sharedMesh and sharedMaterial instanceId
				if ((MeshOutput) meshOutput == MeshOutput.Mesh) {
					originalMeshMaterialId.Add (go.GetComponent<MeshFilter> ().sharedMesh.GetInstanceID (), go.name);
				} else if ((MeshOutput) meshOutput == MeshOutput.SkinnedMesh) { 
					originalMeshMaterialId.Add (go.GetComponent<SkinnedMeshRenderer> ().sharedMesh.GetInstanceID (), go.name);
				}
			}
		} else {
			// Create a copy of all game objects children of this one
			CopyGameObjectsHierarchy ();
			List<MeshRenderer> copyMeshList = FindEnabledMeshes (modifiedParent.transform, false);
			List<SkinnedMeshRenderer> copySkinnedMeshList = FindEnabledSkinnedMeshes (modifiedParent.transform, false);
			modifiedObjectList = GenerateTransformedGameObjects (modifiedParent.transform, copyMeshList);
			modifiedSkinnedObjectList = GenerateTransformedGameObjects (modifiedParent.transform, copySkinnedMeshList);

			// Generate UVs for simple meshes
			for (int i=0; i<modifiedObjectList.Count; i++) {
				GenerateUVs(modifiedObjectList [i].GetComponent<MeshFilter> ().sharedMesh, originalObjectList [i].GetComponent<Renderer>().sharedMaterials, originalObjectList [i].name);
			}
			// Generate UVs for skinned meshes
			for (int i=0; i<modifiedSkinnedObjectList.Count; i++) {
				GenerateUVs(modifiedSkinnedObjectList [i].GetComponent<SkinnedMeshRenderer>().sharedMesh, originalObjectList [modifiedObjectList.Count+i].GetComponent<SkinnedMeshRenderer>().sharedMaterials, originalObjectList [modifiedObjectList.Count+i].name);
			}
		}

		#if UNITY_EDITOR
		// Unwrapp UV so that lightmapping still works
		for (int i = 0; i < modifiedObjectList.Count; i++) {
			// Cancelable UI Progress bar display in Editor
			bool cancel = EditorUtility.DisplayCancelableProgressBar("Super Combiner", "Creating combined GameObjects and generating UVs...", 0.2f + 0.8f * (i/(float)modifiedObjectList.Count));
			if(cancel) {
				UnCombine();
				return;
			}
			if(!combineMeshes || (MeshOutput) meshOutput == MeshOutput.Mesh) {
				//Unwrapping.GenerateSecondaryUVSet (modifiedObjectList[i].GetComponent<MeshFilter> ().sharedMesh);
			}
		}
		#endif

		// Deactivate original renderers
		DisableRenderers (originalObjectList);

		#if UNITY_EDITOR
		// Combine process is finished
		EditorUtility.ClearProgressBar();

		if (folderDestination == "") {
			// Default export folder destination
			folderDestination = "Assets/SuperCombiner/Combined";
		}
		#endif

		combiningState = CombineStatesList.Combined;

		TimeSpan duration = DateTime.Now - timeStart;
		Debug.Log ("[Super Combiner] Successfully combined game objects!\nExecution time is " + duration);
	}

	// Copy all GameObjects children
	private void CopyGameObjectsHierarchy() {
		Transform[] children = this.transform.GetComponentsInChildren<Transform>();

		foreach (Transform child in children) {
			if (child.parent == this.transform && child != modifiedParent.transform) {
				GameObject go = InstantiateCopy (child.gameObject, false);
				go.transform.SetParent (modifiedParent.transform);
			}
		}
	}

	// Generate the new uvs of the mesh in texture atlas
	private void GenerateUVs(Mesh mesh, Material[] materials, string objectName) {
		int[] textureIndexes = new int[materials.Length];
		
		for (int j = 0; j < materials.Length; j++) {
			Material mat = materials [j];
			textureIndexes [j] = texturePacker.FindCorrespondingMaterialIndex ( mat);
		}
		
		if (!meshCombiner.GenerateUV (mesh, textureIndexes, objectName, materials)) {
			UnCombine();
			return;
		}
	}

	// Reactivate original GameObjects
	private void EnableRenderers(List<GameObject> gameObjects)
	{
		foreach (GameObject go in gameObjects)
		{
			if (go != null) {
				go.gameObject.SetActive (true);
				go.GetComponent<Renderer> ().enabled = true;
			}
		}
	}

	// Deactivate original GameObjects
	private void DisableRenderers(List<GameObject> gameObjects)
	{
		foreach (GameObject go in gameObjects)
		{
			go.GetComponent<Renderer>().enabled = false;
			go.gameObject.SetActive(false);
		}
	}

	// Generate the new transformed gameobjects and apply new materials to them, when no combining meshes
	private List<GameObject> GenerateTransformedGameObjects(Transform parent, List<MeshRenderer> objects)
	{
		List<GameObject> copyList = new List<GameObject> ();

		for(int i=0; i<objects.Count; i++) {
			// Copy the new mesh to the created GameObject copy
			Mesh copyOfMesh = meshCombiner.copyMesh(objects[i].GetComponent<MeshFilter> ().sharedMesh);

			// Add the copy mesh instanceId with its original sharedMesh and sharedMaterial instanceId
			if (objects [i].GetComponent<Renderer> ().sharedMaterial != null) {
				originalMeshMaterialId.Add (copyOfMesh.GetInstanceID (), objects [i].GetComponent<MeshFilter> ().sharedMesh.GetInstanceID ().ToString () + objects [i].GetComponent<Renderer> ().sharedMaterial.GetInstanceID ().ToString () + copyOfMesh.name);
			} else {
				originalMeshMaterialId.Add (copyOfMesh.GetInstanceID (), objects [i].GetComponent<MeshFilter> ().sharedMesh.GetInstanceID ().ToString ()  + copyOfMesh.name);
			}
			copyMeshId[objects[i].GetComponent<MeshFilter> ().sharedMesh.GetInstanceID()] = originalMeshMaterialId[copyOfMesh.GetInstanceID ()];
			objects[i].GetComponent<MeshFilter> ().sharedMesh = copyOfMesh;

			// Assign new materials
			if(combineMaterials) {
				Material mat = objects [i].GetComponent<Renderer> ().sharedMaterial;
				Material[] newMats = new Material[objects [i].GetComponent<Renderer> ().sharedMaterials.Length];
				for (int k = 0; k < newMats.Length; k++) {
					newMats [k] = texturePacker.getCombinedMaterialValue ();
				}
				objects[i].GetComponent<Renderer>().sharedMaterials = newMats;
				// Find corresponding material
				combinedTextureIndex.Add (texturePacker.FindCorrespondingMaterialIndex(mat));
			}
			else {
				Material[] mat = objects [i].GetComponent<Renderer> ().sharedMaterials;
				Material[] newMats = new Material[mat.Length];
				for (int a = 0; a < mat.Length; a++) {
					newMats [a] = texturePacker.getTransformedMaterialValue (objects [i].GetComponent<Renderer> ().sharedMaterials [a].name);
					// Find corresponding material
					combinedTextureIndex.Add (texturePacker.FindCorrespondingMaterialIndex(mat[a]));
				}
				objects[i].GetComponent<Renderer> ().sharedMaterials = newMats;
			}

			copyList.Add(objects[i].gameObject);
		}

		return copyList;
	}

	// For Skinned Mesh renderers, when no combining meshes
	private List<GameObject> GenerateTransformedGameObjects(Transform parent, List<SkinnedMeshRenderer> skinnedObjects) 
	{
		List<GameObject> copyList = new List<GameObject> ();

		for(int i=0; i<skinnedObjects.Count; i++) {
			// Copy the new mesh to the created GameObject copy
			Mesh copyOfMesh = meshCombiner.copyMesh(skinnedObjects[i].GetComponent<SkinnedMeshRenderer> ().sharedMesh);

			// Add the copy mesh instanceId with its original sharedMesh and sharedMaterial instanceId
			if (skinnedObjects [i].GetComponent<Renderer> ().sharedMaterial != null) {
				originalMeshMaterialId.Add (copyOfMesh.GetInstanceID (), skinnedObjects [i].GetComponent<SkinnedMeshRenderer> ().sharedMesh.GetInstanceID ().ToString () + skinnedObjects [i].GetComponent<Renderer> ().sharedMaterial.GetInstanceID ().ToString () + copyOfMesh.name);
			} else {
				originalMeshMaterialId.Add (copyOfMesh.GetInstanceID (), skinnedObjects [i].GetComponent<SkinnedMeshRenderer> ().sharedMesh.GetInstanceID ().ToString () + copyOfMesh.name);
			}
			copyMeshId[skinnedObjects[i].GetComponent<SkinnedMeshRenderer> ().sharedMesh.GetInstanceID()] = originalMeshMaterialId[copyOfMesh.GetInstanceID ()];
			skinnedObjects[i].GetComponent<SkinnedMeshRenderer>  ().sharedMesh = copyOfMesh;
			
			// Assign new materials
			if(combineMaterials) {
				Material mat = skinnedObjects [i].sharedMaterial;
				Material[] newMats = new Material[skinnedObjects [i].sharedMaterials.Length];
				for (int k = 0; k < newMats.Length; k++) {
					newMats [k] = texturePacker.getCombinedMaterialValue ();
				}
				skinnedObjects[i].GetComponent<SkinnedMeshRenderer>().sharedMaterials = newMats;
				// Find corresponding material
				combinedTextureIndex.Add (texturePacker.FindCorrespondingMaterialIndex(mat));
			}
			else {
				Material[] mat = skinnedObjects [i].sharedMaterials;
				Material[] newMats = new Material[mat.Length];
				for (int a = 0; a < mat.Length; a++) {
					newMats [a] = texturePacker.getTransformedMaterialValue (skinnedObjects [i].sharedMaterials [a].name);
					// Find corresponding material
					combinedTextureIndex.Add (texturePacker.FindCorrespondingMaterialIndex(mat[a]));
				}
				skinnedObjects[i].GetComponent<SkinnedMeshRenderer> ().sharedMaterials = newMats;
			}

			copyList.Add(skinnedObjects[i].gameObject);
		}
		
		return copyList;
	}

	// Instantiate a copy of the GameObject, keeping it's transform values identical
	private GameObject InstantiateCopy(GameObject original, bool deleteChidren = true) {
		GameObject copy = Instantiate(original) as GameObject;
		copy.transform.parent = original.transform.parent;
		copy.transform.localPosition = original.transform.localPosition;
		copy.transform.localRotation = original.transform.localRotation;
		copy.transform.localScale = original.transform.localScale;
		copy.name = original.name;

		if (deleteChidren) {
			// Delete all children
			foreach (Transform child in copy.transform) {
				DestroyImmediate (child.gameObject);
			}
		}

		return copy;
	}

	// Find all enabled mesh colliders
	private List<MeshCollider> FindEnabledMeshColliders(Transform parent) {
		MeshCollider[] colliders;
		colliders = parent.GetComponentsInChildren<MeshCollider> ();

		List<MeshCollider> meshColliders = new List<MeshCollider> ();
		foreach (MeshCollider collider in colliders) {
			if (collider.sharedMesh != null) {
				meshColliders.Add (collider);
			}
		}

		return meshColliders;
	}

	// Find and store all enabled meshes
	private List<MeshRenderer> FindEnabledMeshes(Transform parent, bool contributeGlobal)
	{
		MeshFilter[] filters;
		filters = parent.GetComponentsInChildren<MeshFilter>();

		List<MeshRenderer> meshRendererList = new List<MeshRenderer>();

		foreach (MeshFilter filter in filters) {
			if (filter.sharedMesh != null) {
				MeshRenderer renderer = filter.GetComponent<MeshRenderer> ();
				if (renderer != null && renderer.enabled && renderer.sharedMaterials.Length > 0) {
					meshRendererList.Add (renderer);
					if (contributeGlobal) {
						originalObjectList.Add (renderer.gameObject);

						if (combineMeshes) {
							originalMeshesList.Add (filter);
						}
					}
				}
			}
		}

		return meshRendererList;
	}

	// Find and store all enabled skin meshes
	private List<SkinnedMeshRenderer> FindEnabledSkinnedMeshes(Transform parent, bool contributeGlobal)
	{
		// Skinned meshes
		SkinnedMeshRenderer[] skinnedMeshes = parent.GetComponentsInChildren<SkinnedMeshRenderer>();

		List<SkinnedMeshRenderer> skinnedMeshRendererList = new List<SkinnedMeshRenderer>();

		foreach (SkinnedMeshRenderer skin in skinnedMeshes) {
			if(skin.sharedMesh != null) {
				if(skin.enabled && skin.sharedMaterials.Length > 0) {
					skinnedMeshRendererList.Add(skin);
					if (contributeGlobal) {
						originalObjectList.Add (skin.gameObject);

						if (combineMeshes) {
							originalSkinnedMeshesList.Add (skin);
						}
					}
				}
			}
		}

		return skinnedMeshRendererList;
	}

	// Find and store all enabled materials
	private List<Material> FindEnabledMaterials(List<MeshRenderer> meshes, List<SkinnedMeshRenderer> skinnedMeshes)
	{
		materialUVBounds = new List<Rect> ();
		List<Material> matList = new List<Material> ();
		Dictionary<int, Material> materialsDictionnary = new Dictionary<int, Material> ();
		Dictionary<int, Rect> uvBoundsDictionnary = new Dictionary<int, Rect> ();

		// Meshes renderer
		foreach (MeshRenderer mesh in meshes) {
			Rect uvBound = getUVBounds(mesh.GetComponent<MeshFilter> ().sharedMesh.uv);

			foreach (Material material in mesh.sharedMaterials) {
				if (material != null) {
					int instanceId = material.GetInstanceID ();

					if (!materialsDictionnary.ContainsKey (instanceId)) {
						materialsDictionnary.Add (instanceId, material);
						matList.Add (material);
						uvBoundsDictionnary.Add(instanceId, uvBound);
					} else {
						Rect outRect = new Rect();
						uvBoundsDictionnary.TryGetValue(instanceId, out outRect);
						Rect maxRect = getMaxRect(outRect, uvBound);
						uvBoundsDictionnary[instanceId] = maxRect;
					}
				}
			}
		}

		// SkinnedMeshes renderer
		foreach (SkinnedMeshRenderer skinnedMesh in skinnedMeshes) {
			Rect uvBound = getUVBounds(skinnedMesh.sharedMesh.uv);

			foreach (Material material in skinnedMesh.sharedMaterials) {
				if (material != null) {
					int instanceId = material.GetInstanceID ();
					
					if (!materialsDictionnary.ContainsKey (instanceId)) {
						materialsDictionnary.Add (instanceId, material);
						matList.Add (material);
						uvBoundsDictionnary.Add(instanceId, uvBound);
					} else {
						Rect outRect = new Rect();
						uvBoundsDictionnary.TryGetValue(instanceId, out outRect);
						Rect maxRect = getMaxRect(outRect, uvBound);
						uvBoundsDictionnary[instanceId] = maxRect;
					}
				}
			}
		}

		int progressCount = 0;
		foreach (Material mat in materialsDictionnary.Values) {
			Rect uvBound;
			uvBoundsDictionnary.TryGetValue(mat.GetInstanceID(), out uvBound);
			Rect currentUVBound = new Rect (uvBound);
			if (mat.HasProperty("_MainTex")) {
				currentUVBound.size = Vector2.Scale (currentUVBound.size, mat.mainTextureScale);
				currentUVBound.position += mat.mainTextureOffset;
			}

			#if UNITY_EDITOR
			// Cancelable  UI Progress bar display in Editor
			bool cancel = EditorUtility.DisplayCancelableProgressBar("Super Combiner", "Processing material " + mat.name, progressCount / (float) materialsDictionnary.Values.Count);
			if(cancel) {
				UnCombine();
				return null;
			}
			#endif
			// Add all textures from this material on the list of textures
			texturePacker.SetTextures (mat, combineMaterials, currentUVBound, uvBound);

			if (!mat.HasProperty ("_MainTex") || mat.mainTexture == null) {
				// Correction of uv for mesh without diffuse texture
				uvBound.size = Vector2.Scale (uvBound.size, new Vector2 (1.2f, 1.2f));
				uvBound.position -= new Vector2 (0.1f, 0.1f);
			}
			materialUVBounds.Add (currentUVBound);
			meshCombiner.AddMeshUVBound (uvBound);
			progressCount++;
		}

		return matList;
	}

	// Return the bound of the uv list (min, max for x and y axis)
	private Rect getUVBounds(Vector2[] uvs) {
		Rect uvBound = new Rect (0, 0, 1, 1);
		for (int i = 0; i < uvs.Length; i++) {
			if (uvs [i].x < 0 && uvs[i].x < uvBound.xMin) {
				uvBound.xMin = uvs [i].x;
			}
			if (uvs [i].x > 1 && uvs [i].x > uvBound.xMax) {
				uvBound.xMax = uvs [i].x;
			}
			if (uvs [i].y < 0 && uvs[i].y < uvBound.yMin) {
				uvBound.yMin = uvs [i].y;
			}
			if (uvs [i].y > 1 && uvs [i].y > uvBound.yMax) {
				uvBound.yMax = uvs [i].y;
			}
		}
		return uvBound;
	}

	// Return the maximum rect based on the two rect parameters
	private Rect getMaxRect(Rect uv1, Rect uv2) {
		Rect newRect = new Rect();
		newRect.xMin = Math.Min (uv1.xMin, uv2.xMin);
		newRect.yMin = Math.Min (uv1.yMin, uv2.yMin);
		newRect.xMax= Math.Max (uv1.xMax, uv2.xMax);
		newRect.yMax = Math.Max (uv1.yMax, uv2.yMax);
		return newRect;
	}

	// Reverse combine process, destroy all created objects and reactivate original mesh renderers
	public void UnCombine()
	{
		#if UNITY_EDITOR
		// Hide progressbar
		EditorUtility.ClearProgressBar();
		#endif

		// Reactivate original renderers
		EnableRenderers (originalObjectList);

		// Clear the packed textures
		texturePacker.ClearTextures ();
		meshCombiner.Clear ();
		meshList.Clear ();
		skinnedMeshList.Clear ();
		materialUVBounds.Clear ();
		originalMeshMaterialId.Clear ();
		copyMeshId.Clear ();
		originalObjectList.Clear ();
		modifiedObjectList.Clear ();
		modifiedSkinnedObjectList.Clear ();
		combinedTextureIndex.Clear ();
		originalMeshesList.Clear ();
		originalSkinnedMeshesList.Clear ();
		toSavePrefabList.Clear ();
		toSaveObjectList.Clear ();
		toSaveMeshList.Clear ();
		toSaveSkinnedObjectList.Clear ();

		DestroyImmediate (modifiedParent);
		combiningState = CombineStatesList.Uncombined;

		Debug.Log ("[Super Combiner] Successfully uncombined game objects.");
	}

	// Get the first children list of the parents
	private List<Transform> getFirstChildren(Transform parent) {
		List<Transform> children = new List<Transform>();
		for(int i=0; i<parent.transform.childCount; i++) {
			children.Add(parent.transform.GetChild(i));
		}
		return children;
	}

	// Save combined objects
	public void Save()
	{
		#if UNITY_EDITOR
		// Check if destination folder exists
		if (!Directory.Exists (folderDestination)) {
			Directory.CreateDirectory(folderDestination);
		}

		// Generate new instances (copy from modifiedObjectList) to be saved, so that objects in modifiedObjectList won't be affected by user's modification/deletion
		toSavePrefabList.Clear();
		toSaveObjectList.Clear ();
		toSaveMeshList.Clear ();
		toSaveSkinnedObjectList.Clear ();
		texturePacker.GenerateCopyedMaterialToSave ();

		if (texturePacker.getCombinedMaterialToSave () == null) {
			Debug.LogError ("Instance of combined material has been lost, try to combine again before saving.");
		} else {
			// We need to know if the combined material has already been saved
			savedMaterial = AssetDatabase.LoadAssetAtPath<Material> (folderDestination + "/Materials/" + texturePacker.copyedMaterials[0].name + ".mat");
		}

		// List of all different meshes found on evry game objects to save, with no duplication
		Dictionary<string, Mesh> meshMaterialId = new Dictionary<string, Mesh> ();

		bool outputSkinnedMesh = false;
		List<Transform> children = new List<Transform>();
		if(combineMeshes && (MeshOutput) meshOutput == MeshOutput.SkinnedMesh) {
			// If output is skinnedMesh, we save a copy of modifiedParent as prefab
			GameObject copy = InstantiateCopy(modifiedParent, false);
			toSavePrefabList.Add(copy);
			outputSkinnedMesh = true;
			children = getFirstChildren(copy.transform);
		} else {
			children = getFirstChildren(modifiedParent.transform);
		}

		// Generate copy of game objects to be saved
		foreach (Transform child in children) {
			GameObject copy;
			if(outputSkinnedMesh) {
				copy = child.gameObject;
			} else {
				copy = InstantiateCopy(child.gameObject, false);
			}

			List<MeshRenderer> meshes = FindEnabledMeshes (copy.transform, false);
			List<SkinnedMeshRenderer> skinnedMeshes = FindEnabledSkinnedMeshes (copy.transform, false);
			List<MeshCollider> meshColliders = FindEnabledMeshColliders(copy.transform);
				
			// Create a copy of mesh
			foreach (MeshRenderer mesh in meshes) {
				int instanceId = mesh.GetComponent<MeshFilter> ().sharedMesh.GetInstanceID ();

				if (meshMaterialId.ContainsKey (originalMeshMaterialId [instanceId])) {
					// This mesh is shared with other game objects, so we reuse the first instance to avoid duplication
					mesh.GetComponent<MeshFilter> ().sharedMesh = meshMaterialId[originalMeshMaterialId [instanceId]];
				} else {
					Mesh copyOfMesh = meshCombiner.copyMesh(mesh.GetComponent<MeshFilter> ().sharedMesh);
					mesh.GetComponent<MeshFilter> ().sharedMesh = copyOfMesh;
					meshMaterialId.Add (originalMeshMaterialId [instanceId], copyOfMesh);
					toSaveMeshList.Add (copyOfMesh);
				}

				// Apply a copy of the material to save
				Material[] newMat = new Material[mesh.sharedMaterials.Length];
				for(int j=0; j<mesh.sharedMaterials.Length; j++) {
					if(combineMaterials) {
						if (savedMaterial != null) {
							// If the combined material already exists, assign it
							newMat [j] = savedMaterial;
						} else {
							newMat [j] = texturePacker.getCombinedMaterialToSave (); 
						}
					} else {
						newMat[j] = texturePacker.getTransformedMaterialToSave(mesh.sharedMaterials[j].name);
					}
				}

				mesh.sharedMaterials = newMat;
				toSaveObjectList.Add (mesh);
			}
			foreach (SkinnedMeshRenderer skinnedmesh in skinnedMeshes) {
				int instanceId = skinnedmesh.sharedMesh.GetInstanceID ();

				if (meshMaterialId.ContainsKey (originalMeshMaterialId [instanceId])) {
					// This mesh is shared with other game objects, so we reuse the first instance to avoid duplication
					skinnedmesh.sharedMesh = meshMaterialId[originalMeshMaterialId [instanceId]];
				} else {
					Mesh copyOfMesh = meshCombiner.copyMesh(skinnedmesh.sharedMesh);
					skinnedmesh.sharedMesh = copyOfMesh;
					meshMaterialId.Add (originalMeshMaterialId [instanceId], copyOfMesh);
					toSaveMeshList.Add (copyOfMesh);
				}

				// Apply a copy of the material to save
				Material[] newMat = new Material[skinnedmesh.sharedMaterials.Length];
				for(int j=0; j<skinnedmesh.sharedMaterials.Length; j++) {
					if(combineMaterials) {
						if (savedMaterial != null) {
							// If the combined material already exists, assign it
							newMat [j] = savedMaterial;
						} else {
							newMat [j] = texturePacker.getCombinedMaterialToSave ();
						}
					} else {
						newMat[j] = texturePacker.getTransformedMaterialToSave(skinnedmesh.sharedMaterials[j].name);
					}
				}
				skinnedmesh.sharedMaterials = newMat;
				toSaveSkinnedObjectList.Add (skinnedmesh);
			}

			// Assign to mesh colliders the mesh that will be saved
			foreach (MeshCollider collider in meshColliders) {
				int instanceId = collider.sharedMesh.GetInstanceID ();

				string id = null;
				copyMeshId.TryGetValue (instanceId, out id);
				if (id != null) {
					if (meshMaterialId.ContainsKey (id)) {
						collider.sharedMesh = meshMaterialId [id];
					}
				} else {
					// This means the collider has a mesh that is not present in the combine list
					// In this case, keep the meshCollider component intact
				}
			}

			// Add this GameObject to the list of prefab to save
			if(!outputSkinnedMesh) {
				toSavePrefabList.Add (copy);
			}
		}

		// Saving process
		if (saveTextures) {
			SaveTextures();
		}
		if (saveMaterials) {
			SaveMaterials();
		}
		if (savePrefabs) {
			SavePrefabs();
		}
		if (saveMeshObj) {
			SaveMeshesObj();
		}
		if (saveMeshFbx) {
			SaveMeshesFbx();
		}

		// Delete the copies created for saving process from memory
		for (int i = 0; i < toSaveObjectList.Count; i++) {
			DestroyImmediate (toSaveObjectList [i]);
		}
		for (int i = 0; i < toSaveSkinnedObjectList.Count; i++) {
			DestroyImmediate (toSaveSkinnedObjectList [i]);
		}
		for (int i = 0; i < toSavePrefabList.Count; i++) {
			DestroyImmediate (toSavePrefabList [i]);
		}
			
		EditorUtility.DisplayDialog("Super Combiner", "Objects saved in '" + folderDestination + "/' \n\nThanks for using Super Combiner.", "Ok");

		// Hide progressbar
		EditorUtility.ClearProgressBar();
		#endif
	}

	#if UNITY_EDITOR
	// Save textures
	public void SaveTextures()
	{
		// UI Progress bar display in Editor
		EditorUtility.DisplayProgressBar("Super Combiner", "Saving textures ...", 0.3f);

		if (!Directory.Exists (folderDestination + "/Textures")) {
			Directory.CreateDirectory(folderDestination + "/Textures");
		}

		texturePacker.SaveTextures (folderDestination, sessionName);

		Debug.Log ("[Super Combiner] Textures saved in '" + folderDestination + "/Textures/'");
	}

	// Save materials
	public void SaveMaterials()
	{
		// UI Progress bar display in Editor
		EditorUtility.DisplayProgressBar("Super Combiner", "Saving materials ...", 0.6f);

		if (!Directory.Exists (folderDestination + "/Materials")) {
			Directory.CreateDirectory(folderDestination + "/Materials");
		}

		for (int i=0; i<texturePacker.copyedMaterials.Count; i++) {	
			savedMaterial = AssetDatabase.LoadAssetAtPath<Material> (folderDestination + "/Materials/" + texturePacker.copyedMaterials[i].name + ".mat");

			if (savedMaterial == null) {
				AssetDatabase.CreateAsset (texturePacker.copyedToSaveMaterials [i], folderDestination + "/Materials/" + texturePacker.copyedMaterials [i].name + ".mat");
				AssetDatabase.SaveAssets ();
				AssetDatabase.Refresh ();
			} else {
				EditorUtility.CopySerialized (texturePacker.copyedToSaveMaterials [i], savedMaterial);
			}

			Material material = (Material)(AssetDatabase.LoadAssetAtPath (folderDestination + "/Materials/" + texturePacker.copyedMaterials [i].name + ".mat", typeof(Material)));
			foreach (KeyValuePair<string, Texture2D> keyValue in texturePacker.packedTextures) {
				if (keyValue.Value != null) {
					string textureName = keyValue.Key;
					texturePacker.TexturePropertyNames.TryGetValue (keyValue.Key, out textureName);
					material.SetTexture (keyValue.Key, (Texture2D)(AssetDatabase.LoadAssetAtPath (folderDestination + "/Textures/" + sessionName + "_" + textureName + ".png", typeof(Texture2D)))); 
				}
			}
		}
		Debug.Log ("[Super Combiner] Materials saved in '" + folderDestination + "/Materials/'");
	}

	// Save prefabs
	public void SavePrefabs()
	{
		// UI Progress bar display in Editor
		EditorUtility.DisplayProgressBar("Super Combiner", "Saving Prefabs ...", 0.7f);

		if (!Directory.Exists (folderDestination + "/Prefabs")) {
			Directory.CreateDirectory(folderDestination + "/Prefabs");
		}

		// We have to save meshes first
		SaveMeshes ();

		for (int i=0; i<toSavePrefabList.Count; i++) {
			if (AssetDatabase.LoadAssetAtPath<GameObject> (folderDestination + "/Prefabs/" + sessionName + "_" + toSavePrefabList [i].name + ".prefab")) {			
				// Asset already exist! We just have to update it
				GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject> (folderDestination + "/Prefabs/" + sessionName + "_" + toSavePrefabList [i].name + ".prefab");

				asset.transform.position = toSavePrefabList [i].transform.position;
				asset.transform.localScale = toSavePrefabList [i].transform.localScale;
				asset.transform.rotation = toSavePrefabList [i].transform.rotation;

				AssetDatabase.SaveAssets ();
				AssetDatabase.Refresh ();
			} else {
				// This is a new asset, create it
				PrefabUtility.CreatePrefab(folderDestination + "/Prefabs/" + sessionName + "_" + toSavePrefabList[i].name + ".prefab", toSavePrefabList[i]);
			}
		}

		Debug.Log ("[Super Combiner] Prefabs saved in '" + folderDestination + "/Prefabs/'");
	}

	// Save meshes
	public void SaveMeshes()
	{
		// UI Progress bar display in Editor
		EditorUtility.DisplayProgressBar("Super Combiner", "Saving Meshes ...", 0.75f);

		if (!Directory.Exists (folderDestination + "/Meshes")) {
			Directory.CreateDirectory(folderDestination + "/Meshes");
		}

		// Check if all meshes have different name. This is important not to override a previously saved mesh
		HashSet<string> meshNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		for (int i = 0; i < toSaveMeshList.Count; i++) {
			if (!meshNames.Contains (toSaveMeshList [i].name)) {
				meshNames.Add (toSaveMeshList [i].name);
			} else {
				// A mesh with the same name has been found, rename it
				for (int n = 0; n < 9999; n++) {
					if (!meshNames.Contains (toSaveMeshList [i].name + "(" + n + ")")) {
						meshNames.Add (toSaveMeshList [i].name + "(" + n + ")");
						toSaveMeshList [i].name = toSaveMeshList [i].name + "(" + n + ")";
						break;
					}
				}
			}
		}

		for (int i = 0; i < toSaveMeshList.Count; i++) {
			Mesh dummy = AssetDatabase.LoadAssetAtPath<Mesh> (folderDestination + "/Meshes/" + sessionName + "_" + toSaveMeshList [i].name + ".asset");
			if (dummy == null) {
				// This is a new mesh to create
				AssetDatabase.CreateAsset (toSaveMeshList [i], folderDestination + "/Meshes/" + sessionName + "_" + toSaveMeshList [i].name + ".asset");
			} else {
				// The mesh already exists, just uptade it
				dummy.Clear ();
				EditorUtility.CopySerialized (toSaveMeshList [i], dummy);
			}
		}

		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();

		Debug.Log ("[Super Combiner] Meshes saved in '" + folderDestination + "/Meshes/'");
	}

	// Save .obj
	public void SaveMeshesObj()
	{
		// UI Progress bar display in Editor
		EditorUtility.DisplayProgressBar ("Super Combiner", "Saving obj ...", 0.8f);

		if (!Directory.Exists (folderDestination + "/Objs")) {
			Directory.CreateDirectory(folderDestination + "/Objs");
		}

		for (int i = 0; i < modifiedObjectList.Count; i++) {			
			ObjSaver.SaveObjFile (modifiedObjectList[i], false, folderDestination + "/Objs");
		}
		for (int i = 0; i < modifiedSkinnedObjectList.Count; i++) {
			ObjSaver.SaveObjFile (modifiedSkinnedObjectList[i], false, folderDestination + "/Objs");
		}
	}

	// Save .fbx
	public void SaveMeshesFbx()
	{
		// UI Progress bar display in Editor
		EditorUtility.DisplayProgressBar ("Super Combiner", "Saving fbx ...", 0.9f);

		if (!Directory.Exists (folderDestination + "/Fbx")) {
			Directory.CreateDirectory(folderDestination + "/Fbx");
		}

		// TODO : save mesh to fbx !
	}		
	#endif
}