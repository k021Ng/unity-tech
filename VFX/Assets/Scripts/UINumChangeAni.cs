using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Text))]
public class UINumChangeAni : MonoBehaviour 
{
	public Animator endAnimator;
	public float interval;
	// public bool isFloat;

	private int fromVal;
	private int toVal;
	private int curVal;
	private string strToVal;

	private Text label = null;
	private bool isChanging = false;

	private bool isInit = false;
	private bool isUp = true;
	private int offset = 0;
	private float tick = 0;
	private int stepCount = 0;
	private int addups = 0;

	private void Awake()
	{
		interval = Mathf.Max(0.017f, interval);

		label = GetComponent<Text>();
		DebugUtil.Assert(label != null, "CHECK");
	}

	// test
// 	private void Update()
// 	{
// #if UNITY_EDITOR
// 		if (Input.GetKeyDown(KeyCode.UpArrow))
// 			ChangeToVal(curVal + 21);
// 		else if (Input.GetKeyDown(KeyCode.DownArrow))
// 			ChangeToVal(curVal - 21);
// #endif
// 	}

	private void LateUpdate()
	{
		if (!TryCheckInit())
			return;
		
		TryCheckValueChange();
		if (isChanging == false)
			return;

		float dt = Time.deltaTime;
		if (isChanging)
		{
			tick -= dt;
			if (tick <= 0)
			{
				tick += interval;
				// should calc digitNum before stepCount change
				int digitNum = stepCount / 10 + 1;

				stepCount += 1;
				addups += 1;

				for (int i = 1; i < digitNum; ++i)
				{
					int toadd = (int)Mathf.Pow(10, i);
					if (isUp && toadd >= toVal - curVal)
						break;
					else if (!isUp && toadd >= curVal - toVal)
						break;
					
					addups += toadd;
				}

				if (addups >= offset)
				{
					isChanging = false;
					addups = offset;
				}

				if (isUp)
					curVal = fromVal + addups;
				else
					curVal = fromVal - addups;

				UpdateText();

				if (isChanging == false && endAnimator != null)
				{
					if (isUp)
						endAnimator.SetTrigger("up");
					else
						endAnimator.SetTrigger("down");
				}
			}
		}
	}

	private bool TryCheckInit()
	{
		if (isInit)
			return true;

		int val = 0;
		if (int.TryParse(label.text, out val) == false)
			return false;

		isInit = true;
		curVal = val;
		fromVal = val;
		toVal = val;
		strToVal = curVal.ToString();

		return true;
	}

	private void TryCheckValueChange()
	{
		if (isChanging)
			return;

		if (string.Equals(label.text, strToVal))
			return;

		int val = 0;
		if (int.TryParse(label.text, out val) == false)
			return;

		ChangeToVal(val);
	}

	private void Reset()
	{
		addups = 0;
		stepCount = 0;
		tick = interval;
	}

	public void ChangeToVal(int val)
	{
		if (toVal == val)
			return;

		Reset();
		isChanging = true;

		fromVal = curVal;
		toVal = val;
		strToVal = toVal.ToString();

		if (toVal > fromVal)
		{
			isUp = true;
			offset = toVal - fromVal;
			stepCount = fromVal % 10;
		}
		else
		{
			isUp = false;
			offset = fromVal - toVal;
			stepCount = 9 - fromVal % 10;
		}
	}

	private void UpdateText()
	{
		label.text = curVal.ToString();
	}
}
