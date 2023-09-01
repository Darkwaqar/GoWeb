using System;
using Nop.Services.Logging;
using Nop.Services.Tasks;

namespace Nop.Services.Fcm
{
    public partial class QueuedFcmSendTask : ITask
    {

        private readonly IQueuedFcmService _queuedFcmService;
        private readonly IFcmSender _emailSender;
        private readonly ILogger _logger;
        private readonly IFcmActionService _fcmActionService;

        public QueuedFcmSendTask(IQueuedFcmService queuedFcmService,
            IFcmSender emailSender, ILogger logger, IFcmActionService fcmActionService)
        {
            this._queuedFcmService = queuedFcmService;
            this._emailSender = emailSender;
            this._logger = logger;
            this._fcmActionService = fcmActionService;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public virtual void Execute()
        {
            var maxTries = 3;
            var queuedFcms = _queuedFcmService.SearchFcms(null, null, null, null, 0, 0, null, null,
                true, true, maxTries, false, 0, 500);

            foreach (var queuedFcm in queuedFcms)
            {
                var action = _fcmActionService.GetFcmActionById(queuedFcm.Intent);
                try
                {
                    _emailSender.SendFcmAsync(queuedFcm,action);

                    queuedFcm.SentOnUtc = DateTime.UtcNow;

                }
                catch (Exception exc)
                {
                    _logger.Error(string.Format("Error sending fcm. {0}", exc.Message), exc);
                }
                finally
                {
                    queuedFcm.SentTries = queuedFcm.SentTries + 1;
                    _queuedFcmService.UpdateQueuedFcm(queuedFcm);
                }
            }
        }
    }
}
