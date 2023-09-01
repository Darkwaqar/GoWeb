using Nop.Core;
using Nop.Core.Domain.ShopTheLook;
using System.Collections.Generic;

namespace Nop.Services.ShopTheLook
{
    public partial interface IShopTheLookService
    {
        /// <summary>
        /// Inserts a pointer
        /// </summary>
        /// <param name="pointer">Pointer</param>
        void InsertPointer(Pointer pointer);

        /// <summary>
        /// Updates a pointer
        /// </summary>
        /// <param name="pointer">Pointer</param>
        void UpdatePointer(Pointer pointer);

        /// <summary>
        /// Deleted a pointer
        /// </summary>
        /// <param name="pointer">Pointer</param>
        void DeletePointer(Pointer pointer);

        /// <summary>
        /// Deleted a pointers
        /// </summary>
        /// <param name="pointers">Pointers</param>
        void DeletePointers(IList<Pointer> pointers);


        /// <summary>
        /// Deleted a pointers
        /// </summary>
        /// <param name="PictureId">Picture Id</param>
        void DeleteAllPointers(int PictureId);

        /// <summary>
        /// Gets a pointer by identifier
        /// </summary>
        /// <param name="pointerId">Pointer identifier</param>
        /// <returns>Pointer</returns>
        Pointer GetPointerById(int pointerId);

        /// <summary>
        /// Get pointers by identifiers
        /// </summary>
        /// <param name="pointerIds">pointer identifiers</param>
        /// <returns>Pointers</returns>
        IList<Pointer> GetPointersByIds(int[] pointerIds);

        /// <summary>
        /// Gets all pointers
        /// </summary>
        /// <param name="TaggedProductId">Tagged Product Id</param>
        /// <param name="ProductId">Pointer of Product</param>
        /// <param name="PictureId">Pointer of Picture</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Pointers list</returns>
        IPagedList<Pointer> SearchPointers(int TaggedProductId = 0, int ProductId = 0, int PictureId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets all pointer
        /// </summary>
        /// <returns>Pointer</returns>
        IList<Pointer> GetAllPointer();
    }
}
