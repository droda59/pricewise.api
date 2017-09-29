using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;

using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    internal class AlertRepository : EntityRepository<UserAlert>, IAlertRepository
    {
        private readonly IUserRepository _userRepository;

        public AlertRepository(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
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
            repoUserAlert.LastModifiedAt = DateTime.UtcNow;

            await this._userRepository.UpdateAsync(userId, repoUser);
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

            await this._userRepository.UpdateAsync(userId, repoUser);

            return true;
        }
    }
}