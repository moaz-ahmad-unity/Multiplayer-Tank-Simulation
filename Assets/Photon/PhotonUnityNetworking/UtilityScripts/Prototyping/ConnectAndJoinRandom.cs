// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectAndJoinRandom.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities, 
// </copyright>
// <summary>
//  Simple component to call ConnectUsingSettings and to get into a PUN room easily.
// </summary>
// <remarks>
//  A custom inspector provides a button to connect in PlayMode, should AutoConnect be false.
//  </remarks>                                                                                               
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

using UnityEngine;
using UnityEngine.UI;
//using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>Simple component to call ConnectUsingSettings and to get into a PUN room easily.</summary>
    /// <remarks>A custom inspector provides a button to connect in PlayMode, should AutoConnect be false.</remarks>
    public class ConnectAndJoinRandom : MonoBehaviourPunCallbacks
    {
        /// <summary>Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.</summary>
        public bool AutoConnect = true;

        /// <summary>Used as PhotonNetwork.GameVersion.</summary>
        public byte Version = 1;

        /// <summary>Max number of players allowed in room. Once full, a new room will be created by the next connection attemping to join.</summary>
        [Tooltip("The max number of players allowed in room. Once full, a new room will be created by the next connection attemping to join.")]
        public byte MaxPlayers = 4;

        public int playerTTL = -1;
        //[SerializeField] GameObject roomItem;
        //[SerializeField] GameObject roomScrollView;
        [SerializeField] Text debugText;
        [SerializeField] Button JoinGame;
        [SerializeField] Button CreateGame;
        [SerializeField] Transform[] playerSpawnpoints;
        [SerializeField] GameObject UICamera;
        [SerializeField] GameObject Menu;
        [SerializeField] Button exitGame;

        bool NoInternet;
        public void Start()
        {
            exitGame.onClick.AddListener(Disconnect);

            JoinGame.onClick.AddListener(() =>
            {
                debugText.text = "Joining Game...";
                PhotonNetwork.JoinRandomRoom(null,0);
            });
            CreateGame.onClick.AddListener(CreateRoom);

            JoinGame.interactable = CreateGame.interactable = false;
            if (this.AutoConnect)
            {
                this.ConnectNow();
            }
        }

        public void ConnectNow()
        {
            Debug.Log("ConnectAndJoinRandom.ConnectNow() will now call: PhotonNetwork.ConnectUsingSettings().");
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                debugText.text = "Please check your internet...";
                NoInternet = true;
            }
            else
            {
                debugText.text = "Connecting...";
            }
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = this.Version + "." + SceneManagerHelper.ActiveSceneBuildIndex;
        }
        private void Update()
        {
            if (NoInternet)
            {
                if (Application.internetReachability != NetworkReachability.NotReachable)
                {
                    NoInternet = false;
                    ConnectNow();
                }
            }
        }

        // below, we implement some callbacks of the Photon Realtime API.
        // Being a MonoBehaviourPunCallbacks means, we can override the few methods which are needed here.


        public override void OnConnectedToMaster()
        {
            //Debug.Log("OnConnectedToMaster() was called by PUN. This client is now connected to Master Server in region [" + PhotonNetwork.CloudRegion +
            //    "] and can join a room. Calling: PhotonNetwork.JoinRandomRoom();");
            //PhotonNetwork.JoinRandomRoom();
            debugText.text = "Connected to Server.";
            JoinGame.interactable = CreateGame.interactable = true;
        }

        public override void OnJoinedLobby()
        {
            //Debug.Log("OnJoinedLobby(). This client is now connected to Relay in region [" + PhotonNetwork.CloudRegion + "]. This script now calls: PhotonNetwork.JoinRandomRoom();");
            //PhotonNetwork.JoinRandomRoom();
            //hotonNetwork.GetCustomRoomList();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available in region [" + PhotonNetwork.CloudRegion + "], so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
            debugText.text = "No Game Available... Host Game";
            //CreateRoom();
        }

        private void CreateRoom()
        {
            debugText.text = "Creating new Game...";

            RoomOptions roomOptions = new RoomOptions() { MaxPlayers = this.MaxPlayers , IsOpen=true, IsVisible=true};
            if (playerTTL >= 0)
                roomOptions.PlayerTtl = playerTTL;

            PhotonNetwork.CreateRoom(null, roomOptions, null);
        }



        public override void OnJoinedRoom()
        {
            debugText.text = "Spawning Player...";
            Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room in region [" + PhotonNetwork.CloudRegion + "]. Game is now running.");
            SpawnPlayer();
            UICamera.SetActive(false);
            Menu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        private void SpawnPlayer()
        {
            int actorNo = PhotonNetwork.PlayerList.Length;
            print("ActorNo: " + actorNo);
            PhotonNetwork.Instantiate("Cromwell_ST_Network", playerSpawnpoints[actorNo - 1].position, Quaternion.identity);
        }
        void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }
        // the following methods are implemented to give you some context. re-implement them as needed.
        public override void OnDisconnected(DisconnectCause cause)
        {
            SceneManager.LoadScene(0);
        }
    }


    //#if UNITY_EDITOR
    //[CanEditMultipleObjects]
    //[CustomEditor(typeof(ConnectAndJoinRandom), true)]
    //public class ConnectAndJoinRandomInspector : Editor
    //{
    //    void OnEnable() { EditorApplication.update += Update; }
    //    void OnDisable() { EditorApplication.update -= Update; }

    //    bool isConnectedCache = false;

    //    void Update()
    //    {
    //        if (this.isConnectedCache != PhotonNetwork.IsConnected)
    //        {
    //            this.Repaint();
    //        }
    //    }

    //    public override void OnInspectorGUI()
    //    {
    //        this.isConnectedCache = !PhotonNetwork.IsConnected;


    //        this.DrawDefaultInspector(); // Draw the normal inspector

    //        if (Application.isPlaying && !PhotonNetwork.IsConnected)
    //        {
    //            if (GUILayout.Button("Connect"))
    //            {
    //                ((ConnectAndJoinRandom)this.target).ConnectNow();
    //            }
    //        }
    //    }
    //}
    //#endif
}
