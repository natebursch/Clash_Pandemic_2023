// Designed by Kinemation, 2022
//edited by nate bursch. 1-19-2023

using System.Collections.Generic;
using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Demo.Scripts.Runtime
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip gunShotClip;
        [SerializeField] private ParticleSystem muzzleEffect;

        [SerializeField] private GameObject shellEjection;
        [SerializeField] private Transform shellEjectionPort;


        [SerializeField] private List<Transform> scopes;
        [SerializeField] public WeaponAnimData gunData;
        [SerializeField] public RecoilAnimData recoilData;
        
        public FireMode fireMode;
        public float fireRate;
        public int burstAmount;

        private Animator _animator;
        private int _scopeIndex;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public Transform GetScope()
        {
            _scopeIndex++;
            _scopeIndex = _scopeIndex > scopes.Count - 1 ? 0 : _scopeIndex;
            return scopes[_scopeIndex];
        }
        
        public void OnFire()
        {
            PlayFireAnim();
        }

        private void PlayFireAnim()
        {
            if (_animator == null)
            {
                return;
            }
            _animator.Play("Fire", 0, 0f);

            //play the sound
            audioSource.PlayOneShot(gunShotClip);
            //play the muzzleflash
            muzzleEffect.Play();

            //shoot a raycast bullet
        }
    }
}
