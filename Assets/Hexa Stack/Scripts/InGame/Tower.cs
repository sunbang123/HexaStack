using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Tower : MonoBehaviour
{
    [Header("Elements")]
    //[SerializeField] private Animator animator;
    private Renderer renderer;

    [Header("Settings")]
    [SerializeField] private float fillIncrement;
    [SerializeField] private float maxFillPercent;
    private float fillPercent;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        UpdateMaterials();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
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
        foreach(Material material in renderer.materials)
        {
            material.SetFloat("_FillPercent", fillPercent * maxFillPercent);
        }
    }
}
