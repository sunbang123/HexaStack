using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Renderer))]
public class Tower : MonoBehaviour
{
    [Header("Elements")]
    //[SerializeField] private Animator animator;
    private Renderer _renderer;

    [Header("Settings")]
    [SerializeField] private float fillIncrement;
    [SerializeField] private float maxFillPercent;
    private float fillPercent;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        UpdateMaterials();
    }

    void Update()
    {
        bool mouseHeld = Mouse.current != null && Mouse.current.leftButton.isPressed;
        bool touchHeld = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;
        
        if (mouseHeld || touchHeld)
            Fill();
    }

    private void Fill()
    {
        if (fillPercent >= 1)
            return;

        fillPercent += fillIncrement;
        UpdateMaterials();

        //animator.Play("Bump");
    }

    private void UpdateMaterials()
    {
        foreach(Material material in _renderer.materials)
        {
            material.SetFloat("_FillPercent", fillPercent * maxFillPercent);
        }
    }
}
