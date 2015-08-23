﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using ParticlePhysics2D;

[CustomEditor(typeof(Branch_Mono))]
public class Branch_Mono_Editor : Editor {
	
	Simulation sim;
	Branch_Mono temp;
	
	bool reGenBranch;
	PivotMode lastPMode;
	PivotRotation lastRMode;
	
	void OnEnable() {
		temp = target as Branch_Mono;
		sim = temp.GetSimulation;
		if (sim==null) {
			BinaryTree.ResetParams(temp.length);
			temp.ResetForm();
			this.sim = temp.GetSimulation;
		} else {
			//Branch.lengthExit = temp.lengthExit;
			BinaryTree.angleOffsetMax = temp.angleOffsetMax;
			BinaryTree.angleOffsetMin = temp.angleOffsetMin;
			BinaryTree.lengthMax1 = temp.lengthMax1;
			BinaryTree.lengthMax2 = temp.lengthMax2;
			BinaryTree.lengthMin1 = temp.lengthMin1;
			BinaryTree.lengthMin2 = temp.lengthMin2;
			BinaryTree.lengthBranchAThreshold = temp.lengthBranchAThreshold;
			BinaryTree.lengthBranchBThreshold = temp.lengthBranchBThreshold;
			//Branch.lengthExitRatio = temp.lengthExitRatio;
			//Branch.lengthExit = temp.lengthExitRatio * temp.length;
			BinaryTree.maxDepth = temp.maxDepth;
		}
		
		lastPMode = Tools.pivotMode;
		lastRMode = Tools.pivotRotation;
		Tools.pivotMode = PivotMode.Pivot;
		Tools.pivotRotation = PivotRotation.Local;
		
	}
	
	void OnDisable() {
		SaveParams();
		Tools.pivotMode =lastPMode;
		Tools.pivotRotation = lastRMode;
		
	}
	
	void SaveParams() {
		temp.maxDepth = BinaryTree.maxDepth;
		temp.angleOffsetMax = BinaryTree.angleOffsetMax;
		temp.angleOffsetMin = BinaryTree.angleOffsetMin;
		temp.lengthMax1 = BinaryTree.lengthMax1;
		temp.lengthMax2 = BinaryTree.lengthMax2;
		temp.lengthMin1 = BinaryTree.lengthMin1;
		temp.lengthMin2 = BinaryTree.lengthMin2;
		temp.lengthBranchAThreshold = BinaryTree.lengthBranchAThreshold;
		temp.lengthBranchBThreshold = BinaryTree.lengthBranchBThreshold;
	}
	
//	Texture2D MakeTex(int width, int height, Color col) {
//		var pix = new Color[width * height];
//		for (int i = 0; i < pix.Length; i++) {
//			pix[i] = col;
//		}
//		var result = new Texture2D(width, height);
//		result.SetPixels(pix);
//		result.Apply();
//		return result;
//	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();

		//Branch Debug params
		EditorGUILayout.LabelField("-------------------------------------------------------------------------------------------------");
		BinaryTree.debugBranch = EditorGUILayout.Toggle("Debug Branch",BinaryTree.debugBranch,GUILayout.ExpandWidth(true));
		BinaryTree.debugBranchLeaf = EditorGUILayout.Toggle("Debug Leaf",BinaryTree.debugBranchLeaf,GUILayout.ExpandWidth(true));
		BinaryTree.debugBranchBoundingCircle = EditorGUILayout.Toggle("Debug Bounding Circle",BinaryTree.debugBranchBoundingCircle,GUILayout.ExpandWidth(true));
		BinaryTree.debugBoundingCircleDepth = EditorGUILayout.IntSlider("Depth",BinaryTree.debugBoundingCircleDepth,-1,temp.maxDepth + 1);
		if (BinaryTree.debugBranch) EditorUtility.SetDirty(temp);
		
		if (sim==null) return;
		
		//Branch params
		EditorGUILayout.LabelField("-----------------------------------------------------------------------------------------------------");
		
		//display how many particles are created in the Simulation
		EditorGUILayout.HelpBox("Particle Count: "+sim.numberOfParticles(),MessageType.None);
		
		//display how many leaf
		EditorGUILayout.HelpBox("Leaf Count: "+temp.leafCount,MessageType.None);
		
		//display how many strings are created in the Simulation
		EditorGUILayout.HelpBox("Spring Count: "+sim.numberOfSprings(),MessageType.None);
		
		//rotation
		EditorGUILayout.BeginHorizontal();
		bool rotateLeft = GUILayout.RepeatButton("Rotate Left",GUILayout.ExpandWidth(true));
		if (rotateLeft) {
			temp.gameObject.transform.Rotate(0f,0f,5f,Space.Self); 
		}
		bool rotateRight = GUILayout.RepeatButton("Rotate Right",GUILayout.ExpandWidth(true));
		if (rotateRight) { 
			temp.gameObject.transform.Rotate(0f,0f,-5f,Space.Self);
		}
		EditorGUILayout.EndHorizontal();
		
		//exit length: how complex should the branch be
		BinaryTree.maxDepth = EditorGUILayout.IntSlider(
			new GUIContent("Max Depth","This generally controls how complex the branch is"),
			BinaryTree.maxDepth,
			3,
			BinaryTree.maxDepthLimit 
		);
		
		
		//how wide
		string range = "Angle Offset: " + (BinaryTree.angleOffsetMin * Mathf.Rad2Deg).ToString("F1") + "-" + (BinaryTree.angleOffsetMax * Mathf.Rad2Deg).ToString("F1");
		EditorGUILayout.MinMaxSlider(
			new GUIContent(range,"How much should the children branch be rotated from parent" ),
			ref BinaryTree.angleOffsetMin,
			ref BinaryTree.angleOffsetMax,
			0f,
			1f
		);
		
		//branch threshold A
		BinaryTree.lengthBranchAThreshold = EditorGUILayout.Slider(
			new GUIContent("Threshold A",""),
			BinaryTree.lengthBranchAThreshold,
			0.01f,
			temp.length
		);
		
		//branch threshold A
		BinaryTree.lengthBranchBThreshold = EditorGUILayout.Slider(
			new GUIContent("Threshold B",""),
			BinaryTree.lengthBranchBThreshold,
			0.01f,
			temp.length
		);
		
		//above threshold
		string t1 = "Above T. "+BinaryTree.lengthMin1.ToString("F1") + "-" + BinaryTree.lengthMax1.ToString("F1");
		EditorGUILayout.MinMaxSlider(
			new GUIContent(t1,"When children branch go over threshold, the factor multiplied to parent branch" ),
			ref BinaryTree.lengthMin1,
			ref BinaryTree.lengthMax1,
			0.5f,
			1f
		);
		
		//above threshold
		t1 = "Beblow T. "+BinaryTree.lengthMin2.ToString("F1") + "-" + BinaryTree.lengthMax2.ToString("F1");
		EditorGUILayout.MinMaxSlider(
			new GUIContent(t1,"When children branch go below threshold, the factor multiplied to parent branch" ),
			ref BinaryTree.lengthMin2,
			ref BinaryTree.lengthMax2,
			0.5f,
			1f
		);
		
		GUI.backgroundColor = Color.green * 0.8f;
		reGenBranch = GUILayout.Button("Re-Generate Branch",GUILayout.ExpandWidth(true));
		if (reGenBranch) {
			temp.ResetForm();
			EditorUtility.SetDirty(temp);
		}
		
		GUI.backgroundColor = Color.cyan * 0.8f;
		bool resetParam = false;
		resetParam = GUILayout.Button("Reset Branch Generation Params",GUILayout.ExpandWidth(true));
		if (resetParam) BinaryTree.ResetParams(temp.length);
		
		//clear branch
		GUI.backgroundColor = Color.red * 0.8f;
		bool clearBranch = GUILayout.Button("Clear Branchs",GUILayout.ExpandWidth(true));
		if (clearBranch) {
			temp.ClearForm();
			EditorUtility.SetDirty(temp);
		}
		//EditorUtility.SetDirty(temp);
		SaveParams();
		
	}
	
	public void OnSceneGUI() {
		if (sim!=null) {
			if (temp.debugParticleIndex) sim.DebugParticle(temp.transform.localToWorldMatrix);
			if (temp.debugSpringIndex) sim.DebugSpringIndex(temp.transform.localToWorldMatrix);
		}
	}
}
