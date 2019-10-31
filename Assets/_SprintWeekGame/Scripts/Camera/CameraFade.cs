using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFade : MonoBehaviour
{
    private Image m_uiImage;

    private Color m_startColor;

    private void Start()
    {
        m_uiImage = GetComponent<Image>();
        m_startColor = Color.black;
    }

    public void FindFadeProgress(float p_progress)
    {
        m_uiImage.color = Color.Lerp(m_startColor, Color.clear, p_progress);
    }

    public void FindReverseProgress(float p_progress)
    {
        m_uiImage.color = Color.Lerp(Color.clear, m_startColor, p_progress);
    }

    public void ResetColor()
    {
        m_uiImage.color = m_startColor;
    }

    public void FindColorLerpProgress(Color p_startColor, Color p_endColor, float p_progress)
    {
        m_uiImage.color = Color.Lerp(p_startColor, p_endColor, p_progress);
    }
}
