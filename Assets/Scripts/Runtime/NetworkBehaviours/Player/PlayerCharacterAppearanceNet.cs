using System.Linq;
using Core.ScriptableObjects;
using Interfaces;
using UnityEngine;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerCharacterAppearanceNet : MonoBehaviour, ICharacterAppearance
    {
        private GameObject _playerVisuals;
        public MeshRenderer MeshRenderer { get; private set; }

        public void SetNewAppearance(ICharacterData characterData)
        {
            _playerVisuals = Instantiate(characterData.Visuals, transform);
            
            Animator animator = _playerVisuals.GetComponent<Animator>();
            animator.avatar = characterData.Avatar;
            animator.runtimeAnimatorController = characterData.AnimatorController;
            animator.applyRootMotion = false;
            
            // var bones = rig.GetComponentsInChildren<Transform>();
            //
            // if (mesh.TryGetComponent(out SkinnedMeshRenderer skinnedMeshRenderer))
            // {
            //     skinnedMeshRenderer.rootBone = rig.GetChild(0);
            //     skinnedMeshRenderer.bones = new Transform[bones.Length];
            //     
            //     for (int i = 0; i < skinnedMeshRenderer.bones.Length; i++)
            //     {
            //         var bone = skinnedMeshRenderer.bones[i];    
            //         bone = bones.ToList().Find(bones[i]);
            //     }
            // }

            // if (skinnedMesh.TryGetComponent(out SkinnedMeshRenderer skinnedMeshRenderer))
            // {
            //     skinnedMeshRenderer.rootBone = rig.transform.GetChild(0);
            //     var bones = rig.GetComponentsInChildren<Transform>();
            //     
            //     // Привязываем все кости
            //     for (int i = 0; i < skinnedMeshRenderer.bones.Length; i++)
            //     {
            //         var bone = skinnedMeshRenderer.bones[i];
            //         if (bone == null)
            //         {
            //             // Пытаемся найти по имени в риге
            //             var foundBone = bones.FirstOrDefault(b => b.name == bone.name);
            //             if (foundBone != null)
            //                 skinnedMeshRenderer.bones[i] = foundBone;
            //         }
            //     }
            //     skinnedMeshRenderer.forceMatrixRecalculationPerRender = true;
            //}
        }

        public void ClearAppearance()
        {
            
        }
    }
}
