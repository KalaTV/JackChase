using UnityEngine;

public class DianaCollectibleHandler : MonoBehaviour
{
    [Header("Collectible Settings")]
    [Tooltip("Nombre de collectibles à atteindre pour rapprocher Diana.")]
    public int collectiblesThreshold = 5;
    
    private int _collectibleCount = 0;

    [Header("Référence à Diana")]
    [Tooltip("Référence vers le script de mouvement (ou state machine) de Diana.")]
    public DianaMovement dianaMovement; 

    public void OnCollectibleCollected()
    {
        _collectibleCount++;
        Debug.Log("Collectible récupéré. Total = " + _collectibleCount);
  
        if (_collectibleCount % collectiblesThreshold == 0)
        {
            dianaMovement.OnCollectibleCollected(_collectibleCount);
            Debug.Log("Palier atteint, réduction de la distance de Diana.");
        }
    }

    public int GetCollectibleCount()
    {
        return _collectibleCount;
    }
}