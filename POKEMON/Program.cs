using System;
using System.Security.Cryptography.X509Certificates;
using static Pokemon;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class Pokemon
{
    const int MapWidth = 40;
    const int MapHeight = 40;

    char[,] map = new char[MapWidth, MapHeight];

    int playerX = 0;
    int playerY = 0;

    const char grassSymbol = 'o';
    const char collisionSymbol = '#';

    public void InitializeMap(char[,] customMap)
    {
        if (customMap.GetLength(0) != MapWidth || customMap.GetLength(1) != MapHeight)
        {
            Console.WriteLine("El mapa personalizado no tiene las dimensiones correctas. Se utilizará un mapa predeterminado.");
            InitializeDefaultMap();
            return;
        }

        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                map[x, y] = customMap[x, y];
                if (map[x, y] == '@')
                {
                    playerX = x;
                    playerY = y;
                }
            }
        }
    }

    private void InitializeDefaultMap()
    {
        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                map[x, y] = '.';
            }
        }

        playerX = MapWidth / 2;
        playerY = MapHeight / 2;
        map[playerX, playerY] = '@';
    }

    public void PrintMap()
    {
        Console.Clear();
        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                Console.Write(map[x, y] + " ");
            }
            Console.WriteLine();
        }
    }

    public void MovePlayerUp()
    {
        if (playerY > 0 && map[playerX, playerY - 1] != collisionSymbol)
        {
            if (map[playerX, playerY - 1] != grassSymbol)
            {
                map[playerX, playerY] = '.';
            }
            else
            {
                map[playerX, playerY] = grassSymbol;
            }
            playerY--;
            ProcessInteraction();
            map[playerX, playerY] = '@';
        }
    }

    public void MovePlayerDown()
    {
        if (playerY < MapHeight - 1 && map[playerX, playerY + 1] != collisionSymbol)
        {
            if (map[playerX, playerY + 1] != grassSymbol)
            {
                map[playerX, playerY] = '.';
            }
            else
            {
                map[playerX, playerY] = grassSymbol;
            }
            playerY++;
            ProcessInteraction();
            map[playerX, playerY] = '@';
        }
    }

    public void MovePlayerLeft()
    {
        if (playerX > 0 && map[playerX - 1, playerY] != collisionSymbol)
        {
            if (map[playerX - 1, playerY] != grassSymbol)
            {
                map[playerX, playerY] = '.';
            }
            else
            {
                map[playerX, playerY] = grassSymbol;
            }
            playerX--;
            ProcessInteraction();
            map[playerX, playerY] = '@';
        }
    }

    public void MovePlayerRight()
    {
        if (playerX < MapWidth - 1 && map[playerX + 1, playerY] != collisionSymbol)
        {
            if (map[playerX + 1, playerY] != grassSymbol)
            {
                map[playerX, playerY] = '.';
            }
            else
            {
                map[playerX, playerY] = grassSymbol;
            }
            playerX++;
            ProcessInteraction();
            map[playerX, playerY] = '@';
        }
    }

    public void MovePlayer(char direction)
    {
        switch (direction)
        {
            case 'w':
            case 'W':
                MovePlayerUp();
                break;
            case 's':
            case 'S':
                MovePlayerDown();
                break;
            case 'a':
            case 'A':
                MovePlayerLeft();
                break;
            case 'd':
            case 'D':
                MovePlayerRight();
                break;
            default:
                Console.WriteLine("Tecla no válida.");
                break;
        }
    }
    private void ProcessInteraction()
    {
        if (map[playerX, playerY] == grassSymbol)
        {
            CheckForEncounter();
        }
    }

    internal void CheckForEncounter()
    {
        Random random = new Random();
        int encounterChance = random.Next(1, 101);

        // Hay un 15% de probabilidades de encontrar un objeto en la hierba alta
        if (encounterChance <= 15)
        {
            Console.WriteLine("¡Has encontrado un objeto en la hierba alta!");

            // Crear el Pokémon del jugador
            Pokemons playerPokemon = Program.pokemonSeleccionado; // O llama a la función que crea al Pokémon del jugador

            // Crear un Pokémon enemigo aleatorio
            Pokemons enemyPokemon = ChooseRandomEnemyPokemon();

            // Iniciar la batalla
            MoveToBattleZone(playerPokemon, enemyPokemon);
        }
    }
    public class Pokemons
    {
        public string Nombre { get; set; }
        public int Nivel { get; set; }
        public int HP { get; set; }
        public int Speed { get; set; }
        public string[] Sprite { get; set; }
        public Movimiento[] Movimientos { get; set; }

        public Pokemons(string nombre, int nivel, int hp, int speed, string[] sprite)
        {
            Nombre = nombre;
            Nivel = nivel;
            HP = hp;
            Speed = speed;
            Sprite = sprite;
            Movimientos = new Movimiento[4]; // Inicializar el arreglo con tamaño 4
        }
        public void RecibirDanio(int danio)
        {
            HP -= danio;
            if (HP < 0)
                HP = 0;
        }

        public bool RealizarAtaque(Movimiento movimiento, Pokemons oponente)
        {
            // Calcular la probabilidad de acierto basada en la diferencia de velocidad
            double probabilidadAcierto = (double)Speed / oponente.Speed;

            // Generar un número aleatorio entre 0 y 1 para determinar si el ataque acierta
            Random random = new Random();
            double randomNumber = random.NextDouble();

            // Si el número aleatorio es menor que la probabilidad de acierto, el ataque falla
            if (randomNumber > probabilidadAcierto)
            {
                Console.WriteLine($"{Nombre} ha intentado usar {movimiento.Nombre} pero ha fallado!");
                return false;
            }

            // Si el ataque acierta, puedes implementar aquí la lógica para calcular el daño
            int danio = CalcularDanio(movimiento, oponente);
            oponente.RecibirDanio(danio);

            Console.WriteLine($"{Nombre} ha usado {movimiento.Nombre} y ha causado {danio} de daño a {oponente.Nombre}!");

            return true;
        }

        private int CalcularDanio(Movimiento movimiento, Pokemons oponente)
        {
            return (movimiento.Potencia * Nivel) / 10;
        }
    }

    public class Movimiento
    {
        public string Nombre { get; set; }
        public int Potencia { get; set; }
        public double Precision { get; set; }

        public Movimiento(string nombre, int potencia, double precision)
        {
            Nombre= nombre;
            Potencia= potencia;
            Precision = precision;
        }
    }
    private Pokemons ChooseRandomEnemyPokemon()
    {
        Random random = new Random();
        int randomIndex = random.Next(4); // Genera un número aleatorio entre 0 y 3

        switch (randomIndex)
        {
            case 0:
                return Program.Pikachu();
            case 1:
                return Program.Rattata();
            case 2:
                return Program.Psyduck();
            case 3:
                return Program.Squirtle();
            default:
                throw new InvalidOperationException("Índice aleatorio fuera de rango.");
        }
    }   

    private void MoveToBattleZone(Pokemons playerPokemon, Pokemons enemyPokemon)
    {
        bool ejecutado = false;

        if (!ejecutado)
        {
            Console.Clear();
            ejecutado = true;
        }
        if (playerPokemon == null)
        {
            Console.WriteLine("¡No se ha seleccionado ningún Pokémon para el jugador!");
            return;
        }
        Console.WriteLine("¡Un Pokémon salvaje aparece!");
        Console.WriteLine($"Es un {enemyPokemon.Nombre} salvaje.");

        while (true)
        {
            // Turno del jugador
            Console.WriteLine($"{playerPokemon.Nombre}, ¡es tu turno!");

            // Mostrar los movimientos disponibles del Pokémon del jugador
            Console.WriteLine("Elige un movimiento:");
            for (int i = 0; i < playerPokemon.Movimientos.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {playerPokemon.Movimientos[i].Nombre}");
            }

            // Leer la selección del jugador
            int selectedMoveIndex;
            do
            {
                Console.Write("Selecciona un movimiento: ");
            } while (!int.TryParse(Console.ReadLine(), out selectedMoveIndex) ||
                     selectedMoveIndex < 1 ||
                     selectedMoveIndex > playerPokemon.Movimientos.Length);

            // Realizar el ataque del jugador
            Movimiento selectedMove = playerPokemon.Movimientos[selectedMoveIndex - 1];
            if (playerPokemon.RealizarAtaque(selectedMove, enemyPokemon))
            {
                // Verificar si el Pokémon oponente quedó fuera de combate
                if (enemyPokemon.HP <= 0)
                {
                    Console.WriteLine($"{enemyPokemon.Nombre} ha sido derrotado.");
                    break;
                }
            }

            // Turno del Pokémon oponente
            Console.WriteLine($"{enemyPokemon.Nombre} ataca.");

            // Elegir un movimiento aleatorio para el Pokémon oponente
            Random random = new Random();
            Movimiento enemyMove = enemyPokemon.Movimientos[random.Next(enemyPokemon.Movimientos.Length)];

            // Realizar el ataque del Pokémon oponente
            if (enemyPokemon.RealizarAtaque(enemyMove, playerPokemon))
            {
                // Verificar si el Pokémon del jugador quedó fuera de combate
                if (playerPokemon.HP <= 0)
                {
                    Console.WriteLine($"{playerPokemon.Nombre} ha sido derrotado.");
                    break;
                }
            }
        }
    }
}
public class Program
{
    internal static Pokemons pokemonSeleccionado;
    static void Main(string[] args)
    {
        // Menú de selección de Pokémon
        Console.WriteLine("Bienvenido al mundo Pokémon!");
        Console.WriteLine("Selecciona tu Pokémon:");
        Console.WriteLine("1. Pikachu");
        Console.WriteLine("2. Rattata");
        Console.WriteLine("3. Psyduck");
        Console.WriteLine("4. Squirtle");

        // Obtener selección del usuario
        int seleccion = Convert.ToInt32(Console.ReadLine());

        // Crear Pokémon según la selección del usuario
        switch (seleccion)
        {
            case 1:
                pokemonSeleccionado = Pikachu();
                break;
            case 2:
                pokemonSeleccionado = Rattata();
                break;
            case 3:
                pokemonSeleccionado = Psyduck();
                break;
            case 4:
                pokemonSeleccionado = Squirtle();
                break;
            default:
                Console.WriteLine("Selección no válida. Seleccionando Pikachu por defecto.");
                pokemonSeleccionado = Pikachu(); // Opción predeterminada: Pikachu
                break;
        }

        // Continuar con el Pokémon seleccionado
        Console.WriteLine($"Has seleccionado a {pokemonSeleccionado.Nombre}.");

        // Llamar a la función para cargar el mapa principal
        CargarMapaPrincipal();

    // Método para cargar el mapa principal
    static void CargarMapaPrincipal()
    {
        Console.WriteLine("¡Bienvenido al mapa principal!");
            Pokemon game = new Pokemon();
            Console.Clear();

            char[,] customMap = new char[,]
            {
            { '.', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '.', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '.', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '.', '#', '#', '#', '#', '#', '#', '#', '#', '.', '.', '#', '.', '.', '.', '.', '.', '.', '.', '.', 'o', '#', 'o', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '.', '#', '#', '#', '#', '#', '#', '#', '#', '.', '.', '#', '.', '.', '.', '.', '.', '.', '.', '.', 'o', '#', 'o', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '.', '.', 'o', '#', '#', '#', '#', '#', '#', '.', '.', '#', '.', 'o', 'o', 'o', 'o', '.', '.', '#', 'o', '#', 'o', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '.', '#', 'o', '#', '#', '#', '#', '#', '#', '.', '.', '.', '.', 'o', 'o', 'o', 'o', '.', '.', '#', 'o', '#', 'o', '#', 'o', 'o', '#', 'o', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', 'o', '#', '#', '#', '#', '#', '#', '.', '.', 'o', '.', 'o', 'o', 'o', 'o', '.', '.', '#', 'o', 'o', 'o', '#', 'o', 'o', '#', 'o', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', 'o', '#', '#', '#', '#', '#', '#', '.', '.', '.', '.', 'o', 'o', 'o', 'o', '.', '.', '#', '#', 'o', 'o', '#', 'o', 'o', '#', 'o', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', 'o', 'o', 'o', 'o', '#', '#', '#', '.', '.', '#', '.', 'o', 'o', 'o', 'o', '.', '.', '.', '#', 'o', 'o', '#', 'o', 'o', '#', 'o', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', 'o', 'o', 'o', 'o', '#', 'o', '#', '.', '.', '#', '.', 'o', 'o', 'o', 'o', '.', '.', '.', '#', '#', 'o', '#', 'o', '.', 'o', 'o', '.', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', 'o', 'o', 'o', 'o', '#', 'o', 'o', '.', '.', '#', '.', '.', '.', '.', '.', '.', '.', '.', '.', '#', 'o', '#', 'o', 'o', '#', 'o', '.', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', 'o', 'o', 'o', 'o', 'o', 'o', 'o', '.', '.', '#', '.', '.', '.', '.', '.', '.', '.', '.', '.', '#', 'o', '#', '.', '.', '#', '.', 'o', '#', '.', '.', '.', '.', '.', '.', '.', '#', '#', '#'},
            { '#', '#', 'o', 'o', 'o', 'o', 'o', 'o', 'o', '.', '#', '#', '#', '#', '#', '#', '#', '#', '#', '.', '#', '#', 'o', '#', '#', '#', '#', '#', '.', '#', 'o', 'o', 'o', 'o', 'o', 'o', 'o', '#', '#', '#'},
            { 'o', '#', 'o', 'o', 'o', 'o', '#', 'o', 'o', 'o', 'o', 'o', 'o', 'o', 'o', 'o', 'o', 'o', '#', '.', '#', '.', 'o', '.', 'o', '.', '.', '.', '.', '.', 'o', '.', 'o', 'o', '.', 'o', '.', '#', '#', '#'},
            { 'o', '#', 'o', 'o', 'o', 'o', '#', 'o', '.', '#', 'o', 'o', 'o', 'o', 'o', 'o', 'o', '#', '#', '.', '#', '.', '.', 'o', 'o', '.', 'o', 'o', '.', '.', 'o', '.', 'o', 'o', '.', 'o', '.', '#', '#', '#'},
            { 'o', '#', 'o', 'o', 'o', 'o', '#', '.', '.', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '.', '#', '.', '.', 'o', 'o', '.', 'o', 'o', '.', '.', 'o', '.', 'o', 'o', '.', 'o', '.', '#', '#', '#'},
            { 'o', 'o', 'o', 'o', 'o', 'o', '#', '.', '.', '#', '.', '.', '.', '.', '.', '.', '.', '.', '#', '.', '#', '.', '.', 'o', 'o', '.', 'o', 'o', '.', '.', 'o', '.', 'o', 'o', '.', 'o', '.', '#', '#', '#'},
            { 'o', 'o', 'o', 'o', 'o', 'o', '#', '.', '.', '#', '.', '.', '.', '.', 'o', 'o', '.', '.', '#', '.', '#', '.', '.', 'o', 'o', '.', 'o', 'o', '.', '.', 'o', '.', 'o', 'o', '.', 'o', '.', '#', '#', '#'},
            { '#', 'o', 'o', 'o', 'o', 'o', '#', '.', '.', '#', '.', '.', '.', 'o', 'o', 'o', 'o', '.', '#', '.', '.', '.', '.', 'o', 'o', '.', 'o', 'o', '.', '.', 'o', '.', 'o', 'o', '.', 'o', '.', '#', '#', '#'},
            { '#', '#', 'o', 'o', 'o', 'o', '#', '.', '.', '#', '.', '.', 'o', 'o', 'o', 'o', 'o', 'o', '#', '.', '.', '.', '.', 'o', 'o', '.', 'o', 'o', '.', '.', 'o', 'o', 'o', 'o', '.', 'o', '.', '#', '#', '#'},
            { '#', '#', 'o', 'o', 'o', 'o', '#', '.', '.', '.', '.', '.', '.', 'o', 'o', 'o', 'o', '.', '#', '.', '.', '.', '.', 'o', 'o', '.', 'o', 'o', '.', '.', 'o', '.', 'o', 'o', '.', 'o', '.', '#', '#', '#'},
            { '#', '#', '#', '#', '#', '#', '#', '.', '.', '.', '.', '.', '.', '.', 'o', 'o', '.', '.', '#', '.', '.', '.', '.', 'o', 'o', '.', 'o', 'o', '.', '.', 'o', '.', 'o', 'o', '.', 'o', '.', '#', '#', '#'},
            { '#', '#', '.', '.', '.', '.', '.', '.', '.', '#', '.', '.', '.', '.', '.', '.', '.', '.', '#', '.', '.', '.', '.', 'o', 'o', '.', 'o', 'o', '.', '.', 'o', '.', 'o', 'o', '.', 'o', '.', '#', '#', '#'},
            { '#', '#', 'o', 'o', 'o', '.', '.', '.', '#', '#', '.', '.', '#', '#', '#', '#', '#', '#', '#', 'o', '.', '.', '.', 'o', 'o', '.', 'o', 'o', '.', '.', 'o', '.', 'o', 'o', '.', 'o', '.', '#', '#', '#'},
            { '#', '#', '.', 'o', 'o', '.', 'o', '.', '#', '#', '.', '.', '#', '.', '.', '.', '.', '.', 'o', 'o', '.', '.', '.', 'o', 'o', '.', 'o', 'o', '.', '.', 'o', '.', 'o', 'o', '.', 'o', '.', '#', '#', '#'},
            { '#', '#', '.', 'o', '.', 'o', '.', '.', '#', '#', '.', '.', '#', '.', '.', '.', '.', 'o', 'o', 'o', '.', '.', '.', 'o', 'o', '.', 'o', 'o', '.', '.', 'o', 'o', 'o', 'o', 'o', 'o', 'o', '#', '#', '#'},
            { '.', '#', 'o', 'o', 'o', 'o', '.', '.', '#', '#', '.', '.', '#', '.', '.', 'o', 'o', 'o', 'o', 'o', '.', '.', '.', 'o', 'o', '.', 'o', 'o', '.', '.', '.', '.', '.', '.', '.', '.', '.', '#', '#', '#'},
            { '.', '.', 'o', 'o', 'o', '.', 'o', '.', '.', '.', '.', '.', '#', '.', '.', 'o', 'o', 'o', 'o', 'o', '.', '.', '.', 'o', 'o', '.', 'o', 'o', '.', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '.', '.', 'o', 'o', 'o', '.', '.', '.', '.', '.', '.', '.', '#', '.', '.', 'o', 'o', 'o', 'o', 'o', 'o', '.', '.', 'o', 'o', '.', 'o', 'o', '.', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '.', '.', '.', 'o', '.', '#', '.', '.', '.', '.', '.', '.', '#', '.', '.', 'o', 'o', '.', '.', 'o', 'o', '.', '.', 'o', 'o', '.', '.', '.', '.', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', '.', 'o', '.', '#', 'o', '.', '.', '.', '.', '.', '#', '.', '.', '.', '.', '.', '.', '.', 'o', 'o', '.', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', 'o', 'o', '.', '#', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '#', '#', '#', '.', '.', 'o', '.', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', 'o', 'o', '.', '#', '.', '.', '.', '.', '.', '.', '.', '.', '.', '#', '#', '#', '#', '#', '.', 'o', '.', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', '.', 'o', '.', '#', '.', '.', '.', '.', '.', '.', '.', '.', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            };
            game.InitializeMap(customMap);

            while (true)
            {
                game.PrintMap();
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Escape)
                    break;

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        game.MovePlayer('w');
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        game.MovePlayer('s');
                        break;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        game.MovePlayer('a');
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        game.MovePlayer('d');
                        break;
                    default:
                        break;
                }
            }

            Console.WriteLine("Saliendo del juego.");
        }
    }

    // Métodos de creación de Pokémon
        internal static Pokemons Pikachu()
        {
            string[] spritePikachu = {
            "                 /#                                                             ",
            "               ,#&@##                                                           ",
            "              (#@@@@@                                                           ",
            "              @@@%#,,##                             #####@@@##                  ",
            "              @% .,,,@@                        #@@@@  @@@@@@##                  ",
            "            ###( .,,,@@                    ##@@     ,,@@@@@@                    ",
            "            @@  ,,,####        ,,@@@@@( .    ,,,,,,,##@@@                       ",
            "            @@,*#,         ,,,,,,,,,,,,,,,,,,,,,,,,,@@          @@@@            ",
            "            ##       ,,,,,,,,,,,,,,,,,,,,,,,,,,,,,@@          @@    @@          ",
            "          &&((...,,,,,,,,,,,,&&&&,,,,,,,,,,,,,,&,.          &&,,..  ..,.**&&    ",
            "       ,@@  @@,,,,,,,,,,,,,@&  @@@@,,,,,,,,@@@@             @@    ,,  ,,  ,,@@  ",
            "       ,@@@@@@,,,,,,,,,,,,,@@@@@@@@,,,,,,,,,,@@           @@    ,,  ,,  ,,,,@@  ",
            "      #(,,@@,,,,,%@,,,,,,,,,,@@@@,,,,,,,,,,,,##           @@  ,,,,,,,,,,,,,,##  ",
            "      @&##,,,,@%,%@@@,,,,%@,,,,,,########,,,,,,@         @,,,,,,,,,,,,,,@@      ",
            "      @&##,,,,,/@/,,,@@@@/,,,,,,,########,,,,,,,##    @@@,,,,,,,,,,,,,@@        ",
            "       ,@@,,,,,,,,,,,,,,,,,,,,,,,,,###/,,,,,,,,,@@    @@@,,,,,,,,,,,@@          ",
            "       ,@@,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,##@@  @@@,,,,,,,,,@@            ",
            "            ###(,,,,,,,,,,,,,,,,.####@@@@,,,,,,,##@@      @@,,,,,,,,,,@@        ",
            "            @@,,,,,,,,,,,,,,,,,##    ,,,,,,,,,,,,,,,@@    @@##,,,,,,@@@@        ",
            "       ,@@@@,,,,,,,,,,,,,,,,,,,@@,,,,,,,,,,,,,,,,,,,@@   @,,,,,,,,@@            ",
            "    @@..,,,,#(,,,,,,,,,,,,,,,,,,,*,,,@%((,,,,,,(####@@...@,,,,@@..              ",
            "    @@,,,,,,@@,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,####@@@@@@##,,,,@@               ",
            "      @@@@@@@@,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,@@######@@@@                ",
            "            @@,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,@@#####@@@                    ",
            "             .@%,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,@@                            ",
            "              @% .,,,,,,,,,,,,,,,  ,,,,,,,,,,,,,,,@@                            ",
            "              @%   ,,,,,,,,,,,,,,,,   .,,,,,,,,,@@                              ",
            "                .#&((((,,,,&&@@......&&&&&&((&&@..                              ",
            "               ,@/,,,,,,,%@           (@@,,,,,,@                                ",
            "                 %@@@@@@@,               @@@@@@  "
            };
            Pokemons pokemon1 = new Pokemons("Pikachu", 15, 250, 90, spritePikachu);
            //Informacion basica del pokemon
            Console.WriteLine($"{pokemon1.Sprite}");
            Console.WriteLine("Informacion del pokemon:");
            Console.WriteLine($"Nombre: {pokemon1.Nombre}");
            Console.WriteLine($"Nivel: {pokemon1.Nivel}");
            Console.WriteLine($"HP: {pokemon1.HP}");
            //Movimientos del pokemon
            Movimiento movimiento1 = new Movimiento("Placaje", 40, 100);
            Movimiento movimiento5 = new Movimiento("Arañazo", 40, 100);
            Movimiento movimiento7 = new Movimiento("Doble Patada", 30, 100);
            Movimiento movimiento9 = new Movimiento("Patada Salto", 100, 95);
            pokemon1.Movimientos[0] = movimiento1;
            pokemon1.Movimientos[1] = movimiento5;
            pokemon1.Movimientos[2] = movimiento7;
            pokemon1.Movimientos[3] = movimiento9;
            //Informacion de movimientos
            foreach (Movimiento movimiento in pokemon1.Movimientos)
            {
                Console.WriteLine($"Nombre: {movimiento.Nombre}");
                Console.WriteLine($"Potencia: {movimiento.Potencia}");
                Console.WriteLine($"Precisión: {movimiento.Precision}");
                Console.WriteLine();
            }
            return pokemon1;
        }

        internal static Pokemons Rattata()
        {
            string[] spriteRattata = {
            "                                           /,,,%#                               ",
            "                                           (,,((##%                             ",
            "                                                 ((@                            ",
            "                                                 *(@                            ",
            "                                                 ((@                            ",
            "                        **,((       (,,(         (@                             ",
            "                        *.%(%(,,,,/*%**/(%     %#/                              ",
            "                      /*  %(/,,((((((#./(@((((((#                               ",
            "                       .(((((((% # /(%@@(((((((((%                              ",
            "                        */,...........,%%%%(((((%&                              ",
            "                          .  *@@%...,((*,(%/(%%%%%@                             ",
            "                          %###,///.& %,,,@/*    &*,@                            ",
            "                        (/,,#* @@&  %,,/@       &/                              ",
            "                                  ,./.,/                                        "
        };
            Pokemons pokemon2 = new Pokemons("Rattata", 2, 100, 100, spriteRattata);
            Console.WriteLine($"{pokemon2.Sprite}");
            Console.WriteLine("Informacion del pokemon:");
            Console.WriteLine($"Nombre: {pokemon2.Nombre}");
            Console.WriteLine($"Nivel: {pokemon2.Nivel}");
            Console.WriteLine($"HP: {pokemon2.HP}");
            Movimiento movimiento2 = new Movimiento("Megapuño", 80, 85);
            Movimiento movimiento14 = new Movimiento("Derribo", 90, 85);
            Movimiento movimiento11 = new Movimiento("Patada Giro", 60, 85);
            Movimiento movimiento4 = new Movimiento("Corte", 50, 95);
            pokemon2.Movimientos[0] = movimiento2;
            pokemon2.Movimientos[1] = movimiento14;
            pokemon2.Movimientos[2] = movimiento11;
            pokemon2.Movimientos[3] = movimiento4;
            foreach (Movimiento movimiento in pokemon2.Movimientos)
            {
                Console.WriteLine($"Nombre: {movimiento.Nombre}");
                Console.WriteLine($"Potencia: {movimiento.Potencia}");
                Console.WriteLine($"Precisión: {movimiento.Precision}");
                Console.WriteLine();
            }
            return pokemon2;
        }

        internal static Pokemons Psyduck()
        {
            string[] spritePsyduck = {
            "                           (*,                                                  ",
            "                   *@(%.   &%%.                                                 ",
            "                    #&@@/# @@@.                                                 ",
            "                  (%,,,@#%%#//&@@@&@@/@@,%                                      ",
            "                  /@@&@@%@&&%%/,,,,,....,.,*&%**/&                              ",
            "                     ,@@#/,,,****,..........*(&//*&/&/                          ",
            "                     &@/,,,,,,*,,/,,..../..*,*//&@/***%@                        ",
            "                    &/*.,*(%&&&&&(,,,,,,*(#&    %&@,,,,*&,                      ",
            "                 .%@#/*,#%& . . .%#%*****@@*.,@., @(*,**&,(                     ",
            "               %**#@@%#*&&*   ,  ,@&,,%@@@@@/*,,(@@(*****@@                     ",
            "               /&,,,,%@*@@#&&@&@&%@&@,,*,  .  .@(*@/*,**%@@                     ",
            "              @@#,*,,*/*@@((((%(%/,* ,**, . #*   *@%@#(%@@(                     ",
            "              @@#//*****#%@%%##@@*/(. ///,.  ,  ,. ..@@@%.                      ",
            "             ##%&##(****#%@#&@@@@@*/%   ,  * ...  *  (@@                        ",
            "             @((###((#####@#((#(##@&,,     ,      /,*@@(                        ",
            "             @&%#%%###%####%(#######%@&@*,*,***@%@#&&@                          ",
            "             ,&@%%%%%%%%%%#%%#%%%%%%%%&%@@@@@@@*****/@*                         ",
            "                *#&@@@@#%###%##%#%%%%***/******/**(**#@@                        ",
            "                  *@######&##%###**,**,**,,(,,*/*******/@ @@*                   ",
            "            &@&&&%&###%#(%#(##***,,**,,,*,,*,,**,**,,**#@, .@*,(@%              ",
            "          #@%%%%%@@%%%%%&&&&&&/*/(##********/***/***//@#..**@*//@@@             ",
            "           /@&&%%@@#%%#%@@.., @@@ .*@**************//@.,./(/*/(/@@(,            ",
            "            .@@@%%%%@@@@@@(. /   ...@***********//#@@,.**///*/#@&/              ",
            "                 %@@%#@*,**,**,*    @%##%#%&##%###@,.,**/**/@&#*                ",
            "                     %&%&@*****,*   @##%%##@@@&@@@@(*/*/&&@&*                   ",
            "                          @%///(/*&@@@@@@@@*   ,, ,@@@@@//,                     ",
            "                            (#@@@@@#.                                           "
        };
            Pokemons pokemon3 = new Pokemons("Psyduck", 20, 270, 70, spritePsyduck);
            Console.WriteLine($"{pokemon3.Sprite}");
            Console.WriteLine("Informacion del pokemon:");
            Console.WriteLine($"Nombre: {pokemon3.Nombre}");
            Console.WriteLine($"Nivel: {pokemon3.Nivel}");
            Console.WriteLine($"HP: {pokemon3.HP}");
            Movimiento movimiento6 = new Movimiento("Látigo Cepa", 45, 100);
            Movimiento movimiento10 = new Movimiento("Golpe Cabeza", 70, 100);
            Movimiento movimiento12 = new Movimiento("Doble Filo", 120, 100);
            Movimiento movimiento13 = new Movimiento("Saña", 120, 100);
            pokemon3.Movimientos[0] = movimiento6;
            pokemon3.Movimientos[1] = movimiento10;
            pokemon3.Movimientos[2] = movimiento12;
            pokemon3.Movimientos[3] = movimiento13;
            foreach (Movimiento movimiento in pokemon3.Movimientos)
            {
                Console.WriteLine($"Nombre: {movimiento.Nombre}");
                Console.WriteLine($"Potencia: {movimiento.Potencia}");
                Console.WriteLine($"Precisión: {movimiento.Precision}");
                Console.WriteLine();
            }
            return pokemon3;
        }
        
        internal static Pokemons Squirtle()
        {
            string[] spriteSquirtle = {
            "                           //@@@@@@/                                            ",
            "                        @  ..........@@                                         ",
            "                      //.............../                                        ",
            "                     @..@ ......@@ .....@                                       ",
            "                     @...@/////.//@.///.@                                       ",
            "                      //.@@///////.....@  @@                                    ",
            "                        @..////.....@//.    @                                   ",
            "                      //@.@///@//////@@//@// @                                  ",
            "                   / .../.   .@.   ..//@@   @/@                                 ",
            "                    /@@@/@/  ./   /@@////@../  @@   //...@@                     ",
            "                    /@@@@  .../     ...//@../..@@ @.  /@/  .@                   ",
            "                   @  ...@@   /    .@@@//@..@ @  @ /   .   @.@                  ",
            "                    @...//@.../ ..../@@@@/..@@@//  .//  @//.@                   ",
            "                      //@@ @@@/@@@//...///@@//.   . @@/@@//                     ",
            "                                   / ...//@@ @.   /                             ",
            "                                   @   ..@                                      "
        };
            Pokemons pokemon4 = new Pokemons("Squirtle", 15, 250, 80, spriteSquirtle);
            Console.WriteLine($"{pokemon4.Sprite}");
            Console.WriteLine("Informacion del pokemon:");
            Console.WriteLine($"Nombre: {pokemon4.Nombre}");
            Console.WriteLine($"Nivel: {pokemon4.Nivel}");
            Console.WriteLine($"HP: {pokemon4.HP}");
            Movimiento movimiento3 = new Movimiento("Golpe Karate", 50, 100);
            Movimiento movimiento8 = new Movimiento("Megapatada", 120, 75);
            Movimiento movimiento15 = new Movimiento("Hidrobomba", 110, 80);
            Movimiento movimiento16 = new Movimiento("Pistola Agua", 40, 100);
            pokemon4.Movimientos[0] = movimiento15;
            pokemon4.Movimientos[1] = movimiento16;
            pokemon4.Movimientos[2] = movimiento8;
            pokemon4.Movimientos[3] = movimiento3;
            foreach (Movimiento movimiento in pokemon4.Movimientos)
            {
                Console.WriteLine($"Nombre: {movimiento.Nombre}");
                Console.WriteLine($"Potencia: {movimiento.Potencia}");
                Console.WriteLine($"Precisión: {movimiento.Precision}");
                Console.WriteLine();
            }
            return pokemon4;
        }
}