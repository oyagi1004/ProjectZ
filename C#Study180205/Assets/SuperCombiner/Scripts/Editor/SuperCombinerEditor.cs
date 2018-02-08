using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LunarCatsStudio.SuperCombiner;

public enum MeshOutput {Mesh, SkinnedMesh}
[CustomEditor(typeof(SuperCombiner))]
public class SuperCombinerEditor : Editor {

	public enum CombineStatesList {Uncombined, Combining, Combined}

	// Constants
	private const int MIN_VERITCES_COUNT = 32;
	private const int MAX_VERITCES_COUNT = 65534;
	private const string VERSION_NUMBER = "1.4.1";

	// Reference to the SuperCombiner script
	private SuperCombiner SuperCombiner;

	// Editor parameters
	private bool showOriginalMaterials = false;
	private bool showCombinedMaterials = false;

	// Saving options
	private bool savingParametersFoldout = false;
	private bool foldout = true;

	private void OnEnable()
	{
		this.SuperCombiner = (SuperCombiner)target;
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.Space();

		// Disable fields when combine is finished
		if ((CombineStatesList)this.SuperCombiner.combiningState == CombineStatesList.Uncombined) {
			GUI.enabled = true;
		} else {
			GUI.enabled = false;
		}

		GUILayout.Label ("*** Super combiner " + VERSION_NUMBER + " ***", EditorStyles.boldLabel);
		foldout = EditorGUILayout.Foldout (foldout, "Instructions");
		if (foldout) {
			GUILayout.Label ("Put all you prefabs to combine as children of me. " +
			"Select your session name, the texture atlas size and whether or not to combine meshes. " +
			"When you are ready click 'Combine' button to start the process (it may take a while depending on the quantity of different assets). " +
			"When the process is finished you'll see the result on the scene (all original mesh renderers will be deactivated). " +
			"If you want to save the combined assets, select your saving options and click 'Save' button. " +
			"To revert the process just click 'Uncombine' button.", EditorStyles.wordWrappedLabel);
		}
		////
		/// Defines parameters
		/// 

		EditorGUILayout.Space();
		this.SuperCombiner.sessionName = EditorGUILayout.TextField ("Session name", this.SuperCombiner.sessionName, GUILayout.ExpandWidth (true));
		this.SuperCombiner.combineAtRuntime = EditorGUILayout.Toggle (new GUIContent("Combine at runtime?", "Set to true if you want the process to combine at startup during runtime (beware that combining is a complex task that may takes some time to process)"), this.SuperCombiner.combineAtRuntime);

		GUILayout.Label ("Texture Atlas", EditorStyles.boldLabel);
		GUILayout.Label ("The first material found in all game objects to combine will be used as a reference for the combined material.", EditorStyles.wordWrappedMiniLabel);

		if (this.SuperCombiner.combineMaterials) {
			// Atlas Texture Size choice
			this.SuperCombiner.textureAtlasSize = EditorGUILayout.IntPopup ("Texture Atlas size", this.SuperCombiner.textureAtlasSize, this.SuperCombiner.TextureAtlasSizesNames.ToArray(), this.SuperCombiner.TextureAtlasSizes.ToArray(), GUILayout.ExpandWidth (true));

			// Combine meshes ?
			GUILayout.Label ("Meshes", EditorStyles.boldLabel);
			this.SuperCombiner.combineMeshes = EditorGUILayout.Toggle (new GUIContent("Combine meshes?", "If set to false, only materials and textures will be combined, all meshes will remain separated. If set to true, all meshes will be combined into a unique combined mesh."), this.SuperCombiner.combineMeshes);
			if(this.SuperCombiner.combineMeshes) {
				this.SuperCombiner.meshOutput = EditorGUILayout.IntPopup (new GUIContent("Mesh output", "Chose to combine into a Mesh or a SkinnedMesh. Combining into SkinnedMesh is in alpha release, it will only works properly if there are only SkinnedMeshes as input. Combining Meshes and SkinnedMeshes into a SkinnedMesh is not supported yet."), this.SuperCombiner.meshOutput, new GUIContent[] {new GUIContent ("Mesh"), new GUIContent ("SkinnedMesh (alpha)")}, new int[] {0, 1}, GUILayout.ExpandWidth (true));
				this.SuperCombiner.maxVerticesCount = EditorGUILayout.IntSlider (new GUIContent("Max vertices count", "If the combined mesh has more vertices than this parameter, it will be split into various meshes with 'Max vertices count' vertices"), this.SuperCombiner.maxVerticesCount, MIN_VERITCES_COUNT, MAX_VERITCES_COUNT, GUILayout.ExpandWidth (true));
			}
		}

		EditorGUILayout.Space();
		GUI.enabled = true;

		if ((CombineStatesList)this.SuperCombiner.combiningState == CombineStatesList.Uncombined) {
			if (GUILayout.Button ("Combine", GUILayout.MinHeight (30))) {
				this.SuperCombiner.Combine ();
			}
		} else if ((CombineStatesList)this.SuperCombiner.combiningState == CombineStatesList.Combining) {
			EditorGUILayout.Space();
			if (GUILayout.Button("Uncombine", GUILayout.MinHeight(30))) {
				this.SuperCombiner.UnCombine();
			}
			Rect r = EditorGUILayout.BeginVertical ();
			EditorGUI.ProgressBar (r, 0.1f, "Combining in progress ... ");
			GUILayout.Space (20);
			EditorGUILayout.EndVertical ();
		} else {
			GUILayout.Label ("Combine results:", EditorStyles.boldLabel);
			GUILayout.Label ("Found " + SuperCombiner.meshList.Count + " different mesh(s)");
			if(SuperCombiner.skinnedMeshList.Count > 0) {
				GUILayout.Label ("Found " + SuperCombiner.skinnedMeshList.Count + " different skinned mesh(es)");
			}
			GUILayout.Label ("Found " + SuperCombiner.texturePacker.materialList.Count + " different material(s)");

			// Display Textures Atlas
			foreach(KeyValuePair<string, Texture2D> keyValue in this.SuperCombiner.texturePacker.packedTextures) {
				if (keyValue.Value != null) {
					string PropertyName = keyValue.Key;
					this.SuperCombiner.texturePacker.TexturePropertyNames.TryGetValue (keyValue.Key, out PropertyName);
					GUILayout.Label (PropertyName + " AtlasTexture preview:", EditorStyles.boldLabel);
					EditorGUILayout.ObjectField("", keyValue.Value, typeof (Texture), false);			
					EditorGUILayout.Space();
				}
			}

			// Display original materials
			showOriginalMaterials = EditorGUILayout.Foldout(showOriginalMaterials, "Original Material(s)");
			if (showOriginalMaterials)
			{
				for (int i=0; i<SuperCombiner.texturePacker.materialList.Count; i++) {
					EditorGUILayout.ObjectField ("Material n°" +  i + ":", SuperCombiner.texturePacker.materialList [i], typeof(Material), false);
				}
			}
			if(!Selection.activeTransform) {
				showOriginalMaterials = false;	
			}

			// Display created materials
			if(this.SuperCombiner.combineMaterials) {
				showCombinedMaterials = EditorGUILayout.Foldout(showCombinedMaterials, "Combined Material");
			} else {
				showCombinedMaterials = EditorGUILayout.Foldout(showCombinedMaterials, "Combined Materials");
			}

			if (showCombinedMaterials)
			{
				for (int i=0; i<SuperCombiner.texturePacker.copyedMaterials.Count; i++) {
					EditorGUILayout.ObjectField ("Material n°" +  i + ":", SuperCombiner.texturePacker.copyedMaterials [i], typeof(Material), false);
				}
			}
			if(!Selection.activeTransform) {
				showCombinedMaterials = false;	
			}
			EditorGUILayout.Space();

			// Display created meshes
			if (this.SuperCombiner.combineMeshes) {
				for (int i = 0; i < SuperCombiner.modifiedObjectList.Count; i++) {
					if (SuperCombiner.modifiedObjectList [i] != null) {
						if (this.SuperCombiner.meshOutput == 0) {
							EditorGUILayout.ObjectField ("Mesh n°" + i + ":", SuperCombiner.modifiedObjectList [i].GetComponent<MeshFilter> ().sharedMesh, typeof(MeshFilter), false);
						} else if (this.SuperCombiner.meshOutput == 1) {
							EditorGUILayout.ObjectField ("SkinnedMesh n°" + i + ":", SuperCombiner.modifiedObjectList [i].GetComponent<SkinnedMeshRenderer> ().sharedMesh, typeof(SkinnedMeshRenderer), false);
						}
					}
				}
			}

			// Saving settings
			savingParametersFoldout = EditorGUILayout.Foldout (savingParametersFoldout, "Saving settings");
			if(savingParametersFoldout) {

				this.SuperCombiner.saveMaterials = EditorGUILayout.Toggle ("Save materials", this.SuperCombiner.saveMaterials);
				this.SuperCombiner.saveTextures = EditorGUILayout.Toggle ("Save textures", this.SuperCombiner.saveTextures);
				this.SuperCombiner.savePrefabs = EditorGUILayout.Toggle ("Save prefabs", this.SuperCombiner.savePrefabs);
				this.SuperCombiner.saveMeshObj = EditorGUILayout.Toggle ("Save meshes as Obj", this.SuperCombiner.saveMeshObj);
				//this.SuperCombiner.saveMeshFbx = EditorGUILayout.Toggle ("Save meshes as Fbx", this.SuperCombiner.saveMeshFbx);

				GUILayout.Label ("Save in: " + this.SuperCombiner.folderDestination, EditorStyles.label);
				if (GUILayout.Button ("Change export folder", GUILayout.MinHeight (20))) {
					//this.SuperCombiner.folderDestination = EditorUtility.OpenFolderPanel("Destination Directory", "", "");
					string folderPath = EditorUtility.SaveFolderPanel("Destination Directory", "", "combined");
					string relativePath = folderPath.Substring(folderPath.IndexOf("Assets/"));
					this.SuperCombiner.folderDestination = relativePath;
				}
			}
			EditorGUILayout.Space();
			
			if (GUILayout.Button("Save", GUILayout.MinHeight(30))) {
				this.SuperCombiner.Save();
			}

			EditorGUILayout.Space();
			if (GUILayout.Button("Uncombine", GUILayout.MinHeight(30))) {
				this.SuperCombiner.UnCombine();
			}
		}
	}
}
