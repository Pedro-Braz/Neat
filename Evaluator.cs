using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neat
{
    public class Evaluator
    {
        private NEATConfiguration config;

        public List<Genome> genomes;                     // stores all genomes of current generation
        protected List<Genome> nextGeneration;              // stores next generation of genomes (used during evaluation)
        public Genome fittestGenome;              // fittest genome w/ score form last run generation
        protected List<Species> species;

        protected List<Genome> lastGenerationResults;    // contains a sorted list of the previous generations genomes

        public int i1, i2, i3, i4;
        public int g1, g2, g3, g4;

        public float fitnessAverage;
        public float maxFitness;
        public Boolean complete;

        Random r = new Random();
        public Evaluator(NEATConfiguration configuration, Func<Genome> generateGenome)
        {
            this.config = configuration;

            genomes = new List<Genome>();
            for (int i = 0; i < configuration.getPopulationSize(); i++)
            {
                Genome g = generateGenome();
                genomes.Add(g);
            }
            fittestGenome = genomes[0];
            nextGeneration = new List<Genome>();
            lastGenerationResults = new List<Genome>();
            species = new List<Species>();
            createSpecies();
            /*i4 = 1;*/
            maxFitness = config.getMaxFitness();
            complete = false;
        }

        public void evaluateGeneration()
        {
/*            i1 = 0;
            i2 = 0;
            i3 = 0;*/

            resetGeneration();

            evaluateInitialize();

/*            getPopulationCounts();
*/
            species_Into_nextGeneration();

            //fill in next generation by random mating and mutation
            while (nextGeneration.Count() < config.getPopulationSize())
            {
                Species s, s2;

                s = getRandomSpeciesBiasedAjdustedFitness();
                s2 = getRandomSpeciesBiasedAjdustedFitness();

                int genomeIndex2 = r.Next(s.genomes.Count);
                int genomeIndex = 0;

                Genome parent1 = new Genome(s.genomes[genomeIndex]);
                Genome child, parent2;

                if ((float)r.NextDouble() > config.ASEXUAL_REPRODUCTION_RATE)
                { // sexual reproduction
                    if ((float)r.NextDouble() > config.INTERSPECIES_MATING_RATE)
                    { // interspecies crossover
                        if (s.genomes.Count > 1)
                        {
                            while (genomeIndex == genomeIndex2)
                            {
                                genomeIndex2 = r.Next(s.genomes.Count);
                            }
                        }
                        parent2 = new Genome(s.genomes[genomeIndex2]);
                        child = genomeReproduction(parent1, parent2, 1);
                    }
                    else
                    {
                        genomeIndex2 = r.Next(s2.genomes.Count);
                        parent2 = new Genome(s2.genomes[genomeIndex2]);
                        child = genomeReproduction(parent1, parent2, 1);
                    }
                }
                else
                {// asexual reproduction
                    child = genomeReproduction(parent1, parent1, 0);
                }
                child.Fitness = 0;
                nextGeneration.Add(new Genome(child));
            }

            lastGenerationResults.Clear();
            lastGenerationResults = new List<Genome>(genomes);

            genomes.Clear();
            genomes = new List<Genome>(nextGeneration);
            genomes.Add(fittestGenome);
           /* save();*/
            createSpecies();
            genomes.ForEach(genome => genome.Fitness = 0);
            /*i4++;*/
        }

        public Genome genomeReproduction(Genome parent1, Genome parent2, int sexuaReprodution)
        {
            Genome child = new Genome(parent1);
            if (sexuaReprodution == 1)
            {
                if (parent1.Fitness > parent2.Fitness)
                {
                    child = new Genome(Genome.crossover(parent1, parent2, 1, config.DISABLED_GENE_INHERITING_CHANCE));
                }
                else if (parent1.Fitness < parent2.Fitness)
                {
                    child = new Genome(Genome.crossover(parent2, parent1, -1, config.DISABLED_GENE_INHERITING_CHANCE));
                }
                else
                {
                    child = new Genome(Genome.crossover(parent2, parent1, 0, config.DISABLED_GENE_INHERITING_CHANCE));
                }
            }

            if ((float)r.NextDouble() < config.MUTATION_RATE)
            {
                child.mutation(config.PERTURBING_RATE);
            }
            if ((float)r.NextDouble() < config.ADD_CONNECTION_RATE)
            {   // add mutation from adding connection and nodes
                child.addConnectionMutation(100);
            }
            if ((float)r.NextDouble() < config.ADD_NODE_RATE)
            {   // add mutation from adding node
                child.addNodeMutation();
            }
            return child;
        }

        public void evaluateInitialize()
        {
            //Sort Genomes
            genomes.Sort((x, y) => -x.Fitness.CompareTo(y.Fitness));
            species.Sort((x, y) => -x.totalAdjustedFitness.CompareTo(y.totalAdjustedFitness));
            genomes.ForEach(genome => fitnessAverage += genome.Fitness);

            fitnessAverage = fitnessAverage / genomes.Count;
            //Save fittestGenome
            fittestGenome = new Genome(genomes[0]);
            nextGeneration.Add(new Genome(genomes[0]));
        }
        public void species_Into_nextGeneration()
        {
            //Best genomes from each species into next generation
            foreach (Species s in species)
            {
                s.sortGenomes();
                nextGeneration.Add(new Genome(s.bestGenome));
                getSpeciesCounts(s);
                int count = s.genomes.Count();
                int cutoffIndex = 1;
                if (count >= 20) { cutoffIndex = count / 10; }
                s.killWorstsGenomes(cutoffIndex);
                for (int i = 1; i < s.genomes.Count; i++)
                {
                    nextGeneration.Add(new Genome(s.genomes[i]));
                }

            }
        }
        public void resetGeneration()
        {
            nextGeneration.Clear();
            fitnessAverage = 0;
        }

/*        public void getPopulationCounts()
        {
            i1 = 0;
            i2 = 0;
            i3 = 0;
            g1 = 0;
            g2 = 0;
            g3 = 0;

            genomes.ForEach((genome) =>
            {
                if (genome.Fitness > 20)
                {
                    g3++;
                }
                else if (genome.Fitness > 11)
                {
                    g2++;
                }
                else if (genome.Fitness > 7)
                {
                    g1++;
                }
            });
        }*/

        public void getSpeciesCounts(Species s)
        {
            if (s.bestGenome.Fitness > 20)
            {
                i3++;
            }
            else if (s.bestGenome.Fitness > 11)
            {
                i2++;
            }
            else if (s.bestGenome.Fitness > 7)
            {
                i1++;
            }
        }
        public int getSpecies()
        {
            return species.Count();
        }

        public void save()
        {
            string genXspec = "D:/Windows/Desktop/Neat/jogo/GenxSpec.json";
            string stats = $"D:/Windows/Desktop/Neat/jogo/stats-{config.getPopulationSize()}.json";
            string completeGeneration = "D:/Windows/Desktop/Neat/jogo/completeGeneration.json";

            if (!complete && fittestGenome.Fitness >= maxFitness)
            {
                complete = true;
                File.AppendAllText(completeGeneration, "\n{\n\"Generation\": " + i4 + "\n}");
            }

            if (i4 == 1 || i4 == 5 || i4 == 15 || i4 == 25 || i4 == 50)
            {
                File.AppendAllText(genXspec, "\n{\n\"Generation\": " + i4 + ",");
                File.AppendAllText(genXspec, "\n\"Species\": " + species.Count + ",");
                File.AppendAllText(genXspec, "\n\"FitnessAverage\": " + fitnessAverage + ",");
                File.AppendAllText(genXspec, "\n\"species_obstaculo_1\": " + i1 + ",");
                File.AppendAllText(genXspec, "\n\"species_obstaculo_2\": " + i2 + ",");
                File.AppendAllText(genXspec, "\n\"species_obstaculo_3\": " + i3 + ",");
                File.AppendAllText(genXspec, "\n\"population_obstaculo_1\": " + g1 + ",");
                File.AppendAllText(genXspec, "\n\"population_obstaculo_2\": " + g2 + ",");
                File.AppendAllText(genXspec, "\n\"population_obstaculo_3\": " + g3);
                File.AppendAllText(genXspec, "\n},");

                File.AppendAllText(stats, "\n{\"Generation\": " + i4 + ",");
                File.AppendAllText(stats, "\n\"FittestGenome\": {");
                File.AppendAllText(stats, "\n\"Nodes\": " + fittestGenome.Nodes.Count + ",");
                File.AppendAllText(stats, "\n\"Con\": " + fittestGenome.Connections.Count + ",");
                File.AppendAllText(stats, "\n\"Fitness\": " + fittestGenome.Fitness + ",");
                int i = 0;
                foreach (var item in fittestGenome.Connections)
                {
                    ConnectionGene con = item.Value;
                    File.AppendAllText(stats, $"\n\"Con{item.Key}\":" + "{");

                    File.AppendAllText(stats, $"\n\"NIn\": {con.InNode},");
                    File.AppendAllText(stats, $"\n\"Nout\": {con.OutNode},");
                    File.AppendAllText(stats, $"\n\"Peso\": {con.Weight},");
                    File.AppendAllText(stats, $"\n\"Expressed\": {(con.Expressed ? "true" : "false")},");
                    File.AppendAllText(stats, $"\n\"Innovation\":{con.Innovation}");
                    if (i == fittestGenome.Connections.Count - 1)
                    {
                        File.AppendAllText(stats, "\n}");
                    }
                    else
                    {
                        File.AppendAllText(stats, "\n},");
                    }
                    i++;
                }
                File.AppendAllText(stats, "\n}");
                if (i4 > 30)
                {
                    File.AppendAllText(stats, "\n}");
                }
                else
                {
                    File.AppendAllText(stats, "\n},");
                }


                File.AppendAllText("stats.json", "\n},");
            }

        }

        public void createSpecies()
        {
            // Place genomes into species
            foreach (Genome g in genomes)
            {
                Boolean foundSpecies = false;
                foreach (Species s in species)
                {
                    float distance = Genome.compatibilityDistance(g, s.genomes[0], config.C1, config.C2, config.C3);
                    if (distance == 0)
                    {
                        foundSpecies = true;
                        break;
                    }
                    else if (distance < config.DT)
                    { // compatibility distance is less than DT, so genome belongs to this species
                        s.addGenome(g);
                        foundSpecies = true;
                        break;
                    }
                }
                if (!foundSpecies)
                { // if there is no appropiate species for genome, make a new one
                    Species newSpecies = new Species(g);
                    species.Add(newSpecies);

                }
            }
        }

        private Species getRandomSpeciesBiasedAjdustedFitness()
        {
            Random random = new Random();
            double completeWeight = 0.0;    
            foreach (Species s in species)
            {
                completeWeight += s.totalAdjustedFitness;
            }
            double r = random.NextDouble() * completeWeight;
            double countWeight = 0.0;
            foreach (Species s in species)
            {
                countWeight += s.totalAdjustedFitness;
                if (countWeight >= r)
                {
                    return s;
                }
            }
            throw new Exception();
        }
    }
}
