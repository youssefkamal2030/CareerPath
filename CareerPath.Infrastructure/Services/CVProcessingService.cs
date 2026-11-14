using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto.CVAnalysis;

namespace CareerPath.Infrastructure.Services
{
    public class CVProcessingService : ICVProcessingService
    {
        public Task<CVAnalysisResponse> AnalyzeCVAsync(CVAnalysisRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ExtractedCVDataDto?> GetExtractedCVDataAsync(string analysisId)
        {
            throw new NotImplementedException();
        }
    }
}
