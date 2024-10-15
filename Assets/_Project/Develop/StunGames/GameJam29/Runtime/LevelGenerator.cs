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
        [SerializeField] private int emptyPositionsFromExit = 0; // Минимальное количество незанятых позиций сетки от выхода (0 - выход может появиться в сторону соседней существующей комнаты)
        [SerializeField] private Transform startGridPoint; // Стартовая точка таблицы
        [SerializeField] private Room roomPrefab; // Префаб комнаты
        [SerializeField] private RoomConnector connectorPrefab;
        private List<Room> _allRooms = new List<Room>(); // Список всех созданных комнат
        private Dictionary<Vector2Int, Room> _roomsByGridPosition = new Dictionary<Vector2Int, Room>(); // Словарь для хранения комнат по их позициям в сетке
        private Dictionary<Room, Vector2Int> _gridPositionsByRoom = new Dictionary<Room, Vector2Int>(); // Словарь для хранения позиций в сетке по занятым ими комнатам

        public List<Room> AllRooms => _allRooms;


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
                    availableGridPositions.Remove(gridPosition); // Если в этой стороне уже есть комната то удаляем её из списка доступных сторон
                    if (_roomsByGridPosition[gridPosition].ConnectedRooms.Contains(_roomsByGridPosition[currentGridPosition])) continue; // Пропускаем если комнаты уже соединены
                    if (Random.Range(1, 100) < _gameConfig.RoomConnectChance) // Или соединяем с заданной вероятностью
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
                        
                        // Заканчиваем создание если достигли нужного количества комнат
                        if (_allRooms.Count >= _gameConfig.RoomsOnLevel) break;
                    }
                }
            }

            // Наполняем комнаты предметами
            FillRooms();
            // Debug.Log("AllRooms.Count = " + AllRooms.Count);
        }

        public void ClearLevel()
        {
            _roomsByGridPosition.Clear();
            _gridPositionsByRoom.Clear();
            foreach (Room room in _allRooms)
            {
                Destroy(room.gameObject);
            }
            _allRooms.Clear();
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
            Room newRoom = Instantiate(roomPrefab, GetWorldPosition(gridPosition), Quaternion.identity, transform);
            _allRooms.Add(newRoom);
            newRoom.gameObject.name = $"Room_{_allRooms.Count}";
            _roomsByGridPosition[gridPosition] = newRoom; // Сохраняем комнату по ее позиции
            _gridPositionsByRoom[newRoom] = gridPosition; // Сохраняем позицию по занятой ею комнате
        }
        
        private void ConnectRooms(Room originRoom, Room targetRoom)
        {
            RoomConnector newConnector = Instantiate(connectorPrefab, originRoom.transform);
            originRoom.ConnectedRooms.Add(targetRoom);
            targetRoom.ConnectedRooms.Add(originRoom);
            originRoom.RoomConnectors.Add(newConnector);
            targetRoom.RoomConnectors.Add(newConnector);
            newConnector.Connect(originRoom, targetRoom);
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
            Dictionary<Room, List<ExitPosition>> emptyExitAvailableRooms = GetExitAvailableRooms(emptyRooms); // Вычленяем крайние свободные комнаты и получаем пары комната - доступные выходы (края)
            
            int randomRoomIndex = Random.Range(0, emptyExitAvailableRooms.Count); // Выбираем случайную крайнюю свободную комнату
            Room exitRoom = emptyExitAvailableRooms.ElementAt(randomRoomIndex).Key; // Получаем комнату
            List<ExitPosition> exitPositions = emptyExitAvailableRooms.ElementAt(randomRoomIndex).Value; // Получаем выходы (края) комнаты
            
            int randomExitPositionIndex = Random.Range(0, exitPositions.Count); // Выбираем случайный выход (край) комнаты
            ExitPosition exitPosition = exitPositions[randomExitPositionIndex]; // Получаем выход (край) комнаты
            
            exitRoom.Configure(ItemType.None, false, exitPosition); // Инициализируем (конфигурируем) содержимое комнаты
            emptyRooms.Remove(exitRoom); // Удаляем комнату из списка свободных
        }
        
        // Метод для получения крайних комнат
        private Dictionary<Room, List<ExitPosition>> GetExitAvailableRooms(List<Room> rooms)
        {
            Dictionary<Room, List<ExitPosition>> exitAvailableRooms = new Dictionary<Room, List<ExitPosition>>();

            foreach (Room room in rooms)
            {
                Vector2Int gridPosition = _gridPositionsByRoom[room]; // Получаем координаты комнаты в сетке
                List<ExitPosition> availableExitPositions = new List<ExitPosition>(); // Создаём список для возможных расположений выхода в комнате
                if (IsExitPathClear(room, gridPosition, Vector2Int.up)) availableExitPositions.Add(ExitPosition.Up); // Проверяем вверх
                if (IsExitPathClear(room, gridPosition, Vector2Int.down)) availableExitPositions.Add(ExitPosition.Down); // Проверяем вниз
                if (IsExitPathClear(room, gridPosition, Vector2Int.left)) availableExitPositions.Add(ExitPosition.Left); // Проверяем влево
                if (IsExitPathClear(room, gridPosition, Vector2Int.right)) availableExitPositions.Add(ExitPosition.Right); // Проверяем вправо
                
                if (availableExitPositions.Count > 0) exitAvailableRooms.Add(room, availableExitPositions); // Если был найден край, добавляем крайнюю комнату
            }
            
            return exitAvailableRooms;
        }

        // Метод для проверки, свободен ли путь от комнаты в указанном направлении
        private bool IsExitPathClear(Room originRoom, Vector2Int originRoomGridPosition, Vector2Int direction)
        {
            Vector2Int currentGridPosition = originRoomGridPosition;

            for (int i = 0; i <= emptyPositionsFromExit; i++)
            {
                currentGridPosition += direction; // Двигаемся в указанном направлении
                if (!IsValidPosition(currentGridPosition)) return true;; // Если достигли границы без препятствий, путь свободен
                
                // Если не достигли границы
                if (emptyPositionsFromExit == 0) // и минимальное расстояние равно нулю (выход может спавниться в сторону смежных комнат)
                {
                    if (_roomsByGridPosition.ContainsKey(currentGridPosition)) // Если соседняя позиция занята
                        if (_roomsByGridPosition[currentGridPosition].ConnectedRooms.Contains(originRoom)) // и если туда ведёт коридор
                            return false; // Путь не свободен
                }
                else // иначе, если минимальное расстояние больше 0 (выход НЕ может спавниться в сторону смежных комнат)
                {
                    if (_roomsByGridPosition.ContainsKey(currentGridPosition)) return false; // Если позиция занята, то путь не свободен
                }
            }
            
            return true; // Если на минимальном пути не нашли препятствий, то путь свободен
        }

        private void OnDrawGizmos()
        {
            if (!_gameConfig.drawLevelGizmos) return;
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