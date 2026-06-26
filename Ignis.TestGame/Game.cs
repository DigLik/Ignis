using System.Numerics;
using Ignis.Core.Graphics;
using Ignis.Core.Input;
using Ignis.Platform.Graphics;
using Ignis.Platform.Input;
using Ignis.Platform.Windowing;

namespace Ignis.TestGame;

internal sealed class Game
{
    private readonly Window _window;
    private readonly Keyboard _keyboard;
    private readonly Mouse _mouse;
    private readonly Renderer _renderer;

    private Vector2 _lastWindowSize;

    // FPS
    private float _fpsTimer;
    private int _fpsCounter;
    private int _currentFps;

    // Игровое состояние
    private bool _isGameOver;
    private bool _isPaused;
    private int _score;
    private int _highScore;
    private float _asteroidSpawnTimer;
    private float _difficultyMultiplier = 1.0f;
    private float _timeSinceStart;

    // Игровые сущности
    private PlayerShip _player = null!;
    private readonly List<Star> _stars = [];
    private readonly List<Laser> _lasers = [];
    private readonly List<Asteroid> _asteroids = [];
    private readonly List<Particle> _particles = [];
    private readonly List<PowerUp> _powerUps = [];

    // Состояние бенчмарка
    private bool _isBenchmarkMode;
    private int _benchmarkPreset = 1;
    private float _benchmarkObjectCountFloat = 1000f;
    private int _benchmarkObjectCount = 1000;
    private readonly List<BenchmarkObject> _benchmarkObjects = [];

    public Game(Window window, Keyboard keyboard, Mouse mouse, Renderer renderer)
    {
        _window = window;
        _keyboard = keyboard;
        _mouse = mouse;
        _renderer = renderer;

        _lastWindowSize = new Vector2(window.Size.X, window.Size.Y);
        if (_lastWindowSize.X <= 0 || _lastWindowSize.Y <= 0)
            _lastWindowSize = new Vector2(800, 600);

        InitializeGame();
    }

    private void InitializeGame()
    {
        _score = 0;
        _isGameOver = false;
        _isPaused = false;
        _asteroidSpawnTimer = 0f;
        _difficultyMultiplier = 1.0f;
        _timeSinceStart = 0f;

        Vector2 size = new(_window.Size.X, _window.Size.Y);
        if (size.X <= 0 || size.Y <= 0) size = new Vector2(800, 600);

        _player = new PlayerShip(new Vector2(size.X / 2f, size.Y * 0.8f));

        // Инициализируем звезды
        _stars.Clear();
        for (int i = 0; i < 120; i++)
        {
            _stars.Add(new Star(
                new Vector2((float)SafeRandom.NextDouble() * size.X, (float)SafeRandom.NextDouble() * size.Y),
                (float)SafeRandom.NextDouble() * 80f + 20f,
                (float)SafeRandom.NextDouble() * 1.5f + 0.5f,
                new Color(1f, 1f, 1f, (float)SafeRandom.NextDouble() * 0.5f + 0.3f)
            ));
        }

        _lasers.Clear();
        _asteroids.Clear();
        _particles.Clear();
        _powerUps.Clear();
    }

    public void UpdateAndRender()
    {
        float rawDeltaTime = _renderer.DeltaTime;
        float deltaTime = rawDeltaTime;

        // Ограничиваем максимальный deltaTime, чтобы избежать телепортаций при фризах
        if (deltaTime > 0.1f) deltaTime = 0.1f;

        // Расчет FPS
        _fpsTimer += rawDeltaTime;
        _fpsCounter++;
        if (_fpsTimer >= 0.5f)
        {
            _currentFps = (int)MathF.Round(_fpsCounter / _fpsTimer);
            _fpsTimer = 0f;
            _fpsCounter = 0;
        }

        Vector2 size = new(_window.Size.X, _window.Size.Y);
        if (size.X <= 0 || size.Y <= 0) size = new Vector2(800, 600);

        // Обработка клавиши F1 для переключения режима
        if (_keyboard.IsKeyPressed(Key.F1))
        {
            _isBenchmarkMode = !_isBenchmarkMode;
            if (_isBenchmarkMode)
            {
                AdjustBenchmarkObjects(size);
            }
        }

        if (_isBenchmarkMode)
        {
            UpdateAndRenderBenchmark(deltaTime, size);
            return;
        }

        // Проверяем изменение размера окна
        if (size != _lastWindowSize)
        {
            HandleResize(_lastWindowSize, size);
            _lastWindowSize = size;
        }

        _renderer.UpdateCamera(size / 2, size);

        // Обработка клавиш глобального состояния
        if (_keyboard.IsKeyPressed(Key.Escape) && !_isGameOver)
        {
            _isPaused = !_isPaused;
        }

        if (_isGameOver && _keyboard.IsKeyPressed(Key.Enter))
        {
            InitializeGame();
            return;
        }

        if (!_isPaused && !_isGameOver)
        {
            UpdatePhysics(deltaTime, size);
        }

        RenderGame(size);
    }

    private void HandleResize(Vector2 oldSize, Vector2 newSize)
    {
        if (oldSize.X <= 0f || oldSize.Y <= 0f) return;

        float scaleX = newSize.X / oldSize.X;
        float scaleY = newSize.Y / oldSize.Y;

        if (_player != null)
        {
            _player.Position.X *= scaleX;
            _player.Position.Y *= scaleY;
        }

        foreach (var star in _stars)
        {
            star.Position.X *= scaleX;
            star.Position.Y *= scaleY;
        }

        foreach (var laser in _lasers)
        {
            laser.Position.X *= scaleX;
            laser.Position.Y *= scaleY;
        }

        foreach (var asteroid in _asteroids)
        {
            asteroid.Position.X *= scaleX;
            asteroid.Position.Y *= scaleY;
        }

        foreach (var pu in _powerUps)
        {
            pu.Position.X *= scaleX;
            pu.Position.Y *= scaleY;
        }

        foreach (var p in _particles)
        {
            p.Position.X *= scaleX;
            p.Position.Y *= scaleY;
        }
    }

    private void UpdatePhysics(float deltaTime, Vector2 size)
    {
        _timeSinceStart += deltaTime;
        _difficultyMultiplier = 1.0f + _timeSinceStart * 0.02f; // Сложность растет со временем

        // 1. Обновление звездного фона
        foreach (var star in _stars)
        {
            star.Position.Y += star.Speed * deltaTime;
            if (star.Position.Y > size.Y)
            {
                star.Position.Y = 0;
                star.Position.X = (float)SafeRandom.NextDouble() * size.X;
            }
        }

        // 2. Обновление игрока
        _player.Update(deltaTime, _keyboard, size);

        // Стрельба игрока
        if (_keyboard.IsKeyDown(Key.Space) || _mouse.IsButtonDown(MouseButton.Left))
        {
            var newLasers = _player.TryShoot(deltaTime);
            if (newLasers != null)
            {
                _lasers.AddRange(newLasers);
            }
        }

        // 3. Обновление лазеров
        for (int i = _lasers.Count - 1; i >= 0; i--)
        {
            var laser = _lasers[i];
            laser.Position += laser.Velocity * deltaTime;

            // Удаляем вылетевшие за экран
            if (laser.Position.Y < -50 || laser.Position.Y > size.Y + 50)
            {
                _lasers.RemoveAt(i);
            }
        }

        // 4. Спавн астероидов
        _asteroidSpawnTimer += deltaTime;
        float spawnInterval = Math.Max(0.3f, 1.5f / _difficultyMultiplier);
        if (_asteroidSpawnTimer >= spawnInterval)
        {
            _asteroidSpawnTimer = 0f;
            float radius = (float)SafeRandom.NextDouble() * 30f + 10f;
            float speed = (float)SafeRandom.NextDouble() * 100f + 50f;
            speed *= (1.0f + (_difficultyMultiplier - 1.0f) * 0.3f); // Метеориты ускоряются

            _asteroids.Add(new Asteroid(
                new Vector2((float)SafeRandom.NextDouble() * (size.X - radius * 2) + radius, -radius),
                new Vector2(((float)SafeRandom.NextDouble() - 0.5f) * 40f, speed),
                radius
            ));
        }

        // 5. Обновление астероидов
        for (int i = _asteroids.Count - 1; i >= 0; i--)
        {
            var asteroid = _asteroids[i];
            asteroid.Position += asteroid.Velocity * deltaTime;

            // Если улетел вниз экрана
            if (asteroid.Position.Y > size.Y + asteroid.Radius)
            {
                _asteroids.RemoveAt(i);
                continue;
            }

            // Коллизия с игроком
            if (!_player.IsDead &&
                Vector2.Distance(_player.Position, asteroid.Position) < _player.Radius + asteroid.Radius)
            {
                // Игрок получает урон
                float damage = asteroid.Radius * 1.2f;
                _player.TakeDamage(damage);

                // Спавним взрыв астероида
                SpawnExplosion(asteroid.Position, asteroid.Radius, Color.Yellow);

                // Удаляем астероид
                _asteroids.RemoveAt(i);

                if (_player.IsDead)
                {
                    _isGameOver = true;
                    if (_score > _highScore) _highScore = _score;
                    SpawnExplosion(_player.Position, _player.Radius * 2, Color.Red);
                }
            }
        }

        // 6. Проверка коллизий лазеров и астероидов
        for (int i = _lasers.Count - 1; i >= 0; i--)
        {
            var laser = _lasers[i];
            bool laserDestroyed = false;

            for (int j = _asteroids.Count - 1; j >= 0; j--)
            {
                var asteroid = _asteroids[j];

                // Проверяем коллизию лазера и астероида (круг-круг, лазер имеет виртуальный радиус)
                if (Vector2.Distance(laser.Position, asteroid.Position) < asteroid.Radius + 6f)
                {
                    laserDestroyed = true;
                    asteroid.Hp -= 20f; // Урон лазера

                    // Искры на месте попадания
                    SpawnSparks(laser.Position, laser.Velocity * -0.2f, laser.Color);

                    if (asteroid.Hp <= 0)
                    {
                        _score += asteroid.ScoreValue;

                        // Спавним взрыв
                        SpawnExplosion(asteroid.Position, asteroid.Radius, Color.Yellow);

                        // Шанс выпадения бонуса
                        if (SafeRandom.NextDouble() < 0.15)
                        {
                            SpawnPowerUp(asteroid.Position);
                        }

                        // Если астероид был большим, раскалываем его на два маленьких
                        if (asteroid.Radius > 22f)
                        {
                            float newRad = asteroid.Radius * 0.55f;
                            var offset1 = new Vector2(-newRad, 0);
                            var offset2 = new Vector2(newRad, 0);
                            var vel1 = new Vector2(asteroid.Velocity.X - 50f, asteroid.Velocity.Y * 1.1f);
                            var vel2 = new Vector2(asteroid.Velocity.X + 50f, asteroid.Velocity.Y * 1.1f);

                            _asteroids.Add(new Asteroid(asteroid.Position + offset1, vel1, newRad));
                            _asteroids.Add(new Asteroid(asteroid.Position + offset2, vel2, newRad));
                        }

                        _asteroids.RemoveAt(j);
                    }

                    break;
                }
            }

            if (laserDestroyed)
            {
                _lasers.RemoveAt(i);
            }
        }

        // 7. Обновление бонусов
        for (int i = _powerUps.Count - 1; i >= 0; i--)
        {
            var powerUp = _powerUps[i];
            powerUp.Position += powerUp.Velocity * deltaTime;

            // Удаляем улетевшие
            if (powerUp.Position.Y > size.Y + 20)
            {
                _powerUps.RemoveAt(i);
                continue;
            }

            // Коллизия с игроком
            if (!_player.IsDead &&
                Vector2.Distance(_player.Position, powerUp.Position) < _player.Radius + powerUp.Radius)
            {
                // Применяем бонус
                _player.ApplyPowerUp(powerUp.Type);

                // Эффект сбора бонуса
                SpawnExplosion(powerUp.Position, 15f, powerUp.Color);

                _powerUps.RemoveAt(i);
            }
        }

        // 8. Обновление частиц
        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            var p = _particles[i];
            p.Position += p.Velocity * deltaTime;
            p.Velocity *= MathF.Exp(-1.5f * deltaTime); // Замедление
            p.Age += deltaTime;

            if (p.Age >= p.MaxAge)
            {
                _particles.RemoveAt(i);
            }
        }
    }

    private void SpawnExplosion(Vector2 pos, float radius, Color baseColor)
    {
        int count = (int)(radius * 1.2f);
        if (count < 10) count = 10;
        if (count > 50) count = 50;

        for (int i = 0; i < count; i++)
        {
            float angle = (float)(SafeRandom.NextDouble() * Math.PI * 2);
            float speed = (float)SafeRandom.NextDouble() * 150f + 50f;
            Vector2 velocity = new(MathF.Cos(angle) * speed, MathF.Sin(angle) * speed);

            float maxAge = (float)SafeRandom.NextDouble() * 0.6f + 0.3f;
            float size = (float)SafeRandom.NextDouble() * 3f + 1f;

            // Разнообразие цвета взрыва (желтый, оранжевый, красный)
            Color pColor = Color.Lerp(baseColor, new Color(1f, 0.3f, 0f), (float)SafeRandom.NextDouble());

            _particles.Add(new Particle(pos, velocity, pColor, maxAge, size));
        }
    }

    private void SpawnSparks(Vector2 pos, Vector2 baseVel, Color color)
    {
        for (int i = 0; i < 5; i++)
        {
            float angle = (float)(SafeRandom.NextDouble() * Math.PI * 2);
            float speed = (float)SafeRandom.NextDouble() * 50f + 20f;
            Vector2 velocity = baseVel + new Vector2(MathF.Cos(angle) * speed, MathF.Sin(angle) * speed);

            float maxAge = (float)SafeRandom.NextDouble() * 0.3f + 0.1f;
            float size = (float)SafeRandom.NextDouble() * 2f + 1f;

            _particles.Add(new Particle(pos, velocity, color, maxAge, size));
        }
    }

    private void SpawnPowerUp(Vector2 pos)
    {
        int typeVal = SafeRandom.Next(3);
        PowerUpType type = typeVal switch
        {
            0 => PowerUpType.Health,
            1 => PowerUpType.TripleShot,
            _ => PowerUpType.Shield
        };

        _powerUps.Add(new PowerUp(pos, new Vector2(0f, 80f), type));
    }

    private void RenderGame(Vector2 size)
    {
        // 1. Очистка экрана (глубокий космос)
        _renderer.ClearBackground(new Color(0.03f, 0.03f, 0.07f));

        // 2. Отрисовка звезд
        foreach (var star in _stars)
        {
            _renderer.DrawRectangle(star.Position, new Vector2(star.Size), 0f, star.Color);
        }

        // 3. Отрисовка бонусов
        foreach (var pu in _powerUps)
        {
            // Мерцающий эффект
            float pulse = 1f + 0.2f * MathF.Sin(_timeSinceStart * 8f);
            Vector2 puSize = new(pu.Radius * 1.5f * pulse);

            if (pu.Type == PowerUpType.Health)
            {
                // Зеленый крест
                _renderer.DrawRectangle(pu.Position, new Vector2(puSize.X, puSize.Y / 3f), 0f, pu.Color);
                _renderer.DrawRectangle(pu.Position, new Vector2(puSize.X / 3f, puSize.Y), 0f, pu.Color);
            }
            else if (pu.Type == PowerUpType.TripleShot)
            {
                // Синий треугольник
                Vector2 v1 = pu.Position + new Vector2(0f, -pu.Radius * pulse);
                Vector2 v2 = pu.Position + new Vector2(-pu.Radius * pulse, pu.Radius * pulse);
                Vector2 v3 = pu.Position + new Vector2(pu.Radius * pulse, pu.Radius * pulse);
                _renderer.DrawTriangle(v1, v2, v3, pu.Color);
            }
            else
            {
                // Желтый ромб/щит
                _renderer.DrawEllipse(pu.Position, puSize * 0.7f, 0f, pu.Color);
                _renderer.DrawEllipse(pu.Position, puSize * 0.5f, MathF.PI / 4f, Color.White);
            }
        }

        // 4. Отрисовка лазеров
        foreach (var laser in _lasers)
        {
            _renderer.DrawRectangle(laser.Position, new Vector2(laser.Width, laser.Height), 0f, laser.Color);
        }

        // 5. Отрисовка астероидов
        foreach (var ast in _asteroids)
        {
            // Рисуем астероид как многоугольник или эллипс с деталями
            _renderer.DrawEllipse(ast.Position, new Vector2(ast.Radius * 2f), 0f, new Color(0.35f, 0.3f, 0.28f));
            // Внутренний круг для объема
            _renderer.DrawEllipse(ast.Position - new Vector2(ast.Radius * 0.2f), new Vector2(ast.Radius * 1.3f), 0f,
                new Color(0.28f, 0.24f, 0.22f));
            // Кратеры
            _renderer.DrawCircle(ast.Position + new Vector2(ast.Radius * 0.4f, -ast.Radius * 0.2f), ast.Radius * 0.25f,
                new Color(0.18f, 0.15f, 0.14f));
            _renderer.DrawCircle(ast.Position + new Vector2(-ast.Radius * 0.3f, ast.Radius * 0.3f), ast.Radius * 0.2f,
                new Color(0.18f, 0.15f, 0.14f));
        }

        // 6. Отрисовка частиц
        foreach (var p in _particles)
        {
            float lifePercent = 1f - (p.Age / p.MaxAge);
            Color fadeColor = p.Color.WithAlpha(lifePercent);
            _renderer.DrawRectangle(p.Position, new Vector2(p.Size), 0f, fadeColor);
        }

        // 7. Отрисовка игрока
        if (!_player.IsDead)
        {
            // Эффект реактивного следа двигателя
            float flamePulse = 1f + 0.3f * MathF.Sin(_timeSinceStart * 30f);
            Vector2 flamePos = _player.Position + new Vector2(0f, _player.Radius * 0.9f);
            _renderer.DrawTriangle(
                flamePos,
                flamePos + new Vector2(-8f, 0f),
                flamePos + new Vector2(0f, 15f * flamePulse),
                new Color(1f, 0.4f, 0f)
            );
            _renderer.DrawTriangle(
                flamePos,
                flamePos + new Vector2(8f, 0f),
                flamePos + new Vector2(0f, 15f * flamePulse),
                new Color(1f, 0.4f, 0f)
            );
            // Ядро пламени
            _renderer.DrawTriangle(
                flamePos,
                flamePos + new Vector2(-4f, 0f),
                flamePos + new Vector2(0f, 8f * flamePulse),
                Color.Yellow
            );

            // Корабль: треугольник корпуса
            Vector2 shipPeak = _player.Position + new Vector2(0f, -_player.Radius);
            Vector2 shipLeft = _player.Position + new Vector2(-_player.Radius, _player.Radius * 0.8f);
            Vector2 shipRight = _player.Position + new Vector2(_player.Radius, _player.Radius * 0.8f);
            _renderer.DrawTriangle(shipPeak, shipLeft, shipRight, new Color(0.2f, 0.6f, 1f));

            // Крылья / боковые ускорители
            _renderer.DrawRectangle(_player.Position + new Vector2(-_player.Radius * 0.8f, _player.Radius * 0.5f),
                new Vector2(8f, 16f), 0f, new Color(0.1f, 0.4f, 0.8f));
            _renderer.DrawRectangle(_player.Position + new Vector2(_player.Radius * 0.8f, _player.Radius * 0.5f),
                new Vector2(8f, 16f), 0f, new Color(0.1f, 0.4f, 0.8f));

            // Кабина (белый эллипс)
            _renderer.DrawEllipse(_player.Position + new Vector2(0f, -_player.Radius * 0.2f), new Vector2(6f, 10f), 0f,
                Color.White);

            // Отрисовка активного щита
            if (_player.Shield > 0)
            {
                float shieldAlpha = 0.2f + 0.1f * MathF.Sin(_timeSinceStart * 10f);
                shieldAlpha *= (_player.Shield / 50f); // Яркость зависит от прочности
                _renderer.DrawCircle(_player.Position, _player.Radius * 1.5f, new Color(0f, 0.6f, 1f, shieldAlpha));
            }
        }

        // 8. Интерфейс (UI)
        // Здоровье
        _renderer.DrawText("HP", new Vector2(20f, 20f), 32f, Color.White);
        _renderer.DrawRectangle(new Vector2(284f, 36f), new Vector2(208f, 28f), 0f, Color.Black);
        if (_player.Hp > 0)
        {
            _renderer.DrawRectangle(new Vector2(184f + _player.Hp, 36f), new Vector2(_player.Hp * 2f, 20f), 0f,
                Color.Red);
        }

        // Щит
        if (_player.Shield > 0)
        {
            _renderer.DrawText("SHIELD", new Vector2(20f, 60f), 32f, Color.White);
            _renderer.DrawRectangle(new Vector2(284f, 76f), new Vector2(208f, 28f), 0f, Color.Black);
            _renderer.DrawRectangle(new Vector2(184f + _player.Shield * 2f, 76f), new Vector2(_player.Shield * 4f, 20f),
                0f,
                new Color(0f, 0.6f, 1f));
        }

        // Тройной выстрел статус
        if (_player.TripleShotTimer > 0)
        {
            _renderer.DrawText("3-WAY SHOT", new Vector2(20f, 105f), 24f, Color.Cyan);
        }

        // Очки и рекорд
        string scoreStr = $"SCORE: {_score}";
        string highScoreStr = $"HIGH: {_highScore}";
        float scoreWidth = Renderer.MeasureText(scoreStr, 32f);
        float highScoreWidth = Renderer.MeasureText(highScoreStr, 32f);
        _renderer.DrawText(scoreStr, new Vector2(size.X - scoreWidth - 20f, 20f), 32f, Color.Yellow);
        _renderer.DrawText(highScoreStr, new Vector2(size.X - highScoreWidth - 20f, 60f), 32f, Color.White);

        // Пауза
        if (_isPaused)
        {
            // Затемнение
            _renderer.DrawRectangle(size / 2f, size, 0f, new Color(0f, 0f, 0f, 0.6f));
            string pausedStr = "PAUSED";
            float pausedWidth = Renderer.MeasureText(pausedStr, 64f);
            _renderer.DrawText(pausedStr, new Vector2((size.X - pausedWidth) / 2f, size.Y / 2f - 20f), 64f,
                Color.White);
            string resumeStr = "PRESS ESC TO RESUME";
            float resumeWidth = Renderer.MeasureText(resumeStr, 32f);
            _renderer.DrawText(resumeStr, new Vector2((size.X - resumeWidth) / 2f, size.Y / 2f + 30f), 32f,
                Color.White);
        }

        // Game Over
        if (_isGameOver)
        {
            _renderer.DrawRectangle(size / 2f, size, 0f, new Color(0f, 0f, 0f, 0.7f));
            string gameOverStr = "GAME OVER";
            float gameOverWidth = Renderer.MeasureText(gameOverStr, 72f);
            _renderer.DrawText(gameOverStr, new Vector2((size.X - gameOverWidth) / 2f, size.Y / 2f - 50f), 72f,
                Color.Red);
            string finalScoreStr = $"FINAL SCORE: {_score}";
            float finalScoreWidth = Renderer.MeasureText(finalScoreStr, 40f);
            _renderer.DrawText(finalScoreStr, new Vector2((size.X - finalScoreWidth) / 2f, size.Y / 2f), 40f,
                Color.Yellow);
            string restartStr = "PRESS ENTER TO RESTART";
            float restartWidth = Renderer.MeasureText(restartStr, 32f);
            _renderer.DrawText(restartStr, new Vector2((size.X - restartWidth) / 2f, size.Y / 2f + 40f), 32f,
                Color.White);
        }

        // 9. Отрисовка FPS
        string fpsStr = $"FPS: {_currentFps}";
        float fpsWidth = Renderer.MeasureText(fpsStr, 28f);
        _renderer.DrawText(fpsStr, new Vector2(size.X - fpsWidth - 20f, size.Y - 45f), 28f,
            new Color(0.6f, 0.6f, 0.6f));
    }

    private void AdjustBenchmarkObjects(Vector2 size)
    {
        if (_benchmarkObjects.Count < _benchmarkObjectCount)
        {
            int toAdd = _benchmarkObjectCount - _benchmarkObjects.Count;
            for (int i = 0; i < toAdd; i++)
            {
                float sizeVal = (float)SafeRandom.NextDouble() * 30f + 5f;
                float speed = (float)SafeRandom.NextDouble() * 150f + 50f;
                float angle = (float)(SafeRandom.NextDouble() * Math.PI * 2);
                Vector2 velocity = new(MathF.Cos(angle) * speed, MathF.Sin(angle) * speed);

                // Генерация ярких красивых цветов
                float r = (float)SafeRandom.NextDouble();
                float g = (float)SafeRandom.NextDouble();
                float b = (float)SafeRandom.NextDouble();
                float max = MathF.Max(r, MathF.Max(g, b));
                if (max > 0)
                {
                    r /= max;
                    g /= max;
                    b /= max;
                }

                Color color = new Color(r, g, b, (float)SafeRandom.NextDouble() * 0.5f + 0.5f);

                _benchmarkObjects.Add(new BenchmarkObject
                {
                    Position = new Vector2((float)SafeRandom.NextDouble() * size.X,
                        (float)SafeRandom.NextDouble() * size.Y),
                    Velocity = velocity,
                    Size = sizeVal,
                    Color = color,
                    Angle = (float)(SafeRandom.NextDouble() * Math.PI * 2),
                    RotationSpeed = (float)(SafeRandom.NextDouble() * 4f - 2f),
                    PulsePhase = (float)(SafeRandom.NextDouble() * Math.PI * 2),
                    PulseSpeed = (float)(SafeRandom.NextDouble() * 5f + 2f)
                });
            }
        }
        else if (_benchmarkObjects.Count > _benchmarkObjectCount)
        {
            _benchmarkObjects.RemoveRange(_benchmarkObjectCount, _benchmarkObjects.Count - _benchmarkObjectCount);
        }
    }

    private void UpdateAndRenderBenchmark(float deltaTime, Vector2 size)
    {
        // 1. Обработка клавиш выбора пресета
        if (_keyboard.IsKeyPressed(Key.D1) || _keyboard.IsKeyPressed(Key.Numpad1)) _benchmarkPreset = 1;
        if (_keyboard.IsKeyPressed(Key.D2) || _keyboard.IsKeyPressed(Key.Numpad2)) _benchmarkPreset = 2;
        if (_keyboard.IsKeyPressed(Key.D3) || _keyboard.IsKeyPressed(Key.Numpad3)) _benchmarkPreset = 3;
        if (_keyboard.IsKeyPressed(Key.D4) || _keyboard.IsKeyPressed(Key.Numpad4)) _benchmarkPreset = 4;
        if (_keyboard.IsKeyPressed(Key.D5) || _keyboard.IsKeyPressed(Key.Numpad5)) _benchmarkPreset = 5;
        if (_keyboard.IsKeyPressed(Key.D6) || _keyboard.IsKeyPressed(Key.Numpad6)) _benchmarkPreset = 6;
        if (_keyboard.IsKeyPressed(Key.D7) || _keyboard.IsKeyPressed(Key.Numpad7)) _benchmarkPreset = 7;
        if (_keyboard.IsKeyPressed(Key.D8) || _keyboard.IsKeyPressed(Key.Numpad8)) _benchmarkPreset = 8;
        if (_keyboard.IsKeyPressed(Key.D9) || _keyboard.IsKeyPressed(Key.Numpad9)) _benchmarkPreset = 9;
        if (_keyboard.IsKeyPressed(Key.D0) || _keyboard.IsKeyPressed(Key.Numpad0)) _benchmarkPreset = 10;

        // 2. Регулировка нагруженности сцены
        float speedMultiplier = 1f;
        if (_keyboard.IsKeyDown(Key.Shift) || _keyboard.IsKeyDown(Key.LeftShift) || _keyboard.IsKeyDown(Key.RightShift))
            speedMultiplier = 5f;

        if (_keyboard.IsKeyDown(Key.Up))
        {
            _benchmarkObjectCountFloat += 2000f * deltaTime * speedMultiplier;
            if (_benchmarkObjectCountFloat > 150000f) _benchmarkObjectCountFloat = 150000f;
        }

        if (_keyboard.IsKeyDown(Key.Down))
        {
            _benchmarkObjectCountFloat -= 2000f * deltaTime * speedMultiplier;
            if (_benchmarkObjectCountFloat < 0f) _benchmarkObjectCountFloat = 0f;
        }

        _benchmarkObjectCount = (int)_benchmarkObjectCountFloat;

        AdjustBenchmarkObjects(size);

        // 3. Физика объектов бенчмарка
        for (int i = 0; i < _benchmarkObjects.Count; i++)
        {
            var obj = _benchmarkObjects[i];
            obj.Position += obj.Velocity * deltaTime;
            obj.Angle += obj.RotationSpeed * deltaTime;

            float padding = obj.Size;
            if (obj.Position.X < padding)
            {
                obj.Position.X = padding;
                obj.Velocity.X = MathF.Abs(obj.Velocity.X);
            }
            else if (obj.Position.X > size.X - padding)
            {
                obj.Position.X = size.X - padding;
                obj.Velocity.X = -MathF.Abs(obj.Velocity.X);
            }

            if (obj.Position.Y < padding)
            {
                obj.Position.Y = padding;
                obj.Velocity.Y = MathF.Abs(obj.Velocity.Y);
            }
            else if (obj.Position.Y > size.Y - padding)
            {
                obj.Position.Y = size.Y - padding;
                obj.Velocity.Y = -MathF.Abs(obj.Velocity.Y);
            }
        }

        // 4. Отрисовка бенчмарка
        _renderer.UpdateCamera(size / 2, size);
        _renderer.ClearBackground(new Color(0.02f, 0.02f, 0.05f));

        for (int i = 0; i < _benchmarkObjects.Count; i++)
        {
            RenderBenchmarkObject(_benchmarkObjects[i], i);
        }

        RenderBenchmarkHUD(size);
    }

    private void RenderBenchmarkObject(BenchmarkObject obj, int index)
    {
        switch (_benchmarkPreset)
        {
            case 1: // Вращающиеся прямоугольники
                _renderer.DrawRectangle(obj.Position, new Vector2(obj.Size * 1.6f, obj.Size * 0.9f), obj.Angle,
                    obj.Color);
                break;
            case 2: // Пульсирующие круги
                float scale2 = 1f + 0.25f * MathF.Sin(_timeSinceStart * obj.PulseSpeed + obj.PulsePhase);
                _renderer.DrawCircle(obj.Position, obj.Size * scale2, obj.Color);
                break;
            case 3: // Вращающиеся треугольники
                float r3 = obj.Size * 1.2f;
                Vector2 v1 = obj.Position + new Vector2(MathF.Cos(obj.Angle), MathF.Sin(obj.Angle)) * r3;
                Vector2 v2 = obj.Position +
                             new Vector2(MathF.Cos(obj.Angle + 2.0944f), MathF.Sin(obj.Angle + 2.0944f)) * r3;
                Vector2 v3 = obj.Position +
                             new Vector2(MathF.Cos(obj.Angle + 4.1888f), MathF.Sin(obj.Angle + 4.1888f)) * r3;
                _renderer.DrawTriangle(v1, v2, v3, obj.Color);
                break;
            case 4: // Вращающиеся линии
                Vector2 dir = new Vector2(MathF.Cos(obj.Angle), MathF.Sin(obj.Angle)) * (obj.Size * 2.5f);
                _renderer.DrawLine(obj.Position - dir, obj.Position + dir, 1.5f, obj.Color);
                break;
            case 5: // Пульсирующие эллипсы
                float scale5 = 1f + 0.3f * MathF.Sin(_timeSinceStart * obj.PulseSpeed + obj.PulsePhase);
                _renderer.DrawEllipse(obj.Position, new Vector2(obj.Size * 1.5f * scale5, obj.Size * 0.7f * scale5),
                    obj.Angle, obj.Color);
                break;
            case 6: // Текст
                _renderer.DrawText("IGNIS", obj.Position, (obj.Size + 6f) * 2f, obj.Color);
                break;
            case 7: // Мелкие частицы
                _renderer.DrawRectangle(obj.Position, new Vector2(obj.Size * 0.4f), 0f, obj.Color);
                break;
            case 8: // Пятиугольники
                Span<Vector2> poly = stackalloc Vector2[5];
                for (int j = 0; j < 5; j++)
                {
                    float a = obj.Angle + j * 1.2566f;
                    poly[j] = obj.Position + new Vector2(MathF.Cos(a), MathF.Sin(a)) * obj.Size;
                }

                _renderer.DrawPolygon(poly, obj.Color);
                break;
            case 9: // Смешанный режим
                int mode = index % 8;
                if (mode == 0)
                    _renderer.DrawRectangle(obj.Position, new Vector2(obj.Size * 1.6f, obj.Size * 0.9f), obj.Angle,
                        obj.Color);
                else if (mode == 1)
                    _renderer.DrawCircle(obj.Position, obj.Size, obj.Color);
                else if (mode == 2)
                {
                    float r = obj.Size * 1.2f;
                    Vector2 p1 = obj.Position + new Vector2(MathF.Cos(obj.Angle), MathF.Sin(obj.Angle)) * r;
                    Vector2 p2 = obj.Position +
                                 new Vector2(MathF.Cos(obj.Angle + 2.0944f), MathF.Sin(obj.Angle + 2.0944f)) * r;
                    Vector2 p3 = obj.Position +
                                 new Vector2(MathF.Cos(obj.Angle + 4.1888f), MathF.Sin(obj.Angle + 4.1888f)) * r;
                    _renderer.DrawTriangle(p1, p2, p3, obj.Color);
                }
                else if (mode == 3)
                {
                    Vector2 d = new Vector2(MathF.Cos(obj.Angle), MathF.Sin(obj.Angle)) * (obj.Size * 2.5f);
                    _renderer.DrawLine(obj.Position - d, obj.Position + d, 1.5f, obj.Color);
                }
                else if (mode == 4)
                    _renderer.DrawEllipse(obj.Position, new Vector2(obj.Size * 1.5f, obj.Size * 0.7f), obj.Angle,
                        obj.Color);
                else if (mode == 5)
                    _renderer.DrawText("IGNIS", obj.Position, (obj.Size + 6f) * 2f, obj.Color);
                else if (mode == 6)
                    _renderer.DrawRectangle(obj.Position, new Vector2(obj.Size * 0.4f), 0f, obj.Color);
                else
                {
                    Span<Vector2> p5 = stackalloc Vector2[5];
                    for (int j = 0; j < 5; j++)
                    {
                        float a = obj.Angle + j * 1.2566f;
                        p5[j] = obj.Position + new Vector2(MathF.Cos(a), MathF.Sin(a)) * obj.Size;
                    }

                    _renderer.DrawPolygon(p5, obj.Color);
                }

                break;
            case 10: // Cyber Grid
                Vector2 center = new Vector2(_window.Size.X / 2f, _window.Size.Y / 2f);
                _renderer.DrawLine(center, obj.Position, 1f, obj.Color.WithAlpha(0.25f));
                _renderer.DrawCircle(obj.Position, 3.5f, obj.Color);
                break;
        }
    }

    private void RenderBenchmarkHUD(Vector2 size)
    {
        Vector2 panelSize = new Vector2(680f, 440f);
        Vector2 panelPos = new Vector2(size.X - panelSize.X / 2f - 20f, panelSize.Y / 2f + 20f);
        _renderer.DrawRectangle(panelPos, panelSize, 0f, new Color(0f, 0f, 0.05f, 0.85f));
        _renderer.DrawRectangle(new Vector2(panelPos.X, panelPos.Y - panelSize.Y / 2f), new Vector2(panelSize.X, 2f),
            0f, Color.Cyan);
        _renderer.DrawRectangle(new Vector2(panelPos.X, panelPos.Y + panelSize.Y / 2f), new Vector2(panelSize.X, 2f),
            0f, Color.Cyan);
        _renderer.DrawRectangle(new Vector2(panelPos.X - panelSize.X / 2f, panelPos.Y), new Vector2(2f, panelSize.Y),
            0f, Color.Cyan);
        _renderer.DrawRectangle(new Vector2(panelPos.X + panelSize.X / 2f, panelPos.Y), new Vector2(2f, panelSize.Y),
            0f, Color.Cyan);

        float startX = panelPos.X - panelSize.X / 2f + 30f;
        float startY = panelPos.Y - panelSize.Y / 2f + 50f;
        float stepY = 44f;

        _renderer.DrawText("IGNIS ENGINE BENCHMARK", new Vector2(startX, startY), 32f, Color.Cyan);
        startY += stepY + 10f;

        Color fpsColor = _currentFps >= 60 ? Color.Green : (_currentFps >= 30 ? Color.Yellow : Color.Red);
        _renderer.DrawText($"FPS: {_currentFps}", new Vector2(startX, startY), 36f, fpsColor);
        _renderer.DrawText($"Frame Time: {1000f / Math.Max(1, _currentFps):F2} ms", new Vector2(startX + 240f, startY),
            28f, Color.White);
        startY += stepY;

        string presetName = _benchmarkPreset switch
        {
            1 => "1 - Rectangles",
            2 => "2 - Circles",
            3 => "3 - Triangles",
            4 => "4 - Lines",
            5 => "5 - Ellipses",
            6 => "6 - Text Render",
            7 => "7 - Starfield Particles",
            8 => "8 - Polygons (Pentagons)",
            9 => "9 - Mixed Geometry",
            10 => "10 - Cyber Grid Nodes",
            _ => "Unknown"
        };
        _renderer.DrawText($"Preset: {presetName}", new Vector2(startX, startY), 28f, Color.Yellow);
        startY += stepY;

        _renderer.DrawText($"Objects Count: {_benchmarkObjectCount:N0}", new Vector2(startX, startY), 28f, Color.White);
        startY += stepY + 20f;

        _renderer.DrawLine(new Vector2(startX, startY), new Vector2(startX + panelSize.X - 60f, startY), 2f,
            new Color(0.3f, 0.3f, 0.3f));
        startY += stepY - 10f;

        _renderer.DrawText("CONTROLS:", new Vector2(startX, startY), 24f, Color.Cyan);
        startY += 30f;
        _renderer.DrawText("- [F1]: Switch to Game Mode", new Vector2(startX, startY), 22f, Color.White);
        startY += 30f;
        _renderer.DrawText("- [0-9]: Select Scene Preset", new Vector2(startX, startY), 22f, Color.White);
        startY += 30f;
        _renderer.DrawText("- [Up/Down]: Adjust Objects Count", new Vector2(startX, startY), 22f, Color.White);
        startY += 30f;
        _renderer.DrawText("- [Shift + Up/Down]: Fast Adjust", new Vector2(startX, startY), 22f, Color.White);
    }
}

internal sealed class BenchmarkObject
{
    internal Vector2 Position;
    internal Vector2 Velocity;
    internal float Size;
    internal Color Color;
    internal float Angle;
    internal float RotationSpeed;
    internal float PulsePhase;
    internal float PulseSpeed;
}

internal sealed class Star(Vector2 pos, float speed, float size, Color color)
{
    internal Vector2 Position = pos;
    internal float Speed = speed;
    internal float Size = size;
    internal Color Color = color;
}

internal sealed class Particle(Vector2 pos, Vector2 vel, Color color, float maxAge, float size)
{
    internal Vector2 Position = pos;
    internal Vector2 Velocity = vel;
    internal Color Color = color;
    internal float Age;
    internal float MaxAge = maxAge;
    internal float Size = size;
}

internal sealed class Laser(Vector2 pos, Vector2 vel, Color color)
{
    internal Vector2 Position = pos;
    internal Vector2 Velocity = vel;
    internal float Width = 4f;
    internal float Height = 16f;
    internal Color Color = color;
}

internal sealed class Asteroid
{
    internal Vector2 Position;
    internal Vector2 Velocity;
    internal float Radius;
    internal float MaxHp;
    internal float Hp;
    internal int ScoreValue;

    internal Asteroid(Vector2 pos, Vector2 vel, float radius)
    {
        Position = pos;
        Velocity = vel;
        Radius = radius;
        MaxHp = radius * 1.5f;
        Hp = MaxHp;
        ScoreValue = (int)(50f - radius);
        if (ScoreValue < 10) ScoreValue = 10;
    }
}

internal enum PowerUpType
{
    Health,
    TripleShot,
    Shield
}

internal sealed class PowerUp(Vector2 pos, Vector2 vel, PowerUpType type)
{
    internal Vector2 Position = pos;
    internal Vector2 Velocity = vel;
    internal float Radius = 12f;
    internal PowerUpType Type = type;

    internal Color Color => Type switch
    {
        PowerUpType.Health => Color.Green,
        PowerUpType.TripleShot => Color.Cyan,
        _ => Color.Yellow
    };
}

internal sealed class PlayerShip(Vector2 startPos)
{
    internal Vector2 Position = startPos;
    internal Vector2 Velocity;
    internal float Radius = 18f;

    internal float MaxHp = 100f;
    internal float Hp = 100f;
    internal float Shield;
    internal float MaxShield = 50f;

    internal float TripleShotTimer;
    internal float ShootCooldown;
    internal float MaxShootCooldown = 0.12f; // Быстрая стрельба

    internal bool IsDead => Hp <= 0f;

    internal void Update(float deltaTime, Keyboard keyboard, Vector2 size)
    {
        ArgumentNullException.ThrowIfNull(keyboard);

        if (IsDead) return;

        // Таймеры
        if (ShootCooldown > 0f) ShootCooldown -= deltaTime;
        if (TripleShotTimer > 0f) TripleShotTimer -= deltaTime;

        // Ввод направления движения
        Vector2 targetVelocity = Vector2.Zero;
        if (keyboard.IsKeyDown(Key.A) || keyboard.IsKeyDown(Key.Left)) targetVelocity.X = -1f;
        if (keyboard.IsKeyDown(Key.D) || keyboard.IsKeyDown(Key.Right)) targetVelocity.X = 1f;
        if (keyboard.IsKeyDown(Key.W) || keyboard.IsKeyDown(Key.Up)) targetVelocity.Y = -1f;
        if (keyboard.IsKeyDown(Key.S) || keyboard.IsKeyDown(Key.Down)) targetVelocity.Y = 1f;

        float maxSpeed = 380f;
        if (targetVelocity != Vector2.Zero)
        {
            targetVelocity = Vector2.Normalize(targetVelocity) * maxSpeed;
        }

        // Плавная интерполяция скорости для приятного управления
        Velocity = Vector2.Lerp(Velocity, targetVelocity, 12f * deltaTime);
        Position += Velocity * deltaTime;

        // Ограничения экрана
        Position.X = Math.Clamp(Position.X, Radius, size.X - Radius);
        Position.Y = Math.Clamp(Position.Y, size.Y * 0.35f, size.Y - Radius); // Летаем только в нижней 65% части
    }

    internal IReadOnlyList<Laser>? TryShoot(float deltaTime)
    {
        if (IsDead || ShootCooldown > 0f) return null;

        ShootCooldown = MaxShootCooldown;
        var lasers = new List<Laser>();

        if (TripleShotTimer > 0f)
        {
            // Три лазера веером
            lasers.Add(new Laser(Position + new Vector2(0f, -Radius), new Vector2(0f, -700f), Color.Cyan));
            lasers.Add(new Laser(Position + new Vector2(-10f, -Radius * 0.5f), new Vector2(-150f, -650f), Color.Cyan));
            lasers.Add(new Laser(Position + new Vector2(10f, -Radius * 0.5f), new Vector2(150f, -650f), Color.Cyan));
        }
        else
        {
            // Одиночный лазер
            lasers.Add(new Laser(Position + new Vector2(0f, -Radius), new Vector2(0f, -700f), Color.Yellow));
        }

        return lasers;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        if (Shield > 0f)
        {
            Shield -= amount;
            if (Shield < 0f)
            {
                Hp += Shield; // Переносим остаточный урон на здоровье
                Shield = 0f;
            }
        }
        else
        {
            Hp -= amount;
        }

        if (Hp < 0f) Hp = 0f;
    }

    internal void ApplyPowerUp(PowerUpType type)
    {
        if (IsDead) return;

        switch (type)
        {
            case PowerUpType.Health:
                Hp = Math.Min(MaxHp, Hp + 35f);
                break;
            case PowerUpType.TripleShot:
                TripleShotTimer = 8f; // 8 секунд тройного выстрела
                break;
            case PowerUpType.Shield:
                Shield = MaxShield;
                break;
        }
    }
}

internal static class SafeRandom
{
    public static double NextDouble()
    {
        return (double)System.Security.Cryptography.RandomNumberGenerator.GetInt32(0, int.MaxValue) / int.MaxValue;
    }

    public static int Next(int maxValue)
    {
        return System.Security.Cryptography.RandomNumberGenerator.GetInt32(0, maxValue);
    }
}
