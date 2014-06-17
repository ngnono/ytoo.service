﻿using com.intime.fashion.service;
using com.intime.fashion.service.analysis;
using com.intime.fashion.service.search;
using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Model.ES;
using Yintai.Hangzhou.Model.ESModel;

namespace com.intime.jobscheduler.Job.Analysis
{

    [DisallowConcurrentExecution]
    class PullAnalysisData2ESJob : IJob
    {
        
        public void Execute(IJobExecutionContext context)
        {
            ILog log = LogManager.GetLogger(this.GetType());

            JobDataMap data = context.JobDetail.JobDataMap;

            var interval = data.ContainsKey("intervalOfDays") ? data.GetInt("intervalOfDays") : 1;
            if (!data.ContainsKey("benchtime"))
            {
                data.Put("benchtime", DateTime.Now.AddDays(-interval));
            }
            else
            {
                data["benchtime"] = data.GetDateTimeValue("benchtime").AddDays(-interval);
            }
            var benchTime = data.GetDateTime("benchtime");

            var sw = new Stopwatch();
            var analysisClient = new AnalysisService(benchTime);
            using (var slt = new ScopedLifetimeDbContextManager())
            {
    
                ESAnalysisSummary summary = analysisClient.GetSummary();
                bool isIndexedSummary = true;
                if (!SearchLogic.IndexSingle<ESAnalysisSummary>(summary))
                    isIndexedSummary = false;
                log.Info(string.Format("index analysis summary:{0}", isIndexedSummary));
    
                bool isIndexedCombos = true;
                analysisClient.ThreholdControl();
                IEnumerable<ESAnalysisCombo> combos = analysisClient.GetComboEvent();
                if (!SearchLogic.IndexMany<ESAnalysisCombo>(combos))
                    isIndexedCombos = false;
                log.Info(string.Format("index analysis combos:{0}", isIndexedCombos));

                bool isIndexedGiftCard = true;
                analysisClient.ThreholdControl();
                IEnumerable<ESAnalysisGiftCard> gfs = analysisClient.GetGiftCardEvent();
                if (!SearchLogic.IndexMany<ESAnalysisGiftCard>(gfs))
                    isIndexedGiftCard = false;
                log.Info(string.Format("index analysis giftcards:{0}", isIndexedGiftCard));

                bool isIndexedStores = true;
                analysisClient.ThreholdControl();
                IEnumerable<ESAnalysisStore> stores = analysisClient.GetStoreEvent();
                if (!SearchLogic.IndexMany<ESAnalysisStore>(stores))
                    isIndexedStores = false;
                log.Info(string.Format("index analysis stores:{0}", isIndexedStores));
            }
            sw.Stop();
            
        }
    }
}
