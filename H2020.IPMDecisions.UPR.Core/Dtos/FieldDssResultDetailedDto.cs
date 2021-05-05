using System;
using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldDssResultDetailedDto : FieldDssResultBaseDto
    {
        private List<ResultParameters> resultParameters = new List<ResultParameters>();

        public FieldDssResultDetailedDto()
        {
            ResultParameters = resultParameters;
        }

        public List<ResultParameters> ResultParameters { get; set; }
        public string OutputTimeStart { get; set; }
        public string OutputTimeEnd { get; set; }
        public string Interval { get; set; }
    }

    public class ResultParameters
    {
        private List<double> data = new List<double>();
        public ResultParameters()
        {
            Data = data;
        }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<double> Data { get; set; }
    }
}