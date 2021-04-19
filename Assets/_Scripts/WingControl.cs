using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WingControl : NetworkBehaviour
{
    Rigidbody2D rb2d;
    public float torqueForce = 5;
    public float pushForce = 5;
    ForceMode2D forceMode2d = ForceMode2D.Force;
    public GameObject ll, lr, ul, ur;
    public bool llFlap, lrFlap, ulFlap, urFlap;
    public Animator llAnim, lrAnim, ulAnim, urAnim;

    public List<NetworkPlayer> players = new List<NetworkPlayer>();

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public override void OnStopServer()
    {
        players = new List<NetworkPlayer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey("z") || llFlap)
        {
            print("z key was pressed");
            rb2d.AddTorque(-torqueForce * Time.deltaTime, forceMode2d);
            rb2d.AddRelativeForce(new Vector2(-pushForce,pushForce) * Time.deltaTime, forceMode2d);
            ll.SetActive(true);
            llAnim.SetBool("isFlapping", true);
        }
        else
        {
            ll.SetActive(false);
            llAnim.SetBool("isFlapping", false);

        }
        if(Input.GetKey("x") || lrFlap)
        {
            print("x key was pressed");
            rb2d.AddTorque(torqueForce * Time.deltaTime, forceMode2d);
            rb2d.AddRelativeForce(new Vector2(pushForce,pushForce) * Time.deltaTime, forceMode2d);
            lr.SetActive(true);
            lrAnim.SetBool("isFlapping", true);

        }
        else
        {
            lrAnim.SetBool("isFlapping", false);

            lr.SetActive(false);
        }
        if(Input.GetKey(",") || ulFlap)
        {
            print("< key was pressed");
            rb2d.AddTorque(torqueForce * Time.deltaTime, forceMode2d);
            rb2d.AddRelativeForce(new Vector2(-pushForce,-pushForce) * Time.deltaTime, forceMode2d);
            ul.SetActive(true);
            ulAnim.SetBool("isFlapping", true);
        }
        else
        {

            ulAnim.SetBool("isFlapping", false);
            ul.SetActive(false);
        }
        if(Input.GetKey(".") || urFlap)
        {
            print("> key was pressed");
            rb2d.AddTorque(-torqueForce * Time.deltaTime, forceMode2d);
            rb2d.AddRelativeForce(new Vector2(pushForce,-pushForce) * Time.deltaTime, forceMode2d);
            ur.SetActive(true);
            urAnim.SetBool("isFlapping", true);
        }
        else
        {

            urAnim.SetBool("isFlapping", false);
            ur.SetActive(false);
        }
    }
}
