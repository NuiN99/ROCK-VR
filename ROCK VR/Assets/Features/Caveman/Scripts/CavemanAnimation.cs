using Animancer;
using UnityEngine;

public class CavemanAnimation : MonoBehaviour
{
    [SerializeField] AnimancerComponent animator;
    [SerializeField] AnimationClip chaseAnim;
    [SerializeField] AnimationClip attackAnim;

    public void PlayAnimation(CavemanBrain.State state)
    {
        switch (state)
        {
            case CavemanBrain.State.Chase:
                animator.Play(chaseAnim);
                break;
            case CavemanBrain.State.Attack:
                animator.Play(attackAnim);
                break;
        }
    }
}