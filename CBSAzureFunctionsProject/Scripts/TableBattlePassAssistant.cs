using CBS.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace CBS
{
    public class TableBattlePassAssistant
    {
        private static readonly string BankPartitionKey = "CBSBank";

        private static readonly string ExpKey = "Exp";
        private static readonly string RowKey = "RowKey";

        public static async Task<ExecuteResult<Azure.Response>> UpdateBankExpAsync(string profileID, string battlePassInstanceID, int exp)
        {
            var profileEntity = GetBankEntity(profileID);
            profileEntity[ExpKey] = exp;
            var updateResult = await CosmosTable.UpdateEntityAsync(battlePassInstanceID, profileEntity);
            if (updateResult.Error != null)
            {
                return ErrorHandler.ThrowError<Azure.Response>(updateResult.Error);
            }
            return new ExecuteResult<Azure.Response>
            {
                Result = updateResult.Result
            };
        }

        public static async Task<ExecuteResult<Azure.Response>> AddBankExpAsync(string profileID, string battlePassInstanceID, int exp)
        {
            var profileEntity = GetBankEntity(profileID);
            profileEntity[ExpKey] = exp;
            var addResult = await CosmosTable.AddEntityAsync(battlePassInstanceID, profileEntity);
            if (addResult.Error != null)
            {
                return ErrorHandler.ThrowError<Azure.Response>(addResult.Error);
            }
            return new ExecuteResult<Azure.Response>
            {
                Result = addResult.Result
            };
        }

        public static async Task GrantBankRewards(string battlePassID, string battlePassInstanceID, ILogger log)
        {
            var instanceResult = await BattlePassModule.LoadBattlePassInstnanceByIDAsync(battlePassID);
            var passInstance = instanceResult.Result;
            var stepExp = passInstance.ExpStep;
            var bankLevels = passInstance.GetBankLevels(0, true);
            var rewardDelivery = passInstance.BankRewardDelivery ?? new RewardDelivery();
            var deliveryType = rewardDelivery.DeliveryType;

            var client = CosmosTable.GetTableClient(battlePassInstanceID);
            await client.CreateIfNotExistsAsync();
            var entityResult = client.QueryAsync<Azure.Data.Tables.TableEntity>(select: GetBankEntityKeys());

            string token = null;
            do
            {
                await foreach (var page in entityResult.AsPages(token))
                {
                    var pageList = page.Values;
                    foreach (var qEntity in pageList)
                    {
                        var profileID = qEntity.RowKey;
                        var exp = qEntity.GetInt32(ExpKey);
                        if (exp == null)
                        {
                            exp = 0;
                        }
                        var profileLevel = exp/stepExp;
                        var passedLevels = bankLevels.Where(x=>profileLevel >= x.TargetIndex);
                        var rewards = passedLevels.Select(x=>x.Reward);
                        var bankReward = new RewardObject();
                        foreach (var reward in rewards)
                        {
                            bankReward = bankReward.MergeReward(reward);
                        }
                        if (deliveryType == RewardDeliveryType.GRANT_IMMEDIATELY)
                        {
                            await RewardModule.GrantRewardToProfileAsync(bankReward, profileID);
                        }
                        else
                        {
                            // send reward to inbox
                            var notification = CBSNotification.FromRewardDelivery(rewardDelivery, bankReward);
                            await NotificationModule.SendNotificationAsync(notification, profileID);
                        }
                    }
                    token = page.ContinuationToken;
                }
            }
            while(!string.IsNullOrEmpty(token));
            await CosmosTable.DeleteTableAsync(battlePassInstanceID);
        }

        private static Azure.Data.Tables.TableEntity GetBankEntity(string profileID)
        {
            return new Azure.Data.Tables.TableEntity{
                RowKey = profileID,
                PartitionKey = BankPartitionKey
            };
        }

        private static string [] GetBankEntityKeys()
        {
            return new string [] 
            {
                RowKey,
                ExpKey
            };
        }
    }
}