using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager m_instance;

    public List<PlayerGameComponent> m;

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

    public void KillPlayer(PlayerGameComponent p_playerToKill)
    {
        StartCoroutine(RespawnPlayer(p_playerToKill));
    }

    private IEnumerator RespawnPlayer(PlayerGameComponent p_respawningPlayer)
    {
        p_respawningPlayer.gameObject.SetActive(false);

        float t = 0;

        while (t < p_respawningPlayer.m_respawnTime)
        {
            t += Time.deltaTime;
            yield return null;
        }

        p_respawningPlayer.transform.position = Vector3.zero;

        p_respawningPlayer.gameObject.SetActive(true);
    }
}
