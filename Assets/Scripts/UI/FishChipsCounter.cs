using UnityEngine;
using TMPro;
public class FishChipsCounter : MonoBehaviour
{
    TMP_Text text;

    public static int fishChips;

    void Start()
    {
        text= GetComponent < TMP_Text> ();
    }
    
    void Update()
    {
        text.text = fishChips.ToString();
    }
}
