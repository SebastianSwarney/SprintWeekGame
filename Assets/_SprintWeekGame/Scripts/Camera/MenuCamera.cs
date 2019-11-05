using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    public CameraFade m_fadeImage;
    public AnimationCurve m_fadeCurve;
    public float m_fadeInTime;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float t = 0;

        while (t < m_fadeInTime)
        {
            t += Time.deltaTime;

            float progress = m_fadeCurve.Evaluate(t / m_fadeInTime);

            m_fadeImage.FindFadeProgress(progress);

            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        float t = 0;

        while (t < m_fadeInTime)
        {
            t += Time.deltaTime;

            float progress = m_fadeCurve.Evaluate(t / m_fadeInTime);

            m_fadeImage.FindReverseProgress(progress);

            yield return null;
        }

    }
}
