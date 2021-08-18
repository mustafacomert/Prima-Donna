using UnityEngine;
using TMPro;
using System.Collections;

public class SizeUpText : MonoBehaviour
{
    TextMeshProUGUI text;
    Animator animator;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        Collectables.Collected -= MakeVisible;
        Collectables.Collected += MakeVisible;
    }

    private void OnDisable()
    {
        Collectables.Collected -= MakeVisible;
    }

    private void MakeVisible()
    {
        animator.Play("TextAnim", -1, 0);
    }
}
