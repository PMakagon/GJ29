using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private int gridWidth = 4; // ширина сетки уровня
        [SerializeField] private int gridHeight = 4; // высота сетки уровня
        [SerializeField] private float xMultiplier = 28.0f; // разница координат по оси X
        [SerializeField] private float yMultiplier = 18.0f; // разница координат по оси Y
        [SerializeField] private float roomConnectChance = 15.0f; // Вероятность соединения с уже существующей комнатой, от 0 до 100%
        [SerializeField] private Transform startGridPoint; // Стартовая точка таблицы
        [SerializeField] private Room roomPrefab; // Префаб комнаты
        private List<Room> _allRooms = new List<Room>(); // Список всех созданных комнат
        private Dictionary<Vector2Int, Room> _roomsByGridPosition = new Dictionary<Vector2Int, Room>(); // Словарь для хранения комнат по их позициям в сетке
        private Dictionary<Room, Vector2Int> _gridPositionsByRoom = new Dictionary<Room, Vector2Int>(); // Словарь для хранения позиций в сетке по занятым ими комнатам

        public List<Room> AllRooms => _allRooms;
        
        public bool drawGizmos = false;


        public void GenerateLevel()
        {
            // Создаем список обрабатываемых комнат (доступных для ответвлений) 
            List<Vector2Int> roomGridPositionsList = new List<Vector2Int>();
            
            // Создаем стартовую коанмту
            Vector2Int startGridPosition = GetRandomGridPosition();
            CreateRoom(startGridPosition);
            roomGridPositionsList.Add(startGridPosition);
            
            // Создаем остальные комнаты, ответвления 
            while (_allRooms.Count < _gameConfig.RoomsOnLevel && roomGridPositionsList.Count > 0)
            {
                // Берем случайную комнату из доступных
                int currentGridPositionIndex = Random.Range(0, roomGridPositionsList.Count);
                Vector2Int currentGridPosition = roomGridPositionsList[currentGridPositionIndex];
                
                // Проверяем её стороны
                List<Vector2Int> availableGridPositions = GetAvailableGridPositions(currentGridPosition);
                
                // Если сторона уже занята другой комнатой, то с заданной вероятностью строим к ней проход
                foreach (var gridPosition in availableGridPositions.ToList().Where(gridPosition => _roomsByGridPosition.ContainsKey(gridPosition)))
                {
                    availableGridPositions.Remove(gridPosition);
                    if (Random.Range(1, 100) < roomConnectChance)
                    {
                        ConnectRooms(_roomsByGridPosition[currentGridPosition], _roomsByGridPosition[gridPosition]); 
                    }
                }
                
                if (availableGridPositions.Count <= 0)
                {
                    // Если все стороны заняты то удаляем позицию комнаты из списка 
                    roomGridPositionsList.RemoveAt(currentGridPositionIndex);
                }
                else
                {
                    // Иначе выбираем случайное количество (от 1 до 4) доступных новых сторон и создаём от них новые комнаты
                    int numberOfConnectors = Random.Range(1, Random.Range(1, availableGridPositions.Count));
                    for (int i = 0; i < numberOfConnectors; i++)
                    {
                        // Выбор случайной стороны для создания комнаты
                        int positionIndex = Random.Range(0, availableGridPositions.Count);
                        Vector2Int newGridPosition = availableGridPositions[positionIndex];

                        // Создание комнаты
                        CreateRoom(newGridPosition);
                        ConnectRooms(_roomsByGridPosition[currentGridPosition], _roomsByGridPosition[newGridPosition]); 
                        roomGridPositionsList.Add(newGridPosition);

                        // Удаление стороны из списка доступных
                        availableGridPositions.RemoveAt(positionIndex);
                    }
                }
            }

            // Наполняем комнаты предметами
            FillRooms();
            Debug.Log("AllRooms.Count = " + AllRooms.Count);
        }
        
        private Vector2Int GetRandomGridPosition() => new Vector2Int(
            Random.Range(0, gridWidth), 
            Random.Range(0, gridHeight));

        private Vector3 GetWorldPosition(Vector2Int gridPosition) => new Vector3(
            gridPosition.x * xMultiplier + startGridPoint.position.x, 
            gridPosition.y * yMultiplier + startGridPoint.position.y, 
            0);

        private bool IsValidPosition(Vector2Int position) =>
            position.x >= 0 && position.x < gridWidth &&
            position.y >= 0 && position.y < gridHeight;
        
        private List<Vector2Int> GetAvailableGridPositions(Vector2Int currentPosition)
        {
            List<Vector2Int> availablePositions = new List<Vector2Int>();
            if (IsValidPosition(currentPosition + Vector2Int.right)) availablePositions.Add(currentPosition + Vector2Int.right); // вправо
            if (IsValidPosition(currentPosition + Vector2Int.left)) availablePositions.Add(currentPosition + Vector2Int.left); // влево
            if (IsValidPosition(currentPosition + Vector2Int.up)) availablePositions.Add(currentPosition + Vector2Int.up); // вверх
            if (IsValidPosition(currentPosition + Vector2Int.down)) availablePositions.Add(currentPosition + Vector2Int.down); // вниз
            return availablePositions;
        }
        
        private void CreateRoom(Vector2Int gridPosition)
        {
            Room newRoom = Instantiate(roomPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
            _allRooms.Add(newRoom);
            _roomsByGridPosition[gridPosition] = newRoom; // Сохраняем комнату по ее позиции
            _gridPositionsByRoom[newRoom] = gridPosition; // Сохраняем позицию по занятой ею комнате
        }
        
        private void ConnectRooms(Room room1, Room room2)
        {
            room1.ConnectedRooms.Add(room2);
            room2.ConnectedRooms.Add(room1);
        }

        private void FillRooms()
        {
            List<Room> emptyRooms = new List<Room>(_allRooms);
            int items = _gameConfig.AlarmsOnLevel + _gameConfig.HpPacksCount + _gameConfig.LampsOnLevel + _gameConfig.TerminalsCount + 1;
            if (emptyRooms.Count < items) Debug.LogWarning($"Количество комнат ({emptyRooms.Count}) меньше чем предметов ({items})! Лишние предметы не будут расставлены!");;

            FillExitRoom(emptyRooms); // выход
            FillRoom(emptyRooms, ItemType.Key, false); // ключ
            for (int i = 0; i < _gameConfig.HpPacksCount; i++) FillRoom(emptyRooms, ItemType.Health, false); // здоровье
            for (int i = 0; i < _gameConfig.TerminalsCount; i++) FillRoom(emptyRooms, ItemType.Terminal, false); // терминалы
            for (int i = 0; i < _gameConfig.AlarmsOnLevel; i++) FillRoom(emptyRooms, ItemType.None, true); // сигнализации
            for (int i = 0; i < _gameConfig.LampsOnLevel; i++) FillRoom(emptyRooms, ItemType.Lamp, false); // лампы
            while (emptyRooms.Count > 0) FillRoom(emptyRooms, ItemType.None, false); // пустышки
        }

        private void FillRoom(List<Room> emptyRooms, ItemType item, bool isAlarmable)
        {
            if (emptyRooms.Count <= 0) return; // Если свободных комнат нет - ничего не делаем
            int randomRoomIndex = Random.Range(0, emptyRooms.Count); // Выбираем случайную свободную комнату
            emptyRooms[randomRoomIndex].Configure(item, isAlarmable, ExitPosition.None); // Инициализируем (конфигурируем) содержимое комнаты
            emptyRooms.RemoveAt(randomRoomIndex); // Удаляем комнату из списка свободных
        }

        private void FillExitRoom(List<Room> emptyRooms)
        {
            Dictionary<Room, List<ExitPosition>> emptyEdgeRooms = GetEdgeRooms(emptyRooms); // Вычленяем крайние свободные комнаты и получаем пары комната - доступные выходы (края)
            
            int randomRoomIndex = Random.Range(0, emptyEdgeRooms.Count); // Выбираем случайную крайнюю свободную комнату
            Room exitRoom = emptyEdgeRooms.ElementAt(randomRoomIndex).Key; // Получаем комнату
            List<ExitPosition> exitPositions = emptyEdgeRooms.ElementAt(randomRoomIndex).Value; // Получаем выходы (края) комнаты
            
            int randomExitPositionIndex = Random.Range(0, exitPositions.Count); // Выбираем случайный выход (край) комнаты
            ExitPosition exitPosition = exitPositions[randomExitPositionIndex]; // Получаем выход (край) комнаты
            
            exitRoom.Configure(ItemType.None, false, exitPosition); // Инициализируем (конфигурируем) содержимое комнаты
            emptyRooms.Remove(exitRoom); // Удаляем комнату из списка свободных
        }
        
        // Метод для получения крайних комнат
        private Dictionary<Room, List<ExitPosition>> GetEdgeRooms(List<Room> rooms)
        {
            Dictionary<Room, List<ExitPosition>> edgeRooms = new Dictionary<Room, List<ExitPosition>>();

            foreach (Room room in rooms)
            {
                Vector2Int gridPosition = _gridPositionsByRoom[room]; // Получаем координаты комнаты в сетке
                List<ExitPosition> availableExitPositions = new List<ExitPosition>(); // Создаём список для возможных расположений выхода в комнате
                if (IsPathToEdgeClear(gridPosition, Vector2Int.up)) availableExitPositions.Add(ExitPosition.Up); // Проверяем вверх
                if (IsPathToEdgeClear(gridPosition, Vector2Int.down)) availableExitPositions.Add(ExitPosition.Down); // Проверяем вниз
                if (IsPathToEdgeClear(gridPosition, Vector2Int.left)) availableExitPositions.Add(ExitPosition.Left); // Проверяем влево
                if (IsPathToEdgeClear(gridPosition, Vector2Int.right)) availableExitPositions.Add(ExitPosition.Right); // Проверяем вправо
                
                if (availableExitPositions.Count > 0) edgeRooms.Add(room, availableExitPositions); // Если был найден край, добавляем крайнюю комнату
            }
            
            return edgeRooms;
        }

        // Метод для проверки, свободен ли путь от комнаты до границы в указанном направлении
        private bool IsPathToEdgeClear(Vector2Int gridPosition, Vector2Int direction)
        {
            Vector2Int currentGridPosition = gridPosition;
            while (true)
            {
                currentGridPosition += direction; // Двигаемся в указанном направлении до границы уровня
                if (!IsValidPosition(currentGridPosition)) return true;; // Если достигли границы без препятствий, путь свободен
                if (_roomsByGridPosition.ContainsKey(currentGridPosition)) return false; // Если находим занятую позицию, то путь не свободен
            }
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;
            Gizmos.color = Color.green;
            for (int i = 0; i < gridWidth; i++)
            for (int j = 0; j < gridHeight; j++)
                Gizmos.DrawCube(GetWorldPosition(new Vector2Int(i, j)), new Vector3(20f, 12f, 0.1f));
        }
        
        [ContextMenu("GenerateGrid")]
        private void GenerateGrid()
        {
            for (int i = 0; i < gridWidth; i++)
            for (int j = 0; j < gridHeight; j++)
            {
                Instantiate(roomPrefab, GetWorldPosition(new Vector2Int(i, j)), Quaternion.identity);
            }
        }
    }
}