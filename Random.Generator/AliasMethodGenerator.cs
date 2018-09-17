using System;
using System.Linq;
using System.Collections.Generic;

namespace Random.Generator
{
    public class AliasMethodGenerator 
    {
        private IRandom _random;

        private double[] _probabilities;
        private int[] _aliases;
        private double _total;
        
        public AliasMethodGenerator(IList<double> input, IRandom random)
        {
            if (input == null || !input.Any())
            {
                throw new ArgumentException("Empty input collection !");
            }

            if(input.Any(i => i < 0))
            {
                throw new ArgumentException("Input collection should contain only positive values");
            }

            _random = random ?? throw new ArgumentException("Empty rendom generator!");

            _probabilities = new double[input.Count];
            _aliases = new int[input.Count];

            Init(input);
        }

        public int GetValue()
        {
            int column = _random.Generate(_probabilities.Length);
            var  coinToss = _random.Generate((int)_total) < _probabilities[column];
            return coinToss ? column : _aliases[column];
        }

        private void Init(IList<double> input)
        {
            _total = input.Sum();

            var scaledInput = input.Scale();
            var largeAndSmallStacks = CreateStacks(scaledInput);
            var large = largeAndSmallStacks.large;
            var small = largeAndSmallStacks.small;

            int smallItem;
            int largeItem;

            while (small.TryPop(out smallItem) && large.TryPop(out largeItem))
            {
                _probabilities[smallItem] = scaledInput[smallItem];
                _aliases[smallItem] = largeItem;

                scaledInput[largeItem] = scaledInput[largeItem] + scaledInput[smallItem] - _total;

                if (scaledInput[largeItem] < _total)
                    small.Push(largeItem);
                else
                    large.Push(largeItem);
            }

            RewriteToProbabilites(large);
            RewriteToProbabilites(small);          
        }

        private void RewriteToProbabilites(IEnumerable<int> rest)
        {
            foreach (var r in rest)
                _probabilities[r] = _total;
        }

        private (Stack<int> small, Stack<int> large) CreateStacks(IList<double> input)
        {
            var small = new Stack<int>();
            var large = new Stack<int>();


            for (var i = 0; i < input.Count; i++)
            {
                if (input[i] < _total)
                    small.Push(i);
                else
                    large.Push(i);
            }

            return (small, large);
        }

    }
}
