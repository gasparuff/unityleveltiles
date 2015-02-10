using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TilesCreator)), CanEditMultipleObjects]
public class TilesCreatorInspector : Editor {


	// Will every second row be shifted?
	public bool shiftedRows;

	public override void OnInspectorGUI () {
		serializedObject.Update();

		//Painting the GUI
		EditorList.Show(serializedObject.FindProperty("tiles"), EditorListOption.ListLabel | EditorListOption.Buttons);
		EditorList.Show(serializedObject.FindProperty("aliases"), EditorListOption.ListLabel | EditorListOption.Buttons);
		shiftedRows = EditorGUILayout.Toggle ("Row shift", shiftedRows);
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("levelInput"));

		//Here's where all the magic happens
		if(GUILayout.Button("Build Object")){
			TilesCreator tc = (TilesCreator) serializedObject.targetObject;

			//Read the textfile
			string text = tc.levelInput.text;

			//Split it into rows
			string[] rows = text.Split("\n" [0]);

			//Get the size of the first tile - all tiles must have the same size, otherwise this won't look good
			Vector3 sizeOfFirstTile = tc.tiles[0].GetComponent<MeshFilter>().sharedMesh.bounds.extents;
			sizeOfFirstTile *= 2;

			//Create a reference position where everything begins
			Vector3 firstpos = new Vector3();


			//Prepare rowcounter
			int rowsAdded = 0;

			//Loop over every single row
			foreach(string row in rows){

				//Get all characters of current row
				char[] chars = row.ToCharArray();

				//Shift those rows if requested
				if(shiftedRows){
					if(rowsAdded%2!=0){
						firstpos.x += sizeOfFirstTile.x/4;
					}else{
						firstpos.x -= sizeOfFirstTile.x/4;
					}
				}

				//Loop over every tuple
				for(int i=0; i<chars.Length; i=i+2){


					//Get the Array Index of the current tuple
					string val = ""+chars[i]+chars[i+1];
					int index = System.Array.IndexOf(tc.aliases, val);

					//Making sure the tuple exists
					if(index!=-1){
						//Instantiate tile 
						GameObject instantiated = (GameObject) PrefabUtility.InstantiatePrefab(tc.tiles[index]);

						//Move into position
						instantiated.transform.position = firstpos;

						//Add as child
						instantiated.transform.parent = tc.transform;
					}

					//Move the "cursor" ahead
					firstpos.x += sizeOfFirstTile.x;
				}

				//Move the "cursor" to the beginning of the next row
				firstpos.z += sizeOfFirstTile.z;
				firstpos.x = 0.0f;
				rowsAdded++;
			}
		}

		serializedObject.ApplyModifiedProperties();
	}
}
