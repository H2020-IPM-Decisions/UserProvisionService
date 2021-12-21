using System;
using System.Text.RegularExpressions;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.BLL.Helpers
{
    public static class DssDataHelper
    {
        public static string AddDefaultDatesToDssJsonInput(string jsonAsString, int? currentYear = -1)
        {
            if (currentYear == -1) currentYear = DateTime.Today.Year;

            Regex currentYearRegex = new Regex(@"[{]+[\scurrent_year]+[}]+",
               RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            if (currentYearRegex.IsMatch(jsonAsString))
            {
                jsonAsString = currentYearRegex.Replace(jsonAsString, currentYear.ToString());
            }
            Regex previousYearRegex = new Regex(@"[{]+[\sprevious_year]+[}]+",
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            if (previousYearRegex.IsMatch(jsonAsString))
            {
                jsonAsString = previousYearRegex.Replace(jsonAsString, (currentYear - 1).ToString());
            }
            return jsonAsString;
        }

        public static int GetCurrentYearForDssDefaultDates(DssModelInformation dssInputInformation)
        {
            // ToDo Get current year based on weather
            /**
            The next step will be to ensure that when the model is first selected, that the CURRENT_YEAR is set appropriately.
            So lets assume that DATE  = the current date (i.e. TODAY),CURRENT_YEAR is year of current date, END_DATE is the last date for which we require weather data & START_DATE is the first date we require weather for – we can then implement the following process:
            •	Check If DATE > END_DATE 
                o	If YES - set CURRENT_YEAR = DATE.Year + 1
                    	Check If DATE < START_DATE
                        •	If yes - weather data will not currently be available for next season so cannot run model
                            [we might need to pass a message to UI to indicate that model cannot be run as outside model range]
                        •	If no – do nothing as we are in new growing season.
            o	If no – do nothing as CURRENT_YEAR does not need to change

            **/
            return -1;
        }
    }
}