using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Shapes;
using WpfArcanoid;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    
    public class MainWindowTests 
    {
        
        [TestMethod]
        public void TestIsCollision()
        {            
            MainWindow mainWindow = new MainWindow();
            mainWindow.InitializeBlocks();
            Rectangle block = mainWindow._blocks[0];          
            double ballX = Canvas.GetLeft(block) + 10; 
            double ballY = Canvas.GetTop(block) + 10;
            double ballWidth = 20;
            double ballHeight = 20;          
            bool collision = mainWindow.IsCollision(ballX, ballY, ballWidth, ballHeight, block);
            // Проверяем, что столкновение действительно произошло
            Assert.IsTrue(collision);
        }

        [TestMethod]
        public void TestInitializeBlocks()
        {           
            MainWindow mainWindow = new MainWindow();
            mainWindow.InitializeBlocks();
            // Проверяем, что в списке _blocks содержится 100 элементов
            Assert.AreEqual(100, mainWindow._blocks.Count); 
            foreach (var block in mainWindow._blocks)
            {
                // Проверяем, что каждый блок имеет ширину 70 и длину 20
                Assert.AreEqual(70, block.Width); 
                Assert.AreEqual(20, block.Height); 
            }
        }

        [TestMethod]
        public void TestInitializeGame()
        {
            var mainWindow = new MainWindow();
            mainWindow.InitializeGame();

            // Проверка, что мяч и платформа созданы
            Assert.IsNotNull(mainWindow._ball, "Ball should be created");
            Assert.AreEqual(150, mainWindow._platformX, "Initial platform position should be 150");
        }
        [TestMethod]
        public void TestResetGame()
        {
            var mainWindow = new MainWindow();
            mainWindow.InitializeGame();
            mainWindow.ResetGame();

            // Проверка сброса позиции мяча и платформы
            Assert.AreEqual(200, mainWindow._ballX, "BallX should be reset to 200");
            Assert.AreEqual(260, mainWindow._ballY, "BallY should be reset to 260");
            Assert.AreEqual(150, mainWindow._platformX, "PlatformX should be reset to 150");
        }
        [TestMethod]
        public void TestLoadGameState()
        {
            var mainWindow = new MainWindow();           

            // Загрузка состояния игры
            var gameState = new GameState
            {
                BallX = 200,
                BallY = 260,
                BallSpeedX = 3,
                BallSpeedY = 3,
                PlatformX = 150,
                Blocks = new List<BlockState>
                {
                    new BlockState { Left = 10, Top = 10 },
                    new BlockState { Left = 50, Top = 50 },
                     // Добавьте блоки по вашему усмотрению
                }
            };

            var json = JsonConvert.SerializeObject(gameState);
            File.WriteAllText("testLoadGameState.json", json);

            // Загружаем состояние из файла
            mainWindow.LoadGameState("testLoadGameState.json");

            // Проверяем, что состояние игры в MainWindow соответствует загруженному состоянию
            Assert.AreEqual(200, mainWindow._ballX, "BallX should match loaded value");
            Assert.AreEqual(260, mainWindow._ballY, "BallY should match loaded value");
            Assert.AreEqual(3, mainWindow._ballSpeedX, "BallSpeedX should match loaded value");
            Assert.AreEqual(3, mainWindow._ballSpeedY, "BallSpeedY should match loaded value");
            Assert.AreEqual(150, mainWindow._platformX, "PlatformX should match loaded value");

            // Проверяем количество загруженных блоков
            Assert.AreEqual(2, mainWindow._blocks.Count, "Number of loaded blocks should match");
        }
        [TestMethod]
        public void TestSaveGameState()
        {
            var mainWindow = new MainWindow();
            

            // Сохранение состояния игры
            mainWindow.SaveGameState("wwe.json");

            // Проверяем, что JSON-файл существует
            Assert.IsTrue(File.Exists("wwe.json"), "JSON file should be created");

            
        }
           
    }
}
