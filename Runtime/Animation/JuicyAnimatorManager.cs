using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

namespace LycheeLabs.FruityInterface.Animation {

    /// <summary>
    /// Ticks only the JuicyAnimators that are currently animating, once per frame, after the LateUpdate phase.
    /// Idle animators are not visited at all.
    /// </summary>
    public static class JuicyAnimatorManager {

        private static readonly HashSet<JuicyAnimator> dirty = new HashSet<JuicyAnimator>();
        private static readonly List<JuicyAnimator> buffer = new List<JuicyAnimator>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize() {
            dirty.Clear();
            buffer.Clear();

            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            for (int i = 0; i < playerLoop.subSystemList.Length; i++) {
                if (playerLoop.subSystemList[i].type != typeof(UnityEngine.PlayerLoop.PreLateUpdate)) continue;

                var preLateUpdate = playerLoop.subSystemList[i];
                var systems = new List<PlayerLoopSystem>(preLateUpdate.subSystemList);
                int index = systems.FindIndex(s => s.type == typeof(UnityEngine.PlayerLoop.PreLateUpdate.ScriptRunBehaviourLateUpdate));
                var manager = new PlayerLoopSystem { type = typeof(JuicyAnimatorManager), updateDelegate = Run };
                systems.Insert(index < 0 ? systems.Count : index + 1, manager);

                preLateUpdate.subSystemList = systems.ToArray();
                playerLoop.subSystemList[i] = preLateUpdate;
                PlayerLoop.SetPlayerLoop(playerLoop);
                return;
            }
        }

        internal static void MarkDirty(JuicyAnimator animator) {
            dirty.Add(animator);
        }

        internal static void Remove(JuicyAnimator animator) {
            dirty.Remove(animator);
        }

        private static void Run() {
            if (dirty.Count == 0) return;

            buffer.AddRange(dirty);
            dirty.Clear();

            for (int i = 0; i < buffer.Count; i++) {
                var animator = buffer[i];
                if (animator == null || !animator.isActiveAndEnabled) continue;
                if (animator.Step()) {
                    dirty.Add(animator);
                }
            }

            buffer.Clear();
        }

    }

}
