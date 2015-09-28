﻿//Yves Wang @ FISH, 2015, All rights reserved

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ParticlePhysics2D {

	[AddComponentMenu("ParticlePhysics2D/Simulation Manager",13)]
	public class SimulationManager : Singleton<SimulationManager> {
		
		//singleton instance
		public static SimulationManager Instance {
			get { return ((SimulationManager)mInstance);} 
			set { mInstance = value;}
		}
		
		//global parameters
		
		///////////////////////////////////////////////////////////////////
		//// Collision Update Params									///
		///////////////////////////////////////////////////////////////////
		[HideInInspector]
		public float FixedTimestep_Collision = 1f/30f;
		
		[Space(10f)]
		
		[Range(10,60)]
		public int UpdatePerSecond_Collision = 30;
		private int _updatePerSecond_Collision;
		
		///////////////////////////////////////////////////////////////////
		//// Verlet Update Params										///
		///////////////////////////////////////////////////////////////////
		[HideInInspector]
		public float FixedTimestep_Verlet = 1f/30f;
		
		[Space(10)]
		
		[Range(10,60)]
		public int UpdatePerSecond_Verlet = 60;
		int _updatePerSecond_Verlet;
		
		
		public bool IsDebugOn = false;
		CollisionProcessor bpProcessor = new CollisionProcessor ();	//broad phase processor
		CollisionProcessor npProcessor = new CollisionProcessor ();  //narrow phase processor
		
		//generic add and remove
		public void AddCollisionObject ( CollisionHolder2D obj ) {
			bpProcessor.AddObject(obj);
		}
		
		public void AddCollisionObject( CollisionTarget2D obj) {
			npProcessor.AddObject(obj);
		}
		
		public void RemoveCollisionObject ( CollisionHolder2D obj) {
			bpProcessor.RemoveObject(obj); 
		}
		
		public void RemoveCollisionObject ( CollisionTarget2D obj) {
			npProcessor.RemoveObject(obj);
		}
		
		void Start() {
			_updatePerSecond_Collision = this.UpdatePerSecond_Collision;
			this.FixedTimestep_Collision = 1f/this.UpdatePerSecond_Collision;
			
			_updatePerSecond_Verlet = this.UpdatePerSecond_Verlet;
			this.FixedTimestep_Verlet = 1f/this.UpdatePerSecond_Verlet;
		}
		
		void Update() {
			if (UpdatePerSecond_Collision != _updatePerSecond_Collision) {
				_updatePerSecond_Collision = UpdatePerSecond_Collision;
				this.FixedTimestep_Collision = 1f/this.UpdatePerSecond_Collision;
			}
			
			if (UpdatePerSecond_Verlet != _updatePerSecond_Verlet) {
				_updatePerSecond_Verlet = UpdatePerSecond_Verlet;
				this.FixedTimestep_Verlet = 1f/this.UpdatePerSecond_Verlet;
			}
		}
		
		void FixedUpdate () {
			bpProcessor.Update(Time.deltaTime);
			npProcessor.Update(Time.deltaTime);
		}
	}
}

