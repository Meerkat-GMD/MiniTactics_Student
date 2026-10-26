using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiniTactics.Lesson12
{
    public sealed class PresentationQueue : MonoBehaviour
    {
        [SerializeField] private UnitSpawner _spawner;
        [SerializeField] private DamageNumber _numberPrefab;
        [SerializeField] private Transform _effectsRoot;
        private readonly Queue<PresentationEvent> _events = new Queue<PresentationEvent>();
        private CancellationTokenSource _cancellation;
        public bool IsBusy { get; private set; }

        private void ResetUnits()
        {
            // OnDisable can run after other scene objects have been destroyed.
            if (_spawner == null) return;
            foreach (Unit unit in _spawner.Units)
                if (unit != null) unit.ResetPresentation();
        }

        public void Cancel()
        {
            _cancellation?.Cancel();
            _cancellation?.Dispose();
            _cancellation = null;
            _events.Clear();
            ResetUnits();
            if (_effectsRoot != null)
                foreach (Transform effect in _effectsRoot) Destroy(effect.gameObject);
            IsBusy = false;
        }

        private void OnDisable() => Cancel();

        private async UniTask PlayHealthAsync(PresentationEvent item, UnitAnimator animator, CancellationToken token)
        {
            item.Actor.GetComponentInChildren<UnitHealthBar>(true).Show(item.AfterHp, item.MaxHp);
            int change = item.AfterHp - item.BeforeHp;
            if (change < 0) animator.PlayHit(item.To);
            Services.Audio.Play(change < 0 ? SoundCue.Damage : SoundCue.Healing);
            DamageNumber number = Instantiate(_numberPrefab, _effectsRoot);
            number.Show(Mathf.Abs(change), change > 0, item.To);
            await UniTask.Delay(TimeSpan.FromSeconds(item.Seconds), cancellationToken: token);
            if (item.AfterHp > 0) return;
            FrameEffect.SpawnOwned(item.DeathFrames, item.To, item.SortingOrder, _effectsRoot);
            item.Actor.gameObject.SetActive(false);
            await UniTask.Delay(TimeSpan.FromSeconds(item.DeathSeconds), cancellationToken: token);
        }

        private async UniTask PlayAsync(PresentationEvent item, CancellationToken token)
        {
            UnitAnimator animator = item.Actor.GetComponent<UnitAnimator>();
            animator.SetLook(item.Look);
            switch (item.Kind)
            {
                case PresentationEvent.EventKind.Move: animator.PlayMove(item.From, item.To); break;
                case PresentationEvent.EventKind.Attack:
                    Services.Audio.Play(SoundCue.Attack);
                    animator.PlayAttack(item.From, item.TargetPosition);
                    break;
                case PresentationEvent.EventKind.Skill:
                    Services.Audio.Play(SoundCue.Attack);
                    animator.PlaySkill(item.From, item.TargetPosition);
                    break;
                default: await PlayHealthAsync(item, animator, token); return;
            }
            await UniTask.Delay(TimeSpan.FromSeconds(item.Seconds), cancellationToken: token);
        }

        private async UniTask ConsumeAsync(CancellationToken token)
        {
            try
            {
                await UniTask.Yield(token);
                while (_events.Count > 0) await PlayAsync(_events.Dequeue(), token);
                ResetUnits();
                _cancellation.Dispose();
                _cancellation = null;
                IsBusy = false;
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                // Cancel owns cleanup. An old consumer must not finish a new one.
            }
        }

        public bool TryEnqueue(PresentationEvent item)
        {
            if (!isActiveAndEnabled) return false;
            _events.Enqueue(item);
            if (!IsBusy)
            {
                IsBusy = true;
                _cancellation = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
                ConsumeAsync(_cancellation.Token).Forget();
            }
            return true;
        }
    }
}
