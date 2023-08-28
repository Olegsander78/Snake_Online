using UnityEngine;

public class Apple : MonoBehaviour
{
    private Vector2Float _apple;

    public void Init(Vector2Float apple)
    {
        _apple = apple;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }    
}
