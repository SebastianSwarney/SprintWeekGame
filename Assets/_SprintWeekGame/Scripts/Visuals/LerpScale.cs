using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpScale : MonoBehaviour
{
    public Vector3 m_targetScale;

    private Vector3 m_startScale;

    private void Start()
    {
        m_startScale = transform.localScale;
    }

    public void FindLerpProgress(float p_progress)
    {
        transform.localScale = Vector3.Lerp(m_startScale, m_targetScale, p_progress);
    }

    public void ResetScale()
    {
        transform.localScale = m_startScale;
    }
}
