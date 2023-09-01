using Nop.Core;
using Nop.Core.Domain.Magazines;
using System.Collections.Generic;

namespace Nop.Services.Magazines
{
    public partial interface IMagazineService
    {
        /// <summary>
        /// Inserts a magazine
        /// </summary>
        /// <param name="magazine">Magazine</param>
        void InsertMagazine(Magazine magazine);

        /// <summary>
        /// Updates a magazine
        /// </summary>
        /// <param name="magazine">Magazine</param>
        void UpdateMagazine(Magazine magazine);

        /// <summary>
        /// Deleted a magazine
        /// </summary>
        /// <param name="magazine">Magazine</param>
        void DeleteMagazine(Magazine magazine);

        /// <summary>
        /// Deleted a magazines
        /// </summary>
        /// <param name="magazines">Magazines</param>
        void DeleteMagazines(IList<Magazine> magazines);

        /// <summary>
        /// Gets a magazine by identifier
        /// </summary>
        /// <param name="magazineId">Magazine identifier</param>
        /// <returns>Magazine</returns>
        Magazine GetMagazineById(int magazineId);

        /// <summary>
        /// Get magazines by identifiers
        /// </summary>
        /// <param name="magazineIds">magazine identifiers</param>
        /// <returns>Magazines</returns>
        IList<Magazine> GetMagazinesByIds(int[] magazineIds);

        /// <summary>
        /// Search magazines
        /// </summary>
        /// <param name="SearchName">Search by name</param>
        /// <param name="Showhidden">Is Action status Active  ;false to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Magazines</returns>
        IPagedList<Magazine> SearchMagazines(string SearchName=null, string SearchDescription=null, bool SearchActive = false, int pageIndex = 0, int pageSize = int.MaxValue);


        /// <summary>
        /// Gets all magazine
        /// </summary>
        /// <returns>Magazine</returns>
        IList<Magazine> GetAllMagazine();
      
    }
}
