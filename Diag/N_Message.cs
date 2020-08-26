using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFService.Diag
{
	public class N_Message
	{
		uint MAX_N_MSG_DATA_LENGHT = 512;

		public short TA;
		public N_Types MsgType;
		public N_State State;
		public short MsgNum;
		public ushort Index;
		public N_Res Result;
		public short DataLength;
		public byte[] data;

		public N_Message()
		{
			data = new byte[MAX_N_MSG_DATA_LENGHT];
		}
	}

	public enum N_State
	{		
		IDLE,							// Ожидание
		TX_SEND							// Отправить следующее
	}

	public enum N_Types
	{
		ecan_none = 0,
		ecan_profile_fun_read = 1,
		ecan_profile_fun_write,
		ecan_array_fun_read,
		ecan_diag_value_read,
	}

	public enum N_Res
	{
		OK_UPDATE_PROFILE = 0,         // Всё ок, ждём следующей послки
		UPDATE_PROFILE_FINISIHED,      // Обновление завершено
		CRC_MISTMATCH_ERROR,           // Не совпали контрольные суммы
		INDEX_OUTSIDE_BOUNDSE_ERROR,   // Индекс за пределами допустимых границ
		SERIES_ERROR,                  // Ошибка последовательности
		END_OF_COMMUNICATION,           // Конец обновления

	}

}
