using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager m_instance;

    public PlayerGameComponent[] m_players;

    [HideInInspector]
    public bool m_onePlayerLeft;

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

        foreach (PlayerGameComponent player in m_players)
        {
            player.m_lives = RoundManager.m_instance.m_playerLives;

            player.PlayerSetup();
        }
    }

    public void FreezeAllPlayers()
    {
        for (int i = 0; i < m_players.Length; i++)
        {
            m_players[i].m_movementController.m_movementControll = PlayerMovementController.MovementControllState.MovementDisabled;
        }
    }

    public void UnFreezeAllPlayers()
    {
        for (int i = 0; i < m_players.Length; i++)
        {
            m_players[i].m_movementController.m_movementControll = PlayerMovementController.MovementControllState.MovementEnabled;
        }
    }

    public void ScorePlayer(PlayerGameComponent p_playerToKill)
    {
        p_playerToKill.KillPlayer();

        if (p_playerToKill.m_lives > 0)
        {
            StartCoroutine(RespawnPlayer(p_playerToKill));
        }

        m_onePlayerLeft = true;

        int index = 0;

        int lastPlayer = 0;

        for (int i = 0; i < m_players.Length; i++)
        {
            if (m_players[i].m_lives > 0)
            {
                index++;
                lastPlayer = i;
            }
        }

        if (index > 1)
        {
            m_onePlayerLeft = false;
        }

        RoundManager.m_instance.CheckScore(lastPlayer);

    }

    private IEnumerator RespawnPlayer(PlayerGameComponent p_respawningPlayer)
    {
        float t = 0;

        while (t < p_respawningPlayer.m_respawnTime)
        {
            t += Time.deltaTime;
            yield return null;
        }

        p_respawningPlayer.Respawn();
    }
}
