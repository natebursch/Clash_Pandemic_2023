// Designed by Kinemation, 2022

using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Demo.Scripts.Runtime.Layers
{
    public class DemoBlendingLayer : AnimLayer
    {
        // Source static pose
        [SerializeField] private AnimationClip anim;
        // Character ref
        [SerializeField] private GameObject character;
        [SerializeField] private Transform rootBone;
        [SerializeField] private Transform spineRootBone;
        [SerializeField] private Quaternion spineBoneRotMS;

        private float _smoothAlpha;

        private void Start()
        {
            _smoothAlpha = layerAlpha;
        }

        // MS: mesh space
        public void EvaluateSpineMS()
        {
            if (character == null || anim == null || rootBone == null || spineRootBone == null)
            {
                return;
            }

            Vector3 cachedLoc = character.transform.position;
            anim.SampleAnimation(character, 0f);
            character.transform.position = cachedLoc;

            // To mesh space
            spineBoneRotMS = Quaternion.Inverse(rootBone.rotation) * spineRootBone.rotation;
        }

        public override void OnPreAnimUpdate()
        {
            var finalAlpha = layerAlpha;
            if (GetMovementState() == FPSMovementState.Sprinting)
            {
                finalAlpha = 0f;
            }

            _smoothAlpha = CoreToolkitLib.Glerp(_smoothAlpha, finalAlpha, 9f);
            
            GetAnimator().SetLayerWeight(2, _smoothAlpha);
            GetAnimator().SetLayerWeight(3, _smoothAlpha);
            
            spineRootBone.rotation = Quaternion.Slerp(spineRootBone.rotation,
                rootBone.rotation * spineBoneRotMS, _smoothAlpha);
        }
    }
}
