using System;
using System.Collections.Generic;
using System.Linq;

namespace Neat
{
    public class NeuralNetwork
    {
		public Dictionary<int, Neuron> neurons; // All neurons in genome, mapped by ID

		public List<int> input;    // IDs of input neurons
        public List<int> output;   // IDs of output neurons

		private List<Neuron> unprocessed; // neurons unprocessed

        public Genome genome;

		public NeuralNetwork(Genome genome)
		{
            this.genome = genome;
			this.input = new List<int>();
			this.output = new List<int>();
			this.neurons = new Dictionary<int, Neuron>();
			this.unprocessed = new List<Neuron>();

            foreach (NodeGene node in genome.Nodes.Values)
            {
                Neuron neuron = new Neuron();

                if (node.Type == NodeGene.TYPE.INPUT)
                {
                    neuron.addInputConnection();
                    input.Add(node.Id);
                }
                else if (node.Type == NodeGene.TYPE.OUTPUT)
                {
                    output.Add(node.Id);
                }
                neurons.Add(node.Id, neuron);
            }

            foreach (ConnectionGene conn in genome.Connections.Values)
            {
                if (!conn.Expressed)
                {
                    continue;
                }
                Neuron? inputter = neurons[conn.InNode]; 

                inputter.addOutputConnection(conn.OutNode, conn.Weight);
                Neuron outputReceiver = neurons[conn.OutNode]; 
                outputReceiver.addInputConnection();
            }
        }

        public void setFitness(float val)
        {
            genome.Fitness = val;
        }
		public List<float> calculate(List<float> input_parameter)
		{
			if (input_parameter.Count != input.Count())
			{
				 Console.WriteLine("Number of inputs must match number of input neurons in genome");
				return new List<float>();
			}
			unprocessed.Clear();

			foreach (Neuron neuron in neurons.Values)
			{
				neuron.reset();
				unprocessed.Add(neuron);
			}

            // ready the inputs
            for (int i = 0; i < input_parameter.Count; i++)
            {// loop through each input
                Neuron inputNeuron = neurons[input[i]];
                inputNeuron.feedInput(input_parameter[i]);// input neurons only have one input
                inputNeuron.calculate();
                for (int k = 0; k < inputNeuron.outputIDs.Count; k++)
                {// loop through receivers of this input
                    Neuron receiver = neurons[inputNeuron.outputIDs[k]];
                    receiver.feedInput(inputNeuron.output * inputNeuron.outputWeights[k]);     
                }
                unprocessed.Remove(inputNeuron);
            }

            int loops = 0;
            List<Neuron> neuronList = new List<Neuron>();
            while (unprocessed.Count > 0)
            {
                loops++;
                if (loops > 1000)
                {
                    Console.WriteLine("Can't solve network... Giving up to return null");
                    return new List<float>();
                }

                foreach (Neuron n in unprocessed)
                {
                    if (n.isReady())
                    {// if neuron has all inputs, calculate the neuron
                        neuronList.Add(n);
                        n.calculate();
                        for (int i = 0; i < n.outputIDs.Count; i++)
                        {
                            int receiverID = n.outputIDs[i];
                            float receiverValue = n.output * n.outputWeights[i];
                            neurons[receiverID].feedInput(receiverValue);
                        }
                    }
                }
                foreach (Neuron n in neuronList)
                {
                    unprocessed.Remove(n);
                }
            }

            //copy output from output neurons, and copy it into array
            List<float> outputs = new List<float>();
            foreach (int id in  output)
            {
                outputs.Add(neurons[id].output);
            }

            return outputs;
        }
	}
}
