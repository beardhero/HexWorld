using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using GameDevWare.Serialization;
using GameDevWare.Serialization.MessagePack;

using UnityEngine;

namespace Colyseus
{
	public class RoomAvailable {
        public string roomId { get; set; }
        public uint clients { get; set; }
        public uint maxClients { get; set; }
        public object metadata { get; set; }
	}

	/// <summary>
	/// </summary>
	public class Room : StateContainer
	{
		public string id;
		public string name;
		public string sessionId;

		public Dictionary<string, object> options;

		protected Connection connection;
		protected byte[] _previousState = null;

		public event EventHandler OnReadyToConnect;
		public event EventHandler OnJoin;
		public event EventHandler<ErrorEventArgs> OnError;
		public event EventHandler OnLeave;
		public event EventHandler<MessageEventArgs> OnMessage;
		public event EventHandler<RoomUpdateEventArgs> OnStateChange;

		public ColyseusClient colyseusClient;

		public Room() : base(new IndexedDictionary<string, object>()){}

		public void Initialize(String name, Dictionary<string, object> options = null){
			this.name = name;
			this.options = options;
		}

		public void Recv ()
		{
			byte[] data = this.connection.Recv();
			if (data != null)
			{
				this.ParseMessage(data);
			}
		}

		public IEnumerator Connect (ColyseusClient c)
		{
			colyseusClient = c;
			return this.connection.Connect ();
		}

		public void SetConnection (Connection connection)
		{
			this.connection = connection;

			this.connection.OnClose += (object sender, EventArgs e) => {
				if (this.OnLeave != null) {
					this.OnLeave.Invoke (this, e);
				}
			};

			this.connection.OnError += (object sender, ErrorEventArgs e) => {
				if (this.OnError != null) {
					this.OnError.Invoke(this, e);
				}
			};

			this.OnReadyToConnect.Invoke (this, new EventArgs());
		}

		public void SetState( byte[] encodedState, uint remoteCurrentTime, uint remoteElapsedTime)
		{
			// Deserialize
			var state = MsgPack.Deserialize<IndexedDictionary<string, object>> (new MemoryStream(encodedState));

			this.Set(state);

			if (this.OnStateChange != null) {
				this.OnStateChange.Invoke (this, new RoomUpdateEventArgs (state, true));
			}

			this._previousState = encodedState;
		}

		/// <summary>
		/// Leave the room.
		/// </summary>
		public void Leave ()
		{
			if (this.id != null) {
				this.connection.Send(new object[]{Protocol.LEAVE_ROOM});

			} else {
				this.OnLeave.Invoke (this, new EventArgs ());
			}
		}

		/// <summary>
		/// Send data to this room.
		/// </summary>
		/// <param name="data">Data to be sent</param>
		public void Send (object data)
		{
			this.connection.Send(new object[]{Protocol.ROOM_DATA, this.id, data});
		}

		protected void ParseMessage (byte[] recv)
		{
			var message = MsgPack.Deserialize<List<object>> (new MemoryStream(recv));
			var code = (byte) message [0];

			if (code == Protocol.JOIN_ROOM) {
				this.sessionId = (string) message [1];

				if (this.OnJoin != null) {
					this.OnJoin.Invoke (this, new EventArgs ());
				}

			} else if (code == Protocol.JOIN_ERROR) {
				this.OnError.Invoke (this, new ErrorEventArgs ((string) message [1]));

			} else if (code == Protocol.LEAVE_ROOM) {
				this.Leave ();

			} else if (code == Protocol.ROOM_STATE) {
				byte[] encodedState = (byte[]) message [1];

				// TODO:
				// https://github.com/deniszykov/msgpack-unity3d/issues/8

				// var remoteCurrentTime = (double) message [2];
				// var remoteElapsedTime = (int) message [3];

				// this.SetState (state, remoteCurrentTime, remoteElapsedTime);

				this.SetState (encodedState, 0, 0);

			} else if (code == Protocol.ROOM_STATE_PATCH) {

				var data = (List<object>) message [1];
				byte[] patches = new byte[data.Count];

				uint i = 0;
				foreach (var b in data) {
					patches [i] = Convert.ToByte(b);
					i++;
				}

				this.Patch (patches);

			} else if (code == Protocol.ROOM_DATA) {
				if (this.OnMessage != null) {
					this.OnMessage.Invoke (this, new MessageEventArgs (message [1]));
				}
			}
		}

		protected void Patch (byte[] delta)
		{
			this._previousState = Fossil.Delta.Apply (this._previousState, delta);

			var newState = MsgPack.Deserialize<IndexedDictionary<string, object>> (new MemoryStream(this._previousState));

			this.Set(newState);

			if (this.OnStateChange != null)
				this.OnStateChange.Invoke(this, new RoomUpdateEventArgs(this.state));
		}
	}
}
