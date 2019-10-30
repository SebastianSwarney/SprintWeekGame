using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager m_instance;

    public int m_scoreToWin;

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

    public void CheckScore()
    {
        /*
        for (int i = 0; i < PlayerManager.m_instance.m_players.Length; i++)
        {
            if (PlayerManager.m_instance.m_players[i].m_currentScore >= m_scoreToWin)
            {
                CameraController.m_instance.WinZoomToPlayer(i);
            }
        }
        */
    }

}
