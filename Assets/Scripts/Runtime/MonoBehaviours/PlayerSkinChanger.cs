using System.Collections;
using UnityEngine;

namespace Runtime.MonoBehaviours
{
    public class PlayerSkinChanger : MonoBehaviour
    {
        [SerializeField]
        private SkinnedMeshRenderer visualsRenderer;

        private void OnEnable()
        {
            //healthComponent.OnGetImmune += ApplyGhostEffect;
        }

        private void OnDisable()
        {
            //healthComponent.OnGetImmune -= ApplyGhostEffect;
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
