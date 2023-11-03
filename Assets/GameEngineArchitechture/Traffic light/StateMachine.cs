using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField] List<BaseState> states;

    [SerializeField]BaseState currentState;

    void Start() {
	    currentState = states[0];
    }
    
    void Update() {
	    if (currentState.nextState != null) {
			SwitchStates(currentState.nextState);
	    }
	    currentState.StateUpdate();
    }

    void PhysicsUpdate() {
	    if (currentState.nextState != null) {
		    SwitchStates(currentState.nextState);
	    }
	    currentState.StatePhysicsUpdate();
    }

    void SwitchStates(BaseState newState) {
	    if (currentState != null) {
		    currentState.StateExited();
			currentState.nextState = null;
	    }
	    currentState = newState;
	    currentState.StateEntered();
    }
}
