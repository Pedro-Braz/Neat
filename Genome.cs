using System;
using System.Collections.Generic;
using System.Linq;

namespace Neat
{
#pragma warning disable 8604, 8600, 8602
    public class Genome
    {
        private Dictionary<int, ConnectionGene> connections;
        private Dictionary<int, NodeGene> nodes;
        private float fitness;
        public Genome()
        {
            this.connections = new Dictionary<int, ConnectionGene>();
            this.nodes = new Dictionary<int, NodeGene>();
            this.fitness = 0;
        }

        public Genome(Genome toBeCopied)
        {
            connections = new Dictionary<int, ConnectionGene>();
            nodes = new Dictionary<int, NodeGene>();

            foreach (ConnectionGene gene in toBeCopied.connections.Values)
            {
                addDefaultConnectionGene(gene);
            }
            foreach(NodeGene node in toBeCopied.nodes.Values)
            {
                addNodeGene(node);
            }
            fitness = toBeCopied.fitness;
        }
        public Dictionary<int, ConnectionGene> Connections { get => connections; }
        public Dictionary<int, NodeGene> Nodes { get => nodes; }
        public float Fitness { get => fitness; set => fitness = value; }



        public void addNodeGene(NodeGene gene)
        {
            nodes.Add(gene.Id, gene);
        }

        public void addDefaultConnectionGene(ConnectionGene gene)
        {
            connections.Add(gene.Innovation, gene);
        }

        public void addConnectionGene(ConnectionGene gene)
        {
            int inn = ConnectionsDataBase.getInnovationNumber(gene);
            gene.Innovation = inn;
            connections.Add(inn, gene);
        }

        public void mutation(float PROBABILITY_PERTURBING)
        {
            Random r = new Random();
            foreach (ConnectionGene con in connections.Values)
            { //PROBABILITY_PERTURBING 90% perturbing and 10% new weight
                float randomFloat = (float)r.NextDouble();
                if (randomFloat < PROBABILITY_PERTURBING)
                {// uniformly perturbing weights
                    con.Weight = con.Weight * (float)r.NextDouble(); 
                }
                else
                {                                               
                    con.Weight = (float)r.NextDouble() * 4f - 2f; // assign new weight between -2 and 2
                }
            }
        }
        public void addConnectionMutation(int maxAttempts)
        {
            Random r = new Random();
            List<int> nodeInnovationNumbers = new List<int>(new int[nodes.Count]);
            int tries = 0;
            while (tries < maxAttempts)
            {
                tries++;
                int keyNode1 = r.Next(nodeInnovationNumbers.Count);
                int keyNode2 = r.Next(nodeInnovationNumbers.Count);

                NodeGene node1 = nodes[keyNode1];
                NodeGene node2 = nodes[keyNode2];
                float weight = (float)r.NextDouble() * 2f - 1f;

                Boolean reversed = checkConnectionReversed(node1, node2);

                if (reversed)
                {
                    NodeGene temp = node1;
                    node1 = node2;
                    node2 = temp;
                }
                Boolean connectionExists = checkConnectionsExists(node1, node2);
                Boolean connectionImpossible = checkConnectionsImpossible(node1, node2);
                Boolean hasCicle = false;


                if (!connectionExists && !connectionImpossible)
                {

                    ConnectionGene newConnection = new ConnectionGene(node1.Id, node2.Id, weight, true, 10000);
                    addDefaultConnectionGene(newConnection);
                    hasCicle = dfs(node2.Id);
                    Connections.Remove(10000);

                    if (!hasCicle)
                    {
                        addConnectionGene(newConnection);
                        return;
                    }

                }
            }
            //Console.WriteLine("Tried, but could not do add connection mutation");
        }


        public void addNodeMutation()
        {
            List<ConnectionGene> suitableConnections = new List<ConnectionGene>();
            foreach (ConnectionGene connection in connections.Values)
            {
                if (connection.Expressed) { suitableConnections.Add(connection); }
            }
            if (suitableConnections.Count == 0)
            {
                //Console.WriteLine("Tried, but could not do add node mutation");

                return;
            }

            Random r = new Random();
            List<int> keyList = connections.Keys.ToList();
            int randomIndex = r.Next(connections.Count());

            ConnectionGene con = new ConnectionGene(connections[keyList[randomIndex]]);
            NodeGene inNode = nodes[con.InNode];
            NodeGene outNode = nodes[con.OutNode];

            con.disable();

            NodeGene newNode = new NodeGene(NodeGene.TYPE.HIDDEN, nodes.Count());
            ConnectionGene inToNew = new ConnectionGene(inNode.Id, newNode.Id, 1f, true, 0);
            ConnectionGene newToOut = new ConnectionGene(newNode.Id, outNode.Id, con.Weight, true, 0);

            addNodeGene(newNode);
            addConnectionGene(inToNew);
            addConnectionGene(newToOut);

        }
        /*
             @param fit
                = 1  : parent 1 more fit
                = 0  : equal fitness
                = -1 : parent 2 more fit
        */
        public static Genome crossover(Genome parent1, Genome parent2, int fit, float DISABLED_GENE_INHERITING_CHANCE)
        {
            Random r = new Random();
            Genome child = new Genome();
            Genome temp = new Genome();
            Boolean hasCicle = false;

            if (fit == -1)
            {
                temp = parent1;
                parent1 = parent2;
                parent2 = temp;
            }

            foreach (NodeGene parent1Node in parent1.Nodes.Values)
            {
                child.addNodeGene(parent1Node.copy());
            }

            foreach (ConnectionGene parent1Conn in parent1.Connections.Values)
            {
                if (parent2.Connections.ContainsKey(parent1Conn.Innovation))
                { // matching gene
                    ConnectionGene parent2Conn = parent2.Connections.GetValueOrDefault(parent1Conn.Innovation);

                    Boolean disabled = !parent1Conn.Expressed || !parent2Conn.Expressed;

                    Boolean randomBoolean = r.Next(2) == 0;
                    ConnectionGene childConGene = randomBoolean ? new ConnectionGene(parent1Conn) : new ConnectionGene(parent2Conn);
                    if (disabled && (float)r.NextDouble() < DISABLED_GENE_INHERITING_CHANCE)
                    {
                        childConGene.disable();
                    }
                    child.addConnectionGene(childConGene);
                }
                else
                { // disjoint or excess
                    ConnectionGene childConGene = new ConnectionGene(parent1Conn);
                    child.addConnectionGene(childConGene);
                }
            }
            if (fit == 0)
            {
                foreach (NodeGene parent2Node in parent2.Nodes.Values)
                {
                    if (!child.Nodes.ContainsKey(parent2Node.Id))
                    {
                        child.addNodeGene(parent2Node.copy());
                    }

                }
                foreach (ConnectionGene parent2Conn in parent2.Connections.Values)
                {
                    if (!child.Connections.ContainsKey(parent2Conn.Innovation))
                    {
                        ConnectionGene childConGene = new ConnectionGene(parent2Conn);
                        child.addConnectionGene(childConGene);
                        hasCicle = child.dfs(childConGene.OutNode);
                        if (hasCicle)
                        {
                            foreach (int id in child.connections.Keys)
                            {
                                if (child.connections[id] == childConGene)
                                {
                                    child.connections.Remove(id);
                                }
                            }
                            hasCicle = false;
                        }
                    }
                }
            }
            return child;

        }
        public Boolean checkConnectionsExists(NodeGene n1, NodeGene n2)
        {
            Boolean connectionExists = false;
            foreach (ConnectionGene con in connections.Values)
            {
                if ((con.InNode == n1.Id && con.OutNode == n2.Id) || (con.InNode == n2.Id && con.OutNode == n2.Id))
                {
                    connectionExists = true;
                    break;
                }
            }
            return connectionExists;
        }

        public Boolean checkConnectionsImpossible(NodeGene n1, NodeGene n2)
        {
            Boolean connectionImpossible = false;
            if (n1.Type == NodeGene.TYPE.INPUT && n2.Type == NodeGene.TYPE.INPUT)
            {
                connectionImpossible = true;
            }
            else if (n1.Type == NodeGene.TYPE.OUTPUT && n2.Type == NodeGene.TYPE.OUTPUT)
            {
                connectionImpossible = true;
            }
            else if (n1 == n2)
            {
                connectionImpossible = true;
            }
            return connectionImpossible;
        }

        public Boolean checkConnectionReversed(NodeGene n1, NodeGene n2)
        {
            Boolean reversed = false;
            if (n1.Type == NodeGene.TYPE.HIDDEN && n2.Type == NodeGene.TYPE.INPUT)
            {
                reversed = true;
            }
            else if (n1.Type == NodeGene.TYPE.OUTPUT && n2.Type == NodeGene.TYPE.HIDDEN)
            {
                reversed = true;
            }
            else if (n1.Type == NodeGene.TYPE.OUTPUT && n2.Type == NodeGene.TYPE.INPUT)
            {
                reversed = true;
            }
            return reversed;
        }

        public static float compatibilityDistance(Genome genome1, Genome genome2, float c1, float c2, float c3)
        {
            //Numbers of genes in the larger genome, fewer than 20 genes N=1
            int matchingGenes = 0;
            int excessGenes = 0;
            int disjointGenes = 0;
            int N = 1;
            float weightDifference = 0;
            float avgWeightDiff = 0;

            List<int> conKeys1 = genome1.Connections.Keys.ToList();
            List<int> conKeys2 = genome2.Connections.Keys.ToList();
            conKeys1.Sort();
            conKeys2.Sort();
            int highestInnovation1 = conKeys1[conKeys1.Count() - 1];
            int highestInnovation2 = conKeys2[conKeys2.Count() - 1];
            int indices = Math.Max(highestInnovation1, highestInnovation2);

            for (int i = 0; i <= indices; i++)
            {// loop through genes -> i is innovation numbers
                ConnectionGene connection1 = null;
                ConnectionGene connection2 = null;

                if (genome1.Connections.ContainsKey(i)) { connection1 = genome1.Connections[i]; }
                if (genome2.Connections.ContainsKey(i)) { connection2 = genome2.Connections[i]; }

                if (connection1 != null)
                {
                    if (connection2 != null)
                    {
                        matchingGenes++;
                        weightDifference += Math.Abs(connection1.Weight - connection2.Weight);
                    }
                    else if (highestInnovation2 < i)
                    {
                        excessGenes++;
                    }
                    else
                    {
                        disjointGenes++;
                    }
                }
                else if (connection2 != null)
                {
                    if (highestInnovation1 < i)
                    {
                        excessGenes++;
                    }
                    else
                    {
                        disjointGenes++;
                    }
                }
            }
            avgWeightDiff = weightDifference / matchingGenes;
            //Console.WriteLine("excess:" + excessGenes + "\n" +"matching: "+matchingGenes  +"\n" + "disjoints:" + disjointGenes + "\n" + "avgWeightDiff:" + avgWeightDiff);
            return (excessGenes * c1) / N + (disjointGenes * c2) / N + avgWeightDiff * c3;
        }

        public Boolean dfs(int start)
        {
            Dictionary<int, Boolean> visited = new Dictionary<int, bool>();
            List<int> stack = new List<int>();
            for (int i = 0; i < nodes.Count; i++)
            {
                visited[i] = false;
            }
            stack.Add(start);
            while (stack.Count > 0)
            {
                int u = stack.Last();
                stack.Remove(u);
                if (visited[u] == false)
                {
                    visited[u] = true;
                    foreach (ConnectionGene gene in Connections.Values)
                    {
                        if (gene.InNode == u)
                        {
                            stack.Add(gene.OutNode);
                        }
                    }
                }
                else if (u == start)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

//Problemas
//1- addConnectionMutation isReversed existe?
//2- excess or disjoint chose more fit parent. equal fitness chose random

