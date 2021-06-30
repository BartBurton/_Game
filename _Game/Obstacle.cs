using System;
using SFML.Graphics;
using SFML.System;
using SFML.Audio;
using System.Numerics;

namespace _Game
{
    class Obstacle
    {
        //Просто земля
        static Texture textureG = new Texture(PlayGame.FileSprites, new IntRect(215, 10, 168, 56));
        static Sprite Ground;
        static Vector2f PosG;

        //Препядствия
        const byte OBSTACLE_W = 26;
        const byte OBSTACLE_H = 160;
        static Texture textureU = new Texture(PlayGame.FileSprites, new IntRect(152, 3, OBSTACLE_W, OBSTACLE_H));
        static Texture textureD = new Texture(PlayGame.FileSprites, new IntRect(180, 3, OBSTACLE_W, OBSTACLE_H));
        static Sprite[] Up = new Sprite[4];
        static Sprite[] Down = new Sprite[4];

        //Позиция предствия над игрком
        public static ushort PosCenterObs { get; private set; } = 0;
        //Кол-во препядствий прошедших над игроком
        public static BigInteger Count = 0;

        //Разъемы для прохождения препядствия
        static Vector2f Pos;
        static Random random = new Random();
        static byte RandTop = 10, RandDown = 35;

        //Звуки
        public static SoundBuffer PointBuffer = new SoundBuffer("point.wav");
        public static SoundBuffer FastPointBuffer = new SoundBuffer("Bonus.wav");
        public static Sound Point = new Sound();

        static Obstacle()
        {
            Ground = new Sprite(textureG);
            Ground.Scale = new Vector2f(((float)PlayGame.WIGTH / 168) * 2, 2);
            PosG.Y = PlayGame.HEIGHT - 50;
            Ground.Position = PosG;

            //Наложить текстуры на спрайты препядствий
            for (byte i = 0; i < Up.Length; i++)
            {
                Up[i] = new Sprite(textureU);
                Down[i] = new Sprite(textureD);
            }

            //Увеличить размер текстур в 2.5 раза
            Up[0].Scale = new Vector2f(2.5f, 2.5f);
            Down[0].Scale = Up[0].Scale;
            for (byte i = 1; i < Up.Length; i++)
            {
                Up[i].Scale = Up[0].Scale;
                Down[i].Scale = Up[0].Scale;
            }

            ObstacleClear();
        }

        //Рисуем препядствия и землю с движением
        public static void DrawObstacle()
        {
            for (byte i = 0; i < Up.Length; i++)
            {
                MovePillars(ref Up[i], 5);
                Down[i].Position = Pos;

                HeightPillars(ref Up[i]);
                //Спустить нижнее препядствие относительно верхнего
                Pos.Y += (OBSTACLE_H * 2.5f) + 190;
                Down[i].Position = Pos;

                if (!MainPlayer.Die) CenterObstacle(i, 2);

                PlayGame.Window.Draw(Up[i]);
                PlayGame.Window.Draw(Down[i]);
            }
            DrawGround();
        }

        public static void DrawOneOstacle()
        {
            for (int i = 0; i < 5; i++)
            {
                MovePillars(ref Up[0], 5);
                Down[0].Position = Pos;

                HeightPillars(ref Up[0]);
                Pos.Y += (OBSTACLE_H * 2.5f) + 190;
                Down[0].Position = Pos;

                if (!MainPlayer.Die) CenterObstacle(0, 4);
            }

            PlayGame.Window.Draw(Up[0]);
            PlayGame.Window.Draw(Down[0]);

            DrawGround();
        }

        //Рисуем только землю
        public static void DrawGround()
        {
            if (PosG.X == -PlayGame.WIGTH) PosG.X = 0;
            PosG.X -= 5;
            Ground.Position = PosG;
            PlayGame.Window.Draw(Ground);
        }

        //Рисуем без движения препядствий и земли
        public static void DrawNotMove()
        {
            for (byte i = 0; i < Up.Length; i++)
            {
                PlayGame.Window.Draw(Up[i]);
                PlayGame.Window.Draw(Down[i]);
            }
            PlayGame.Window.Draw(Ground);
        }

        public static void ObstacleClear()
        {
            Count = 0;
            PosCenterObs = 0;

            //Задать расстояние м/у препядствиями
            Pos.X = PlayGame.WIGTH + PlayGame.WIGTH / 2;
            Up[0].Position = Pos;
            Down[0].Position = Pos;
            for (byte i = 1; i < Down.Length; i++)
            {
                Pos = Up[i - 1].Position;
                Pos.X += PlayGame.WIGTH / 4 + 15;

                Up[i].Position = Pos;
                Down[i].Position = Pos;
            }

            //Задать область для прохождения препядствия
            for (byte i = 0; i < Down.Length; i++)
            {
                Pos.X = Up[i].Position.X;
                Pos.Y = -(random.Next(RandTop, RandDown) / 10f) * 100;
                Up[i].Position = Pos;
            }
        }

        //Сдвинуть препядствие
        static void MovePillars(ref Sprite Pillar, byte Step)
        {
            if (Pillar.Position.X > -OBSTACLE_W * 2.5f)
            {
                Pos = Pillar.Position;
                Pos.X -= Step;
                Pillar.Position = Pos;
            }
        }

        //Задать область для прохождения препядствия,
        //если препядствие за пределами видимости
        //Вернуть препядствие в начало пути
        static void HeightPillars(ref Sprite Pillar)
        {
            if (Pillar.Position.X <= -OBSTACLE_W * 2.5f)
            {
                Pos.X = PlayGame.WIGTH;
                Pos.Y = -(random.Next(RandTop, RandDown) / 10f) * 100;
                Pillar.Position = Pos;
            }
        }

        //Возвращать позицию препядствия, которое проходит мимо игрока
        private static void CenterObstacle(byte i, byte div)
        {
            if (Up[i].Position.X <= PlayGame.WIGTH / div && Up[i].Position.X > ((PlayGame.WIGTH / div) - ((OBSTACLE_W * 2.5f) + 50)))
            {
                PosCenterObs = (ushort)Down[i].Position.Y;
                return;
            }
            else if (Up[i].Position.X == ((PlayGame.WIGTH / div) - ((OBSTACLE_W * 2.5f) + 50)))
            {
                Count++;
                Point.Play();
                PosCenterObs = 0;
                return;
            }
        }
    }
}