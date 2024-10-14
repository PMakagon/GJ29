using System.Collections.Generic;
using UnityEngine;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    public static class RoomExtensions
    {
        // Метод для обхода в ширину. distances - пары комната - расстяние до исходной комнаты, visited - набор посещённых комнат
        private static (Dictionary<Room, int> distances, HashSet<Room> visited) BreadthFirstSearch(
            Room startRoom, // начальная комната
            Room targetRoom = null, // искомая комната (null означает что мы анализируем все комнаты)
            int maxSearchableDistance = -1) // максимальное расстояние которое мы анализируем (на котором ищем комнаты)
        {
            Queue<Room> queue = new Queue<Room>(); // Очередь в которую добавляются комнаты для обхода
            HashSet<Room> visited = new HashSet<Room>(); // Посещённые (помеченные) комнаты
            Dictionary<Room, int> distances = new Dictionary<Room, int>(); // Пары комната - расстяние до исходной комнаты

            queue.Enqueue(startRoom); // Добавляем стартовую комнату в очередь для обхода
            visited.Add(startRoom); // Помечаем стартовую комнату как посещённую
            distances[startRoom] = 0; // Записываем расстояние до стартовой позиции

            while (queue.Count > 0)
            {
                Room current = queue.Dequeue();
                int currentDistance = distances[current];
                
                // Если у нас есть целевая комната и мы ее достигли, выходим из метода
                if (current == targetRoom) return (distances, visited);
                
                // Если максимальное расстояние указано и мы его достигли, прекращаем обрабатывать дальше
                if (maxSearchableDistance != -1 && currentDistance >= maxSearchableDistance) continue; // и переходим к следующей комнате в очереди

                foreach (Room neighbor in current.ConnectedRooms)
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                        distances[neighbor] = currentDistance + 1; // Увеличиваем расстояние на 1
                    }
                }
            }

            return (distances, visited);
        }

        // Метод для получения кратчайшего пути между комнатами
        public static Queue<Room> GetShortestPath(this Room currentRoom, Room targetRoom)
        {
            Stack<Room> shortestPathStack = new Stack<Room>(); // Стек для записи короткого пути (работаем в процессе)
            Dictionary<Room, int> distances; // Пары комната - расстяние до исходной комнаты
            HashSet<Room> visited; // Посещённые (помеченные) комнаты
            (distances, visited) = BreadthFirstSearch(currentRoom, targetRoom); // Производим поиск в ширину целевой комнаты 

            if (!visited.Contains(targetRoom)) return new Queue<Room>(); // Если комната недостижима возвращаем пустую очередь

            // Восстанавливаем путь
            Room current = targetRoom; // Начинаем с конца
            while (current != currentRoom) // Пока не достигли исходной комнаты
            {
                shortestPathStack.Push(current); // Добавляем в путь текущую комнату
                foreach (Room neighbor in current.ConnectedRooms) // Проверяем соседей
                {
                    if (!distances.ContainsKey(neighbor)) continue; // Если соседа не обработали (значит он дальше), пропускаем шаг 
                    if (distances[neighbor] + 1 == distances[current]) // Если сосед ближе к исходной комнате, то переходим к нему
                    {
                        current = neighbor;
                        break;
                    }
                }
            }
            shortestPathStack.Push(currentRoom); // Добавляем начальную комнату

            Queue<Room> shortestPathQueue = new Queue<Room>(shortestPathStack); // Создаем очередь короткого пути (для выдачи результата)

            return shortestPathQueue;
        }

        // Метод для получения кратчайшего расстояния между комнатами
        public static int GetShortestDistance(this Room currentRoom, Room targetRoom)
        {
            Dictionary<Room, int> distances; // Пары комната - расстяние до исходной комнаты
            HashSet<Room> visited; // Посещённые (помеченные) комнаты
            (distances, visited) = BreadthFirstSearch(currentRoom, targetRoom); // Производим поиск в ширину целевой комнаты 
            return visited.Contains(targetRoom) ? distances[targetRoom] : -1; // Возвращаем расстояние. Если комната недостижимо, вернуть -1
        }

        // Метод для получения списка всех комнат, расположенных строго на заданном расстоянии (опционально с учетом количества присоеденённых комнат)
        public static List<Room> GetRoomsAtDistance(this Room currentRoom, int distance, int connectedRoomsCount = 1)
        {
            Dictionary<Room, int> distances; // Пары комната - расстяние до исходной комнаты
            (distances, _) = BreadthFirstSearch(currentRoom, null, distance); // Производим поиск в ширину
            List<Room> roomsAtDistance = new List<Room>(); // Спиоск найденных комнат

            foreach (var pair in distances)
                if (pair.Value == distance && pair.Key.ConnectedRooms.Count >= connectedRoomsCount)
                    roomsAtDistance.Add(pair.Key);

            return roomsAtDistance;
        }

        // Метод для получения списка всех комнат, расположенных дальше заданного расстояния (опционально с учетом количества присоеденённых комнат)
        public static List<Room> GetRoomsBeyondDistance(this Room currentRoom, int distance, int connectedRoomsCount = 1)
        {
            Dictionary<Room, int> distances; // Пары комната - расстяние до исходной комнаты
            (distances, _) = BreadthFirstSearch(currentRoom); // Производим поиск в ширину
            List<Room> roomsBeyondDistance = new List<Room>(); // Спиоск найденных комнат

            foreach (var pair in distances)
                if (pair.Value > distance && pair.Key.ConnectedRooms.Count >= connectedRoomsCount)
                    roomsBeyondDistance.Add(pair.Key);

            return roomsBeyondDistance;
        }

        // Метод для получения списка всех комнат, в пределах заданного расстояния (опционально с учетом количества присоеденённых комнат)
        public static List<Room> GetRoomsWithinDistance(this Room currentRoom, int distance, int connectedRoomsCount = 1)
        {
            Dictionary<Room, int> distances; // Пары комната - расстяние до исходной комнаты
            (distances, _) = BreadthFirstSearch(currentRoom, null, distance); // Производим поиск в ширину
            List<Room> rooms = new List<Room>(); // Спиоск найденных комнат

            foreach (var pair in distances)
                if (pair.Value <= distance && pair.Key.ConnectedRooms.Count >= connectedRoomsCount)
                    rooms.Add(pair.Key);

            return rooms;
        }
    }
}