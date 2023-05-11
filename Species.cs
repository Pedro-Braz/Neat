using System;
using System.Collections.Generic;
using System.Linq;

namespace Neat
{
    public class Species
    {
        public Genome bestGenome;
        public List<Genome> genomes;
        public float totalAdjustedFitness = 0f;

        public Species(Genome g)
        {
            this.bestGenome = new Genome(g);
            this.genomes = new List<Genome>();
            this.genomes.Add(g);
        }

        public float SpeciesAdjustedFitness()
        {
            float totalFitness = 0;
            for (int i = 0; i < genomes.Count; i++)
            {
                totalFitness += genomes[i].Fitness;
            }
            totalAdjustedFitness = totalFitness / genomes.Count;
            return totalAdjustedFitness;
        }

        public void addGenome(Genome g)
        {
            genomes.Add(g);
        }
        public void sortGenomes()
        {
            genomes.Sort((x, y) => -x.Fitness.CompareTo(y.Fitness));
            bestGenome = new Genome(genomes[0]);
        }

        public void reset()
        {
            genomes.Clear();
            genomes.Add(new Genome(bestGenome));
            totalAdjustedFitness = 0f;
        }

        public void killWorstsGenomes(int cutoffIndex)
        {
            while (genomes.Count > cutoffIndex)
            {
                genomes.RemoveAt(genomes.Count - 1);
            }
        }
    }
}
