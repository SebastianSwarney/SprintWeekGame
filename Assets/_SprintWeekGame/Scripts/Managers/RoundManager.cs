using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

[System.Serializable]
public class RoundManagerEvent : UnityEvent { }

public class RoundManager : MonoBehaviour
{
    public static RoundManager m_instance;

    public int m_roundCountDownTime;

    public int m_playerLives;

    public TextMeshProUGUI m_countdownText;

    public RoundManagerEvent m_roundManagerEvent;

    public float m_countdownPunchAmount;

    private bool m_playersLeft;

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

    private void Start()
    {
        GameStart();
    }

    public void GameStart()
    {
        StartCoroutine(GameCountDown());
    }

    IEnumerator GameCountDown()
    {
        PlayerManager.m_instance.FreezeAllPlayers();

        int t = m_roundCountDownTime + 1;

        while (t > 0)
        {
            t--;

            m_countdownText.text = t.ToString();
            m_roundManagerEvent.Invoke();
            iTween.PunchScale(m_countdownText.gameObject, Vector3.one * m_countdownPunchAmount, 0.8f);

            yield return new WaitForSeconds(1f);
        }

        m_countdownText.gameObject.SetActive(false);

        PlayerManager.m_instance.UnFreezeAllPlayers();
    }

    public void CheckScore(int p_lastPlayer)
    {
        m_playersLeft = false;

        if (PlayerManager.m_instance.m_onePlayerLeft)
        {
            CameraController.m_instance.WinZoomToPlayer(p_lastPlayer);
        }
    }
}
