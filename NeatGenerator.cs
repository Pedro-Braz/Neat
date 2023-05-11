using System;
using System.Collections.Generic;
using System.Text;

namespace Neat
{
    public class NeatGenerator
    {
        NEATConfiguration config;
        public List<NeuralNetwork> networks;
        public Evaluator evl;
        private int inputs;
        private int outputs;
        Random random = new Random();

        public NeatGenerator(NEATConfiguration config, int nInputs, int nOutputs)
        {
            this.networks = new List<NeuralNetwork>(config.getPopulationSize());
            this.inputs = nInputs;
            this.outputs = nOutputs;
            this.config = config;
        }

        public void generateNetworks()
        {
            evl = new Evaluator(config, genomeGenerator);
            foreach (Genome genome in evl.genomes)
            {
                networks.Add(new NeuralNetwork(genome));
            }
        }

        public void evolve()
        {
            evl.evaluateGeneration();
            networks.Clear();
            foreach (Genome genome in evl.genomes)
            {
                networks.Add(new NeuralNetwork(genome));
            }
        }

        public Genome genomeGenerator()
        {
            Genome genome = new Genome();

            int inputNum;
            int outputNum;

            for (inputNum = 0; inputNum < inputs; inputNum++)
            {
                genome.addNodeGene(new NodeGene(NodeGene.TYPE.INPUT, inputNum));
            }

            for (outputNum = inputs; outputNum < outputs + inputs; outputNum++)
            {
                genome.addNodeGene(new NodeGene(NodeGene.TYPE.OUTPUT, outputNum));
            }

            for (inputNum = 0; inputNum < inputs; inputNum++)
            {
                for (outputNum = inputs; outputNum < outputs + inputs; outputNum++)
                {
                    genome.addConnectionGene(new ConnectionGene(inputNum, outputNum, (float)random.NextDouble() * 4f - 2f, true, 1));
                }
            }

            return genome;
        }
    }
}
