using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIFillAmount : MonoBehaviour
{
    public Image Bar;
    public TMPro.TextMeshPro score;
    [SerializeField][Range(0, 1)] public float fillAmount;
    // Start is called before the first frame update

    // Update is called once per frame
    public void updateFill(int health)
    {
        fillAmount = health / 5; //current health /max health
        fillAmount = Mathf.Clamp(fillAmount, 0, 1); //clamps
        Bar.fillAmount = fillAmount; //sets fill amount
    }

    public void scoreEdit(int newScore)
    {
        score.text = newScore.ToString();
    }
}
