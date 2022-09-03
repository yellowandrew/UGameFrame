using System;
using UnityEngine;

public class Block2D : Block
{
	
	
	////////private variables///////////
	Vector2 moveTo = new Vector2(0, 0);
	Vector2 scale;
	
	float speed = 0.3f;
	//string spriteName = "";
	Sprite sprite;
	Sprite upSprite, downSprite;
	void Start()
	{
		sprite = GetComponent<SpriteRenderer>().sprite;
		//spriteName = sprite.name;
	}

    public override void setSprite(Sprite s1, Sprite s2)
    {
        base.setSprite(s1, s2);
		
		upSprite = s1;
		downSprite = s2;
		GetComponent<SpriteRenderer>().sprite = s1;

	}
    //Block mouse clicked or touched 
    public override void touchDown()
	{
		GetComponent<SpriteRenderer>().sprite = downSprite;
		//sprite.name = spriteName + "_down";
		scale = this.transform.localScale;
		this.transform.localScale = new Vector2(
			scale.x + scale.x / 30,
			scale.y + scale.y / 30
		);
	}

	//Block mouse click release or touch end
	public override void touchUp()
	{
		GetComponent<SpriteRenderer>().sprite = upSprite;
		//sprite.name = spriteName;
		if (scale.x == 0) scale = this.transform.localScale;
		this.transform.localScale = new Vector2(scale.x, scale.y);
	}

	// Update is called once per frame
	// transform-tweening. if _dieAfterAnim is True, Destroy object after Tweening end.
	void Update()
	{
		if (isAnimation == false)
		{
			if (dieAfterAnim)
			{
				Destroy(gameObject);
			}
			return;
		}

		float x = (transform.localPosition.x - moveTo.x) * speed;
		float y = (transform.localPosition.y - moveTo.y) * speed;

		transform.localPosition = new Vector3(transform.localPosition.x - x,
													transform.localPosition.y - y);

		if (Math.Abs(x) < 0.01f && Math.Abs(y) < 0.01f)
        {
			transform.localPosition = moveTo;
			isAnimation = false;
		}
			
	}

	//start transform-tweening. go to argument x-position from now position (only move x axis)
	public override void moveToX(float x)
	{
		move(new Vector2(x, this.transform.localPosition.y));
	}

	//start transform-tweening. go to argument y-position from now position (only move y axis) 
	public override void moveToY(float y)
	{
		move(new Vector2(this.transform.localPosition.x, y));
	}

	//start transform-tweening. go to argument position from now position 
	public override void move(Vector2 vec)
	{
		moveTo = vec;
		isAnimation = true;
	}
}
