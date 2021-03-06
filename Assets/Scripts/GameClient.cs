using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine.UI;

public enum ControlMsg { forward, backward, left, right, light }

public class GameClient : MonoBehaviour, INetEventListener
{
    
    public RawImage clientRenderTex;
    Texture2D clientTex;

    private NetManager _netClient;
    bool imageInitialized;
    //image chunks are written to this while we go
    public static byte[] buffer;

    public void Init ()
    {
        _netClient = new NetManager(this, "lego_eXplorer");
	    _netClient.Start();
	    _netClient.UpdateTime = 15;
        print("client initialized");
    }

    public void Shutdown()
    {
        if (_netClient != null)
            _netClient.Stop();
    }

    public void Update ()
    {
        if (_netClient == null) return;

	    _netClient.PollEvents();

        var peer = _netClient.GetFirstPeer();
        if (peer != null && peer.ConnectionState == ConnectionState.Connected && clientTex!=null)
        {
            clientTex.LoadRawTextureData(buffer);
            clientTex.Apply();
        }
        else
        {
            _netClient.SendDiscoveryRequest(new byte[] { 1 }, 5000);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) OnRemoteControlBtnDown((int)ControlMsg.left);
        if (Input.GetKeyUp(KeyCode.LeftArrow)) OnRemoteControlBtnUp((int)ControlMsg.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) OnRemoteControlBtnDown((int)ControlMsg.right);
        if (Input.GetKeyUp(KeyCode.RightArrow)) OnRemoteControlBtnUp((int)ControlMsg.right);

        if (Input.GetKeyDown(KeyCode.UpArrow)) OnRemoteControlBtnDown((int)ControlMsg.forward);
        if (Input.GetKeyUp(KeyCode.UpArrow)) OnRemoteControlBtnUp((int)ControlMsg.forward);
        if (Input.GetKeyDown(KeyCode.DownArrow)) OnRemoteControlBtnDown((int)ControlMsg.backward);
        if (Input.GetKeyUp(KeyCode.DownArrow)) OnRemoteControlBtnUp((int)ControlMsg.backward);


    }

    NetDataWriter cmdWriter = new NetDataWriter(); 
    public void OnRemoteControlBtnDown(int parameter) //param is casted to ControlMs on the other side of connection
    {
        ControlMsg msg = (ControlMsg)parameter;
        var peer = _netClient.GetFirstPeer();
        if (peer == null) return;
        cmdWriter.Reset();
        cmdWriter.Put(parameter);
        int on = 1;
        cmdWriter.Put(on);
        peer.Send(cmdWriter, SendOptions.ReliableOrdered);             // Send with reliability
    }

    public void OnRemoteControlBtnUp(int parameter) //param is casted to ControlMs on the other side of connection
    {
        ControlMsg msg = (ControlMsg)parameter;
        var peer = _netClient.GetFirstPeer();
        if (peer == null) return;
        cmdWriter.Reset();
        cmdWriter.Put(parameter);
        int off = 0;
        cmdWriter.Put(off);
        peer.Send(cmdWriter, SendOptions.ReliableOrdered);             // Send with reliability
    }

    public void OnToggleFlashLightBtnClick()
    {
        ControlMsg msg = ControlMsg.light;
        var peer = _netClient.GetFirstPeer();
        if (peer == null) return;
        cmdWriter.Reset();
        cmdWriter.Put((int)msg);        
        cmdWriter.Put(1);
        peer.Send(cmdWriter, SendOptions.ReliableOrdered);             // Send with reliability
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[CLIENT] We connected to " + peer.EndPoint);
    }

    public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
    {
        Debug.Log("[CLIENT] We received error " + socketErrorCode);
    }

    
    public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
    {
        if (!imageInitialized)
        {
            string s = reader.GetString(32);
            if (s != string.Empty)
            {
                print("received: " + s);
                int w = 0;
                if (int.TryParse(Utils.EatString(ref s), out w))
                {
                    int h;
                    if (int.TryParse(Utils.EatString(ref s), out h))
                    {
                        clientTex = new Texture2D(w, h, StreamingManager.Inst.textureFormat, false);
                        buffer = new byte[w * h * Utils.SomeTextureFormatsToBytes(StreamingManager.Inst.textureFormat)];//where 16 is fixed render texture bit depth
                        clientRenderTex.texture = clientTex;
                    }    
                }
                imageInitialized = true;                
            }
            return;
        }
        //partially fill buffer at given index 
        int start = reader.GetInt();        
        CompressionMode compMode = (CompressionMode)reader.GetByte();
        byte[] payload = reader.GetRemainingBytes();
        if(compMode!=CompressionMode.none) payload = Compressor.UnPack(payload, compMode);
        System.Array.Copy(payload, 0, buffer, start, payload.Length);        
    }

    public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.DiscoveryResponse && _netClient.PeersCount == 0)
        {
            Debug.Log("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
            _netClient.Connect(remoteEndPoint);
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[CLIENT] We disconnected because " + disconnectInfo.Reason);
    }
}
