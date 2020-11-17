using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public UdpClient udpClient = new UdpClient();
    public IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);//server endpoint

    public bool isGameStarted = false;
    public static ConnectionManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void MakeConnection()
    {
        udpClient.Connect(serverEndPoint);
    }


    public void SendDirection(Vector2 direction)
    {
        Debug.Log(direction);
        List<byte> message = new List<byte>();
        message.AddRange(BitConverter.GetBytes(direction.x));
        message.AddRange(BitConverter.GetBytes(direction.y));
        udpClient.Send(message.ToArray(), message.Count);
    }


    public void SendUsername(string username)
    {
        udpClient.Send(Encoding.ASCII.GetBytes(username), username.Length);
    }

    public int ReceiveId()
    {
        // then receive data
        var receivedData = udpClient.Receive(ref serverEndPoint);

        Debug.Log("receive data from " + serverEndPoint.ToString());
        int id = 0;
        if (receivedData.Length > 3)
        {
            // If there are unread bytes
            id = BitConverter.ToInt32(receivedData, 0); // Convert the bytes to an int
        }
        else
        {
            throw new Exception("Could not read value of type 'int'!");
        }

        Debug.Log(id);
        return id;
    }

    public void ReceiveAndManagePacket()
    {
        int packetNumber;
        byte[] receivedData = ReceivePacket(out packetNumber);
        while (packetNumber != 0) 
        { 
            if (packetNumber == 1) 
            {
                Vector2 foodPosition = ParsePosition(receivedData);
                FoodManager.instance.InstantiateFood(foodPosition);
            }
            else
            {
                Debug.Log("Om-nom-nom");
            }
            receivedData = ReceivePacket(out packetNumber);
        }
        var position = ParsePosition(receivedData);
        UpdatePlayerPosition(position);
    }

    private byte[] ReceivePacket(out int packetNumber)
    {
        var receivedData = udpClient.Receive(ref serverEndPoint);
        Debug.Log("receive data from " + serverEndPoint.ToString());
        packetNumber = BitConverter.ToInt32(receivedData, 0);
        return receivedData;
    }

    private Vector2 ParsePosition(byte[] receivedData)
    {
        Vector2 position;
        position.x = BitConverter.ToSingle(receivedData, 4);
        position.y = BitConverter.ToSingle(receivedData, 8);
        return position;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameStarted)
        {
            SendCameraDirection();
            ReceiveAndManagePacket();
        }
    }

    private void UpdatePlayerPosition(Vector2 newPosition)
    {
        PlayerManager.instance.transform.position = new Vector3(newPosition.x, newPosition.y, 0);
    }

    private void SendCameraDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - PlayerManager.instance.transform.position).normalized;
        //var velocity = new Vector2(direction.x * speed, direction.y * speed);
        //transform.position += new Vector3(0.1f * velocity.x, 0.1f * velocity.y, 0);
        SendDirection(direction);
    }
}
