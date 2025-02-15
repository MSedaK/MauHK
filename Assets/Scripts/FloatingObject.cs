using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float floatSpeed = 1f; // Yukarı-aşağı hareket hızı
    public float floatAmount = 0.5f; // Maksimum hareket mesafesi
    private Vector3 startPosition;


void Start()
    {
        startPosition = transform.position; // Başlangıç pozisyonunu kaydet
    }

    void Update()
    {
        // Nesneyi yukarı-aşağı hareket ettir
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }


}