using System;
using System.Security.Cryptography;
using JetBrains.Annotations;

namespace QuikGraph.Utils
{
    /// <summary>
    /// Secure random number generator.
    /// </summary>
    /// <remarks>
    /// Note that because of security issue the seed is unused in this random number generator.
    /// Note also that it is slower than classic <see cref="T:System.Random"/> but on purpose.
    /// </remarks>
    public class CryptoRandom : Random
    {
        [NotNull]
        private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        [NotNull]
        private readonly byte[] _uint32Buffer = new byte[4];

        /// <summary>
        /// Initializes a new <see cref="CryptoRandom"/> class.
        /// </summary>
        public CryptoRandom()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="CryptoRandom"/> class.
        /// </summary>
        /// <param name="ignoredSeed">Seed is ignored, just to keep same API as <see cref="T:System.Random"/>.</param>
        // ReSharper disable once UnusedParameter.Local
        public CryptoRandom(int ignoredSeed)
        {
        }

        /// <inheritdoc />
        public override int Next()
        {
            _rng.GetBytes(_uint32Buffer);
            return BitConverter.ToInt32(_uint32Buffer, 0) & 0x7FFFFFFF;
        }

        /// <inheritdoc />
        public override int Next(int maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue));
            return Next(0, maxValue);
        }

        /// <inheritdoc />
        public override int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue));
            if (minValue == maxValue)
                return minValue;
            long diff = maxValue - minValue;
            while (true)
            {
                _rng.GetBytes(_uint32Buffer);
                uint rand = BitConverter.ToUInt32(_uint32Buffer, 0);

                const long max = (1 + (long)uint.MaxValue);
                long remainder = max % diff;
                if (rand < max - remainder)
                {
                    return (int)(minValue + rand % diff);
                }
            }
        }

        /// <inheritdoc />
        public override double NextDouble()
        {
            _rng.GetBytes(_uint32Buffer);
            uint rand = BitConverter.ToUInt32(_uint32Buffer, 0);
            return rand / (1.0 + uint.MaxValue);
        }

        /// <inheritdoc />
        public override void NextBytes(byte[] buffer)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));
            _rng.GetBytes(buffer);
        }
    }
}