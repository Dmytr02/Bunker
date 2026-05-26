    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Photon.Pun;
    using TMPro;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class BunkerStats : MonoBehaviourPunCallbacks
    {
        #region Fields

        

        private int _Size = 0;

        public int Size
        {
            get { return _Size; }
            set { _Size = value; UpdateStats(); }
        }
        private int _TimeInside = 0;

        public int TimeInside
        {
            get { return _TimeInside; }
            set { _TimeInside = value; UpdateStats(); }
        }
        private Supplies _Supplies = 0;

        public Supplies Supplies
        {
            get { return _Supplies; }
            set { _Supplies = value; UpdateStats(); }
        }
        [SerializeField] private List<TMP_Text> textList = new List<TMP_Text>();
     
        public static BunkerStats Instance;
        #endregion

        private void Awake()
        {
            if(Instance) Destroy(this);
            else Instance = this;
        }

        private void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _Size = Random.Range(8, 200);
                _TimeInside = Random.Range(1, 50);
                _Supplies = (Supplies)Random.Range(0, 4);
                UpdateStats();
            }
        }

        public static object GetRandomStat(string stat)
        {
            switch (stat)
            {
                case "Size":
                    return Random.Range(8, 200);
                case "TimeInside":
                    return Random.Range(1, 50);
                case "Supplies":
                    return (Supplies)Random.Range(0, 4);
            }
            return null;
        }

        public bool SetStat(string stat, object value)
        {
            switch (stat)
            {
                case "Size":
                    if(value is int) {Size = (int)value; return true;}
                    break;
                case "TimeInside":
                    if(value is int) {TimeInside = (int)value; return true;}
                    break;
                case "Supplies":
                    if(value is Supplies) {Supplies = (Supplies)value; return true;}
                    break;
            }
            return false;
        }
        
        private async void UpdateStats()
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, nameof(UpdateStats_RPC));
            UpdateStats_RPC(Size, TimeInside, (int)Supplies);
            await Task.Delay(100);
            photonView.RPC(nameof(UpdateStats_RPC), RpcTarget.AllBuffered, Size, TimeInside, Supplies);
            //photonView.RpcSecure(nameof(UpdateStats_RPC), RpcTarget.AllBuffered, true, Size, TimeInside, (int)Supplies);
        }

        [PunRPC]
        private void UpdateStats_RPC(int size, int timeInside, int supplies)
        {
            _Size = size;
            _TimeInside = timeInside;
            _Supplies = (Supplies)supplies;
            foreach (var text in textList)
            {
                text.text = $"<sprite=0>Size: {_Size} m²\n<sprite=1>TimeInside: {_TimeInside} years\n<sprite=2>Supplies: {_Supplies}\n<sprite=3>Capacity: 2";
            }
        }
    }

    public enum Supplies
    {
        Full,
        Stable,
        Low,
        Critical
    }


