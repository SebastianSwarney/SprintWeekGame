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

        RoundManager.m_instance.CheckScore();

        StartCoroutine(RespawnPlayer(p_playerToKill));
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
