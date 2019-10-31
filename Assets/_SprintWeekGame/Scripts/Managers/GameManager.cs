using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager m_instance;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RoundWon()
    {
        SceneManager.LoadScene("New Level");
    }
}
