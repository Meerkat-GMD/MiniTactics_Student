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

        private async UniTask ConsumeAsync(CancellationToken token)
        {
            try
            {
                await UniTask.Yield(token);
                while (_events.Count > 0)
                    await PlayAsync(_events.Dequeue(), token);
                ResetUnits();
                IsBusy = false;
                _cancellation.Dispose();
                _cancellation = null;
            }
            catch (OperationCanceledException)
            {
                // Cancel owns cleanup; an old consumer must not change a new one's state.
            }
        }

        private async UniTask PlayAsync(PresentationEvent item, CancellationToken token)
        {
            UnitAnimator animator = item.Actor.GetComponent<UnitAnimator>();
            float seconds;
            switch (item.Kind)
            {
                case PresentationEvent.EventKind.Move:
                    seconds = animator.PlayMove(item.From, item.To);
                    break;
                case PresentationEvent.EventKind.Attack:
                    Services.Audio.Play(SoundCue.Attack);
                    seconds = animator.PlayAttack(item.From, item.TargetPosition);
                    break;
                case PresentationEvent.EventKind.Skill:
                    Services.Audio.Play(SoundCue.Attack);
                    seconds = animator.PlaySkill(item.From, item.TargetPosition);
                    break;
                default:
                    await PlayHealthAsync(item, animator, token);
                    return;
            }
            await UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: token);
        }

        private async UniTask PlayHealthAsync(PresentationEvent item, UnitAnimator animator, CancellationToken token)
        {
            item.Actor.GetComponentInChildren<UnitHealthBar>(true).Show(item.AfterHp, item.MaxHp);
            int change = item.AfterHp - item.BeforeHp;
            Services.Audio.Play(change > 0 ? SoundCue.Healing : SoundCue.Damage);
            if (change < 0) animator.PlayHit(item.To);
            DamageNumber number = Instantiate(_numberPrefab, _effectsRoot);
            number.Show(Mathf.Abs(change), change > 0, item.To);
            await UniTask.Delay(350, cancellationToken: token);
            if (item.AfterHp > 0) return;
            FrameEffect.SpawnOwned(animator.DeathFrames, item.To, animator.SortingOrder, _effectsRoot);
            item.Actor.gameObject.SetActive(false);
            await UniTask.Delay(TimeSpan.FromSeconds(animator.DeathFrames.Length / 10f), cancellationToken: token);
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

        private void ResetUnits()
        {
            // Unity may already have destroyed part of this scene during OnDisable.
            if (_spawner == null) return;
            foreach (Unit unit in _spawner.Units)
                if (unit != null) unit.ResetPresentation();
        }

        private void OnDisable() => Cancel();
    }
}
