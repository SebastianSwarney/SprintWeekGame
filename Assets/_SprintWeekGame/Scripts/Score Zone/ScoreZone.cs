using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    public LayerMask m_playerMask;

    public bool CheckCollisionLayer(LayerMask p_layerMask, GameObject p_object)
    {
        if (p_layerMask == (p_layerMask | (1 << p_object.layer)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Score(PlayerGameComponent p_scoredPlayer)
    {
        PlayerManager.m_instance.ScorePlayer(p_scoredPlayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CheckCollisionLayer(m_playerMask, collision.gameObject))
        {
            PlayerMovementController player = collision.gameObject.GetComponentInParent<PlayerMovementController>();

            if (player.m_hasBounced)
            {
                Score(player.GetComponentInParent<PlayerGameComponent>());
            }
        }
    }
}