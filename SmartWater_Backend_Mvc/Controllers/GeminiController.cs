using Microsoft.AspNetCore.Mvc;
using Smart_Water_Consumption_Service.Services.AiService.DeepLearningModel;
using Smart_Water_Consumption_Service.Services.DbServices;
using SmartWater_Backend_Mvc.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SmartWater_Backend_Mvc.Controllers
{
    public class GeminiController : Controller
    {
        private readonly RegionConsumptionDbService _regionConsumptionDbService;
        private readonly Sub_RegionConsumptionDbService _subregionConsumptionDbService;
        private readonly HouseConsumptionDbService _houseConsumptionDbService;
        private MyLLMAiService _myAi;

        private readonly EmployeeDbService _employeeDbService;
        private readonly ChatMessagesDbService _chatMessagesDbService;
        private readonly MessageOfUsersDbService _messageOfUsersDbService;
        public GeminiController( MyLLMAiService myAi, RegionConsumptionDbService regionConsumptionDbService1, Sub_RegionConsumptionDbService sub_RegionConsumptionDbService, HouseConsumptionDbService houseConsumptionDbService, EmployeeDbService employeeDbService, ChatMessagesDbService chatMessagesDbService, MessageOfUsersDbService messageOfUsersDbService)
        {
            _houseConsumptionDbService = houseConsumptionDbService;
            _regionConsumptionDbService = regionConsumptionDbService1;
            _subregionConsumptionDbService = sub_RegionConsumptionDbService;
            _myAi = myAi;
            _employeeDbService = employeeDbService;
            _chatMessagesDbService = chatMessagesDbService;
            _messageOfUsersDbService = messageOfUsersDbService;
        }

        [HttpGet]
        public async Task<IActionResult> DropWiseAi(int? chatId)
        {
            List<ChatMessage> chatHistory = new List<ChatMessage>();
            try
            {
                string staffId = HttpContext.Session.GetString("stuffId");
                var staff = await _employeeDbService.GetEmployee(staffId);

                // 1. Fetch saved chats for the sidebar list
                chatHistory = await FetchAllUserChatHistory(staffId);

                // 2. Default to 0 (Fresh Welcome View) if no chatId is passed in URL
                int activeChatId = chatId ?? 0;

                // 3. Add the fresh welcome state (ChatId = 0) to the list
                chatHistory.Add(new ChatMessage
                {
                    ChatId = 0,
                    UserId = staffId,
                    ChatTitle = "New Conversation",
                    Timestamp = DateTime.Now,
                    Message = new List<MyMessage>
            {
                new MyMessage
                {
                    IsUser = false,
                    Message = $"Welcome {staff?.Data?.EmployeeFullNames ?? "User"}! How can I assist you with the water infrastructure today?"
                }
            }
                });

                ViewBag.ActiveChatId = activeChatId;
                return PartialView("AiView", chatHistory);
            }
            catch
            {
                return PartialView("AiView", chatHistory);
            }
        }

        [HttpGet]
        public async Task<IActionResult> QueryDatabasePrompt(string prompt, int chatId = 0)
        {
            string staffId = HttpContext.Session.GetString("stuffId");
            var staff = await _employeeDbService.GetEmployee(staffId);

            if (string.IsNullOrWhiteSpace(prompt))
            {
                var currentHistory = await FetchAllUserChatHistory(staffId);
                ViewBag.ActiveChatId = chatId;
                return PartialView("AiView", currentHistory);
            }

            try
            {
                string analyticalSystemPrompt = $@"
        You are DropWise Intelligence, an AI assistant for a municipal water infrastructure management system.

        ### CURRENT EMPLOYEE
        Name: {staff.Data.EmployeeFullNames}
        Role: {staff.Data.EmployeeRole}

        ### ROLE-SPECIFIC RESPONSIBILITY
        {GetRoleProfile(staff.Data.EmployeeRole)}

        ### GENERAL CONTEXT
        The interface already displays live water infrastructure data.
        Do NOT recreate raw data tables.
        Do NOT define basic terminology.
        Do NOT output raw LaTeX equations.

        ### OBJECTIVE
        Answer the employee's question using supplied data directly and concisely.
        ";

                var mydata = await GetFullHydraulicHierarchyAsync();
                var contextData = new { HydraulicRegions = mydata };
                string serializedJsonData = JsonSerializer.Serialize(contextData);

                string executionPrompt = $"{analyticalSystemPrompt}\n\n" +
                                         $"### LIVE HYDRAULIC NETWORK CONTEXT PAYLOAD:\n{serializedJsonData}\n\n" +
                                         $"### FIELD AUDIT REQUEST:\n{prompt}\n\n" +
                                         $"Generate the professional brief:";


                var response = await _myAi.GetLLMResponse(executionPrompt);

                
                int activeChatId = chatId;
                if (activeChatId == 0)
                {
                    Smart_Water_Consumption_Service.Models.ChatMessage newChatMessage = new Smart_Water_Consumption_Service.Models.ChatMessage
                    {
                        UserId = staffId,
                        ChatTitle = GenerateChatTitle(prompt),
                        Timestamp = DateTime.Now
                    };
                    var newChatResult = await _chatMessagesDbService.AddChatMessage(newChatMessage);
                    activeChatId = newChatResult.Data.ChatId;
                }
                var newUserMessage = new Smart_Water_Consumption_Service.Models.MessageOfUser
                {
                    ChatId = activeChatId,
                    IsUser = true,
                    Message = prompt,
                    UserId = staffId
                };
                await _messageOfUsersDbService.AddMessageOfUser(newUserMessage);
                var newAiMessage = new Smart_Water_Consumption_Service.Models.MessageOfUser
                {
                    ChatId = activeChatId,
                    IsUser = false,
                    Message = response,
                    UserId = staffId
                };
                await _messageOfUsersDbService.AddMessageOfUser(newAiMessage);

                var updatedHistory = await FetchAllUserChatHistory(staffId);

                ViewBag.ActiveChatId = activeChatId;
                return PartialView("AiView", updatedHistory);
            }
            catch (Exception ex)
            {
                var fallbackHistory = await FetchAllUserChatHistory(staffId);
                var activeChat = fallbackHistory.FirstOrDefault(c => c.ChatId == chatId);
                if (activeChat != null)
                {
                    activeChat.Message.Add(new MyMessage { IsUser = true, Message = prompt });
                    activeChat.Message.Add(new MyMessage { IsUser = false, Message = $"❌ AI Engineering Pipeline Failure: {ex.Message}" });
                }
                ViewBag.ActiveChatId = chatId;
                return PartialView("AiView", fallbackHistory);
            }
        }

        private async Task<List<ChatMessage>> FetchAllUserChatHistory(string staffId)
        {
            List<ChatMessage> chatHistory = new List<ChatMessage>();
            var chats = await _chatMessagesDbService.GetAllUseChatMessage(staffId);

            if (chats?.Data != null && chats.Data.Any())
            {
                foreach (var x in chats.Data)
                {
                    var messagesList = await GetAllChatMessages(x.ChatId);
                    chatHistory.Add(new ChatMessage
                    {
                        ChatId = x.ChatId,
                        UserId = x.UserId,
                        ChatTitle = x.ChatTitle,
                        Timestamp = x.Timestamp,
                        Message = messagesList
                    });
                }
            }

            return chatHistory;
        }

        private async Task<List<MyMessage>> GetAllChatMessages(int chatId)
        {
            var data = await _messageOfUsersDbService.GetAllChatMessages(chatId);
            var messages = data?.Data?.Select(x => new MyMessage
            {
                IsUser = x.IsUser,
                Message = x.Message
            }).ToList();

            return messages ?? new List<MyMessage>();
        }
        private string GetRoleProfile(string role)
        {
            return role?.Trim().ToLower() switch
            {
                    "engineer" => @"
                    This employee is responsible for hydraulic infrastructure.

                    Prioritize:
                    - Leakage detection
                    - Water transmission anomalies
                    - Regional and sub-regional consumption patterns
                    - Night-time usage
                    - Usage spikes
                    - Structural infrastructure risks
                    - Water sent versus water used
                    - Relationships between regions, sub-regions and households

                    When an anomaly is detected, trace it through the hierarchy
                    and identify the most specific affected location possible.

                    Do not focus on financial information.",

                "finance" => @"
                    This employee is responsible for financial analysis of the
                    water network.

                    Prioritize:
                    - Water billing
                    - Outstanding amounts
                    - Expected versus actual amounts
                    - Revenue discrepancies
                    - Financial impact of water losses
                    - Regional financial performance
                    - Financial anomalies

                    When possible, connect financial anomalies to the affected
                    region or sub-region.",

                "admin" => @"
                    This employee oversees the overall water network operation.

                    Prioritize:
                    - Network-wide performance
                    - Major regional anomalies
                    - Operational risks
                    - Severe leakage
                    - Unusual consumption
                    - System-level trends
                    - Areas requiring immediate attention

                    Provide a high-level operational overview.",

                _ => @"
                Provide a balanced overview of the available water infrastructure
                data.

                Focus on significant anomalies, consumption patterns, leakage,
                operational risks and relationships between regions,
                sub-regions and households."
            };
        }
        private string GenerateChatTitle(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return "New Conversation";

            string title = prompt.Trim();

            title = Regex.Replace(title, @"\s+", " ");

            if (title.Length > 45)
                title = title.Substring(0, 45).Trim();

            title = title.Trim('.', ',', '?', '!', ':', ';');

            return title;
        }


        private async Task<List<RegionHierarchyDto>> GetFullHydraulicHierarchyAsync()
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            var regionsTask = _regionConsumptionDbService.GetAllRegionsConsumptions();
            var subRegionsTask = _subregionConsumptionDbService.GetAllSub_RegionConsumption();
            var housesTask = _houseConsumptionDbService.GetAllHouseConsumption();

            await Task.WhenAll(regionsTask, subRegionsTask, housesTask);

            var regionsData = regionsTask.Result.Data
                .Where(r => r.Date.HasValue && r.Date.Value.Year == currentYear && r.Date.Value.Month == currentMonth);

            var subRegionsData = subRegionsTask.Result.Data
                .Where(sr => sr.Date.HasValue && sr.Date.Value.Year == currentYear && sr.Date.Value.Month == currentMonth);

            var housesData = housesTask.Result.Data
                .Where(h => h.Date.HasValue && h.Date.Value.Year == currentYear && h.Date.Value.Month == currentMonth);

            var housesBySubRegion = housesData
                .Where(h => h.Sub_RegionId.HasValue)
                .GroupBy(h => h.Sub_RegionId!.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(h => new HouseholdHierarchyDto
                    {
                        AccountNumber = h.Owner_AccountNumber,
                        DailyUsage = h.Daily_Usage.GetValueOrDefault(),
                        NightUsage = h.Night_Usage.GetValueOrDefault(),
                        LeakagePercent = h.Leakage_Percent.GetValueOrDefault(),
                        SpikeUsage = h.Spike_Usage.GetValueOrDefault()
                    }).ToList()
                );

            var subRegionsByRegion = subRegionsData
                .GroupBy(sr => sr.RegionId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(sr => new SubRegionHierarchyDto
                    {
                        SubRegionId = sr.Sub_RegionId,
                        SubRegionName = sr.Sub_RegionName,
                        WaterSent = sr.Water_Sent.GetValueOrDefault(),
                        WaterUsed = sr.Water_Payed.GetValueOrDefault(),
                        DailyUsage = sr.Daily_Usage.GetValueOrDefault(),
                        NightUsage = sr.Night_Usage.GetValueOrDefault(),
                        LeakagePercent = sr.Leakage_Percent.GetValueOrDefault(),
                        SpikeUsage = sr.Spike_Usage.GetValueOrDefault(),
                        Households = housesBySubRegion.GetValueOrDefault(sr.Sub_RegionId, new())
                    }).ToList()
                );

            var hierarchy = regionsData.Select(r => new RegionHierarchyDto
            {
                RegionId = r.Region_Id,
                RegionName = r.Region_Name,
                WaterSent = r.Water_Sent.GetValueOrDefault(),
                WaterUsed = r.Water_Payed.GetValueOrDefault(),
                DailyUsage = r.Daily_Usage.GetValueOrDefault(),
                NightUsage = r.Night_Usage.GetValueOrDefault(),
                LeakagePercent = r.Leakage_Percent.GetValueOrDefault(),
                SpikeUsage = r.Spike_Usage.GetValueOrDefault(),
                SubRegions = subRegionsByRegion.GetValueOrDefault(r.Region_Id, new())
            }).ToList();

            return hierarchy;
        }
    }

    public class RegionHierarchyDto
    {
        public int RegionId { get; set; }
        public string RegionName { get; set; } = string.Empty;
        public double WaterSent { get; set; }
        public double WaterUsed { get; set; }
        public double DailyUsage { get; set; }
        public double NightUsage { get; set; }
        public double LeakagePercent { get; set; }
        public double SpikeUsage { get; set; }
        public List<SubRegionHierarchyDto> SubRegions { get; set; } = new();
    }

    public class SubRegionHierarchyDto
    {
        public int SubRegionId { get; set; }
        public string SubRegionName { get; set; } = string.Empty;
        public double WaterSent { get; set; }
        public double WaterUsed { get; set; }
        public double DailyUsage { get; set; }
        public double NightUsage { get; set; }
        public double LeakagePercent { get; set; }
        public double SpikeUsage { get; set; }
        public List<HouseholdHierarchyDto> Households { get; set; } = new();
    }

    public class HouseholdHierarchyDto
    {
        public string AccountNumber { get; set; } = string.Empty;
        public double DailyUsage { get; set; }
        public double NightUsage { get; set; }
        public double LeakagePercent { get; set; }
        public double SpikeUsage { get; set; }
    }
}
