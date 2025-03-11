using System.Collections;
using UnityEngine;

namespace Runtime.MonoBehaviours
{
    public class PlayerSkinChanger : MonoBehaviour
    {
        [SerializeField]
        private GameObject Visuals;
        [SerializeField]
        private HealthComponent _healthComponent;

        private Renderer _renderer;

        private void Awake()
        {
            _renderer = Visuals.GetComponent<Renderer>();
        }

        private void OnEnable()
        {
            _healthComponent.OnGetImmune += ApplyGhostEffect;
        }

        private void OnDisable()
        {
            _healthComponent.OnGetImmune -= ApplyGhostEffect;
        }
        
        private void ApplyGhostEffect(float time)
        {
            var mainColor = _renderer.material.color;
            StartCoroutine(ReturnBackBaseColor(time, mainColor));
            mainColor = new Color(mainColor.r, mainColor.g, mainColor.b, 0.5f);
            _renderer.material.color = mainColor;
        }

        private IEnumerator ReturnBackBaseColor(float time, Color baseColor)
        {
            yield return new WaitForSeconds(time);
            _renderer.material.color = baseColor;
            yield return null;
        }
    }
}
