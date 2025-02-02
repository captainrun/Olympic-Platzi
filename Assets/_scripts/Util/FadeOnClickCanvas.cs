﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeOnClickCanvas : MonoBehaviour
{
    public TextMeshProUGUI textFade;
    private bool _goOn;
    private Coroutine _cFade;
    public Image imgFade;

    private void Awake()
    {
        if (textFade != null)
            InvokeRepeating(nameof(Fade), 0.05f, 0.05f);
        _goOn = false;
    }

    private void Update()
    {
        if (Input.anyKey && _cFade == null)
        {
            _cFade = StartCoroutine(CFade());
        }
    }

    /// <summary>
    /// Toggle color alpha of textFade (Invoked on awake) 
    /// </summary>
    private void Fade()
    {
        switch (textFade is null)
        {
            case false when textFade.color.a <= 0:
                _goOn = true;
                break;
            case false when textFade.color.a >= 1:
                _goOn = false;
                break;
        }

        if (!(textFade is null))
            textFade.color = _goOn
                ? new Color(textFade.color.r, textFade.color.g, textFade.color.b, textFade.color.a + 0.05f)
                : new Color(textFade.color.r, textFade.color.g, textFade.color.b, textFade.color.a - 0.05f);
    }

    /// <summary>
    /// Make fade effect in imgFade
    /// </summary>
    /// <returns></returns>
    private IEnumerator CFade()
    {
        CancelInvoke();
        for (float i = 0; i < 1; i += 0.05f)
        {
            imgFade.color = new Color(255, 255, 255, imgFade.color.a - i);
            textFade.color = new Color(255, 255, 255, imgFade.color.a - i);
            yield return new WaitForSecondsRealtime(0.05f);
        }

        gameObject.SetActive(false);
    }
}