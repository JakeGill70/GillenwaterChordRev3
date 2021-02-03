using System;
using System.Collections;
using System.Threading.Tasks;
using GillenwaterChordRev3.Messages;

namespace GillenwaterChordRev3
{
    class Program
    {
        LocalNode localNode;

        static void Main()
        {
            Program app = new Program();

            // Determine the port this node will operate on
            int localServerPort = 5000 + EZRandom.Next(0, 999);
            OutputManager.Ui.Write("Node Server Port: " + localServerPort);
            app.localNode = new LocalNode(localServerPort);

            bool appIsRunning = true;

            // Communicate with the remote node
            while (appIsRunning)
            {
                // Prompt the user for test data
                OutputManager.Ui.Write("What to do?");
                string cmd = Console.ReadLine();

                switch (cmd.ToLower()) {
                    case "exit":
                        app.ProcExit();
                        appIsRunning = false;
                        break;
                    case "info":
                        app.ProcInfo();
                        break;
                    case "join":
                        app.ProcJoin();
                        break;
                    case "add":
                        app.ProcAddResource();
                        break;
                    case "storage":
                        app.ProcStorage();
                        break;
                    case "get":
                        app.ProcGet();
                        break;
                }
            }

            // Disconnect from the remote node before closing
            app.localNode.DisconnectFromNode();
        }

        void ProcInfo() {
            OutputManager.Ui.Write("Self port: " + localNode.Port);
            OutputManager.Ui.Write("Pred port: " + localNode.predNode.Port);
            OutputManager.Ui.Write("Succ port: " + localNode.succNode.Port);
        }

        void ProcConnect() {
            // Determine where this node should connect to
            OutputManager.Ui.Write("Node to connect to: ");
            string nodeAddr = "127.0.0.1";
            OutputManager.Ui.Write(nodeAddr);

            // Determine what port this node should connect to
            OutputManager.Ui.Write("Port to connect to: ");
            int nodePort = int.Parse(Console.ReadLine());

            // Connect to the remote node
            OutputManager.Ui.Write("Connecting to remote node...");
            localNode.ConnectToNode(nodeAddr, nodePort);
            OutputManager.Ui.Write("Connected");
        }

        void ProcJoin() {
            // Connect to arbitrary node in the ring
            ProcConnect();
            // Make a join request
            OutputManager.Ui.Write("Join Request...");
            JoinRequest joinRequest = new JoinRequest(this.localNode);
            Message joinResponseTmp = localNode.SendMessage(joinRequest);
            JoinResponse joinResponse = new JoinResponse(joinResponseTmp.ToString());
            // Disconnect from the arbitary node in the ring
            OutputManager.Ui.Write("Disconnecting from node...");
            localNode.DisconnectFromNode();
            // Update local node connection info
            OutputManager.Ui.Write("Updating local info...");
            localNode.predNode = new ChordNode(joinResponse.predIpAddress, joinResponse.predPort, joinResponse.predId);
            localNode.succNode = new ChordNode(joinResponse.succIpAddress, joinResponse.succPort, joinResponse.succId);
            // Connect to pred
            OutputManager.Ui.Write("Connecting to pred...");
            localNode.ConnectToNode(localNode.predNode.IpAddress, localNode.predNode.Port);
            // Have pred update their succ
            OutputManager.Ui.Write("Having pred update their succ...");
            UpdateSuccNodeRequest uSuccRequest = new UpdateSuccNodeRequest(this.localNode, this.localNode);
            Message uSuccResponseTmp = localNode.SendMessage(uSuccRequest);
            UpdateNodeResponse uSuccResponse = new UpdateNodeResponse(uSuccResponseTmp.ToString());
            // TODO: Ensure that update was successful
            // Disconnect from pred
            OutputManager.Ui.Write("Disconnecting from pred...");
            localNode.DisconnectFromNode();
            // Connect to succ
            OutputManager.Ui.Write("Connecting to succ...");
            localNode.ConnectToNode(localNode.succNode.IpAddress, localNode.succNode.Port);
            // Have succ update their pred
            OutputManager.Ui.Write("Having succ update their pred...");
            UpdatePredNodeRequest uPredRequest = new UpdatePredNodeRequest(this.localNode, this.localNode);
            UpdateNodeResponse uPredResponse = (localNode.SendMessage(uPredRequest)) as UpdateNodeResponse;
            // TODO: Ensure that update was successful
            OutputManager.Ui.Write("Successfully joined to ring!");
            // TODO: Get resources that should now belong to this node from succ node
        }

        void ProcAddResource() {
            OutputManager.Ui.Write("What is the name of this new resouce?");
            string resourceName = Console.ReadLine();
            OutputManager.Ui.Write("What is the contents of this new resouce?");
            string resourceContent = Console.ReadLine();
            string resourceId = EZHash.GetHashString(resourceName);
            Messages.AddResourceRequest addResourceReq = new AddResourceRequest(this.localNode, resourceId, resourceName, resourceContent);
            Messages.Message tmpResponse = localNode.SendMessage(addResourceReq);
            Messages.AddResourceResponse addResourceResponse = new AddResourceResponse(tmpResponse.ToString());
            if (addResourceResponse.resourceAddedSuccessfully)
            {
                OutputManager.Ui.Write($"The resource \"{resourceName}\" was added successfully.");
            }
            else {
                OutputManager.Ui.Write($"Failed to add the resource \"{resourceName}\"");
            }
        }

        void ProcStorage() {
            string[] resourcesInLocalStorage = this.localNode.GetLocalResourceNames();
            if (resourcesInLocalStorage.Length == 0)
            {
                OutputManager.Ui.Write("No resources are stored on this node.");
            }
            else {
                for (int i = 0; i < resourcesInLocalStorage.Length; i++)
                {
                    OutputManager.Ui.Write($"\t{i+1}. {resourcesInLocalStorage[i]}");
                }
            }
        }

        void ProcGet() {
            OutputManager.Ui.Write("What is the name the resource to get?");
            string resourceName = Console.ReadLine();
            string resourceId = EZHash.GetHashString(resourceName);
            Messages.GetResourceRequest grr = new GetResourceRequest(this.localNode, resourceId);
            Messages.Message tmpResponse = this.localNode.SendMessage(grr);
            Messages.GetResourceResponse rep = new GetResourceResponse(tmpResponse.ToString());
            if (rep.resourceFoundSuccessfully)
            {
                OutputManager.Ui.Write(rep.resourceContent);
            }
            else {
                OutputManager.Ui.Write("A resource with that name could not be found.");
            }
        }

        void ProcExit() {
            // display message showing that exit process has started
            // Update current pred's succ to current succ
            // Update current succ's pred to current pred
            // Foreach resource in local resources:
            //      Add resource to current succ
            //      display message showing status of exit process
            // disconnect from pred
            // disconnect from succ
            // display message showing that exit process has concluded
            FinalMessageQueue<FinalMessage> fmq = new FinalMessageQueue<FinalMessage>();
        }
    }
}

public interface IFinalMessageQueue<IComparable> : System.Collections.ICollection {
    // To begin, because nothing is known about the Message class, I can see 
    // no reason to restrict this class to only work with the Message class. 
    // Instead, I have set up this queue to work with any class that implements 
    // at least IComparable, for reasons explained below.

    // I am assuming that the queue should really be a priority queue decided
    // by comparing some internals of the message class. From that, I concluded 
    // that the Message class must implement at least IComparable. This is 
    // necessary for this class to compare message to determine their relative 
    // priority within the queue.

    // Next, notice that this interface inherits from ICollection. This forces 
    // the implementation of most commonly used collection methods and properties. 
    // As well as some that assist with data syschronization. More information 
    // about this interface and what it requires can be found on the microsoft 
    // documentation website, here:
    // https://docs.microsoft.com/en-us/dotnet/api/system.collections.icollection?view=net-5.0#remarks

    // This is a blocking method for retrieving the first item from a queue.
    // This method returns the next item from the queue and removes it.
    // This method was named "get" by Maarten van Steen and Andrew Tanenbaum in
    // their book "Distributed Systems". I felt Dequeue was a more appropriate.
    public IComparable Dequeue();

    // Notice that each of the following asynchronous methods should be 
    // implemented with the "async" keyword, as denoted by convention in their 
    // name. In C#, you cannot force a method to be implemented with the "async" 
    // keyword because the keyword can only be added to method headers
    // with an associated method body which does not exists in an interface.

    // This is a asynchonrous method for adding messages to a queue.
    // This method was named "put" by Maarten van Steen and Andrew Tanenbaum in
    // their book "Distributed Systems". I felt EnqueueAsync was a more appropriate.
    public Task EnqueueAsync(IComparable obj);

    // This is a asynchonrous method for retrieving the first item from a queue.
    // This method returns the next item from the queue and removes it.
    // This method was named "poll" by Maarten van Steen and Andrew Tanenbaum in
    // their book "Distributed Systems". I felt DequeueAsync was a more appropriate.
    public Task<IComparable> DequeueAsync(IComparable obj);

    // This is an event for whenever a new message is added to the queue.
    // This event is necessary for a publish-subscribe style design.
    public event EventHandler Notify;
}


public class FinalMessageQueue<IComparable> : IFinalMessageQueue<IComparable>
{
    
}

public class FinalMessage : IComparable<FinalMessage>
{
    public int CompareTo(FinalMessage obj)
    {
        throw new NotImplementedException();
    }
}