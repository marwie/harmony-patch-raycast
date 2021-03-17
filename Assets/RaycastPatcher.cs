using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Random = UnityEngine.Random;

public class RaycastPatcher : MonoBehaviour
{
	private Harmony instance;

	public float PatchAfterSeconds = -1;
	private bool didPatch;

	[ContextMenu(nameof(PatchNow))]
	private void PatchNow()
	{
		if (instance == null)
			instance = new Harmony("MyRaycastPatch");
		Debug.Log("Patch now");
		instance.PatchAll();
		didPatch = true;
	}

	[ContextMenu(nameof(Unpatch))]
	private void Unpatch()
	{
		if (instance != null)
		{
			instance.UnpatchAll();
			didPatch = false;
		}
	}
	
	private void Update()
	{
		if (!didPatch && PatchAfterSeconds >= 0 && Time.time > PatchAfterSeconds)
		{
			PatchNow();
		}
		
		var cam = Camera.main;
		var ray = new Ray(Random.insideUnitSphere * 3f, Random.insideUnitCircle.normalized);
		Debug.DrawRay(ray.origin, ray.direction, Color.gray, .1f);
		if (Physics.Raycast(ray, out var hit))
		{
			Debug.DrawLine(ray.origin, hit.point, Color.green, .1f);
		}
	}

	[HarmonyPatch(typeof(Ray))]
	public class Patch
	{
		[HarmonyPostfix]
		[HarmonyPatch(MethodType.Getter)]
		[HarmonyPatch("origin")]
		public static void Postfix()
		{
			// var r = new Ray();
			// r.origin
			// Debug.Log("Get origin " + System.Reflection.MethodBase.GetCurrentMethod().Name);
		}
	}
}