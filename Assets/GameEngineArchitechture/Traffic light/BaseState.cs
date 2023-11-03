using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState {
	public string stateName;
	StateMachine stateMachine;
	public BaseState nextState = null;

	public BaseState(string stateName, StateMachine stateMachine) {
		this.stateName = stateName;
		this.stateMachine = stateMachine;
	}
	
	public virtual void StateEntered() { return; }
	
	public virtual void StateUpdate() { return; }

	public virtual void StatePhysicsUpdate() { return; }
	
	public virtual void StateExited() { return; }
}
