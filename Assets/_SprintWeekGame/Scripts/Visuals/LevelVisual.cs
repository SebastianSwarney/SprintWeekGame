using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelVisual : MonoBehaviour
{
    public float m_selectPunchTime;
    public float m_selectPunchAmount;
    private GameObject lastSelected;

    public string m_levelToSelect;

    public void LevelSelected()
    {
        GameManager.m_instance.LoadLevel(m_levelToSelect);
    }

    private void Update()
    {
        EventSystem es = EventSystem.current;
        if (es)
        {
            if (es.currentSelectedGameObject != lastSelected)
            {
                lastSelected = es.currentSelectedGameObject;
                if (lastSelected == gameObject)
                {
                    iTween.PunchScale(gameObject, Vector3.one * m_selectPunchAmount, m_selectPunchTime);
                }
            }
        }
    }
}
