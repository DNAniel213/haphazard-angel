using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysUpright : MonoBehaviour
{
    public GameObject parent;
    public Quaternion rotation;
    // Start is called before the first frame update
    void Awake()
    {
        rotation = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //this.gameObject.transform.Rotate(new Vector3(0,0,parent.transform.rotation.z * -1), Space.Self);
        transform.rotation = rotation;
    }
}
