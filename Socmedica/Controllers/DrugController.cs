using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Socmedica.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class DrugController : ControllerBase
    {
        private const string apiKey = "a2469ddfb5d573d3";

        private readonly HttpClient httpClient = new HttpClient();

        
        private readonly ILogger<DrugController> _logger;

        public DrugController(ILogger<DrugController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<DrugControllerResponse> Post([FromBody] DrugControllerRequest request)
        {
            DrugControllerResponse ret = new DrugControllerResponse();
            ret.DrugIntersections = new List<DrugIntersectionData>();
            List<Drug> drugs = new List<Drug>();

            //get drug data
            foreach(var drug in request.Names)
            {
                var drugInfo = await GetDrugInfo(drug);
                if (drugInfo != null && !drugs.Where(x => x.Id == drugInfo.Id).Any())
                {
                    drugs.Add(drugInfo);
                    if(drugInfo.ActiveIngridients == null)
                        ret.ErrorMessage += "Не удалось получить данные о действущем веществе препарата " + drug + Environment.NewLine;
                    else if(drugInfo.AnatomicalTherapeuticChemicalClassification == null)
                        ret.ErrorMessage += "Не удалось получить данные об АТХ препарата " + drug + Environment.NewLine;
                }
                else if (drugInfo == null)
                    ret.ErrorMessage += "Не удалось запросить данные о препарате " + drug + Environment.NewLine;
            }
            //calculate intersections
            DrugIntersectionData intersection = new DrugIntersectionData();
            for(int i=0; i<drugs.Count-1;i++)
            {
                if (drugs[i].ActiveIngridients == null) 
                    continue;
                for (int j = i + 1; j < drugs.Count; j++)
                {
                    if (drugs[j].ActiveIngridients == null)
                        continue;

                    var chemIntersections = drugs[i].ActiveIngridients.Join(drugs[j].ActiveIngridients, x => x.Key, y => y.Key, (x, y) => x.Value).ToList();
                    if (drugs[i].AnatomicalTherapeuticChemicalClassification != null && drugs[j].AnatomicalTherapeuticChemicalClassification != null)
                        chemIntersections.AddRange(drugs[i].AnatomicalTherapeuticChemicalClassification.Join(drugs[j].AnatomicalTherapeuticChemicalClassification, x => x.Key, y => y.Key, (x, y) => x.Value).ToList());
                    if (chemIntersections.Count > 0)
                    {
                        intersection.Names = new List<string> { drugs[i].Name, drugs[j].Name };
                        intersection.Intersections = chemIntersections;
                        ret.DrugIntersections.Add(intersection);
                        intersection = new DrugIntersectionData();
                    }

                }
            }

            return ret;
        }

        private async Task<Drug> GetDrugInfo(string drudName)
        {
            Thread.Sleep(200);
            Drug ret = new Drug();
            var drugId = await GetDrugId(drudName);
            if (drugId == null)
                return null;
            ret.Id = drugId.Value.id;
            ret.Name = drugId.Value.name;
            //Костыль чтоб билинг не зарубил запрос
            Thread.Sleep(200);

            ret.ActiveIngridients = await GetActiveIngredients(ret.Id);
            if (ret.ActiveIngridients == null)
                return ret;
            ret.AnatomicalTherapeuticChemicalClassification = new Dictionary<uint, string>();
            //По плану тут был Parallel.Foreach, однако биллинговая система не позволила мне этого сделать =(
            foreach (var x in ret.ActiveIngridients) {
                //Костыль чтоб билинг не зарубил запрос
                Thread.Sleep(200);
                var atcc = await GetAnatomicalTherapeuticChemicalClassification(x.Key);
                if (atcc != null)
                {
                    lock (ret.AnatomicalTherapeuticChemicalClassification)
                    {
                        foreach (var record in atcc)
                        {
                            if(!ret.AnatomicalTherapeuticChemicalClassification.ContainsKey(record.Key))
                                ret.AnatomicalTherapeuticChemicalClassification.Add(record.Key, record.Value);
                        }
                    }
                }
            } 
            return ret;
        }

        private async Task<(uint id, string name)?> GetDrugId(string drugName)
        {
            Core.Models.ApiRequestData requestData = new Core.Models.ApiRequestData();
            requestData.Key = apiKey;
            requestData.Lib = new List<ushort> { 62 };
            requestData.Binarization = 0;
            requestData.Level = new List<ushort> { 1600 };
            requestData.Route = 2;
            requestData.Deepness = 0;
            requestData.Text = drugName;
            requestData.Valmore = false;

            var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(requestData));
            HttpContent content = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            var resp = await httpClient.PostAsync("https://cs.socmedica.com/api/umkb/getsemantic", content);

            if (resp.Content == null)
                return null;

            var responseContent = await resp.Content.ReadAsStringAsync();
            Core.Models.ApiResponseData responseData = JsonConvert.DeserializeObject<Core.Models.ApiResponseData>(responseContent);
            //get the trademark's node
            if (responseData.Graph == null)
                return null;
            var tm = responseData.Graph.FirstOrDefault();
            if (tm == null)
                return null;
            return (tm.Id, responseData.Names[tm.Id]);
        }

        private async Task<Dictionary<uint,string>> GetAnatomicalTherapeuticChemicalClassification(uint activeIngredientId)
        {
            Core.Models.ApiRequestData requestData = new Core.Models.ApiRequestData();
            requestData.Key = apiKey;
            requestData.Lib = new List<ushort> { 62 };
            requestData.Binarization = 0;
            requestData.Level = new List<ushort> { 1621 };
            requestData.Route = 0;
            requestData.Deepness = 1;
            requestData.LibId = new List<string> { activeIngredientId.ToString() };
            requestData.Valmore = false;

            var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(requestData));
            HttpContent content = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            var resp = await httpClient.PostAsync("https://cs.socmedica.com/api/umkb/getsemantic", content);

            if (resp.Content == null)
                return null;

            var responseContent = await resp.Content.ReadAsStringAsync();
            Core.Models.ApiResponseData responseData = JsonConvert.DeserializeObject<Core.Models.ApiResponseData>(responseContent);
            //get attached nodes' names
            Dictionary<uint, string> ret = new Dictionary<uint, string>();
            if (responseData.Graph == null)
                return null;
            responseData.Graph.ForEach(x =>
            {
                if (x.Level == 1621)
                {
                    if (x.IdA == activeIngredientId && !ret.ContainsKey(x.IdB))
                        ret.Add(x.IdB, responseData.Names[x.IdB]);
                    if (x.IdB == activeIngredientId && !ret.ContainsKey(x.IdA))
                        ret.Add(x.IdA, responseData.Names[x.IdA]);
                }
            });
            return ret;
        }
        private async Task<Dictionary<uint,string>> GetActiveIngredients(uint drugId)
        {
            Core.Models.ApiRequestData requestData = new Core.Models.ApiRequestData();
            requestData.Key = apiKey;
            requestData.Lib = new List<ushort> { 62 };
            requestData.Binarization = 0;
            requestData.Level = new List<ushort> { 1600 };
            requestData.Route = 2;
            requestData.Deepness = 1;
            requestData.LibId = new List<string> { drugId.ToString() };
            requestData.Valmore = false;

            var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(requestData));
            HttpContent content = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            var resp = await httpClient.PostAsync("https://cs.socmedica.com/api/umkb/getsemantic", content);
            
            if (resp.Content == null)
                return null;

            var responseContent = await resp.Content.ReadAsStringAsync();
            Core.Models.ApiResponseData responseData = JsonConvert.DeserializeObject<Core.Models.ApiResponseData>(responseContent);
            //get the trademark's node
            var tm = responseData.Graph.FirstOrDefault(x => x.IdA == 0 && x.IdB == 0);
            if (tm == null)
                return null;
            //get active ingredients
            Dictionary<uint, string> ret = new Dictionary<uint, string>();
            responseData.Graph.ForEach(x =>
            {
                if (x.Level == 1600)
                {
                    if(x.IdA == drugId)
                        ret.Add(x.IdB, responseData.Names[x.IdB]);
                    if (x.IdB == drugId)
                        ret.Add(x.IdA, responseData.Names[x.IdA]);
                }
            });
            return ret;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            return "working!";
        }
    }
}
