using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;  
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;

// Controls Client connection with the input server
namespace ClientController {
    public class Client {


    public int port;
    public string serverIPAddress;

    public static UdpClient client;

    byte[] exitSignal = Encoding.ASCII.GetBytes("exit");

    public IPEndPoint remoteEndPoint;

    public Client(int _port){
        port = _port;
        client = new UdpClient(port-1);

        // First Connection to open the port
        try {
            client.Connect("127.0.0.1", port);
            remoteEndPoint = new IPEndPoint(IPAddress.Any, port);

            byte[] sendBytes = Encoding.ASCII.GetBytes("Connection Message");
            client.Send(sendBytes, sendBytes.Length);

            

        }
        catch(Exception e)
        {
            Debug.Log("Exception thrown " + e.Message);
        }

    }

    // Initialize game client
    public void init() {
        try {
            sbyte[] receiveBytes = getKeyboardInput();
            while (!(receiveBytes[4] == 1)) {
                Debug.Log("Waiting Game Begin");
                receiveBytes = getKeyboardInput();
                
            }

        }
        catch(Exception e)
        {
            Debug.Log("Exception thrown " + e.Message);
        }
    }

    void sendExitSignal(){
        client.Send(exitSignal, exitSignal.Length);

    }

    public void sendMessage(byte[] message){
        client.Send(message, message.Length);

    }

    public byte[] receiveMessage(){
        byte[] receiveBytes = client.Receive(ref remoteEndPoint);
        return receiveBytes;

    }

    public sbyte[] getKeyboardInput(){


        sbyte[] receiveBytes = Array.ConvertAll(client.Receive(ref remoteEndPoint), b => unchecked((sbyte)b));

        return receiveBytes;
        

    }

}

}


