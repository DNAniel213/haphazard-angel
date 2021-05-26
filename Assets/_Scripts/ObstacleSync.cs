using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ObstacleSync : NetworkBehaviour
{
    private Rigidbody2D rb2d;
    [Header("Transforms")]
    public Vector3 mostRecentPos;
    public Vector3 prevPos;
    public Quaternion mostRecentRot;
    public Quaternion prevRot;
    public Vector2 mostRecentVelocity;
    public Vector2 prevVelocity;
    public float mostRecentAngularVelocity;
    public float prevAngularVelocity;
    public float smoothSpeed = 50f;

    private void Start() {
        rb2d= GetComponent<Rigidbody2D>();
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
        if(prevPos != transform.position || prevRot != transform.rotation)
        {
            RPCSendPos(transform.position, transform.rotation);
            prevPos = transform.position;
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
    void RPCSendPos(Vector3 pos, Quaternion rot)
    {
        mostRecentPos = pos;
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
        if(isServer)
        {
            
        }
        else
        {
            transform.SetPositionAndRotation(Vector3.Lerp(transform.position, mostRecentPos, smoothSpeed * Time.deltaTime),
                                              Quaternion.Lerp(transform.rotation, mostRecentRot, smoothSpeed * Time.deltaTime));
            rb2d.velocity = Vector2.Lerp(rb2d.velocity, mostRecentVelocity, smoothSpeed * Time.deltaTime * 2);
            rb2d.angularVelocity = Mathf.Lerp(rb2d.angularVelocity, mostRecentAngularVelocity, smoothSpeed * Time.deltaTime * 2);
        }
    }

}