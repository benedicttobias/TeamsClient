using System;

namespace Model.Weather
{
    public class WeatherForecastModel
    {
        public DayForecastModel[] Consolidated_weather { get; set; }
        public DateTime Sun_rise { get; set; }
        public DateTime Sun_set { get; set; }
        public string Title { get; set; }
        public string Timezone { get; set; }
    }

    public class DayForecastModel
    {
        public string weather_state_name { get; set; }
        public string applicable_date { get; set; }
        public float min_temp { get; set; }
        public float max_temp { get; set; }
    }
}
