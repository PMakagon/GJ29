using System.Collections.Generic;
using UnityEngine;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private int gridWidth = 3; // ширина сетки уровня
        [SerializeField] private int gridHeight = 3; // высота сетки уровня
        [SerializeField] private int numberOfRooms = 7; // количество комнат на уровень
        [SerializeField] private int numberOfAlarms = 1; // количество сигнализаций на уровень
        [SerializeField] private int numberOfLamps = 1; // количество ламп на уровень
        [SerializeField] private int numberOfTerminals = 1; // количество терминалов на уровень
        [SerializeField] private int numberOfHealth = 2; // количество здоровья (дополнительных кликов) на уровень
        [SerializeField] private GameObject roomPrefab; // Префаб комнаты
        [SerializeField] private RoomConnector connectorPrefab; // Префаб коридора

        private List<Room> _allRooms;
        private HashSet<Vector3> _occupiedPositions;

        private void Start()
        {
            _allRooms = new List<Room>();
            _occupiedPositions = new HashSet<Vector3>();
            GenerateLevel();
        }

        private void GenerateLevel()
        {
            // Очередь из комнат которым необходимо создать коннекторы
            Queue<Room> roomQueue = new Queue<Room>();
            // Очередь из комнат оставшихся со свободными коннекторами (нужна для тупиковых случаев)
            Queue<Room> freeRoomQueue = new Queue<Room>();

            // Создаем первую комнату
            Vector3 startPosition = new Vector3(Random.Range(0, gridWidth), 0, Random.Range(0, gridHeight));
            Room startRoom = CreateRoom(startPosition);
            _allRooms.Add(startRoom);
            roomQueue.Enqueue(startRoom);
            _occupiedPositions.Add(startPosition);
            
            // Генерация остальных комнат
            while (roomQueue.Count > 0 && _allRooms.Count < numberOfRooms)
            {
                Room currentRoom = GetNextRoom(roomQueue, freeRoomQueue);
                if (currentRoom == null) break; // Если комнаты закончились - выходим
                
                List<Vector3> availablePositions = GetAvailablePositions(currentRoom.transform.position);
                if (availablePositions.Count == 0) continue; // Если некуда пристроить коннектор то скипаем 
                int numberOfConnectors = Random.Range(1, availablePositions.Count); // Иначе создаем от 1 до 4 новых коннекторов
                if (numberOfConnectors < availablePositions.Count) freeRoomQueue.Enqueue(startRoom); // Если остались слоты - сохраняем

                for (int i = 0; i < numberOfConnectors; i++)
                {
                    int positionIndex = Random.Range(0, availablePositions.Count - 1); // Выбор случайного коннектора
                    Room newRoom = CreateRoom(availablePositions[positionIndex]);
                    ConnectRooms(currentRoom, newRoom);
                    roomQueue.Enqueue(newRoom);
                    _occupiedPositions.Add(availablePositions[positionIndex]);
                    availablePositions.RemoveAt(positionIndex);
                }
            }

            // Наполняем комнаты предметами
            FillRooms();
        }
        
        private Room CreateRoom(Vector3 position)
        {
            var newRoom = Instantiate(roomPrefab, position, Quaternion.identity);
            Room room = newRoom.GetComponent<Room>(); // Заменить сразу на Room
            return room;
        }

        private void ConnectRooms(Room room1, Room room2)
        {
            RoomConnector newConnector = Instantiate(connectorPrefab, (room1.transform.position + room2.transform.position) / 2, Quaternion.identity);
            room1.RoomConnectors.Add(newConnector);
            room2.RoomConnectors.Add(newConnector);
            newConnector.Connect(room1, room2);
            room1.ConnectedRooms.Add(room2);
            room2.ConnectedRooms.Add(room1);
        }

        private bool IsValidPosition(Vector3 position)
        {
            return position.x >= 0 && position.x < gridWidth &&
                   position.z >= 0 && position.z < gridHeight &&
                   !_occupiedPositions.Contains(position);
        }
        
        private Room GetNextRoom(Queue<Room> roomQueue, Queue<Room> freeRoomQueue)
        {
            if (roomQueue.Count > 0) return roomQueue.Dequeue();
            if (freeRoomQueue.Count > 0) return freeRoomQueue.Dequeue();
            return null; // Если ни одной комнаты нет, возвращаем null
        }
        
        private List<Vector3> GetAvailablePositions(Vector3 currentPosition)
        {
            List<Vector3> availablePositions = new List<Vector3>();
            if (IsValidPosition(currentPosition + Vector3.right)) availablePositions.Add(currentPosition + Vector3.right); // вправо
            if (IsValidPosition(currentPosition + Vector3.left)) availablePositions.Add(currentPosition + Vector3.left); // влево
            if (IsValidPosition(currentPosition + Vector3.forward)) availablePositions.Add(currentPosition + Vector3.forward); // вверх
            if (IsValidPosition(currentPosition + Vector3.back)) availablePositions.Add(currentPosition + Vector3.back); // вниз
            return availablePositions;
        }

        private void FillRooms()
        {
            List<Room> emptyRooms = _allRooms;
            int items = numberOfAlarms + numberOfHealth + numberOfLamps + numberOfTerminals + 1;
            if (emptyRooms.Count < items) Debug.LogWarning($"Количество комнат ({emptyRooms.Count}) меньше чем предметов ({items})! Лишние предметы не будут расставлены!");;
            
            FillRoom(emptyRooms, ItemType.None, false, true); // выход
            FillRoom(emptyRooms, ItemType.Key, false, false); // ключ
            for (int i = 0; i < numberOfTerminals; i++) FillRoom(emptyRooms, ItemType.Terminal, false, false); // терминалы
            for (int i = 0; i < numberOfHealth; i++) FillRoom(emptyRooms, ItemType.Health, false, false); // здоровье
            for (int i = 0; i < numberOfAlarms; i++) FillRoom(emptyRooms, ItemType.None, true, false); // сигнализации
            for (int i = 0; i < numberOfLamps; i++) FillRoom(emptyRooms, ItemType.Lamp, false, false); // лампы
            while (emptyRooms.Count > 0) FillRoom(emptyRooms, ItemType.None, false, false); // пустышки
        }

        private void FillRoom(List<Room> emptyRooms, ItemType item, bool isAlarmable, bool hasExit)
        {
            if (emptyRooms.Count <= 0) return;
            int randomRoomIndex = Random.Range(0, emptyRooms.Count);
            emptyRooms[randomRoomIndex].Configure(item, isAlarmable, hasExit);
            emptyRooms.RemoveAt(randomRoomIndex);
        }
    }
}