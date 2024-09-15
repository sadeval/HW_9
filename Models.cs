using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TrainStationApp
{
    public class Station
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public ICollection<Train> Trains { get; set; } = new List<Train>();
    }

    public class Train
    {
        public int Id { get; set; }
        public string? Model { get; set; }
        public int Age { get; set; } // Возраст в годах
        public double RouteDuration { get; set; } // Длительность маршрута в часах
        public int StationId { get; set; }
        public Station? Station { get; set; }
    }
}
