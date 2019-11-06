using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelVisual : ButtonVisual, ISelectHandler, IDeselectHandler
{
    public string m_levelToSelect;

    public void LevelSelected()
    {
        GameManager.m_instance.LoadLevel(m_levelToSelect);
    }
}
