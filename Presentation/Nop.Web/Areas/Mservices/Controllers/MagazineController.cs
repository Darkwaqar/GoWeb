using Nop.Admin.Models.Magazines;
using Nop.Core;
using Nop.Core.Domain.Magazines;
using Nop.Services.Helpers;
using Nop.Services.Magazines;
using Nop.Services.Media;
using Nop.Web.Areas.MServices.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Web.Areas.Mservices.Controllers
{
    public class MagazineController : BaseController
    {
        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IMagazineService _magazineService;
        private readonly IPictureService _pictureService;
        private readonly IDownloadService _downloadService;
        private readonly IStoreContext _storeContext;
        #endregion

        #region Ctor
        public MagazineController(IWorkContext workContext,
            IDateTimeHelper dateTimeHelper,
            IMagazineService queuedFcmService,
            IPictureService pictureService,
            IDownloadService downloadService,
            IStoreContext storeContext)
        {
            this._workContext = workContext;
            this._dateTimeHelper = dateTimeHelper;
            this._magazineService = queuedFcmService;
            this._pictureService = pictureService;
            this._downloadService = downloadService;
            this._storeContext = storeContext;
        }
        #endregion

        #region Utilities
        [NonAction]
        protected virtual MagazineModel PrepareMagazineModelForList(Magazine magazine)
        {
            return new MagazineModel
            {
                Id = magazine.Id,
                Name = magazine.Name,
                Description = magazine.Description,
                PictureThumbnailUrl = _pictureService.GetPictureUrl(magazine.PictureId, 600, true),
                Published = magazine.Published,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(magazine.CreatedOnUtc, DateTimeKind.Utc),
                UpdatedOn = _dateTimeHelper.ConvertToUserTime(magazine.UpdatedOnUtc, DateTimeKind.Utc),
                DownloadExpirationDays = magazine.DownloadExpirationDays,
                DownloadId = magazine.DownloadId,
                HasSampleDownload = magazine.HasSampleDownload,
                HasUserAgreement = magazine.HasUserAgreement,
                MaxNumberOfDownloads = magazine.MaxNumberOfDownloads,
                PictureId = magazine.PictureId,
                SampleDownloadId = magazine.SampleDownloadId,
                UnlimitedDownloads = magazine.UnlimitedDownloads,
                UserAgreementText = magazine.UserAgreementText
            };
        }
        #endregion

        public ActionResult GetMagazines(int pageNumber = 0, int pageSize = 10)
        {
            var magazines = _magazineService.SearchMagazines(
               SearchActive: true,
               pageIndex: pageNumber,
               pageSize: pageSize);

            var model = magazines.Select(PrepareMagazineModelForList);

            if (model.Count() > 0)
                return View(model);
            else
                return InvokeHttp400("No Magazines Found");
        }

        public ActionResult GetDownloadById(int id = 0)
        {

            var download = _downloadService.GetDownloadById(id);
            if (download==null)
                return InvokeHttp400("Not a valid download");


            if (download.UseDownloadUrl)
                return View(download.DownloadUrl);

            //use stored data
            if (download.DownloadBinary == null)
                return View(string.Format("Download data is not available any more. Download GD={0}", download.Id));

            string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : download.Id.ToString();
            string contentType = !String.IsNullOrWhiteSpace(download.ContentType)
                ? download.ContentType
                : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType)
            {
                FileDownloadName = fileName + download.Extension
            };

        }
    }
}