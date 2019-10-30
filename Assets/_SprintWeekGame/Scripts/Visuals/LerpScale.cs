using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpScale : MonoBehaviour
{
    public Vector3 m_targetScale;

    public AnimationCurve m_soloCurve;

    private Vector3 m_startScale;



    private void Start()
    {
        m_startScale = transform.localScale;
    }

    public IEnumerator RunLerpScale(float p_lerpTime)
    {
        float t = 0;

        while (t < p_lerpTime)
        {
            t += Time.deltaTime;

            float progress = m_soloCurve.Evaluate(t / p_lerpTime);

            transform.localScale = Vector3.Lerp(m_startScale, m_targetScale, progress);

            yield return null;
        }

        ResetScale();
    }

    public IEnumerator RunLerpScale(float p_lerpTime, float p_targetScale)
    {
        Vector3 targetScale = new Vector3(p_targetScale, p_targetScale, p_targetScale);

        float t = 0;

        while (t < p_lerpTime)
        {
            t += Time.deltaTime;

            float progress = m_soloCurve.Evaluate(t / p_lerpTime);

            transform.localScale = Vector3.Lerp(m_startScale, targetScale, progress);

            yield return null;
        }

        ResetScale();
    }

    public void FindLerpProgress(float p_progress)
    {
        transform.localScale = Vector3.Lerp(m_startScale, m_targetScale, p_progress);
    }

    public void FindLerpProgressSet(float p_progress, float p_targetScale)
    {
        Vector3 targetScale = new Vector3(p_targetScale, p_targetScale, p_targetScale);

        transform.localScale = Vector3.Lerp(m_startScale, targetScale, p_progress);
    }

    public void SetScaleRadius(float p_currentRadius)
    {
        transform.localScale = new Vector3(p_currentRadius + p_currentRadius, p_currentRadius + p_currentRadius, p_currentRadius + p_currentRadius);
    }

    public void ResetScale()
    {
        transform.localScale = m_startScale;
    }
}
