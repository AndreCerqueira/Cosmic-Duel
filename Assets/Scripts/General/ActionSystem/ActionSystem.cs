using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Runtime.Scripts.General.ActionSystem
{
    public class ActionSystem : Singleton<ActionSystem>
    {
        private List<GameAction> _reactions = null;
        public bool IsPerforming { get; private set; } = false;
        private static Dictionary<Type, List<Action<GameAction>>> _preSubs = new();
        private static Dictionary<Type, List<Action<GameAction>>> _postSubs = new();
        private static Dictionary<Type, Func<GameAction, IEnumerator>> _performers = new();
        private static Dictionary<Delegate, Action<GameAction>> _reactionMap = new();

        public void Perform(GameAction action, Action onPerformFinished = null)
        {
            if (IsPerforming) return;
            IsPerforming = true;
            StartCoroutine(Flow(action, () =>
            {
                IsPerforming = false;
                onPerformFinished?.Invoke();
            }));
        }
        
        public void AddReaction(GameAction gameAction) 
        {
            _reactions?.Add(gameAction);
        }

        private IEnumerator Flow(GameAction action, Action onPerformFinished = null)
        {
            _reactions = action.PreReactions;
            PerformSubscribers(action, _preSubs);
            yield return PerformReactions();
            
            _reactions = action.PerformReactions;
            yield return PerformPerformer(action);
            yield return PerformReactions();
            
            _reactions = action.PostReactions;
            PerformSubscribers(action, _postSubs);
            yield return PerformReactions();
            
            onPerformFinished?.Invoke();
        }

        private IEnumerator PerformReactions()
        {
            foreach (var reaction in _reactions)
            {
                yield return Flow(reaction);
            }
        }

        private IEnumerator PerformPerformer(GameAction action)
        {
            var type = action.GetType();
            if (_performers.ContainsKey(type))
            {
                yield return _performers[type](action);
            }
        }

        private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs)
        {
            var type = action.GetType();
            if (subs.ContainsKey(type))
            {
                foreach (var sub in subs[type])
                {
                    sub(action);
                }
            }
        }

        public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
        {
            var type = typeof(T);
            IEnumerator wrappedPerformer(GameAction action) => performer((T)action);
            if (_performers.ContainsKey(type)) _performers[type] = wrappedPerformer;
            else _performers.Add(type, wrappedPerformer);
        }

        public static void DetachPerformer<T>() where T : GameAction
        {
            var type = typeof(T);
            if (_performers.ContainsKey(type)) _performers.Remove(type);
        }

        public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
        {
            var subs = timing == ReactionTiming.PRE ? _preSubs : _postSubs;
    
            Action<GameAction> wrappedReaction = (action) => reaction((T)action);
            _reactionMap[reaction] = wrappedReaction;

            if (subs.ContainsKey(typeof(T)))
            {
                subs[typeof(T)].Add(wrappedReaction);
            }
            else
            {
                subs.Add(typeof(T), new List<Action<GameAction>> { wrappedReaction });
            }
        }

        public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
        {
            var subs = timing == ReactionTiming.PRE ? _preSubs : _postSubs;

            if (_reactionMap.TryGetValue(reaction, out var wrappedReaction))
            {
                if (subs.ContainsKey(typeof(T)))
                {
                    subs[typeof(T)].Remove(wrappedReaction);
                    if (subs[typeof(T)].Count == 0)
                        subs.Remove(typeof(T));
                }

                _reactionMap.Remove(reaction);
            }
        }
    }
}
