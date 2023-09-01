using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.ShopTheLook;
using Nop.Data;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.ShopTheLook
{
    public partial class ShopTheLookService : IShopTheLookService
    {

        private readonly IRepository<Pointer> _pointerRepository;
        private readonly IDbContext _dbContext;
        private readonly IDataProvider _dataProvider;
        private readonly CommonSettings _commonSettings;
        private readonly IEventPublisher _eventPublisher;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pointerRepository">Pointer repository</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="dbContext">DB context</param>
        /// <param name="dataProvider">WeData provider</param>
        /// <param name="commonSettings">Common settings</param>
        public ShopTheLookService(IRepository<Pointer> pointerRepository,
            IEventPublisher eventPublisher,
            IDbContext dbContext,
            IDataProvider dataProvider,
            CommonSettings commonSettings)
        {
            _pointerRepository = pointerRepository;
            _eventPublisher = eventPublisher;
            this._dbContext = dbContext;
            this._dataProvider = dataProvider;
            this._commonSettings = commonSettings;
        }

        /// <summary>
        /// Inserts a pointer 
        /// </summary>
        /// <param name="pointer">Pointer</param>        
        public virtual void InsertPointer(Pointer pointer)
        {
            if (pointer == null)
                throw new ArgumentNullException("pointer");

            _pointerRepository.Insert(pointer);

            //event notification
            _eventPublisher.EntityInserted(pointer);
        }

        /// <summary>
        /// Updates a pointer 
        /// </summary>
        /// <param name="pointer">Pointer</param>
        public virtual void UpdatePointer(Pointer pointer)
        {
            if (pointer == null)
                throw new ArgumentNullException("pointer");

            _pointerRepository.Update(pointer);

            //event notification
            _eventPublisher.EntityUpdated(pointer);
        }

        /// <summary>
        /// Deleted a pointer 
        /// </summary>
        /// <param name="pointer">Pointer</param>
        public virtual void DeletePointer(Pointer pointer)
        {
            if (pointer == null)
                throw new ArgumentNullException("pointer");

            _pointerRepository.Delete(pointer);

            //event notification
            _eventPublisher.EntityDeleted(pointer);
        }

        /// <summary>
        /// Deleted a pointers
        /// </summary>
        /// <param name="PictureId">Picture Id</param>
        public virtual void DeleteAllPointers(int PictureId)
        {
            var query = _pointerRepository.Table;
            if (PictureId > 0) query = query.Where(qe => qe.PictureId == PictureId);
            var pointers = query.ToList();

            _pointerRepository.Delete(pointers);

            //event notification
            foreach (var pointer in pointers)
            {
                _eventPublisher.EntityDeleted(pointer);
            }
        }

        /// <summary>
        /// Deleted a pointer s
        /// </summary>
        /// <param name="pointers">Pointers</param>
        public virtual void DeletePointers(IList<Pointer> pointers)
        {
            if (pointers == null)
                throw new ArgumentNullException("pointers");

            _pointerRepository.Delete(pointers);

            //event notification
            foreach (var pointer in pointers)
            {
                _eventPublisher.EntityDeleted(pointer);
            }
        }

        /// <summary>
        /// Gets a pointer  by identifier
        /// </summary>
        /// <param name="pointerId">Pointer identifier</param>
        /// <returns>Pointer</returns>
        public virtual Pointer GetPointerById(int pointerId)
        {
            if (pointerId == 0)
                return null;

            return _pointerRepository.GetById(pointerId);

        }

        /// <summary>
        /// Get pointers by identifiers
        /// </summary>
        /// <param name="pointerIds">pointer  identifiers</param>
        /// <returns>Pointers</returns>
        public virtual IList<Pointer> GetPointersByIds(int[] pointerIds)
        {
            if (pointerIds == null || pointerIds.Length == 0)
                return new List<Pointer>();

            var query = from qe in _pointerRepository.Table
                        where pointerIds.Contains(qe.Id)
                        select qe;
            var pointers = query.ToList();
            //sort by passed identifiers
            var sortedPointers = new List<Pointer>();
            foreach (int id in pointerIds)
            {
                var pointer = pointers.Find(x => x.Id == id);
                if (pointer != null)
                    sortedPointers.Add(pointer);
            }
            return sortedPointers;
        }

        /// <summary>
        /// Gets all pointers
        /// </summary>
        /// <param name="TaggedProductId">Tagged Product Id</param>
        /// <param name="ProductId">Pointer of Product</param>
        /// <param name="PictureId">Picture Id</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Pointers list</returns>
        public virtual IPagedList<Pointer> SearchPointers(int TaggedProductId = 0, int ProductId = 0, int PictureId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _pointerRepository.Table;
            if (TaggedProductId > 0) query = query.Where(qe => qe.TaggedProductId == TaggedProductId);
            if (ProductId > 0) query = query.Where(qe => qe.ProductId == ProductId);
            if (PictureId > 0) query = query.Where(qe => qe.PictureId == PictureId);

            query = query.OrderByDescending(qe => qe.Id);

            var pointers = new PagedList<Pointer>(query, pageIndex, pageSize);
            return pointers;
        }


        /// <summary>
        /// Gets all fcmapplication
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Pointer</returns>
        public IList<Pointer> GetAllPointer()
        {
            var query = _pointerRepository.Table;
            var applications = query.ToList();
            return applications;
        }
    }
}
