using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI killCountText;
    public TextMeshProUGUI timeText;

    public int currentHealth;
    public int killCount;
    public float timeLeft = 15;

    // Reference to other scripts

    public PlayerAttributes playerAttributes;
    public EnemySpawner enemySpawner;

    // Start is called before the first frame update
    void Start()
    {
        UpdateKillCountUI();
        UpdateHealthUI();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimeUI();
    }
    public void UpdateHealthUI()
    {
        if (playerAttributes != null)
        {
            healthText.text = $"Health: {playerAttributes.health}";
        }
    }

    public void UpdateKillCountUI()
    {
        if (enemySpawner != null)
        {
            killCountText.text = $"Kill Count: {enemySpawner.enemyCounter}";
        }
    }

    public void UpdateTimeUI()
    {
        // Update countdown timer
        timeLeft -= UnityEngine.Time.deltaTime;
        if (timeLeft <= 0)
        {
            timeLeft = 15; // reset timer
        }
        timeText.text = $"Time: {Mathf.Round(timeLeft)}";
    }
}
