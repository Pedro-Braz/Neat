using System;
using System.Collections.Generic;
using System.Linq;

namespace Neat
{
    public class NodeGene
    {
        public enum TYPE { INPUT, HIDDEN, OUTPUT };
        private TYPE type;
        private int id;
        //private List<int> inNodes;
        //private List<int> outNodes;
        //private List<int> hiddenNodes;


        public NodeGene(TYPE type, int id)
        {
            this.type = type;
            this.id = id;
        }

        public TYPE Type { get => type; set => type = value; }
        public int Id { get => id; set => id = value; }

        public NodeGene copy()
        {
            return new NodeGene(type, id);
        }
    }
}
