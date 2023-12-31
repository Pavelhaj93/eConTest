﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using eContracting.Models;
using eContracting.Storage;

namespace eContracting.Services
{
    /// <inheritdoc/>
    /// <seealso cref="eContracting.ILoginFailedAttemptBlockerStore" />
    [ExcludeFromCodeCoverage]
    public class DbLoginFailedAttemptBlockerStore : ILoginFailedAttemptBlockerStore
    {
        /// <summary>
        /// The connection string.
        /// </summary>
        protected readonly string ConnectionString;

        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// SettingsReader, needed for database connection string
        /// </summary>
        protected readonly ISettingsReaderService SettingsReaderService;

        /// <summary>
        ///         Initializes a new instance of the <see cref="DbLoginFailedAttemptBlockerStore"/> class.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="settingsReaderService">SettingsReader</param>
        public DbLoginFailedAttemptBlockerStore(ILogger logger,ISettingsReaderService settingsReaderService)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
            this.ConnectionString = settingsReaderService.GetCustomDatabaseConnectionString();
        }


        /// <inheritdoc/>
        public void Add(LoginFailureModel loginAttemptModel)
        {
            try
            {
                using (var context = new DatabaseContext(this.ConnectionString))
                {
                    var record = new LoginAttempt()
                    {
                        Guid = loginAttemptModel.Guid,
                        SessionId = loginAttemptModel.SessionId,
                        Time = loginAttemptModel.Timestamp,
                        IsBirthdateValid = loginAttemptModel.IsBirthdateValid,
                        LoginTypeKey = loginAttemptModel.LoginType?.Key,
                        IsLoginValueValid = loginAttemptModel.IsValueValid,
                        BrowserAgent = loginAttemptModel.BrowserAgent,
                        LoginState = loginAttemptModel.LoginState.ToString(),
                        IsBlocking = (loginAttemptModel.IsBirthdateValid == false || loginAttemptModel.IsValueValid == false),
                        CampaignCode = loginAttemptModel.CampaignCode
                    };
                    context.LoginAttempts.Add(record);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error(loginAttemptModel?.Guid, "Cannot Add failed log-in attempts into database", ex);
            }
        }


        /// <inheritdoc/>
        public bool IsAllowed(string guid, int maxFailedAttempts, TimeSpan delayAfterFailedAttempts)
        {
            bool result = false;
            try
            {
                using (var context = new DatabaseContext(this.ConnectionString))
                {
                    var failedBlockingAttempts = context.LoginAttempts.Where(a =>a.Guid == guid && (a.IsBlocking == true));
                    if (failedBlockingAttempts.Count() < maxFailedAttempts)
                    {
                        // there are none or few bad attempts, let the user try log in
                        result = true;
                    }
                    else
                    {
                        if (maxFailedAttempts > 0)
                        {
                            var lastTimeOfFailedAttempt = failedBlockingAttempts.OrderByDescending(a => a.Time).Skip(maxFailedAttempts - 1).FirstOrDefault()?.Time;
                            if (lastTimeOfFailedAttempt.HasValue && lastTimeOfFailedAttempt < (DateTime.UtcNow - delayAfterFailedAttempts))
                            {
                                // last N bad attempts are old enough, let the user try again
                                result = true;
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                this.Logger.Error(guid, "Cannot check failed log-in attempts saved in database", ex);
                return true; // do not block visitor due to an internal exception
            }
        }

        /// <inheritdoc/>
        public void Clear(string guid)
        {
            try
            {
                using (var context = new DatabaseContext(this.ConnectionString))
                {
                    var attemptsToClear = context.LoginAttempts.Where(a => a.Guid == guid && (a.IsBlocking == true)).ToList();
                    if (attemptsToClear != null && attemptsToClear.Any())
                    {
                        attemptsToClear.ForEach(at => at.IsBlocking = false);                        
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error(guid, "Cannot Clear failed log-in attempts from database", ex);
            }
        }

        /// <inheritdoc/>
        public int DeleteAllOlderThan(DateTime date, bool previewOnly)
        {
            int result = 0;

            try
            {
                using (var context = new DatabaseContext(this.ConnectionString))
                {
                    var attemptsToClear = context.LoginAttempts.Where(a => a.Time <date).ToList();
                    if (attemptsToClear != null && attemptsToClear.Any())
                    {
                        result = attemptsToClear.Count;
                        if (!previewOnly)
                        {
                            attemptsToClear.ForEach(at => { context.LoginAttempts.Remove(at); });
                        }
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error(String.Empty, "Cannot DeleteAllOlderThan failed log-in attempts from database", ex);
            }

            return result;
        }
    }
}
