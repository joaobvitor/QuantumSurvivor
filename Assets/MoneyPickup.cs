using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    public int money = 10;
    
    AudioSource pickupSource;

    void Awake()
    {
        pickupSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        PlayerController playerController = collision.GetComponent<PlayerController>();

        if (playerController) {
            playerController.Money += money;
            if (pickupSource)
                AudioSource.PlayClipAtPoint(pickupSource.clip, gameObject.transform.position, pickupSource.volume);
            Destroy(gameObject);
        }
    }
}
