using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class IrisMove : MonoBehaviour
{
    // Start is called before the first frame update
    public LevelManager levelManager;
    public float nearestOrb =999;
    public Transform nearestOrbTransform = null;
    public float speed = 0.001f;
    public GameObject parent;

    public void Start()
    {
        if(NetworkPlayer.localPlayer == null)
            InvokeRepeating("SearchNearest", 1.0f, 1.0f);
    }
    private void SearchNearest()
    {
        if(nearestOrbTransform != null)
        {
            if(!nearestOrbTransform.gameObject.activeSelf)
            {
                nearestOrb = 999;
                nearestOrbTransform = null;
                this.transform.localPosition = Vector3.zero;
            }

        }
        else
        {
            GameObject[] pointOrbs = GameObject.FindGameObjectsWithTag("Point");

            if(pointOrbs.Length > 0 )
            {
                foreach(GameObject orb in pointOrbs)
                {
                    if(orb.activeSelf)
                    {
                        float dist = Vector2.Distance(this.transform.position, orb.transform.position);
                        if(dist < nearestOrb)
                        {
                            nearestOrb = dist;
                            nearestOrbTransform = orb.transform;
                        }
                    }

                }
            }
            else
            {
                nearestOrb = 999;
                nearestOrbTransform = null;
                this.transform.localPosition = Vector3.zero;
            }
        }


    }

    private void FixedUpdate() {
        //this.gameObject.transform.Rotate(new Vector3(0,0,parent.transform.rotation.z * -1), Space.Self);

        if(nearestOrbTransform != null)
        {
            Vector3 pos = transform.position;
            Vector3 target = pos - nearestOrbTransform.position ;
            target.Normalize();
            float factor = Time.deltaTime * speed;


            this.transform.localPosition = new Vector3(Mathf.Clamp(target.x * -1, -0.2f, 0.2f), Mathf.Clamp(target.y * -1, -0.1f, 0.1f),0);
            //this.transform.localPosition = new Vector3(Mathf.Clamp((pos.x - target.x) * -1, -0.32f, 0.32f), Mathf.Clamp((pos.y - target.y) *-1, -0.13f, 0.343f),0);
            //this.transform.Translate(Mathf.Clamp( target.x, -0.32f, 0.32f) * factor,   Mathf.Clamp(target.y, -0.13f, 0.343f) * factor, 0, Space.World);
        }
    }

    // 0.383 to -0.142
    //-0.38 to 0.38
    void Update()
    {
        
    }
}
