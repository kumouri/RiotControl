﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using System.Data.Common;

using LibOfLegends;

using com.riotgames.platform.statistics;
using com.riotgames.platform.summoner;
using com.riotgames.platform.gameclient.domain;

namespace RiotControl
{
	partial class Worker
	{
		public WorkerResult UpdateSummonerByAccountId(int accountId)
		{
			if (!Connected)
				return WorkerResult.NotConnected;

			try
			{
				//It is sub-optimal to have this blocking RPC before performing multiple concurrent non-blocking RPCs
				//The only advantage is that it avoids hammering the server with several invalid requests at once
				AllPublicSummonerDataDTO publicSummonerData = RPC.GetAllPublicSummonerDataByAccount(accountId);
				if (publicSummonerData != null)
				{
					Summoner summoner = new Summoner(publicSummonerData, Region);
					using (var connection = Provider.GetConnection())
						InsertNewSummoner(summoner, connection);
					using (var connection = Provider.GetConnection())
						UpdateSummoner(summoner, connection);
					return WorkerResult.Success;
				}
				else
				{
					//The summoner could not be found on the server
					return WorkerResult.NotFound;
				}
			}
			catch (RPCTimeoutException)
			{
				return WorkerResult.Timeout;
			}
		}
	}
}