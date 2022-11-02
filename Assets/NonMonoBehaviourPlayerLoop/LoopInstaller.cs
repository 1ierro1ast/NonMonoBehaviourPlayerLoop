using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NonMonoBehaviourPlayerLoop
{
    public static class LoopInstaller
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            RegisterAll();
            //Frame = 0;
            var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
#if UNITY_EDITOR
            CheckIntegrity(currentPlayerLoopSystem);
            // Make sure we unregister before we do register.
            EditorApplication.playModeStateChanged -= OnPlayModeState;
            EditorApplication.playModeStateChanged += OnPlayModeState;
#endif
            //currentPlayerLoopSystem.subSystemList[UpdateType.Update.ToIndex()].updateDelegate += FrameCounter;
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
        }

        private static void RegisterAll()
        {
            Type interfaceType = typeof(IUpdate);
            IEnumerable<Type> types = interfaceType.Assembly.GetTypes()
                .Where(type => type.IsAssignableFrom(interfaceType));

            foreach (var type in types)
            {
                
            }
        }

        private static void CheckIntegrity(UnityEngine.LowLevel.PlayerLoopSystem playerLoopSystem)
        {
            for (var updateType = UpdateType.EarlyUpdate; updateType <= UpdateType.PostLateUpdate; updateType++)
                Debug.Assert(playerLoopSystem.subSystemList[updateType.ToIndex()].type == updateType.ToType(),
                    $"Fatal Error: Unity player-loop incompatible ({updateType})!");
        }

        private static void OnPlayModeState(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                    var currentPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
                    UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
                    EditorApplication.playModeStateChanged -= OnPlayModeState;
                    break;

                case PlayModeStateChange.EnteredPlayMode: break;
                case PlayModeStateChange.ExitingEditMode: break;
                case PlayModeStateChange.ExitingPlayMode: break;
                default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}