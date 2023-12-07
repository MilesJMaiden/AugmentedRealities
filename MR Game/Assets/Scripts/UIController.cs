using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Add this line

public class UIController : MonoBehaviour
{
    public Text healthText;
    public Text killCountText;
    public Text timeText;
    public Text bulletText;

    public int currentHealth;
    public int killCount;
    public float TimeLeft = 15;
    public int CurrentBullet;
    public int maxBullet;

    // Reference to other scripts

    public PlayerAttributes PlayerAttributes;
    public EnemySpawner EnemySpawner;
    //public GunController GunController;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Pull data from other scripts
        if (PlayerAttributes != null) currentHealth = PlayerAttributes.health;
      //  if (EnemySpawner != null) killCount = ;
        //if (GunController != null)
        //{
       //     CurrentBullet = GunController.CurrentBullet;
       //     maxBullet = GunController.MaxBullet;
      //  }

        // Display values in UI
        healthText.text = "Health: " + currentHealth;
        killCountText.text = "Kill Count: " + killCount;
        bulletText.text = "Bullet: " + CurrentBullet + "/" + maxBullet;

        // Update countdown timer
        TimeLeft -= UnityEngine.Time.deltaTime;
        if (TimeLeft <= 0)
        {
            TimeLeft = 15; // reset timer
        }
        timeText.text = "Time: " + Mathf.Round(TimeLeft);
    }
}