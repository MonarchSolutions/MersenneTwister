using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MersenneTwister
{
    public static class Randoms
    {
        private static readonly ThreadLocal<Random> wellBalanced = new(() => Create(RandomType.WellBalanced), false);
        private static readonly ThreadLocal<Random> fastestInt32 = new(() => Create(RandomType.FastestInt32), false);
        private static readonly ThreadLocal<Random> fastestDouble = new(() => Create(RandomType.FastestDouble), false);

        private static readonly Lazy<Random> shared = new(() => Create(RandomType.WellBalanced), LazyThreadSafetyMode.ExecutionAndPublication);

        public static Random? WellBalanced {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return wellBalanced.Value;
            }
        }

        public static Random? FastestInt32 {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return fastestInt32.Value;
            }
        }

        public static Random? FastestDouble {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return fastestDouble.Value;
            }
        }

        public static Random Create(RandomType type = RandomType.WellBalanced)
        {
            return type switch {
                RandomType.WellBalanced => DsfmtRandom.Create(),
                RandomType.FastestInt32 => MT64Random.Create(),
                RandomType.FastestDouble => DsfmtRandom.Create(),
                _ => throw new ArgumentException()
            };
        }

        public static Random Create(int seed, RandomType type = RandomType.WellBalanced)
        {
            return type switch {
                RandomType.WellBalanced => DsfmtRandom.Create(seed),
                RandomType.FastestInt32 => MT64Random.Create(seed),
                RandomType.FastestDouble => DsfmtRandom.Create(seed),
                _ => throw new ArgumentException()
            };
        }

        public static int Next()
        {
            var rng = shared.Value;
            lock (rng) {
                return rng.Next();
            }
        }

        public static int Next(int maxValue)
        {
            var rng = shared.Value;
            lock (rng) {
                return rng.Next(maxValue);
            }
        }

        public static int Next(int minValue, int maxValue)
        {
            var rng = shared.Value;
            lock (rng) {
                return rng.Next(minValue, maxValue);
            }
        }

        public static double NextDouble()
        {
            var rng = shared.Value;
            lock (rng) {
                return rng.NextDouble();
            }
        }

        public static void NextBytes(byte[] buffer)
        {
            var rng = shared.Value;
            lock (rng) {
                rng.NextBytes(buffer);
            }
        }
    }

    public enum RandomType
    {
        WellBalanced,
        FastestInt32,
        FastestDouble,
    }
}
