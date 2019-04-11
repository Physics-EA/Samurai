using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*  负责游戏的开始，存档，及游戏关卡切换，游戏结束
 */

public class GameController : MonoSingleton<GameController> 
{
    private string currentLevel;

	// Use this for initialization
	void Start () 
	{
	}

    public void SetCurrentLevel(string nextLevel)
    {
        currentLevel = nextLevel;
    }

    public void LoadToNextLevel()
    {
        StartCoroutine(NextLevel());
    }

    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(6);

        SceneManager.LoadScene(currentLevel);
    }

}
