using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayController : MonoBehaviour
{
    #region FIELDS
    private float blinkTime = 0.0f;
    private float deltaTime = 1.0f;
    private IEnumerator _hitCoroutine;
    private Transform hitSignal;
    private Transform trialSignal;
    #endregion
    #region PROPERTIES
    #endregion
    #region UNITY_METHODS
    void Awake()
    {
        trialSignal = this.transform.Find("Display/Trial");
        hitSignal = this.transform.Find("Display/Hit");
    }
    #endregion
    #region PUBLIC_METHODS
    public void Trial(int trial)
    {
        trialSignal.GetComponent<Text>().text = String.Format("Trial {0}", trial);
    }
    public void Hit()
    {
        blinkTime += 6.0f*deltaTime;
        if (_hitCoroutine == null)
        {
            _hitCoroutine = Blink();
            StartCoroutine(Blink());
        }
    }
    #endregion
    #region PRIVATE_METHODS
    // every 2 seconds perform the print()
    private IEnumerator Blink()
    {
        while (blinkTime > 0.0f)
        {
            hitSignal.gameObject.SetActive (!hitSignal.gameObject.activeInHierarchy);
            yield return new WaitForSeconds(deltaTime);
            blinkTime = (blinkTime > deltaTime)? (blinkTime-deltaTime) : 0.0f;
        }
        _hitCoroutine = null;
    }
    #endregion
}
