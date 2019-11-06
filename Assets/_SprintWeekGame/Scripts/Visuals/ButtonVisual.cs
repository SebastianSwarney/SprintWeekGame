using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonVisual : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public float m_selectPunchTime;
    public float m_selectPunchAmount;
    public AnimationCurve m_selectPunchCurve;

    private LerpScale m_lerpScale;

    private void Start()
    {
        m_lerpScale = GetComponent<LerpScale>();
    }

    private IEnumerator ScaleButtonUp()
    {
        float t = 0;

        while (t < m_selectPunchTime)
        {
            t += Time.deltaTime;

            float progress = m_selectPunchCurve.Evaluate(t / m_selectPunchTime);

            m_lerpScale.FindLerpProgress(progress);

            yield return null;
        }
    }

    private IEnumerator ScaleButtonDown()
    {
        float t = 0;

        while (t < m_selectPunchTime)
        {
            t += Time.deltaTime;

            float progress = m_selectPunchCurve.Evaluate(t / m_selectPunchTime);

            m_lerpScale.FindReverseLerpProgress(progress);

            yield return null;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        StartCoroutine(ScaleButtonUp());
    }

    public void OnDeselect(BaseEventData eventData)
    {
        StartCoroutine(ScaleButtonDown());
    }
}
