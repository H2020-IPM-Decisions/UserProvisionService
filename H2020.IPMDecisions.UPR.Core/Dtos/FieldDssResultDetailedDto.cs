using System;
using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldDssResultDetailedDto : FieldDssResultBaseDto
    {
        private List<ResultParameters> resultParameters = new List<ResultParameters>();
        private List<ChartGroup> chartGroups = new List<ChartGroup>();

        public FieldDssResultDetailedDto()
        {
            ResultParameters = resultParameters;
            ChartGroups = chartGroups;
        }

        public List<ResultParameters> ResultParameters { get; set; }
        public string OutputTimeStart { get; set; }
        public string OutputTimeEnd { get; set; }
        public List<int> WarningStatusPerDay { get; set; }
        public List<string> WarningStatusLabels { get; set; }
        public string Interval { get; set; }
        public string DssTypeOfOutput { get; set; }
        public string DssTypeOfDecision { get; set; }
        public int ResultParametersLength { get; set; }
        public int ResultParametersWidth { get; set; }
        public List<ChartGroup> ChartGroups { get; set; }
    }

    public class ChartGroup
    {
        private List<ResultParameters> resultParameters = new List<ResultParameters>();
        public ChartGroup()
        {
            ResultParameters = resultParameters;
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public List<string> ResultParameterIds { get; set; }
        public List<ResultParameters> ResultParameters { get; set; }
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
        public string ChartType { get; set; }
        public string Color { get; set; }
    }
}