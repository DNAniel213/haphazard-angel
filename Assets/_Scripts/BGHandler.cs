using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGHandler : MonoBehaviour {

	public float verticalDifference = 0, horizontalDifference = 0;
	public WingControl angel;
    public float scrollSpeed = 1;
	public GameObject background, middleGround;
	
	// Use this for initialization
	void Awake () {
		angel = GameObject.Find("Angel(Clone)").GetComponent<WingControl>();
	}

	private void Start() {
        transform.position = new Vector3(Mathf.Round(angel.transform.position.x), 0, 0); 
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(angel!=null)
		{
			if (angel.rb2d.velocity.y > 0 && verticalDifference<=0.8)
				MoveBackgroundDownward();
			else if (angel.rb2d.velocity.y < 0 && verticalDifference>=-0.8)
				MoveBackgroundUpward();

			if (angel.rb2d.velocity.x > 0 && horizontalDifference<=0.8)
				MoveBackgroundRight();
			else if (angel.rb2d.velocity.x < 0 && horizontalDifference>=-0.8)
				MoveBackgroundLeft();
		
		}

	}

	void MoveBackgroundDownward()
	{
		float moveRate = angel.rb2d.velocity.y * scrollSpeed;
		background.transform.position -= new Vector3(0,moveRate,0);
		middleGround.transform.position -= new Vector3(0,moveRate/2,0);

		verticalDifference += moveRate;

	}

		void MoveBackgroundRight()
	{
		float moveRate = angel.rb2d.velocity.y * scrollSpeed;
		background.transform.position -= new Vector3(moveRate,0,0);
		middleGround.transform.position -= new Vector3(moveRate/2,0,0);

		horizontalDifference += moveRate;

	}

	void MoveBackgroundUpward()
	{
		float moveRate = angel.rb2d.velocity.y *  scrollSpeed;
		background.transform.position -= new Vector3(0,moveRate,0);
		middleGround.transform.position -= new Vector3(0,moveRate/2,0);
		

		verticalDifference += moveRate;
	}

		void MoveBackgroundLeft()
	{
		float moveRate = angel.rb2d.velocity.y *  scrollSpeed;
		background.transform.position -= new Vector3(moveRate,0,0);
		middleGround.transform.position -= new Vector3(moveRate/2,0,0);
		

		horizontalDifference += moveRate;
	}

	public void DeployDeathBackground()
	{

	}



}


