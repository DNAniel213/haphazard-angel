using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBlink : MonoBehaviour
{
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Blink", Random.Range(1, 15));
    }

    void Blink()
    {
        anim.SetTrigger("Blink");
        Invoke("Blink", Random.Range(3, 20));
    }

}
