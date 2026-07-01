using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Pull this object to the left based on the global world speed
        transform.Translate(Vector3.left * WorldManager.Instance.currentWorldSpeed * Time.deltaTime);
    }
}
