using UnityEngine;
using TMPro;

public class FishChips : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        FishChipsCounter.fishChips += 1;
        Destroy(this.gameObject);
    }
}
