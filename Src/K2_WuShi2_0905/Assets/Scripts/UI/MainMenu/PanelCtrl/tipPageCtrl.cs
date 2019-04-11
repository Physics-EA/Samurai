using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Assets.Scripts.GameData;

public class tipPageCtrl : MonoBehaviour 
{
    private float step = 1280;
    private int curPageNum;
    private int curLevel;

    public Sprite[] pages;
    public Image curPageImage;
    public GameObject playPanel;
    public Button backBtn;
    public Button lastBtn;
    public Button nextBtn;
    public Text curLevelText;

    void Awake()
    {
        if (curLevelText == null)
        {
            return;
        }

        curLevel = int.Parse(curLevelText.text);
    }

    void Update()
    {
        curPageNum = FindCurPageNum();

        if (curPageNum <= 1)
        {
            backBtn.interactable = false;
            lastBtn.interactable = false;
        }
        else
        {
            backBtn.interactable = true;
            lastBtn.interactable = true;
        }

        if (curPageNum == pages.Length)
        {
            nextBtn.interactable = false;
        }
        else
        {
            nextBtn.interactable = true;
        }
    }

    public void NextPage()
    {
        curPageNum = FindCurPageNum();

        //  如果已经到了最后一页
        if (curPageNum == pages.Length)
        {
            return;
        }

        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        curPageImage.sprite = pages[curPageNum];
        float curPosX=GetComponent<RectTransform>().localPosition.x;
        this.GetComponent<RectTransform>().DOAnchorPosX(curPosX-step, 0.1f);
    }

    public void LastPage() 
    {
        curPageNum = FindCurPageNum();

        if (curPageNum <= 1)
        {
            return;
        }

        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        curPageImage.sprite = pages[curPageNum - 2];
        float curPosX = GetComponent<RectTransform>().localPosition.x;
        this.GetComponent<RectTransform>().DOAnchorPosX(curPosX + step, 0.1f);
    }

    public void FirstPage()
    {
        curPageNum = 1;
        curPageImage.sprite=pages[0];
        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        this.GetComponent<RectTransform>().DOAnchorPosX(0, 0.1f);
    }

    public void LoadLevel()
    {
        playPanel.SetActive(false);
        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        this.GetComponent<RectTransform>().DOAnchorPosX(-10240, 0.1f);

        StartCoroutine(ChangeScene());
    }

    private IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(0.3f);

        Debug.Log("进入章节"+curLevel);

        if (curLevel == 8)
        {
            Debug.Log("恭喜你通关了");
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene("MainMenu");
            //Application.LoadLevel("MainMenu");
        }

        if (curLevel < 8)
        {
            SceneManager.LoadScene(curLevel);
            //Application.LoadLevel(curLevel);
        }   
    }

    private int FindCurPageNum()
    {
        //  找到当前的所在的页数
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] == curPageImage.sprite)
            {
                return i + 1;
            }
        }

        //  没有就返回-1
        Debug.LogError("没有该页");
        return -1;
    }
}
