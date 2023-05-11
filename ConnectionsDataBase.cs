using System;
using System.Collections.Generic;
using System.Linq;

namespace Neat
{
    public class ConnectionsDataBase
    {
        public static Dictionary<ConnectionGene, int> connections = new Dictionary<ConnectionGene, int>();
        public static int getInnovationNumber(ConnectionGene connection)
        {
            foreach (ConnectionGene connectionGene in connections.Keys)
            {
                if (connectionGene.InNode == connection.InNode && connectionGene.OutNode == connection.OutNode)
                {
                    return connections[connectionGene];
                }
            }
            connections.Add(connection, connections.Count() + 1);
            return connections.Count();
        }
        public static void reset()
        {
            connections.Clear();
        }
    }
}
