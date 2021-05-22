using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
//offset from the viewport center to fix damping
    public float m_DampTime = 10f;
    public Transform m_Target;
    public float m_XOffset = 0;
    public float m_YOffset = 0;
	public Vector2 minimumBoundary, maximumBoundary;

	private float margin = 0.01f;

	void Start () {
		if (m_Target==null){
			m_Target = GameObject.FindGameObjectWithTag("Player").transform;
		}
	}

    void FixedUpdate() {
        if(m_Target) {
			float targetX = m_Target.position.x + m_XOffset;
			float targetY = m_Target.position.y + m_YOffset;

			if (Mathf.Abs(transform.position.x - targetX) > margin )
				targetX = Mathf.Lerp(transform.position.x, targetX, 1/m_DampTime * Time.deltaTime);

			if (Mathf.Abs(transform.position.y - targetY) > margin)
				targetY = Mathf.Lerp(transform.position.y, targetY, m_DampTime * Time.deltaTime);
            
			transform.position = new Vector3(targetX, targetY, transform.position.z);

			transform.position = new Vector3
			(
				Mathf.Clamp (transform.position.x, minimumBoundary.x, maximumBoundary.x),
				Mathf.Clamp (transform.position.y, minimumBoundary.y, maximumBoundary.y),
				transform.position.z
			);
        }
    }
}
