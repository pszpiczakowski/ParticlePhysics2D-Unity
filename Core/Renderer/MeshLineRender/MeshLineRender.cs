﻿//Yves Wang @ FISH, 2015, All rights reserved
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ParticlePhysics2D {
	
	
	[RequireComponent(typeof(ParticlePhysics2D.IFormLayer),typeof(MeshRenderer),typeof(MeshFilter))]
	[ExecuteInEditMode]
	public class MeshLineRender : MonoBehaviour {
		
		#region Fields and Properties
		[HideInInspector] [SerializeField] Simulation sim;
		[HideInInspector] [SerializeField] Mesh mesh;
		[HideInInspector] [SerializeField] MeshRenderer meshRenderer;
		[HideInInspector] [SerializeField] MeshFilter meshFilter;
		[HideInInspector] [SerializeField] int particleNumCache;
		[HideInInspector] [SerializeField] int stringNumCache;
		
		[HideInInspector] [SerializeField] Color colorCache;
		public Color color = Color.white;
		
		
		MaterialPropertyBlock mpb;
		
		
		static Material mtl;
		static Material Mtl {
			get {
				if (mtl==null) mtl = new Material (Shader.Find("ParticlePhysics2D/MeshLineRender"));
				return mtl;
			}
		}
		#endregion
		
		#region Unity Methods
		void Start () {
			if (IsInitialized==false) {
				MeshLineRender_Ctor();
			}
		}
		
		public void LateUpdate(){
			if (mpb==null) {
				mpb = new MaterialPropertyBlock ();
				mpb.AddColor("_Color",color);
			}
			
			if (mesh!=null) {
				//if the sim changes data topology
				if (this.particleNumCache!=sim.numberOfParticles() || this.stringNumCache != sim.numberOfSprings()) {
					mesh.Clear();
					CreateMesh();
					this.particleNumCache=sim.numberOfParticles();
					this.stringNumCache = sim.numberOfSprings();
				} else {
					Vector3[] v = sim.getVertices();
					if (v!=null) {
						mesh.vertices = v;
						mesh.RecalculateBounds();
					}
				}
				
			}
			
			if (color != colorCache) {
				SetColor(color);
				colorCache = color;
			}
		}
		
		#endregion
		
		//if the data has been initialized already?
		bool IsInitialized {
			get {
				if (sim!=null && mesh!=null && meshRenderer!=null && meshFilter!=null) {
					return true;
				} else return false;
			}
		}
		
		void MeshLineRender_Ctor () {
			
			this.sim = this.GetComponent<IFormLayer>().GetSimulation;
			this.particleNumCache = sim.numberOfParticles();
			this.stringNumCache = sim.numberOfSprings();
			
			meshRenderer = this.GetComponent<MeshRenderer>();
			if (meshRenderer == null) meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
			meshRenderer.receiveShadows = false;
			meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			meshRenderer.useLightProbes = false;
			meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
			meshRenderer.sharedMaterial = Mtl;
			meshRenderer.hideFlags = HideFlags.NotEditable;
			
			mpb = new MaterialPropertyBlock ();
			meshRenderer.GetPropertyBlock(mpb);
			mpb.AddColor("_Color",color);
			meshRenderer.SetPropertyBlock(mpb);
			colorCache = color;
			
			if (mesh==null) {
				mesh = new Mesh ();
				CreateMesh();	
			}
			
			meshFilter = this.GetComponent<MeshFilter>();
			if (meshFilter==null)
				meshFilter = this.gameObject.AddComponent<MeshFilter>();
			meshFilter.mesh = mesh;
			meshFilter.hideFlags = HideFlags.NotEditable;
			
		}
		
		
		
		public void SetColor(Color c) {
			if (mpb==null) mpb = new MaterialPropertyBlock ();
			meshRenderer.GetPropertyBlock(mpb);
			mpb.AddColor("_Color",c);
			meshRenderer.SetPropertyBlock(mpb);
		}
		
		void CreateMesh () {
			
			if (sim.numberOfParticles()<2) {
				mesh.Clear();
				return;
			}
			
			//create vertex
			Vector3[] v = sim.getVertices();
			if (v==null) {
				mesh.Clear();
				return;
			}
			else {
				mesh.vertices = v;
			}
			
			//create edges
			int[] ic = sim.getIndices();
			if (ic==null) {
				mesh.Clear();
				return;
			} else {
				mesh.SetIndices(ic,MeshTopology.Lines,0);
				mesh.RecalculateBounds();
				mesh.MarkDynamic();
			}
			
		}
		
		public void RemoveResources() {

			ObjExtension.ObjDestroy(meshRenderer);
			ObjExtension.ObjDestroy(meshFilter);
			ObjExtension.ObjDestroy(mesh);
			
		}
		
		
		
	}
	
	public static class ObjExtension {
	
		public static void ObjDestroy(this UnityEngine.Object obj) {
			
			if (Application.isEditor) {
				if (obj)  {
					try {
						UnityEngine.Object.DestroyImmediate(obj);	
					} catch (System.Exception e) {
						
					}
					
				}
			} else {
				if (obj) {
					try {
						UnityEngine.Object.Destroy(obj);	
					} catch (System.Exception e) {
						
					}	
				}
			}	
			
		}
	}
	
}
