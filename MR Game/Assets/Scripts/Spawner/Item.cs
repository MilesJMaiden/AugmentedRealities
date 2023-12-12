using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { Medicine, Bullet }
    public ItemType itemType;
    public PlayerAttributes PlayerAttributes;

    private void OnTriggerEnter(Collider other)
    {
        if (PlayerAttributes != null)
        {
            PlayerAttributes player = other.gameObject.GetComponent<PlayerAttributes>();
            if (player != null)
            {
                switch (itemType)
                {
                    case ItemType.Medicine:
                        player.health += 20;
                        break;
                    case ItemType.Bullet:
                        player.attack += 10;
                        break;
                }
            }
            Destroy(gameObject);
        }
    }
}