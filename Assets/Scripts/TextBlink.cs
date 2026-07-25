using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBlink : MonoBehaviour
{
    [SerializeField] private float minBlinkInterval = 0.5f;
    [SerializeField] private float maxBlinkInterval = 5.0f;
    [SerializeField] private float fadeDuration = 0.3f;
    
    private Text _text;
    private Coroutine _blinkCoroutine;
    
    void Awake()
    {
        _text = GetComponent<Text>();
        if (_text == null)
        {
            Debug.LogError("Text component not found on " + gameObject.name);
        }
    }
    
    void OnEnable()
    {
        StartBlinking();
    }
    
    void OnDisable()
    {
        StopBlinking();
    }
    
    public void StartBlinking()
    {
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
        }
        _blinkCoroutine = StartCoroutine(BlinkLoop());
    }
    
    public void StopBlinking()
    {
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }
        if (_text != null)
        {
            Color c = _text.color;
            c.a = 1f;
            _text.color = c;
        }
    }
    
    private IEnumerator BlinkLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minBlinkInterval, maxBlinkInterval);
            yield return new WaitForSeconds(waitTime);
            
            yield return StartCoroutine(FadeText(1f, 0.3f));
            yield return new WaitForSeconds(0.1f);
            yield return StartCoroutine(FadeText(0.3f, 1f));
        }
    }
    
    private IEnumerator FadeText(float startAlpha, float endAlpha)
    {
        if (_text == null) yield break;
        
        Color startColor = _text.color;
        startColor.a = startAlpha;
        
        Color endColor = _text.color;
        endColor.a = endAlpha;
        
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            _text.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        
        _text.color = endColor;
    }
}