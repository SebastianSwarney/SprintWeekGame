using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager m_instance;

    public PlayerGameComponent[] m_players;

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
        m_players = FindObjectsOfType<PlayerGameComponent>();
    }

    public void ScorePlayer(PlayerGameComponent p_playerToKill)
    {
        StartCoroutine(RespawnPlayer(p_playerToKill));
    }

    private IEnumerator RespawnPlayer(PlayerGameComponent p_respawningPlayer)
    {
        

        iTween.ShakePosition(Camera.main.gameObject, Vector3.one, 1f);

        p_respawningPlayer.KillPlayer();

        RoundManager.m_instance.CheckScore();

        float t = 0;

        while (t < p_respawningPlayer.m_respawnTime)
        {
            t += Time.deltaTime;
            yield return null;
        }

        p_respawningPlayer.Respawn();
    }
}
