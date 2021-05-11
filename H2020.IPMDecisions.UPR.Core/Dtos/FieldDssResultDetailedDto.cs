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
        public List<int> WarningStatusPerDay { get; set; }
        public string Interval { get; set; }
        public string DssDescription { get; set; }
        public string DssTypeOfOutput { get; set; }
        public string DssTypeOfDecision { get; set; }
        public string DssDescriptionUrl { get; set; }
        public int ResultParametersLength { get; set; }
        public int ResultParametersWidth { get; set; }
    }

    public class ResultParameters
    {
        private List<double> data = new List<double>();
        private List<string> labels = new List<string>();
        public ResultParameters()
        {
            Data = data;
            Labels = labels;
        }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<double> Data { get; set; }
        public List<string> Labels { get; set; }
        public DssParameterChartInformation ChartInformation { get; set; }
    }

    public class DssParameterChartInformation
    {
        public bool DefaultVisible { get; set; }
        public string Unit { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
    }
}