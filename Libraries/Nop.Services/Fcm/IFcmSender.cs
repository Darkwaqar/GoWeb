
using Nop.Core.Domain.Fcm;
using Nop.Core.Domain.Messages;

namespace Nop.Services.Fcm
{
    /// <summary>
    /// Fcm sender
    /// </summary>
    public partial interface IFcmSender
    {
        /// <summary>
        /// Sends an fcm
        /// </summary>
        /// <param name="appKey">Fcm app to use</param>
        /// <param name="package">package name of application</param>
        /// <param name="topic">Topic name / vendor name </param>
        /// <param name="title">Title Message header</param>
        /// <param name="deviceId">DeviceId Message header</param>
        /// <param name="message">Message Body</param>
        /// <param name="image">Image url to be send</param>
        /// <param name="action">Action done by notification</param>
        bool SendFcmAsync(QueuedFcm queuedFcm, FcmAction action);
    }
}
