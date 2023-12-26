using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugHotkeys : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.Equals))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().Heal(1);
        }

        if (Input.GetKeyDown(KeyCode.Minus))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().TakeDamage(1);
        }
    }
}
