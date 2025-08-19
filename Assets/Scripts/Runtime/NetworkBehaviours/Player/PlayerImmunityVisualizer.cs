using Interfaces;
using System;
using UnityEngine;
using System.Collections;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerImmunityVisualizer : MonoBehaviour, IImmuneVisualizer
    {
        [SerializeField] private Renderer _playerMesh;
        private float _targetAlpha = 0.5f;
        public event Action<float> OnImmuneVisualized;

        private void Start()
        {
            if (_playerMesh == null)
            {
                Debug.LogError("PlayerImmunityVisualizer: Renderer component not found on the GameObject.");
            }
        }        

        public void VisualizeImmunity(float time)
        {
            StartCoroutine(ActiveVisualizeImmunity(time));
        }
        private IEnumerator ActiveVisualizeImmunity(float time)
        {
            var material = new MaterialPropertyBlock();
            _playerMesh.GetPropertyBlock(material);
            Color color = new Color(1, 1, 1, _targetAlpha);          
           
            material.SetColor("_BaseColor", color);
            _playerMesh.SetPropertyBlock(material);
            OnImmuneVisualized?.Invoke(time);

            yield return new WaitForSeconds(time);

            color.a = 1f;             
            material.SetColor("_BaseColor", color);
            _playerMesh.SetPropertyBlock(material);

            yield return null;
        }
    }
}
