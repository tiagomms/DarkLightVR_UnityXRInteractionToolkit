using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpiritAnimationManager : MonoBehaviour {
	private Animator anim;
    private float lsOriginalMatAlphaValue;
    private bool isLockedAnimOn = false;
    private bool isRandomAnimOn = false;

    public static float FADE_TO_DURATION = 5.0f;

    public bool IsLockedAnimOn
    {
        get
        {
            return isLockedAnimOn;
        }

        set
        {
            isLockedAnimOn = value;
        }
    }

    public float LsOriginalMatAlphaValue
    {
        get
        {
            return lsOriginalMatAlphaValue;
        }
    }

    public bool IsRandomAnimOn
    {
        get
        {
            return isRandomAnimOn;
        }

        set
        {
            isRandomAnimOn = value;
        }
    }

    private void Awake()
	{
        // generate random seed everytime
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

		anim = gameObject.GetComponent<Animator>();
        lsOriginalMatAlphaValue = gameObject.GetComponentInChildren<Renderer>().material.color.a;
    }

	public void SetActionByValue(int actionValue, bool forceAnimation = false) {
		if (forceAnimation) {
            IsLockedAnimOn = false;
        }
        if (!IsLockedAnimOn) {
        	anim.SetInteger("Action", actionValue);
		}
    }

    public void SetRandomAnimation(bool forceAnimation = false) {
        float randProb = UnityEngine.Random.value;
        int action = 0;
        if (randProb < 0.01f)
        {
            action = (int)LightSpiritsController.LSAnimations.STILL;
        }
        else if (randProb < 0.85f)
        {
            action = (int)LightSpiritsController.LSAnimations.IDLE;
        }
        else
        {
            action = (int)LightSpiritsController.LSAnimations.FLYING;
        }
        SetActionByValue(action, forceAnimation);
    }

    public bool IsCurrentAnimState(string stateName) {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    public void FadeToAlphaLightSpirit(float alphaTo = 0f) {
        LeanTween.alpha(gameObject, alphaTo, FADE_TO_DURATION)
            .setEase(LeanTweenType.easeInOutSine);
    }
    public void FadeBackToOriginalAlphaLightSpirit() {
        FadeToAlphaLightSpirit(LsOriginalMatAlphaValue);
    }

    public IEnumerator StartRandomAnimationCycle() {
        while (IsRandomAnimOn) {
            SetRandomAnimation(true);
            yield return new WaitForSecondsRealtime(5.0f);
        }
    }
}
