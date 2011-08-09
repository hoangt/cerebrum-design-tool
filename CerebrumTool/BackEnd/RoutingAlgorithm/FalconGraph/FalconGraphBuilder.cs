/******************************************************************** 
 * Cerebrum Embedded System Design Automation Framework
 * Copyright (C) 2010  The Pennsylvania State University
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 ********************************************************************/
/**********************************************************
 * FalconGraphBuilder.cs
 * Name S. Desmond Nathanson (with code inherited from Ahmed Al Maashri)
 * Date: June 4 2010
 * Description: This is the heart of the routing algorithm,
 *              responsible for most of the heavy lifting.
 *              It inherits the structure FalconGraph and
 *              adds many new methods and private varialbes
 *              in order to allow it to read in an XML file,
 *              generate routing tables, and save the data
 *              to an XML file.
 * 
 * Hisory
 * >> Matthew Cotter       (18 August, 2010): Added call to FalconGlobal.FalconFileRoutines.WriteCerebrumDisclaimerXml() to WriteTables
 * >> S. Desmond Nathanson (4 June, 2010): Comments added
 * >> S. Desmond Nathanson (23 July, 2010): Adding support for arbitrary string names to identify the FPGA.
 * >> S. Desmond Nathanson (9 August, 2010): Components are now getting assigned to arbitrary types
 * >> S. Desmond Nathanson (25 August, 2010): Fixed a bug which caused the reuse of any source node to throw an exception. Also added
 *                          exception protection. Then added .ToLower to all strings read in so the XML file is now case-insensitive. 
 *********************************************************/
#region UsingDirectives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using SkmDataStructures2;
//using DisplayClient;
using System.Xml;
using FalconGlobal;
#endregion

namespace FalconGraph
{
    /// <summary>
    /// The major class in the routing algorithm. The two methods that are needed are its constructor
    /// and the generate tables methods.
    /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/Routing_Specs.pdf">
    /// The input specifications for the FalconGraphBuilder constructor.
    /// </seealso>
    /// </summary>
    public class FalconGraphBuilder<T>: IFalconLibrary 
    {
        #region Private
        // Step 1: create graph instance
        private FalconGraph<T> Design_Graph = new FalconGraph<T>();
        // create a Hashtable for the shortest distance and paths
        private ArrayList dist_table;
        private ArrayList route_table;
        private ArrayList port_table;
        private int NumOfNodes;
        private int NumOfComponents;
        private int FalconNodeNoPorts;        
        private ArrayList Component_Connections = new ArrayList(); //An array which will hold the tuples that describe which components need to be connected
        //private int highest_component; //Used to know how big the above 2d array is
        private Dictionary<T, int> NodeLookup = new Dictionary<T,int>(); //Used to compare different nodes, T doesn't support this
        private Dictionary<T, int> ComponentLookup = new Dictionary<T, int>(); //Used to compare different components, T doesn't support this
        private struct ComponentSaver
        {
            public T SourceComponent;
            public T SinkComponent;
            public T SourceFPGA;
            public T SinkFPGA;
            public float Weight; //In case we can find out the relative weights of the respective component connections. Currently unused.
        }
        private ComponentSaver ComponentInfo;
        private ArrayList FPGA_connections = new ArrayList(); //An array which will hold FPGA edges

        #endregion

        #region Constructors
        
        #region LegacyConstructors
        //**********************************************
        //These are the old constructors from when this class was FantomGraphBuilder. They're left in
        //just in case someone needs a new constructor
        //**********************************************
        //public FalconGraphBuilder()
        //{
        //    NumOfNodes = 4;
        //    dist_table = new ArrayList();
        //    route_table = new ArrayList();
        //    port_table = new ArrayList();

        //    for (int i = 0; i < NumOfNodes; i++)
        //    {
        //        dist_table.Add(new Hashtable());
        //        route_table.Add(new Hashtable());
        //        port_table.Add(new Hashtable());
        //    }

        //    FalconNodeNoPorts = 8;    
        //}

        ///// <summary>
        ///// Constructor
        ///// </summary>
        ///// <param name="num">Number of Nodes</param>
        //public FalconGraphBuilder(int no_nodes, int no_ports)
        //{
        //    NumOfNodes = no_nodes;
        //    dist_table = new ArrayList();
        //    route_table = new ArrayList();
        //    port_table = new ArrayList();

        //    for (int i = 0; i < NumOfNodes; i++)
        //    {
        //        dist_table.Add(new Hashtable());
        //        route_table.Add(new Hashtable());
        //        port_table.Add(new Hashtable());
        //    }

        //    FalconNodeNoPorts = no_ports;           
        //}
        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="XML_path">String which points the program to the relevant XML file</param>
        public FalconGraphBuilder(string XML_path)
        {
            try
            {
                XmlReader FPGA_data = XmlReader.Create(XML_path);
                NumOfNodes = 0;
                NumOfComponents = 0;
                //highest_component = 0; //Used to help build the routing table 2D array
                while (FPGA_data.Read())
                {
                    switch (((string)FPGA_data.Name).ToLower())
                    {
                        case "node":
                            //Contains information on the number of FPGAs and their IDs
                            //Add the node to the graph
                            T newnode = (T)(Convert.ChangeType(FPGA_data.GetAttribute("ID"), typeof(T)));
                            Design_Graph.AddNode(newnode);
                            NodeLookup.Add(newnode, NumOfNodes);
                            NumOfNodes++;
                            break;

                        case "connectioninfo":
                            //Contains information on component locations and connections
                            //Read in the data into the struct ComponentInfo
                            ComponentInfo.SourceComponent = (T)Convert.ChangeType((string)(FPGA_data.GetAttribute("SourceComponent")), typeof(T));
                            ComponentInfo.SinkComponent = (T)Convert.ChangeType((string)(FPGA_data.GetAttribute("SinkComponent")), typeof(T));
                            ComponentInfo.SourceFPGA = (T)Convert.ChangeType((string)FPGA_data.GetAttribute("SourceNode"), typeof(T));
                            ComponentInfo.SinkFPGA = (T)Convert.ChangeType((string)FPGA_data.GetAttribute("SinkNode"), typeof(T));
                            ComponentInfo.Weight = float.Parse(FPGA_data.GetAttribute("Weight"));
                            
                            //Add the entry to the component dictionary if it doesn't already exist
                            if (!ComponentLookup.ContainsKey(ComponentInfo.SourceComponent))
                            {    
                                ComponentLookup.Add(ComponentInfo.SourceComponent, NumOfComponents);
                                NumOfComponents = NumOfComponents + 1; 
                            }
                            if (!ComponentLookup.ContainsKey(ComponentInfo.SinkComponent))
                            {
                                ComponentLookup.Add(ComponentInfo.SinkComponent, NumOfComponents);
                                NumOfComponents = NumOfComponents + 1; 
                            }

                            //Save the struct to the ArrayList component_connections
                            Component_Connections.Add(ComponentInfo);
                            /*
                            if (ComponentInfo.SourceComponent > highest_component)
                                highest_component = ComponentInfo.SourceComponent;
                            if (ComponentInfo.SinkComponent > highest_component)
                                highest_component = ComponentInfo.SinkComponent;
                             */
                            break;

                        case "linkinfo":
                            //Contains information on the links of the FPGAs
                            //Add the edge to the graph (it should now be populated if the XML was made correctly)
                            float weight = 1 / float.Parse(FPGA_data.GetAttribute("LinkSpeed"));
                            T Source = (T)Convert.ChangeType(FPGA_data.GetAttribute("SourceNode"), typeof(T));
                            T Sink = (T)Convert.ChangeType(FPGA_data.GetAttribute("SinkNode"), typeof(T));
                            
                            Design_Graph.AddDirectedEdge(Source, Sink, weight);
                            if (((string)FPGA_data.GetAttribute("Bidirectional")).ToLower() == "true")
                                Design_Graph.AddDirectedEdge(Sink, Source, weight);
                            break;
                    }
                }
                FPGA_data.Close();

                //Make the routing table with bad addresses by default
                //RoutingTable = new int[NumOfNodes + 1, highest_component + 1];
                //for (int i = 0; i <= NumOfNodes; i++)
                //    for (int j = 0; j <= highest_component; j++)
                //       RoutingTable[i, j] = -1;

                dist_table = new ArrayList();
                route_table = new ArrayList();
                port_table = new ArrayList();

                foreach (FalconNode<T> SourceNode in Design_Graph.Nodes)
                {
                    dist_table.Add(new Hashtable());
                    route_table.Add(new Hashtable());
                    port_table.Add(new Hashtable());
                }

                foreach (FalconNode<T> SourceNode in Design_Graph.Nodes)
                    foreach (T Component in ComponentLookup.Keys)
                        ((Hashtable)route_table[NodeLookup[SourceNode.Value]]).Add(ComponentLookup[Component], default(T));

                foreach (FalconNode<T> SourceNode in Design_Graph.Nodes)
                {
                    foreach (FalconNode<T> SinkNode in Design_Graph.Nodes)
                    {

                        if (SourceNode.Neighbors.Contains(SinkNode))
                            ((Hashtable)(dist_table[NodeLookup[SourceNode.Value]])).Add(NodeLookup[SinkNode.Value], SourceNode.Costs[SourceNode.Neighbors.IndexOf(SinkNode)]);
                        else if (SourceNode == SinkNode)
                            ((Hashtable)(dist_table[NodeLookup[SourceNode.Value]])).Add(NodeLookup[SinkNode.Value], (float)0);
                        else
                            ((Hashtable)(dist_table[NodeLookup[SourceNode.Value]])).Add(NodeLookup[SinkNode.Value], Single.PositiveInfinity);
                    }
                }
                FalconNodeNoPorts = 8;

            }
            catch(Exception ex)
            {
                Console.WriteLine("Error in the construction of FalconGraphBuilder:" + ex.Message);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Generates the routing tables so the platforms can communicate, this one doesn't save the data externally
        /// </summary>
        public void GenerateTables()
        {
            try
            {
                foreach (ComponentSaver RouteRequired in Component_Connections)
                {
                    if (NodeLookup[RouteRequired.SourceFPGA] != NodeLookup[RouteRequired.SinkFPGA]) //If they are equal, no route is needed
                    {
                        T destination = RouteRequired.SinkComponent;
                        Stack<T> MapStack = ComputeShortestPath(RouteRequired.SourceFPGA, RouteRequired.SinkFPGA);

                        T CurrentNode = (T)Convert.ChangeType(MapStack.Pop(), typeof(T));
                        T NextNode = default(T);
                        //Follow the path, incrementing the edges by epsilon as you go
                        while (MapStack.Count > 0)
                        {//Trace through Djikstra's path until a pre-existing route is found
                            if (((Hashtable)route_table[NodeLookup[CurrentNode]])[destination] == null && NodeLookup[CurrentNode] != NodeLookup[RouteRequired.SinkFPGA]) //Not yet mapped
                            {
                                NextNode = MapStack.Pop();
                                Design_Graph.UpdateWeight(CurrentNode, NextNode, UpdateCalculator());
                                ((Hashtable)route_table[NodeLookup[CurrentNode]]).Remove(destination);
                                ((Hashtable)route_table[NodeLookup[CurrentNode]]).Add(destination, NextNode);
                                CurrentNode = NextNode;
                            }
                            else
                                break; //It's hit a pre-existing route or it's routed to the end, start the second loop
                        }

                        while (NodeLookup[CurrentNode] != NodeLookup[RouteRequired.SinkFPGA])
                        {//Trace the premapped route, incrementing as you go
                            NextNode = (T)Convert.ChangeType((((Hashtable)route_table[NodeLookup[CurrentNode]])[destination]),typeof(T));
                            Design_Graph.UpdateWeight(CurrentNode, NextNode, UpdateCalculator());
                            CurrentNode = NextNode;
                        }
                    }
                }
            }
            catch(Exception ex)
            
            {
                Console.WriteLine("Error in FalconGraphBuilder routing table generation:\n" + ex.Message);
            }
        }

        /// <summary>
        /// Generates the routing tables so the platforms can communicate, this one saves the data to an XML file indicated by
        /// the input string
        /// </summary>
        ///<param name="XML_dest">String pointing to the XML file to be written to</param>
        /// <returns>True if completed successfully</returns>
        public void GenerateTables(string XML_dest)
        {
            GenerateTables();
            WriteTables(XML_dest);
        }

        ///<summary> Writes the tables to an XML file at the location specified</summary>
        ///<param name="XML_dest"> String containing the path to the XML file to be written</param>
        ///<returns>True if succesful, false otherwise</returns>
        private void WriteTables(string XML_dest)
        {
            //XmlTextWriter writeout = new XmlTextWriter(XML_dest, Encoding.UTF8);
            //XmlDocument writeout = new XmlDocument();
            try
            {
                XmlDocument writeout = new XmlDocument();

                XmlTextWriter xmlWriter = new XmlTextWriter(XML_dest, System.Text.Encoding.UTF8);
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                xmlWriter.WriteStartElement("Routing_Tables");
                xmlWriter.Close();
                writeout.Load(XML_dest);

                XmlNode root = writeout.DocumentElement;
                FalconFileRoutines.WriteCerebrumDisclaimerXml(root);    // Added by Matthew Cotter 8/18/2010
                IEnumerator<T> FPGA_set = Design_Graph.GetEnumerator();
                T NodeID;
                while (FPGA_set.MoveNext())
                {

                    XmlElement FPGA = writeout.CreateElement("Node");
                    FPGA.SetAttribute("ID", (string)Convert.ChangeType(FPGA_set.Current,typeof(string)));
                    foreach(T Component in ComponentLookup.Keys)
                    {
                        if (((Hashtable)(route_table[NodeLookup[FPGA_set.Current]]))[Component] != null)
                        {
                            XmlElement xComponent = writeout.CreateElement("Component");
                            xComponent.SetAttribute("ID", Convert.ToString(Component));
                            NodeID = (T)(((Hashtable)(route_table[NodeLookup[FPGA_set.Current]]))[Component]);
                            xComponent.SetAttribute("Destination", Convert.ToString(NodeID));
                            FPGA.AppendChild(xComponent);
                        }
                    }
                    root.AppendChild(FPGA);
                }
                writeout.Save(XML_dest);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error in saving the tables in FalconGraphBuilder" + ex.Message);
            }
        }

        /// <summary>
        /// This is the method to calculate how much weight each edge should gain
        /// when it is assigned to be used by some routing table
        /// !!!WILL BE UPDATED -- CURRENTLY ONLY A PLACEHOLDER!!!!
        /// </summary>
        /// <returns>Float weight</returns>
        
        private float UpdateCalculator()
        {//!!!WILL BE UPDATED -- CURRENTLY ONLY A PLACEHOLDER!!!!
            float weight = (float).001;
            return weight;
        }
        
        //Walk through the Dijkstra path, halting once it arrives at an old path
        /// <summary>
        /// Finds the shortest path from origin to destination
        /// </summary>
        /// <param name="start">Source FalconNode</param>
        /// <param name="end">Destination FalconNode</param>
        /// <returns> A stack containing the path (in ints) of the shortest path</returns>
        public Stack<T> ComputeShortestPath(T start, T end)
        {
            FalconNodeList<T> nodes = Design_Graph.Nodes;	// nodes == Q
            
            ArrayList Node_stacks = new ArrayList(); //Saves the route used to get to a given node from start
            foreach (FalconNode<T> u_node in nodes)
                Node_stacks.Add(new Stack<T>());
            foreach (FalconNode<T> u_node in nodes.FindByValue(start).Neighbors)
                ((Stack<T>)(Node_stacks[NodeLookup[u_node.Value]])).Push(start);

            /**** START DIJKSTRA ****/
            while (nodes.Count > 0)
            {
                FalconNode<T> ClosestNode = GetMin(nodes, start);		// get the minimum node
                nodes.Remove(ClosestNode);			// remove it from the set Q

                // iterate through all of ClosestNode's neighbors
                if (ClosestNode.Neighbors != null)
                    for (int i = 0; i < ClosestNode.Neighbors.Count; i++)
                        Node_stacks = Relax(start, ClosestNode.Value, ClosestNode.Neighbors[i].Value, ClosestNode.Costs[i], Node_stacks);		// relax each edge
            }	// repeat until Q is empty
            /**** END DIJKSTRA ****/

            // Display results
            string results = "The shortest path from " + start + " to " + end + " is " + ((Hashtable)dist_table[NodeLookup[start]])[NodeLookup[end]].ToString()
                             + " and goes as follows: ";

            Stack traceBackSteps = new Stack();
            Stack<T> DijkstraPath = new Stack<T>();
            T current = end, prev = default(T);
            Stack<T> temp_stack = new Stack<T>(((Stack<T>)(Node_stacks[NodeLookup[end]])));
            Stack<T> workingstack = new Stack<T>(temp_stack);
            do
            {

                DijkstraPath.Push(current); //Save data for use as a "road map"
                prev = current;
                current = workingstack.Pop();             
                

                string temp = current + " to " + prev + "\r\n";
                traceBackSteps.Push(temp);
            } while (NodeLookup[current] != NodeLookup[start]);
            DijkstraPath.Push(current);
            

            StringBuilder sb = new StringBuilder(30 * traceBackSteps.Count);
            while (traceBackSteps.Count > 0)
                sb.Append((string)traceBackSteps.Pop());

            Console.WriteLine(results + "\r\n\r\n" + sb.ToString());
            T k = end;
            //Reset the table so the next execution can use the updated edge weights
            foreach (FalconNode<T> SourceNode in Design_Graph.Nodes)
            {
                foreach (FalconNode<T> SinkNode in Design_Graph.Nodes)
                {
                    if (SourceNode.Neighbors.Contains(SinkNode))
                    {
                        ((Hashtable)(dist_table[NodeLookup[SourceNode.Value]])).Remove(NodeLookup[SinkNode.Value]);
                        ((Hashtable)(dist_table[NodeLookup[SourceNode.Value]])).Add(NodeLookup[SinkNode.Value], SourceNode.Costs[SourceNode.Neighbors.IndexOf(SinkNode)]);
                    }
                    else if (SourceNode == SinkNode)
                    {
                        ((Hashtable)(dist_table[NodeLookup[SourceNode.Value]])).Remove(SinkNode.Value);
                        ((Hashtable)(dist_table[NodeLookup[SourceNode.Value]])).Add(SinkNode.Value, (float)0);
                    }
                    else
                    {
                        ((Hashtable)(dist_table[NodeLookup[SourceNode.Value]])).Remove(SinkNode.Value);
                        ((Hashtable)(dist_table[NodeLookup[SourceNode.Value]])).Add(SinkNode.Value, Single.PositiveInfinity);
                    }
                }
            }
            //Reset the disttance table for the next operation of the algorithm
            foreach (Hashtable toremove in dist_table)
                toremove.Clear();
                
            foreach (FalconNode<T> SourceNode in Design_Graph.Nodes)
            {
                foreach (FalconNode<T> SinkNode in Design_Graph.Nodes)
                {

                    if (SourceNode.Neighbors.Contains(SinkNode))
                        ((Hashtable)(dist_table[NodeLookup[SourceNode.Value]])).Add(NodeLookup[SinkNode.Value], SourceNode.Costs[SourceNode.Neighbors.IndexOf(SinkNode)]);
                    else if (SourceNode == SinkNode)
                        ((Hashtable)(dist_table[NodeLookup[SourceNode.Value]])).Add(NodeLookup[SinkNode.Value], (float)0);
                    else
                        ((Hashtable)(dist_table[NodeLookup[SourceNode.Value]])).Add(NodeLookup[SinkNode.Value], Single.PositiveInfinity);
                }
            }
            return DijkstraPath;
        }

        /// <summary>
        /// Add a node to the graph
        /// </summary>
        /// <param name="_node">Node number</param>
        public void AddNode(T _node)
        {
            Design_Graph.AddNode(_node, FalconNodeNoPorts);
        }

        /// <summary>
        /// Adds a directed edge with cost to graph
        /// </summary>
        /// <param name="from">Origin Node</param>
        /// <param name="to">Destination Node</param>
        /// <param name="cost">Cost of edge</param>
        /// <param name="outport">Port that it is put out to</param>
        public void AddEdge(T from, T to, float cost, int outport)
        {
            Design_Graph.AddDirectedEdge(from, to, cost, outport);
        }
        
        /// <summary>
        /// Looks at the nodes of the graph
        /// </summary>
        /// <returns>Nodes of the graph</returns>
        public FalconNodeList<T> Nodes()
        {
            return Design_Graph.Nodes;
        }

        /// <summary>
        /// Finds the node with a given value
        /// </summary>
        /// <param name="index">Node's value</param>
        /// <returns>The node at the value</returns>
        public FalconNode<T> FalconNodeAt(int index)
        {
            return Design_Graph.FalconNodeAt(index);
        }

        /// <summary>
        /// Gets a node with a given value
        /// </summary>
        /// <param name="Val">Value</param>
        /// <returns>The node</returns>
        public FalconNode<T> GetFalconNodeByVal(T Val)
        {
            return Design_Graph.GetFalconNodeByVal(Val);
        }
        #region Inherited unimplimented method
        /*/// <summary>
        /// THIS METHOD IS NOT READY YET
        /// TODO: Implement display of pair of port and destination
        /// </summary>
        /// <param name="NodeIndex">NOT READY</param>
        public void NodePorts(int NodeIndex)
        {
            FalconNodeList<int> nodes = Design_Graph.Nodes;
            FalconNode<T> fnode = (FalconNode<T>)nodes.FindByValue(NodeIndex);
            List<int> port_list = fnode.Ports;
            foreach (int entry in port_list)
                Console.WriteLine(entry.ToString());
        }*/
        #endregion
        #region LegacyFunction
        //**This is now done by the constructor**//
        // <summary>
        // Initializes the distance and route tables used for Dijkstra's Algorithm.
        // </summary>
        // <param name="start">The <b>Key</b> to the source Node.</param>
        //public void InitRouteTables(int start)
        //{


            // set the initial distance and route for each node in the graph
            //foreach (int s_node in Design_Graph)
            //{
                //((Hashtable)(dist_table[start])).Add(s_node, Int32.MaxValue);
                //((Hashtable)(route_table[start])).Add(s_node, null);
            //}

            // set the initial distance of start to 0
            //((Hashtable)(dist_table[start])).Remove(start);
            //((Hashtable)(dist_table[start])).Add(start, (float)0);
        //}
        #endregion

        /// <summary>
        /// Retrieves the Node from the passed-in FalconNodeList that has the <i>smallest</i> value in the distance table.
        /// </summary>
        /// <remarks>This method of grabbing the smallest Node gives Dijkstra's Algorithm a running time of
        /// O(<i>n</i><sup>2</sup>), where <i>n</i> is the number of nodes in the graph.  A better approach is to
        /// use a <i>priority queue</i> data structure to store the nodes, rather than an array.  Using a priority queue
        /// will improve Dijkstra's running time to O(E lg <i>n</i>), where E is the number of edges.  This approach is
        /// preferred for sparse graphs.  For more information on this, consult the README included in the download.</remarks>
        private FalconNode<T> GetMin(FalconNodeList<T> nodes, T start)
        {
            // find the node in nodes with the smallest distance value
            
                float minDist = Single.PositiveInfinity;
                FalconNode<T> minNode = null;
                foreach (FalconNode<T> n in nodes)
                {
                    if (((float)(((Hashtable)(dist_table[NodeLookup[start]]))[NodeLookup[n.Value]])) <= minDist)
                        {
                            minDist = (float)(((Hashtable)(dist_table[NodeLookup[start]]))[NodeLookup[n.Value]]);
                            minNode = n;
                        }
                }
                T debug = minNode.Value;
                return minNode;
        }

        /// <summary>
        /// Relaxes the edge from the Node start to end.
        /// </summary>
        /// <param name="cost">The distance between two nodes.</param>
        /// <param name="CloseNode">The node that is being deleted</param>
        /// <param name="Neighbor">A neighbor of the deleted node</param>
        /// <param name="node_stacks">The route to get to the neighbor</param>
        /// <param name="start">The source node that all distances are measured from</param>
        private ArrayList Relax(T start, T CloseNode, T Neighbor, float cost, ArrayList node_stacks)
        {
            float distToStart = (float)(((Hashtable)(dist_table[NodeLookup[start]]))[NodeLookup[CloseNode]]);
            float distToThis = (float)(((Hashtable)(dist_table[NodeLookup[start]]))[NodeLookup[Neighbor]]);

            if (distToThis > distToStart + cost)
            {
                // update distance and route
                ((Stack<T>)(node_stacks[NodeLookup[Neighbor]])).Clear();
                Stack<T> temp = new Stack<T>();
                //((Stack<T>)node_stacks[CloseNode]).CopyTo(node_stacks[], 0);
                foreach (T value in ((Stack<T>)node_stacks[NodeLookup[CloseNode]]))
                    temp.Push(value);
                foreach (T value in temp)
                    ((Stack<T>)(node_stacks[NodeLookup[Neighbor]])).Push(value);

                ((Stack<T>)(node_stacks[NodeLookup[Neighbor]])).Push(CloseNode);
                
                ((Hashtable)(dist_table[NodeLookup[start]])).Remove(NodeLookup[Neighbor]);
                ((Hashtable)(dist_table[NodeLookup[start]])).Add(NodeLookup[Neighbor], distToStart + cost);
                
            }
            return node_stacks;
        }
        #region Port table related methods
        /*
        /// <summary>
        /// Generate the Port table for the Falcon node
        /// This function should be used only after both dest_table and route_table
        /// have been populated
        /// </summary>
        /// <param name="fnode">FalconNode that we want to generate its port table </param>
        public void GeneratePortTable(T fnode)
        {
            port_table[fnode] = new Hashtable();
            List<T> dest_list; // list of destinations associated to port

            // instantiate hash table
            for (int i = 0; i < FalconNodeNoPorts; i++)
                ((Hashtable)(port_table[fnode])).Add(i, null);

            for (int i = 0; i < NumOfNodes; i++)
            {
                if (i == fnode)
                    continue;
                int Destination = i;
                int Current_Destination = Destination;
                int Current_Route = (int)((Hashtable)(route_table[fnode]))[Current_Destination];
                while (Current_Route != fnode)
                {
                    Current_Destination = Current_Route;
                    Current_Route = (int)((Hashtable)(route_table[fnode]))[Current_Route];
                }

                FalconNodeList<int> neighbors = ((FalconNode<T>)((FalconNodeList<int>)Design_Graph.Nodes).FindByValue(fnode)).Neighbors;
                int Port_Index = neighbors.IndexOf((FalconNode<T>)neighbors.FindByValue(Current_Destination));
                int Port_Number = ((FalconNode<T>)((FalconNodeList<int>)Design_Graph.Nodes).FindByValue(fnode)).Ports[Port_Index];

                dest_list = new List<int>();

                if (((Hashtable)port_table[fnode])[Port_Number] != null)
                    dest_list = (List<int>)((Hashtable)port_table[fnode])[Port_Number];

                dest_list.Add(Destination);

                ((Hashtable)port_table[fnode])[Port_Number] = dest_list;
            }
        }
        
        /// <summary>
        /// Prints out the contents of the port table for all nodes
        /// </summary>
        public void PrintPortTable()
        {
            for (int i = 0; i < NumOfNodes; i++)
            {
                Console.WriteLine("Port Table for FalconNode " + i + ":");
                Hashtable ht = (Hashtable)port_table[i];
                for (int j = 0; j < FalconNodeNoPorts; j++)
                {
                    Console.WriteLine("Port (" + j + "):\t");
                    if (ht[j] == null || ht.Count == 0)
                    {
                        Console.WriteLine("-");
                    }
                    else
                    {
                        foreach (int node in ((List<int>)ht[j]))
                            Console.WriteLine(node + "\t");
                        Console.WriteLine("");
                    }
                }
                Console.WriteLine("");
            }
        }

        /// <summary>
        /// Prints out the port table for a node
        /// </summary>
        /// <param name="NodeNum">Node that we wih to display its port table</param>
        public void PrintPortTable(int NodeNum)
        {
            Console.WriteLine("Port Table for FalconNode " + NodeNum + ":");
            Hashtable ht = (Hashtable)port_table[NodeNum];
            for (int j = 0; j < FalconNodeNoPorts; j++)
            {
                Console.WriteLine("Port (" + j + "):\t");
                if (ht[j] == null || ht.Count == 0)
                {
                    Console.WriteLine("-");
                }
                else
                {
                    foreach (int node in ((List<int>)ht[j]))
                        Console.WriteLine(node + "\t");
                    Console.WriteLine("");
                }
            }
            Console.WriteLine("");
        }
        */
        #endregion
        #endregion

        #region IFalconLibrary Members

        string IFalconLibrary.FalconComponentName
        {
            get { return "Falcon Graph Builder"; }
        }

        string IFalconLibrary.FalconComponentVersion
        {
            get { return "Version 1.0.0"; }
        }

        string IFalconLibrary.GetFalconComponentVersion()
        {
            return "Falcon Graph Builder 1.0.0 Copyright (c) 2010 PennState";
        }

        #endregion
    }
}