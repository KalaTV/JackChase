using UnityEngine;

public class FishNChips : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        FishNChipsCounter.fishChips += 1;
        Destroy(this.gameObject);
    }
}
