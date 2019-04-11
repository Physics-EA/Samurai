using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadNextLevel : MonoBehaviour 
{
    public string nextLevel;
    public bool ToNextLevel = false;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameController.Instance.SetCurrentLevel(nextLevel);

            if (ToNextLevel)
            {
                GameController.Instance.LoadToNextLevel();
            }
        }
    }
}
