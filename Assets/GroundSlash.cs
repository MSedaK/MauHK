using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSlash : MonoBehaviour
{
    public float speed = 30;
    public float slowDownRate = 0.01f;
    public float detectingDistance = 0.1f;
    public float destroyDelay = 5;

    private Rigidbody rb;
    private bool stopped;

    void Start()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        if (GetComponent<Rigidbody>() != null)
        {
            rb = GetComponent<Rigidbody>();
            StartCoroutine(SlowDown());
        }
        else
        {
            Debug.Log("No Rigidbody");
        }

        Destroy(gameObject, destroyDelay);
    }

    private void FixedUpdate()
    {
        if (!stopped)
        {
            RaycastHit hit;
            Vector3 distance = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            if (Physics.Raycast(distance, transform.TransformDirection(-Vector3.up), out hit, detectingDistance))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                Debug.DrawRay(distance, transform.TransformDirection(-Vector3.up) * detectingDistance, Color.red);
            }
        }
    }
    IEnumerator SlowDown()
    {
        float t = 1; // Yavaþlama baþlangýç zamaný
        while (t > 0)
        {
            // Rigidbody'nin hýzýný kademeli olarak azaltýr
            rb.velocity = Vector3.Lerp(Vector3.zero, rb.velocity, t);

            // Zaman deðerini azaltýr
            t -= slowDownRate;

            // Her adýmda kýsa bir süre bekler
            yield return new WaitForSeconds(0.1f);
        }

        // Yavaþlama tamamlandýðýnda durdurulmuþ olarak iþaretler
        stopped = true;
    }

}
