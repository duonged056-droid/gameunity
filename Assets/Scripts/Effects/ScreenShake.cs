using System.Collections;
using UnityEngine;

namespace DragonBallDefenders
{
    public class ScreenShake : MonoBehaviour
    {
        public static ScreenShake Instance { get; private set; }

        private Vector3 initialPosition;
        private Coroutine currentShakeCoroutine;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            initialPosition = transform.localPosition;
        }

        public void Shake(float duration = 0.2f, float magnitude = 0.2f)
        {
            if (currentShakeCoroutine != null)
            {
                StopCoroutine(currentShakeCoroutine);
            }
            currentShakeCoroutine = StartCoroutine(DoShake(duration, magnitude));
        }

        private IEnumerator DoShake(float duration, float magnitude)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
                float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = new Vector3(initialPosition.x + x, initialPosition.y + y, initialPosition.z);

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            transform.localPosition = initialPosition;
            currentShakeCoroutine = null;
        }
    }
}
