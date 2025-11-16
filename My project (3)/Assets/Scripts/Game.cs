using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class Game : MonoBehaviour
{
    public static Game Default;

    private void Awake()
    {
        Default = this;
    }

    private void OnDestroy()
    {
        Default = null;
    }

    Text mScoreText;

    private void Start()
    {
        mScoreText = transform.Find("UI/ScoreText").GetComponent<Text>();
        Score = 0;
        UpdateScoreText();
    }

    public static void ReloadScene()
    {
        Default.GetComponent<AudioSource>().Stop();
        Default.StartCoroutine(DoReloadScene());
    }

    private static IEnumerator DoReloadScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("SampleScene");
    }

    public static int Score = 0;

    public static void AddScore(int score)
    {
        Score += score;
        UpdateScoreText();
    }

    static void UpdateScoreText()
    {
        Default.mScoreText.text = "Score:" + Score.ToString();
    }
}