using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpColor : MonoBehaviour
{
    private SpriteRenderer m_spriteRenderer;

    private Color m_startColor;

    private void Start()
    {

        m_spriteRenderer = GetComponent<SpriteRenderer>();

        m_startColor = m_spriteRenderer.color;
    }

    public void FindFadeProgress(float p_progress)
    {
        m_spriteRenderer.color = Color.Lerp(m_startColor, Color.clear, p_progress);
    }

    public void FindReverseProgress(float p_progress)
    {
        m_spriteRenderer.color = Color.Lerp(Color.clear, m_startColor, p_progress);
    }

    public void ResetColor()
    {
        m_spriteRenderer.color = m_startColor;
    }

    public void FindColorLerpProgress(Color p_startColor, Color p_endColor, float p_progress)
    {
        m_spriteRenderer.color = Color.Lerp(p_startColor, p_endColor, p_progress);
    }
}
