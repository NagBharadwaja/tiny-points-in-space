using UnityEngine;
using System.Collections;

public class UiCircle: MonoBehaviour {

	float currentAlpha = 0.0f;
	float currentScale = 0.0f;
	float currentZpos = 0.0f;

	float targetAlpha = 0.0f;
	float targetScale = 0.0f;
	float targetZpos = 0.0f;

	float animationDuration = 0.0f;
	float animationStartTime = 0.0f;
	bool isAnimating = false;
	
	void Update () {
		if (isAnimating) {
			if (Time.time > animationStartTime + animationDuration) {
				SetAlpha(targetAlpha);
				SetScale(targetScale);
				SetZPos(targetZpos);
				isAnimating = false;
			} else {
				SetAlpha(AnimateFloat(currentAlpha, targetAlpha));
				SetScale(AnimateFloat(currentScale, targetScale));
				SetZPos(AnimateFloat(currentZpos, targetZpos));
			}
		}
	}
	
	void SetAlpha (float alpha) {
		Color aColor = renderer.material.color;
		aColor.a = alpha;
		renderer.material.color = aColor;
		currentAlpha = alpha;
	}

	void SetScale (float scale) {
		transform.localScale = new Vector3(scale, 1, scale);
		currentScale = scale;
	}

	void SetZPos (float zPos) {
		transform.localPosition = Vector3.forward * zPos;
		currentZpos = zPos;
	}

	void SetAnimationDuration (float time) {
		animationDuration = time;
	}

	void StartAnimation () {
		animationStartTime = Time.time;
		isAnimating = true;
	}

	float AnimateFloat (float current, float target) {
		return current + (target - current) * ((Time.time - animationStartTime) / (animationDuration * 2));
	}

	void AnimateAlpha (float newAlpha) {
		targetAlpha = newAlpha;
		StartAnimation();
	}

	void AnimateScale (float newScale) {
		targetScale = newScale;
		StartAnimation();
	}

	void AnimateZPos (float newZpos) {
		targetZpos = newZpos;
		StartAnimation();
	}

}
