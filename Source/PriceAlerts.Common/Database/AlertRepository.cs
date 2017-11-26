using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson;

using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    internal class AlertRepository : IAlertRepository
    {
        private readonly IUserRepository _userRepository;
        private readonly IListRepository _listRepository;

        public AlertRepository(IUserRepository userRepository, IListRepository listRepository)
        {
            this._userRepository = userRepository;
            this._listRepository = listRepository;
        }

        public async Task<IEnumerable<UserAlert>> GetAllAsync(string userId)
        {
            var repoUser = await this._userRepository.GetAsync(userId);

            return repoUser.Alerts;
        }

        public async Task<UserAlert> GetAsync(string userId, string alertId)
        {
            var repoUser = await this._userRepository.GetAsync(userId);
            var repoUserAlert = repoUser.Alerts.SingleOrDefault(x => x.Id == alertId);
            if (repoUserAlert == null)
            {
                throw new KeyNotFoundException();
            }

            return repoUserAlert;
        }

        public async Task<UserAlert> UpdateAsync(string userId, UserAlert data)
        {
            var repoUser = await this._userRepository.GetAsync(userId);
            var repoUserAlert = repoUser.Alerts.SingleOrDefault(x => x.Id == data.Id);
            if (repoUserAlert == null)
            {
                throw new KeyNotFoundException();
            }
            
            repoUserAlert.Title = data.Title;
            repoUserAlert.BestCurrentDeal = data.BestCurrentDeal;
            repoUserAlert.Entries = data.Entries;
            repoUserAlert.IsActive = data.IsActive;
            repoUserAlert.IsDeleted = data.IsDeleted;
            repoUserAlert.LastModifiedAt = DateTime.UtcNow;

            Task.WaitAll(
                this._userRepository.UpdateAsync(userId, repoUser), 
                this.UpdateListsWithAlert(userId, repoUserAlert));
            
            var updatedAlert = await this.GetAsync(userId, data.Id);

            return updatedAlert;
        }

        public async Task<UserAlert> InsertAsync(string userId, UserAlert data)
        {
            data.Id = ObjectId.GenerateNewId().ToString();
            data.LastModifiedAt = DateTime.UtcNow;

            var repoUser = await this._userRepository.GetAsync(userId);
            repoUser.Alerts.Add(data);

            await this._userRepository.UpdateAsync(userId, repoUser);
            var createdAlert = await this.GetAsync(userId, data.Id);

            return createdAlert;
        }

        public async Task<bool> DeleteAsync(string userId, string alertId)
        {
            var repoUser = await this._userRepository.GetAsync(userId);
            var repoUserAlert = repoUser.Alerts.SingleOrDefault(x => x.Id == alertId);
            if (repoUserAlert == null)
            {
                throw new KeyNotFoundException();
            }

            repoUserAlert.IsDeleted = true;
            repoUserAlert.IsActive = false;
            repoUserAlert.LastModifiedAt = DateTime.UtcNow;

            Task.WaitAll(
                this._userRepository.UpdateAsync(userId, repoUser), 
                this.UpdateListsWithDeletedAlert(userId, repoUserAlert));

            return true;
        }
        
        private async Task UpdateListsWithAlert(string userId, UserAlert repoUserAlert)
        {
            var userLists = await this._listRepository.GetUserListsAsync(userId);
            var listsWithCurrentAlert = userLists.Where(x => x.Alerts.Contains(repoUserAlert)).ToList();
            foreach (var list in listsWithCurrentAlert)
            {
                var alertIndex = ((List<UserAlert>) list.Alerts).IndexOf(repoUserAlert);
                ((List<UserAlert>) list.Alerts)[alertIndex] = repoUserAlert;
                await this._listRepository.UpdateAsync(list);
            }
        }

        private async Task UpdateListsWithDeletedAlert(string userId, UserAlert repoUserAlert)
        {
            var userLists = await this._listRepository.GetUserListsAsync(userId);
            var listsWithCurrentAlert = userLists.Where(x => x.Alerts.Contains(repoUserAlert)).ToList();
            foreach (var list in listsWithCurrentAlert)
            {
                var alertIndex = ((List<UserAlert>) list.Alerts).IndexOf(repoUserAlert);
                ((List<UserAlert>) list.Alerts).RemoveAt(alertIndex);
                await this._listRepository.UpdateAsync(list);
            }
        }
    }
}