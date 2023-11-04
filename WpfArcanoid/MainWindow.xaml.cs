using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Win32;

namespace WpfArcanoid;


public partial class MainWindow : Window
{

    public  double _ballX = 200; // Начальное положение мяча по X
    public  double _ballY = 260; // Начальное положение мяча по Y
    public  double _ballSpeedX = 3; // Скорость мяча по X
    public  double _ballSpeedY = 3; // Скорость мяча по Y
    public  double _platformX = 150; // Начальное положение платформы по X
    public  List<Rectangle> _blocks = new List<Rectangle>();
    public  Ellipse _ball;
    public  DispatcherTimer _gameTimer;


    public MainWindow()
    {
        InitializeComponent();
        InitializeGame();
        InitializeBlocks();
    }

    
    public  void GameTimer_Tick(object sender, EventArgs e)
    {
        // Проверка столкновения мяча с блоками
        foreach (var block in _blocks.Where(block => IsCollision(_ballX, _ballY, _ball.Width, _ball.Height, block)))
        {
            _ballSpeedY *= -1; // Изменить направление движения мяча при столкновении
            gameCanvas.Children.Remove(block); // Удалить блок из Canvas
            _blocks.Remove(block); // Удалить блок из списка блоков
            break; // Выход из цикла, чтобы не столкнуться с несколькими блоками одновременно
        }

        // Проверка столкновения мяча с платформой (игроком)
        if (IsCollision(_ballX, _ballY, _ball.Width, _ball.Height, gameCanvas.Children[1] as Rectangle))
        {
            _ballSpeedY *= -1; // Изменить направление движения мяча при столкновении с платформой
        }

        // Проверка столкновения мяча с нижней границей поля
        if (_ballY + _ball.Height >= gameCanvas.ActualHeight)
        {
            MessageBox.Show("Игрок проиграл!");
            gamePanel.Visibility = Visibility.Visible;
            _gameTimer.Stop();
            return;
        }

        // Обновление позиции мяча
        _ballX += _ballSpeedX;
        _ballY += _ballSpeedY;

        // Обработка столкновения мяча с границами игрового поля
        if (_ballX < 0 || _ballX > gameCanvas.ActualWidth - 20)
        {
            _ballSpeedX *= -1;
        }

        if (_ballY < 0 || _ballY > gameCanvas.ActualHeight - 20)
        {
            _ballSpeedY *= -1;
        }

        // Обновление позиции мяча на Canvas
        Canvas.SetLeft(gameCanvas.Children[0], _ballX);
        Canvas.SetTop(gameCanvas.Children[0], _ballY);
    }
    

    public  bool IsCollision(double x1, double y1, double width1, double height1, Rectangle block)
    {
        var x2 = Canvas.GetLeft(block);
        var y2 = Canvas.GetTop(block);
        var width2 = block.Width;
        var height2 = block.Height;

        if (x1 + width1 >= x2 && x1 <= x2 + width2 &&
            y1 + height1 >= y2 && y1 <= y2 + height2)
            return true; // Столкновение произошло

        return false; // Нет столкновения
    }

    public  void InitializeGame()
    {
        // Создание мяча
        _ball = new Ellipse
        {
            Width = 20,
            Height = 20,
            Fill = Brushes.White
        };
        Canvas.SetLeft(_ball, _ballX);
        Canvas.SetTop(_ball, _ballY);
        gameCanvas.Children.Add(_ball);
        // Создание платформы
        var platform = new Rectangle
        {
            Width = 100,
            Height = 10,
            Fill = Brushes.Blue
        };
        Canvas.SetLeft(platform, _platformX);
        Canvas.SetTop(platform, 300);
        gameCanvas.Children.Add(platform);

        // Обработка события нажатия клавиш
        KeyDown += MainWindow_KeyDown;

        // Создание таймера для обновления игры
        _gameTimer = new DispatcherTimer();
        _gameTimer.Tick += GameTimer_Tick;
        _gameTimer.Interval = TimeSpan.FromMilliseconds(10);
        _gameTimer.Start();
        gamePanel.Visibility = Visibility.Hidden;
    }

    public  void InitializeBlocks()
    {
        var numRows = 5; // Количество рядов блоков
        const int numCols = 10; // Количество блоков в каждом ряду
        double blockWidth = 70;
        double blockHeight = 20;
        double spacing = 10; // Расстояние между блоками

        for (var row = 0; row < numRows; row++)
        {
            for (var col = 0; col < numCols; col++)
            {
                var block = new Rectangle
                {
                    Width = blockWidth,
                    Height = blockHeight,
                    Fill = Brushes.Green
                };

                var blockX = col * (blockWidth + spacing);
                var blockY = row * (blockHeight + spacing);

                Canvas.SetLeft(block, blockX);
                Canvas.SetTop(block, blockY);
                gameCanvas.Children.Add(block);
                _blocks.Add(block);

            }
        }
    }

    public  void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {

        switch (e.Key)
        {
            // Обработка нажатия клавиш для перемещения платформы
            case Key.Left when _platformX > 0:
                _platformX -= 20;
                break;
            case Key.Right when _platformX < gameCanvas.ActualWidth - ((Rectangle)gameCanvas.Children[1]).Width:
                _platformX += 20;
                break;
            case Key.Escape:
                _gameTimer.Stop();
                gamePanel.Visibility = Visibility.Visible;
                break;
            case Key.LeftShift:
                gamePanel.Visibility = Visibility.Hidden;
                _gameTimer.Start();
                break;

        }

        // Обновление позиции платформы на Canvas
        Canvas.SetLeft(gameCanvas.Children[1], _platformX);
    }

    public  void ResetGame()
    {
        // Сброс позиции мяча и платформы
        _ballX = 200;
        _ballY = 260;
        _platformX = 150;

        // Удаление блоков с игрового поля
        foreach (var block in _blocks)
        {
            gameCanvas.Children.Remove(block);
        }
        _blocks.Clear();

        // Создание новых блоков
        InitializeBlocks();
        _gameTimer.Start();

    }
    public  void RestartButton_Click(object sender, RoutedEventArgs e)
    {
        ResetGame();
        gamePanel.Visibility = Visibility.Hidden;
    }
    public  void ExitButton_Click(object sender, RoutedEventArgs e)
    {

        Application.Current.Shutdown();
    }

    public  void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        SaveFileDialog saveDialog = new SaveFileDialog();
        saveDialog.Filter = "JSON Files|*.json";
        if (saveDialog.ShowDialog() == true)
        {
            SaveGameState(saveDialog.FileName);
        }
    }
    public  void LoadButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openDialog = new OpenFileDialog();
        openDialog.Filter = "JSON Files|*.json";
        if (openDialog.ShowDialog() == true)
        {
            LoadGameState(openDialog.FileName);
        }
    }
    public  void SaveGameState(string filePath)
    {
        var gameState = new GameState
        {
            BallX = _ballX,
            BallY = _ballY,
            BallSpeedX = _ballSpeedX,
            BallSpeedY = _ballSpeedY,
            PlatformX = _platformX,
            Blocks = _blocks.Select(block => new BlockState
            {
                Left = Canvas.GetLeft(block),
                Top = Canvas.GetTop(block)
            }).ToList()
        };

        var json = JsonConvert.SerializeObject(gameState);
        File.WriteAllText(filePath, json);
    }

    public  void LoadGameState(string filePath)
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            var gameState = JsonConvert.DeserializeObject<GameState>(json);

            _ballX = gameState.BallX;
            _ballY = gameState.BallY;
            _ballSpeedX = gameState.BallSpeedX;
            _ballSpeedY = gameState.BallSpeedY;
            _platformX = gameState.PlatformX;

            foreach (var block in _blocks)
            {
                gameCanvas.Children.Remove(block);
            }
            _blocks.Clear();

            foreach (var blockState in gameState.Blocks)
            {
                var block = new Rectangle
                {
                    Width = 70,
                    Height = 20,
                    Fill = Brushes.Green
                };

                Canvas.SetLeft(block, blockState.Left);
                Canvas.SetTop(block, blockState.Top);

                gameCanvas.Children.Add(block);
                _blocks.Add(block);
            }
        }
    }
}

[Serializable]
public class GameState
{
    public double BallX { get; set; }
    public double BallY { get; set; }
    public double BallSpeedX { get; set; }
    public double BallSpeedY { get; set; }
    public double PlatformX { get; set; }
    public List<BlockState> Blocks { get; set; }
}

[Serializable]
public class BlockState
{
    public double Left { get; set; }
    public double Top { get; set; }
}