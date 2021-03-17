using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class RaycastPatcher : MonoBehaviour
{
	private Harmony instance;

	public bool EnableDebug = true;
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

	private void Start()
	{
		if (PatchAfterSeconds >= 0)
			Debug.Log(
				"Will auto patch after " + PatchAfterSeconds +
				" second(s), set this to < 0 to disable auto patching. you can also manually patch it using the PatchNow context menu on " + this, this);
	}

	private void Update()
	{
		if (!Application.isPlaying) return;
		
		if(EnableDebug)
			Harmony.DEBUG = true;
		
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
			// var r = new Ray().origin;
			// Debug.Log("Get origin " + System.Reflection.MethodBase.GetCurrentMethod().Name);
		}
	}
}