using UnityEngine;

namespace Examples.Unity.Cosmetic
{
    public class PlayerAnimations: MonoBehaviour
    {
        [SerializeField] private Animator animator;
        
        public const string SHOOT_ANIMATION = "Shoot";
        
        public void TriggerAnimation(string animationName)
        {
            animator.SetTrigger(animationName);
        }
    }
}