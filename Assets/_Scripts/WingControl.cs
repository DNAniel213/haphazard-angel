using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WingControl : NetworkBehaviour
{
    [Header("Transforms")]
    public Vector3 mostRecentPos;
    public Vector3 prevPos;
    public Quaternion mostRecentRot;
    public Quaternion prevRot;
    public Vector2 mostRecentVelocity;
    public Vector2 prevVelocity;
    public float mostRecentAngularVelocity;
    public float prevAngularVelocity;

    [SyncVar]
    public Match currentMatch;

    public Rigidbody2D rb2d;
    public float torqueForce = 5;
    public float pushForce = 5;
    ForceMode2D forceMode2d = ForceMode2D.Force;
    public GameObject ll, lr, ul, ur;
    [SyncVar]
    public bool llFlap, lrFlap, ulFlap, urFlap;
    public Animator llAnim = null, lrAnim = null, ulAnim = null, urAnim = null;
    
    [SyncVar]
    public List<NetworkPlayer> players = new List<NetworkPlayer>();



    public float smoothSpeed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        //players.Add(NetworkPlayer.localPlayer);
        if(NetworkPlayer.localPlayer != null)
            NetworkPlayer.localPlayer.angel = this;



        foreach(NetworkPlayer playerobj in players)
        {
            print(playerobj.matchID + " AA " + currentMatch.matchID);
            if(playerobj.matchID == currentMatch.matchID)
            {
                playerobj.angel = this;
            }
        }


        rb2d = GetComponent<Rigidbody2D>();

        if(isServer)
        {
            InvokeRepeating("SendTransform", 0, NetworkManagerLobby.tickrate);
        }
        else
        {
            rb2d.interpolation = RigidbodyInterpolation2D.Extrapolate;
        }
    }

    void SendTransform()
    {
        if(prevPos != transform.position)
        {
            RPCSendPos(transform.position);
            prevPos = transform.position;
        }

        if(prevRot != transform.rotation)
        {
            RPCSendRot(transform.rotation);
            prevRot = transform.rotation;
        }

        if(prevVelocity != rb2d.velocity)
        {
            RPCSendVelocity(rb2d.velocity);
            prevVelocity = rb2d.velocity;
        }

        if(prevAngularVelocity != rb2d.angularVelocity)
        {
            RPCSendAngularVelocity(rb2d.angularVelocity);
            prevAngularVelocity = rb2d.angularVelocity;
        }

    }

    [ClientRpc]
    void RPCSendPos(Vector3 pos)
    {
        mostRecentPos = pos;
    }

    [ClientRpc]
    void RPCSendRot(Quaternion rot)
    {
        mostRecentRot = rot;
    }

    [ClientRpc]
    void RPCSendVelocity(Vector2 vel)
    {
        mostRecentVelocity = vel;
    }

    [ClientRpc]
    void RPCSendAngularVelocity(float angVel)
    {
        mostRecentAngularVelocity = angVel;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        ServerDoControls();

        if(isServer)
        {
            
        }
        else
        {
            DoFlapAnimation();
            transform.position = Vector3.Slerp(transform.position, mostRecentPos, smoothSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, mostRecentRot, smoothSpeed * Time.deltaTime);
            rb2d.velocity = Vector2.Lerp(rb2d.velocity, mostRecentVelocity, smoothSpeed * Time.deltaTime);
            rb2d.angularVelocity = Mathf.Lerp(rb2d.angularVelocity, mostRecentAngularVelocity, smoothSpeed * Time.deltaTime);
        }
    }


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

    [Client]
    void DoFlapAnimation()
    {
        if (llFlap)
        {
            if(llAnim!=null)
                llAnim.SetBool("isFlapping", true);
        }
        else
        {
            if(llAnim!=null)
                llAnim.SetBool("isFlapping", false);

        }
        if(lrFlap)
        {
            if(lrAnim!=null)
                lrAnim.SetBool("isFlapping", true);

        }
        else
        {
            if(lrAnim!=null)
                lrAnim.SetBool("isFlapping", false);

        }
        if( ulFlap)
        {
            if(ulAnim!=null)
                ulAnim.SetBool("isFlapping", true);
        }
        else
        {

            if(ulAnim!=null)
                ulAnim.SetBool("isFlapping", false);
        }
        if(urFlap)
        {
            if(urAnim!=null)
                urAnim.SetBool("isFlapping", true);
        }
        else
        {

            if(urAnim!=null)
                urAnim.SetBool("isFlapping", false);
        }
    }

    

    
    //[Server]
    void ServerDoControls()
    {
        float torque = 0;
        Vector2 force = new Vector2(0,0);

        /*
        if(Input.GetKey("z"))
            llFlap = true;
        else
            llFlap = false;
            
        if(Input.GetKey("x"))
            lrFlap = true;
        else
            lrFlap = false;
        
        if(Input.GetKey(","))
            ulFlap = true;
        else
            ulFlap = false;

        if(Input.GetKey("."))
            urFlap = true;
        else
            urFlap = false;
        */


        if (llFlap)
        {
            //print("Lower Left Flap!");
            torque -= torqueForce;
            force.x -= pushForce;
            force.y += pushForce;

            //rb2d.AddTorque(-torqueForce * Time.deltaTime, forceMode2d);
            //rb2d.AddRelativeForce(new Vector2(-pushForce,pushForce) * Time.deltaTime, forceMode2d);


        }
        else
        {
        }
        if(lrFlap)
        {
            //print("Lower Right Flap");

            torque += torqueForce;
            force.x += pushForce;
            force.y += pushForce;
            //rb2d.AddTorque(torqueForce * Time.deltaTime, forceMode2d);
            //rb2d.AddRelativeForce(new Vector2(pushForce,pushForce) * Time.deltaTime, forceMode2d);


        }
        else
        {


        }
        if( ulFlap)
        {
            //print("Upper Left Flap");
            torque += torqueForce;
            force.x -= pushForce;
            force.y -= pushForce;

            //rb2d.AddTorque(torqueForce * Time.deltaTime, forceMode2d);
            //rb2d.AddRelativeForce(new Vector2(-pushForce,-pushForce) * Time.deltaTime, forceMode2d);

        }
        else
        {


        }
        if( urFlap)
        {
            //print("Upper Right Flap");
            torque -= torqueForce;
            force.x += pushForce;
            force.y -= pushForce;

            //rb2d.AddTorque(-torqueForce * Time.deltaTime, forceMode2d);
            //rb2d.AddRelativeForce(new Vector2(pushForce,-pushForce) * Time.deltaTime, forceMode2d);

        }
        else
        {

        }

        rb2d.AddTorque(torque, forceMode2d);
        rb2d.AddRelativeForce(force , forceMode2d);
    }
}
