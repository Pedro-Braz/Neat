using System;
using System.Collections.Generic;
using System.Text;

namespace Neat
{
    public class NeatGenerator2
    {
        NEATConfiguration config;
        public List<NeuralNetwork> networks;
        public Evaluator evl;
        private int inputs;
        private int outputs;
        private int test;
        Random random = new Random();

        public NeatGenerator2(NEATConfiguration config, int nInputs, int nOutputs)
        {
            this.networks = new List<NeuralNetwork>(config.getPopulationSize());
            this.inputs = nInputs;
            this.outputs = nOutputs;
            this.config = config;
            this.test = 0;
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

        public Genome genomeGeneratorBestGenomes()
        {
            Genome genome = new Genome();
            genome.addNodeGene(new NodeGene(NodeGene.TYPE.INPUT, 0));

            genome.addNodeGene(new NodeGene(NodeGene.TYPE.INPUT, 1));



            genome.addNodeGene(new NodeGene(NodeGene.TYPE.OUTPUT, 2));
            genome.addNodeGene(new NodeGene(NodeGene.TYPE.OUTPUT, 3));
            genome.addNodeGene(new NodeGene(NodeGene.TYPE.HIDDEN, 4));

            if (test == 0)
            {
                /* node3-con5 */
                genome.addConnectionGene(new ConnectionGene(0, 2, -0.26344338f, true, 1));
                genome.addConnectionGene(new ConnectionGene(1, 2, 0.26887998f, true, 2));
                genome.addConnectionGene(new ConnectionGene(0, 3, 0.0013257209f, true, 3));
                genome.addConnectionGene(new ConnectionGene(1, 3, -0.0023996248f, true, 4));

            }
            else
            {
                genome.addNodeGene(new NodeGene(NodeGene.TYPE.HIDDEN, 5));

                if (test == 1)
                {
                    /* node50-con5 */
                    genome.addConnectionGene(new ConnectionGene(0, 2, -0.3840507f, true, 1));
                    genome.addConnectionGene(new ConnectionGene(1, 2, 1.1055824f, true, 2));
                    genome.addConnectionGene(new ConnectionGene(0, 3, 0.00012524724f, true, 3));
                    genome.addConnectionGene(new ConnectionGene(1, 3, 0.15820676f, true, 4));
                    genome.addConnectionGene(new ConnectionGene(0, 4, -0.0033450294f, true, 5));
                    genome.addConnectionGene(new ConnectionGene(4, 3, 0.00025929091f, true, 6));
                    genome.addConnectionGene(new ConnectionGene(0, 5, 1f, true, 7));
                    genome.addConnectionGene(new ConnectionGene(5, 2, -0.3840507f, true, 8));
                }
                else if (test == 2)
                {
                    /* node50-con50 */
                    genome.addConnectionGene(new ConnectionGene(0, 2, -0.46518087f, true, 1));
                    genome.addConnectionGene(new ConnectionGene(1, 2, 1.3174572f, true, 2));
                    genome.addConnectionGene(new ConnectionGene(0, 3, -0.00025128108f, true, 3));
                    genome.addConnectionGene(new ConnectionGene(1, 3, 0.120640464f, true, 4));
                    genome.addConnectionGene(new ConnectionGene(0, 4, 1f, true, 5));
                    genome.addConnectionGene(new ConnectionGene(4, 2, -0.46518087f, true, 6));
                    genome.addConnectionGene(new ConnectionGene(0, 5, 1f, true, 7));
                    genome.addConnectionGene(new ConnectionGene(5, 4, 1f, true, 8));
                }
                else
                {
                    /* node50-con50-dt1 */

                    genome.addConnectionGene(new ConnectionGene(0, 2, -0.061588813f, true, 1));
                    genome.addConnectionGene(new ConnectionGene(1, 2, 0.1745969f, true, 2));
                    genome.addConnectionGene(new ConnectionGene(0, 3, 4.3529904E-05f, true, 3));
                    genome.addConnectionGene(new ConnectionGene(1, 3, 0.010006959f, true, 4));
                    genome.addConnectionGene(new ConnectionGene(0, 4, 1f, true, 5));
                    genome.addConnectionGene(new ConnectionGene(4, 2, -0.061588813f, true, 6));
                }
            }
            test++;
            return genome;

        }
        public Genome genomeGenerator()
        {
            Genome genome = new Genome();
            genome.addNodeGene(new NodeGene(NodeGene.TYPE.INPUT, 0));

            genome.addNodeGene(new NodeGene(NodeGene.TYPE.INPUT, 1));



            genome.addNodeGene(new NodeGene(NodeGene.TYPE.OUTPUT, 2));
            genome.addNodeGene(new NodeGene(NodeGene.TYPE.OUTPUT, 3));


            genome.addConnectionGene(new ConnectionGene(0, 2, (float)(random.NextDouble() * 4f - 2f), true, 1));
            genome.addConnectionGene(new ConnectionGene(1, 2, (float)(random.NextDouble() * 4f - 2f), true, 2));
            genome.addConnectionGene(new ConnectionGene(0, 3, (float)(random.NextDouble() * 4f - 2f), true, 3));
            genome.addConnectionGene(new ConnectionGene(1, 3, (float)(random.NextDouble() * 4f - 2f), true, 4));

            return genome;
        }
    }
}
