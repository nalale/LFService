using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DEService;

namespace MFService.Diag
{
	public class A_Diag
	{
		N_Diag nDiag;
		byte[] cfg;

		List<uint> availableNodes = new List<uint>();
		List<byte> buf;
		List<ResponseData_ReadDataByIdentifier> Response_IDs;

		object locker;

		volatile AutoResetEvent waitRx;
		volatile AutoResetEvent waitTx;		

		public A_Diag(ICanDriver can)
		{
			cfg = new byte[256];
			buf = new List<byte>();			
			Response_IDs = new List<ResponseData_ReadDataByIdentifier>();

			nDiag = new N_Diag(can);
			
			locker = new object();

			waitRx = new AutoResetEvent(false);
			waitTx = new AutoResetEvent(false);
		}

		void OnRxComplete(N_Message nmsg)
		{
			if (nmsg.MsgType == N_Types.ecan_profile_fun_read)
			{
                if (nmsg.Result == N_Res.END_OF_COMMUNICATION)
                {
                    buf.Clear();
                    for (int i = 0; i < nmsg.data.Length; i++)
                    {
                        //cfg[i] = nmsg.data[i];
                        buf.Add(nmsg.data[i]);
                    }

                    waitRx.Set();
                }
                else if(nmsg.Result == N_Res.CRC_MISTMATCH_ERROR)
                {
                    buf.Clear();

                    waitRx.Set();
                }
			}
			else if (nmsg.MsgType == N_Types.ecan_diag_value_read && nmsg.Result == N_Res.END_OF_COMMUNICATION)
			{
				int index = Response_IDs.FindIndex((req) => req.DID == nmsg.Index);
                if (index < 0)
                {
                    return;
                }

				Response_IDs[index].DV.Clear();
				for (int i = 0; i < nmsg.MsgNum * 4; i++)
				{
					Response_IDs[index].DV.Add(nmsg.data[i]);
				}

				waitRx.Set();
			}
						
		}

		// Прием одиночных данный без запроса, например Diag ID
		void OnRxDiagIdComplete(N_Message nmsg)
		{
			//if (cnt == 0)
			//{
			//	//Array.Clear(cfg, 0, cfg.Length);
			//	buf.Clear();
			//}

			//cfg[cnt] = (byte)nmsg.TA;
			//cnt = (cnt >= cfg.Length - 1) ? 0 : cnt + 1;

			//if (!buf.Contains((byte)nmsg.TA))
			//	buf.Add((byte)nmsg.TA);			
		}

		void OnTxComplete(N_Message nmsg)
		{
			waitTx.Set();
		}

		public Task<byte[]> ReadDataByID(byte Address, byte ID)
		{
			nDiag.OnRxComplete += OnRxComplete;

			nDiag.Send(Address, N_Types.ecan_profile_fun_read, null, ID);
			
			return Task.Run(() =>
			{
				waitRx.WaitOne();
				nDiag.OnRxComplete -= OnRxComplete;

                return buf.ToArray();// cfg;
			});
		}

		public Task<bool> WriteDataByID(byte Address, byte ID, byte[] buf)
		{
			nDiag.OnTxComplete += OnTxComplete;

			nDiag.Send(Address, N_Types.ecan_profile_fun_write, buf, ID);

			return Task.Run(() =>
			{                
				bool res = waitTx.WaitOne(5000);
				nDiag.OnTxComplete -= OnTxComplete;
				return res;
			});
		}

		public async Task<List<ResponseData_ReadDataByIdentifier>> ReadDataByIDs(byte Address, ICollection<int> ID)
		{
			Response_IDs.Clear();
			nDiag.OnRxComplete += OnRxComplete;

			return await Task.Run(() =>
			{                
                foreach (byte id in ID)
				{
					if (!Response_IDs.Exists((req) => req.DID == id))
						Response_IDs.Add(new ResponseData_ReadDataByIdentifier(id));
                    
					nDiag.Send(Address, N_Types.ecan_diag_value_read, null, id);
                    Thread.Sleep(50);
                    waitRx.WaitOne(2000);
                }

                
                nDiag.OnRxComplete -= OnRxComplete;
				return Response_IDs;
			});
		}
		public IEnumerable<uint> ReadDiagID()
		{
			availableNodes.Clear();
			availableNodes.AddRange(nDiag.GetOnlineIds());

			return availableNodes;

			//lstIds.Clear();

			//nDiag.OnRxIDsComplete += OnRxDiagIdComplete;

			//return Task.Run(() =>
			//{
			//	waitRx.WaitOne(1000);
			//	nDiag.OnRxIDsComplete -= OnRxDiagIdComplete;
			//	return lstIds.ToList<byte>();
			//});
		}



	}

	public class A_Message
	{
		public A_Message()
		{

		}

		#region Свойства

		
		public short Address;
		public short[] Data;

		#endregion


		public void Set(N_Message nmsg)
		{
			Address = nmsg.TA;

			Data = new short[nmsg.DataLength];
			for (int i = 0; i < nmsg.DataLength; i++)
				Data[i] = nmsg.data[i];
		}
	}

	interface A_Service
	{
		A_ServiceState State { get; set; }

	}

	enum A_ServiceState
	{
		OK = 0,
		Timeout = 1,
	}


	public class A_Service_SearchOnlineNodes
	{
		
	}

    public class A_Service_ReadDataByIdentifier
    {
        public A_Service_ReadDataByIdentifier()
        {

            reqRespPair = new ConcurrentDictionary<int, ResponseData_ReadDataByIdentifier>();
        }

        public async Task<bool> RequestService(int EcuAddress)
        {
            // Запрос диагностических значений
            var result = await Global.diag.ReadDataByIDs((byte)EcuAddress, RequestedDIDs);

            if (result.Count > 0)
            {
                foreach (var r in result)
                    addResponse(r);

                return true;
            }
            else
                return false;
        }

        public void AddRequestedDID(uint Did)
        {
            reqRespPair.TryAdd((byte)Did, new ResponseData_ReadDataByIdentifier((short)Did));
        }

        public void RemoveRequestedDID(uint Did)
        {
            ResponseData_ReadDataByIdentifier resp;
            reqRespPair.TryRemove((byte)Did, out resp);
        }

        public List<int> GetResponseValue(int Did)
        {
            ResponseData_ReadDataByIdentifier response;
            List<int> result = new List<int>();

            if (reqRespPair.TryGetValue((byte)Did, out response))
            {
                for (int i = 0; i < response.DV.Count / 4; i++)
                    result.Add(BitConverter.ToInt32(response.DV.ToArray(), i * 4));

                return result;
            }
            else
                return null;
        }

        public ICollection<int> RequestedDIDs
        {
            get { return reqRespPair.Keys; }
            private set {; }
        }

        private ConcurrentDictionary<int, ResponseData_ReadDataByIdentifier> reqRespPair;
        private bool addResponse(ResponseData_ReadDataByIdentifier Response)
        {
            if (Response == null || !reqRespPair.ContainsKey(Response.DID))
                return false;

            reqRespPair.TryUpdate(Response.DID, Response, reqRespPair[(byte)Response.DID]);
            return true;
        }
    }

	//TODO: сделать закрытым членом A_Service_ReadDataByIdentifier
	public class ResponseData_ReadDataByIdentifier
    {
        public short DID;
        public List<byte> DV;
        public byte Result { get; set; }

        public ResponseData_ReadDataByIdentifier(short did)
        {
            DID = did;
            DV = new List<byte>();
        }
    }
}
