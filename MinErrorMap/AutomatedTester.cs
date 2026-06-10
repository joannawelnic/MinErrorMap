using System;
using System.Collections.Generic;
using System.Threading;

namespace MinErrorMap
{
    /// <summary>
    /// Konfiguracja jednego punktu testu (jeden zestaw parametrów).
    /// </summary>
    public class TestConfig
    {
        public string Label { get; set; }          // opis w tabeli wyników (np. "tenure=5")
        public int Rows { get; set; }
        public int Cols { get; set; }
        public double ErrorPercent { get; set; }   // 0.0–1.0 (np. 0.03 = 3%)
        public int TabuTenure { get; set; }
        public int MaxIterations { get; set; }
        public int Restarts { get; set; }
        public int PerturbationSize { get; set; }
        public double NeighborhoodPct { get; set; } // 0.0–1.0
    }

    /// <summary>
    /// Wynik uśredniony po wielu powtórzeniach dla jednego TestConfig.
    /// </summary>
    public class AggregatedResult
    {
        public string Label { get; set; }
        public int Rows { get; set; }
        public int Cols { get; set; }
        public int KnownErrors { get; set; }
        public double ErrorPct { get; set; }
        public int TabuTenure { get; set; }
        public int MaxIterations { get; set; }
        public int Restarts { get; set; }
        public int PerturbationSize { get; set; }
        public int NeighborhoodPct { get; set; }
        public int Repetitions { get; set; }
        // Statystyki wyników
        public double AvgScore { get; set; }
        public double StdDev { get; set; }
        public int BestScore { get; set; }
        public int WorstScore { get; set; }
        public double AvgDistFromOpt { get; set; }  // avg(score − knownErrors)
        public double AvgRelErrPct { get; set; }    // avg(score − known) / known × 100
        public double AvgIterations { get; set; }
        public double AvgTimeMs { get; set; }
    }

    public class AutomatedTester
    {
        private readonly MatrixGenerator _generator = new MatrixGenerator();
        private readonly TabuSearchAlgorithm _tabuSearch = new TabuSearchAlgorithm();

        public delegate void ProgressCallback(int done, int total, string message);

        /// <summary>
        /// Uruchamia serię testów: dla każdego TestConfig generuje 'repetitions' losowych
        /// instancji o tych samych parametrach i uśrednia wyniki.
        /// </summary>
        public List<AggregatedResult> RunTestSeries(
            List<TestConfig> configs,
            int repetitions,
            ProgressCallback onProgress,
            CancellationToken cancellationToken)
        {
            var results = new List<AggregatedResult>();
            int totalTests = configs.Count * repetitions;
            int completed  = 0;

            foreach (var cfg in configs)
            {
                if (cancellationToken.IsCancellationRequested) break;

                var scores     = new List<double>();
                var distances  = new List<double>();
                var iterations = new List<double>();
                var times      = new List<double>();
                int lastKnownErrors = 0;

                for (int rep = 0; rep < repetitions; rep++)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    // Generuj nową instancję o zadanych parametrach
                    int[,] matrix;
                    try
                    {
                        matrix = _generator.GenerateMatrix(cfg.Rows, cfg.Cols);
                        int errCount = Math.Max(1,
                            (int)Math.Round(cfg.Rows * cfg.Cols * cfg.ErrorPercent));
                        _generator.ApplyErrors(matrix, errCount);
                        lastKnownErrors = _generator.KnownErrors;
                        matrix = _generator.ShuffleColumns(matrix);
                    }
                    catch (Exception ex)
                    {
                        completed++;
                        onProgress?.Invoke(completed, totalTests,
                            $"Błąd generowania [{cfg.Label}]: {ex.Message}");
                        continue;
                    }

                    // Uruchom Tabu Search (bez callbacku UI – tryb wsadowy)
                    var sr = _tabuSearch.RunTabuSearch(
                        matrix,
                        cfg.MaxIterations,
                        cfg.TabuTenure,
                        cfg.Restarts,
                        cfg.PerturbationSize,
                        cfg.NeighborhoodPct,
                        onProgressUpdate: null,
                        cancellationToken: cancellationToken,
                        pauseEvent: new ManualResetEventSlim(true));

                    scores.Add(sr.BestScore);
                    distances.Add(sr.BestScore - lastKnownErrors);
                    iterations.Add(sr.TotalIterations);
                    times.Add(sr.ElapsedMs);

                    completed++;
                    onProgress?.Invoke(completed, totalTests,
                        $"[{cfg.Label}] rep {rep + 1}/{repetitions} | wynik: {sr.BestScore}");
                }

                if (scores.Count == 0) continue;

                double avgScore = Mean(scores);
                double avgDist  = Mean(distances);
                double relErr   = lastKnownErrors > 0
                    ? avgDist / lastKnownErrors * 100.0
                    : 0.0;
                double variance = 0;
                foreach (var s in scores) variance += (s - avgScore) * (s - avgScore);
                variance /= scores.Count;

                results.Add(new AggregatedResult
                {
                    Label           = cfg.Label,
                    Rows            = cfg.Rows,
                    Cols            = cfg.Cols,
                    KnownErrors     = lastKnownErrors,
                    ErrorPct        = cfg.ErrorPercent * 100.0,
                    TabuTenure      = cfg.TabuTenure,
                    MaxIterations   = cfg.MaxIterations,
                    Restarts        = cfg.Restarts,
                    PerturbationSize= cfg.PerturbationSize,
                    NeighborhoodPct = (int)Math.Round(cfg.NeighborhoodPct * 100),
                    Repetitions     = scores.Count,
                    AvgScore        = Math.Round(avgScore,    2),
                    StdDev          = Math.Round(Math.Sqrt(variance), 2),
                    BestScore       = (int)ListMin(scores),
                    WorstScore      = (int)ListMax(scores),
                    AvgDistFromOpt  = Math.Round(avgDist,     2),
                    AvgRelErrPct    = Math.Round(relErr,      1),
                    AvgIterations   = Math.Round(Mean(iterations)),
                    AvgTimeMs       = Math.Round(Mean(times))
                });
            }

            return results;
        }

        // ── Pomocnicze statystyki ──────────────────────────────────────────
        private static double Mean(List<double> list)
        {
            double s = 0;
            foreach (var v in list) s += v;
            return s / list.Count;
        }

        private static double ListMin(List<double> list)
        {
            double m = double.MaxValue;
            foreach (var v in list) if (v < m) m = v;
            return m;
        }

        private static double ListMax(List<double> list)
        {
            double m = double.MinValue;
            foreach (var v in list) if (v > m) m = v;
            return m;
        }
    }
}
