using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using DEService;

namespace BoadService.Diag
{
	class N_Diag
	{
		ICanDriver can;
		N_Message nmsg;
		object locker;

		public event Action<N_Message> OnRxComplete;
		public event Action<N_Message> OnTxComplete;
        public event Action<N_Message> OnTxError;
        public event Action<N_Message> OnRxIDsComplete;

        byte RequestCnt = 0;

        System.Timers.Timer rxTimer;

        public N_Diag(ICanDriver can)
		{
			locker = new object();
			this.can = can;
			this.can.OnReadMessage += ProcessMessage;

			this.nmsg = new N_Message();

            rxTimer = new System.Timers.Timer(100);
            rxTimer.Elapsed += RxTimer_Elapsed;
            
        }

        private void RxTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            
            if (nmsg.TA > 0 && RequestCnt < 10)
            {
                RequestCnt++;
                nmsg.State = N_State.TX_SEND;
                NSend(nmsg);
            }
            else if(RequestCnt >= 10)
            {
                rxTimer.Stop();
            }
        }

        #region Threading        

        void ProcessMessage(CAN_Message msg)
		{
			lock (locker)
			{
				if ((msg.id == (0x580 + nmsg.TA)))
				{
                    rxTimer.Stop();
                    RequestCnt = 0;
                    nmsg.MsgType = (N_Types)(msg.data[0] & 0x0F);
					nmsg.Result = (N_Res)((msg.data[0] >> 4) & 0x0F);
					nmsg.MsgNum = msg.data[3];
					nmsg.Index = (ushort)((((msg.data[2]) << 8) + msg.data[1]));                    

                    switch (nmsg.MsgType)
					{
						case N_Types.ecan_profile_fun_write:							

							if (nmsg.Result == N_Res.OK_UPDATE_PROFILE)
							{
								nmsg.MsgNum++;
								nmsg.State = N_State.TX_SEND;
							}
							else if (nmsg.Result == N_Res.UPDATE_PROFILE_FINISIHED)
							{
								nmsg.MsgNum = 0;
								nmsg.State = N_State.IDLE;

								OnTxComplete?.Invoke(nmsg);
							}
							else if (nmsg.Result == N_Res.SERIES_ERROR)
							{
                                nmsg.MsgNum = 0;
                                nmsg.State = N_State.IDLE;

                                OnTxError?.Invoke(nmsg);
                            }
							else if (nmsg.Result == N_Res.CRC_MISTMATCH_ERROR)
							{
                                nmsg.MsgNum = 0;
                                nmsg.State = N_State.IDLE;

                                OnTxError?.Invoke(nmsg);
                            }

							NSend(nmsg);
							break;

						case N_Types.ecan_profile_fun_read:
                            
                            if (nmsg.Result == N_Res.OK_UPDATE_PROFILE)
							{
                                nmsg.data[((nmsg.MsgNum + 1) * 4) - 1] = msg.data[7];
                                nmsg.data[((nmsg.MsgNum + 1) * 4) - 2] = msg.data[6];
                                nmsg.data[((nmsg.MsgNum + 1) * 4) - 3] = msg.data[5];
								nmsg.data[((nmsg.MsgNum + 1) * 4) - 4] = msg.data[4];
								nmsg.MsgNum++;
								nmsg.State = N_State.TX_SEND;

                                NSend(nmsg);
                            }
							else if (nmsg.Result == N_Res.END_OF_COMMUNICATION)
							{
								nmsg.MsgNum = 0;
								nmsg.State = N_State.IDLE;

								OnRxComplete?.Invoke(nmsg);
							}
							else if (nmsg.Result == N_Res.SERIES_ERROR)
							{
                                nmsg.MsgNum = 0;
                                nmsg.State = N_State.IDLE;
                            }
							else if (nmsg.Result == N_Res.CRC_MISTMATCH_ERROR)
							{
                                nmsg.MsgNum = 0;
                                nmsg.State = N_State.IDLE;
                            }
							
							break;

						case N_Types.ecan_diag_value_read:    
                            if (nmsg.Result == N_Res.OK_UPDATE_PROFILE)
							{
                                nmsg.data[((nmsg.MsgNum + 1) * 4) - 1] = msg.data[7];
                                nmsg.data[((nmsg.MsgNum + 1) * 4) - 2] = msg.data[6];
                                nmsg.data[((nmsg.MsgNum + 1) * 4) - 3] = msg.data[5];
                                nmsg.data[((nmsg.MsgNum + 1) * 4) - 4] = msg.data[4];
                                nmsg.MsgNum++;
								nmsg.State = N_State.TX_SEND;
							}
							else if (nmsg.Result == N_Res.END_OF_COMMUNICATION)
							{
                                //nmsg.MsgNum = 0;
                                nmsg.State = N_State.IDLE;

								OnRxComplete?.Invoke(nmsg);
							}
							else if (nmsg.Result == N_Res.SERIES_ERROR)
							{
								nmsg.State = N_State.TX_SEND;
							}
							else if (nmsg.Result == N_Res.CRC_MISTMATCH_ERROR)
							{
								int a = 5;
							}
							NSend(nmsg);
							//if (nmsg.Result == N_Res.END_OF_COMMUNICATION)
							//{
							//	nmsg.data[1] = msg.data[5];
							//	nmsg.data[0] = msg.data[4];								
							//	nmsg.State = N_State.IDLE;

							//	OnRxComplete?.Invoke(nmsg);
							//}
							break;
					}
				}	
				else if((msg.id & ~0x1F) == 0x700)
				{
                    if (OnRxIDsComplete != null)
                    {
                        N_Message loc_msg = new N_Message();

                        loc_msg.Result = N_Res.OK_UPDATE_PROFILE;
                        loc_msg.TA = (byte)(msg.id & 0x1F);

                        OnRxIDsComplete?.Invoke(loc_msg);                        
                    }

				}
			}
		}

		void NSend(N_Message nmsg)
		{
			CAN_Message msg = new CAN_Message();
			msg.Ext = false;


            if (nmsg.State == N_State.TX_SEND)
			{
				msg.id = (uint)(0x600 + nmsg.TA);
				msg.data[0] = (byte)nmsg.MsgType;
				msg.data[1] = (byte)nmsg.Index;
				msg.data[2] = (byte)(nmsg.Index >> 8);
				msg.data[3] = (byte)nmsg.MsgNum;

				msg.data[4] = (byte)nmsg.data[((nmsg.MsgNum + 1) * 4) - 4];
				msg.data[5] = (byte)(nmsg.data[((nmsg.MsgNum + 1) * 4) - 3]);
                msg.data[6] = (byte)(nmsg.data[((nmsg.MsgNum + 1) * 4) - 2]);
                msg.data[7] = (byte)(nmsg.data[((nmsg.MsgNum + 1) * 4) - 1]);
                can.SendMessage(msg);

				nmsg.State = N_State.IDLE;

                rxTimer.Start();                

            }
			else if(nmsg.State == N_State.IDLE)
			{

			}
		}

		public void Send(byte address, N_Types type, byte[] buf, byte ID)
		{
			// Формируем адрес
			nmsg.TA = address;
			nmsg.MsgNum = 0;
			nmsg.Index = ID;
			nmsg.State = N_State.TX_SEND;
			nmsg.MsgType = type;
			// Данные
			if (buf != null)
				buf.CopyTo(nmsg.data, 0);
			else
				Array.Clear(nmsg.data, 0, nmsg.data.Length);



            NSend(nmsg);
		}

        private void NMsgClear(N_Message nmsg)
        {
            nmsg.TA = 0x00;
            nmsg.MsgNum = 0;
            nmsg.Index = 00;
            nmsg.State = N_State.IDLE;
            nmsg.MsgType = 0;
        }

		#endregion


		
	}

}
