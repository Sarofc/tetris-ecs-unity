using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class View : MonoBehaviour
{
    public TMP_Text countDown;
    public TMP_Text score;
    public TMP_Text level;
    public TMP_Text goal;
    public TMP_Text time;
    public GameObject gameOverPanel;

    private void OnEnable()
    {
        Tetris.OnScoreChanged += UpdateScore;
        Tetris.OnLevelChanged += UpdateLevel;
        Tetris.OnGoalChanged += UpdateGoal;
        Tetris.OnTimeChanged += UpdateTime;
        Tetris.OnGameOver += OnGameOver;

        gameOverPanel.SetActive(false);

        CountDown();
    }

    private void UpdateTime(float val)
    {
        time.text = string.Format("{0:00}:{1:00}", val / 60, val % 60);
    }

    private void UpdateGoal(int val)
    {
        goal.text = val.ToString();
    }

    private void OnDisable()
    {
        Tetris.OnScoreChanged -= UpdateScore;
        Tetris.OnLevelChanged -= UpdateLevel;
        Tetris.OnGameOver -= OnGameOver;
    }

    private void OnGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void CountDown()
    {
        StartCoroutine(CountDownCoroutine());
    }

    private IEnumerator CountDownCoroutine()
    {
        countDown.enabled = true;
        countDown.fontSize = 24;
        countDown.color = Color.white;
        countDown.text = "3";
        yield return new WaitForSeconds(1f);
        countDown.text = "2";
        yield return new WaitForSeconds(1f);
        countDown.text = "1";
        yield return new WaitForSeconds(1f);
        countDown.fontSize = 36;
        countDown.color = Color.red;
        countDown.text = "GO!";
        yield return new WaitForSeconds(1f);
        countDown.enabled = false;
    }

    private void UpdateLevel(int val)
    {
        level.text = val.ToString();
    }

    private void UpdateScore(int val)
    {
        score.text = val.ToString();
    }
}
