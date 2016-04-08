﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PathRequestManager : MonoBehaviour {
	
	static PathRequestManager instance;

	Queue <PathRequest> pathRequestQueue = new Queue <PathRequest>();
	PathRequest currentPathRequest;
	Pathfinding pathingController;
	bool isProcessingPath;

	void Awake ()
	{
		instance = this;
		pathingController = GetComponent<Pathfinding>();
	}

	public static void RequestPath (Vector2 pathStart, Vector2 pathEnd, Action <Vector2[], bool> callback)
	{
		PathRequest newRequest = new PathRequest (pathStart, pathEnd, callback);
		instance.pathRequestQueue.Enqueue (newRequest);
		instance.TryProcessNext ();
	}

	void TryProcessNext ()
	{
		if (!isProcessingPath && pathRequestQueue.Count > 0)
		{
			currentPathRequest = pathRequestQueue.Dequeue ();
			isProcessingPath = true;
			pathingController.StartFindPath (currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}
	}

	public void FinishedProcessingPath (Vector2[] path, bool success)
	{
		currentPathRequest.callback(path,success);
		isProcessingPath = false;
		TryProcessNext ();
	}
	struct PathRequest
	{
		public Vector2 
		pathStart,
		pathEnd;
		public Action <Vector2[], bool> callback;

		public PathRequest (Vector2 _start, Vector2 _end, Action <Vector2[], bool> _callback)
		{
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
		}
	}
}