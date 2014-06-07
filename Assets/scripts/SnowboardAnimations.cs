using System;
using UnityEngine;

public class SnowboardAnimations : MonoBehaviour
{
    public static string air = "air";
    private string carvefastleft = "carveFastRight";
    private string carvefastright = "carveFastLeft";

    // Падения
    private string fallback = "fallback";
    private string fallfront = "fallfront";

    //
    private bool isJumped;
    private float oldSideCoef = 0;
    private AnimationState currentAnimation;
    
    public void setupSimple(string name, int layer, float speed, WrapMode mode)
    {
        base.animation[name].enabled = true;
        base.animation[name].weight = 1f;
        base.animation[name].layer = layer;
        base.animation[name].speed = speed;
        base.animation[name].wrapMode = mode;
        base.animation[name].blendMode = AnimationBlendMode.Blend;
    }

    private void Start()
    {
        int layer = 1;

        this.setupSimple(air, layer, 0.3f, WrapMode.Loop);
        this.setupSimple(this.carvefastright, layer, 0f, WrapMode.ClampForever);
        this.setupSimple(this.carvefastleft, layer, 0f, WrapMode.ClampForever);

        //Анимации падения
        this.setupSimple(this.fallfront, layer, 1f, WrapMode.ClampForever);
        base.animation[this.fallfront].normalizedTime = 1f;
        this.setupSimple(this.fallback, layer, 1f, WrapMode.ClampForever);
        base.animation[this.fallback].normalizedTime = 1f;

        base.animation.Stop();
        base.animation.Play(this.carvefastleft);
        currentAnimation = base.animation[this.carvefastleft];
        base.animation[this.carvefastleft].normalizedTime = 0f;
        isJumped = false;
    }

    public void setSide(float x)
    {
        if (!isJumped) {
            if (x > 0 && oldSideCoef <= 0) {
                base.animation.CrossFade(this.carvefastright, 0, PlayMode.StopAll);
                currentAnimation = base.animation[this.carvefastright];
            } else
            if (x <= 0 && oldSideCoef >= 0) {
                base.animation.CrossFade(this.carvefastleft, 0, PlayMode.StopAll);
                currentAnimation = base.animation[this.carvefastleft];
            }

            oldSideCoef = x;
            currentAnimation.normalizedTime = Math.Min(Math.Abs(x), 1);
        }
    }

    /** Прыжок. */
    public void jump()
    {
        isJumped = true;
        base.animation.CrossFade(SnowboardAnimations.air, 0.2f, PlayMode.StopAll);
        currentAnimation = base.animation[SnowboardAnimations.air];
    }

    /** Приземление. */
    public void landed()
    {
        isJumped = false;
        base.animation.CrossFade(this.carvefastleft, 0.3f, PlayMode.StopAll);
        currentAnimation = base.animation[this.carvefastleft];
        currentAnimation.normalizedTime = 0;
        oldSideCoef = 0;
    }
}

