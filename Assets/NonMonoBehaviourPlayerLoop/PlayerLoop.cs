using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NonMonoBehaviourPlayerLoop
{
	public static class PlayerLoop
	{
		private static EditorCallbackStorage _editorCallbackStorage = new EditorCallbackStorage();
		public static ulong Frame { get; private set; }
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init()
		{
			Frame = 0;
			var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
#if UNITY_EDITOR
			CheckIntegrity(currentPlayerLoopSystem);
			// Make sure we unregister before we do register.
			EditorApplication.playModeStateChanged -= OnPlayModeState;
			EditorApplication.playModeStateChanged += OnPlayModeState;
#endif
			currentPlayerLoopSystem.subSystemList[UpdateType.Update.ToIndex()].updateDelegate += FrameCounter;
			UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
		}

#if UNITY_EDITOR
		private static void CheckIntegrity(UnityEngine.LowLevel.PlayerLoopSystem playerLoopSystem)
		{
			for (var uType = UpdateType.EarlyUpdate; uType <= UpdateType.PostLateUpdate; uType++)
				Debug.Assert(playerLoopSystem.subSystemList[uType.ToIndex()].type == uType.ToType(),
					$"Fatal Error: Unity player-loop incompatible ({uType})!");
		}

		private static void OnPlayModeState(PlayModeStateChange state)
		{
			switch (state)
			{
				case PlayModeStateChange.EnteredEditMode:
					var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
					UnregisterAll(ref currentPlayerLoopSystem);
					UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
					EditorApplication.playModeStateChanged -= OnPlayModeState;
					break;

				case PlayModeStateChange.EnteredPlayMode: break;
				case PlayModeStateChange.ExitingEditMode: break;
				case PlayModeStateChange.ExitingPlayMode: break;
				default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
			}
		}

		private static void UnregisterAll(ref UnityEngine.LowLevel.PlayerLoopSystem defPls)
		{
			defPls.subSystemList[UpdateType.Update.ToIndex()].updateDelegate -= FrameCounter;
			foreach(var earlyUpdate in _editorCallbackStorage.EarlyUpdates) defPls.subSystemList[UpdateType.EarlyUpdate.ToIndex()]
			    .updateDelegate -= earlyUpdate.OnEarlyUpdate; 
			foreach(var fixedUpdate in _editorCallbackStorage.FixedUpdates) defPls.subSystemList[UpdateType.FixedUpdate.ToIndex()]
			    .updateDelegate -= fixedUpdate.OnFixedUpdate; 
			foreach(var preUpdate in _editorCallbackStorage.PreUpdates) defPls.subSystemList[UpdateType.PreUpdate.ToIndex()]
			    .updateDelegate -= preUpdate.OnPreUpdate; 
			foreach(var update in _editorCallbackStorage.Updates) defPls.subSystemList[UpdateType.Update.ToIndex()]
			    .updateDelegate -= update.OnUpdate; 
			foreach(var preLateUpdate in _editorCallbackStorage.PreLateUpdates) defPls.subSystemList[UpdateType.PreLateUpdate.ToIndex()]
			    .updateDelegate -= preLateUpdate.OnPreLateUpdate; 
			foreach(var postLateUpdate in _editorCallbackStorage.PostLateUpdates) defPls.subSystemList[UpdateType.PostLateUpdate.ToIndex()]
			    .updateDelegate -= postLateUpdate.OnPostLateUpdate; 
			
			_editorCallbackStorage.EarlyUpdates.Clear();
			_editorCallbackStorage.FixedUpdates.Clear();
			_editorCallbackStorage.PreUpdates.Clear();
			_editorCallbackStorage.Updates.Clear();
			_editorCallbackStorage.PreLateUpdates.Clear();
			_editorCallbackStorage.PostLateUpdates.Clear();
		}
		
#endif

		private static void FrameCounter() => Frame++;

		public static void AddListener(IEarlyUpdate client)
		{
#if UNITY_EDITOR
			_editorCallbackStorage.EarlyUpdates.Add(client);
#endif
			var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
			currentPlayerLoopSystem.subSystemList[UpdateType.EarlyUpdate.ToIndex()].updateDelegate +=
				client.OnEarlyUpdate;
			UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
		}

		public static void AddListener(IFixedUpdate client)
		{
#if UNITY_EDITOR
			_editorCallbackStorage.FixedUpdates.Add(client);
#endif
			var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
			currentPlayerLoopSystem.subSystemList[UpdateType.FixedUpdate.ToIndex()].updateDelegate += client.OnFixedUpdate;
			UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
		}

		public static void AddListener(IPreUpdate client)
		{
#if UNITY_EDITOR
			_editorCallbackStorage.PreUpdates.Add(client);
#endif
			var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
			currentPlayerLoopSystem.subSystemList[UpdateType.PreUpdate.ToIndex()].updateDelegate += client.OnPreUpdate;
			UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
		}

		public static void AddListener(IUpdate client)
		{
#if UNITY_EDITOR
			_editorCallbackStorage.Updates.Add(client);
#endif
			var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
			currentPlayerLoopSystem.subSystemList[UpdateType.Update.ToIndex()].updateDelegate += client.OnUpdate;
			UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
		}

		public static void AddListener(IPreLateUpdate client)
		{
#if UNITY_EDITOR
			_editorCallbackStorage.PreLateUpdates.Add(client);
#endif
			var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
			currentPlayerLoopSystem.subSystemList[UpdateType.PreLateUpdate.ToIndex()].updateDelegate += client.OnPreLateUpdate;
			UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
		}

		public static void AddListener(IPostLateUpdate client)
		{
#if UNITY_EDITOR
			_editorCallbackStorage.PostLateUpdates.Add(client);
#endif
			var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
			currentPlayerLoopSystem.subSystemList[UpdateType.PostLateUpdate.ToIndex()].updateDelegate += client.OnPostLateUpdate;
			UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
		}

		public static void RemoveListener(IEarlyUpdate client)
		{
			var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
			currentPlayerLoopSystem.subSystemList[UpdateType.EarlyUpdate.ToIndex()].updateDelegate -= client.OnEarlyUpdate;
			UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
#if UNITY_EDITOR
			_editorCallbackStorage.EarlyUpdates.Remove(client);
#endif
		}

		public static void RemoveListener(IFixedUpdate client)
		{
			var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
			currentPlayerLoopSystem.subSystemList[UpdateType.FixedUpdate.ToIndex()].updateDelegate -= client.OnFixedUpdate;
			UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
#if UNITY_EDITOR
			_editorCallbackStorage.FixedUpdates.Remove(client);
#endif
		}

		public static void RemoveListener(IPreUpdate client)
		{
			var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
			currentPlayerLoopSystem.subSystemList[UpdateType.PreUpdate.ToIndex()].updateDelegate -= client.OnPreUpdate;
			UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
#if UNITY_EDITOR
			//_preUpdates.Remove(client);
#endif
		}

		public static void RemoveListener(IUpdate client)
		{
			var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
			currentPlayerLoopSystem.subSystemList[UpdateType.Update.ToIndex()].updateDelegate -= client.OnUpdate;
			UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
#if UNITY_EDITOR
			//_updates.Remove(client);
#endif
		}

		public static void RemoveListener(IPreLateUpdate client)
		{
			var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
			currentPlayerLoopSystem.subSystemList[UpdateType.PreLateUpdate.ToIndex()].updateDelegate -= client.OnPreLateUpdate;
			UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
#if UNITY_EDITOR
			//_preLateUpdates.Remove(client);
#endif
		}

		public static void RemoveListener(IPostLateUpdate client)
		{
			var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
			currentPlayerLoopSystem.subSystemList[UpdateType.PostLateUpdate.ToIndex()].updateDelegate -= client.OnPostLateUpdate;
			UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
#if UNITY_EDITOR
			//_postLateUpdates.Remove(client);
#endif
		}
	}
}