using System;
using System.Linq;
using NonMonoBehaviourPlayerLoop.Callbacks;
using UnityEditor;
using UnityEngine;

namespace NonMonoBehaviourPlayerLoop
{
    public static class PlayerLoop
    {
        private static readonly EditorCallbackStorage _editorCallbackStorage = new EditorCallbackStorage();
        public static ulong Frame { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            Frame = 0;
            var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
#if UNITY_EDITOR
            CheckIntegrity(currentPlayerLoopSystem);

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

        private static void UnregisterAll(ref UnityEngine.LowLevel.PlayerLoopSystem playerLoopSystem)
        {
            playerLoopSystem.subSystemList[UpdateType.Update.ToIndex()].updateDelegate -= FrameCounter;
            foreach (var earlyUpdate in _editorCallbackStorage.EarlyUpdates)
                playerLoopSystem.subSystemList[UpdateType.EarlyUpdate.ToIndex()]
                    .updateDelegate -= earlyUpdate.OnEarlyUpdate;
            foreach (var preUpdate in _editorCallbackStorage.PreUpdates)
                playerLoopSystem.subSystemList[UpdateType.PreUpdate.ToIndex()]
                    .updateDelegate -= preUpdate.OnPreUpdate;
            foreach (var update in _editorCallbackStorage.Updates)
                playerLoopSystem.subSystemList[UpdateType.Update.ToIndex()]
                    .updateDelegate -= update.OnUpdate;
            foreach (var preLateUpdate in _editorCallbackStorage.PreLateUpdates)
                playerLoopSystem.subSystemList[UpdateType.PreLateUpdate.ToIndex()]
                    .updateDelegate -= preLateUpdate.OnPreLateUpdate;
            foreach (var postLateUpdate in _editorCallbackStorage.PostLateUpdates)
                playerLoopSystem.subSystemList[UpdateType.PostLateUpdate.ToIndex()]
                    .updateDelegate -= postLateUpdate.OnPostLateUpdate;

            _editorCallbackStorage.EarlyUpdates.Clear();
            _editorCallbackStorage.PreUpdates.Clear();
            _editorCallbackStorage.Updates.Clear();
            _editorCallbackStorage.PreLateUpdates.Clear();
            _editorCallbackStorage.PostLateUpdates.Clear();
        }
#endif

        private static void FrameCounter() => Frame++;

        public static void AddListeners(object clientObject)
        {
            var interfaces = clientObject.GetType()
                .GetInterfaces();

            foreach (var @interface in interfaces)
            {
                if (@interface == typeof(IEarlyUpdate)) AddListener((IEarlyUpdate) clientObject);
                if (@interface == typeof(IPostLateUpdate)) AddListener((IPostLateUpdate) clientObject);
                if (@interface == typeof(IPreLateUpdate)) AddListener((IPreLateUpdate) clientObject);
                if (@interface == typeof(IPreUpdate)) AddListener((IPreUpdate) clientObject);
                if (@interface == typeof(IUpdate)) AddListener((IUpdate) clientObject);
            }
        }

        public static void RemoveListeners(object clientObject)
        {
            var interfaces = clientObject.GetType()
                .GetInterfaces()
                .Where(t => t.IsAssignableFrom(typeof(ILoop)));

            foreach (var @interface in interfaces)
            {
                if (@interface == typeof(IEarlyUpdate)) RemoveListener((IEarlyUpdate) clientObject);
                if (@interface == typeof(IPostLateUpdate)) RemoveListener((IPostLateUpdate) clientObject);
                if (@interface == typeof(IPreLateUpdate)) RemoveListener((IPreLateUpdate) clientObject);
                if (@interface == typeof(IPreUpdate)) RemoveListener((IPreUpdate) clientObject);
                if (@interface == typeof(IUpdate)) RemoveListener((IUpdate) clientObject);
            }
        }

        private static void AddListener(IEarlyUpdate client)
        {
#if UNITY_EDITOR
            _editorCallbackStorage.EarlyUpdates.Add(client);
#endif
            var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            currentPlayerLoopSystem.subSystemList[UpdateType.EarlyUpdate.ToIndex()].updateDelegate +=
                client.OnEarlyUpdate;
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
        }

        private static void AddListener(IPreUpdate client)
        {
#if UNITY_EDITOR
            _editorCallbackStorage.PreUpdates.Add(client);
#endif
            var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            currentPlayerLoopSystem.subSystemList[UpdateType.PreUpdate.ToIndex()].updateDelegate += client.OnPreUpdate;
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
        }

        private static void AddListener(IUpdate client)
        {
#if UNITY_EDITOR
            _editorCallbackStorage.Updates.Add(client);
#endif
            var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            currentPlayerLoopSystem.subSystemList[UpdateType.Update.ToIndex()].updateDelegate += client.OnUpdate;
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
        }

        private static void AddListener(IPreLateUpdate client)
        {
#if UNITY_EDITOR
            _editorCallbackStorage.PreLateUpdates.Add(client);
#endif
            var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            currentPlayerLoopSystem.subSystemList[UpdateType.PreLateUpdate.ToIndex()].updateDelegate +=
                client.OnPreLateUpdate;
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
        }

        private static void AddListener(IPostLateUpdate client)
        {
#if UNITY_EDITOR
            _editorCallbackStorage.PostLateUpdates.Add(client);
#endif
            var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            currentPlayerLoopSystem.subSystemList[UpdateType.PostLateUpdate.ToIndex()].updateDelegate +=
                client.OnPostLateUpdate;
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
        }

        private static void RemoveListener(IEarlyUpdate client)
        {
            var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            currentPlayerLoopSystem.subSystemList[UpdateType.EarlyUpdate.ToIndex()].updateDelegate -=
                client.OnEarlyUpdate;
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
#if UNITY_EDITOR
            _editorCallbackStorage.EarlyUpdates.Remove(client);
#endif
        }

        private static void RemoveListener(IPreUpdate client)
        {
            var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            currentPlayerLoopSystem.subSystemList[UpdateType.PreUpdate.ToIndex()].updateDelegate -= client.OnPreUpdate;
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
#if UNITY_EDITOR
            _editorCallbackStorage.PreUpdates.Remove(client);
#endif
        }

        private static void RemoveListener(IUpdate client)
        {
            var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            currentPlayerLoopSystem.subSystemList[UpdateType.Update.ToIndex()].updateDelegate -= client.OnUpdate;
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
#if UNITY_EDITOR
            _editorCallbackStorage.Updates.Remove(client);
#endif
        }

        private static void RemoveListener(IPreLateUpdate client)
        {
            var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            currentPlayerLoopSystem.subSystemList[UpdateType.PreLateUpdate.ToIndex()].updateDelegate -=
                client.OnPreLateUpdate;
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
#if UNITY_EDITOR
            _editorCallbackStorage.PreLateUpdates.Remove(client);
#endif
        }

        private static void RemoveListener(IPostLateUpdate client)
        {
            var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            currentPlayerLoopSystem.subSystemList[UpdateType.PostLateUpdate.ToIndex()].updateDelegate -=
                client.OnPostLateUpdate;
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
#if UNITY_EDITOR
            _editorCallbackStorage.PostLateUpdates.Remove(client);
#endif
        }
    }
}