using UnityEngine;

public class ClipTrigger : StateMachineBehaviour {

    [SerializeField] private string eventName = "";

    public override void OnStateEnter (Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        animator.GetBehaviour<AnimatorTrigger>().OnStateEnter(eventName);
    }

    public override void OnStateExit (Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        animator.GetBehaviour<AnimatorTrigger>().OnStateExit(eventName);
    }

}
