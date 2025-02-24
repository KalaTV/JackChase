using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector2 direction;
    public float _speed = 25f; 
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal"); 
        float vertical = Input.GetAxis("Vertical");  
        direction = new Vector2(horizontal, vertical).normalized;
        
        transform.Translate(Vector3.up * vertical * _speed * Time.deltaTime, Space.Self);
    }
}