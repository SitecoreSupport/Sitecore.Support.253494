namespace Sitecore.Support.EmailCampaign.Server.Helpers
{
  using Microsoft.Extensions.DependencyInjection;
  using Sitecore;
  using Sitecore.Abstractions;
  using Sitecore.Configuration.KnownSettings;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.DependencyInjection;
  using Sitecore.Diagnostics;
  using Sitecore.EmailCampaign.Server.Helpers;
  using Sitecore.Framework.Conditions;
  using Sitecore.Globalization;
  using Sitecore.Modules.EmailCampaign;
  using Sitecore.Modules.EmailCampaign.Core;
  using Sitecore.Modules.EmailCampaign.Core.Data;
  using Sitecore.Modules.EmailCampaign.Messages;
  using Sitecore.Modules.EmailCampaign.Services;
  using System;
  using System.Web;

  public class MessageHelper : IMessageHelper
  {
    private EcmDataProvider _dataProvider;
    private readonly IExmCampaignService _exmCampaignService;
    private readonly IManagerRootService _managerRootService;
    private CoreSettings coreSettings;

    public MessageHelper(EcmDataProvider dataProvider, IExmCampaignService exmCampaignService, IManagerRootService managerRootService)
    {
      Condition.Requires<EcmDataProvider>(dataProvider, "dataProvider").IsNotNull<EcmDataProvider>();
      Condition.Requires<IExmCampaignService>(exmCampaignService, "exmCampaignService").IsNotNull<IExmCampaignService>();
      Condition.Requires<IManagerRootService>(managerRootService, "managerRootService").IsNotNull<IManagerRootService>();
      this._dataProvider = dataProvider;
      this._exmCampaignService = exmCampaignService;
      this._managerRootService = managerRootService;
      coreSettings = new CoreSettings(ServiceProviderServiceExtensions.GetService<BaseSettings>(ServiceLocator.ServiceProvider));
    }

    public string CreateNewMessage(string managerRootId, string messageTemplateId, string messageName, string messageTypeTemplateId, string layoutId)
    {
      Assert.ArgumentNotNull(managerRootId, "managerRootId");
      Assert.ArgumentNotNull(messageTemplateId, "messageTemplateId");
      Assert.ArgumentNotNull(messageName, "messageName");
      Assert.ArgumentNotNull(messageTypeTemplateId, "messageTypeTemplateId");
      ManagerRoot managerRootFromId = this._managerRootService.GetManagerRootFromId(Guid.Parse(managerRootId));
      if (managerRootFromId != null)
      {
        MessageItem item2;
        if (string.IsNullOrEmpty(messageTypeTemplateId))
        {
          return null;
        }
        Item item = managerRootFromId.InnerItem.Axes.SelectSingleItem($"./descendant::*[@@tid='{new ID(messageTypeTemplateId)}']");
        if (item == null)
        {
          return null;
        }
        Language result = null;

        if (string.IsNullOrEmpty(Context.User.Profile.ContentLanguage))
        {
          if (string.IsNullOrEmpty(coreSettings.DefaultLanguage))
          {
            result = null;
          }
          else
          {
            ParseLanguage(coreSettings.DefaultLanguage, out result);
          }
        }
        else
        {
          ParseLanguage(Context.User.Profile.ContentLanguage, out result);
        }

        if (string.IsNullOrEmpty(layoutId))
        {
          item2 = MessageItemSource.Create(HttpUtility.HtmlEncode(messageName), messageTemplateId, item.ID.ToString(), this._exmCampaignService, this._managerRootService, result);
        }
        else
        {
          Item item3 = new ItemUtilExt().GetItem(layoutId);
          if (item3 == null)
          {
            return null;
          }
          item2 = ABTestMessageSource.CreateAbTestMessage(messageName, item3.ID, item.ID, messageTemplateId, this._exmCampaignService, this._managerRootService, result);
        }
        if (item2 != null)
        {
          this._dataProvider.SaveCampaign(item2, null, SaveCampaignFlags.None, null);
          return item2.ID;
        }
      }
      return null;
    }

    private void ParseLanguage(string language, out Language result)
    {
      Language.TryParse(language, out result);

      if (!Util.GetContentDb().GetLanguages().Contains(result))
      {
        result = null;
      }
    }
  }
}