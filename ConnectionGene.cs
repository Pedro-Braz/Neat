using System;
using System.Collections.Generic;
using System.Linq;

namespace Neat
{
    public class ConnectionGene
    {
        private int inNode;
        private int outNode;
        private float weight;
        private Boolean expressed;
        private int innovation;

        public ConnectionGene(int inNode, int outNode, float weight, bool expressed, int innovation)
        {
            this.inNode = inNode;
            this.outNode = outNode;
            this.weight = weight;
            this.expressed = expressed;
            this.innovation = innovation;
        }

        public ConnectionGene(ConnectionGene toBeCopied)
        {
            this.inNode = toBeCopied.inNode;
            this.outNode = toBeCopied.outNode;
            this.weight = toBeCopied.weight;
            this.expressed = toBeCopied.expressed;
            this.innovation = toBeCopied.innovation;
        }

        public int InNode { get => inNode; set => inNode = value; }
        public int OutNode { get => outNode; set => outNode = value; }
        public float Weight { get => weight; set => weight = value; }
        public bool Expressed { get => expressed; set => expressed = value; }
        public int Innovation { get => innovation; set => innovation = value; }

        public void disable() {
            this.expressed = false;
        }

        public ConnectionGene copy()
        {
            return new ConnectionGene(this.inNode, this.outNode, this.weight, this.expressed, this.innovation);
        }

    }
}
