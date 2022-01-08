//using System;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//namespace Tetris
//{


//    public class View : MonoBehaviour
//    {
//        public TMP_Text score;
//        public TMP_Text level;
//        public TMP_Text goal;
//        public TMP_Text time;
//        public GameObject gameOverPanel;

//        private void OnEnable()
//        {
//            Tetris.OnScoreChanged += UpdateScore;
//            Tetris.OnLevelChanged += UpdateLevel;
//            Tetris.OnGoalChanged += UpdateGoal;
//            Tetris.OnTimeChanged += UpdateTime;
//            Tetris.OnGameOver += OnGameOver;

//            gameOverPanel.SetActive(false);

//        }

//        private void OnDisable()
//        {
//            Tetris.OnScoreChanged -= UpdateScore;
//            Tetris.OnLevelChanged -= UpdateLevel;
//            Tetris.OnGoalChanged -= UpdateGoal;
//            Tetris.OnTimeChanged -= UpdateTime;
//            Tetris.OnGameOver -= OnGameOver;
//        }

//        private int m, s, ms;

//        private void UpdateTime(float val)
//        {
//            m = (int)val / 60;
//            s = (int)val % 60;
//            ms = ((int)(val * 1000) % 1000) / 10;

//            time.text = string.Format("{0:00}:{1:00}.{2:00}", m, s, ms);
//        }

//        private void UpdateGoal(int val)
//        {
//            goal.text = val.ToString();
//        }

//        private void OnGameOver()
//        {
//            gameOverPanel.SetActive(true);
//        }


//        private void UpdateLevel(int val)
//        {
//            level.text = val.ToString();
//        }

//        private void UpdateScore(int val)
//        {
//            score.text = val.ToString();
//        }
//    }

//}