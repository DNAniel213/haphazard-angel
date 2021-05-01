using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WingControl : NetworkBehaviour
{
    public Rigidbody2D rb2d;
    public float torqueForce = 5;
    public float pushForce = 5;
    ForceMode2D forceMode2d = ForceMode2D.Force;
    public GameObject ll, lr, ul, ur;
    [SyncVar]
    public bool llFlap, lrFlap, ulFlap, urFlap;
    public Animator llAnim = null, lrAnim = null, ulAnim = null, urAnim = null;
    

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
        if(isServer)
        {
            DoControls();
        }
      
    }

    [Command(requiresAuthority = false)]

    public void SetFlap(PlayerPosition pos, bool toggle)
    {

        if(toggle)
            {
                switch(pos)
                {
                    case PlayerPosition.ULEFT : ulFlap = true; break;
                    case PlayerPosition.URIGHT : urFlap = true; break;
                    case PlayerPosition.LLEFT :  llFlap = true; break;
                    case PlayerPosition.LRIGHT : lrFlap = true; break;
                }
            }
            else
            {

                switch(pos)
                {
                    case PlayerPosition.ULEFT : ulFlap = false; break;
                    case PlayerPosition.URIGHT : urFlap = false; break;
                    case PlayerPosition.LLEFT : llFlap = false; break;
                    case PlayerPosition.LRIGHT : lrFlap = false; break;
                }
            }

    }



    
    [Server]
    void DoControls()
    {
        if (Input.GetKey("z") || llFlap)
        {
            print("z key was pressed");
            rb2d.AddTorque(-torqueForce * Time.deltaTime, forceMode2d);
            rb2d.AddRelativeForce(new Vector2(-pushForce,pushForce) * Time.deltaTime, forceMode2d);
            ll.SetActive(true);
            if(llAnim!=null)
                llAnim.SetBool("isFlapping", true);
        }
        else
        {
            ll.SetActive(false);
            if(llAnim!=null)
                llAnim.SetBool("isFlapping", false);

        }
        if(Input.GetKey("x") || lrFlap)
        {
            print("x key was pressed");
            rb2d.AddTorque(torqueForce * Time.deltaTime, forceMode2d);
            rb2d.AddRelativeForce(new Vector2(pushForce,pushForce) * Time.deltaTime, forceMode2d);
            lr.SetActive(true);
            if(lrAnim!=null)
                lrAnim.SetBool("isFlapping", true);

        }
        else
        {
            if(lrAnim!=null)
                lrAnim.SetBool("isFlapping", false);

            lr.SetActive(false);
        }
        if(Input.GetKey(",") || ulFlap)
        {
            print("< key was pressed");
            rb2d.AddTorque(torqueForce * Time.deltaTime, forceMode2d);
            rb2d.AddRelativeForce(new Vector2(-pushForce,-pushForce) * Time.deltaTime, forceMode2d);
            ul.SetActive(true);
            if(ulAnim!=null)
                ulAnim.SetBool("isFlapping", true);
        }
        else
        {

            if(ulAnim!=null)
                ulAnim.SetBool("isFlapping", false);
            ul.SetActive(false);
        }
        if(Input.GetKey(".") || urFlap)
        {
            print("> key was pressed");
            rb2d.AddTorque(-torqueForce * Time.deltaTime, forceMode2d);
            rb2d.AddRelativeForce(new Vector2(pushForce,-pushForce) * Time.deltaTime, forceMode2d);
            ur.SetActive(true);
            if(urAnim!=null)
                urAnim.SetBool("isFlapping", true);
        }
        else
        {

            if(urAnim!=null)
                urAnim.SetBool("isFlapping", false);
            ur.SetActive(false);
        }
    }
}
