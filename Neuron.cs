using System;
using System.Collections.Generic;
using System.Linq;

namespace Neat
{
    public class Neuron
    {
        public float output;
        public List<float?> inputs;

        public List<int> outputIDs;
        public List<float> outputWeights;

        public Neuron()
        {
            inputs = new List<float?>();
            outputIDs = new List<int>();
            outputWeights = new List<float>();
        }


        public void addOutputConnection(int outputID, float weight)
        {
            if (!outputIDs.Contains(outputID))
            {
                outputIDs.Add(outputID);
                outputWeights.Add(weight);
            }
            
        }

        /** 
		 * Adds a connection to this neuron from another neuron
		 */
        public void addInputConnection()
        {
            inputs.Add(null);
        }

        /**
		 * Takes all the inputs, and calculates them into an output
		 * This can only happen if the neuron {@link #isReady() isReady}
		 */
        public float calculate()
        {
            float sum = 0f;
            foreach (float f in inputs)
            {
                sum += f;
            }
            output = sigmoidActivationFunction(sum);
            return output;
        }

        /**
		 * If a neuron is ready, it has all the needed inputs to do calculation
		 * @return true if ready
		 */
        public Boolean isReady()
        {
            Boolean foundNull = false;
            foreach (float? f in inputs)
            {
                if (f == null)
                {
                    foundNull = true;
                    break;
                }
            }
            return !foundNull;
        }

        /**
		 * Adds an input to the neuron in the first slot available
		 */
        public void feedInput(float input)
        {
            //System.out.println("Feeding input\tInput slots total: "+inputs.length);
            Boolean foundSlot = false;
            for (int i = 0; i < inputs.Count; i++)
            {
                if (inputs[i] == null)
                {
                    inputs[i] = input;
                    foundSlot = true;
                    break;
                }
            }
            if (!foundSlot)
            {
                Console.WriteLine("No input slot ready for input. Input array: " + inputs.ToString());
            }
        }

        /**
		 * Resets the inputs on this neuron, as well as the calculation
		 */
        public void reset()
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                inputs[i] = null;
            }
            output = 0f;
        }

        /* Takes any float, and returns a value between 0 and 1. 0f returns 0.5f */
        private float sigmoidActivationFunction(float value)
        {
            float result = (float)(1f / (1f + Math.Exp(-4.9d * value)));
            return result;
        }
    }
}
