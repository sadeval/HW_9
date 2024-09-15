using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TrainStationApp
{
    public class StationInfoDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Model { get; set; }
    }

    public class BusyStationDto
    {
        public string? Name { get; set; }
    }

    class Program
    {
        static void Main()
        {
            using (var context = new ApplicationContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // 1. Добавление данных про станции и поезда
                context.Database.ExecuteSqlRaw(@"
                    INSERT INTO Stations (Name) VALUES ('Station A'), ('Station B'), ('Station C');
                    INSERT INTO Trains (Model, Age, RouteDuration, StationId) 
                    VALUES ('Pell 200', 10, 3.5, 1), ('Rock 300', 16, 5.5, 1), ('Pell 400', 5, 6, 2);
                ");
                Console.WriteLine("Данные про станции и поезда успешно добавлены.\n");

                // 2. Поезда, у которых длительность маршрута более 5 часов
                var longRouteTrains = context.Trains
                    .FromSqlRaw("SELECT * FROM Trains WHERE RouteDuration > 5")
                    .ToList();
                Console.WriteLine("Поезда с длительностью маршрута более 5 часов:");
                foreach (var train in longRouteTrains)
                {
                    Console.WriteLine($"Модель: {train.Model}, Длительность маршрута: {train.RouteDuration} часов");
                }
                Console.WriteLine();

                // 3. Общая информация о станции и ее поездах
                var stationInfos = context.Stations
                    .FromSqlRaw(@"
                        SELECT s.Id, s.Name, t.Model 
                        FROM Stations s 
                        LEFT JOIN Trains t ON s.Id = t.StationId
                    ")
                    .Select(x => new StationInfoDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Model = x.Model
                    })
                    .ToList();

                Console.WriteLine("Общая информация о станциях и их поездах:");
                foreach (var info in stationInfos)
                {
                    Console.WriteLine($"Станция: {info.Name}, Поезд: {info.Model ?? "Нет поездов"}");
                }
                Console.WriteLine();

                // 4. Название станций, у которых в наличии более 3 поездов
                var busyStations = context.Stations
                    .FromSqlRaw(@"
                        SELECT s.Name 
                        FROM Stations s 
                        JOIN Trains t ON s.Id = t.StationId
                        GROUP BY s.Name 
                        HAVING COUNT(t.Id) > 3
                    ")
                    .Select(s => new BusyStationDto { Name = s.Name })
                    .ToList();

                Console.WriteLine("Станции с более чем 3 поездами:");
                foreach (var station in busyStations)
                {
                    Console.WriteLine($"Станция: {station.Name}");
                }
                Console.WriteLine();

                // 5. Все поезда, модель которых начинается на подстроку «Pell»
                var pellTrains = context.Trains
                    .FromSqlRaw("SELECT * FROM Trains WHERE Model LIKE 'Pell%'")
                    .ToList();
                Console.WriteLine("Поезда, модель которых начинается на подстроку 'Pell':");
                foreach (var train in pellTrains)
                {
                    Console.WriteLine($"Модель: {train.Model}");
                }
                Console.WriteLine();

                // 6. Все поезда, у которых возраст более 15 лет с текущей даты
                var oldTrains = context.Trains
                    .FromSqlRaw("SELECT * FROM Trains WHERE Age > 15")
                    .ToList();
                Console.WriteLine("Поезда, у которых возраст более 15 лет:");
                foreach (var train in oldTrains)
                {
                    Console.WriteLine($"Модель: {train.Model}, Возраст: {train.Age} лет");
                }
                Console.WriteLine();

                // 7. Получение станций с хотя бы одним поездом с длительностью маршрута менее 4 часов
                var fastRouteStations = context.Stations
                    .FromSqlRaw(@"
                        SELECT DISTINCT s.Id, s.Name
                        FROM Stations s
                        JOIN Trains t ON s.Id = t.StationId
                        WHERE t.RouteDuration < 4
                    ")
                    .Select(s => new BusyStationDto { Name = s.Name })
                    .ToList();

                Console.WriteLine("Станции с поездами, у которых длительность маршрута менее 4 часов:");
                foreach (var station in fastRouteStations)
                {
                    Console.WriteLine($"Станция: {station.Name}");
                }
                Console.WriteLine();

                // 8. Все станции без поездов
                var stationsWithoutTrains = context.Stations
                    .FromSqlRaw(@"
                        SELECT s.Id, s.Name
                        FROM Stations s 
                        LEFT JOIN Trains t ON s.Id = t.StationId
                        WHERE t.Id IS NULL
                    ")
                    .Select(s => new BusyStationDto { Name = s.Name })
                    .ToList();

                Console.WriteLine("Станции без поездов:");
                foreach (var station in stationsWithoutTrains)
                {
                    Console.WriteLine($"Станция: {station.Name}");
                }
                Console.WriteLine();
            }
        }
    }
}
