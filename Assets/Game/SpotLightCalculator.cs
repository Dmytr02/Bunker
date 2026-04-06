using System;
using UnityEngine;

public class SpotLightCalculator : MonoBehaviour
{
    private static readonly int PointLight = Shader.PropertyToID("_Point_Light");
    private static readonly int MainColor = Shader.PropertyToID("_Main_Color");
    private static readonly int ShadowColor = Shader.PropertyToID("_Shadow_Color");

    [SerializeField]
   private Renderer meshRenderer;

   [SerializeField]
   private Transform lightPoint;
   
   [SerializeField]
   private Color mainColor = Color.white;
   [SerializeField]
   private Color shadowColor = Color.grey;
   private Material _material;

   private void Awake()
   {
     _material = new Material(meshRenderer.sharedMaterial);
     _material.SetColor(MainColor, mainColor);
     _material.SetColor(ShadowColor, shadowColor);
     meshRenderer.material = _material;
     
   }
   
   private void Update()
   {
     var direction = GetLightDirection();
     _material.SetVector(PointLight, direction);
   }

   private Vector3 GetLightDirection() =>
     transform.position - lightPoint.position;
}
