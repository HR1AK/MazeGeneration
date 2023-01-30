using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContorll : MonoBehaviour
{
    [Header("Скорость игрока")]
    public float Speed = 1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.localPosition += transform.up * Time.deltaTime * Speed;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.localPosition += -transform.up * Time.deltaTime * Speed;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.localPosition += transform.right * Time.deltaTime * Speed;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.localPosition += -transform.right * Time.deltaTime * Speed;
        }
    }

}
