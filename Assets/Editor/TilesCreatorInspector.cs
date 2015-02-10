using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(TilesCreator)), CanEditMultipleObjects]
public class TilesCreatorInspector : Editor {


	private TilesCreator tc;

	public override void OnInspectorGUI () {
		serializedObject.Update();
		tc = (TilesCreator) serializedObject.targetObject;

		
		//Painting the GUI
		EditorList.Show(serializedObject.FindProperty("tiles"), EditorListOption.ListLabel | EditorListOption.Buttons);
		EditorList.Show(serializedObject.FindProperty("aliases"), EditorListOption.ListLabel | EditorListOption.Buttons);
//		EditorList.Show(serializedObject.FindProperty("teleporters"), EditorListOption.ListLabel | EditorListOption.Buttons);
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("teleporter"));
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("target"));
		tc.teleporterTag = EditorGUILayout.TextField("Teleporter Tag", tc.teleporterTag);
		tc.targetTag = EditorGUILayout.TextField("Target Tag", tc.targetTag);

		EditorGUILayout.PropertyField (serializedObject.FindProperty ("levelInput"));

		//Here's where all the magic happens
		if(GUILayout.Button("Build Object")){

			//Read the textfile
			string text = tc.levelInput.text;

			//Split it into rows
			string[] rows = text.Split("\n" [0]);

			//Get the size of the first tile - all tiles must have the same size, otherwise this won't look good
			Vector3 sizeOfFirstTile = tc.tiles[0].GetComponent<MeshFilter>().sharedMesh.bounds.extents;
			sizeOfFirstTile *= 2;

			//Create a reference position where everything begins
			Vector3 firstpos = tc.transform.position;


			//Prepare rowcounter
			int rowsAdded = 0;

			bool lastRowShifted = false;

			ArrayList teleportPositionArrayholder = new ArrayList();
			ArrayList targetPositionArrayholder = new ArrayList();

			
			//Loop over every single row
			foreach(string row in rows){

				//Get all characters of current row
				char[] chars = row.ToCharArray();

				float shift = 0.0f;

				int loopStart = 0;

				string firstChar = ""+chars[0];

				if(firstChar=="-"){
					shift = sizeOfFirstTile.x/2;
					loopStart = 1;
				}

				//Loop over every tuple
				for(int i=loopStart; i<chars.Length; i=i+2){


					//Get the Array Index of the current tuple
					string val = ""+chars[i]+chars[i+1];


					int index = System.Array.IndexOf(tc.aliases, val);

					//Making sure the tuple exists
					if(index!=-1){
						//Instantiate tile 
						GameObject instantiated = (GameObject) PrefabUtility.InstantiatePrefab(tc.tiles[index]);

						if(shift>0f){
							firstpos.x += shift;
							shift = 0.0f;
						}

						//Move into position
						instantiated.transform.position = firstpos;

						//Add as child
						instantiated.transform.parent = tc.transform;
					}else{

						if(val==tc.targetTag){
							targetPositionArrayholder.Add(firstpos);
						}else if(val==tc.teleporterTag){
							teleportPositionArrayholder.Add(firstpos);
						}


//						GameObject go = getGameObjectFor(teleporterTag);
//						if(go!=null){
//							go.transform.position = firstpos;
//							go.transform.parent = tc.transform;
//						}
					}

					//Move the "cursor" ahead
					firstpos.x += sizeOfFirstTile.x;
				}

//				teleportPositionArrayholder = getGameObjectAndPosition(teleportPositionArrayholder, teleporterTag);
//				targetPositionArrayholder = getGameObjectAndPosition(targetPositionArrayholder, teleporterTag);

				getGameObjectAndPosition (ref teleportPositionArrayholder, tc.teleporterTag);
				getGameObjectAndPosition (ref targetPositionArrayholder, tc.targetTag);
				//				
//				if(targetPositionArrayholder.Count==4){
//					Vector3 calculatedPosition = Vector3.zero;
//					
//					foreach(Vector3 pos in targetPositionArrayholder){
//						calculatedPosition += pos;
//					}
//					calculatedPosition = calculatedPosition / targetPositionArrayholder.Count;
//					GameObject go = getGameObjectFor(targetTag);
//					if(go!=null){
//						go.transform.position = calculatedPosition;
//						go.transform.parent = tc.transform;
//					}
//					targetPositionArrayholder = new ArrayList();
//				}
				
				//Move the "cursor" to the beginning of the next row
				firstpos.z -= sizeOfFirstTile.z;
				firstpos.x = tc.transform.position.x;
				rowsAdded++;
			}
		}

		if (GUILayout.Button ("Clear Tiles")) {
			tc = (TilesCreator) serializedObject.targetObject;

			int childs = tc.transform.childCount;
			for (int i = childs - 1; i >= 0; i--)
			{
				GameObject.DestroyImmediate(tc.transform.GetChild(i).gameObject);
			}
		}

		serializedObject.ApplyModifiedProperties();
	}

	void getGameObjectAndPosition (ref ArrayList teleportPositionArrayholder, string tag)
	{
		if (teleportPositionArrayholder.Count == 4) {
			Vector3 calculatedPosition = Vector3.zero;
			foreach (Vector3 pos in teleportPositionArrayholder) {
				calculatedPosition += pos;
			}
			calculatedPosition = calculatedPosition / teleportPositionArrayholder.Count;
			GameObject go = getGameObjectFor (tag);
			if (go != null) {
				go.transform.position = calculatedPosition;
				go.transform.parent = tc.transform;
			}
			teleportPositionArrayholder = new ArrayList ();
		}
	}

	private ArrayList getGameObjectAndPosition(ArrayList positions, string tag){
		if(positions.Count==4){
			Vector3 calculatedPosition = Vector3.zero;
			
			foreach(Vector3 pos in positions){
				calculatedPosition += pos;
			}
			calculatedPosition = calculatedPosition / positions.Count;
			GameObject go = getGameObjectFor(tc.targetTag);
			if(go!=null){
				go.transform.position = calculatedPosition;
				go.transform.parent = tc.transform;
			}
			positions = new ArrayList();
		}

		return positions;

	}

	private GameObject getGameObjectFor(string tag){
		if(tag==tc.teleporterTag){
			GameObject instantiated = (GameObject) PrefabUtility.InstantiatePrefab(tc.teleporter);
			return instantiated;
		}
		if(tag==tc.targetTag){
			GameObject instantiated = (GameObject) PrefabUtility.InstantiatePrefab(tc.target);
			return instantiated;
		}

		return null;
	}
}
