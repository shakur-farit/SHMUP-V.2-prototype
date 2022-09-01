using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSin : MonoBehaviour
{
    float sinCenterY; //value for sin movement
    public float amplitude = 2; //amplitude of wave
    public float frequency = 2; //distance between waves
    public bool inverted = false; //for inverted sin move

    // Start is called before the first frame update
    void Start()
    {
        sinCenterY = transform.position.y; //sin movement
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        Vector2 pos = transform.position;

        float sin = Mathf.Sin(pos.x * frequency) * amplitude;
        if (inverted)
        {
            sin *= -1;
        }
        pos.y = sinCenterY + sin;

        transform.position = pos;
    }
}
