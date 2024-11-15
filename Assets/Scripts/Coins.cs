using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{

    [SerializeField] int value = 1;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.PickedUpCoin(value);
            Destroy(this.gameObject);
        }
    }

}
