using UnityEngine;

public class TestSocket : MonoBehaviour
{
    SocketClient mSocket;

    // Use this for initialization
    void Start()
    {
        mSocket = new SocketClient();
        mSocket.Connect("127.0.0.1", 8088);
        mSocket.SendMessage("client send msg to server!");

    }

    void OnApplicationQuit()
    {
        mSocket.Disconnect();
    }
}
