using System.Collections.Generic;
using UnityEngine;
using System;

namespace NSG.Utils
{
    /// <summary>
    /// Utility class providing timer functionality and common numeric clamping operations.
    /// Manages timers without requiring MonoBehaviour attachment to game objects.
    /// </summary>
    public class NSGMain
    {
        #region Timers

        [Header("Timer Setup")]
        private static List<Timer> activeTimers = new List<Timer>();
        private static bool isInitialized = false;

        /// <summary>
        /// Represents an individual timer.
        /// </summary>
        private class Timer
        {
            public float Duration;  // The total duration of the timer
            public float ElapsedTime;  // The time elapsed since the timer started
            public Action OnComplete;  // Action to execute when the timer completes
            public bool Repeat;  // Indicates if the timer should restart after completing

            /// <summary>
            /// Constructor to initialize a Timer instance.
            /// </summary>
            /// <param name="duration">Duration of the timer in seconds.</param>
            /// <param name="onComplete">Action to execute upon timer completion.</param>
            /// <param name="repeat">Indicates if the timer should repeat.</param>
            public Timer(float duration, Action onComplete, bool repeat)
            {
                Duration = duration;
                ElapsedTime = 0f;
                OnComplete = onComplete;
                Repeat = repeat;
            }
        }

        /// <summary>
        /// Initializes the NSGMain timer system by setting up an update loop.
        /// </summary>
        private static void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;

            // Attach to Unity's update loop and manage timer updates.
            Application.quitting += () => activeTimers.Clear(); // Clear timers on application quit
            var managerObject = new GameObject("NSGUpdateManager");
            GameObject.DontDestroyOnLoad(managerObject);
            managerObject.AddComponent<NSGUpdateManager>().Initialize(UpdateTimers);
        }

        /// <summary>
        /// Updates all active timers. Called once per frame.
        /// </summary>
        private static void UpdateTimers()
        {
            for (int i = activeTimers.Count - 1; i >= 0; i--)
            {
                var timer = activeTimers[i];
                timer.ElapsedTime += Time.deltaTime;

                if (timer.ElapsedTime >= timer.Duration)
                {
                    timer.OnComplete?.Invoke();

                    if (timer.Repeat)
                    {
                        timer.ElapsedTime = 0f; // Reset elapsed time for repeating timers
                    }
                    else
                    {
                        activeTimers.RemoveAt(i); // Remove non-repeating timer
                    }
                }
            }
        }

        #endregion

        #region Timer Methods

        /// <summary>
        /// Starts a one-shot timer that executes an action after a specified duration.
        /// </summary>
        /// <param name="durationInSeconds">Duration of the timer in seconds.</param>
        /// <param name="onComplete">Action to execute upon timer completion.</param>
        public static void StartOneShotTimer(float durationInSeconds, Action onComplete)
        {
            Initialize();
            activeTimers.Add(new Timer(durationInSeconds, onComplete, false));
        }

        /// <summary>
        /// Starts a one-shot timer using an integer duration.
        /// </summary>
        /// <param name="durationInSeconds">Duration of the timer in seconds.</param>
        /// <param name="onComplete">Action to execute upon timer completion.</param>
        public static void StartOneShotTimer(int durationInSeconds, Action onComplete)
        {
            Initialize();
            activeTimers.Add(new Timer(durationInSeconds, onComplete, false));
        }

        /// <summary>
        /// Starts a repeating timer that executes an action at regular intervals.
        /// </summary>
        /// <param name="durationInSeconds">Interval duration in seconds.</param>
        /// <param name="onComplete">Action to execute at each interval.</param>
        public static void StartRepeatingTimer(float durationInSeconds, Action onComplete)
        {
            Initialize();
            activeTimers.Add(new Timer(durationInSeconds, onComplete, true));
        }

        /// <summary>
        /// Starts a repeating timer using an integer interval.
        /// </summary>
        /// <param name="durationInSeconds">Interval duration in seconds.</param>
        /// <param name="onComplete">Action to execute at each interval.</param>
        public static void StartRepeatingTimer(int durationInSeconds, Action onComplete)
        {
            Initialize();
            activeTimers.Add(new Timer(durationInSeconds, onComplete, true));
        }

        #endregion

        #region Clamp Methods

        /// <summary>
        /// Deducts a value from a given amount, clamping the result within a range.
        /// </summary>
        /// <param name="deductFrom">The value to deduct from.</param>
        /// <param name="deductValue">The value to deduct.</param>
        /// <param name="max">Maximum allowable value.</param>
        /// <param name="min">Minimum allowable value (default: 0).</param>
        /// <returns>The clamped result after deduction.</returns>
        public static float ClampDeductable(float deductFrom, float deductValue, float max, float min = 0)
        {
            deductFrom -= deductValue;
            return Mathf.Clamp(deductFrom, min, max);
        }

        /// <summary>
        /// Deducts a value from a given amount (integer), clamping the result within a range.
        /// </summary>
        /// <param name="deductFrom">The value to deduct from.</param>
        /// <param name="deductValue">The value to deduct.</param>
        /// <param name="max">Maximum allowable value.</param>
        /// <param name="min">Minimum allowable value (default: 0).</param>
        /// <returns>The clamped result after deduction.</returns>
        public static int ClampDeductable(int deductFrom, int deductValue, int max, int min = 0)
        {
            deductFrom -= deductValue;
            return Mathf.Clamp(deductFrom, min, max);
        }

        /// <summary>
        /// Adds a value to a given amount, clamping the result within a range.
        /// </summary>
        /// <param name="addToValue">The value to add to.</param>
        /// <param name="addAmount">The value to add.</param>
        /// <param name="max">Maximum allowable value.</param>
        /// <param name="min">Minimum allowable value (default: 0).</param>
        /// <returns>The clamped result after addition.</returns>
        public static float ClampAdditive(float addToValue, float addAmount, float max, float min = 0)
        {
            addToValue += addAmount;
            return Mathf.Clamp(addToValue, min, max);
        }

        /// <summary>
        /// Adds a value to a given amount (integer), clamping the result within a range.
        /// </summary>
        /// <param name="addToValue">The value to add to.</param>
        /// <param name="addAmount">The value to add.</param>
        /// <param name="max">Maximum allowable value.</param>
        /// <param name="min">Minimum allowable value (default: 0).</param>
        /// <returns>The clamped result after addition.</returns>
        public static int ClampAdditive(int addToValue, int addAmount, int max, int min = 0)
        {
            addToValue += addAmount;
            return Mathf.Clamp(addToValue, min, max);
        }

        #endregion

        /// <summary>
        /// MonoBehaviour helper for handling Unity's update loop.
        /// </summary>
        private class NSGUpdateManager : MonoBehaviour
        {
            private Action updateAction;

            /// <summary>
            /// Initializes the update manager with an action to invoke each frame.
            /// </summary>
            /// <param name="updateAction">The action to execute in Update().</param>
            public void Initialize(Action updateAction)
            {
                this.updateAction = updateAction;
            }

            private void Update()
            {
                updateAction?.Invoke();
            }
        }
    }
}