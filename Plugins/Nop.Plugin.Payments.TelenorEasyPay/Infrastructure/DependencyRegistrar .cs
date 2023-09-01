using Nop.Core.Data;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Payments.TelenorEasyPay.Data;
using Nop.Plugin.Payments.TelenorEasyPay.Domain;
using Nop.Plugin.Payments.TelenorEasyPay.Services;
using Autofac;
using Autofac.Core;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.TelenorEasyPay.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {

        private const string CONTEXT_NAME = "nop_object_context_easy_pay_status";

        public void Register(ContainerBuilder builder, Core.Infrastructure.ITypeFinder typeFinder, Core.Configuration.NopConfig config)
        {
            builder.RegisterType<EasyPayResponseService>().As<IEasyPayResponseService>().InstancePerLifetimeScope();
            builder.RegisterType<EasyPayPaymentStatusUrlService>().As<IEasyPayPaymentStatusUrlService>().InstancePerLifetimeScope();
            builder.RegisterType<EasyPayPaymentStatusService>().As<IEasyPayPaymentStatusService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<EasyPayStatusObjectContext>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<EasyPayResponse>>()
                .As<IRepository<EasyPayResponse>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
            
            builder.RegisterType<EfRepository<EasyPayPaymentStatusUrl>>()
                .As<IRepository<EasyPayPaymentStatusUrl>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<EasyPayPaymentStatus>>()
                .As<IRepository<EasyPayPaymentStatus>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 1; }
        }

    }

}
