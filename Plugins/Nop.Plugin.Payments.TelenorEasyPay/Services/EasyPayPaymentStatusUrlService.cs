using Nop.Core.Data;
using Nop.Plugin.Payments.TelenorEasyPay.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.TelenorEasyPay.Services
{
    public class EasyPayPaymentStatusUrlService : IEasyPayPaymentStatusUrlService
    {
        private readonly IRepository<EasyPayPaymentStatusUrl> _easyPayRepository;

        public EasyPayPaymentStatusUrlService(IRepository<EasyPayPaymentStatusUrl> easyPayRepository)
        {
            _easyPayRepository = easyPayRepository;
        }


        public void Log(EasyPayPaymentStatusUrl record)
        {
            _easyPayRepository.Insert(record);
        }


        public EasyPayPaymentStatusUrl GetEasyPayPaymentStatusUrlByOrderId(int orderId)
        {
            var query = from o in _easyPayRepository.Table
                        where o.OrderId == orderId
                        select o;

            return query.FirstOrDefault();
        }

    }
}
