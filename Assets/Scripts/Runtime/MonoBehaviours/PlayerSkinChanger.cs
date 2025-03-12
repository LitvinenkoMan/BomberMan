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
        [SerializeField]
        private Renderer visualsRenderer;

        private void Awake()
        {
            visualsRenderer = Visuals.GetComponent<Renderer>();
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
            var mainColor = visualsRenderer.material.color;
            StartCoroutine(ReturnBackBaseColor(time, mainColor));
            mainColor = new Color(mainColor.r, mainColor.g, mainColor.b, 0.5f);
            visualsRenderer.material.color = mainColor;
        }

        private IEnumerator ReturnBackBaseColor(float time, Color baseColor)
        {
            yield return new WaitForSeconds(time);
            visualsRenderer.material.color = baseColor;
            yield return null;
        }
    }
}
