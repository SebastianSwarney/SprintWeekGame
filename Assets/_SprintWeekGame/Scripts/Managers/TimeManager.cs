using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager m_instance;

    private bool m_isRunningSlowMo;


    public AnimationCurve m_critSlowMoCurve;
    public float m_critSlowMoTime;
    public float m_cirtSlowMoAmount;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RunCritSlowMo()
    {
        if (!m_isRunningSlowMo)
        {
            StartCoroutine(CritSlowMo());
        }
    }

    private IEnumerator CritSlowMo()
    {
        float t = 0;

        Time.timeScale = m_cirtSlowMoAmount;

        while (t < m_critSlowMoTime)
        {
            t += Time.unscaledDeltaTime;

            Time.fixedDeltaTime = Time.timeScale * 0.02f;

            float progress = m_critSlowMoCurve.Evaluate(t / m_critSlowMoTime);

            Time.timeScale = Mathf.Lerp(m_cirtSlowMoAmount, 1f, progress);

            yield return null;
        }

        Time.timeScale = 1f;

    }
}
