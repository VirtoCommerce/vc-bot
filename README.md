# Virto Commerce Order Bot. Configuration instructions

## Prerequisites
* Azure subscription https://azure.microsoft.com/en-us/free/
* Access to running Virto Commerce Platform

## Configuring platform and bot
1. Create "BotUserName" dynamic property (Value Type: Short Text) to VirtoCommerce.Domain.Customer.Model.Contact type. Restart the application.
2. Create security account for authenticating as a bot. 
   * Mark it "Is administrator".
   * Add API Account (Hmac type)
3. Fill VirtoCommerce.OrderBot\appsettings.json

## Deploy to Azure
1. Sign in to Azure portal
2. Create Bot Channel (Create a resource -> Bot Channels Registration)
3. Add application secrets (Registered bot channels -> Settings -> Microsoft App ID (Manage))
4. Fill "AppId" and "SecretKey" in the VirtoCommerce.OrderBot\appsettings.json
5. Deploy bot as regular web application
6. Fill Messaging endpoint (Registered bot channels -> Settings -> Messaging endpoint) (applicationEndpoint/api/messages)
7. Add channels as you need
8. Enjoy!

https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-deploy-az-cli?view=azure-bot-service-4.0

## Bot authorization

The first time when you contact the bot, it will send you your identifier. You need to save the identifier to your Contact's dynamic property (BotUserName), reindex the entity, and try to authorize with the bot.

# Bot's technical details

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows the minimum code required to build a bot.

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.2

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## To try this sample

- In a terminal, navigate to `VirtoCommerce.OrderBot`

    ```bash
    # change into project folder
    cd VirtoCommerce.OrderBot
    ```

- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `VirtoCommerce.OrderBot` folder
  - Select `VirtoCommerce.OrderBot.csproj` file
  - Press `F5` to run the project

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)