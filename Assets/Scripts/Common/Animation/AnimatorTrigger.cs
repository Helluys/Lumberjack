using System;
using UnityEngine;

public class AnimatorTrigger : StateMachineBehaviour {

    public event Action<string> OnEnter, OnExit;

    public void OnStateEnter (string eventName) {
        if (OnEnter != null)
            OnEnter(eventName);
    }

    public void OnStateExit (string eventName) {
        if (OnExit != null)
            OnExit(eventName);
    }
}
