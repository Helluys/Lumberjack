using System;
using UnityEngine;

/// <summary>
/// Gather events from Animator finite state machine and allow accessing them as C# events.
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimationManager : MonoBehaviour {

    /// <summary>
    /// This event is trigger by all Animation events names AnimationEvent with a string parameter that will be passed as the argument of the event.
    /// </summary>
    public event Action<string> OnAnimationEvent;

    /// <summary>
    /// This event is trigger by all Animation events names AnimationEvent with a string parameter that will be passed as the argument of the event.
    /// </summary>
    public event Action<string> OnEnter, OnExit;

    public Animator animator { get; private set; }

    private void Start () {
        animator = GetComponent<Animator>();
        AnimatorTrigger animatorTrigger = animator.GetBehaviour<AnimatorTrigger>();
        if (animatorTrigger) {
            animatorTrigger.OnEnter += AnimatorTrigger_OnEnter;
            animatorTrigger.OnExit += AnimatorTrigger_OnExit;
        }
    }

    public void AnimationEvent (string eventName) {
        if (OnAnimationEvent != null)
            OnAnimationEvent(eventName);
    }

    private void AnimatorTrigger_OnEnter (string clipName) {
        if (OnEnter != null)
            OnEnter(clipName);
    }

    private void AnimatorTrigger_OnExit (string clipName) {
        if (OnExit != null)
            OnExit(clipName);
    }
}
