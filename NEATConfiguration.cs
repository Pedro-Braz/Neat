
namespace Neat
{
    public class NEATConfiguration
	{
		/***
	 * constant used in genomic distance calculation - this is the weight of excess genes
	 */
		public float C1 = 1.0f;

		/***
		 * constant used in genomic distance calculation - this is the weight of disjoint genes
		 */
		public float C2 = 0.5f;

		/**
		 * constant used in genomic distance calculation - this is the weight of average connection weight difference
		 */
		public float C3 = 0.4f;

		/**
		 * genomic distance 
		 */
		public float DT = 3f;


		public float ASEXUAL_REPRODUCTION_RATE = 0.25f;

		/**
		 * chance for each child to have it's weights mutated
		 */
		public float MUTATION_RATE = 0.8f;

		/**
		 * This applies to mutation of genomes.
		 */
		public float PERTURBING_RATE = 0.9f;

		/**
		 * Chance of a weight being disabled if it is disabled in either parent
		 */
		public float DISABLED_GENE_INHERITING_CHANCE = 0.75f;

		/**
		 * Chance of mutating a child in a way that adds a connection to the genome.
		 */
		public float ADD_CONNECTION_RATE = 0.5f;

		/**
		 * Chance of mutating a child in a way that adds a node to the genome.
		 */
		public float ADD_NODE_RATE = 0.5f;

		public float OFFSPRING_FROM_CROSSOVER = 0.75f;

		public float INTERSPECIES_MATING_RATE = 0.01f;

		private int populationSize;

		private float fitness;

		public NEATConfiguration(int popSize, float fitness)
		{
			this.populationSize = popSize;
			this.fitness = fitness;

		}

		public int getPopulationSize()
		{
			return populationSize;
		}

		public float getMaxFitness()
		{
			return fitness;
		}
	}
}
