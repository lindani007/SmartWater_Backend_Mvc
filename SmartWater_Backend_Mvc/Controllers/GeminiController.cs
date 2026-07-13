using Microsoft.AspNetCore.Mvc;
using Smart_Water_Consumption_Service.Services.AiService.DeepLearningModel;
using Smart_Water_Consumption_Service.Services.DbServices;
using SmartWater_Backend_Mvc.Models;
using System.Text.Json;

namespace SmartWater_Backend_Mvc.Controllers
{
    public class GeminiController : Controller
    {
        private readonly RegionConsumptionDbService _regionConsumptionDbService;
        private readonly Sub_RegionConsumptionDbService _subregionConsumptionDbService;
        private readonly HouseConsumptionDbService _houseConsumptionDbService;
        private GeminiCore _geminiCore;
        public GeminiController(RegionConsumptionDbService regionConsumptionDbService1, Sub_RegionConsumptionDbService sub_RegionConsumptionDbService, HouseConsumptionDbService houseConsumptionDbService)
        {
            _houseConsumptionDbService = houseConsumptionDbService;
            _regionConsumptionDbService = regionConsumptionDbService1;
            _subregionConsumptionDbService = sub_RegionConsumptionDbService;
            _geminiCore = new GeminiCore();
        }

        [HttpGet]
        public async Task<IActionResult> SummarizeAdminRegionsReport()
        {
            var chatHistory = new List<ChatMessage>();
            ChatMessage userMassege = new ChatMessage
            {
                IsUser = true,
                Message = "Summarize admin report"
            };
            chatHistory.Add(userMassege);
            chatHistory.Add(new ChatMessage
            {
                IsUser = false,
                Message = @"The regional infrastructure assessment indicates stable operational performance across all monitored regions with no significant leakage currently detected. Water supplied matches water paid in every region, resulting in zero outstanding water losses and no measurable non-revenue water at this reporting interval.

From an engineering perspective, Musgrave (18.19 kL) and Umngeni (17.49 kL) recorded the highest distribution volumes and should remain priority monitoring zones due to their larger infrastructure demand. Durban North, Durban Central, and Brea are operating within expected consumption ranges with no abnormal supply imbalance or leakage indicators observed. Night-time consumption exceeds daytime usage across every region, suggesting expected overnight demand patterns; however, continued monitoring is recommended to detect any future abnormal increases that may indicate hidden leaks.

Financial analysis shows strong revenue performance. Regions exceeding the municipal free-water allocation generated billable revenue, with Umngeni producing approximately R362.33 and Musgrave approximately R255.74. All billed amounts have been fully recovered, leaving no outstanding municipal revenue at risk during this reporting period.

Overall municipal network health is currently classified as Low Risk. No immediate engineering intervention or emergency maintenance deployment is required. The recommended operational strategy is continued predictive monitoring, routine infrastructure inspections, and proactive surveillance of high-volume distribution regions to ensure early detection of future anomalies before they develop into costly infrastructure failures or revenue losses.

Executive Summary

Overall Network Status: Operationally Stable
Infrastructure Risk: Low
Leakage Risk: None Detected
Revenue Collection: 100%
Outstanding Water Loss: 0 kL
Priority Monitoring Regions: Musgrave and Umngeni"
            });
            return PartialView("AiView", chatHistory);




            
        }

        [HttpGet]
        public async Task<IActionResult> SummarizeEngineeringReport()
        {
            var chatHistory = new List<ChatMessage>();
            string engineeringSystemPrompt = @"You are the Chief Hydraulics and Water Infrastructure Engineering Analyst. 

                            ### CONTEXT:
                            The user interface already displays the complete structural data tables, telemetry matrices, and raw metrics. Do NOT recreate data tables, do NOT define simple terms, and do NOT output raw LaTeX equations (like $$ or $ tokens). 

                            ### OBJECTIVE:
                            Provide a concise, formal, high-level engineering interpretation of the data payload. Your response should read as a direct executive brief for field deployment teams who need to know exactly what the data implies for infrastructure integrity.

                            ### KEY PROTOCOLS:
                            - **Conciseness**: Keep the response short, clear, and direct. Focus purely on actionable interpretations.
                            - **Tone**: Strictly professional, formal engineering notation, authoritative, and data-grounded.
                            - **Scope**: Target critical anomalies only (e.g., specific distribution mains vs. terminal line ruptures indicated by night flow anomalies).
                            - **Mandatory Guardrail**: Do NOT mention financial data, currencies, or costs under any circumstances.";

            string userPrompt = "Provide a formal, concise engineering interpretation of the current hydraulic network payload. Highlight the exact structural implications of the highest anomalies and state where field crews must prioritize physical line intervention.";

            try
            {
                chatHistory.Add(new ChatMessage { IsUser = true, Message = "Summarize Engineering Report" });

                var regionsTask = GetRegionConsumptions();
                var subRegionsTask = GetAllSubRegionsConsumption();
                var houseHoldsTask = GetAllHouseHoldConsumption();

                await Task.WhenAll(regionsTask, subRegionsTask, houseHoldsTask);

                var contextData = new
                {
                    HydraulicRegions = regionsTask.Result,
                    HydraulicSubRegions = subRegionsTask.Result,
                    TerminalHouseholds = houseHoldsTask.Result
                };
                string serializedJsonData = JsonSerializer.Serialize(contextData);

                string executionPrompt = $"{engineeringSystemPrompt}\n\n" +
                                         $"### LIVE HYDRAULIC NETWORK CONTEXT PAYLOAD:\n{serializedJsonData}\n\n" +
                                         $"### FIELD AUDIT REQUEST:\n{userPrompt}\n\n" +
                                         $"Generate the professional brief:";

                var response = await _geminiCore.GetGeminiResponse(executionPrompt);
                chatHistory.Add(new ChatMessage { IsUser = false, Message = response });

                return PartialView("AiView", chatHistory);
            }
            catch (Exception ex)
            {
                chatHistory.Add(new ChatMessage { IsUser = false, Message = $"❌ AI Engineering Pipeline Failure: {ex.Message}" });
                return PartialView("AiView", chatHistory);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SummarizeFinanceReport()
        {
            var chatHistory = new List<ChatMessage>();
            string financeSystemPrompt = @"You are the Chief Utility Financial Analyst.
        ### CONTEXT:
        The user interface already displays the high-level data tables. Do NOT recreate raw data tables or define basic accounting terminology. Do NOT output raw LaTeX equations.
        ### OBJECTIVE:
        Provide a concise, formal financial executive brief analyzing total systemic revenue, cost implications of unaccounted water, and financial variances. Focus heavily on billing metrics, outstanding amounts, and the financial health of the overall water network.";

            string userPrompt = "Provide a high-level, concise financial assessment of the current water utility payload. Highlight total financial losses resulting from network inefficiencies and flag immediate budgetary risks.";

            try
            {
                chatHistory.Add(new ChatMessage { IsUser = true, Message = "Summarize Finance Report" });

                var regionsTask = GetRegionConsumptions();
                var subRegionsTask = GetAllSubRegionsConsumption();
                var houseHoldsTask = GetAllHouseHoldConsumption();
                await Task.WhenAll(regionsTask, subRegionsTask, houseHoldsTask);

                var contextData = new { HydraulicRegions = regionsTask.Result, HydraulicSubRegions = subRegionsTask.Result, TerminalHouseholds = houseHoldsTask.Result };
                string serializedData = JsonSerializer.Serialize(contextData);

                string executionPrompt = $"{financeSystemPrompt}\n\n### LIVE NETWORK FINANCIAL PAYLOAD:\n{serializedData}\n\n### EXECUTIVE AUDIT REQUEST:\n{userPrompt}\n\nGenerate the financial brief:";

                var response = await _geminiCore.GetGeminiResponse(executionPrompt);
                chatHistory.Add(new ChatMessage { IsUser = false, Message = response });

                return PartialView("AiView", chatHistory);
            }
            catch (Exception ex)
            {
                chatHistory.Add(new ChatMessage { IsUser = false, Message = $"❌ AI Finance Pipeline Failure: {ex.Message}" });
                return PartialView("AiView", chatHistory);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SummarizeEngineeringRegions()
        {
            var chatHistory = new List<ChatMessage>();
            string engineeringPrompt = @"You are the Lead Hydraulic Infrastructure Engineer. 
        ### CONTEXT:
        Analyzing macro-level regional data ONLY. Do NOT build tables or output raw LaTeX tokens.
        ### OBJECTIVE:
        Provide a concise, formal structural brief interpreting hydraulic anomalies at the macro regional level. Identify high-risk regional zones and main transmission line pressure variations.
        ### GUARDRAIL:
        Do NOT mention financial data, currencies, or costs under any circumstances.";

            string userPrompt = "Analyze the macro hydraulic regions. Identify which primary distribution zones exhibit structural integrity anomalies or severe bulk mass supply imbalances.";

            try
            {
                chatHistory.Add(new ChatMessage { IsUser = true, Message = "Summarize Engineering Regions" });

                var regions = await GetRegionConsumptions();
                string serializedData = JsonSerializer.Serialize(regions);

                string executionPrompt = $"{engineeringPrompt}\n\n### REGIONAL HYDRAULIC PAYLOAD:\n{serializedData}\n\n### FIELD AUDIT REQUEST:\n{userPrompt}\n\nGenerate the brief:";

                var response = await _geminiCore.GetGeminiResponse(executionPrompt);
                chatHistory.Add(new ChatMessage { IsUser = false, Message = response });

                return PartialView("AiView", chatHistory);
            }
            catch (Exception ex)
            {
                chatHistory.Add(new ChatMessage { IsUser = false, Message = $"❌ AI Engineering Regions Failure: {ex.Message}" });
                return PartialView("AiView", chatHistory);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SummarizeEngineeringSubRegions()
        {
            var chatHistory = new List<ChatMessage>();
            string engineeringPrompt = @"You are the Lead Hydraulic Infrastructure Engineer. 
            ### CONTEXT:
            Analyzing sub-regional distribution networks and district metered areas (DMAs) ONLY. Do NOT build tables or output raw LaTeX tokens.
            ### OBJECTIVE:
            Provide a concise, formal engineering summary focusing on sub-regional line weeping, pressurized joint failures, and branch-level telemetry anomalies. 
            ### GUARDRAIL:
            Do NOT mention financial data, currencies, or costs under any circumstances.";

            string userPrompt = "Review the sub-regional network telemetry. Pinpoint exactly which distribution branches or sub-zones show continuous minimum night flow anomalies indicating active physical line leakage.";

            try
            {
                chatHistory.Add(new ChatMessage { IsUser = true, Message = "Summarize Engineering SubRegions" });

                var subRegions = await GetAllSubRegionsConsumption();
                string serializedData = JsonSerializer.Serialize(subRegions);

                string executionPrompt = $"{engineeringPrompt}\n\n### SUB-REGIONAL HYDRAULIC PAYLOAD:\n{serializedData}\n\n### FIELD AUDIT REQUEST:\n{userPrompt}\n\nGenerate the brief:";

                var response = await _geminiCore.GetGeminiResponse(executionPrompt);
                chatHistory.Add(new ChatMessage { IsUser = false, Message = response });

                return PartialView("AiView", chatHistory);
            }
            catch (Exception ex)
            {
                chatHistory.Add(new ChatMessage { IsUser = false, Message = $"❌ AI Engineering Sub-Regions Failure: {ex.Message}" });
                return PartialView("AiView", chatHistory);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SummarizeEngineeringHouseholds()
        {
            var chatHistory = new List<ChatMessage>();
            string engineeringPrompt = @"You are the Lead Hydraulic Infrastructure Engineer. 
        ### CONTEXT:
        Analyzing terminal household consumer connections and end-point accounts ONLY. Do NOT build tables or output raw LaTeX tokens.
        ### OBJECTIVE:
        Provide a concise engineering interpretation regarding consumer-side anomalies, domestic consumption patterns, and localized pipe fractures beyond the municipal meters.
        ### GUARDRAIL:
        Do NOT mention financial data, currencies, or costs under any circumstances.";

            string userPrompt = "Assess the terminal household consumer payload. Identify clear patterns of structural weeping or continuous internal flow anomalies at the account level.";

            try
            {
                chatHistory.Add(new ChatMessage { IsUser = true, Message = "Summarize Engineering Households" });

                var households = await GetAllHouseHoldConsumption();
                string serializedData = JsonSerializer.Serialize(households);

                string executionPrompt = $"{engineeringPrompt}\n\n### HOUSEHOLD CONSUMPTION PAYLOAD:\n{serializedData}\n\n### FIELD AUDIT REQUEST:\n{userPrompt}\n\nGenerate the brief:";

                var response = await _geminiCore.GetGeminiResponse(executionPrompt);
                chatHistory.Add(new ChatMessage { IsUser = false, Message = response });

                return PartialView("AiView", chatHistory);
            }
            catch (Exception ex)
            {
                chatHistory.Add(new ChatMessage { IsUser = false, Message = $"❌ AI Engineering Households Failure: {ex.Message}" });
                return PartialView("AiView", chatHistory);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SummarizeFinanceRegions()
        {
            var chatHistory = new List<ChatMessage>();
            string financePrompt = @"You are the Senior Revenue Analyst for Water Operations.
            ### CONTEXT:
            Analyzing financial data at the macro regional level. Do NOT build tables or output raw LaTeX tokens.
            ### OBJECTIVE:
            Provide a formal, concise financial brief focused on macro regional water billing, bulk distribution revenue, and large-scale non-revenue water (NRW) financial losses. Discuss amounts and revenue generation relative to regional volumetric delivery.";

            string userPrompt = "Evaluate the macro regional consumption data from a financial viewpoint. Which billing zones are causing the largest non-revenue water (NRW) financial deficits?";

            try
            {
                chatHistory.Add(new ChatMessage { IsUser = true, Message = "Summarize Finance Regions" });

                var regions = await GetRegionConsumptions();
                string serializedData = JsonSerializer.Serialize(regions);

                string executionPrompt = $"{financePrompt}\n\n### REGIONAL FINANCIAL PAYLOAD:\n{serializedData}\n\n### AUDIT REQUEST:\n{userPrompt}\n\nGenerate the brief:";

                var response = await _geminiCore.GetGeminiResponse(executionPrompt);
                chatHistory.Add(new ChatMessage { IsUser = false, Message = response });

                return PartialView("AiView", chatHistory);
            }
            catch (Exception ex)
            {
                chatHistory.Add(new ChatMessage { IsUser = false, Message = $"❌ AI Finance Regions Failure: {ex.Message}" });
                return PartialView("AiView", chatHistory);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SummarizeFinanceSubRegions()
        {
            var chatHistory = new List<ChatMessage>();
            string financePrompt = @"You are the Senior Revenue Analyst for Water Operations.
        ### CONTEXT:
        Analyzing sub-regional/district branch metrics. Do NOT build tables or output raw LaTeX tokens.
        ### OBJECTIVE:
        Provide a concise financial interpretation detailing sub-regional billing inefficiencies, unmetered water losses, and localized revenue recovery variances. Frame your fiscal analysis around water distribution volumes.";

            string userPrompt = "Analyze the sub-regional data matrices to uncover direct financial impacts. Pinpoint the specific sub-regions where unmetered flow or leakage is generating critical fiscal deficits.";

            try
            {
                chatHistory.Add(new ChatMessage { IsUser = true, Message = "Summarize Finance SubRegions" });

                var subRegions = await GetAllSubRegionsConsumption();
                string serializedData = JsonSerializer.Serialize(subRegions);

                string executionPrompt = $"{financePrompt}\n\n### SUB-REGIONAL FINANCIAL PAYLOAD:\n{serializedData}\n\n### AUDIT REQUEST:\n{userPrompt}\n\nGenerate the brief:";

                var response = await _geminiCore.GetGeminiResponse(executionPrompt);
                chatHistory.Add(new ChatMessage { IsUser = false, Message = response });

                return PartialView("AiView", chatHistory);
            }
            catch (Exception ex)
            {
                chatHistory.Add(new ChatMessage { IsUser = false, Message = $"❌ AI Finance Sub-Regions Failure: {ex.Message}" });
                return PartialView("AiView", chatHistory);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SummarizeFinanceHouseholds()
        {
            var chatHistory = new List<ChatMessage>();
            string financePrompt = @"You are the Senior Revenue Analyst for Water Operations.
    ### CONTEXT:
    Analyzing terminal household end-point consumption accounts. Do NOT build tables or output raw LaTeX tokens.
    ### OBJECTIVE:
    Provide a concise, formal financial brief regarding consumer billing profiles, outstanding accounts, payment variances, and the direct cash flow impact of household water usage anomalies.";

            string userPrompt = "Audit the terminal household consumption payload from a revenue perspective. Highlight extreme account-level anomalies, outstanding amounts, and customer-side non-payment risks.";

            try
            {
                chatHistory.Add(new ChatMessage { IsUser = true, Message = "Summarize Finance Households" });

                var households = await GetAllHouseHoldConsumption();
                string serializedData = JsonSerializer.Serialize(households);

                string executionPrompt = $"{financePrompt}\n\n### HOUSEHOLD FINANCIAL PAYLOAD:\n{serializedData}\n\n### AUDIT REQUEST:\n{userPrompt}\n\nGenerate the brief:";

                var response = await _geminiCore.GetGeminiResponse(executionPrompt);
                chatHistory.Add(new ChatMessage { IsUser = false, Message = response });

                return PartialView("AiView", chatHistory);
            }
            catch (Exception ex)
            {
                chatHistory.Add(new ChatMessage { IsUser = false, Message = $"❌ AI Finance Households Failure: {ex.Message}" });
                return PartialView("AiView", chatHistory);
            }
        }

        [HttpGet]
        public async Task<IActionResult> QueryDatabasePrompt(string prompt)
        {
            var chatHistory = new List<ChatMessage>();
            try
            {
                if (string.IsNullOrWhiteSpace(prompt))
                {
                    chatHistory.Add(new ChatMessage { IsUser = false, Message = "System Error: Prompt parameter cannot be empty." });
                    return PartialView("AiView", chatHistory);
                }

                chatHistory.Add(new ChatMessage { IsUser = true, Message = prompt });

                string analyticalSystemPrompt = @"You are the Chief Hydraulics and Water Infrastructure Engineering Assistant.

                    ### CONTEXT:
                    The user interface is currently displaying live structural engineering data tables, mass-balance calculations, and telemetry metrics to the operator. Do NOT recreate markdown data tables, do NOT define simple baseline terms, and do NOT output raw LaTeX math equation tokens (such as $$ or $).

                    ### OBJECTIVE:
                    Analyze the user's specific request or query regarding the network infrastructure. Provide a formal, highly concise, and data-grounded response. Focus directly on the engineering, structural, or operational implications of their query.

                    ### PROTOCOLS:
                    1. **Conciseness**: Deliver the assessment in a short, punchy paragraph or highly compact bullet points.
                    2. **Tone**: Strictly professional, authoritative, and structured for engineering teams.
                    3. **Safety Guardrail**: Absolutely do NOT mention financial values, costs, or currency under any circumstances.";

                var regionsTask = GetRegionConsumptions();
                var subRegionsTask = GetAllSubRegionsConsumption();
                var houseHoldsTask = GetAllHouseHoldConsumption();

                await Task.WhenAll(regionsTask, subRegionsTask, houseHoldsTask);

                var contextData = new
                {
                    HydraulicRegions = regionsTask.Result,
                    HydraulicSubRegions = subRegionsTask.Result,
                    TerminalHouseholds = houseHoldsTask.Result
                };
                string serializedJsonData = JsonSerializer.Serialize(contextData);

                string executionPrompt = $"{analyticalSystemPrompt}\n\n" +
                                         $"### LIVE HYDRAULIC NETWORK CONTEXT PAYLOAD:\n{serializedJsonData}\n\n" +
                                         $"### FIELD AUDIT REQUEST:\n{prompt}\n\n" +
                                         $"Generate the professional brief:";

                GeminiCore geminiCore = new GeminiCore();
                var response = await geminiCore.GetGeminiResponse(executionPrompt);
                chatHistory.Add(new ChatMessage { IsUser = false, Message = response });

                return PartialView("AiView", chatHistory);
            }
            catch (Exception ex)
            {
                chatHistory.Add(new ChatMessage { IsUser = false, Message = $"❌ AI Engineering Pipeline Failure: {ex.Message}" });
                return PartialView("AiView", chatHistory);
            }
        }
        private async Task<List<RegionConsumption>> GetRegionConsumptions()
        {
            var response = await _regionConsumptionDbService.GetAllRegionsConsumptions();
            var regions = response.Data.Select(x => new RegionConsumption
            {
                Region_Id = x.Region_Id,
                Water_Sent = x.Water_Sent.Value,
                Water_Used = x.Water_Payed.Value,
                Water_Thats_OutStanding = x.OutstandingWater.Value,
                Daily_Usage = x.Daily_Usage,
                Night_Usage = x.Night_Usage,
                Spike_Usage = x.Spike_Usage,
                Leakage_Propability = x.Leakage_Percent.Value,
                Consumption_Month = x.Date.Value,
                FreeWater = x.FreeWater,
                Region_Name = x.Region_Name,
            }).ToList();
            return regions;
        }
        private async Task<List<SubRegion>> GetAllSubRegionsConsumption()
        {
            var response = await _subregionConsumptionDbService.GetAllSub_RegionConsumption();
            var subregions = response.Data.Select(x => new SubRegion
            {
                Daily_Usage = x.Daily_Usage,
                Night_Usage = x.Night_Usage,
                Spike_Usage = x.Spike_Usage,
                Leakage_Propability = x.Leakage_Percent.Value,
                SubRegion_Id = x.Sub_RegionId,
                Region_Id = x.RegionId,
                SubRegion_Name = x.Sub_RegionName,
                Water_Sent = x.Water_Sent.Value,
                Water_Used = x.Water_Payed.Value,
                Water_Thats_OutStanding = x.OutstandingWater.Value,
                FreeWater = x.FreeWater,
                Consumption_Month = x.Date.Value
            }).ToList();
            return subregions;
        }
        private async Task<List<HouseHold>> GetAllHouseHoldConsumption(int subregionId)
        {
            var subregions = await GetAllSubRegionsConsumption();
            var subregion = subregions.FirstOrDefault(x => x.SubRegion_Id == subregionId);
            var response = await _houseConsumptionDbService.GetAllHouseConsumption();
            var houses = response.Data.Select(x => new HouseHold
            {
                Daily_Usage = x.Daily_Usage,
                Night_Usage = x.Night_Usage,
                Spike_Usage = x.Spike_Usage,
                Leakage_Propability = x.Leakage_Percent.Value,
                SubRegion_Id = x.Sub_RegionId.Value,
                Household_Id = x.Household_ID.Length,
                SubRegion_Name = subregion.SubRegion_Name,
                Water_Received = x.Water_Received.Value,
                Consumption_Month = x.Date.Value,
                Account_Number = x.Owner_AccountNumber,
                FreeWater = x.FreeWater
            }).ToList();
            return houses;
        }
        private async Task<List<HouseHold>> GetAllHouseHoldConsumption()
        {
            var response = await _houseConsumptionDbService.GetAllHouseConsumption();
            var houses = response.Data.Select(x => new HouseHold
            {
                Daily_Usage = x.Daily_Usage,
                Night_Usage = x.Night_Usage,
                Spike_Usage = x.Spike_Usage,
                Leakage_Propability = x.Leakage_Percent.Value,
                SubRegion_Id = x.Sub_RegionId.Value,
                Household_Id = x.Household_ID.Length,
                Water_Received = x.Water_Received.Value,
                Consumption_Month = x.Date.Value,
                Account_Number = x.Owner_AccountNumber,
                FreeWater = x.FreeWater
            }).ToList();
            return houses;
        }
    }
}
