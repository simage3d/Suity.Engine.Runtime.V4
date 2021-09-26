// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

namespace Suity.Helpers
{
    public class SeededRandom
    {
        int _seed;
        readonly RandomWELL _rnd;


        public SeededRandom()
        {
            _seed = DateTime.UtcNow.Ticks.GetHashCode();
            _rnd = new RandomWELL(_seed);
        }
        public SeededRandom(int seed)
        {
            _seed = seed;
            _rnd = new RandomWELL(_seed);
        }

        public int Seed
        {
            get { return _seed; }
            set
            {
                _seed = value;
                _rnd.srand(value);
            }
        }
        public float NextFloat()
        {
            return _rnd.frand2();
        }
        public int NextInt()
        {
            return _rnd.rand();
        }
        public float Range(float min, float max)
        {
            return _rnd.Range(min, max);
        }
        public int Range(int min, int max)
        {
            int value = _rnd.Range(min, max);
            //UnityEngine.Debug.Log(string.Format("PRandom : {0}-{1} = {2}", min, max, value));
            return value;
        }
    }
}
