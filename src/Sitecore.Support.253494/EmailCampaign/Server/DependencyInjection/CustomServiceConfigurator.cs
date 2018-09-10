namespace Sitecore.Support.EmailCampaign.Server.DependencyInjection
{
  using Microsoft.Extensions.DependencyInjection;
  using Sitecore.DependencyInjection;
  using Sitecore.EmailCampaign.Server.Controllers.Dispatch.ButtonStates;
  using Sitecore.EmailCampaign.Server.Helpers;
  using Sitecore.EmailCampaign.Server.Model;
  using Sitecore.EmailCampaign.Server.Services;
  using Sitecore.EmailCampaign.Server.Services.Interfaces;
  using Sitecore.ExM.Framework.Formatters;
  using Sitecore.Modules.EmailCampaign.Core.Dispatch;
  using Sitecore.Services.Infrastructure.Sitecore.DependencyInjection;
  using System.Reflection;

  internal class CustomServiceConfigurator : IServicesConfigurator
  {
    public void Configure(IServiceCollection serviceCollection)
    {
      Assembly[] assemblies = new Assembly[] { GetType().Assembly };
      serviceCollection.AddWebApiControllers(assemblies);
      serviceCollection.AddTransient<SortParameterFactory>();
      serviceCollection.AddTransient<IDateTimeFormatter, DateTimeFormatter>();
      serviceCollection.AddTransient<BounceStatisticsService>();
      serviceCollection.AddTransient<DispatchFailedStatisticsService>();
      serviceCollection.AddTransient<OpenClickStatisticsService>();
      serviceCollection.AddTransient<SpamStatisticsService>();
      serviceCollection.AddTransient<UnsubscribeStatisticsService>();
      serviceCollection.AddTransient<ValueEngagementStatisticsService>();
      serviceCollection.AddTransient(provider => Configuration.Factory.CreateObject("exm/recipientValidator", false) as IRecipientValidator);
      serviceCollection.AddTransient<IRecipientListService, RecipientListService>();
      serviceCollection.AddTransient<ICopyToDraftService, CopyToDraftService>();
      serviceCollection.AddTransient<IMessageVariantsService, MessageVariantsService>();
      serviceCollection.AddTransient<IMessageHelper, MessageHelper>();
      serviceCollection.AddTransient<ICanCreateService, CanCreateService>();
      serviceCollection.AddTransient<ICanDeleteService, CanDeleteService>();
      serviceCollection.AddTransient<ICreateLayoutService, CreateLayoutService>();
      serviceCollection.AddTransient<ICreateLayoutHelper, CreateLayoutHelper>();
      serviceCollection.AddTransient<IButtonStateProcessor, ButtonStateProcessor>();
      serviceCollection.AddTransient<IMessageInfoService, MessageInfoService>();
      serviceCollection.AddTransient<IPublishStatisticsService, PublishStatisticsService>();
    }
  }
}