using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private int gridWidth = 4; // ширина сетки уровня
        [SerializeField] private int gridHeight = 4; // высота сетки уровня
        [SerializeField] private float xMultiplier = 28.0f; // разница координат по оси X
        [SerializeField] private float yMultiplier = 18.0f; // разница координат по оси Y
        [SerializeField] private Transform startGridPoint; // Стартовая точка таблицы
        [SerializeField] private int numberOfRooms = 9; // количество комнат на уровень
        [SerializeField] private int numberOfAlarms = 1; // количество сигнализаций на уровень
        [SerializeField] private int numberOfLamps = 1; // количество ламп на уровень
        [SerializeField] private int numberOfTerminals = 1; // количество терминалов на уровень
        [SerializeField] private int numberOfHealth = 2; // количество здоровья (дополнительных кликов) на уровень
        [SerializeField] private Room roomPrefab; // Префаб комнаты
        
        private List<Room> _allRooms = new List<Room>(); // Список всех созданных комнат
        private Dictionary<Vector2Int, Room> _roomsByGridPosition = new Dictionary<Vector2Int, Room>(); // Словарь для хранения комнат по их позициям в сетке
        
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
            while (_allRooms.Count < numberOfRooms && roomGridPositionsList.Count > 0)
            {
                // Берем случайную комнату из доступных
                int currentGridPositionIndex = Random.Range(0, roomGridPositionsList.Count);
                Vector2Int currentGridPosition = roomGridPositionsList[currentGridPositionIndex];
                
                // Проверяем её стороны
                List<Vector2Int> availableGridPositions = GetAvailableGridPositions(currentGridPosition);
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
            position.y >= 0 && position.y < gridHeight &&
            !_roomsByGridPosition.ContainsKey(position);
        
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
        }
        
        private void ConnectRooms(Room room1, Room room2)
        {
            room1.ConnectedRooms.Add(room2);
            room2.ConnectedRooms.Add(room1);
        }

        private void FillRooms()
        {
            List<Room> emptyRooms = new List<Room>(_allRooms);
            int items = numberOfAlarms + numberOfHealth + numberOfLamps + numberOfTerminals + 1;
            if (emptyRooms.Count < items) Debug.LogWarning($"Количество комнат ({emptyRooms.Count}) меньше чем предметов ({items})! Лишние предметы не будут расставлены!");;
            
            FillRoom(emptyRooms, ItemType.None, false, true); // выход
            FillRoom(emptyRooms, ItemType.Key, false, false); // ключ
            for (int i = 0; i < numberOfHealth; i++) FillRoom(emptyRooms, ItemType.Health, false, false); // здоровье
            for (int i = 0; i < numberOfTerminals; i++) FillRoom(emptyRooms, ItemType.Terminal, false, false); // терминалы
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

        private void OnDrawGizmos()
        {
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