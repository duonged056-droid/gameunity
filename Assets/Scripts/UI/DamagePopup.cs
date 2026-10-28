using System.Collections;
using UnityEngine;
using TMPro;

namespace DragonBallDefenders
{
    public class DamagePopup : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMesh;
        private float moveSpeed = 2.5f;
        private float disappearTimer = 0.65f;
        private Color textColor;

        public static void Create(Vector3 position, int damageAmount, bool isCritical = false)
        {
            GameObject popupObj = new GameObject("DamagePopup");
            popupObj.transform.position = position + new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.1f, 0.3f), -2f);

            DamagePopup popup = popupObj.AddComponent<DamagePopup>();
            popup.Setup(damageAmount, isCritical);
        }

        public void Setup(int damageAmount, bool isCritical)
        {
            if (textMesh == null)
            {
                textMesh = gameObject.AddComponent<TextMeshPro>();
                textMesh.alignment = TextAlignmentOptions.Center;
                textMesh.fontSize = isCritical ? 6.5f : 4.8f;
                textMesh.sortingOrder = 50;
            }

            textMesh.text = damageAmount.ToString();
            textColor = isCritical ? new Color(1f, 0.8f, 0.1f, 1f) : new Color(1f, 0.3f, 0.2f, 1f);
            textMesh.color = textColor;

            StartCoroutine(FadeAndDestroy());
        }

        private IEnumerator FadeAndDestroy()
        {
            float elapsed = 0f;
            Vector3 startPos = transform.position;

            while (elapsed < disappearTimer)
            {
                elapsed += Time.deltaTime;
                transform.position = startPos + Vector3.up * (moveSpeed * elapsed);

                float alpha = Mathf.Clamp01(1f - (elapsed / disappearTimer));
                textMesh.color = new Color(textColor.r, textColor.g, textColor.b, alpha);

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
