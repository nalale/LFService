using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DEService;

namespace BoadService.Diag
{
	public class A_Diag
	{
		N_Diag nDiag;
		byte[] cfg;

		List<byte> buf;
		List<ResponseData_ReadDataByIdentifier> Response_IDs;

		object locker;

		volatile AutoResetEvent waitRx;
		volatile AutoResetEvent waitTx;

		int cnt = 0;

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

			if (!buf.Contains((byte)nmsg.TA))
				buf.Add((byte)nmsg.TA);
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

		public async Task<List<ResponseData_ReadDataByIdentifier>> ReadDataByIDs(byte Address, byte[] ID)
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

		public Task<List<byte>> ReadDiagID()
		{
            buf.Clear();

            nDiag.OnRxIDsComplete += OnRxDiagIdComplete;			

			return Task.Run(() =>
			{			
				waitRx.WaitOne(1000);
				nDiag.OnRxIDsComplete -= OnRxDiagIdComplete;
				return buf;
			});
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
}
