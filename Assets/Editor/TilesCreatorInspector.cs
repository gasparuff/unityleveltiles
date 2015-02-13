using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Security.Cryptography;


[CustomEditor(typeof(TilesCreator)), CanEditMultipleObjects]
public class TilesCreatorInspector : Editor {


	private TilesCreator tc;
	private Vector2 scroll;

	static string patternAliasPair = "**";
	static string patternAliasUnpair = "++";

	public override void OnInspectorGUI () {
		serializedObject.Update();
		tc = (TilesCreator) serializedObject.targetObject;

		
		//Painting the GUI
		EditorList.Show(serializedObject.FindProperty("tiles"), EditorListOption.ListLabel | EditorListOption.Buttons);
		EditorList.Show(serializedObject.FindProperty("aliases"), EditorListOption.ListLabel | EditorListOption.Buttons);
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("teleporter"));
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("target"));
		tc.teleporterTag = EditorGUILayout.TextField("Teleporter Tag", tc.teleporterTag);
		tc.targetTag = EditorGUILayout.TextField("Target Tag", tc.targetTag);
//		tc.patternAlias = EditorGUILayout.TextField("Pattern Alias", tc.patternAlias);

		EditorGUILayout.Separator ();

		tc.randomSection = EditorGUILayout.BeginToggleGroup ("Random", tc.randomSection);
		
		if(tc.randomSection) {
			tc.length = EditorGUILayout.TextField("Length", tc.length);
			tc.height = EditorGUILayout.TextField("Height", tc.height);
			EditorList.Show(serializedObject.FindProperty("weights"), EditorListOption.ListLabel | EditorListOption.Buttons);
		}

		EditorGUILayout.EndToggleGroup ();

		EditorGUILayout.Separator ();

		EditorGUILayout.PropertyField (serializedObject.FindProperty ("levelInput"));

		//Here's where all the magic happens
		if(GUILayout.Button("Build Object")){
			ClearTiles();
			if(!tc.randomSection){
				//Read the textfile
				string text = tc.levelInput.text;

				//save text for later
				tc.pattern = text;

				//Split it into rows
				BuildGrid (text);

			}else{
				int product = int.Parse(tc.length) * int.Parse (tc.height);
				int sum = 0;
				float[] probs = new float[tc.weights.Length];
				ArrayList stringTiles = new ArrayList();
				for(int i=0; i<tc.weights.Length; i++){
					sum += tc.weights[i]; 
				}

				for(int i=0; i<tc.weights.Length; i++){
					if(tc.weights[i]!=0){
						probs[i] = (float)tc.weights[i]/sum;
					}else{
						probs[i] = 0.0f;
					}
					int count = (int)(product * probs[i]);
					for(int j=0; j<count; j++){
						stringTiles.Add(tc.aliases[i]);
					}
				}

				stringTiles = ShuffleArrayList(stringTiles);
				string output = "";
				int length = int.Parse(tc.length);
				for(int i=0; i<stringTiles.Count; i++){
					if(i>0 && i%length==0){
						output+= "\n";
					}
					output += stringTiles[i];
				}
				BuildGrid (output);

				//save text for later
				tc.pattern = output;

			}
		}

		if (GUILayout.Button ("Clear Tiles")) {
			ClearTiles ();
		}

		EditorGUILayout.Separator ();

		if (GUILayout.Button ("Create template")){
			BuildTemplate(tc.pattern);
		}

		tc.template = EditorGUILayout.TextArea(tc.template, GUILayout.ExpandHeight (true));		

//		EditorGUILayout.TextArea (tc.patternAlias, GUILayout.Height(ImagePosition.height);



		EditorList.Show(serializedObject.FindProperty("walls"), EditorListOption.ListLabel | EditorListOption.Buttons);
		EditorList.Show(serializedObject.FindProperty("wallsAliases"), EditorListOption.ListLabel | EditorListOption.Buttons);

		if(GUILayout.Button("Build Walls")){
			ClearWalls();
			string text = tc.template;
								

			//Split it into rows
			BuildWalls (text);
				
//			}
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

	private void BuildTemplate (string text)
	{
		string[] rows = text.Split ("\n" [0]);
		//Get the size of the first tile - all tiles must have the same size, otherwise this won't look good
//		Vector3 sizeOfFirstTile = tc.tiles [0].GetComponent<MeshFilter> ().sharedMesh.bounds.extents;
//		sizeOfFirstTile *= 2;
		//Prepare rowcounter
		int rowsAdded = 0;
		bool lastRowShifted = false;
		//Create a reference position where everything begins
		Vector3 firstpos = tc.transform.position;
//		ArrayList teleportPositionArrayholder = new ArrayList ();
//		ArrayList targetPositionArrayholder = new ArrayList ();

		string output = "";

		if(tc.patternAlias == null || tc.patternAlias.Length==0){
			tc.patternAlias = "OO";
		}

		bool pair = false;
		//Loop over every single row
		for(int j=0; j<rows.Length; j++){

//		foreach (string row in rows) {
			//Get all characters of current row
			string row = rows[j];
			char[] chars = row.ToCharArray ();
			float shift = 0.0f;
			int loopStart = 0;
			string firstChar = "" + chars [0];
			if (firstChar == "-") {
				loopStart = 1;
				output += "-";
			}
			//Loop over every tuple
			for (int i = loopStart; i < chars.Length; i = i + 2) {
				//Get the Array Index of the current tuple
//				string pA = tc.patternAlias;
				string val = "" + chars [i] + chars [i + 1];
				val = val.ToUpper();
				if(val==tc.teleporterTag || val==tc.targetTag){
					output += "  ";
					continue;
				}
				if(pair){
					output += patternAliasPair;
				}else{
					output += patternAliasUnpair;
				}
				pair = !pair;
			}
			if(j+1<rows.Length){
				output+= "\n";
			}

			rowsAdded++;
		}

		tc.template = output;
//		serializedObject.Update();

	}


	private void BuildGrid (string text)
	{

		Logger.Log ("Building");
		Logger.Log (text);
		string[] rows = text.Split ("\n" [0]);
		//Get the size of the first tile - all tiles must have the same size, otherwise this won't look good
		Vector3 sizeOfFirstTile = tc.tiles [0].GetComponent<MeshFilter> ().sharedMesh.bounds.extents;
		sizeOfFirstTile *= 2;
		//Prepare rowcounter
		int rowsAdded = 0;
		bool lastRowShifted = false;
		//Create a reference position where everything begins
		Vector3 firstpos = tc.transform.position;
		ArrayList teleportPositionArrayholder = new ArrayList ();
		ArrayList targetPositionArrayholder = new ArrayList ();

		GameObject tiles = new GameObject ("Tiles");
		tiles.transform.parent = tc.transform;

		GameObject bridge = new GameObject ("bridge");
		bridge.transform.parent = tiles.transform;
		//Loop over every single row
		foreach (string row in rows) {
			//Get all characters of current row
			char[] chars = row.ToCharArray ();

			float shift = 0.0f;
			int loopStart = 0;
			string firstChar = "" + chars [0];
			if (firstChar == "-") {
				shift = sizeOfFirstTile.x / 2;
				loopStart = 1;
			}
			//Loop over every tuple
			for (int i = loopStart; i < chars.Length; i = i + 2) {
				//Get the Array Index of the current tuple
				bool isBridge = char.IsLower(chars[i]);


				string val = "" + chars [i] + chars [i + 1];
				val = val.ToUpper();
				int index = System.Array.IndexOf (tc.aliases, val);
				//Making sure the tuple exists
				if (index != -1) {
					//Instantiate tile 
					GameObject instantiated = (GameObject)PrefabUtility.InstantiatePrefab (tc.tiles [index]);
					if (shift > 0f) {
						firstpos.x += shift;
						shift = 0.0f;
					}
					//Move into position
					instantiated.transform.position = firstpos;

					//Add as child
					if(isBridge){
						instantiated.transform.parent = bridge.transform;
					}else{
						instantiated.transform.parent = tiles.transform;
					}


				}
				else {
					if (val == tc.targetTag) {
						targetPositionArrayholder.Add (firstpos);
					}
					else
						if (val == tc.teleporterTag) {
							teleportPositionArrayholder.Add (firstpos);
						}
				}
				//Move the "cursor" ahead
				firstpos.x += sizeOfFirstTile.x;
			}
			getGameObjectAndPosition (ref teleportPositionArrayholder, tc.teleporterTag);
			getGameObjectAndPosition (ref targetPositionArrayholder, tc.targetTag);
			//Move the "cursor" to the beginning of the next row
			firstpos.z -= sizeOfFirstTile.z;
			firstpos.x = tc.transform.position.x;
			rowsAdded++;
		}
	}

	private void BuildWalls (string text)
	{
		string[] rows = text.Split ("\n" [0]);

		Logger.Log ("input: " + text);

		//Get the size of the first tile - all tiles must have the same size, otherwise this won't look good
		Vector3 sizeOfFirstTile = tc.tiles [0].GetComponent<MeshFilter> ().sharedMesh.bounds.extents;
		sizeOfFirstTile *= 2;
		//Prepare rowcounter
		int rowsAdded = 0;
		bool lastRowShifted = false;
		//Create a reference position where everything begins
		Vector3 firstpos = tc.transform.position;

		GameObject walls = new GameObject("Walls");
		walls.transform.parent = tc.transform;
		//Loop over every single row
		foreach (string row in rows) {
			//Get all characters of current row

//			char[] chars = row.ToCharArray ();
			float shift = 0.0f;
			int loopStart = 0;
//			string firstChar = "" + chars [0];
			if(row.Length==0){
				continue;
			}
			string firstChar = row.Substring(0,1);

			if (firstChar == "-") {
				shift = sizeOfFirstTile.x / 2;
				loopStart = 1;
			}
			//Loop over every tuple
			for (int i = loopStart; i < row.Length; i = i + 2) {
				//Get the Array Index of the current tuple
				bool rotate = false;

				string val = row.Substring(i,2);

				string valFirst = val.Substring(0,1);
//				if(valFirst==valFirst.ToLower()){
//					rotate = true;
//				}

				string valRotation = val.Substring(1,1).ToUpper();

//				string val = "" + chars [i] + chars [i + 1];
				val = valFirst.ToUpper();
				int index = System.Array.IndexOf (tc.wallsAliases, val);

				Logger.Log ("value: "+val+" index: "+index);

				//Making sure the tuple exists
				if (index != -1) {
					//Instantiate tile 
					GameObject instantiated = (GameObject)PrefabUtility.InstantiatePrefab (tc.walls [index]);
					if (shift > 0f) {
						firstpos.x += shift;
						shift = 0.0f;
					}
					//Move into position
					instantiated.transform.position = firstpos;
					if(valRotation=="T"){
						instantiated.transform.Rotate(new Vector3(0f,180f,0f));
					}else if(valRotation=="R"){
						instantiated.transform.Rotate(new Vector3(0f,-90f,0f));
					}else if(valRotation=="L"){
						instantiated.transform.Rotate(new Vector3(0f,90f,0f));
					}


					instantiated.transform.parent = walls.transform;
					
				}

				//Move the "cursor" ahead
				firstpos.x += sizeOfFirstTile.x;
			}
//			getGameObjectAndPosition (ref teleportPositionArrayholder, tc.teleporterTag);
//			getGameObjectAndPosition (ref targetPositionArrayholder, tc.targetTag);
			//Move the "cursor" to the beginning of the next row
			firstpos.z -= sizeOfFirstTile.z;
			firstpos.x = tc.transform.position.x;
			rowsAdded++;
		}
	}

	void ClearTiles ()
	{
		tc = (TilesCreator) serializedObject.targetObject;
		GameObject g = GameObject.Find("Tiles");
		if(g!=null && g.transform.childCount>0){
			
			
			//			int childs = g.transform.childCount;
			
			foreach(Transform t in g.transform){
				GameObject.DestroyImmediate(t.gameObject);
			}
			
			//			for (int i = childs - 1; i >= 0; i--) {
			//				GameObject.DestroyImmediate (tc.transform.GetChild (0).GetChild(i).gameObject);
			//			}
			DestroyImmediate(g);
		}
	}
	
	void ClearWalls ()
	{
		tc = (TilesCreator) serializedObject.targetObject;
		GameObject g = GameObject.Find("Walls");
		if(g!=null && g.transform.childCount>0){
			foreach(Transform t in g.transform){
				GameObject.DestroyImmediate(t.gameObject);
			}
			
			//			for (int i = childs - 1; i >= 0; i--) {
			//				GameObject.DestroyImmediate (tc.transform.GetChild (0).GetChild(i).gameObject);
			//			}
			DestroyImmediate(g);
		}
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

	private ArrayList ShuffleArrayList(ArrayList source)
	{
		ArrayList sortedList = new ArrayList();
		Random generator = new Random();
		
		while (source.Count > 0)
		{
			int position = Random.Range(0,source.Count);
			sortedList.Add(source[position]);
			source.RemoveAt(position);
		}
		
		return sortedList;
	}

}
