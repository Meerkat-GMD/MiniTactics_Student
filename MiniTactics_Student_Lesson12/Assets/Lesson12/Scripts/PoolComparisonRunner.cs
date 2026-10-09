using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Unity.Profiling;
using UnityEngine;

namespace MiniTactics.Lesson12
{
    public sealed class PoolComparisonRunner : MonoBehaviour
    {
        [Serializable]
        public sealed class RoundResult
        {
            public bool Pooled;
            public bool Warmup;
            public int NumberCreations;
            public int EffectCreations;
            public int Bursts;
            public int Updates;
            public float[] WallMilliseconds;
            public float[] CpuMilliseconds;
            public long[] ManagedBytes;
            public long AllocationBytes;
            public float MedianMilliseconds;
            public float P95Milliseconds;
            public float MaximumMilliseconds;
        }

        [Serializable]
        public sealed class ComparisonResult
        {
            public string Conditions;
            public string SourceRevision;
            public string GcUnit;
            public string CpuUnit;
            public bool GcAvailable;
            public bool CpuAvailable;
            public int PrewarmNumbers;
            public int PrewarmEffects;
            public float PrewarmWindowMilliseconds;
            public long PrewarmWindowManagedBytes;
            public int BurstCount;
            public int NumbersPerBurst;
            public int EffectsPerBurst;
            public int IntervalFrames;
            public float SimulationStep;
            public int EffectFrameCount;
            public List<RoundResult> Rounds = new List<RoundResult>();
        }

        [SerializeField] private int _burstCount = 30;
        [SerializeField] private int _numbersPerBurst = 16;
        [SerializeField] private int _effectsPerBurst = 4;
        [SerializeField] private int _intervalFrames = 12;
        [SerializeField] private float _simulationStep = 1f / 60f;
        [SerializeField] private int _effectFrameCount;
        [SerializeField] private int _measuredPairs = 5;
        [SerializeField] private string _sourceRevision = "classroom-local";
        [SerializeField] private ComparisonResult _results;
        [SerializeField] private string _outputPath;
        [SerializeField] private bool _completed;
        [SerializeField] private bool _running;
        private ProfilerRecorder _gc;
        private ProfilerRecorder _cpu;

        public Action<bool> RoundStarted;
        public Action<int> Burst;
        public Action<float> Advanced;
        public Action RoundEnded;
        public Func<Vector2Int> ReadCreatedCount;

        public int NumbersPerBurst => _numbersPerBurst;
        public int EffectsPerBurst => _effectsPerBurst;
        public int NumberCapacity => _numbersPerBurst * (Mathf.CeilToInt(0.7f / (_intervalFrames * _simulationStep)) + 1);
        public int EffectCapacity => _effectsPerBurst * (Mathf.CeilToInt(_effectFrameCount / 10f / (_intervalFrames * _simulationStep)) + 1);

        private IEnumerator Start()
        {
            yield return null;
            if (RoundStarted == null || Burst == null || Advanced == null || RoundEnded == null || ReadCreatedCount == null)
            {
                Debug.Log("풀 비교: PoolComparisonDriver 코드를 작성하고 Inspector 참조를 연결한 뒤 Play를 누르세요.");
                yield break;
            }
            _running = true;
            _gc = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");
            _cpu = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "CPU Main Thread Frame Time");
            _results = new ComparisonResult
            {
                Conditions = $"Unity {Application.unityVersion}; Editor={Application.isEditor}; Batch={Application.isBatchMode}; " +
                    $"{SystemInfo.operatingSystem}; CPU={SystemInfo.processorType}; GPU={SystemInfo.graphicsDeviceName}; " +
                    $"{Screen.width}x{Screen.height}; Quality={QualitySettings.GetQualityLevel()}; vSync={QualitySettings.vSyncCount}; targetFPS={Application.targetFrameRate}",
                SourceRevision = _sourceRevision,
                GcUnit = _gc.Valid ? _gc.UnitType.ToString() : "unavailable",
                CpuUnit = _cpu.Valid ? _cpu.UnitType.ToString() : "unavailable",
                BurstCount = _burstCount, NumbersPerBurst = _numbersPerBurst, EffectsPerBurst = _effectsPerBurst,
                IntervalFrames = _intervalFrames, SimulationStep = _simulationStep, EffectFrameCount = _effectFrameCount
            };
            float setupStart = Time.realtimeSinceStartup;
            RoundStarted(true);
            yield return null;
            Vector2Int prewarm = ReadCreatedCount();
            _results.PrewarmNumbers = prewarm.x;
            _results.PrewarmEffects = prewarm.y;
            _results.PrewarmWindowMilliseconds = (Time.realtimeSinceStartup - setupStart) * 1000f;
            _results.PrewarmWindowManagedBytes = ReadGc();
            RoundEnded();
            for (int pair = 0; pair < 2; pair++)
            {
                yield return RunRound(false, true);
                yield return RunRound(true, true);
            }
            for (int pair = 0; pair < _measuredPairs; pair++)
            {
                bool first = pair % 2 == 1;
                yield return RunRound(first, false);
                yield return RunRound(!first, false);
            }
            _results.GcAvailable = _gc.Valid && _gc.UnitType == ProfilerMarkerDataUnit.Bytes && _gc.Count > 0;
            _results.CpuAvailable = _cpu.Valid && _cpu.UnitType == ProfilerMarkerDataUnit.TimeNanoseconds && _cpu.Count > 0 && HasCpuSamples();
            Export();
            _completed = true;
            _running = false;
            DisposeRecorders();
        }

        private IEnumerator RunRound(bool pooled, bool warmup)
        {
            int drain = Mathf.CeilToInt(Mathf.Max(0.7f, _effectFrameCount / 10f) / _simulationStep) + 2;
            int frames = _burstCount * _intervalFrames + drain;
            var row = new RoundResult
            {
                Pooled = pooled, Warmup = warmup,
                WallMilliseconds = new float[frames], CpuMilliseconds = new float[frames], ManagedBytes = new long[frames]
            };
            RoundStarted(pooled);
            Vector2Int before = ReadCreatedCount();
            // Setup and array allocation finish before the sampled workload window.
            yield return null;
            for (int frame = 0; frame < frames; frame++)
            {
                if (frame % _intervalFrames == 0 && frame / _intervalFrames < _burstCount)
                {
                    Burst(frame / _intervalFrames);
                    row.Bursts++;
                }
                Advanced(_simulationStep);
                row.Updates++;
                yield return null;
                row.WallMilliseconds[frame] = Time.unscaledDeltaTime * 1000f;
                row.ManagedBytes[frame] = ReadGc();
                row.CpuMilliseconds[frame] = _cpu.Valid && _cpu.UnitType == ProfilerMarkerDataUnit.TimeNanoseconds && _cpu.Count > 0 ? _cpu.LastValue * 0.000001f : -1f;
            }
            Vector2Int creations = ReadCreatedCount() - before;
            row.NumberCreations = creations.x;
            row.EffectCreations = creations.y;
            RoundEnded();
            foreach (long bytes in row.ManagedBytes) if (bytes >= 0) row.AllocationBytes += bytes;
            float[] sorted = (float[])row.WallMilliseconds.Clone();
            Array.Sort(sorted);
            row.MedianMilliseconds = sorted[sorted.Length / 2];
            row.P95Milliseconds = sorted[Mathf.CeilToInt(sorted.Length * 0.95f) - 1];
            row.MaximumMilliseconds = sorted[sorted.Length - 1];
            _results.Rounds.Add(row);
            // Summary/cleanup and deferred destruction are outside the next round's samples.
            yield return null;
            yield return null;
        }

        private long ReadGc() => _gc.Valid && _gc.UnitType == ProfilerMarkerDataUnit.Bytes && _gc.Count > 0 ? _gc.LastValue : -1;

        private bool HasCpuSamples()
        {
            foreach (RoundResult round in _results.Rounds)
                foreach (float time in round.CpuMilliseconds)
                    if (time > 0f) return true;
            return false;
        }

        private void Export()
        {
            string folder = Path.Combine(Application.persistentDataPath, "PoolComparison");
            Directory.CreateDirectory(folder);
            _outputPath = Path.Combine(folder, "lesson12-pool-results.json");
            File.WriteAllText(_outputPath, JsonUtility.ToJson(_results, true), new UTF8Encoding(false));
            var text = new StringBuilder("# Object Pool 실제 비교 결과\n\n");
            text.AppendLine(_results.Conditions);
            text.AppendLine($"\n소스: {_sourceRevision}. 예열: 숫자 {_results.PrewarmNumbers}, 효과 {_results.PrewarmEffects}. " +
                $"예열 창 경과시간 {Format(_results.PrewarmWindowMilliseconds)} ms (메서드 CPU 시간이 아님), 관리 할당 {_results.PrewarmWindowManagedBytes} bytes.");
            text.AppendLine($"\n부하: {_burstCount}회 × 숫자 {_numbersPerBurst}/효과 {_effectsPerBurst}; {_intervalFrames}프레임 간격; 고정 tick {_simulationStep}; 효과 {_effectFrameCount}장/10fps.");
            text.AppendLine($"\nGC counter: {_results.GcAvailable} ({_results.GcUnit}); CPU frame counter: {_results.CpuAvailable} ({_results.CpuUnit}). -1은 측정 불가. CPU 배열은 nanoseconds를 ms로 변환한 별도 프레임 지표입니다.");
            text.AppendLine("\n|구간|방식|숫자 생성|효과 생성|관리 할당 bytes|벽시계 프레임 중앙값 ms|p95 ms|최대 ms|\n|---|---|---:|---:|---:|---:|---:|---:|");
            foreach (RoundResult row in _results.Rounds)
                text.AppendLine($"|{(row.Warmup ? "워밍업 제외" : "측정")}|{(row.Pooled ? "Pool" : "Instantiate/Destroy")}|{row.NumberCreations}|{row.EffectCreations}|{(_results.GcAvailable ? row.AllocationBytes.ToString() : "unavailable")}|{Format(row.MedianMilliseconds)}|{Format(row.P95Milliseconds)}|{Format(row.MaximumMilliseconds)}|");
            text.AppendLine("\n워밍업은 방식별 동일 부하 2회로 별도 기록했습니다. 관리 할당은 이 씬의 프레임 counter이며 게임 전체 GC나 GC 수거 횟수가 아닙니다. 프레임 잡음·렌더링·문자열 생성·Editor 비용이 남으므로 속도 향상을 보장하지 않습니다. 풀은 예열 시간·상주 메모리·초기화/소유자 관리 비용을 사용합니다.");
            File.WriteAllText(Path.ChangeExtension(_outputPath, ".md"), text.ToString(), new UTF8Encoding(false));
            Debug.Log("풀 비교 완료: Inspector Results에서 실제 값을 확인하세요. JSON/Markdown: " + _outputPath);
            Debug.Log(text.ToString());
        }

        private static string Format(float value) => value.ToString("F3", CultureInfo.InvariantCulture);
        private void DisposeRecorders()
        {
            if (_gc.Valid) _gc.Dispose();
            if (_cpu.Valid) _cpu.Dispose();
        }
        private void OnDisable()
        {
            StopAllCoroutines();
            DisposeRecorders();
            _running = false;
        }
    }
}
